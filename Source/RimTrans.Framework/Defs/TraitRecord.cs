using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Verse;

namespace RimWorld {

    #region Extension
    public static class TraitRecordExtension {
        public static List<TraitEntry> ToTraitEntryList(this List<TraitRecord> traitRecordList) {
            if (traitRecordList == null)
                return null;
            List<TraitEntry> traitEntryList = new List<TraitEntry>();
            foreach (TraitRecord record in traitRecordList) {
                traitEntryList.Add(record.ToEntry());
            }
            return traitEntryList;
        }
    }
    #endregion

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

        public void ResolveReferences(BackstoryDef parent) {
            foreach (TraitDegreeData data in this.trait.degreeDatas) {
                if (data.degree == this.degree) {
                    return;
                }
            }
            Log.Error($"BackstoryDef '{parent.defName}' found no data at degree '{this.degree}' for trait '{this.trait.defName}', set to first defined.");
            this.degree = this.trait.degreeDatas.First().degree;
        }

        public TraitEntry ToEntry() {
            return new TraitEntry(this.trait, this.degree);
        }
    }
}
