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
using System.Data;
using System.Security.Cryptography;

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmConfirmDelete.xaml
	/// </summary>
	public partial class FrmDeleteUser : Window
	{
		DataSet ds;
		string user;

		public FrmDeleteUser(DataSet Parent, string SelectedUser)
		{
			InitializeComponent();
			ds = Parent;
			user = SelectedUser;
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
