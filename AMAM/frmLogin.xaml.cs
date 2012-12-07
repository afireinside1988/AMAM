using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Amam
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private readonly string _xmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AMAM\\users.xml");
        readonly XMLInitializer _initxml = new XMLInitializer(); //Neuen XML-Initialisierer einbinden
        readonly DataSet _ds = new DataSet();

        public MainWindow()
        {

            
            InitializeComponent();

            string[] xmlFiles = {"users"}; //String-Array mit den benötigten Dateien
            _ds.DataSetName = "users";

            if(_initxml.XMLStructureInitialized(xmlFiles))
            {
                InitUserTable();
            }
            else
            {
                _initxml.CreateXMLFileStructure(xmlFiles);
                MessageBox.Show("Dies ist der erste Start von Ambulance Merseburg Apotheken Manager."+Environment.NewLine + "Sie müssen nun einen Benutzer erstellen.", "Willkommen", MessageBoxButton.OK, MessageBoxImage.Information);
                var addUser = new FrmAddUser(_ds);
                addUser.ShowDialog();
                _ds.WriteXml(_xmlPath);
				InitUserTable();
            }
        }

        /// <summary>
        /// Methode zum Laden der User-Daten in die Form
        /// </summary>
        private void InitUserTable()
        {
            CboUsername.Items.Clear();
			
            var xmlFile = XmlReader.Create(_xmlPath, new XmlReaderSettings());
            try
            {

				if(_ds.Tables.Count > 0)
				{
					CboUsername.ItemsSource = _ds.Tables["user"].DefaultView;
					CboUsername.SelectedIndex = 0;
				}
				else
				{
					_ds.ReadXml(xmlFile);
					xmlFile.Close();
					if(_ds.Tables.Count > 0)
					{
						CboUsername.ItemsSource = _ds.Tables["user"].DefaultView;
						CboUsername.SelectedIndex = 0;
					}
				}
            }
            catch(XmlException ex)
            {
                MessageBox.Show("Die Benutzerdatenbank ist korrupt. Es wird nun eine neue Benutzerdatenbank angelegt." + Environment.NewLine + Environment.NewLine + "Die korrupte Benutzerdatenbank wird gesichert und der Administrator per eMail kontaktiert. Bitte einen Moment Geduld.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);


                File.Move(_xmlPath, _xmlPath + ".corrupt");
                File.Delete(_xmlPath);
                _initxml.CreateXMLFileStructure("users");

                var exReport = new ExceptionReporter(ex);
                exReport.ReportExceptionToAdmin(_xmlPath + ".corrupt");
            }
            if(!CboUsername.HasItems)
            {
                MessageBoxResult messageResult = MessageBox.Show("Die Benutzerdatenbank enthält keinen Benutzer." + Environment.NewLine + "Sie müssen nun einen Benutzer anlegen um fortfahren zu können.", "Fehler", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                if(messageResult == MessageBoxResult.OK)
                {
                    var addUser = new FrmAddUser(_ds);
                    addUser.ShowDialog();
                    _ds.WriteXml(_xmlPath);
					CboUsername.ItemsSource = _ds.Tables["user"].DefaultView;
					CboUsername.SelectedIndex = 0;
                }
                else
                {
                    Environment.Exit(1);
                }
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LogIn(object sender, RoutedEventArgs e)
        {
            TbPassword.Background = Brushes.White;
            TbPassword.Foreground = Brushes.Black;

            if(Encryption.CreateHash(TbPassword.Password) == CboUsername.SelectedValue.ToString())
            {
				var mainForm = new FrmMain();
				mainForm.Show();
				Close();
            }
            else
            {
                TbPassword.Background = Brushes.Red;
                TbPassword.Foreground = Brushes.White;
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
            if(TbPassword.Password.Length > 0 && CboUsername.SelectedIndex > -1)
            {
                BtnLogIn.IsEnabled = true;
            }
            else
            {
                BtnLogIn.IsEnabled = false;
            }
        }
    }
}
