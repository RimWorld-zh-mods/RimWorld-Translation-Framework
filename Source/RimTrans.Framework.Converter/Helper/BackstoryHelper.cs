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
    public static class BackstoryHelper
    {
        public static void ConvertDoc(XDocument backstoryDoc, out XDocument backstoryInjectionDoc)
        {
            backstoryInjectionDoc = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement("BackstoryInjections"));
            foreach (XElement backstory in backstoryDoc.Root.Elements())
            {
                XElement backstoryInjection;
                ConvertEle(backstory, out backstoryInjection);
                backstoryInjectionDoc.Root.Add(backstoryInjection);
            }
        }

        public static void ConvertEle(XElement backstory, out XElement backstoryInjection)
        {
            backstoryInjection = new XElement("RimTrans.Framework.Injection.BackstoryInjection");

            if (string.Compare(backstory.Name.ToString(), "Childhood", true) == 0)
            {
                backstoryInjection.Add(new XElement("Slot", "Childhood"));
            }
            else if (string.Compare(backstory.Name.ToString(), "Adulthood", true) == 0)
            {
                backstoryInjection.Add(new XElement("Slot", "Adulthood"));
            }
            else
            {
                XElement slot = backstory.GetField("Slot");
                if (slot != null) backstoryInjection.Add(new XElement("Slot", slot.Value));
            }

            {
                XElement title = backstory.GetField("Title");
                if (title != null) backstoryInjection.Add(new XElement("Title", title.Value));

                XElement titleShort = backstory.GetField("TitleShort");
                if (titleShort != null) backstoryInjection.Add(new XElement("TitleShort", titleShort.Value));

                XElement baseDesc = backstory.GetField("BaseDesc");
                if (baseDesc != null) backstoryInjection.Add(new XElement("BaseDesc", baseDesc.Value));
            }

            {
                XElement title = backstoryInjection.Element("Title");
                XElement baseDesc = backstoryInjection.Element("BaseDesc");
                if (!title.Value.Equals(GenText.ToNewsCase(title.Value)))
                {
                    title.Value = GenText.ToNewsCase(title.Value);
                }
                baseDesc.Value = baseDesc.Value.TrimEnd(new char[0]);
                int num = Mathf.Abs(GenText.StableStringHash(baseDesc.Value.Replace("\\n", "\n")) % 100);
                string s = title.Value.Replace('-', ' ');
                s = GenText.CapitalizedNoSpaces(s);
                backstoryInjection.AddFirst(new XElement("defName", GenText.RemoveNonAlphanumeric(s) + num.ToString()));
            }
        }
    }
}
