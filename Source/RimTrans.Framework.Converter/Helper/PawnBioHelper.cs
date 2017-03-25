using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using RimWorld;
using Verse;
using UnityEngine;

namespace RimTrans.Framework.Converter.Helper
{
    public static class PawnBioHelper
    {
        public static int count = 0;

        public static void ConvertDoc(XDocument pawnBioDoc, out XDocument pawnBioInjectionDoc, out XDocument backstoryInjectionDoc)
        {
            pawnBioInjectionDoc = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement("PawnBioInjections"));
            backstoryInjectionDoc = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement("BackstoryInjections"));

            foreach (XElement pawnBio in pawnBioDoc.Root.Elements())
            {
                XElement pawnBioInjection;
                XElement backstoryInjectionChildhood;
                XElement backstoryInjectionAdulthood;
                ConvertEle(pawnBio, out pawnBioInjection, out backstoryInjectionChildhood, out backstoryInjectionAdulthood);
                pawnBioInjectionDoc.Root.Add(pawnBioInjection);
                if (backstoryInjectionChildhood != null) backstoryInjectionDoc.Root.Add(backstoryInjectionChildhood);
                if (backstoryInjectionAdulthood != null) backstoryInjectionDoc.Root.Add(backstoryInjectionAdulthood);
            }
        }

        public static void ConvertEle(XElement pawnBio, out XElement pawnBioInjection, out XElement backstoryInjectionChildhood, out XElement backstoryInjectionAdulthood)
        {
            pawnBioInjection = new XElement("RimTrans.Framework.Injection.PawnBioInjection");
            backstoryInjectionChildhood = null;
            backstoryInjectionAdulthood = null;

            {
                XElement name = pawnBio.GetField("Name");
                if (name != null)
                {
                    pawnBioInjection.Add(new XElement("Name"));

                    XElement nameInjection = pawnBioInjection.Element("Name");

                    XElement first = name.GetField("First", "firstInt");
                    if (first != null) nameInjection.Add(new XElement("First", first.Value));

                    XElement last = name.GetField("Last", "lastInt");
                    if (last != null) nameInjection.Add(new XElement("Last", last.Value));

                    XElement nick = name.GetField("Nick", "nickInt");
                    if (nick != null) nameInjection.Add(new XElement("Nick", nick.Value));
                }

                XElement gender = pawnBio.GetField("Gender");
                if (gender != null) pawnBioInjection.Add(new XElement("Gender", gender.Value));

                XElement pirateKing = pawnBio.GetField("PirateKing");
                if (pirateKing != null) pawnBioInjection.Add(new XElement("PirateKing", pirateKing.Value.ToLower()));

                XElement childhood = pawnBio.GetField("Childhood");
                if (childhood != null)
                {
                    BackstoryHelper.ConvertEle(childhood, out backstoryInjectionChildhood);
                    pawnBioInjection.Add(new XElement("Childhood", backstoryInjectionChildhood.Element("defName").Value));
                }

                XElement adulthood = pawnBio.GetField("Adulthood");
                if (adulthood != null)
                {
                    BackstoryHelper.ConvertEle(adulthood, out backstoryInjectionAdulthood);
                    pawnBioInjection.Add(new XElement("Adulthood", backstoryInjectionAdulthood.Element("defName").Value));
                }
            }

            {
                XElement nameInjection = pawnBioInjection.Element("Name");
                XElement first = nameInjection.Element("First");
                XElement last = nameInjection.Element("Last");
                XElement nick = nameInjection.Element("Nick");
                string identifier = first.Value + " '" + nick.Value + "' " + last.Value;
                pawnBioInjection.AddFirst(new XElement("identifier", identifier));

                string defName = string.Format("PawnBio_{0:D5}", count);
                count++;
                pawnBioInjection.AddFirst(new XElement("defName", defName));
            }
        }
    }
}
