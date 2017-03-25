using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using RimTrans.Framework.DetourMembers;

namespace RimTrans.Framework.Utility
{
    public static class UtilityHelper
    {
        public static void Initialize()
        {
            // Clear BackstoryDatabase
            BackstoryDatabase.Clear();

            // Append: Clear SolidBioDatabase and PawnNameDatabaseSolid
            PawnBioLoaderUtility.Clear();

            // Append: Clear PawnNameDatabaseShuffled
            ShuffledNameLoaderUtility.Clear();

            Detour_BackstoryDatabase.ReloadAllBackstories();
            BackstoryTranslationUtility.LoadAndInjectBackstoryData(LanguageDatabase.activeLanguage);
            ProceseAllTranslationUtilities();
        }

        public static void ProceseAllTranslationUtilities()
        {
            if (LanguageDatabase.activeLanguage.folderName != LanguageDatabase.DefaultLangFolderName)
            {
                PawnBioTranslationUtility.InjectIntoAllBios();
                ShuffledNameTranslationUtility.InjectIntoAllNames();
            }

            PawnBioLoaderUtility.ResolveAllBiosNameMissingPieces();
        }
    }
}
