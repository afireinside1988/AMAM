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
using System.Data;

namespace Amam
{
    /// <summary>
    /// Interaktionslogik für frmChangeDealer.xaml
    /// </summary>
    public partial class FrmChangeDealer : Window
    {
        string _vertrieb;

        public FrmChangeDealer(string vertrieb)
        {
            InitializeComponent();

            _vertrieb = vertrieb;

            SqlConnectionStringBuilder ConnString = new SqlConnectionStringBuilder();
            ConnString.DataSource = "localhost";
            ConnString.InitialCatalog = "AMAM";
            ConnString.IntegratedSecurity = true;

            using(SqlConnection sqlConn = new SqlConnection(ConnString.ToString()))
            {
                try
                {
                    sqlConn.Open();
                    SqlCommand ReadRow = new SqlCommand("SELECT Vertrieb, eMail, Kundennummer FROM Dealers WHERE Vertrieb = @paramVertrieb", sqlConn);
                    ReadRow.Parameters.Add(new SqlParameter("@paramVertrieb", vertrieb));
                    using(SqlDataReader dr = ReadRow.ExecuteReader())
                    {
                        while(dr.Read())
                        {
                            tbDealer.Text = dr["Vertrieb"].ToString();
                            tbMail.Text = dr["eMail"].ToString();
                            tbCustomerID.Text = dr["Kundennummer"].ToString();
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

        private void ChangeDealer(object sender, RoutedEventArgs e)
        {
            Validator validator = new Validator();

            tbDealer.Background = Brushes.White;
            tbDealer.Foreground = Brushes.Black;
            tbMail.Background = Brushes.White;
            tbMail.Foreground = Brushes.Black;
            tbCustomerID.Background = Brushes.White;
            tbCustomerID.Foreground = Brushes.Black;

            if(tbDealer.Text.Length > 0 && validator.IsMailValid(tbMail.Text) && tbCustomerID.Text.Length > 0)
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
                        SqlCommand changeCommand = new SqlCommand("UPDATE Dealers SET " +
                                                                    "Vertrieb = @paramVertrieb, " +
                                                                    "eMail = @paramMail, " +
                                                                    "Kundennummer = @paramCustomerID " +
                                                                    "WHERE Vertrieb = @paramOldVertrieb", sqlConn);
                        
                        changeCommand.Parameters.Add(new SqlParameter("@paramVertrieb", tbDealer.Text));
                        changeCommand.Parameters.Add(new SqlParameter("@paramMail", tbMail.Text));
                        changeCommand.Parameters.Add(new SqlParameter("@paramCustomerID", tbCustomerID.Text));
                        changeCommand.Parameters.Add(new SqlParameter("@paramOldVertrieb", _vertrieb));

                        changeCommand.ExecuteNonQuery();
                        this.Close();
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
