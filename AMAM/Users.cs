using System;
using System.Data;

namespace Amam
{
    /// <summary>
    /// Klasse, die einen Nutzer darstellt
    /// </summary>
    class UserList : IDisposable
    {
        //Private Variablen für die Wertübergabe innerhalb der Klasse
        #region Klassenweite Variablen

            private string _username;
            private string _passwordHash;
            private readonly DataSet _ds;

        #endregion

		public void Dispose()
			{
				_ds.Dispose();
			}

        /// <summary>
        /// Gibt den Benutzernamen zurück
        /// </summary>
        public string Username
        {
            get
            {
                if(_username != null)
                {
                    return _username;
                }
                throw new ArgumentNullException(Username, @"Sie müssen zuerst über die SelectUser-Methode einen Benutzer selektieren.");
            }
        }

        /// <summary>
        /// Gibt den Hash des Passwortes zurück
        /// </summary>
        public string PasswordHash
        {
            get
            {
                if(_passwordHash != null)
                {
                    return _passwordHash;
                }
                throw new ArgumentNullException(PasswordHash, @"Sie müssen zuerst über die SelectUser-Methode einen Benutzer selektieren.");
            }
        }

        /// <summary>
        /// Erstellt ein neues Objekt vom Typ 'User'
        /// </summary>
        /// <param name="ds"></param>
        public UserList(DataSet ds)
        {
            _ds = ds;
        }

        /// <summary>
        /// Selektiert einen Benutzer der Benutzerliste, sodass er bearbeitet werden kann
        /// </summary>
        /// <param name="username">Übergibt den Benutzer, der selektiert werden soll</param>
        public void SelectUser(string username)
        {
            DataRow[] foundRows = _ds.Tables["user"].Select("username = '" + username + "'");
            if(foundRows.Length == 0)
            {
				throw new ArgumentOutOfRangeException("Der Benutzer " + username + " existiert nicht.");
            }
            if(foundRows.Length == 1)
            {
				DataRow row = foundRows[0];
				_username = row["username"].ToString();
				_passwordHash = row["password"].ToString();
            }
			if(foundRows.Length > 1)
			{
				throw new ArgumentOutOfRangeException(username, @"Der angegebene Benutzername ist mehrfach vorhanden.");
			}
        }

        /// <summary>
        /// Methode zum Erzeugen eines neuen Nutzers
        /// </summary>
        /// <param name="username">Übergibt den Benutzernamen</param>
        /// <param name="password">Übergibt das Passwort</param>
        public void CreateUser(string username, string password)
        {
            if(username.Length != 0 & password.Length != 0)
            {
                if(Validator.IsXmlValid(username))
                {
                    if(_ds.Tables.Count == 0)
                    {
                        _ds.Tables.Add("user");
                        _ds.Tables["user"].Columns.Add("username");
                        _ds.Tables["user"].Columns.Add("password");
                    }

                    DataRow[] foundRows =  _ds.Tables["user"].Select("username = '" + username + "'"); 

                    if(foundRows.Length != 0)
                    {
                        throw new ArgumentException("Der Benutzername ist schon vorhanden.");
                    }
                    DataRow newRow = _ds.Tables["user"].NewRow();
                    newRow["username"] = username;
                    _username = username;
                    _passwordHash = Encryption.CreateHash(password);
                    newRow["password"] = _passwordHash;
                    _ds.Tables["user"].Rows.Add(newRow);
                }
                else
                {
                    throw new ArgumentException("Der Benutzername enthält ein ungültiges Zeichen.");
                }
            }
			if(username.Length == 0 & password.Length != 0)
			{
                    throw new UserNameIsNullOrEmptyException("Es muss ein Benutzername angegeben werden.");
            }
            if(username.Length != 0 & password.Length == 0)
            {
                throw new PasswordIsNullOrEmptyException("Es muss ein Passwort angegeben werden.");
            }
            if(username.Length == 0 & password.Length == 0)
            {
                throw new UserNameIsNullOrEmptyException("Es muss ein Benutzername angegeben werden.");
            }
        }

        /// <summary>
        /// Methode zum Ändern des Benutzernamens
        /// </summary>
        /// <param name="newUsername">Übergibt den neuen Benutzernamen</param>
        public void ChangeUserName(string newUsername)
        {
			if(Validator.IsXmlValid(newUsername))
			{
				if(newUsername.Length != 0)
				{
					if(_username.Length > 0)
					{
						DataRow[] foundRows = _ds.Tables["user"].Select("username = '" + _username + "'");
						if(foundRows.Length == 0)
						{
							throw new ArgumentOutOfRangeException("Der Benutzer " + _username + " existiert nicht.");
						}
						if(foundRows.Length == 1)
						{
							DataRow row = foundRows[0];
							row["username"] = newUsername;
						}
						if(foundRows.Length > 1)
						{
							throw new ArgumentOutOfRangeException(newUsername, @"Der angegebene Benutzername ist mehrfach vorhanden.");
						}
					}
					else
					{
						throw new ArgumentNullException(newUsername, @"Sie müssen erst die SelectUser-Methode aufrufen um einen Benutzer aus der Benutzerliste zu wählen.");
					}
				}
				else
				{
					throw new UserNameIsNullOrEmptyException("Es muss ein Benutzername angegeben werden.");
				}
			}
			else
			{
				 throw new ArgumentException("Der Benutzername enthält ein ungültiges Zeichen.");
			}
        }

        /// <summary>
        /// Funktion zum Ändern des Passwortes
        /// </summary>
        /// <param name="newPassword">Übergibt das neue Passwort</param>
        public void ChangePassword(string newPassword)
        {
			if(newPassword.Length != 0)
			{
				if(_username.Length > 0)
				{
					DataRow[] foundRows = _ds.Tables["user"].Select("username = '" + _username + "'");
					if(foundRows.Length == 0)
					{
						throw new ArgumentOutOfRangeException("Der Benutzer " + _username + " existiert nicht.");
					}
					if(foundRows.Length == 1)
					{
						string passwordHash = Encryption.CreateHash(newPassword);
						DataRow row = foundRows[0];
						row["password"] = passwordHash;
					}
					if(foundRows.Length > 1)
					{
						throw new ArgumentOutOfRangeException(newPassword,@"Der angegebene Benutzername ist mehrfach vorhanden.");

					}
				}
				else
				{
					throw new ArgumentNullException(newPassword, @"Sie müssen erst die SelectUser-Methode aufrufen um einen Benutzer aus der Benutzerliste zu wählen.");
				}
			}
			else
			{
				throw new PasswordIsNullOrEmptyException("Es muss ein Passwort angegeben werden.");
			}
        }

        /// <summary>
        /// Löscht den selektierten Benutzer
        /// </summary>
        public void DeleteUser()
        {
			if(_username.Length > 0)
			{
				DataRow[] foundRows = _ds.Tables["user"].Select("username = '" + _username + "'");
				if(foundRows.Length == 0)
				{
					throw new ArgumentOutOfRangeException("Der Benutzer " + _username + " existiert nicht.");
				}
				if(foundRows.Length == 1)
				{
					DataRow row = foundRows[0];
					row.Delete();
				}
				if(foundRows.Length > 1)
				{
					throw new ArgumentOutOfRangeException(_username, @"Der angegebene Benutzer ist mehrfach vorhanden.");
				}
			}
			else
			{
				throw new ArgumentNullException(_username, @"Sie müssen erst die SelectUser-Methode aufrufen um einen Benutzer aus der Benutzerliste zu wählen.");
			}
        }

    }

}
