using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld {
    public class ShuffledNameDef : Def {
        public PawnNameSlot slot;

        public Gender gender;

        public List<string> shuffledNames = new List<string>();
    }
}
