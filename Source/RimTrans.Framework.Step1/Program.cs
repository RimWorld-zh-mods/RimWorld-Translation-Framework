using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RimTrans.Framework.Step1 {
    class Program {
        static void Main(string[] args) {
            string configFile = "Config.xml";
            if (File.Exists(configFile)) {
                Generate(configFile);
            } else {
                Console.Write("[ERROR] Config.xml no found.");
                Console.ReadKey();
            }
        }

        static void Generate(string configFile) {
            XDocument configDoc = XDocument.Load(configFile);
            string frameworkPath = configDoc.Root.Element("FrameworkPath").Value;
            string modPath = configDoc.Root.Element("ModPath").Value;
            string language = configDoc.Root.Element("Language").Value;

            string defsFolder = Path.Combine(frameworkPath, "Defs");
            string vanillaNamesFolder = Path.Combine(defsFolder, "VanillaNames");
            string solidNamesFile = Path.Combine(vanillaNamesFolder, "SolidNames.xml");
            XDocument solidNamesDoc = XDocument.Load(solidNamesFile);
        }
    }
}
