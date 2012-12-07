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
                TbUsername.Background = Brushes.White;
                TbUsername.Foreground = Brushes.Black;
                TbPassword.Background = Brushes.White;
                TbPassword.Foreground = Brushes.Black;

                var usr = new UserList(_ds);

				if(TbPassword.Password == TbPasswordConfirm.Password)
				{
					try
					{
						usr.CreateUser(TbUsername.Text, TbPassword.Password);
						Close();
					}
					catch(UserNameIsNullOrEmptyException ex)
					{
						MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
						TbUsername.Background = Brushes.Red;
						TbUsername.Foreground = Brushes.White;
						TbUsername.Focus();
					}
					catch(PasswordIsNullOrEmptyException ex)
					{
						MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
						TbPassword.Background = Brushes.Red;
						TbPassword.Foreground = Brushes.White;
						TbPassword.Focus();
					}
					catch(ArgumentException ex)
					{
						MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
						TbUsername.Background = Brushes.Red;
						TbUsername.Foreground = Brushes.White;
						TbUsername.Focus();
					}
				}
				else
				{
					MessageBox.Show("Das Passwort stimmt nicht mit der Passwortbestätigung überein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
					TbPassword.Password = null;
					TbPasswordConfirm.Password = null;
					TbPassword.Focus();
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
