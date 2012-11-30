using System;
using System.Data;
using System.Windows;
using System.Windows.Media;

namespace Amam
{
    /// <summary>
    /// Interaktionslogik für frmAddUser.xaml
    /// </summary>
    public partial class FrmAddUser
    {
        readonly DataSet _ds;

        public FrmAddUser(DataSet parent)
        {
            InitializeComponent();
            _ds = parent;
        }

        /// <summary>
        /// Methode initiiert die Erstellung eines neuen Benutzers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddUser(object sender, RoutedEventArgs e)
        {
                tbUsername.Background = Brushes.White;
                tbUsername.Foreground = Brushes.Black;
                tbPassword.Background = Brushes.White;
                tbPassword.Foreground = Brushes.Black;

                var usr = new UserList(_ds);

				if(tbPassword.Password == tbPasswordConfirm.Password)
				{
					try
					{
						usr.CreateUser(tbUsername.Text, tbPassword.Password);
						Close();
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
					MessageBox.Show("Das Passwort stimmt nicht mit der Passwortbestätigung überein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
					tbPassword.Password = null;
					tbPasswordConfirm.Password = null;
					tbPassword.Focus();
				}
        }

        /// <summary>
        /// Methode schließt das Formular
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
