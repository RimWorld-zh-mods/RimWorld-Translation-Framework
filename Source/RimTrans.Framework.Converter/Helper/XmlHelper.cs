using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace RimTrans.Framework.Converter.Helper
{
    public static class XmlHelper
    {
        public static XElement GetField(this XElement ele, string fieldName, string loadAlias = null)
        {
            foreach (XElement child in ele.Elements())
            {
                if (string.Compare(child.Name.ToString(), fieldName, true) == 0 ||
                    string.Compare(child.Name.ToString(), loadAlias, true) == 0)
                {
                    return child;
                }
            }
            return null;
        }
    }
}
