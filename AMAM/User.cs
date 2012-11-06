using System;
using System.Windows;
using System.Xml;
using System.Data;
using System.Security;
using System.Security.Cryptography;
using System.IO;

namespace Amam
{
    /// <summary>
    /// Klasse, die einen Nutzer darstellt
    /// </summary>
    class UserList : IDisposable
    {
        //Private Variablen für die Wertübergabe innerhalb der Klasse
        #region Klassenweite Variablen

            private string _Username;
            private string _PasswordHash;
            private string _pathToUserList;
            private DataSet ds = new DataSet();

        #endregion

			public void Dispose()
			{
				ds.Dispose();
			}

        /// <summary>
        /// Gibt den Benutzernamen zurück
        /// </summary>
        public string Username
        {
            get
            {
                if(_Username.Length > 0)
                {
                    return _Username;
                }
                else
                {
                    throw new ArgumentNullException(Username, "Sie müssen zuerst über die SelectUser-Methode einen Benutzer selektieren.");
                }
            }
        }

        /// <summary>
        /// Gibt den Hash des Passwortes zurück
        /// </summary>
        public string PasswordHash
        {
            get
            {
                if(_PasswordHash.Length > 0)
                {
                    return _PasswordHash;
                }
                else
                {
                    throw new ArgumentNullException(PasswordHash, "Sie müssen zuerst über die SelectUser-Methode einen Benutzer selektieren.");
                }
            }
        }

        /// <summary>
        /// Erstellt ein neues Objekt vom Typ 'User'
        /// </summary>
        /// <param name="pathToUserList">Gibt den Pfad der XML-Datei, die die Benutzerliste enthält an die Klasse weiter</param>
        public UserList(string pathToUserList)
        {
            _pathToUserList = pathToUserList;

            if(File.Exists(pathToUserList))
            {
                XmlReader xmlFile = XmlReader.Create(pathToUserList, new XmlReaderSettings());
                try
                {
                    ds.ReadXml(xmlFile);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Die Benutzerliste konnte nicht eingelesen werden.", ex);
                }
                xmlFile.Close();
            }
            else
            {
                throw new FileNotFoundException();
            }
            
        }

        /// <summary>
        /// Speichert den aktuellen Zustand der Benutzerliste
        /// </summary>
        public void SaveUserlist()
        {
            try
            {
                ds.WriteXml(_pathToUserList);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Die Benutzerliste konnte nicht gespeichert werden.", ex);
            }
        }

        /// <summary>
        /// Selektiert einen Benutzer der Benutzerliste, sodass er bearbeitet werden kann
        /// </summary>
        /// <param name="Username">Übergibt den Benutzer, der selektiert werden soll</param>
        public void SelectUser(string Username)
        {
            DataRow[] foundRows = ds.Tables["user"].Select("username = '" + Username + "'");
            if(foundRows.Length == 0)
            {
                throw new ArgumentOutOfRangeException("Der Benutzer " + Username + " existiert nicht.");
            }
            if(foundRows.Length == 1)
            {
				DataRow row = foundRows[0];
				_Username = row["username"].ToString();
				_PasswordHash = row["password"].ToString();
            }
			if(foundRows.Length > 1)
			{
				throw new ArgumentOutOfRangeException(Username, "Der angegebene Benutzername ist mehrfach vorhanden.");
			}
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
                if(Validator.IsXMLValid(Username))
                {
                    if(ds.Tables.Count == 0)
                    {
                        ds.Tables.Add("user");
                        ds.Tables["user"].Columns.Add("username");
                        ds.Tables["user"].Columns.Add("password");
                    }

                    DataRow[] foundRows =  ds.Tables["user"].Select("username = '" + Username + "'"); 

                    if(foundRows.Length != 0)
                    {
                        throw new ArgumentException("Der Benutzername ist schon vorhanden.");
                    }
                    else
                    {
                        DataRow newRow = ds.Tables["user"].NewRow();
                        newRow["username"] = Username;
                        _Username = Username;
						_PasswordHash = Encryption.CreateHash(Password);
                        newRow["password"] = _PasswordHash;
                        ds.Tables["user"].Rows.Add(newRow);
                    }
                }
                else
                {
                    throw new ArgumentException("Der Benutzername enthält ein ungültiges Zeichen.");
                }
            }

