using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Media;

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmAddProduct.xaml
	/// </summary>
	public partial class FrmAddProduct
	{
	    readonly DataTable _productDataTable = new DataTable();

		public FrmAddProduct()
		{
			InitializeComponent();

			_productDataTable.Columns.Add(new DataColumn("VertriebID"));
			_productDataTable.Columns.Add(new DataColumn("Artikelnummer"));
			_productDataTable.Columns.Add(new DataColumn("Preis"));
			_productDataTable.Columns.Add(new DataColumn("Verpackungseinheit"));

			DgProductData.DataContext = _productDataTable.DefaultView;
			_productDataTable.PrimaryKey = new[] {_productDataTable.Columns["Artikelnummer"]};
			DgProductData.SelectedValuePath = "Artikelnummer";

			ReadDealers();

			#region Tabellenexistenz sicherstellen

			var connString = new SqlConnectionStringBuilder
				{
					DataSource = "localhost",
					InitialCatalog = "AMAM",
					IntegratedSecurity = true
				};

			using (var sqlConn = new SqlConnection(connString.ToString()))
			{
				try
				{
					sqlConn.Open();

					if (!Sqlhelper.TableExists(sqlConn, "ProductNames"))
					{
						var newTable = new SqlCommand("CREATE TABLE ProductNames (Produktnummer int IDENTITY(1,1) PRIMARY KEY, " +
						                              "Produktname nvarchar(255) NOT NULL)", sqlConn);
						newTable.ExecuteNonQuery();
					}
					if (!Sqlhelper.TableExists(sqlConn, "Units"))
					{
						var newTable =
							new SqlCommand("CREATE TABLE Units (EinheitID int IDENTITY(1,1) CONSTRAINT pkEinheitID PRIMARY KEY, " +
							               "Einheit nvarchar(128) NOT NULL)", sqlConn);
						newTable.ExecuteNonQuery();

						var newRow =
							new SqlCommand("INSERT INTO Units (Einheit) VALUES(@paramEinheit1), (@paramEinheit2), (@paramEinheit3)", sqlConn);
						newRow.Parameters.Add(new SqlParameter("@paramEinheit1", "Stück"));
						newRow.Parameters.Add(new SqlParameter("@paramEinheit2", "Packung"));
						newRow.Parameters.Add(new SqlParameter("@paramEinheit3", "Originalpackung"));

						newRow.ExecuteNonQuery();
					}

					if (!Sqlhelper.TableExists(sqlConn, "ProductData"))
					{
						var newTable =
							new SqlCommand("CREATE TABLE ProductData (ProduktID int IDENTITY(1,1) CONSTRAINT pkProduktID PRIMARY KEY, " +
							               "Produktnummer int NOT NULL CONSTRAINT fkProduktnummer FOREIGN KEY REFERENCES ProductNames(Produktnummer) ON DELETE CASCADE, " +
							               "Artikelnummer nvarchar(255) NOT NULL, " +
							               "VertriebID int NOT NULL CONSTRAINT fkVertriebID FOREIGN KEY REFERENCES Dealers(VertriebID) ON DELETE CASCADE, " +
							               "Preis nvarchar(128) NOT NULL, " +
							               "EinheitID int NOT NULL CONSTRAINT fkEinheitID FOREIGN KEY REFERENCES Units(EinheitID) ON DELETE CASCADE)", sqlConn);
						newTable.ExecuteNonQuery();
					}
				}
				catch (SqlException ex)
				{
					var reporter = new ExceptionReporter(ex);
					reporter.ReportExceptionToAdmin();
					MessageBox.Show(
						"Auf die Datenbank konnte nicht zugegriffen werden. Ein Fehlerbericht wurde an den Administrator gesendet.",
						"Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				finally
				{
					if (sqlConn.State == ConnectionState.Open)
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
			
			CboDealers.Items.Clear();

			var connString = new SqlConnectionStringBuilder
			    {
			        DataSource = "localhost",
			        InitialCatalog = "AMAM",
			        IntegratedSecurity = true
			    };

		    using(var sqlConn = new SqlConnection(connString.ToString()))
			{
				try
				{
					sqlConn.Open();

					if(Sqlhelper.TableExists(sqlConn, "Dealers"))
					{
						var dealerTable = new DataTable();
						var dataAdapter = new SqlDataAdapter("SELECT VertriebID, Vertrieb FROM Dealers", sqlConn);
						dealerTable.Clear();
						dataAdapter.Fill(dealerTable);

						CboDealers.ItemsSource = dealerTable.DefaultView;
						CboDealers.DisplayMemberPath = "Vertrieb";
						CboDealers.SelectedValuePath = "VertriebID";
						CboDealers.SelectedIndex = 0;

						if(CboDealers.Items.Count == 0)
						{
							MessageBoxResult answer = MessageBox.Show("Es wurden keine Vertriebe gefunden. Möchten Sie jetzt einen neuen Vertrieb hinzufügen?", "Fehler", MessageBoxButton.YesNo, MessageBoxImage.Question);
							if(answer == MessageBoxResult.Yes)
							{
								var dealerManager = new FrmDealersList();
								dealerManager.ShowDialog();
								ReadDealers();
							}
						}
					}
					else
					{
						MessageBoxResult answer = MessageBox.Show("Es wurden keine Vertriebe gefunden. Möchten Sie jetzt einen neuen Vertrieb hinzufügen?", "Fehler", MessageBoxButton.YesNo, MessageBoxImage.Question);
						if(answer == MessageBoxResult.Yes)
						{
							var dealerManager = new FrmDealersList();
							dealerManager.ShowDialog();
							ReadDealers();
						}
					}
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

		private void ReadUnits()
		{

			CboPackageMass.Items.Clear();

			var connString = new SqlConnectionStringBuilder
			    {
			        DataSource = "localhost",
			        InitialCatalog = "AMAM",
			        IntegratedSecurity = true
			    };

		    using(var sqlConn = new SqlConnection(connString.ToString()))
			{
				try
				{
					sqlConn.Open();

					if(Sqlhelper.TableExists(sqlConn, "Units"))
					{
						var dealerTable = new DataTable();
						var dataAdapter = new SqlDataAdapter("SELECT EinheitID, Einheit FROM Units", sqlConn);
						dealerTable.Clear();
						dataAdapter.Fill(dealerTable);

						CboPackageMass.ItemsSource = dealerTable.DefaultView;
						CboPackageMass.DisplayMemberPath = "Einheit";
						CboPackageMass.SelectedValuePath = "Einheit";
						CboPackageMass.SelectedIndex = 0;

						if(CboDealers.Items.Count == 0)
						{
							MessageBoxResult answer = MessageBox.Show("Es wurden keine Verpackungseinheiten gefunden. Möchten Sie jetzt eine neue Verpackungseinheit hinzufügen?", "Fehler", MessageBoxButton.YesNo, MessageBoxImage.Question);
							if(answer == MessageBoxResult.Yes)
							{
								var dealerManager = new FrmDealersList();
								dealerManager.ShowDialog();
								ReadDealers();
							}
						}
					}
					else
					{
						MessageBoxResult answer = MessageBox.Show("Es wurden keine Verpackungseinheiten gefunden. Möchten Sie jetzt eine neue Verpackungseinheit hinzufügen?", "Fehler", MessageBoxButton.YesNo, MessageBoxImage.Question);
						if(answer == MessageBoxResult.Yes)
						{
							var dealerManager = new FrmDealersList();
							dealerManager.ShowDialog();
							ReadDealers();
						}
					}
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

		private void AddProductNameToDataBase()
		{
			TbProductName.Background = Brushes.White;
			TbProductName.Foreground = Brushes.Black;

			if(TbProductName.Text.Length > 0)
			{
				var connString = new SqlConnectionStringBuilder
				    {
				        DataSource = "localhost",
				        InitialCatalog = "AMAM",
				        IntegratedSecurity = true
				    };

			    using(var sqlConn = new SqlConnection(connString.ToString()))
				{
					try
					{
						sqlConn.Open();

						var newRow = new SqlCommand("INSERT INTO ProductNames (Produktname) VALUES(@paramName)", sqlConn);
						newRow.Parameters.Add(new SqlParameter("@paramName", TbProductName.Text));
						newRow.ExecuteNonQuery();
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
			else
			{
				MessageBox.Show("Sie müssen einen Produktnamen eingeben.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
				TbProductName.Background = Brushes.Red;
				TbProductName.Foreground = Brushes.Black;
			}
		}

		private void AddProductDataToDataBase()
		{
			var connString = new SqlConnectionStringBuilder
			    {
			        DataSource = "localhost",
			        InitialCatalog = "AMAM",
			        IntegratedSecurity = true
			    };

		    using(var sqlConn = new SqlConnection(connString.ToString()))
			{
				try
				{
					sqlConn.Open();

					var newRow = new SqlCommand("INSERT INTO ProductData (Produktnummer, Artikelnummer, VertriebID, Preis, EinheitID) " +
														"VALUES((SELECT Produktnummer FROM ProductNames WHERE ProductNames.Produktname = @paramName), " +
														"@paramArtikelnummer, " + 
														"(SELECT VertriebID FROM Dealers WHERE Dealers.VertriebID = @paramVertriebID), " + 
														"@paramPreis, "+
														"(SELECT EinheitID FROM Units WHERE Units.Einheit = @paramEinheit))", sqlConn);

					foreach(DataRow row in _productDataTable.Rows)
					{
						newRow.Parameters.Add(new SqlParameter("@paramName", TbProductName.Text));
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

		private void AddDataSet(object sender, RoutedEventArgs e)
		{
			if(TbArticleNumber.Text.Length > 0)
			{
				try
				{
					TbPrice.Text = Validator.FormatPrice(TbPrice.Text);

					try
					{
						DataRow newDataRow = _productDataTable.NewRow();
						newDataRow["VertriebID"] = CboDealers.SelectedValue;
						newDataRow["Artikelnummer"] = TbArticleNumber.Text;
						newDataRow["Preis"] = TbPrice.Text;
						newDataRow["Verpackungseinheit"] = CboPackageMass.SelectedValue;

						_productDataTable.Rows.Add(newDataRow);
					}
					catch(ConstraintException)
					{
						MessageBox.Show("Ein Datensatz für die Artikelnummer " + TbArticleNumber.Text + " existiert bereits.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
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
			Close();
		}

		private void SaveProduct(object sender, RoutedEventArgs e)
		{
			AddProductNameToDataBase();
			AddProductDataToDataBase();
			Close();
		}

	}
}
