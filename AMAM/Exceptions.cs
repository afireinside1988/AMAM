using System;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Runtime.Serialization;

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
        private string _ExceptionMessage;
        private string _ReportingApplication;
        private string _ReportingMachine;

        public ExceptionReporter(Exception ex)
        {
            if(ex != null)
            {
            _ExceptionMessage = ex.Message;
            _ReportingApplication = ex.Source + " in " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            _ReportingMachine = Environment.MachineName;
            }
        }

        /// <summary>
        /// Methode zum Versenden eines Fehlerberichtes per eMail
        /// </summary>
        /// <param name="attachmentPath">Optional kann eine Datei an die eMail angehangen werden</param>
        public void ReportExceptionToAdmin(string attachmentPath)
        {
            string Admin = Amam.Properties.Resources.AdminMail;
            MailMessage Mail = new MailMessage(Amam.Properties.Resources.HostMailAdress, Admin);
            Mail.Subject = _ReportingApplication + " auf " + _ReportingMachine + " hat einen Fehler verursacht";
            Mail.Body = _ExceptionMessage;
            if(!(attachmentPath == null))
            {
                Mail.Attachments.Add(new Attachment(attachmentPath));
                MessageBox.Show("Es konnte keine Datei an die eMail angefügt werden. Die eMail wird ohne Anhang versendet.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        ///  Methode zum Versenden eines Fehlerberichtes per eMail
        /// </summary>
        public void ReportExceptionToAdmin()
        {
			Validator Validate = new Validator();
			if(Validate.IsMailValid(Amam.Properties.Resources.AdminMail) & Validate.IsMailValid(Amam.Properties.Resources.HostMailAdress))
			{
				string Admin = Amam.Properties.Resources.AdminMail;
				MailMessage Mail = new MailMessage(Amam.Properties.Resources.HostMailAdress, Admin);
				Mail.Subject = _ReportingApplication + " auf " + _ReportingMachine + " hat einen Fehler verursacht";
				Mail.Body = _ExceptionMessage;

				SmtpClient client = new SmtpClient();
				client.Host = Amam.Properties.Resources.Host;
				NetworkCredential AuthenticationCredentials = new NetworkCredential(Amam.Properties.Resources.HostMailAdress, Amam.Properties.Resources.HostMailPassword);
				client.Credentials = AuthenticationCredentials;
				client.Send(Mail);
			}
        }
    }
}
