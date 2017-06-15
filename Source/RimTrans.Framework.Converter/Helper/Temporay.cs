using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace RimTrans.Framework.Converter.Helper {
    public static class Temporay {
        public static void foo1() {
            XDocument doc_Def = XDocument.Load(@"D:\Translating\人名翻译项目\Def.xml");
            XDocument doc_DefInjected = XDocument.Load(@"D:\Translating\人名翻译项目\DefInjected.xml");
            XElement LanguageData = doc_DefInjected.Root;
            XDocument doc_new = XDocument.Load(@"D:\Translating\人名翻译项目\DefInjected.xml");
            XElement newLanguageData = doc_new.Root;
            newLanguageData.RemoveAll();
            XElement newDefs = new XElement("Defs");
            foreach (XElement curDef in doc_Def.Root.Elements()) {
                XElement Name = curDef.Element("Name");
                if (Name != null) {
                    XElement newDef = new XElement("SolidNameDef");
                    string defName = curDef.Element("defName").Value;
                    XElement Gender = curDef.Element("Gender");
                    if (Gender == null) Gender = new XElement("Gender", "Either");
                    newDef.Add(new XElement("gender", Gender.Value));
                    XElement First = Name.Element("First");
                    XElement Last = Name.Element("Last");
                    XElement Nick = Name.Element("Nick");
                    StringBuilder sb = new StringBuilder();
                    sb.Append(Gender.Value);
                    sb.Append("_");
                    if (!string.IsNullOrWhiteSpace(First.Value)) {
                        sb.Append(First.Value.Replace("'", "").Replace(" ", ""));
                        newDef.Add(new XElement("first", LanguageData.Element(defName + ".Name.First").Value));
                    } else {
                        sb.Append("EMPTY");
                    }
                    sb.Append("_");
                    if (!string.IsNullOrWhiteSpace(Last.Value)) {
                        sb.Append(Last.Value.Replace("'", "").Replace(" ", ""));
                        newDef.Add(new XElement("last", LanguageData.Element(defName + ".Name.Last").Value));
                    } else {
                        sb.Append("EMPTY");
                    }
                    sb.Append("_");
                    if (!string.IsNullOrWhiteSpace(Nick.Value)) {
                        sb.Append(Nick.Value.Replace("'", "").Replace(" ", ""));
                        newDef.Add(new XElement("nick", LanguageData.Element(defName + ".Name.Nick").Value));
                    } else {
                        sb.Append("EMPTY");
                    }
                    newDef.AddFirst(new XElement("defName", sb.ToString()));
                    newDefs.Add(newDef);
                }
            }
            Dictionary<string, int> duplicate = new Dictionary<string, int>();
            foreach (XElement curDef in newDefs.Elements()) {
                string defName = curDef.Element("defName").Value;
                if (!duplicate.ContainsKey(defName)) {
                    duplicate.Add(defName, 0);
                } else {
                    string temp = defName;
                    duplicate[defName]++;
                    defName += "_" + duplicate[defName];
                    curDef.Element("defName").Value = defName;
                    Console.WriteLine("Duplicate defName '{0}' => '{1}'", temp, defName);
                }
            }
            newLanguageData.Add("\n\n");
            foreach (XElement curDef in newDefs.Elements()) {
                string defName = curDef.Element("defName").Value;
                XElement first = curDef.Element("first");
                XElement last = curDef.Element("last");
                XElement nick = curDef.Element("nick");
                if (!string.IsNullOrWhiteSpace(first.Value)) {
                    newLanguageData.Add("  ", new XElement(defName + ".first", first.Value), "\n");
                }
                if (!string.IsNullOrWhiteSpace(last.Value)) {
                    newLanguageData.Add("  ", new XElement(defName + ".last", last.Value), "\n");
                }
                if (nick != null && !string.IsNullOrWhiteSpace(nick.Value)) {
                    newLanguageData.Add("  ", new XElement(defName + ".nick", nick.Value), "\n");
                }
                newLanguageData.Add("\n");
            }
            newLanguageData.Add("\n");
            doc_new.Save(@"D:\Translating\人名翻译项目\New.xml");
        }

        public static void foo2() {
            string forderOld = @"D:\Translating\人名翻译项目\RimTrans.Framework.Injection.ShuffledNameInjection";
            string forderNew = @"C:\Git\RW\RimWorld-Translation-Framework\Languages\English\DefInjected\ShuffledNameDef";
            string[] files = { "First_Female.xml", "First_Male.xml", "Last.xml", "Nick_Female.xml", "Nick_Male.xml", "Nick_Unisex.xml" };
            foreach (string curFile in files) {
                XDocument docOld = XDocument.Load(Path.Combine(forderOld, curFile));
                XDocument docNew = XDocument.Load(Path.Combine(forderNew, curFile));
                var elesOld = docOld.Root.Elements().ToArray();
                var elesNew = docNew.Root.Elements().ToArray();
                for (int i = 0; i < elesOld.Count(); i++) {
                    elesOld[i].Name = elesNew[i].Name;
                }
                docOld.Save(Path.Combine(forderOld, "New_" + curFile));
            }
        }

        public static void foo3() {
            XDocument docNew = XDocument.Load(@"D:\Translating\人名翻译项目\New.xml", LoadOptions.PreserveWhitespace);
            XDocument docCN = XDocument.Load(@"C:\Git\RW\RimWorld-PawnName-Chinese\Languages\ChineseSimplified\DefInjected\SolidNameDef\SolidNames.xml", LoadOptions.PreserveWhitespace);
            bool flag = false;
            foreach (XElement itemNew in docNew.Root.Elements()) {
                if (!flag) {
                    if (itemNew.Name.ToString() == "Male_Vernon_Gaudi_EMPTY.last") {
                        flag = true;
                    }
                } else {
                    if (itemNew.Name.ToString() == "PawnBio_03389.Name.First") {
                        break;
                    }
                    XElement itemCN = docCN.Root.Element(itemNew.Name);
                    if (itemCN != null) {
                        itemCN.Value = itemNew.Value;
                    } else {
                        Console.WriteLine($"itemCN no found. itemNew = {itemNew.Name.ToString()}");
                    }
                }
            }
            docCN.Save(@"C:\Git\RW\RimWorld-PawnName-Chinese\Languages\ChineseSimplified\DefInjected\SolidNameDef\SolidNames.xml");
        }
    }
}
