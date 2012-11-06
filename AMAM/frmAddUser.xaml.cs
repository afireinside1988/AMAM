using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Data;

namespace Amam
{
    /// <summary>
    /// Interaktionslogik für frmAddUser.xaml
    /// </summary>
    public partial class FrmAddUser : Window
    {
        private string xmlPath;

        public FrmAddUser()
        {
            InitializeComponent();
            xmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AMAM\\users.xml");
        }

        /// <summary>
        /// Methode initiiert die Erstellung eines neuen Benutzers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddUser(object sender, RoutedEventArgs e)
        {
            if(File.Exists(xmlPath))
            {
                tbUsername.Background = Brushes.White;
                tbUsername.Foreground = Brushes.Black;
                tbPassword.Background = Brushes.White;
                tbPassword.Foreground = Brushes.Black;

                UserList usr = new UserList(xmlPath);

                try
                {
					usr.CreateUser(tbUsername.Text, tbPassword.Password);
                    usr.SaveUserlist();
                    this.Close();
                }
                catch(UserNameIsNullOrEmptyException ex)
                {
					MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
					tbUsername.Background = Brushes.Red;
					tbUsername.Foreground = Brushes.White;
					tbUsername.Focus();
				}
                catch(PasswordIsNullOrEmptyException ex)
                {
					MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
					tbPassword.Background = Brushes.Red;
					tbPassword.Foreground = Brushes.White;
					tbPassword.Focus();
                }
				catch(ArgumentException ex)
                {
					MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
					tbUsername.Background = Brushes.Red;
					tbUsername.Foreground = Brushes.White;
					tbUsername.Focus();
                }
			}
            else
            {
                MessageBoxResult MessageResult = MessageBox.Show("Die Benutzerdatenbank konnte nicht geladen werden. Soll sie Jetzt neu erstellt werden?", "Fehler", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if(MessageResult == MessageBoxResult.Yes)
                {
                    XMLInitializer Init = new XMLInitializer();
                    Init.CreateXMLFileStructure("users");
                }
               

                this.Close();
            }
        }

        /// <summary>
        /// Methode schließt das Formular
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
