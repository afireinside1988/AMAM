using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows;

namespace System.Security.Cryptography
{
    /// <summary>
    /// Klasse zur Erzeugung und Verarbeitung von Salted Hashs;
    /// </summary>
    class SaltedHashProvider
    {


        /// <summary>
        /// Eigenschaft, die den erzeugten Salt übergibt
        /// </summary>   
        private Int32 _Salt;
        public Int32 Salt
        {
            get { return _Salt; }
        }

        /// <summary>
        /// Eigenschaft, die den erzeugten SaltedHash übergibt
        /// </summary>
        private String _SaltedHash;
        public String SaltedHash 
        { 
            get { return _SaltedHash; } 
        }

        /// <summary>
        /// Erzeugt einen Salted Hash aus dem übergebenen Paramter
        /// </summary>
        /// <param name="value"></param>
        public void CreateSaltedHash(string value)
        {
            Random rnd = new Random(); //Zufallszahlengenerator initialisieren
            _Salt = rnd.Next(); //Salt aus einem zufälligen Integer generieren
            value += Convert.ToString(_Salt); //Salt an das Passwort anhängen
            
            Byte[] PlainTextArray = Encoding.UTF8.GetBytes(value);
            MD5CryptoServiceProvider Hasher = new MD5CryptoServiceProvider();
            Byte[] HashBytes;

            try
            {
                HashBytes = Hasher.ComputeHash(PlainTextArray);
                _SaltedHash = BitConverter.ToString(HashBytes).Replace("-","");
            }
            catch
                {
                    MessageBox.Show("Es ist ein Fehler bei der Verschlüsselung des Passwortes aufgetreten!" + Environment.NewLine + "Bitte versuchen Sie es mit einem anderen Passwort.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }

        /// <summary>
        /// Überprüft, ob ein String einem Salted Hash entspricht
        /// </summary>
        /// <param name="value">Übergibt den String, der auf Übereinstimmung mit dem Hash überprüft werden soll</param>
        /// <param name="Salt">Übergibt den Salt</param>
        /// <param name="Hash">Übergibt den Hash für den Vergleich</param>
        /// <returns></returns>
        public Boolean IsValid(String value, Int32 Salt, String Hash)
        {
            Byte[] PlainTextArray = Encoding.UTF8.GetBytes(value + Convert.ToString(Salt));
            MD5CryptoServiceProvider Hasher = new MD5CryptoServiceProvider();
            Byte[] HashBytes;

            try
            {
                HashBytes = Hasher.ComputeHash(PlainTextArray);
                if(Hash == BitConverter.ToString(HashBytes).Replace("-", "")) return true;
                else return false;
            }
            catch
            {
                MessageBox.Show("Beim Verschlüsseln des Passwortes ist ein Fehler aufgetreten." + Environment.NewLine + "Bitte versuchen Sie ein anderes Passwort.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }
    }
}
