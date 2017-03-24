using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using RimTrans.Framework.Detour;
using RimTrans.Framework.Utility;

namespace RimTrans.Framework.DetourMembers
{
    public static class Detour_PlayDataLoader
    {
        [DetourMethod(typeof(PlayDataLoader), "ClearAllPlayData")]
        public static void ClearAllPlayData()
        {
#if DEBUG
            Log.Message("Detour_PlayDataLoader.ClearAllPlayData()");
#endif
            LanguageDatabase.Clear();
            LoadedModManager.ClearDestroy();
            foreach (Type current in typeof(Def).AllSubclasses())
            {
                GenGeneric.InvokeStaticMethodOnGenericType(typeof(DefDatabase<>), current, "Clear");
            }
            ThingCategoryNodeDatabase.Clear();
            BackstoryDatabase.Clear();
            SolidBioDatabase.Clear();

            // Append: Clear PawnNameDatabaseSolid
            PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Either).Clear();
            PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Male).Clear();
            PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Female).Clear();

            // Append: Clear PawnNameDatabaseShuffled
            NameBank nameBank = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard);
            Type t_NameBank = typeof(NameBank);
            FieldInfo f_names = t_NameBank.GetField("names", BindingFlags.NonPublic | BindingFlags.Instance);
            List<string>[,] names = (List<string>[,])f_names.GetValue(nameBank);
            nameBank.NamesFor(PawnNameSlot.First, Gender.Male).Clear();
            nameBank.NamesFor(PawnNameSlot.First, Gender.Female).Clear();
            nameBank.NamesFor(PawnNameSlot.Nick, Gender.Male).Clear();
            nameBank.NamesFor(PawnNameSlot.Nick, Gender.Female).Clear();
            nameBank.NamesFor(PawnNameSlot.Nick, Gender.None).Clear();
            nameBank.NamesFor(PawnNameSlot.Last, Gender.None).Clear();

            Current.Game = null;

            //PlayDataLoader.loadedInt = false;
            Type t_PlayDataLoader = typeof(PlayDataLoader);
            FieldInfo f_loadedInt = t_PlayDataLoader.GetField("loadedInt", BindingFlags.NonPublic | BindingFlags.Static);
            f_loadedInt.SetValue(null, false);
        }
    }
}
