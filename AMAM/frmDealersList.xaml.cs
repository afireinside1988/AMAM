using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmDealersList.xaml
	/// </summary>
	public partial class FrmDealersList : Window
	{
		DataTable dt = new DataTable();

		public FrmDealersList()
		{
			InitializeComponent();
			SearchForDataBase();
			RefreshDataBase();
		}

		private void SearchForDataBase()
		{

			SqlConnectionStringBuilder ConnStringBuilder = new SqlConnectionStringBuilder();
			ConnStringBuilder.DataSource = "localhost";
			ConnStringBuilder.IntegratedSecurity = true;

			using(SqlConnection sqlConn = new SqlConnection(ConnStringBuilder.ToString()))
			{
				try
				{
					sqlConn.Open();
					DataTable tblDatabases = sqlConn.GetSchema("Databases");
					sqlConn.Close();

					tblDatabases.PrimaryKey = new DataColumn[] {tblDatabases.Columns["database_name"]};
					if(tblDatabases.Rows.Contains("AMAM"))
					{
						btnAddDealer.IsEnabled = true;
					}
					else
					{
						try
						{
							sqlConn.Open();
                            SqlCommand newDataBaseCommand = new SqlCommand("CREATE DATABASE AMAM", sqlConn);
							newDataBaseCommand.ExecuteNonQuery();
							tblDatabases = sqlConn.GetSchema("Databases");

							tblDatabases.PrimaryKey = new DataColumn[] { tblDatabases.Columns["database_name"] };
							if(tblDatabases.Rows.Contains("AMAM"))
							{
								btnAddDealer.IsEnabled = true;
							}
						}
						catch(SqlException ex)
						{
							ExceptionReporter Reporter = new ExceptionReporter(ex);
							Reporter.ReportExceptionToAdmin();
							MessageBox.Show("Auf den SQL-Server konnte nicht zugegriffen werden. Ein Fehlerbericht wurde an den Administrator gesendet.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
							return;
						}
						finally
						{
							if(sqlConn.State == ConnectionState.Open)
							{
								sqlConn.Close();
							}
						}
					}
				}
				catch(SqlException ex)
				{
					ExceptionReporter Reporter = new ExceptionReporter(ex);
					Reporter.ReportExceptionToAdmin();
					MessageBox.Show("Auf den SQL-Server konnte nicht zugegriffen werden. Ein Fehlerbericht wurde an den Administrator gesendet.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
			}
		}

		private void RefreshDataBase()
		{
            dt.Clear();
			SqlConnectionStringBuilder ConnStringBuilder = new SqlConnectionStringBuilder();
			ConnStringBuilder.DataSource = "localhost";
			ConnStringBuilder.IntegratedSecurity = true;
			ConnStringBuilder.InitialCatalog = "AMAM";

			using(SqlConnection sqlConn = new SqlConnection(ConnStringBuilder.ToString()))
			{
				try
				{
					sqlConn.Open();
					if(sqlhelper.TableExists(sqlConn, "Dealers"))
					{
						SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Dealers", sqlConn);
						SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(dataAdapter);
						dataAdapter.Fill(dt);
					}
					else
					{
						SqlCommand newTable = new SqlCommand("CREATE TABLE Dealers (VertriebID int IDENTITY(1,1 )CONSTRAINT pkVertriebID PRIMARY KEY, "+
																"Vertrieb nvarchar(255) NOT NULL, "+ 
																"eMail nvarchar(255) NOT NULL, " +
																"Kundennummer nvarchar(255) NOT NULL)", sqlConn);
						newTable.ExecuteNonQuery();

						SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Dealers;", sqlConn);
						SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(dataAdapter);
						dataAdapter.Fill(dt);
					}
					dt.PrimaryKey = new DataColumn[] { dt.Columns["pkVertriebID"] };
					dgDealers.DataContext = dt;
                    dgDealers.SelectedValuePath = "VertriebID";
				}
				catch(SqlException ex)
				{
					ExceptionReporter Reporter = new ExceptionReporter(ex);
					Reporter.ReportExceptionToAdmin();
					MessageBox.Show("Auf die Datenbank konnte nicht zugegriffen werden. Ein Fehlerbericht wurde an den Administrator gesendet.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				finally
				{
					if(sqlConn.State == ConnectionState.Open)
					{
						sqlConn.Close();
					}
				}
			}
		}

		private void Close(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void AddDealer(object sender, RoutedEventArgs e)
		{
			FrmAddDealer AddDealer = new FrmAddDealer();
			AddDealer.ShowDialog();
			RefreshDataBase();
		}

        private void RemoveDealer(object sender, RoutedEventArgs e)
        {
            MessageBoxResult ExecuteRemove = MessageBox.Show("Sind Sie sicher, dass Sie den Vertrieb " + dgDealers.SelectedValue.ToString() + " löschen möchten?", "Vertrieb löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(ExecuteRemove == MessageBoxResult.Yes)
            {
                SqlConnectionStringBuilder ConnStringBuilder = new SqlConnectionStringBuilder();
                ConnStringBuilder.DataSource = "localhost";
                ConnStringBuilder.IntegratedSecurity = true;
                ConnStringBuilder.InitialCatalog = "AMAM";

                using(SqlConnection sqlConn = new SqlConnection(ConnStringBuilder.ToString()))
                {
                    try
                    {
                        sqlConn.Open();

                        SqlCommand RemoveDealerCommand = new SqlCommand("DELETE FROM Dealers WHERE VertriebID = @paramPK", sqlConn);
                        RemoveDealerCommand.Parameters.Add(new SqlParameter("@paramPK", dgDealers.SelectedValue));

                        RemoveDealerCommand.ExecuteNonQuery();
                        RefreshDataBase();
                    }
                    catch(SqlException ex)
                    {
                        ExceptionReporter Reporter = new ExceptionReporter(ex);
                        Reporter.ReportExceptionToAdmin();
                        MessageBox.Show("Auf die Datenbank konnte nicht zugegriffen werden. Ein Fehlerbericht wurde an den Administrator gesendet.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    finally
                    {
                        if(sqlConn.State == ConnectionState.Open)
                        {
                            sqlConn.Close();
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void DataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dgDealers.Items.Count > 0)
            {
                btnChangeDealer.IsEnabled = true;
                btnDeleteDealer.IsEnabled = true;
            }
            else
            {
                btnDeleteDealer.IsEnabled = false;
                btnChangeDealer.IsEnabled = false;
            }
        }

        private void ChangeDealer(object sender, RoutedEventArgs e)
        {
            FrmChangeDealer ChangeDealerForm = new FrmChangeDealer(dgDealers.SelectedValue.ToString());
            ChangeDealerForm.ShowDialog();
            RefreshDataBase();
        }
	}
}
