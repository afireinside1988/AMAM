using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmDealersList.xaml
	/// </summary>
	public partial class FrmDealersList
	{
	    readonly DataTable _dt = new DataTable();

		public FrmDealersList()
		{
			InitializeComponent();
			SearchForDataBase();
			RefreshDataBase();
		}

		private void SearchForDataBase()
		{

			var connStringBuilder = new SqlConnectionStringBuilder {DataSource = "localhost", IntegratedSecurity = true};

		    using(var sqlConn = new SqlConnection(connStringBuilder.ToString()))
			{
				try
				{
					sqlConn.Open();
					DataTable tblDatabases = sqlConn.GetSchema("Databases");
					sqlConn.Close();

					tblDatabases.PrimaryKey = new[] {tblDatabases.Columns["database_name"]};
					if(tblDatabases.Rows.Contains("AMAM"))
					{
						btnAddDealer.IsEnabled = true;
					}
					else
					{
						try
						{
							sqlConn.Open();
                            var newDataBaseCommand = new SqlCommand("CREATE DATABASE AMAM", sqlConn);
							newDataBaseCommand.ExecuteNonQuery();
							tblDatabases = sqlConn.GetSchema("Databases");

							tblDatabases.PrimaryKey = new[] { tblDatabases.Columns["database_name"] };
							if(tblDatabases.Rows.Contains("AMAM"))
							{
								btnAddDealer.IsEnabled = true;
							}
						}
						catch(SqlException ex)
						{
							var reporter = new ExceptionReporter(ex);
							reporter.ReportExceptionToAdmin();
							MessageBox.Show("Auf den SQL-Server konnte nicht zugegriffen werden. Ein Fehlerbericht wurde an den Administrator gesendet.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
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
					var reporter = new ExceptionReporter(ex);
					reporter.ReportExceptionToAdmin();
					MessageBox.Show("Auf den SQL-Server konnte nicht zugegriffen werden. Ein Fehlerbericht wurde an den Administrator gesendet.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void RefreshDataBase()
		{
            _dt.Clear();
			var connStringBuilder = new SqlConnectionStringBuilder
			    {
			        DataSource = "localhost",
			        IntegratedSecurity = true,
			        InitialCatalog = "AMAM"
			    };

		    using(var sqlConn = new SqlConnection(connStringBuilder.ToString()))
			{
				try
				{
					sqlConn.Open();
					if(Sqlhelper.TableExists(sqlConn, "Dealers"))
					{
						var dataAdapter = new SqlDataAdapter("SELECT * FROM Dealers", sqlConn);
						dataAdapter.Fill(_dt);
					}
					else
					{
						var newTable = new SqlCommand("CREATE TABLE Dealers (VertriebID int IDENTITY(1,1 )CONSTRAINT pkVertriebID PRIMARY KEY, "+
														"Vertrieb nvarchar(255) NOT NULL, "+ 
														"eMail nvarchar(255) NOT NULL, " +
														"Kundennummer nvarchar(255) NOT NULL)", sqlConn);
						newTable.ExecuteNonQuery();

						var dataAdapter = new SqlDataAdapter("SELECT * FROM Dealers;", sqlConn);
						dataAdapter.Fill(_dt);
					}
					_dt.PrimaryKey = new[] { _dt.Columns["pkVertriebID"] };
					dgDealers.DataContext = _dt;
                    dgDealers.SelectedValuePath = "VertriebID";
				}
				catch(SqlException ex)
				{
					var reporter = new ExceptionReporter(ex);
					reporter.ReportExceptionToAdmin();
					MessageBox.Show("Auf die Datenbank konnte nicht zugegriffen werden. Ein Fehlerbericht wurde an den Administrator gesendet.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
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
			Close();
		}

		private void AddDealer(object sender, RoutedEventArgs e)
		{
			var addDealer = new FrmAddDealer();
			addDealer.ShowDialog();
			RefreshDataBase();
		}

        private void RemoveDealer(object sender, RoutedEventArgs e)
        {
            MessageBoxResult executeRemove = MessageBox.Show("Sind Sie sicher, dass der Vertrieb " + dgDealers.SelectedValue + " gelöscht werden soll?", "Vertrieb löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(executeRemove == MessageBoxResult.Yes)
            {
                var connStringBuilder = new SqlConnectionStringBuilder
                    {
                        DataSource = "localhost",
                        IntegratedSecurity = true,
                        InitialCatalog = "AMAM"
                    };

                using(var sqlConn = new SqlConnection(connStringBuilder.ToString()))
                {
                    try
                    {
                        sqlConn.Open();

                        var removeDealerCommand = new SqlCommand("DELETE FROM Dealers WHERE VertriebID = @paramPK", sqlConn);
                        removeDealerCommand.Parameters.Add(new SqlParameter("@paramPK", dgDealers.SelectedValue));

                        removeDealerCommand.ExecuteNonQuery();
                        RefreshDataBase();
                    }
                    catch(SqlException ex)
                    {
                        var reporter = new ExceptionReporter(ex);
                        reporter.ReportExceptionToAdmin();
                        MessageBox.Show("Auf die Datenbank konnte nicht zugegriffen werden. Ein Fehlerbericht wurde an den Administrator gesendet.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
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
            var changeDealerForm = new FrmChangeDealer(dgDealers.SelectedValue.ToString());
            changeDealerForm.ShowDialog();
            RefreshDataBase();
        }
	}
}
