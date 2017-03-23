using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using RimTrans.Framework.Detour;

namespace RimTrans.Framework.DetourMembers
{
    public static class Detour_PawnNameDatabaseShuffled
    {
        // This method do not need to be detoured actually.
        public static void LoadAllNames()
        {
#if DEBUG
            Log.Message("Detour_PawnNameDatabaseShuffled.LoadAllNames()");
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
    }
}
