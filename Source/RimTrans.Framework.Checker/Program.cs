using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RimTrans.Framework.Checker {
    class Program {
        static void Main(string[] args) {
            Console.OutputEncoding = Encoding.Unicode;
            string configFile = "Config.xml";
            if (File.Exists(configFile)) {
                XDocument configDoc = XDocument.Load(configFile);
                string frameworkPath = configDoc.Root.Element("FrameworkPath").Value;
                string modPath = configDoc.Root.Element("ModPath").Value;
                string language = configDoc.Root.Element("Language").Value;
                if (!Directory.Exists(modPath)) {
                    Console.Write($"[ERROR] '{modPath}' no found.");
                } else {
                    Check(frameworkPath, modPath, language);
                }
                Console.Write("\n[Completed] Press any key to exit.");
                Console.ReadKey();
            } else {
                Console.Write("[ERROR] Config.xml no found.");
                Console.ReadKey();
            }
        }

        static readonly HashSet<string> detected = new HashSet<string>();
        static readonly Regex validNameRegex = new Regex("^[\\-\\u4e00-\\u9fff\\uf900-\\ufaff]*$");
        static readonly Regex acronymsRegex = new Regex("^[A-Z]*$");

        static void Check(string frameworkPath, string modPath, string language) {
            string sourcePath = Path.Combine(modPath, "Source", language);
            Check_Map(sourcePath, "SolidNames_Last.xml");
            Check_Map(sourcePath, "SolidNames_First_Male.xml");
            Check_Map(sourcePath, "SolidNames_First_Female.xml");
            Check_Map(sourcePath, "SolidNames_First_Either.xml");
            Check_Unique(sourcePath, "SolidNames_Unique.xml");
            Check_SolidNames(frameworkPath, modPath, language);
        }

        static void Check_Map(string sourcePath, string fileName) {
            string filePath = Path.Combine(sourcePath, fileName);
            XDocument map = XDocument.Load(filePath);
            StringBuilder sb = new StringBuilder();
            foreach (XElement curSubMap in map.Root.Elements()) {
                string name = curSubMap.Attribute("Name").Value;
                string tran = curSubMap.Element("Translation").Value;
                if (name == tran && !acronymsRegex.IsMatch(tran)) {
                    sb.AppendLine($"Name=\"{name}\"");
                }
                foreach (XElement field in curSubMap.Element("Fields").Elements()) {
                    detected.Add(field.Name.ToString());
                }
            }
            if (sb.Length > 0) {
                Console.WriteLine($">>>> Not-Translated: {filePath}");
                Console.WriteLine();
                Console.WriteLine(sb);
            }
        }

        static void Check_Unique(string sourcePath, string fileName) {
            string filePath = Path.Combine(sourcePath, fileName);
            XDocument uniqueNames = XDocument.Load(filePath);
            List<XElement> notTranslated = new List<XElement>();
            foreach (XElement item in uniqueNames.Root.Elements()) {
                string first = item.Attribute("first").Value;
                string last = item.Attribute("last").Value;
                string nick = item.Attribute("nick")?.Value;
                foreach (XElement field in item.Elements()) {
                    string eleName = field.Name.ToString();
                    if (eleName.EndsWith(".first")) {
                        if (field.Value == first && !acronymsRegex.IsMatch(first)) {
                            notTranslated.Add(field);
                        }
                    } else if (eleName.EndsWith(".last")) {
                        if (field.Value == last && !acronymsRegex.IsMatch(last)) {
                            notTranslated.Add(field);
                        }
                    } else if (eleName.EndsWith(".nick")) {
                        if (field.Value == nick && !acronymsRegex.IsMatch(nick)) {
                            notTranslated.Add(field);
                        }
                    }
                    detected.Add(field.Name.ToString());
                }
            }
            if (notTranslated.Count > 0) {
                Console.WriteLine($">>>> Not-Translated: {filePath}");
                Console.WriteLine();
                foreach (XElement field in notTranslated) {
                    Console.WriteLine(field.ToString());
                }
                Console.WriteLine();
            }
        }

        static void Check_SolidNames(string frameworkPath, string modPath, string language) {
            string filePath_Framework = Path.Combine(frameworkPath, "Languages", "English", "DefInjected", "SolidNameDef", "SolidNames.xml");
            string filePath_Mod = Path.Combine(modPath, "Languages", language, "DefInjected", "SolidNameDef", "SolidNames.xml");
            XElement solidNames_Root_Framework = XDocument.Load(filePath_Framework).Root;
            XElement solidNames_Root_Mod = XDocument.Load(filePath_Mod).Root;
            List<XElement> notTranslated = new List<XElement>();
            foreach (XElement field in solidNames_Root_Mod.Elements()) {
                if (!detected.Contains(field.Name.ToString())) {
                    if (field.Value == solidNames_Root_Framework.Element(field.Name)?.Value && !acronymsRegex.IsMatch(field.Value)) {
                        notTranslated.Add(field);
                    }
                }
            }
            if (notTranslated.Count > 0) {
                Console.WriteLine($">>>> Not-Translated: {filePath_Mod}");
                Console.WriteLine();
                foreach (XElement field in notTranslated) {
                    Console.WriteLine(field.ToString());
                }
                Console.WriteLine();
            }
            var fields = solidNames_Root_Mod.Elements();
            Check_SolidNames_NullOrWhiteSpace(fields);
            Check_SolidNames_Chinese(fields);
        }

        static void Check_SolidNames_NullOrWhiteSpace(IEnumerable<XElement> fields) {
            List<XElement> invalids = new List<XElement>();
            foreach (XElement field in fields) {
                if (string.IsNullOrWhiteSpace(field.Value)) {
                    invalids.Add(field);
                }
            }
            if (invalids.Count > 0) {
                Console.WriteLine("==== Null or white space fields ====");
                Console.WriteLine();
                foreach (XElement field in invalids) {
                    Console.WriteLine(field.ToString());
                }
                Console.WriteLine();
            }
        }

        static void Check_SolidNames_Chinese(IEnumerable<XElement> fields) {
            List<XElement> notTranslated = new List<XElement>();
            foreach (XElement field in fields) {
                if (!validNameRegex.IsMatch(field.Value) && !acronymsRegex.IsMatch(field.Value)) {
                    notTranslated.Add(field);
                }
            }
            if (notTranslated.Count > 0) {
                Console.WriteLine("==== Non-Chinese names ====");
                Console.WriteLine();
                foreach (XElement field in notTranslated) {
                    Console.WriteLine(field.ToString());
                }
                Console.WriteLine();
            }
        }
    }
}
