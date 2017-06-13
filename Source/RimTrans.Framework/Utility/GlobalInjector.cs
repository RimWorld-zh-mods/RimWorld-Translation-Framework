using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using Verse;
using RimTrans.Framework.Injector;

namespace RimTrans.Framework.Utility {
    public static class GlobalInjector {
        public static void Inject() {
            if (LanguageDatabase.activeLanguage.FriendlyNameEnglish != "English") {
                BackstoryDatabase.Clear();
                BackstoryDatabaseInjector.Inject();

                PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Either).Clear();
                PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Female).Clear();
                PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Male).Clear();
                SolidBioDatabase.Clear();
                SolidBioDatabaseInjector.Inject();

                PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard).Clear();
                PawnNameDatabaseShuffledInjector.Inject();

                BackstoryTranslationUtility.LoadAndInjectBackstoryData(LanguageDatabase.activeLanguage);
            }
        }
    }
}
