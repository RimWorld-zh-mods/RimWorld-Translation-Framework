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
    public static class Detour_LoadedLanguage
    {
        [DetourMethod(typeof(LoadedLanguage), "InjectIntoData")]
        public static void InjectIntoData(this LoadedLanguage instance)
        {
            //if (!instance.dataIsLoaded)
            //{
            //    instance.LoadData();
            //}
            bool dataIsLoaded = (bool)typeof(LoadedLanguage).GetField("dataIsLoaded", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(instance);
            if (dataIsLoaded)
            {
                instance.LoadData();
            }

            foreach (DefInjectionPackage current in instance.defInjections)
            {
                current.InjectIntoDefs();
            }
            //BackstoryTranslationUtility.LoadAndInjectBackstoryData(this);

            // Append: Custom TranslationUtility
            UtilityHelper.ProceseAllTranslationUtilities();
        }
    }
}
