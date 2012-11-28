﻿using System.Text;
using System.Windows;

namespace System.Security.Cryptography
{
    /// <summary>
    /// Klasse zur Erzeugung und Verarbeitung von Salted Hashs;
    /// </summary>
    static class Encryption
    {

		/// <summary>
        /// Erzeugt einen Salted Hash aus dem übergebenen Paramter
        /// </summary>
        /// <param name="value"></param>
		/// <returns>Gibt den aus value erzeugten Hash wieder</returns>
		public static string CreateHash(string value)
		{
			Byte[] PlainTextArray = Encoding.UTF8.GetBytes(value);
			MD5CryptoServiceProvider Hasher = new MD5CryptoServiceProvider();
			Byte[] HashBytes;

			HashBytes = Hasher.ComputeHash(PlainTextArray);
			return BitConverter.ToString(HashBytes).Replace("-", "");
		}
    }
}
