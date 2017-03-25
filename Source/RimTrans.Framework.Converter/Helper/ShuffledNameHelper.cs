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

namespace RimTrans.Framework.Converter.Helper
{
    public static class ShuffledNameHelper
    {
        public static void ConvertDoc(string filePath, out XDocument ShuffledNameInjectionDoc)
        {
            ShuffledNameInjectionDoc = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement("ShuffledNameInjections"));
            XElement shuffledNameInjection = new XElement("RimTrans.Framework.Injection.ShuffledNameInjection");
            XElement namePairs = new XElement("namePairs");
            using (StreamReader sr = new StreamReader(filePath))
            {
                while (sr.Peek() >= 0)
                {
                    string name = sr.ReadLine();
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        if (name.IndexOf("//") == 0)
                        {
                            namePairs.Add(new XComment(name.Substring(2)));
                        }
                        else
                        {
                            XElement li = new XElement("li",
                                new XElement("englishName", name),
                                new XElement("nativeName", name));
                            namePairs.Add(li);
                        }
                    }
                }
            }
            shuffledNameInjection.Add(namePairs);
            shuffledNameInjection.AddFirst(new XElement("defName", Path.GetFileName(filePath).Replace(".txt", string.Empty)));
            ShuffledNameInjectionDoc.Root.Add(shuffledNameInjection);
        }
    }
}
