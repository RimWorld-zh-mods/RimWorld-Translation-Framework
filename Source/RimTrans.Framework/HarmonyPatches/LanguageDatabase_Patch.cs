using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;

namespace RimTrans.Framework.HarmonyPatches {
    [HarmonyPatch(typeof(LanguageDatabase), "SelectLanguage")]
    public static class LanguageDatabase_Patch {
        public static void Postfix(LoadedLanguage lang) {
            RimTransFrameworkController.CallAll();
        }
    }
}
