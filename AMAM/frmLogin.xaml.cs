using System;
using System.IO;
using System.Data;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Data;
using System.Windows.Media;
using System.Security.Cryptography;

namespace Amam
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string xmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AMAM\\users.xml");
        XMLInitializer initxml = new XMLInitializer(); //Neuen XML-Initialisierer einbinden
		DataSet ds = new DataSet();

        public MainWindow()
        {

            
            InitializeComponent();

            string[] xmlFiles = {"users"}; //String-Array mit den benötigten Dateien
            ds.DataSetName = "users";

            if(initxml.XMLStructureInitialized(xmlFiles))
            {
                InitUserTable();
            }
            else
            {
                initxml.CreateXMLFileStructure(xmlFiles);
                MessageBox.Show("Dies ist der erste Start von Ambulance Merseburg Apotheken Manager."+Environment.NewLine + "Sie müssen nun einen Benutzer erstellen.", "Willkommen", MessageBoxButton.OK, MessageBoxImage.Information);
                FrmAddUser AddUser = new FrmAddUser(ds);
                AddUser.ShowDialog();
                ds.WriteXml(xmlPath);
				InitUserTable();
            }
        }

        /// <summary>
        /// Methode zum Laden der User-Daten in die Form
        /// </summary>
        private void InitUserTable()
        {
            cboUsername.Items.Clear();
			
            XmlReader xmlFile = XmlReader.Create(xmlPath, new XmlReaderSettings());
            try
            {

				if(ds.Tables.Count > 0)
				{
					cboUsername.ItemsSource = ds.Tables["user"].DefaultView;
				}
				else
				{
					ds.ReadXml(xmlFile);
					xmlFile.Close();
					if(ds.Tables.Count > 0)
					{
						cboUsername.ItemsSource = ds.Tables["user"].DefaultView;
					}
				}
            }
            catch(XmlException ex)
            {
                MessageBox.Show("Die Benutzerdatenbank ist korrupt. Es wird nun eine neue Benutzerdatenbank angelegt." + Environment.NewLine + Environment.NewLine + "Die korrupte Benutzerdatenbank wird gesichert und der Administrator per eMail kontaktiert. Bitte einen Moment Geduld.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);


                File.Move(xmlPath, xmlPath + ".corrupt");
                File.Delete(xmlPath);
                initxml.CreateXMLFileStructure("users");

                ExceptionReporter ExReport = new ExceptionReporter(ex);
                ExReport.ReportExceptionToAdmin(xmlPath + ".corrupt");
            }
            if(!cboUsername.HasItems)
            {
                MessageBoxResult MessageResult = MessageBox.Show("Die Benutzerdatenbank enthält keinen Benutzer." + Environment.NewLine + "Sie müssen nun einen Benutzer anlegen um fortfahren zu können.", "Fehler", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                if(MessageResult == MessageBoxResult.OK)
                {
                    FrmAddUser AddUser = new FrmAddUser(ds);
                    AddUser.ShowDialog();
                    ds.WriteXml(xmlPath);
                }
                else
                {
                    Environment.Exit(1);
                }
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LogIn(object sender, RoutedEventArgs e)
        {
            tbPassword.Background = Brushes.White;
            tbPassword.Foreground = Brushes.Black;

            if(Encryption.CreateHash(tbPassword.Password) == cboUsername.SelectedValue.ToString())
            {
				frmUserlist Userlist = new frmUserlist();
				Userlist.Show();
				this.Close();
				//MessageBox.Show("Das Passwort ist richtig.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                tbPassword.Background = Brushes.Red;
                tbPassword.Foreground = Brushes.White;
                MessageBox.Show("Das Passwort ist falsch.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Überprüft den Status der Eingabe, um den Login-Button freizugeben
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordInputChanged(object sender, RoutedEventArgs e)
        {
            if(tbPassword.Password.Length > 0 && cboUsername.SelectedIndex > -1)
            {
                btnLogIn.IsEnabled = true;
            }
            else
            {
                btnLogIn.IsEnabled = false;
            }
        }
    }
}
