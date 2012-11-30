using System.Windows;

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmMain.xaml
	/// </summary>
	public partial class FrmMain
	{
		public FrmMain()
		{
			InitializeComponent();
		}

		private void Close(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void ShowUserManager(object sender, RoutedEventArgs e)
		{
			var userManager = new FrmUserlist();
			userManager.ShowDialog();
		}

		private void ShowDealerManager(object sender, RoutedEventArgs e)
		{
			var dealerManager = new FrmDealersList();
			dealerManager.ShowDialog();
		}

		private void ShowProductManager(object sender, RoutedEventArgs e)
		{
			var productManager = new FrmProductList();
			productManager.ShowDialog();
		}
	}
}
