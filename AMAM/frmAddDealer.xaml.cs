using System.Data.SqlClient;
using System.Windows;
using System.Windows.Media;

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmAddDealer.xaml
	/// </summary>
	public partial class FrmAddDealer
	{
		public FrmAddDealer()
		{
			InitializeComponent();
		}

		private static void AddDealerToDataBase(string dealerName, string mail, string cutomerID)
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
                    var newRowCommand = new SqlCommand("INSERT INTO Dealers (Vertrieb, eMail, Kundennummer) "+
                                                       "VALUES(@paramVertrieb, @paramMail, @paramCustomerID)", sqlConn);
 
                    newRowCommand.Parameters.Add(new SqlParameter("@paramVertrieb", dealerName));
                    newRowCommand.Parameters.Add(new SqlParameter("@paramMail", mail));
                    newRowCommand.Parameters.Add(new SqlParameter("@paramCustomerID", cutomerID));

                    newRowCommand.ExecuteNonQuery();
                }
                catch(SqlException ex)
                {
                    var reporter = new ExceptionReporter(ex);
                    reporter.ReportExceptionToAdmin();
                    MessageBox.Show("Auf die Datenbank konnte nicht zugegriffen werden. Ein Fehlerbericht wurde an den Administrator gesendet.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    if(sqlConn.State == System.Data.ConnectionState.Open)
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
            TbDealer.Background = Brushes.White;
            TbDealer.Foreground = Brushes.Black;
            TbMail.Background = Brushes.White;
            TbMail.Foreground = Brushes.Black;
            TbCustomerId.Background = Brushes.White;
            TbCustomerId.Foreground = Brushes.Black;

            var validator = new Validator();

            if(TbDealer.Text.Length > 0 && validator.IsMailValid(TbMail.Text) && TbCustomerId.Text.Length > 0)
            {
                AddDealerToDataBase(TbDealer.Text, TbMail.Text, TbCustomerId.Text);
                Close();
            }
            else
            {
                if(TbDealer.Text.Length == 0)
                {
                    TbDealer.Background = Brushes.Red;
                    TbDealer.Foreground = Brushes.Black;
                    MessageBox.Show("Sie müssen einen Vertriebspartner eingeben.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                if(!validator.IsMailValid(TbMail.Text))
                {
                    TbMail.Background = Brushes.Red;
                    TbMail.Foreground = Brushes.Black;
                    MessageBox.Show("Die eingegebene eMail-Adresse ist nicht gültig.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                if(TbCustomerId.Text.Length == 0)
                {
                    TbCustomerId.Background = Brushes.Red;
                    TbCustomerId.Foreground = Brushes.Black;
                    MessageBox.Show("Sie müssen eine Kundennummer eingeben.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
	}
}
