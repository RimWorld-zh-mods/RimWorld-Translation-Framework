using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;
using Harmony;
using RimTrans.Framework.Utility;

namespace RimTrans.Framework.HarmonyPatches {
    [HarmonyPatch(typeof(SolidBioDatabase), "LoadAllBios")]
    public static class SolidBioDatabase_LoadAllBios_Patch {
        public static bool Prefix() {
            PawnBioAndBacktoryUtility.duplicateCount = new Dictionary<string, int>();
            PawnBioAndBacktoryUtility.pendingResolveNames = new Dictionary<string, NameTriple>();
            foreach (PawnBio curPawnBio in DirectXmlLoader.LoadXmlDataInResourcesFolder<PawnBio>("Backstories/Solid")) {
                string curDefName = curPawnBio.name.ToDefName(curPawnBio.gender);
                if (PawnBioAndBacktoryUtility.duplicateCount.ContainsKey(curDefName)) {
                    PawnBioAndBacktoryUtility.duplicateCount[curDefName]++;
                    curDefName += "_" + PawnBioAndBacktoryUtility.duplicateCount[curDefName];
                } else {
                    PawnBioAndBacktoryUtility.duplicateCount.Add(curDefName, 0);
                }
                PawnBioAndBacktoryUtility.pendingResolveNames.Add(curDefName, curPawnBio.name);
                if (curPawnBio.childhood == null || curPawnBio.adulthood == null) {
                    PawnNameDatabaseSolid.AddPlayerContentName(curPawnBio.name, curPawnBio.gender);
                } else {
                    curPawnBio.PostLoad();
                    curPawnBio.ResolveReferences();
                    foreach (string curError in curPawnBio.ConfigErrors()) {
                        Log.Error(curError);
                    }
                    SolidBioDatabase.allBios.Add(curPawnBio);
                    curPawnBio.childhood.shuffleable = false;
                    curPawnBio.childhood.slot = BackstorySlot.Childhood;
                    BackstoryDatabase.AddBackstory(curPawnBio.childhood);
                    curPawnBio.adulthood.shuffleable = false;
                    curPawnBio.adulthood.slot = BackstorySlot.Adulthood;
                    BackstoryDatabase.AddBackstory(curPawnBio.adulthood);
                }
            }
            return false;
        }
    }
}
