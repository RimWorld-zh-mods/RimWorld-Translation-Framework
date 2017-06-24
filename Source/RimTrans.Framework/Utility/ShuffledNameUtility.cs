using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using Verse;

namespace RimTrans.Framework.Utility {
    public static class ShuffledNameUtility {

        #region Fields

        private static readonly FieldInfo f_names = typeof(NameBank).GetField("names", BindingFlags.Instance | BindingFlags.NonPublic);

        #endregion

        #region Methods

        public static void TranslateVanilla() {
            NameBank nameBank = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard);
            nameBank.Clear();
            foreach (ShuffledNameDef curDef in DefDatabase<ShuffledNameDef>.AllDefs) {
                nameBank.AddNames(curDef.slot, curDef.gender, curDef.shuffledNames);
            }
            nameBank.ErrorCheck();
        }

        public static void AddAllCustom() {
            NameBank nameBank = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard);
            foreach (RandomNameDef curDef in DefDatabase<RandomNameDef>.AllDefs) {
                nameBank.AddNamesDedup(curDef.slot, curDef.gender, curDef.shuffledNames);
            }
        }
        
        #endregion

    }
}
