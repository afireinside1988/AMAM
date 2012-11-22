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
using System.Data.Sql;
using System.Data.SqlClient;

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmAddDealer.xaml
	/// </summary>
	public partial class FrmAddDealer : Window
	{
		public FrmAddDealer()
		{
			InitializeComponent();
		}

		private void AddDealerToDataBase(string dealerName, string mail, string cutomerID)
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
                    SqlCommand newRowCommand = new SqlCommand("INSERT INTO Dealers (Vertrieb, eMail, Kundennummer) "+
                                                                "VALUES(@paramVertrieb, @paramMail, @paramCustomerID)", sqlConn);
 
                    newRowCommand.Parameters.Add(new SqlParameter("@paramVertrieb", dealerName));
                    newRowCommand.Parameters.Add(new SqlParameter("@paramMail", mail));
                    newRowCommand.Parameters.Add(new SqlParameter("@paramCustomerID", cutomerID));

                    newRowCommand.ExecuteNonQuery();
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
                    if(sqlConn.State == System.Data.ConnectionState.Open)
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
            tbDealer.Background = Brushes.White;
            tbDealer.Foreground = Brushes.Black;
            tbMail.Background = Brushes.White;
            tbMail.Foreground = Brushes.Black;
            tbCustomerID.Background = Brushes.White;
            tbCustomerID.Foreground = Brushes.Black;

            Validator validator = new Validator();

            if(tbDealer.Text.Length > 0 && validator.IsMailValid(tbMail.Text) && tbCustomerID.Text.Length > 0)
            {
                AddDealerToDataBase(tbDealer.Text, tbMail.Text, tbCustomerID.Text);
                this.Close();
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
