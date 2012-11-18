using System.Windows;
using System.IO;
using System;
using System.Data;
using System.Xml;
using System.Collections.ObjectModel;

namespace Amam
{
	/// <summary>
	/// Interaktionslogik für frmUserlist.xaml
	/// </summary>
	public partial class frmUserlist : Window
	{
        private string xmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AMAM\\users.xml");
		DataSet ds = new DataSet();

		public frmUserlist()
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
					lvUsers.ItemsSource = ds.Tables["user"].DefaultView;
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
				btnChange.IsEnabled = true;
				btnRemove.IsEnabled = true;
			}
			else
			{
				btnChange.IsEnabled = false;
				btnRemove.IsEnabled = false;
			}
		}

		private void ChangeUser(object sender, RoutedEventArgs e)
		{
			frmChangeUser frmChange = new frmChangeUser(ds, lvUsers.SelectedValue.ToString());
			frmChange.ShowDialog();
			ds.WriteXml(xmlPath);
		}
	}
}
