using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld {
    public class ShuffledNameDef : Def {

        #region Fields

        public PawnNameSlot slot;

        public Gender gender;

        public List<string> shuffledNames = new List<string>();

        #endregion

        [DebuggerHidden]
        public override IEnumerable<string> ConfigErrors() {
            if (this.shuffledNames == null || this.shuffledNames.Count == 0) {
                yield return "the field 'shuffledNames' cannot be null or empty.";
            }
        }

        public override string ToString() {
            return $"{GetType().Name} '{this.defName}'";
        }
    }
}
