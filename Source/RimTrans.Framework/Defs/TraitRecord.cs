using System;
using System.Xml;
using Verse;

namespace RimWorld {
    public class TraitRecord {

        public TraitDef trait;

        public int degree;

        public void LoadDataFromXmlCustom(XmlNode xmlRoot) {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "trait", xmlRoot.Name);
            if (xmlRoot.HasChildNodes) {
                this.degree = (int)ParseHelper.FromString(xmlRoot.FirstChild.Value, typeof(int));
            } else {
                this.degree = 0;
            }
        }

        public TraitEntry ToEntry() {
            return new TraitEntry(this.trait, this.degree);
        }
    }
}
