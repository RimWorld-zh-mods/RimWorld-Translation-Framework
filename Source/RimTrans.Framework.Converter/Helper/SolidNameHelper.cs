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
using UnityEngine;

namespace RimTrans.Framework.Converter.Helper
{
    public static class SolidNameHelper
    {
        public static string GetIdentifier(XElement bs) {
            XElement BaseDesc = bs.GetField("BaseDesc");
            int num = Mathf.Abs(GenText.StableStringHash(BaseDesc.Value) % 100);
            XElement Title = bs.GetField("Title");
            string s = Title.Value.Replace('-', ' ');
            s = GenText.CapitalizedNoSpaces(s);
            return GenText.RemoveNonAlphanumeric(s) + num;
        }

        public static void Convert(string pathTextAsset, string pathDefs) {
            int count = 0;
            XDocument source = XDocument.Load(Path.Combine(pathTextAsset, "rimworld_creations.xml"));
            XElement root = new XElement("Defs", new XComment("These names are from RimWorld Creative Rewards."));
            foreach (XElement bio in source.Root.Elements()) {
                XElement Name = bio.GetField("Name");
                if (Name != null) {
                    XElement def = new XElement("SolidNameDef");
                    XElement Gender = bio.GetField("Gender");
                    if (Gender == null) Gender = new XElement("Gender", GenderPossibility.Either);
                    def.Add(new XElement("gender", Gender.Value));
                    XElement First = Name.GetField("First");
                    XElement Last = Name.GetField("Last");
                    XElement Nick = Name.GetField("Nick");
                    StringBuilder sb = new StringBuilder();
                    sb.Append(Gender.Value);
                    sb.Append("_");
                    if (!string.IsNullOrWhiteSpace(First.Value)) {
                        sb.Append(First.Value.Replace("'", "").Replace(" ", ""));
                        def.Add(new XElement("first", First.Value));
                    } else {
                        sb.Append("EMPTY");
                    }
                    sb.Append("_");
                    if (!string.IsNullOrWhiteSpace(Last.Value)) {
                        sb.Append(Last.Value.Replace("'", "").Replace(" ", ""));
                        def.Add(new XElement("last", Last.Value));
                    } else {
                        sb.Append("EMPTY");
                    }
                    sb.Append("_");
                    if (!string.IsNullOrWhiteSpace(Nick.Value)) {
                        sb.Append(Nick.Value.Replace("'", "").Replace(" ", ""));
                        def.Add(new XElement("nick", Nick.Value));
                    } else {
                        sb.Append("EMPTY");
                    }
                    def.AddFirst(new XElement("defName", sb.ToString()));
                    XElement Childhood = bio.GetField("Childhood");
                    if (Childhood != null) def.Add(new XElement("childhood", GetIdentifier(Childhood)));
                    XElement Adulthood = bio.GetField("Adulthood");
                    if (Adulthood != null) def.Add(new XElement("adulthood", GetIdentifier(Adulthood)));
                    XElement PirateKing = bio.GetField("PirateKing");
                    if (PirateKing != null) def.Add(new XElement("pirateKing", PirateKing.Value.ToLower()));
                    root.Add(def);
                    count++;
                }
            }
            Console.WriteLine("SolidNames: " + count);
            Dictionary<string, int> duplicate = new Dictionary<string, int>();
            foreach (XElement def in root.Elements()) {
                string defName = def.Element("defName").Value;
                if (!duplicate.ContainsKey(defName)) {
                    duplicate.Add(defName, 0);
                } else {
                    string temp = defName;
                    duplicate[defName]++;
                    defName += "_" + duplicate[defName];
                    def.Element("defName").Value = defName;
                    Console.WriteLine("Duplicate defName '{0}' => '{1}'", temp, defName);
                }
            }
            string savePath = Path.Combine(Path.Combine(pathDefs, "VanillaNames"), "SolidNames.xml");
            (new XDocument(new XDeclaration("1.0", "utf-8", null), root)).Save(savePath);
        }


    }
}
