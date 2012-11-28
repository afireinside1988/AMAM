using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmAddProduct.xaml
	/// </summary>
	public partial class FrmAddProduct : Window
	{

		DataTable ProductDataTable = new DataTable();


		public FrmAddProduct()
		{
			InitializeComponent();

			ProductDataTable.Columns.Add(new DataColumn("VertriebID"));
			ProductDataTable.Columns.Add(new DataColumn("Artikelnummer"));
			ProductDataTable.Columns.Add(new DataColumn("Preis"));
			ProductDataTable.Columns.Add(new DataColumn("Verpackungseinheit"));

			dgProductData.DataContext = ProductDataTable.DefaultView;
			ProductDataTable.PrimaryKey = new DataColumn[] {ProductDataTable.Columns["Artikelnummer"]};
			dgProductData.SelectedValuePath = "Artikelnummer";

			ReadDealers();

			#region Tabellenexistenz sicherstellen

			SqlConnectionStringBuilder ConnString = new SqlConnectionStringBuilder();
			ConnString.DataSource = "localhost";
			ConnString.InitialCatalog = "AMAM";
			ConnString.IntegratedSecurity = true;

			using(SqlConnection sqlConn = new SqlConnection(ConnString.ToString()))
			{
				try
				{
					sqlConn.Open();

					if(!sqlhelper.TableExists(sqlConn, "ProductNames"))
					{
						SqlCommand newTable = new SqlCommand("CREATE TABLE ProductNames (Produktnummer int IDENTITY(1,1) PRIMARY KEY, " + 
																"Produktname nvarchar(255) NOT NULL)", sqlConn);
						newTable.ExecuteNonQuery();
					}
					if(!sqlhelper.TableExists(sqlConn, "Units"))
					{
						SqlCommand newTable = new SqlCommand("CREATE TABLE Units (EinheitID int IDENTITY(1,1) CONSTRAINT pkEinheitID PRIMARY KEY, " +
																"Einheit nvarchar(128) NOT NULL)", sqlConn);
						newTable.ExecuteNonQuery();

						SqlCommand newRow = new SqlCommand("INSERT INTO Units (Einheit) VALUES(@paramEinheit1), (@paramEinheit2), (@paramEinheit3)", sqlConn);
						newRow.Parameters.Add(new SqlParameter("@paramEinheit1", "Stück"));
						newRow.Parameters.Add(new SqlParameter("@paramEinheit2", "Packung"));
						newRow.Parameters.Add(new SqlParameter("@paramEinheit3", "Originalpackung"));
						
						newRow.ExecuteNonQuery();
					}

					if(!sqlhelper.TableExists(sqlConn, "ProductData"))
					{
						SqlCommand newTable = new SqlCommand("CREATE TABLE ProductData (ProduktID int IDENTITY(1,1) CONSTRAINT pkProduktID PRIMARY KEY, " +
																"Produktnummer int NOT NULL CONSTRAINT fkProduktnummer FOREIGN KEY REFERENCES ProductNames(Produktnummer), " +
																"Artikelnummer nvarchar(255) NOT NULL, " +
																"VertriebID int NOT NULL CONSTRAINT fkVertriebID FOREIGN KEY REFERENCES Dealers(VertriebID), " +
																"Preis nvarchar(128) NOT NULL, " +
																"EinheitID int NOT NULL CONSTRAINT fkEinheitID FOREIGN KEY REFERENCES Units(EinheitID))", sqlConn);
						newTable.ExecuteNonQuery();
					}

					
				}
				catch(SqlException ex)
				{
					ExceptionReporter Reporter = new ExceptionReporter(ex);
					Reporter.ReportExceptionToAdmin();
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

			#endregion

			ReadUnits();
		}

		private void ReadDealers()
		{
			
			cboDealers.Items.Clear();

			SqlConnectionStringBuilder ConnString = new SqlConnectionStringBuilder();
			ConnString.DataSource = "localhost";
			ConnString.InitialCatalog = "AMAM";
			ConnString.IntegratedSecurity = true;

			using(SqlConnection sqlConn = new SqlConnection(ConnString.ToString()))
			{
				try
				{
					sqlConn.Open();

					if(sqlhelper.TableExists(sqlConn, "Dealers"))
					{
						DataTable DealerTable = new DataTable();
						SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT VertriebID, Vertrieb FROM Dealers", sqlConn);
						SqlCommandBuilder sqlCommand = new SqlCommandBuilder(dataAdapter);
						DealerTable.Clear();
						dataAdapter.Fill(DealerTable);

						cboDealers.ItemsSource = DealerTable.DefaultView;
						cboDealers.DisplayMemberPath = "Vertrieb";
						cboDealers.SelectedValuePath = "VertriebID";
						cboDealers.SelectedIndex = 0;

						if(cboDealers.Items.Count == 0)
						{
							MessageBoxResult Answer = MessageBox.Show("Es wurden keine Vertriebe gefunden. Möchten Sie jetzt einen neuen Vertrieb hinzufügen?", "Fehler", MessageBoxButton.YesNo, MessageBoxImage.Question);
							if(Answer == MessageBoxResult.Yes)
							{
								FrmDealersList DealerManager = new FrmDealersList();
								DealerManager.ShowDialog();
								ReadDealers();
							}
						}
					}
					else
					{
						MessageBoxResult Answer = MessageBox.Show("Es wurden keine Vertriebe gefunden. Möchten Sie jetzt einen neuen Vertrieb hinzufügen?", "Fehler", MessageBoxButton.YesNo, MessageBoxImage.Question);
						if(Answer == MessageBoxResult.Yes)
						{
							FrmDealersList DealerManager = new FrmDealersList();
							DealerManager.ShowDialog();
							ReadDealers();
						}
					}
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

		private void ReadUnits()
		{

			cboPackageMass.Items.Clear();

			SqlConnectionStringBuilder ConnString = new SqlConnectionStringBuilder();
			ConnString.DataSource = "localhost";
			ConnString.InitialCatalog = "AMAM";
			ConnString.IntegratedSecurity = true;

			using(SqlConnection sqlConn = new SqlConnection(ConnString.ToString()))
			{
				try
				{
					sqlConn.Open();

					if(sqlhelper.TableExists(sqlConn, "Units"))
					{
						DataTable DealerTable = new DataTable();
						SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT EinheitID, Einheit FROM Units", sqlConn);
						SqlCommandBuilder sqlCommand = new SqlCommandBuilder(dataAdapter);
						DealerTable.Clear();
						dataAdapter.Fill(DealerTable);

						cboPackageMass.ItemsSource = DealerTable.DefaultView;
						cboPackageMass.DisplayMemberPath = "Einheit";
						cboPackageMass.SelectedValuePath = "Einheit";
						cboPackageMass.SelectedIndex = 0;

						if(cboDealers.Items.Count == 0)
						{
							MessageBoxResult Answer = MessageBox.Show("Es wurden keine Verpackungseinheiten gefunden. Möchten Sie jetzt eine neue Verpackungseinheit hinzufügen?", "Fehler", MessageBoxButton.YesNo, MessageBoxImage.Question);
							if(Answer == MessageBoxResult.Yes)
							{
								FrmDealersList DealerManager = new FrmDealersList();
								DealerManager.ShowDialog();
								ReadDealers();
							}
						}
					}
					else
					{
						MessageBoxResult Answer = MessageBox.Show("Es wurden keine Verpackungseinheiten gefunden. Möchten Sie jetzt eine neue Verpackungseinheit hinzufügen?", "Fehler", MessageBoxButton.YesNo, MessageBoxImage.Question);
						if(Answer == MessageBoxResult.Yes)
						{
							FrmDealersList DealerManager = new FrmDealersList();
							DealerManager.ShowDialog();
							ReadDealers();
						}
					}
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

		private void AddProductNameToDataBase()
		{
			tbProductName.Background = Brushes.White;
			tbProductName.Foreground = Brushes.Black;

			if(tbProductName.Text.Length > 0)
			{
				SqlConnectionStringBuilder ConnString = new SqlConnectionStringBuilder();
				ConnString.DataSource = "localhost";
				ConnString.InitialCatalog = "AMAM";
				ConnString.IntegratedSecurity = true;

				using(SqlConnection sqlConn = new SqlConnection(ConnString.ToString()))
				{
					try
					{
						sqlConn.Open();

						SqlCommand newRow = new SqlCommand("INSERT INTO ProductNames (Produktname) VALUES(@paramName)", sqlConn);
						newRow.Parameters.Add(new SqlParameter("@paramName", tbProductName.Text));
						newRow.ExecuteNonQuery();
					}
					catch(SqlException ex)
					{
						ExceptionReporter Reporter = new ExceptionReporter(ex);
						Reporter.ReportExceptionToAdmin();
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
			else
			{
				MessageBox.Show("Sie müssen einen Produktnamen eingeben.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
				tbProductName.Background = Brushes.Red;
				tbProductName.Foreground = Brushes.Black;
			}
		}

		private void AddProductDataToDataBase()
		{
			SqlConnectionStringBuilder ConnString = new SqlConnectionStringBuilder();
			ConnString.DataSource = "localhost";
			ConnString.InitialCatalog = "AMAM";
			ConnString.IntegratedSecurity = true;

			using(SqlConnection sqlConn = new SqlConnection(ConnString.ToString()))
			{
				try
				{
					sqlConn.Open();

					SqlCommand newRow = new SqlCommand("INSERT INTO ProductData (Produktnummer, Artikelnummer, VertriebID, Preis, EinheitID) " +
														"VALUES((SELECT Produktnummer FROM ProductNames WHERE ProductNames.Produktname = @paramName), " +
														"@paramArtikelnummer, " + 
														"(SELECT VertriebID FROM Dealers WHERE Dealers.VertriebID = @paramVertriebID), " + 
														"@paramPreis, "+
														"(SELECT EinheitID FROM Units WHERE Units.Einheit = @paramEinheit))", sqlConn);

					foreach(DataRow row in ProductDataTable.Rows)
					{
						newRow.Parameters.Add(new SqlParameter("@paramName", tbProductName.Text));
						newRow.Parameters.Add(new SqlParameter("@paramArtikelnummer", row["Artikelnummer"]));
						newRow.Parameters.Add(new SqlParameter("@paramVertriebID", row["VertriebID"]));
						newRow.Parameters.Add(new SqlParameter("@paramPreis", row["Preis"]));
						newRow.Parameters.Add(new SqlParameter("@paramEinheit", row["Verpackungseinheit"]));

						newRow.ExecuteNonQuery();
						newRow.Parameters.Clear();
					}
				}
				catch(SqlException ex)
				{
					ExceptionReporter Reporter = new ExceptionReporter(ex);
					Reporter.ReportExceptionToAdmin();
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

		private void AddDataSet(object sender, RoutedEventArgs e)
		{
			if(tbArticelNumber.Text.Length > 0)
			{
				try
				{
					tbPrice.Text = Validator.FormatPrice(tbPrice.Text);

					try
					{
						DataRow newDataRow = ProductDataTable.NewRow();
						newDataRow["VertriebID"] = cboDealers.SelectedValue;
						newDataRow["Artikelnummer"] = tbArticelNumber.Text;
						newDataRow["Preis"] = tbPrice.Text;
						newDataRow["Verpackungseinheit"] = cboPackageMass.SelectedValue;

						ProductDataTable.Rows.Add(newDataRow);
					}
					catch(ConstraintException)
					{
						MessageBox.Show("Ein Datensatz für die Artikelnummer " + tbArticelNumber.Text + " existiert bereits.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
				catch(ArgumentException ex)
				{
					MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void Close(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void SaveProduct(object sender, RoutedEventArgs e)
		{
			AddProductNameToDataBase();
			AddProductDataToDataBase();
			this.Close();
		}

	}
}
