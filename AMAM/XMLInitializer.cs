using System;
using System.IO;
using System.Xml.Linq;

namespace Amam
{
    /// <summary>
    /// Klasse, zur Intialisierung der XML-Datenbank
    /// </summary>
    class XMLInitializer
    {
        private readonly string _applicationDataPath; //Pfad zum AppData-Verzeichnis der Anwendung

        public XMLInitializer()
        {
            _applicationDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AMAM\\");
        }

        /// <summary>
        /// Funktion, die Untersucht, ob die XML-Datei-Struktur im Anwendungspfad existiert und das Ergebnis als Wert vom Typ Boolean zurückgibt
        /// </summary>
        /// <param name="xmlFiles">String-Array, dass die Dateinamen ohne Datei-Endung beinhaltet</param>
        /// <returns>Gibt True zurück, wenn die Dateistruktur existiert, ansonsten False</returns>
        public Boolean XMLStructureInitialized(params string[] xmlFiles)
        {

            foreach(string xmlFile in xmlFiles)
            {
                if(File.Exists(_applicationDataPath + xmlFile + ".xml"))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Metohde, um die XML-DateiStruktur zu erstellen
        /// </summary>
        /// <param name="xmlFiles"></param>
        public void CreateXMLFileStructure(params string[] xmlFiles)
        {
            foreach(string xmlFile in xmlFiles)
            {
                var xmlDoc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement(xmlFile)
                );

                if(!Directory.Exists(_applicationDataPath))
                {
                    Directory.CreateDirectory(_applicationDataPath);
                }
                xmlDoc.Save(_applicationDataPath + xmlFile + ".xml");
            }
        }

    }
}
