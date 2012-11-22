using System.Data;
using System.Windows;

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmConfirmDelete.xaml
	/// </summary>
	public partial class FrmDeleteUser : Window
	{
		DataSet ds;
		string user;

		public FrmDeleteUser(DataSet parent, string selectedUser)
		{
			InitializeComponent();
			ds = parent;
			user = selectedUser;
		}

		private void PasswordChanged(object sender, RoutedEventArgs e)
		{
			if(tbPassword.Password.Length > 0)
			{
				btnDelete.IsEnabled = true;
			}
			else
			{
				btnDelete.IsEnabled = false;
			}
		}

		private void Close(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void DeleteUser(object sender, RoutedEventArgs e)
		{
			UserList usr = new UserList(ds);
			usr.SelectUser(user);
			usr.DeleteUser();
			this.Close();
		}

	}
}
