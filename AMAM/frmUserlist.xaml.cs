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
	public partial class FrmUserlist : Window
	{
        private string xmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AMAM\\users.xml");
		DataSet ds = new DataSet();

		public FrmUserlist()
		{
			InitializeComponent();
            XMLInitializer initxml = new XMLInitializer();
            if(initxml.XMLStructureInitialized("users"))
            {
                
                XmlReader xmlFile = XmlReader.Create(xmlPath, new XmlReaderSettings());
				try
				{
					ds.ReadXml(xmlFile);
					xmlFile.Close();
					lvUsers.DataContext = ds.Tables["user"].DefaultView;
				}
				catch(XmlException ex)
				{
					MessageBox.Show("Die Benutzerdatenbank ist korrupt. Es wird nun eine neue Benutzerdatenbank angelegt." + Environment.NewLine + Environment.NewLine + "Die korrupte Benutzerdatenbank wird gesichert und der Administrator per eMail kontaktiert. Bitte einen Moment Geduld.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);


					File.Move(xmlPath, xmlPath + ".corrupt");
					File.Delete(xmlPath);
					initxml.CreateXMLFileStructure("users");

					ExceptionReporter ExReport = new ExceptionReporter(ex);
					ExReport.ReportExceptionToAdmin(xmlPath + ".corrupt");
				}
            }

		}

		private void AddUser(object sender, RoutedEventArgs e)
		{
			FrmAddUser frmAdd = new FrmAddUser(ds);
			frmAdd.ShowDialog();
			
            ds.WriteXml(xmlPath);
		}

        private void DeleteUser(object sender, RoutedEventArgs e)
        {
			FrmDeleteUser frmDelete = new FrmDeleteUser(ds, lvUsers.SelectedValue.ToString());
			frmDelete.ShowDialog();
			ds.WriteXml(xmlPath);
        }

		private void Close(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

        private void SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(lvUsers.SelectedItem != null)
            {
                switch(lvUsers.Items.Count)
                {
                    case 0:
                        btnChange.IsEnabled = false;
                        btnRemove.IsEnabled = false;
                        break;
                    case 1:
                        btnChange.IsEnabled = true;
                        btnRemove.IsEnabled = false;
                        break;
                    default:
                        btnChange.IsEnabled = true;
                        btnRemove.IsEnabled = true;
                        break;
                }
            }
            else
            {
                btnChange.IsEnabled = false;
                btnRemove.IsEnabled = false;
            }
        }

		private void ChangeUser(object sender, RoutedEventArgs e)
		{
			FrmChangeUser frmChange = new FrmChangeUser(ds, lvUsers.SelectedValue.ToString());
			frmChange.ShowDialog();
			lvUsers.SelectedIndex = 0;
			ds.WriteXml(xmlPath);
		}

	}

}