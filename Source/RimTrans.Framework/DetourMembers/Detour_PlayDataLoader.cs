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

            // Clear BackstoryDatabase
            BackstoryDatabase.Clear();

            // Append: Clear SolidBioDatabase and PawnNameDatabaseSolid
            PawnBioLoaderUtility.Clear();

            // Append: Clear PawnNameDatabaseShuffled
            ShuffledNameLoaderUtility.Clear();

            Current.Game = null;

            //PlayDataLoader.loadedInt = false;
            Type t_PlayDataLoader = typeof(PlayDataLoader);
            FieldInfo f_loadedInt = t_PlayDataLoader.GetField("loadedInt", BindingFlags.NonPublic | BindingFlags.Static);
            f_loadedInt.SetValue(null, false);
        }
    }
}
