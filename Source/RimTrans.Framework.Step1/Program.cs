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
                XDocument configDoc = XDocument.Load(configFile);
                string frameworkPath = configDoc.Root.Element("FrameworkPath").Value;
                string modPath = configDoc.Root.Element("ModPath").Value;
                string language = configDoc.Root.Element("Language").Value;
                if (!Directory.Exists(frameworkPath)) {
                    Console.Write($"[ERROR] '{frameworkPath}' no found.");
                } else if (!Directory.Exists(modPath)) {
                    Console.Write($"[ERROR] '{modPath}' no found.");
                } else {
                    Generate(frameworkPath, modPath, language);
                }
                Console.Write("\n[Completed] Press any key to exit.");
                Console.ReadKey();
            } else {
                Console.Write("[ERROR] Config.xml no found.");
                Console.ReadKey();
            }
        }

        static void Generate(string frameworkPath, string modPath, string language) {
            Dictionary<string, string> dict_Last;
            Dictionary<string, string> dict_First_Male;
            Dictionary<string, string> dict_First_Female;
            GetDict(frameworkPath, modPath, language,
                out dict_Last,
                out dict_First_Male,
                out dict_First_Female);
            SortedDictionary<string, XElement> map_Last;
            SortedDictionary<string, XElement> map_First_Male;
            SortedDictionary<string, XElement> map_First_Female;
            SortedDictionary<string, XElement> map_First_Either;
            List<XElement> uniqueNames;
            Console.WriteLine("==== Create Map ====");
            GetMap(frameworkPath, modPath, language,
                out map_Last,
                out map_First_Male,
                out map_First_Female,
                out map_First_Either,
                out uniqueNames);
            Console.WriteLine("==== Mass Translating ====");
            ImportDictToMap(dict_Last, map_Last);
            ImportDictToMap(dict_First_Male, map_First_Male);
            ImportDictToMap(dict_First_Female, map_First_Female);

            Save(modPath, language, "SolidNames_Last.xml", map_Last);
            Save(modPath, language, "SolidNames_First_Male.xml", map_First_Male);
            Save(modPath, language, "SolidNames_First_Female.xml", map_First_Female);
            Save(modPath, language, "SolidNames_First_Either.xml", map_First_Either);
            Save(modPath, language, "SolidNames_Unique.xml", uniqueNames);
        }

        static void GetDict(string frameworkPath, string modPath, string language,
            out Dictionary<string, string> dict_Last,
            out Dictionary<string, string> dict_First_Male,
            out Dictionary<string, string> dict_First_Female) {
            string shuffledNameDefFolder_Framework = Path.Combine(frameworkPath, "Languages", "English", "DefInjected", "ShuffledNameDef");
            string shuffledNameDefFolder_Mod = Path.Combine(modPath, "Languages", language, "DefInjected", "ShuffledNameDef");
            string[] files = { "Last.xml", "First_Male.xml", "First_Female.xml",  };
            List<Dictionary<string, string>> dicts = new List<Dictionary<string, string>>();
            foreach (string curFile in files) {
                Dictionary<string, string> curDict = new Dictionary<string, string>();
                string curFilePath_Framework = Path.Combine(shuffledNameDefFolder_Framework, curFile);
                string curFilePath_Mod = Path.Combine(shuffledNameDefFolder_Mod, curFile);
                XDocument curDoc_Framework = XDocument.Load(curFilePath_Framework);
                if (File.Exists(curFilePath_Mod)) {
                    // Import existing translation
                    XDocument curDoc_Mod = XDocument.Load(curFilePath_Mod);
                    XElement curRoot_Framework = curDoc_Framework.Root;
                    XElement curRoot_Mod = curDoc_Mod.Root;
                    foreach (XElement curEle_Framework in curRoot_Framework.Elements()) {
                        XElement curEle_Mod = curRoot_Mod.Element(curEle_Framework.Name);
                        if (curEle_Mod != null) {
                            curDict.Add(curEle_Framework.Value, curEle_Mod.Value);
                        } else {
                            curDict.Add(curEle_Framework.Value, curEle_Framework.Value);
                        }
                    }
                } else {
                    foreach (XElement curEle_Framework in curDoc_Framework.Root.Elements()) {
                        curDict.Add(curEle_Framework.Value, curEle_Framework.Value);
                    }
                }
                dicts.Add(curDict);
            }
            dict_Last = dicts[0];
            dict_First_Male = dicts[1];
            dict_First_Female = dicts[2];
        }
        
        static void GetMap(string frameworkPath, string modPath, string language,
            out SortedDictionary<string, XElement> map_Last,
            out SortedDictionary<string, XElement> map_First_Male,
            out SortedDictionary<string, XElement> map_First_Female,
            out SortedDictionary<string, XElement> map_First_Either,
            out List<XElement> uniqueNames) {
            string solidNamesPath_Framework = Path.Combine(frameworkPath, "Defs", "VanillaNames", "SolidNames.xml");
            map_Last = new SortedDictionary<string, XElement>();
            map_First_Male = new SortedDictionary<string, XElement>();
            map_First_Female = new SortedDictionary<string, XElement>();
            map_First_Either = new SortedDictionary<string, XElement>();
            // Create maps
            XDocument solidNamesDoc_Framework = XDocument.Load(solidNamesPath_Framework);
            foreach (XElement curDef in solidNamesDoc_Framework.Root.Elements()) {
                string defName = curDef.Element("defName").Value;
                {
                    string gender = curDef.Element("gender").Value;
                    string first = curDef.Element("first").Value;
                    SortedDictionary<string, XElement> curMap_First = null;
                    switch (gender) {
                    case "Male":
                        curMap_First = map_First_Male;
                        break;
                    case "Female":
                        curMap_First = map_First_Female;
                        break;
                    case "Either":
                        curMap_First = map_First_Either;
                        break;
                    }
                    XElement subMap;
                    if (!curMap_First.TryGetValue(first, out subMap)) {
                        subMap = new XElement("SubMap",
                            new XElement("Translation", first),
                            new XElement("Fields"));
                        subMap.SetAttributeValue("Name", first);
                        subMap.SetAttributeValue("Category", "First");
                        subMap.SetAttributeValue("Gender", gender);
                        curMap_First.Add(first, subMap);
                    }
                    subMap.Element("Fields").Add(new XElement(defName + ".first", first));
                }
                {
                    string last = curDef.Element("last").Value;
                    XElement subMap;
                    if (!map_Last.TryGetValue(last, out subMap)) {
                        subMap = new XElement("SubMap",
                            new XElement("Translation", last),
                            new XElement("Fields"));
                        subMap.SetAttributeValue("Name", last);
                        subMap.SetAttributeValue("Category", "Last");
                        map_Last.Add(last, subMap);
                    }
                    subMap.Element("Fields").Add(new XElement(defName + ".last", last));
                }
            }
            // Extract unique names
            uniqueNames = new List<XElement>();
            foreach (XElement curDef in solidNamesDoc_Framework.Root.Elements()) {
                string defName = curDef.Element("defName").Value;
                string gender = curDef.Element("gender").Value;
                string first = curDef.Element("first").Value;
                string last = curDef.Element("last").Value;
                SortedDictionary<string, XElement> curMap_First = null;
                switch (gender) {
                case "Male":
                    curMap_First = map_First_Male;
                    break;
                case "Female":
                    curMap_First = map_First_Female;
                    break;
                case "Either":
                    curMap_First = map_First_Either;
                    break;
                }
                XElement subMap_first = curMap_First[first];
                XElement subMap_last = map_Last[last];
                IEnumerable<XElement> fields_first = subMap_first.Element("Fields").Elements();
                IEnumerable<XElement> fields_last = subMap_last.Element("Fields").Elements();
                if (fields_first.Count() == 1 && fields_last.Count() == 1) {
                    curMap_First.Remove(first);
                    map_Last.Remove(last);
                    XElement item = new XElement("li", fields_first, fields_last);
                    item.SetAttributeValue("Gender", gender);
                    item.SetAttributeValue("first", fields_first.First().Value);
                    item.SetAttributeValue("last", fields_last.First().Value);
                    XElement nick = curDef.Element("nick");
                    if (nick != null) {
                        item.Add(new XElement(defName + ".nick", nick.Value));
                        item.SetAttributeValue("nick", nick.Value);
                    }
                    uniqueNames.Add(item);
                }
            }
            var fields_map = map_Last.Concat(map_First_Male).Concat(map_First_Female).Concat(map_First_Either);
            // Import existing translation
            string solidNamesPath_Mod = Path.Combine(modPath, "Languages", language, "DefInjected", "SolidNameDef", "SolidNames.xml");
            if (File.Exists(solidNamesPath_Mod)) {
                XDocument solidNamesDoc_Mod = XDocument.Load(solidNamesPath_Mod);
                XElement root = solidNamesDoc_Mod.Root;
                foreach (XElement curField in (from kvp in fields_map
                                               from cur in kvp.Value.Element("Fields").Elements()
                                               select cur)
                                               .Concat(from item in uniqueNames
                                                       from cur in item.Elements()
                                                       select cur)) {
                    XElement curTrans = root.Element(curField.Name);
                    if (curTrans != null) {
                        curField.Value = curTrans.Value;
                    }
                }
            }
            // Select translation from fields
            foreach (var kvp in fields_map) {
                string name = kvp.Key;
                XElement subMap = kvp.Value;
                XElement translation = subMap.Element("Translation");
                Dictionary<string, int> weights = new Dictionary<string, int>();
                foreach (XElement curTrans in subMap.Element("Fields").Elements()) {
                    if (curTrans.Value != name) {
                        if (weights.ContainsKey(curTrans.Value)) {
                            weights[curTrans.Value]++;
                        } else {
                            weights.Add(curTrans.Value, 1);
                        }
                    }
                }
                int max = 0;
                foreach (var curWeight in weights) {
                    if (curWeight.Value > max) {
                        max = curWeight.Value;
                        translation.Value = curWeight.Key;
                    }
                }
                if (max > 0) {
                    Console.WriteLine($"{name} => {translation.Value}");
                }
            }
        }

        static void ImportDictToMap(Dictionary<string, string> dict, SortedDictionary<string, XElement> map) {
            foreach (var kvp in map) {
                string forceTranslation;
                if (dict.TryGetValue(kvp.Key, out forceTranslation)) {
                    XElement translation = kvp.Value.Element("Translation");
                    Console.WriteLine($"{translation.Value} => {forceTranslation}");
                    translation.Value = forceTranslation;
                }
            }
        }

        static void Save(string modPath, string language, string fileName, object mapOrUnique) {
            string sourceFolder = Path.Combine(modPath, "Source", language);
            if (!Directory.Exists(sourceFolder)) {
                Directory.CreateDirectory(sourceFolder);
            }
            string filePath = Path.Combine(sourceFolder, fileName);
            XDocument doc_New = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement("Data"));
            if (mapOrUnique is SortedDictionary<string, XElement>) {
                var map = mapOrUnique as SortedDictionary<string, XElement>;
                foreach (XElement curSubMap in map.Values) {
                    foreach (XElement curField in curSubMap.Element("Fields").Elements()) {
                        curField.RemoveAll();
                    }
                }
                doc_New.Root.Add(new XComment($"Total: {map.Count}"));
                doc_New.Root.Add(map.Values);
                doc_New.Save(filePath);
            } else if (mapOrUnique is List<XElement>) {
                var uniqueNames = mapOrUnique as List<XElement>;
                doc_New.Root.Add(new XComment($"Total: {uniqueNames.Count}"));
                doc_New.Root.Add(uniqueNames);
                doc_New.Save(filePath);
            }
        }
    }
}
