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
    public static class Detour_BackstoryDatabase
    {
        [DetourMethod(typeof(BackstoryDatabase), "ReloadAllBackstories")]
        public static void ReloadAllBackstories()
        {
#if DEBUG
            Log.Message("Detour_BackstoryDatabase.ReloadAllBackstories()");
#endif
            foreach (Backstory current in DirectXmlLoader.LoadXmlDataInResourcesFolder<Backstory>("Backstories/Shuffled"))
            {
                current.PostLoad();
                current.ResolveReferences();
                foreach (string current2 in current.ConfigErrors(false))
                {
                    Log.Error(current.Title + ": " + current2);
                }
                BackstoryDatabase.AddBackstory(current);
            }
            SolidBioDatabase.LoadAllBios();

            // Append: load PawnNameDatabaseShuffled
            Detour_PawnNameDatabaseShuffled.LoadAllNames();
        }
    }
}
