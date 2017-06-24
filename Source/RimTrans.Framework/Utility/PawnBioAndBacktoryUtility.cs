using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using Verse;
using RimTrans.Framework.HarmonyPatches;

namespace RimTrans.Framework.Utility {
    public static class PawnBioAndBacktoryUtility {

        #region Fields

        public static Dictionary<string, int> duplicateCount;

        public static Dictionary<string, NameTriple> pendingResolveNames;

        private static readonly Type t_NameTriple = typeof(NameTriple);

        private static readonly FieldInfo f_firstInt = t_NameTriple.GetField("firstInt", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo f_lastInt = t_NameTriple.GetField("lastInt", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo f_nickInt = t_NameTriple.GetField("nickInt", BindingFlags.NonPublic | BindingFlags.Instance);

        #endregion

        #region Methods

        public static void TranslateVanilla() {
            List<string> missingDefs = new List<string>();
            List<SolidNameDef> solidNamesMatched = new List<SolidNameDef>();
            foreach (var kvp in pendingResolveNames) {
                SolidNameDef curDef = DefDatabase<SolidNameDef>.GetNamed(kvp.Key, false);
                if (curDef == null) {
                    if (kvp.Key != "Male_Tynan_Sylvester_Tynan") {
                        missingDefs.Add($"{kvp.Key} => {kvp.Value.ToString()}");
                    }
                } else {
                    solidNamesMatched.Add(curDef);
                    f_firstInt.SetValue(kvp.Value, curDef.first);
                    f_lastInt.SetValue(kvp.Value, curDef.last);
                    f_nickInt.SetValue(kvp.Value, curDef.nick);
                }
                kvp.Value.ResolveMissingPieces();
            }
            if (missingDefs.Count != 0) {
                Log.Warning($"[{RimTransFrameworkMod.ModIdentifier}] SolidNameDef no found:\n{string.Join("; ", missingDefs.ToArray())}");
            }
            List<string> solidNamesNonMatched = new List<string>();
            foreach (SolidNameDef curDef in DefDatabase<SolidNameDef>.AllDefs) {
                if (!solidNamesMatched.Contains(curDef)) {
                    solidNamesNonMatched.Add(curDef.defName);
                }
            }
            if (solidNamesNonMatched.Count != 0) {
                Log.Warning($"[{RimTransFrameworkMod.ModIdentifier}] SolidNameDef no matched any name:\n");
            }
        }

        public static void AddAllCustom() {
            foreach (PawnBio curPawnBio in from def in DefDatabase<PawnBioDef>.AllDefsListForReading
                                           select def.ToPawnBio()) {
                curPawnBio.name.ResolveMissingPieces();
                if (curPawnBio.childhood == null || curPawnBio.adulthood == null) {
                    PawnNameDatabaseSolid.AddPlayerContentName(curPawnBio.name, curPawnBio.gender);
                } else {
                    SolidBioDatabase.allBios.Add(curPawnBio);
                    curPawnBio.childhood.shuffleable = false;
                    curPawnBio.childhood.slot = BackstorySlot.Childhood;
                    BackstoryDatabase.AddBackstory(curPawnBio.childhood);
                    curPawnBio.adulthood.shuffleable = false;
                    curPawnBio.adulthood.slot = BackstorySlot.Adulthood;
                    BackstoryDatabase.AddBackstory(curPawnBio.adulthood);
                }
            }
            foreach (Backstory curBackstory in from def in DefDatabase<BackstoryDef>.AllDefsListForReading
                                               where def.shuffleable
                                               select def.ToBackstory()) {
                if (!BackstoryDatabase.allBackstories.ContainsKey(curBackstory.identifier)) {
                    BackstoryDatabase.AddBackstory(curBackstory);
                }
            }
        }

        #endregion
    }
}
