using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RimTrans.Framework.Step2 {
    class Program {
        static void Main(string[] args) {
            string configFile = "Config.xml";
            if (File.Exists(configFile)) {
                XDocument configDoc = XDocument.Load(configFile);
                string frameworkPath = configDoc.Root.Element("FrameworkPath").Value;
                string modPath = configDoc.Root.Element("ModPath").Value;
                string language = configDoc.Root.Element("Language").Value;
                bool overrideMode = true;
                bool autoTranslateNicks = true;
                bool autoCommentInvalidFields = true;
                bool trimFields = true;
                XElement step2 = configDoc.Root.Element("Step2");
                if (step2 != null) {
                    XElement overrideModeEle = step2.Element("OverrideMode");
                    if (overrideModeEle != null && overrideModeEle.Value.ToLower() == "true")
                        overrideMode = true;
                    XElement autoTranslateNicksEle = step2.Element("OverrideMode");
                    if (autoTranslateNicksEle != null && autoTranslateNicksEle.Value.ToLower() == "true")
                        autoTranslateNicks = true;
                    XElement autoCommentInvalidFieldsEle = step2.Element("OverrideMode");
                    if (autoCommentInvalidFieldsEle != null && autoCommentInvalidFieldsEle.Value.ToLower() == "true")
                        autoCommentInvalidFields = true;
                    XElement trimFieldsEle = step2.Element("TrimFields");
                    if (trimFieldsEle != null && trimFieldsEle.Value.ToLower() == "true")
                        trimFields = true;
                }
                if (!Directory.Exists(frameworkPath)) {
                    Console.Write($"[ERROR] '{frameworkPath}' no found.");
                } else if (!Directory.Exists(modPath)) {
                    Console.Write($"[ERROR] '{modPath}' no found.");
                } else {
                    Generate(frameworkPath, modPath, language, overrideMode, autoTranslateNicks, autoCommentInvalidFields, trimFields);
                }
                Console.Write("[Completed] Press any key to exit.");
                Console.ReadKey();
            } else {
                Console.Write("[ERROR] Config.xml no found.");
                Console.ReadKey();
            }
        }

        static void Generate(string frameworkPath, string modPath, string language, bool overrideMode, bool autoTranslateNicks, bool autoCommentInvalidFields, bool trimFields) {
            Console.WriteLine("Please wait...");
            string sourcePath = Path.Combine(modPath, "Source", language);
            XDocument map_Last = XDocument.Load(Path.Combine(sourcePath, "SolidNames_Last.xml"));
            XDocument map_First_Male = XDocument.Load(Path.Combine(sourcePath, "SolidNames_First_Male.xml"));
            XDocument map_First_Female = XDocument.Load(Path.Combine(sourcePath, "SolidNames_First_Female.xml"));
            XDocument map_First_Either = XDocument.Load(Path.Combine(sourcePath, "SolidNames_First_Either.xml"));
            XDocument uniqueNames = XDocument.Load(Path.Combine(sourcePath, "SolidNames_Unique.xml"));

            XDocument solidNamesDoc_Framework = XDocument.Load(Path.Combine(frameworkPath, "Languages", "English", "DefInjected", "SolidNameDef", "SolidNames.xml"), LoadOptions.PreserveWhitespace);

            string solidNamesPath_Mod = Path.Combine(modPath, "Languages", language, "DefInjected", "SolidNameDef", "SolidNames.xml");
            XDocument solidNamesDoc_Mod = XDocument.Load(solidNamesPath_Mod);
            Merge(solidNamesDoc_Framework, solidNamesDoc_Mod, autoCommentInvalidFields);

            MassTransByMap(solidNamesDoc_Framework, map_Last);
            MassTransByMap(solidNamesDoc_Framework, map_First_Male);
            MassTransByMap(solidNamesDoc_Framework, map_First_Female);
            MassTransByMap(solidNamesDoc_Framework, map_First_Either);
            TransUniqueNames(solidNamesDoc_Framework, uniqueNames);

            if (autoTranslateNicks) {
                TransNikcs(frameworkPath, solidNamesDoc_Framework);
            }
            if (overrideMode) {
                string overrideNicks_Path = Path.Combine(sourcePath, "Override.xml");
                if (File.Exists(overrideNicks_Path)) {
                    XDocument doc_Framework = XDocument.Load(overrideNicks_Path);
                    OverrideNicks(solidNamesDoc_Framework, doc_Framework);
                }
            }
            if (trimFields) {
                Trim(solidNamesDoc_Framework);
            }

            solidNamesDoc_Framework.Save(solidNamesPath_Mod);
        }

        static void MassTransByMap(XDocument doc_Framework, XDocument map) {
            XElement root = doc_Framework.Root;
            foreach (XElement curSubMap in map.Root.Elements()) {
                string translation = curSubMap.Element("Translation").Value;
                foreach (XElement curField_Map in curSubMap.Element("Fields").Elements()) {
                    XElement curField_Framework = root.Element(curField_Map.Name);
                    if (curField_Framework != null) {
                        curField_Framework.Value = translation;
                    }
                }
            }
        }

        static void TransUniqueNames(XDocument doc_Framework, XDocument uniqueNames) {
            XElement root = doc_Framework.Root;
            foreach (XElement curItem_Unique in uniqueNames.Root.Elements()) {
                foreach (XElement curField_Unique in curItem_Unique.Elements()) {
                    XElement curField_Framework = root.Element(curField_Unique.Name);
                    if (curField_Framework != null) {
                        curField_Framework.Value = curField_Unique.Value;
                    }
                }
            }
        }

        static void TransNikcs(string frameworkPath, XDocument doc_Framework) {
            XElement root = doc_Framework.Root;
            foreach (XElement curDef in XDocument.Load(Path.Combine(frameworkPath, "Defs", "VanillaNames", "SolidNames.xml")).Root.Elements()) {
                string defName = curDef.Element("defName").Value;
                XElement nick = curDef.Element("nick");
                if (nick != null) {
                    if (nick.Value == curDef.Element("first").Value) {
                        root.Element(defName + ".nick").Value = root.Element(defName + ".first").Value;
                    } else if (nick.Value == curDef.Element("last").Value) {
                        root.Element(defName + ".nick").Value = root.Element(defName + ".last").Value;
                    }
                }
            }
        }

        static void Merge(XDocument doc_Framework, XDocument doc_Mod, bool autoCommentInvalidFields) {
            XElement root = doc_Framework.Root;
            int count = 0;
            foreach (XElement curField_Mod in doc_Mod.Root.Elements()) {
                XElement curField_Framework = root.Element(curField_Mod.Name);
                if (curField_Framework != null) {
                    curField_Framework.Value = curField_Mod.Value;
                } else if (autoCommentInvalidFields) {
                    root.Add("  ", new XComment(curField_Mod.ToString()), "\n");
                    count++;
                }
            }
            XElement lastField = doc_Mod.Root.Elements().Last();
            foreach (XNode curNode in lastField.NodesAfterSelf()) {
                if (curNode.NodeType == System.Xml.XmlNodeType.Comment) {
                    root.Add("  ", curNode, "\n");
                    count++;
                }
            }
            if (count > 0) {
                root.Add("\n");
            }
        }

        static void OverrideNicks(XDocument doc_Framework, XDocument doc_Override) {
            XElement root = doc_Framework.Root;
            foreach (XElement curField_Override in doc_Override.Root.Elements()) {
                XElement curField_Framework = root.Element(curField_Override.Name);
                if (curField_Framework != null) {
                    curField_Framework.Value = curField_Override.Value;
                }
            }
        }

        static void Trim(XDocument doc_Framework) {
            foreach (XElement curField in doc_Framework.Root.Elements()) {
                curField.Value = curField.Value.Trim();
            }
        }
    }
}
