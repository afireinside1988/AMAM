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
				TbNewUsername.Text = user;
			}
			catch(ArgumentOutOfRangeException ex)
			{
				MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void OldPasswordChanged(object sender, RoutedEventArgs e)
		{
			if(Encryption.CreateHash(TbOldPassword.Password) == _usr.PasswordHash)
			{
				GbChangeUser.IsEnabled = true;
				BtnChange.IsEnabled = true;
			}
			else
			{
				GbChangeUser.IsEnabled = false;
				BtnChange.IsEnabled = false;
			}
		}

		private void Close(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void ChangeUser(object sender, RoutedEventArgs e)
		{
			TbNewUsername.Background = Brushes.White;
			TbNewUsername.Foreground = Brushes.Black;
			TbNewPassword.Background = Brushes.White;
			TbNewPassword.Foreground = Brushes.Black;

				try
				{
					_usr.ChangeUserName(TbNewUsername.Text);
				}
				catch(UserNameIsNullOrEmptyException ex)
				{
					MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
					TbNewUsername.Background = Brushes.Red;
					TbNewUsername.Foreground = Brushes.White;
					TbNewUsername.Focus();
					return;
				}
				catch(ArgumentException ex)
				{
					MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
					TbNewUsername.Background = Brushes.Red;
					TbNewUsername.Foreground = Brushes.White;
					TbNewUsername.Focus();
					return;
				}

				if(TbNewPassword.Password.Length > 0)
				{
					if(TbNewPassword.Password == TbNewPasswordConfirm.Password)
					{
						try
						{
							_usr.SelectUser(TbNewUsername.Text);
							_usr.ChangePassword(TbNewPassword.Password);
						}
						catch(PasswordIsNullOrEmptyException ex)
						{
							MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
							TbNewPassword.Background = Brushes.Red;
							TbNewPassword.Foreground = Brushes.White;
							TbNewPassword.Focus();
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
						TbNewPassword.Password = null;
						TbNewPasswordConfirm.Password = null;
						TbNewPassword.Focus();
						return;
					}
				}

				Close();
		}
	}
}
