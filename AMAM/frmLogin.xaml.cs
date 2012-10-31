using System;
using System.Data;
using System.Windows;
using System.Xml;

namespace AMAM
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string xmlPath;
        DataSet ds = new DataSet();
        public MainWindow()
        {

            
            InitializeComponent();

            xmlPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AMAM\\users.xml");

            XMLInitializer initxml = new XMLInitializer(); //Neuen XML-Initialisierer einbinden
            string[] xmlFiles = { "users", "dealers", "products", "prices" }; //String-Array mit den benötigten Dateien

            if(initxml.XMLStructureInitialized(xmlFiles))
            {
                InitUserTable();
            }
            else
            {
                initxml.CreateXMLFileStructure(xmlFiles);
                MessageBox.Show("Dies ist der erste Start von Ambulance Merseburg Apotheken Manager.\nSie müssen nun einen Benutzer erstellen.", "Willkommen", MessageBoxButton.OK, MessageBoxImage.Information);
                frmAddUser AddUser = new frmAddUser();
                AddUser.ShowDialog();
                InitUserTable();

            }
        }

        /// <summary>
        /// Methode zum Laden der User-Daten in die Form
        /// </summary>
        private void InitUserTable()
        {
            XmlReader xmlFile = XmlReader.Create(xmlPath, new XmlReaderSettings());
            ds.ReadXml(xmlFile);
            xmlFile.Close();
            // TODO Hier gehts weiter
            cboUsername.DataContext = ds.Tables["user"].DefaultView;

        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LogIn(object sender, RoutedEventArgs e)
        {

        }

    }
}
