using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using RimTrans.Framework.HarmonyPatches;
using RimTrans.Framework.Utility;

namespace RimTrans.Framework {
    [StaticConstructorOnStartup]
    public class RimTransFrameworkController {
        static RimTransFrameworkController() {
            LongEventHandler.ExecuteWhenFinished(CallAll);
        }

        public static void CallAll() {
            PawnBioAndBacktoryUtility.TranslateVanilla();
            PawnBioAndBacktoryUtility.AddAllCustom();
            ShuffledNameUtility.TranslateVanilla();
            ShuffledNameUtility.AddAllCustom();
        }
    }
}
