using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RimTrans.Framework.Injector {
    public static class PawnNameDatabaseShuffledInjector {
        public static void LoadAllShuffledNames() {
            NameBank nameBank = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard);
            foreach (ShuffledNameDef curDef in DefDatabase<ShuffledNameDef>.AllDefs) {
                nameBank.AddNames(curDef.slot, curDef.gender, curDef.shuffledNames);
            }
        }
    }
}
