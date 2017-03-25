using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using RimTrans.Framework.Converter.Helper;

namespace RimTrans.Framework.Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            string defsPath = @"..\..\..\..\Defs";

            string backstoryInjectionPath = Path.Combine(defsPath, "BackstoryInjection");
            if (!Directory.Exists(backstoryInjectionPath)) Directory.CreateDirectory(backstoryInjectionPath);

            string pawnBioInjectionPath = Path.Combine(defsPath, "PawnBioInjection");
            if (!Directory.Exists(pawnBioInjectionPath)) Directory.CreateDirectory(pawnBioInjectionPath);

            string shuffledNameInjectionPath = Path.Combine(defsPath, "ShuffledNameInjection");
            if (!Directory.Exists(shuffledNameInjectionPath)) Directory.CreateDirectory(shuffledNameInjectionPath);

            DirectoryInfo textAssetDirInfo = new DirectoryInfo(@"..\..\..\..\Resources\TextAsset");

            foreach (FileInfo xmlFileInfo in textAssetDirInfo.GetFiles("*.xml"))
            {
                XDocument rawDoc = XDocument.Load(xmlFileInfo.FullName);
                if (rawDoc.Root.Name.ToString() == "Backstories")
                {
                    XDocument backstoryInjectionDoc;
                    BackstoryHelper.ConvertDoc(rawDoc, out backstoryInjectionDoc);

                    string path = Path.Combine(backstoryInjectionPath, "ShuffledBacktories_" + xmlFileInfo.Name);
                    backstoryInjectionDoc.Save(path);
                }
                else if (rawDoc.Root.Name.ToString() == "PlayerCreatedBios")
                {
                    XDocument pawnBioInjectionDoc;
                    XDocument backstoryInjectionDoc;
                    PawnBioHelper.ConvertDoc(rawDoc, out pawnBioInjectionDoc, out backstoryInjectionDoc);

                    string path_p = Path.Combine(pawnBioInjectionPath, "SolidPawnBios_" + xmlFileInfo.Name);
                    pawnBioInjectionDoc.Save(path_p);

                    string path_b = Path.Combine(backstoryInjectionPath, "SolidBackstories_" + xmlFileInfo.Name);
                    backstoryInjectionDoc.Save(path_b);
                }
            }

            foreach (FileInfo txtFileInfo in textAssetDirInfo.GetFiles("*.txt"))
            {
                XDocument shuffledNameDoc;
                ShuffledNameHelper.ConvertDoc(txtFileInfo.FullName, out shuffledNameDoc);

                string path = Path.Combine(shuffledNameInjectionPath, txtFileInfo.Name.Replace(".txt", ".xml"));
                shuffledNameDoc.Save(path);
            }

            {// Format xml files
                DirectoryInfo defsDirInfo = new DirectoryInfo(defsPath);
                foreach (FileInfo xmlFileInfo in defsDirInfo.GetFiles("*.xml", SearchOption.AllDirectories))
                {
                    XDocument doc = XDocument.Load(xmlFileInfo.FullName, LoadOptions.PreserveWhitespace);
                    doc.Root.AddFirst("\n");
                    foreach (XElement ele in doc.Root.Elements())
                    {
                        ele.AddAfterSelf("\n");
                    }
                    doc.Save(xmlFileInfo.FullName);
                }
            }

            {// Check Backstories
                DirectoryInfo backstoryInjectionDirInfo = new DirectoryInfo(backstoryInjectionPath);
                XDocument Fresh_Backstories = XDocument.Load(@"..\..\..\..\Resources\Fresh_Backstories.xml");
                XElement root = Fresh_Backstories.Root;
                Console.WriteLine("Removing matched backstories....");
                foreach (FileInfo biFileInfo in backstoryInjectionDirInfo.GetFiles("*.xml", SearchOption.TopDirectoryOnly))
                {
                    XDocument backstoryInjectionDoc = XDocument.Load(biFileInfo.FullName);
                    foreach (XElement ele in backstoryInjectionDoc.Root.Elements())
                    {
                        XElement defName = ele.Element("defName");
                        XElement rawBackstory = root.Element(defName.Value);
                        if (rawBackstory == null)
                        {
                            Console.WriteLine(defName);
                        }
                        else
                        {
                            rawBackstory.Remove();
                        }
                    }
                }
                if (root.HasElements)
                {
                    Console.WriteLine("Following backstories no matched:");
                    foreach (XElement ele in root.Elements())
                    {
                        Console.WriteLine(ele.Name.ToString());
                    }
                }
            }

            {// Check PawnBios

            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
