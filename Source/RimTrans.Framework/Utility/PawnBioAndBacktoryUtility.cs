using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RimTrans.Framework.Utility {
    public static class PawnBioAndBacktoryUtility {
        public static void AddAllToDatabase() {
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
    }
}
