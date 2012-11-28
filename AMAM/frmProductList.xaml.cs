using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmProductList.xaml
	/// </summary>
	public partial class FrmProductList : Window
	{
		DataTable ProductsTable = new DataTable();

		public FrmProductList()
		{
			InitializeComponent();

			SqlConnectionStringBuilder ConnString = new SqlConnectionStringBuilder();
			ConnString.DataSource = "localhost";
			ConnString.InitialCatalog = "AMAM";
			ConnString.IntegratedSecurity = true;

			using(SqlConnection sqlConn = new SqlConnection(ConnString.ToString()))
			{
				try
				{
					sqlConn.Open();
					if(sqlhelper.TableExists(sqlConn, "ProductNames") && sqlhelper.TableExists(sqlConn, "ProductData") && sqlhelper.TableExists(sqlConn, "Dealers"))
					{
						sqlConn.Close();
						RefreshProductList();
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

		private void RefreshProductList()
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

					string command = "SELECT ProductNames.Produktnummer, ProductNames.Produktname, Units.Einheit FROM ProductNames " +
										"INNER JOIN ProductData ON ProductData.Produktnummer = ProductNames.Produktnummer " +
										"INNER JOIN Units ON Units.EinheitID = ProductData.EinheitID " + 
										"GROUP BY ProductNames.Produktnummer, ProductNames.Produktname, Units.Einheit";
										

					SqlDataAdapter dataAdapter = new SqlDataAdapter(command, sqlConn);
					SqlCommandBuilder sqlCommand = new SqlCommandBuilder(dataAdapter);
					dataAdapter.Fill(ProductsTable);

					lvProducts.ItemsSource = ProductsTable.DefaultView;
					lvProducts.SelectedValuePath = "Produktnummer";

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

		private void Close(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void AddProduct(object sender, RoutedEventArgs e)
		{
			FrmAddProduct AddProduct = new FrmAddProduct();
			AddProduct.ShowDialog();
			RefreshProductList();
		}

		private void ChangeFilter(object sender, TextChangedEventArgs e)
		{
			ProductsTable.DefaultView.RowFilter = "Produktname LIKE '" + tbFilter.Text + "*'";
		}

		private void SelectedProductChanged(object sender, SelectionChangedEventArgs e)
		{
			//todo DataGrid mit ProductData füllen wenn Produkt gewählt wird
			//todo evtl. Gridsplitter im Design?
		}

	}
}
