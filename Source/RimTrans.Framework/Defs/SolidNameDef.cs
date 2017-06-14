using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld {
    public class SolidNameDef : Def {
        public GenderPossibility gender;

        public string first;

        public string last;

        public string nick;

        public string childhood;

        public string adulthood;

        public bool pirateKing;

        [DebuggerHidden]
        public override IEnumerable<string> ConfigErrors() {
            if (string.IsNullOrEmpty(first)) {
                yield return "the filed 'first' cannot be null or empty.";
            }
            if (string.IsNullOrEmpty(last)) {
                yield return "the filed 'last' cannot be null or empty.";
            }
        }

        public override string ToString() {
            return $"SolidNameDef '{this.defName}'";
        }
    }
}
