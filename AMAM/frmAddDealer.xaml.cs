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
            tbDealer.Background = Brushes.White;
            tbDealer.Foreground = Brushes.Black;
            tbMail.Background = Brushes.White;
            tbMail.Foreground = Brushes.Black;
            tbCustomerID.Background = Brushes.White;
            tbCustomerID.Foreground = Brushes.Black;

            var validator = new Validator();

            if(tbDealer.Text.Length > 0 && validator.IsMailValid(tbMail.Text) && tbCustomerID.Text.Length > 0)
            {
                AddDealerToDataBase(tbDealer.Text, tbMail.Text, tbCustomerID.Text);
                Close();
            }
            else
            {
                if(tbDealer.Text.Length == 0)
                {
                    tbDealer.Background = Brushes.Red;
                    tbDealer.Foreground = Brushes.Black;
                    MessageBox.Show("Sie müssen einen Vertriebspartner eingeben.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                if(!validator.IsMailValid(tbMail.Text))
                {
                    tbMail.Background = Brushes.Red;
                    tbMail.Foreground = Brushes.Black;
                    MessageBox.Show("Die eingegebene eMail-Adresse ist nicht gültig.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                if(tbCustomerID.Text.Length == 0)
                {
                    tbCustomerID.Background = Brushes.Red;
                    tbCustomerID.Foreground = Brushes.Black;
                    MessageBox.Show("Sie müssen eine Kundennummer eingeben.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
	}
}
