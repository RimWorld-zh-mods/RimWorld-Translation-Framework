using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld {
    public class SolidNameDef : Def {

        #region Fields

        public GenderPossibility gender;

        public string first;

        public string last;

        public string nick;

        public string childhood;

        public string adulthood;

        public bool pirateKing;

        #endregion

        [DebuggerHidden]
        public override IEnumerable<string> ConfigErrors() {
            if (string.IsNullOrEmpty(this.first)) {
                yield return "the field 'first' cannot be null or empty.";
            }
            if (string.IsNullOrEmpty(this.last)) {
                yield return "the field 'last' cannot be null or empty.";
            }
        }

        public override string ToString() {
            return $"{GetType().Name} '{this.defName}'";
        }
    }
}
