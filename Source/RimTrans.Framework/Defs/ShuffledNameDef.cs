using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld {
    public class ShuffledNameDef : Def {
        public PawnNameSlot slot;

        public Gender gender;

        public List<string> shuffledNames = new List<string>();

        [DebuggerHidden]
        public override IEnumerable<string> ConfigErrors() {
            if (this.shuffledNames == null || this.shuffledNames.Count == 0) {
                yield return "the filed 'shuffledNames' cannot be null or empty.";
            }
        }

        public override string ToString() {
            return $"ShuffledNameDef '{this.defName}'";
        }
    }
}
