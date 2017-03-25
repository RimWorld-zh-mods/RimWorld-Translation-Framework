using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RimTrans.Framework.Utility
{
    public static class PawnBioLoaderUtility
    {
        public static void LoadAllBios()
        {
            foreach (PawnBio current in DirectXmlLoader.LoadXmlDataInResourcesFolder<PawnBio>("Backstories/Solid"))
            {
                //current.name.ResolveMissingPieces(null); // Move this to after tranlation
                if (current.childhood == null || current.adulthood == null)
                {
                    PawnNameDatabaseSolid.AddPlayerContentName(current.name, current.gender);
                }
                else
                {
                    current.PostLoad();
                    current.ResolveReferences();
                    foreach (string current2 in current.ConfigErrors())
                    {
                        Log.Error(current2);
                    }
                    SolidBioDatabase.allBios.Add(current);
                    current.childhood.shuffleable = false;
                    current.childhood.slot = BackstorySlot.Childhood;
                    current.adulthood.shuffleable = false;
                    current.adulthood.slot = BackstorySlot.Adulthood;
                    BackstoryDatabase.AddBackstory(current.childhood);
                    BackstoryDatabase.AddBackstory(current.adulthood);
                }
            }
        }

        public static void Clear()
        {
            SolidBioDatabase.Clear();

            PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Either).Clear();
            PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Male).Clear();
            PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Female).Clear();
        }

        public static void ResolveAllBiosNameMissingPieces()
        {
            foreach (NameTriple name in PawnNameDatabaseSolid.AllNames())
            {
                name.ResolveMissingPieces(null);
            }
            foreach (PawnBio pawnBio in SolidBioDatabase.allBios)
            {
                pawnBio.name.ResolveMissingPieces(null);
            }
        }
    }
}
