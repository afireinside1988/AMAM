using System.Data;
using System.Windows;

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmConfirmDelete.xaml
	/// </summary>
	public partial class FrmDeleteUser
	{
	    readonly DataSet _ds;
	    readonly string _user;

		public FrmDeleteUser(DataSet parent, string selectedUser)
		{
			InitializeComponent();
			_ds = parent;
			_user = selectedUser;
		}

		private void PasswordChanged(object sender, RoutedEventArgs e)
		{
		    btnDelete.IsEnabled = tbPassword.Password.Length > 0;
		}

	    private void Close(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void DeleteUser(object sender, RoutedEventArgs e)
		{
			var usr = new UserList(_ds);
			usr.SelectUser(_user);
			usr.DeleteUser();
			Close();
		}

	}
}
