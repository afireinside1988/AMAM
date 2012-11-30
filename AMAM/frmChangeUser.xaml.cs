using System;
using System.Data;
using System.Windows;
using System.Windows.Media;

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmChangeUser.xaml
	/// </summary>
	public partial class FrmChangeUser
	{
	    readonly UserList _usr;

		public FrmChangeUser(DataSet parent, string user)
		{
			InitializeComponent();
			_usr = new UserList(parent);
			try
			{
				_usr.SelectUser(user);
				tbNewUsername.Text = user;
			}
			catch(ArgumentOutOfRangeException ex)
			{
				MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void OldPasswordChanged(object sender, RoutedEventArgs e)
		{
			if(Encryption.CreateHash(tbOldPassword.Password) == _usr.PasswordHash)
			{
				gbChangeUser.IsEnabled = true;
				btnChange.IsEnabled = true;
			}
			else
			{
				gbChangeUser.IsEnabled = false;
				btnChange.IsEnabled = false;
			}
		}

		private void Close(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void ChangeUser(object sender, RoutedEventArgs e)
		{
			tbNewUsername.Background = Brushes.White;
			tbNewUsername.Foreground = Brushes.Black;
			tbNewPassword.Background = Brushes.White;
			tbNewPassword.Foreground = Brushes.Black;

				try
				{
					_usr.ChangeUserName(tbNewUsername.Text);
				}
				catch(UserNameIsNullOrEmptyException ex)
				{
					MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
					tbNewUsername.Background = Brushes.Red;
					tbNewUsername.Foreground = Brushes.White;
					tbNewUsername.Focus();
					return;
				}
				catch(ArgumentException ex)
				{
					MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
					tbNewUsername.Background = Brushes.Red;
					tbNewUsername.Foreground = Brushes.White;
					tbNewUsername.Focus();
					return;
				}

				if(tbNewPassword.Password.Length > 0)
				{
					if(tbNewPassword.Password == tbNewPasswordConfirm.Password)
					{
						try
						{
							_usr.SelectUser(tbNewUsername.Text);
							_usr.ChangePassword(tbNewPassword.Password);
						}
						catch(PasswordIsNullOrEmptyException ex)
						{
							MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
							tbNewPassword.Background = Brushes.Red;
							tbNewPassword.Foreground = Brushes.White;
							tbNewPassword.Focus();
							return;
						}
						catch(ArgumentOutOfRangeException ex)
						{
							MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
						}
					}
					else
					{
						MessageBox.Show("Das Passwort stimmt nicht mit der Passwortbestätigung überein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
						tbNewPassword.Password = null;
						tbNewPasswordConfirm.Password = null;
						tbNewPassword.Focus();
						return;
					}
				}

				Close();
		}
	}
}
