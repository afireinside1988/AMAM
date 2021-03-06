﻿using System;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Amam
{
    class Validator
    {
        /// <summary>
        /// Variable, die die Validität einer eMail-Adresse zurückgibt
        /// </summary>
        private bool _boolMailInvalid;

        /// <summary>
        /// Überprüft einen String auf Zeichen, die ein XML-Dokument nicht enthalten darf
        /// </summary>
        /// <param name="value">Übergibt den String, der auf XML-Validität geprüft werden soll</param>
        /// <returns>Gibt True zurück, wenn der String XML-valide ist</returns>
        public static bool IsXmlValid(string value)
        {
            if(value.Contains("&") || value.Contains("<") || value.Contains(">") || value.Contains("'") || value.Contains("»"))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Überprüft einen String darauf, ob es sich um eine gültige eMail-Adresse handelt.
        /// </summary>
        /// <param name="mailAdress">Übergibt den String, der die eMail-Adresse enthält</param>
        /// <returns>Gibt True zurück, wenn es sich bei dem String um eine gültige eMail-Adresse handelt</returns>
        public bool IsMailValid(string mailAdress)
        {
            if(string.IsNullOrEmpty(mailAdress)) return false;

			mailAdress = Regex.Replace(mailAdress, "(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

            if(_boolMailInvalid) return false;
            
			return Regex.IsMatch(mailAdress, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

		public static string FormatPrice(string value)
		{
			value = value.Replace(".", ",");
			if(value.EndsWith("€"))
			{
				value = value.TrimEnd(Convert.ToChar("€"));
			}
			if(!value.Contains(","))
			{
				if(Regex.IsMatch(value, "[0-9]"))
				{
					value += ",00";
					return value;
				}
			}
			if(Regex.IsMatch(value, "[0-9]+(,)[0-9]{0,2}"))
			{
			    string[] cent = value.Split(Convert.ToChar(","));
				if(cent[1].Length < 2)
				{
					cent[1] += "0";
					value = cent[0] + "," + cent[1];
				}
				return value;
			}
		    throw new ArgumentInvalidException("Bitte geben Sie einen gültigen Preis ein.");
		}
        
        private string DomainMapper(Match match)
        {
            var idn = new IdnMapping();
            string domainName = match.Groups[2].Value;

            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
                {
                    _boolMailInvalid = false;
                }

                return match.Groups[1].Value+domainName;
        }
    }
}
