using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace AMAM
{
    class Validator
    {
        /// <summary>
        /// Variable, die die Validität einer eMail-Adresse zurückgibt
        /// </summary>
        private bool boolMailInvalid = false;

        /// <summary>
        /// Überprüft einen String auf Zeichen, die ein XML-Dokument nicht enthalten darf
        /// </summary>
        /// <param name="value">Übergibt den String, der auf XML-Validität geprüft werden soll</param>
        /// <returns>Gibt True zurück, wenn der String XML-valide ist</returns>
        public bool IsXMLValid(string value)
        {
            if(value.Contains("&") || value.Contains("<") || value.Contains(">") || value.Contains("'") || value.Contains("»"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Überprüft einen String darauf, ob es sich um eine gültige eMail-Adresse handelt.
        /// </summary>
        /// <param name="MailAdress">Übergibt den String, der die eMail-Adresse enthält</param>
        /// <returns>Gibt True zurück, wenn es sich bei dem String um eine gültige eMail-Adresse handelt</returns>
        public bool IsMailValid(string MailAdress)
        {
            if(string.IsNullOrEmpty(MailAdress)) return false;

            try
            {
                MailAdress = Regex.Replace(MailAdress, "(@)(.+)$", this.DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (Exception)
            {
                return false;
            }

            if(boolMailInvalid) return false;

            try
            {
                return Regex.IsMatch(MailAdress, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch(Exception)
            {
                return false;
            }

        }
        
        private string DomainMapper(Match match)
        {
            IdnMapping idn = new IdnMapping();
            string domainName = match.Groups[2].Value;

            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
                {
                    boolMailInvalid = false;
                }

                return match.Groups[1].Value+domainName;
        }
    }
}
