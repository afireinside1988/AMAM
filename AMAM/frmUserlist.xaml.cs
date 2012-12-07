using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Xml;

namespace Amam
{

	/// <summary>
	/// Interaktionslogik für frmUserlist.xaml
	/// </summary>
	public partial class FrmUserlist
	{
// ReSharper disable PossiblyMistakenUseOfParamsMethod
        private readonly string _xmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AMAM\\users.xml");
// ReSharper restore PossiblyMistakenUseOfParamsMethod
	    readonly DataSet _ds = new DataSet();

		public FrmUserlist()
		{
			InitializeComponent();
            var initxml = new XMLInitializer();
            if(initxml.XMLStructureInitialized("users"))
            {
                
                XmlReader xmlFile = XmlReader.Create(_xmlPath, new XmlReaderSettings());
				try
				{
					_ds.ReadXml(xmlFile);
					xmlFile.Close();
					LvUsers.DataContext = _ds.Tables["user"].DefaultView;
				}
				catch(XmlException ex)
				{
					MessageBox.Show("Die Benutzerdatenbank ist korrupt. Es wird nun eine neue Benutzerdatenbank angelegt." + Environment.NewLine + Environment.NewLine + "Die korrupte Benutzerdatenbank wird gesichert und der Administrator per eMail kontaktiert. Bitte einen Moment Geduld.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);


					File.Move(_xmlPath, _xmlPath + ".corrupt");
					File.Delete(_xmlPath);
					initxml.CreateXMLFileStructure("users");

					var exReport = new ExceptionReporter(ex);
					exReport.ReportExceptionToAdmin(_xmlPath + ".corrupt");
				}
            }

		}

		private void AddUser(object sender, RoutedEventArgs e)
		{
			var frmAdd = new FrmAddUser(_ds);
			frmAdd.ShowDialog();
			
            _ds.WriteXml(_xmlPath);
		}

        private void DeleteUser(object sender, RoutedEventArgs e)
        {
			var frmDelete = new FrmDeleteUser(_ds, LvUsers.SelectedValue.ToString());
			frmDelete.ShowDialog();
			_ds.WriteXml(_xmlPath);
        }

		private void Close(object sender, RoutedEventArgs e)
		{
			Close();
		}

        private void SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(LvUsers.SelectedItem != null)
            {
                switch(LvUsers.Items.Count)
                {
                    case 0:
                        BtnChange.IsEnabled = false;
                        BtnRemove.IsEnabled = false;
                        break;
                    case 1:
                        BtnChange.IsEnabled = true;
                        BtnRemove.IsEnabled = false;
                        break;
                    default:
                        BtnChange.IsEnabled = true;
                        BtnRemove.IsEnabled = true;
                        break;
                }
            }
            else
            {
                BtnChange.IsEnabled = false;
                BtnRemove.IsEnabled = false;
            }
        }

		private void ChangeUser(object sender, RoutedEventArgs e)
		{
			var frmChange = new FrmChangeUser(_ds, LvUsers.SelectedValue.ToString());
			frmChange.ShowDialog();
			LvUsers.SelectedIndex = 0;
			_ds.WriteXml(_xmlPath);
		}

	}

}