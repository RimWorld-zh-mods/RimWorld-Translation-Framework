using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RimTrans.Framework.Utility
{
    public static class UtilityHelper
    {
        public static void ProceseAllTranslationUtilities()
        {
            if (LanguageDatabase.activeLanguage.folderName == LanguageDatabase.DefaultLangFolderName)
                return;

            PawnBioTranslationUtility.InjectIntoAllBios();
            ShuffledNameTranslationUtility.InjectIntoAllNames();
        }
    }
}
