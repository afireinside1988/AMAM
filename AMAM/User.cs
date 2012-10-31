using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Data;

namespace AMAM
{
    /// <summary>
    /// Klasse, die einen Nutzer darstellt
    /// </summary>
    class User
    {
        //Private Variablen für die Wertübergabe innerhalb der Klasse
        private string PathToUserlist;
        private string _Username;
        private string _PasswordHash;
        private Int32 _Salt;
        private System.Security.Cryptography.SaltedHashProvider HashProvider = new System.Security.Cryptography.SaltedHashProvider();

        /// <summary>
        /// Gibt den Benutzernamen zurück
        /// </summary>
        public string Username
        {
            get
            {
                return _Username;
            }
        }

        /// <summary>
        /// Gibt den Hash des Passwortes zurück
        /// </summary>
        public string PasswordHash
        {
            get
            {
                return _PasswordHash;
            }
        }

        /// <summary>
        /// Gibt den Salt zurück
        /// </summary>
        public Int32 Salt
        {
            get
            {
                return _Salt;
            }
        }

        /// <summary>
        /// Erstellt ein neues Objekt vom Typ 'User'
        /// </summary>
        /// <param name="XmlPath">Gibt den Pfad der XML-Datei, die die Benutzerliste enthält an die Klasse weiter</param>
        public User(string XmlPath)
        {
            PathToUserlist = XmlPath;
        }

        /// <summary>
        /// Methode zum Erzeugen eines neuen Nutzers
        /// </summary>
        /// <param name="Username">Übergibt den Benutzernamen</param>
        /// <param name="Password">Übergibt das Passwort</param>
        public void CreateUser(string Username, string Password)
        {
            if(Username.Length != 0 & Password.Length != 0)
            {
                //TODO Funktion zum Hinzufügen eines neuen Benutzers || Muss noch getestet werden
                Validator validate = new Validator();
                if(!validate.IsXMLValid(Username))
                {
                    MessageBox.Show("Sie haben im Benutzernamen ein ungültiges Zeichen verwendet!\nDer Benutzername darf folgende Zeichen nicht enthalten:\n\n& < > ' »", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    XmlReader xmlFile = XmlReader.Create(PathToUserlist, new XmlReaderSettings());
                    DataSet ds = new DataSet();
                    ds.ReadXml(xmlFile);
                    xmlFile.Close();
                    if(ds.Tables.Count == 0)
                    {
                        ds.Tables.Add("user");
                        ds.Tables["user"].Columns.Add("username");
                        ds.Tables["user"].Columns.Add("salt");
                        ds.Tables["user"].Columns.Add("password");
                    }
                    DataRow[] foundRows =  ds.Tables["user"].Select("username = '" + Username + "'"); 
                    if(foundRows.Length != 0)
                    {
                        MessageBox.Show("Der Benutzername existiert schon!\nBitte wählen Sie einen anderen Benutzernamen.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        DataRow newRow = ds.Tables["user"].NewRow();
                        newRow["username"] = Username;
                        HashProvider.CreateSaltedHash(Password);
                        newRow["salt"] = Convert.ToString(HashProvider.Salt);
                        newRow["password"] = HashProvider.SaltedHash;
                        ds.Tables["user"].Rows.Add(newRow);
                        ds.WriteXml(PathToUserlist);
                    }
                }

            }
            if(Username.Length != 0 & Password.Length == 0)
            {
                MessageBox.Show("Sie müssen ein Passwort eingeben!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if(Username.Length == 0 & Password.Length != 0)
            {
                MessageBox.Show("Sie müssen einen Benutzernamen eingeben!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if(Username.Length == 0 & Password.Length == 0)
            {
                MessageBox.Show("Sie müssen einen Benutzernamen und ein Passwort eingeben!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Methode zum Ändern des Benutzernamens
        /// </summary>
        /// <param name="Username">Übergibt den Benutzernamen</param>
        /// <param name="Password">Übergibt das Passwort</param>
        public void ChangeUserName(string Username, string Password)
        {
        }

        /// <summary>
        /// Funktion zum Ändern des Passwortes
        /// </summary>
        /// <param name="Username">Übergibt den Benutzernamen, für den das Passwort geändert werden soll</param>
        /// <param name="OldPassword">Übergibt das alte Passwort</param>
        /// <param name="NewPassword">Übergibt das neue Passwort</param>
        /// <returns>Gibt True zurück, wenn das Ändern des Passwortes erfolgreich war</returns>
        public bool ChangePassword(string Username, string OldPassword, string NewPassword)
        {
            return false;
        }

        /// <summary>
        /// Methode zum Löschen eines Benutzers
        /// </summary>
        /// <param name="Username">Übergibt den Benutzernamen</param>
        /// <param name="Password">Übergibt das Passwort</param>
        public void DeleteUser(string Username, string Password)
        {

        }

    }
}