			if(Username.Length == 0 & Password.Length != 0)
			{
                    throw new UserNameIsNullOrEmptyException("Es muss ein Benutzername angegeben werden.");
            }
            if(Username.Length != 0 & Password.Length == 0)
            {
                throw new PasswordIsNullOrEmptyException("Es muss ein Passwort angegeben werden.");
            }
            if(Username.Length == 0 & Password.Length == 0)
            {
                throw new UserNameIsNullOrEmptyException("Es muss ein Benutzername angegeben werden.");
                throw new PasswordIsNullOrEmptyException("Es muss ein Passwort angegeben werden.");
            }
        }

        /// <summary>
        /// Methode zum Ändern des Benutzernamens
        /// </summary>
        /// <param name="NewUsername">Übergibt den neuen Benutzernamen</param>
        public void ChangeUserName(string NewUsername)
        {
			if(NewUsername.Length != 0)
			{
				if(_Username.Length > 0)
				{
					DataRow[] foundRows = ds.Tables["user"].Select("username = '" + _Username + "'");
					if(foundRows.Length == 0)
					{
						throw new ArgumentOutOfRangeException("Der Benutzer " + _Username + " existiert nicht.");
					}
					if(foundRows.Length == 1)
					{
						DataRow row = foundRows[0];
						row["username"] = NewUsername;
					}
					if(foundRows.Length > 1)
					{
						throw new ArgumentOutOfRangeException(NewUsername, "Der angegebene Benutzername ist mehrfach vorhanden.");
					}
				}
				else
				{
					throw new ArgumentNullException(NewUsername, "Sie müssen erst die SelectUser-Methode aufrufen um einen Benutzer aus der Benutzerliste zu wählen.");
				}
			}
			else
			{
				throw new ArgumentNullException(NewUsername, "Es muss ein Benutzername angegeben werden.");
			}
        }

        /// <summary>
        /// Funktion zum Ändern des Passwortes
        /// </summary>
        /// <param name="NewPassword">Übergibt das neue Passwort</param>
        public void ChangePassword(string NewPassword)
        {
			if(NewPassword.Length != 0)
			{
				if(_Username.Length > 0)
				{
					DataRow[] foundRows = ds.Tables["user"].Select("username = '" + _Username + "'");
					if(foundRows.Length == 0)
					{
						throw new ArgumentOutOfRangeException("Der Benutzer " + _Username + " existiert nicht.");
					}
					if(foundRows.Length == 1)
					{
						string PasswordHash = Encryption.CreateHash(NewPassword);
						DataRow row = foundRows[0];
						row["password"] = PasswordHash;
					}
					if(foundRows.Length > 1)
					{
						throw new ArgumentOutOfRangeException(NewPassword,"Der angegebene Benutzername ist mehrfach vorhanden.");

					}
				}
				else
				{
					throw new ArgumentNullException(NewPassword, "Sie müssen erst die SelectUser-Methode aufrufen um einen Benutzer aus der Benutzerliste zu wählen.");
				}
			}
			else
			{
				throw new ArgumentNullException(NewPassword, "Es muss ein Passwort angegeben werden.");
			}
        }

        /// <summary>
        /// Löscht den selektierten Benutzer
        /// </summary>
        public void DeleteUser()
        {
			if(_Username.Length > 0)
			{
				DataRow[] foundRows = ds.Tables["user"].Select("username = '" + _Username + "'");
				if(foundRows.Length == 0)
				{
					throw new ArgumentOutOfRangeException("Der Benutzer " + _Username + " existiert nicht.");
				}
				if(foundRows.Length == 1)
				{
					DataRow row = foundRows[0];
					row.Delete();
				}
				if(foundRows.Length > 1)
				{
					throw new ArgumentOutOfRangeException(_Username, "Der angegebene Benutzer ist mehrfach vorhanden.");
				}
			}
			else
			{
				throw new ArgumentNullException(_Username, "Sie müssen erst die SelectUser-Methode aufrufen um einen Benutzer aus der Benutzerliste zu wählen.");
			}
        }

    }
}
