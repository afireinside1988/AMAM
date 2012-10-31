using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace AMAM
{
    /// <summary>
    /// Klasse, zur Intialisierung der XML-Datenbank
    /// </summary>
    class XMLInitializer
    {
        private string ApplicationDataPath; //Pfad zum AppData-Verzeichnis der Anwendung

        public XMLInitializer()
        {
            ApplicationDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AMAM\\");
        }


        /// <summary>
        /// Funktion, die Untersucht, ob die XML-Datei-Struktur im Anwendungspfad existiert und das Ergebnis als Wert vom Typ Boolean zurückgibt
        /// </summary>
        /// <param name="XMLFiles">String-Array, dass die Dateinamen ohne Datei-Endung beinhaltet</param>
        /// <returns>Gibt True zurück, wenn die Dateistruktur existiert, ansonsten False</returns>
        public Boolean XMLStructureInitialized(params string[] XMLFiles)
        {

            foreach(string XMLFile in XMLFiles)
            {
                if(File.Exists(ApplicationDataPath + XMLFile + ".xml"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Metohde, um die XML-DateiStruktur zu erstellen
        /// </summary>
        /// <param name="XMLFiles"></param>
        public void CreateXMLFileStructure(params string[] XMLFiles)
        {
            foreach(string XMLFile in XMLFiles)
            {
                XmlDocument xmlFile = new XmlDocument();
                XmlDeclaration xmlDeclaration = xmlFile.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement RootNode = xmlFile.CreateElement(XMLFile);

                xmlFile.InsertBefore(xmlDeclaration, xmlFile.DocumentElement);
                xmlFile.AppendChild(RootNode);

                if(!Directory.Exists(ApplicationDataPath))
                {
                    Directory.CreateDirectory(ApplicationDataPath);
                }

                xmlFile.Save(ApplicationDataPath + XMLFile + ".xml");
            }
        }

    }
}
