using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Media;

namespace Amam
{
    /// <summary>
    /// Interaktionslogik für frmChangeDealer.xaml
    /// </summary>
    public partial class FrmChangeDealer
    {
        readonly string _id;

        public FrmChangeDealer(string id)
        {
            InitializeComponent();

            _id = id;

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
                    var readRow = new SqlCommand("SELECT Vertrieb, eMail, Kundennummer FROM Dealers WHERE VertriebID = @paramPK", sqlConn);
                    readRow.Parameters.Add(new SqlParameter("@paramPK", id));
                    using(SqlDataReader dr = readRow.ExecuteReader())
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

        private void ChangeDealer(object sender, RoutedEventArgs e)
        {
            var validator = new Validator();

            tbDealer.Background = Brushes.White;
            tbDealer.Foreground = Brushes.Black;
            tbMail.Background = Brushes.White;
            tbMail.Foreground = Brushes.Black;
            tbCustomerID.Background = Brushes.White;
            tbCustomerID.Foreground = Brushes.Black;

            if(tbDealer.Text.Length > 0 && validator.IsMailValid(tbMail.Text) && tbCustomerID.Text.Length > 0)
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
                        var changeCommand = new SqlCommand("UPDATE Dealers SET " +
                                                           "Vertrieb = @paramVertrieb, " +
                                                           "eMail = @paramMail, " +
                                                           "Kundennummer = @paramCustomerID " +
                                                           "WHERE VertriebID = @paramPK", sqlConn);
                        
                        changeCommand.Parameters.Add(new SqlParameter("@paramVertrieb", tbDealer.Text));
                        changeCommand.Parameters.Add(new SqlParameter("@paramMail", tbMail.Text));
                        changeCommand.Parameters.Add(new SqlParameter("@paramCustomerID", tbCustomerID.Text));
                        changeCommand.Parameters.Add(new SqlParameter("@paramPK", _id));

                        changeCommand.ExecuteNonQuery();
                        Close();
                    }
                    catch(SqlException ex)
                    {
                        var reporter = new ExceptionReporter(ex);
                        reporter.ReportExceptionToAdmin();
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
