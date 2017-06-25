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
            CombineNameUtility.CombineAndAddToDatabase();

#if DEBUG
            // DEBUG
            Debug();
#endif
        }

        public static void Debug() {
            NameBank nameBank = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard);
            Log.Message($"PawnNameDatabaseShuffled {PawnNameSlot.First} {Gender.Male} {nameBank.NamesFor(PawnNameSlot.First, Gender.Male).Count}");
            Log.Message($"PawnNameDatabaseShuffled {PawnNameSlot.First} {Gender.Female} {nameBank.NamesFor(PawnNameSlot.First, Gender.Female).Count}");
            Log.Message($"PawnNameDatabaseShuffled {PawnNameSlot.Nick} {Gender.Male} {nameBank.NamesFor(PawnNameSlot.Nick, Gender.Male).Count}");
            Log.Message($"PawnNameDatabaseShuffled {PawnNameSlot.Nick} {Gender.Female} {nameBank.NamesFor(PawnNameSlot.Nick, Gender.Female).Count}");
            Log.Message($"PawnNameDatabaseShuffled {PawnNameSlot.Nick} {Gender.None} {nameBank.NamesFor(PawnNameSlot.Nick, Gender.None).Count}");
            Log.Message($"PawnNameDatabaseShuffled {PawnNameSlot.Last} {Gender.None} {nameBank.NamesFor(PawnNameSlot.Last, Gender.None).Count}");
            Log.Message($"PawnNameDatabaseSolid {PawnNameDatabaseSolid.AllNames().Count()}");
        }
    }
}
