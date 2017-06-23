using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld {
    public class PawnBioDef : Def {

        #region Fields

        public GenderPossibility gender;

        public NameTriple name;

        public BackstoryDef childhood;

        public BackstoryDef adulthood;

        public bool pirateKing;

        #endregion

        [DebuggerHidden]
        public override IEnumerable<string> ConfigErrors() {
            if (this.name == null) {
                yield return "the field 'name' cannot be null or empty.";
            } else {

            }
            if (this.childhood != null && this.adulthood == null) {
                yield return "has childhood backstory, but has no adulthood backstroy.";
            }
            if (this.childhood == null && this.adulthood != null) {
                yield return "has adulthood backstory, but has no childhood backstroy.";
            }
        }

        public override string ToString() {
            return $"{GetType().Name} '{this.defName}'";
        }

        public PawnBio ToPawnBio() {
            PawnBio pb = new PawnBio();
            pb.gender = this.gender;
            pb.name = this.name;
            pb.childhood = this.childhood?.ToBackstory();
            pb.adulthood = this.adulthood?.ToBackstory();
            pb.pirateKing = this.pirateKing;
            return pb;
        }
    }
}
