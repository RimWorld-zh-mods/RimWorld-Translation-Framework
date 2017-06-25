using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RimTrans.Framework.Utility {
    public static class CombineNameUtility {

        #region Methods

        public static void CombineAndAddToDatabase() {
            foreach (CombineNameDef curDef in DefDatabase<CombineNameDef>.AllDefs) {
                foreach (NameTriple curName in curDef.ToNameTriples()) {
                    PawnNameDatabaseSolid.AddPlayerContentName(curName, curDef.gender);
                }
            }
        }

        #endregion

    }
}
