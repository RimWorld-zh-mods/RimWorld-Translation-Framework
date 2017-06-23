using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld {
    public class BackstoryDef : Def {

        #region Fields

        public BackstorySlot slot;

        public string title;

        public string titleShort;

        public string baseDesc;

        public Dictionary<SkillDef, int> skillGains = new Dictionary<SkillDef, int>();

        public WorkTags workDisables;

        public WorkTags requiredWorkTags;

        public List<string> spawnCategories = new List<string>();

        [LoadAlias("bodyNameGlobal")]
        public BodyType bodyTypeGlobal;

        [LoadAlias("bodyNameFemale")]
        public BodyType bodyTypeFemale;

        [LoadAlias("bodyNameMale")]
        public BodyType bodyTypeMale;

        public List<TraitRecord> forcedTraits;

        public List<TraitRecord> disallowedTraits;

        public bool shuffleable = true;

        #endregion

        public override void PostLoad() {
            if (!string.IsNullOrEmpty(this.title)) {
                this.title = GenText.ToNewsCase(this.title.Trim());
            }
            this.titleShort = this.titleShort?.Trim();
            this.baseDesc = this.baseDesc?.Trim();
            if (this.slot == BackstorySlot.Adulthood && this.bodyTypeGlobal == BodyType.Undefined) {
                if (this.bodyTypeMale == BodyType.Undefined)
                    this.bodyTypeMale = BodyType.Male;
                if (this.bodyTypeFemale == BodyType.Undefined)
                    this.bodyTypeFemale = BodyType.Female;
            }
            base.PostLoad();
        }

        [DebuggerHidden]
        public override IEnumerable<string> ConfigErrors() {
            if (string.IsNullOrEmpty(this.title)) {
                yield return "the field 'title' cannot be null or empty.";
            }
            if (string.IsNullOrEmpty(this.titleShort)) {
                yield return "the field 'titleShort' cannot be null or empty.";
            }
            if (string.IsNullOrEmpty(this.baseDesc)) {
                yield return "the field 'baseDesc' cannot be null or empty.";
            }
            if (this.spawnCategories == null || this.spawnCategories.Count == 0) {
                yield return "has no spawn category.";
            } else {
                if (this.spawnCategories.Contains("Raider") &&
                    (this.workDisables & WorkTags.Violent) == WorkTags.Violent) {
                    yield return "cannot do 'Violent' work but has 'Raider' spawn category.";
                }
                if (this.spawnCategories.Contains("Trader") &&
                    this.spawnCategories.Count == 1) {
                    yield return "noly has 'Trader' spawn category.";
                }
            }
            if (this.workDisables != WorkTags.None &&
                this.skillGains != null && this.skillGains.Count != 0) {
                foreach (var skill in this.skillGains.Keys) {
                    if ((this.workDisables & skill.disablingWorkTags) != WorkTags.None) {
                        yield return $"modifies skill '{skill.defName}' by field 'workDisables', but also disables this skill";
                    }
                }
            }
        }

        public override string ToString() {
            return $"{GetType().Name} '{this.defName}'";
        }

        public Backstory ToBackstory() {
            Backstory bs = new Backstory();
            bs.identifier = "BackstoryDef_" + this.defName;
            bs.slot = this.slot;
            bs.SetTitle(this.title);
            bs.SetTitleShort(this.titleShort);
            bs.baseDesc = this.baseDesc;
            bs.skillGainsResolved = this.skillGains;
            bs.workDisables = this.workDisables;
            bs.requiredWorkTags = this.requiredWorkTags;
            bs.spawnCategories = this.spawnCategories;
            bs.bodyTypeGlobal = this.bodyTypeGlobal;
            bs.bodyTypeFemale = this.bodyTypeFemale;
            bs.bodyTypeMale = this.bodyTypeMale;
            if (this.forcedTraits != null) {
                bs.forcedTraits = new List<TraitEntry>();
                foreach (var record in this.forcedTraits) {
                    bs.forcedTraits.Add(record.ToEntry());
                }
            }
            if (this.disallowedTraits != null) {
                bs.disallowedTraits = new List<TraitEntry>();
                foreach (var record in this.disallowedTraits) {
                    bs.disallowedTraits.Add(record.ToEntry());
                }
            }
            bs.shuffleable = this.shuffleable;
            return bs;
        }
    }
}
