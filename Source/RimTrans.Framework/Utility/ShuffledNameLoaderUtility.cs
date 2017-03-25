using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using RimTrans.Framework.Detour;

namespace RimTrans.Framework.Utility
{
    public static class ShuffledNameLoaderUtility
    {
        public static void LoadAllNames()
        {
#if DEBUG
            Log.Message("ShuffledNameLoaderUtility.LoadAllNames()");
#endif
            NameBank nameBank = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard);
            nameBank.AddNamesFromFile(PawnNameSlot.First, Gender.Male, "First_Male");
            nameBank.AddNamesFromFile(PawnNameSlot.First, Gender.Female, "First_Female");
            nameBank.AddNamesFromFile(PawnNameSlot.Nick, Gender.Male, "Nick_Male");
            nameBank.AddNamesFromFile(PawnNameSlot.Nick, Gender.Female, "Nick_Female");
            nameBank.AddNamesFromFile(PawnNameSlot.Nick, Gender.None, "Nick_Unisex");
            nameBank.AddNamesFromFile(PawnNameSlot.Last, Gender.None, "Last");
            //PawnNameDatabaseShuffled.BankOf(PawnNameCategory.NoName).ErrorCheck(); // There is not NameBank of NoName.
            PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard).ErrorCheck();
        }

        public static void Clear()
        {
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
        }
    }
}
