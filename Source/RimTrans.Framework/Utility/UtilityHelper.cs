using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using RimTrans.Framework.DetourMembers;
using RimTrans.Framework.Injection;

namespace RimTrans.Framework.Utility
{
    public static class UtilityHelper
    {
        public static void Initialize()
        {
            BackstoryDatabase.Clear();
            PawnBioLoaderUtility.Clear();
            ShuffledNameLoaderUtility.Clear();

            Detour_BackstoryDatabase.ReloadAllBackstories();
            ProceseAllTranslationUtilities();
        }

        public static void ProceseAllTranslationUtilities()
        {

            if (LanguageDatabase.activeLanguage.folderName != LanguageDatabase.DefaultLangFolderName)
            {
                BackstoryTranslationUtility.LoadAndInjectBackstoryData(LanguageDatabase.activeLanguage);
                PawnBioTranslationUtility.InjectIntoAllBios();
                ShuffledNameTranslationUtility.InjectIntoAllNames();
            }

            PawnBioLoaderUtility.ResolveAllBiosNameMissingPieces();
        }
    }
}
