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
using System.IO;

namespace AMAM
{
    /// <summary>
    /// Interaktionslogik für frmAddUser.xaml
    /// </summary>
    public partial class frmAddUser : Window
    {
        private string xmlPath;
        public frmAddUser()
        {
            InitializeComponent();
            xmlPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AMAM\\users.xml");
        }

        private void AddUser(object sender, RoutedEventArgs e)
        {
            if(File.Exists(xmlPath))
            {
            User usr = new User(xmlPath);
            usr.CreateUser(tbUsername.Text, tbPassword.Password);
            this.Close();
            }
            else
            {
                MessageBox.Show("Die Benutzerdatenbank konnte nicht geladen werden. Soll sie Jetzt neu erstellt werden?", "Fehler", MessageBoxButton.YesNo, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
