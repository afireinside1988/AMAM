using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmProductList.xaml
	/// </summary>
	public partial class FrmProductList
	{
	    readonly DataTable _productsTable = new DataTable();

		public FrmProductList()
		{
			InitializeComponent();

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
					if(Sqlhelper.TableExists(sqlConn, "ProductNames") && Sqlhelper.TableExists(sqlConn, "ProductData") && Sqlhelper.TableExists(sqlConn, "Dealers"))
					{
						sqlConn.Close();
						RefreshProductList();
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

		private void RefreshProductList()
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

					const string command = "SELECT ProductNames.Produktnummer, ProductNames.Produktname, Units.Einheit FROM ProductNames " +
					                       "INNER JOIN ProductData ON ProductData.Produktnummer = ProductNames.Produktnummer " +
					                       "INNER JOIN Units ON Units.EinheitID = ProductData.EinheitID " + 
					                       "GROUP BY ProductNames.Produktnummer, ProductNames.Produktname, Units.Einheit";
										

					var dataAdapter = new SqlDataAdapter(command, sqlConn);
                    _productsTable.Clear();
					dataAdapter.Fill(_productsTable);

					dgProducts.ItemsSource = _productsTable.DefaultView;
					dgProducts.SelectedValuePath = "Produktnummer";

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

		private void AddProduct(object sender, RoutedEventArgs e)
		{
		    var addProduct = new FrmAddProduct();
		    addProduct.ShowDialog();
			RefreshProductList();
		}

		private void ChangeFilter(object sender, TextChangedEventArgs e)
		{
			_productsTable.DefaultView.RowFilter = "Produktname LIKE '*" + tbFilter.Text + "*'";
		}

		private void SelectedProductChanged(object sender, SelectionChangedEventArgs e)
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

					const string command = "SELECT Dealers.Vertrieb, ProductData.Artikelnummer, ProductData.Preis FROM ProductData " +
					                       "INNER JOIN Dealers ON ProductData.VertriebID = Dealers.VertriebID " +
					                       "INNER JOIN Units ON Units.Einheit = @paramEinheit " +
					                       "WHERE ProductData.Produktnummer = @paramProduktnummer " +
					                       "AND ProductData.EinheitID = Units.EinheitID";

					var dataAdapter = new SqlDataAdapter {SelectCommand = new SqlCommand(command, sqlConn)};

				    if(dgProducts.SelectedItem != null)
                    {
                        var currentRowView = (DataRowView)dgProducts.SelectedItem;
                        DataRow row = currentRowView.Row;

                        dataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@paramProduktnummer", row["Produktnummer"].ToString()));
                        dataAdapter.SelectCommand.Parameters.Add(new SqlParameter("@paramEinheit", row["Einheit"].ToString()));
                        var productDataTable = new DataTable();
                        dataAdapter.Fill(productDataTable);

                        dgProductData.DataContext = productDataTable.DefaultView;
                    }
                    else dgProductData.DataContext = null;
                    
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

        private void RemovePoduct(object sender, RoutedEventArgs e)
        {
            var currentRow = (DataRowView)dgProducts.SelectedItem;
            DataRow selectedRow = currentRow.Row;
            MessageBoxResult removeQuestion = MessageBox.Show("Sind Sie sicher, dass das Produkt " + selectedRow["Produktname"] + " gelöscht werden soll?", "Produkt löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (removeQuestion != MessageBoxResult.Yes) return;

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

                    //todo hier Lösch-Kommando ausführen
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
        }

	}
}
