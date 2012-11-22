using System;
using System.Data;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Media;

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmChangeUser.xaml
	/// </summary>
	public partial class FrmChangeUser : Window
	{
		UserList usr;

		public FrmChangeUser(DataSet parent, string user)
		{
			InitializeComponent();
			usr = new UserList(parent);
			try
			{
				usr.SelectUser(user);
				tbNewUsername.Text = user;
			}
			catch(ArgumentOutOfRangeException ex)
			{
				MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void OldPasswordChanged(object sender, RoutedEventArgs e)
		{
			if(Encryption.CreateHash(tbOldPassword.Password) == usr.PasswordHash)
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
			this.Close();
		}

		private void ChangeUser(object sender, RoutedEventArgs e)
		{
			tbNewUsername.Background = Brushes.White;
			tbNewUsername.Foreground = Brushes.Black;
			tbNewPassword.Background = Brushes.White;
			tbNewPassword.Foreground = Brushes.Black;

				try
				{
					usr.ChangeUserName(tbNewUsername.Text);
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
							usr.SelectUser(tbNewUsername.Text);
							usr.ChangePassword(tbNewPassword.Password);
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

				this.Close();
		}
	}
}
