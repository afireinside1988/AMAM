using System;
using System.Security.Cryptography;
using System.Text;

namespace Amam
{
	/// <summary>
	///     Klasse zur Erzeugung und Verarbeitung von Salted Hashs;
	/// </summary>
	internal static class Encryption
	{
		/// <summary>
		///     Erzeugt einen Salted Hash aus dem übergebenen Paramter
		/// </summary>
		/// <param name="value"></param>
		/// <returns>Gibt den aus value erzeugten Hash wieder</returns>
		public static string CreateHash(string value)
		{
			Byte[] plainTextArray = Encoding.UTF8.GetBytes(value);
			var hasher = new MD5CryptoServiceProvider();

			byte[] hashBytes = hasher.ComputeHash(plainTextArray);
			return BitConverter.ToString(hashBytes).Replace("-", "");
		}
	}
}
