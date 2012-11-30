using System;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace Amam
{
    /// <summary>
    /// Ausnahme, die Ausgelöst wird, wenn ein Ausdruck ungültig ist
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors"), Serializable]
    public class ArgumentInvalidException : ArgumentException
    {  
        public ArgumentInvalidException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Ausnahme, die Ausgelöst wird, wenn das Passwort Null ist
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors"), Serializable]
    public class PasswordIsNullOrEmptyException : ArgumentNullException
    {
        public PasswordIsNullOrEmptyException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Ausnahme, die Ausgelöst wird, wenn der Benutzername Null ist
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors"), Serializable]
    public class UserNameIsNullOrEmptyException : ArgumentNullException
    {
        public UserNameIsNullOrEmptyException(string message) : base(message)
        {
        }
    }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors"), Serializable]
	public class SqlServerNotFoundException : Exception
	{
		public SqlServerNotFoundException(string message) : base(message)
		{
		}
	}

    /// <summary>
    /// Die Klasse stellt einen Reporting-Dienst zur Verfügung, der Ausnahmen per Mail an den Administrator übermittelt
    /// </summary>
    public class ExceptionReporter
    {
        private readonly string _exceptionMessage;
        private readonly string _reportingApplication;
        private readonly string _reportingMachine;

        public ExceptionReporter(Exception ex)
        {
            if(ex != null)
            {
            _exceptionMessage = ex.Message;
            _reportingApplication = ex.Source + " in " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            _reportingMachine = Environment.MachineName;
            }
        }

        /// <summary>
        /// Methode zum Versenden eines Fehlerberichtes per eMail
        /// </summary>
        /// <param name="attachmentPath">Optional kann eine Datei an die eMail angehangen werden</param>
        public void ReportExceptionToAdmin(string attachmentPath)
        {
            string admin = Properties.Resources.AdminMail;
            var mail = new MailMessage(Properties.Resources.HostMailAdress, admin)
                {
                    Subject = _reportingApplication + " auf " + _reportingMachine + " hat einen Fehler verursacht",
                    Body = _exceptionMessage
                };
            if(attachmentPath != null)
            {
                mail.Attachments.Add(new Attachment(attachmentPath));
                MessageBox.Show("Es konnte keine Datei an die eMail angefügt werden. Die eMail wird ohne Anhang versendet.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        ///  Methode zum Versenden eines Fehlerberichtes per eMail
        /// </summary>
        public void ReportExceptionToAdmin()
        {
			var validate = new Validator();
			if(validate.IsMailValid(Properties.Resources.AdminMail) & validate.IsMailValid(Properties.Resources.HostMailAdress))
			{
				string admin = Properties.Resources.AdminMail;
				var mail = new MailMessage(Properties.Resources.HostMailAdress, admin)
				    {
				        Subject = _reportingApplication + " auf " + _reportingMachine + " hat einen Fehler verursacht",
				        Body = _exceptionMessage
				    };

			    var client = new SmtpClient {Host = Properties.Resources.Host};
			    var authenticationCredentials = new NetworkCredential(Properties.Resources.HostMailAdress, Properties.Resources.HostMailPassword);
				client.Credentials = authenticationCredentials;
				client.Send(mail);
			}
        }
    }
}
