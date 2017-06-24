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

        public static void TranslateVanillaNames() {
            NameBank nameBank = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard);
            foreach (List<string> namesList in (List<string>[,])f_names.GetValue(nameBank)) {
                if (namesList != null)
                    namesList.Clear();
            }
            foreach (ShuffledNameDef curDef in DefDatabase<ShuffledNameDef>.AllDefs) {
                nameBank.AddNames(curDef.slot, curDef.gender, curDef.shuffledNames);
            }
            nameBank.ErrorCheck();
        }

        #endregion

    }
}
