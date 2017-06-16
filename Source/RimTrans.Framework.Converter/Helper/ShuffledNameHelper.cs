using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using RimWorld;
using Verse;

namespace RimTrans.Framework.Converter.Helper {
    public static class ShuffledNameHelper {

        public static XDocument TxtToXml(PawnNameSlot slot, Gender gender, string defName, string path) {
            int count = 0;
            List<string> names = new List<string>();
            using (StreamReader sr = new StreamReader(path)) {
                while (sr.Peek() >= 0) {
                    names.Add(sr.ReadLine().Trim());
                }
            }
            XElement root = new XElement("Defs", new XComment("NOTE: The names disallow duplicates in the same file."));
            XElement def = new XElement("ShuffledNameDef", new XElement("shuffledNames"));
            foreach (string item in names) {
                if (!string.IsNullOrEmpty(item)) {
                    if (item.Contains("//")) {
                        if (def.Element("shuffledNames").HasElements) {
                            root.Add(def);
                            root.Add(new XComment(item.Replace("//", "")));
                            def = new XElement("ShuffledNameDef", new XElement("shuffledNames"));
                        } else {
                            root.Add(new XComment(item.Replace("//", "")));
                        }
                    } else {
                        def.Element("shuffledNames").Add(new XElement("li", item));
                        count++;
                    }
                }
            }
            if (root.LastNode != def) {
                root.Add(def);
            }
            Console.WriteLine(defName + ": " + count);
            count = 0;
            foreach (XElement curDef in root.Elements()) {
                curDef.AddFirst(new XElement("defName", defName + "_" + count));
                curDef.Element("shuffledNames").AddBeforeSelf(
                    new XElement("slot", slot),
                    new XElement("gender", gender)
                    );
                count++;
            }
            return new XDocument(new XDeclaration("1.0", "utf-8", null), root);
        }

        public static void Convert(string pathTextAsset, string pathDefs) {
            string dirPath = Path.Combine(pathDefs, "VanillaNames");
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            TxtToXml(PawnNameSlot.First, Gender.Male, "First_Male", Path.Combine(pathTextAsset, "First_Male.txt")).Save(Path.Combine(dirPath, "First_Male.xml"));
            TxtToXml(PawnNameSlot.First, Gender.Female, "First_Female", Path.Combine(pathTextAsset, "First_Female.txt")).Save(Path.Combine(dirPath, "First_Female.xml"));
            TxtToXml(PawnNameSlot.Nick, Gender.Male, "Nick_Male", Path.Combine(pathTextAsset, "Nick_Male.txt")).Save(Path.Combine(dirPath, "Nick_Male.xml"));
            TxtToXml(PawnNameSlot.Nick, Gender.Female, "Nick_Female", Path.Combine(pathTextAsset, "Nick_Female.txt")).Save(Path.Combine(dirPath, "Nick_Female.xml"));
            TxtToXml(PawnNameSlot.Nick, Gender.None, "Nick_Unisex", Path.Combine(pathTextAsset, "Nick_Unisex.txt")).Save(Path.Combine(dirPath, "Nick_Unisex.xml"));
            TxtToXml(PawnNameSlot.Last, Gender.None, "Last", Path.Combine(pathTextAsset, "Last.txt")).Save(Path.Combine(dirPath, "Last.xml"));
        }
    }
}
