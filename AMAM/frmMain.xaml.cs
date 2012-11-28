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

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmMain.xaml
	/// </summary>
	public partial class FrmMain : Window
	{
		public FrmMain()
		{
			InitializeComponent();
		}

		private void Close(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void ShowUserManager(object sender, RoutedEventArgs e)
		{
			FrmUserlist UserManager = new FrmUserlist();
			UserManager.ShowDialog();
		}

		private void ShowDealerManager(object sender, RoutedEventArgs e)
		{
			FrmDealersList DealerManager = new FrmDealersList();
			DealerManager.ShowDialog();
		}

		private void ShowProductManager(object sender, RoutedEventArgs e)
		{
			FrmProductList ProductManager = new FrmProductList();
			ProductManager.ShowDialog();
		}
	}
}
