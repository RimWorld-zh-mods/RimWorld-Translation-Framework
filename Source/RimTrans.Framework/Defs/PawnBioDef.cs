using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld {
    /// <summary>
    /// For custom pawn bio
    /// </summary>
    public class PawnBioDef : Def {

        #region Enum
        public enum BackstorySource {
            Custom,
            Vanilla,
        }
        #endregion

        #region Fields

        public GenderPossibility gender;

        public NameTriple name;

        public BackstorySource backstoryFrom;

        public BackstoryDef childhoodDef;

        public BackstoryDef adulthoodDef;

        public string childhoodIdentifier;

        public string adulthoodIdentifier;

        public bool pirateKing;

        #endregion

        [DebuggerHidden]
        public override IEnumerable<string> ConfigErrors() {
            if (this.name == null) {
                yield return "the field 'name' cannot be null or empty.";
            } else {
                if (string.IsNullOrEmpty(this.name.First)) {
                    yield return "the field 'first' cannot be null or empty.";
                }
                if (string.IsNullOrEmpty(this.name.Last)) {
                    yield return "the field 'last' cannot be null or empty.";
                }
            }
            if (this.backstoryFrom == BackstorySource.Custom) {
                if (this.childhoodDef != null && this.adulthoodDef == null) {
                    yield return "has childhood backstory, but has no adulthood backstroy.";
                }
                if (this.childhoodDef == null && this.adulthoodDef != null) {
                    yield return "has adulthood backstory, but has no childhood backstroy.";
                }
            } else if (this.backstoryFrom == BackstorySource.Vanilla) {
                if (this.childhoodIdentifier == null && this.adulthoodIdentifier == null) {
                    yield return "uses backstory from vanilla, but has no backstory identifier for both of childhood and adulthood.";
                }
                if (this.childhoodIdentifier != null && this.adulthoodIdentifier == null) {
                    yield return "has childhood backstory identifier, but has no adulthood backstroy identifier.";
                }
                if (this.childhoodIdentifier == null && this.adulthoodIdentifier != null) {
                    yield return "has adulthood backstory identifier, but has no childhood backstroy identifier.";
                }
            }
        }

        public override void ResolveReferences() {
            if (this.adulthoodDef != null && this.adulthoodDef.spawnCategories != null) {
                if (this.adulthoodDef.spawnCategories.Count == 1 &&
                    this.adulthoodDef.spawnCategories[0] == "Trader") {
                    this.adulthoodDef.spawnCategories.Add("Civil");
                }
            }
        }

        public override string ToString() {
            return $"{GetType().Name} '{this.defName}'";
        }

        public PawnBio ToPawnBio() {
            PawnBio pb = new PawnBio();
            pb.gender = this.gender;
            pb.name = this.name;
            if (this.backstoryFrom == BackstorySource.Custom) {
                pb.childhood = this.childhoodDef?.ToBackstory();
                pb.adulthood = this.adulthoodDef?.ToBackstory();
            } else if (this.backstoryFrom == BackstorySource.Vanilla) {
                BackstoryDatabase.allBackstories.TryGetValue(this.childhoodIdentifier, out pb.childhood);
                BackstoryDatabase.allBackstories.TryGetValue(this.adulthoodIdentifier, out pb.adulthood);
            }
            pb.pirateKing = this.pirateKing;
            return pb;
        }
    }
}
