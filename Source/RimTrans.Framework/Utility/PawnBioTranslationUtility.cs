using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using RimTrans.Framework.Detour;
using RimTrans.Framework.Injection;

namespace RimTrans.Framework.Utility
{
    public static class PawnBioTranslationUtility
    {
        private static int countMissings = 0;
        private readonly static StringBuilder missings = new StringBuilder();

        public static void InjectIntoAllBios()
        {
            countMissings = 0;
            missings.Remove(0, missings.Length);
            IEnumerable<PawnBio> allPawnBios = SolidBioDatabase.allBios;
            IEnumerable<PawnBioInjection> allPawnBioDefs = DefDatabase<PawnBioInjection>.AllDefs;
            InjectIntoPawnBiosByInjections(allPawnBios, allPawnBioDefs);
            InjectIntoSolidNamesByInjections(
                PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Male),
                allPawnBioDefs.Where(d1 => d1.gender == GenderPossibility.Male));
            InjectIntoSolidNamesByInjections(
                PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Female),
                allPawnBioDefs.Where(d1 => d1.gender == GenderPossibility.Female));
            InjectIntoSolidNamesByInjections(
                PawnNameDatabaseSolid.GetListForGender(GenderPossibility.Either),
                allPawnBioDefs.Where(d1 => d1.gender == GenderPossibility.Either));

            if (missings.Length > 0)
            {
                string message = $"Folloing {countMissings} PawnBios can not find matching PawnBioInjections:\n" + missings.ToString();
                Log.Warning(message.ToString());
            }
            countMissings = 0;
            missings.Remove(0, missings.Length);
        }

        private static void InjectIntoPawnBiosByInjections(IEnumerable<PawnBio> pawnBios, IEnumerable<PawnBioInjection> pawnBioInjections)
        {
            foreach (PawnBio pawnBio in pawnBios)
            {
                NameTriple pawnName = pawnBio.name;
                string identifier = pawnName.ToString();
                PawnBioInjection pawnBioInjection = null;
                foreach (PawnBioInjection current in pawnBioInjections)
                {
                    if (current.identifier == identifier)
                    {
                        pawnBioInjection = current;
                        break;
                    }
                }
                if (pawnBioInjection == null)
                {
                    countMissings++;
                    missings.Append(identifier);
                    missings.Append(", ");
                }
                else
                {
                    try
                    {
                        pawnBio.name.SetValueByInjection(pawnBioInjection);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }
                }
            }
#if DEBUG
            StringBuilder sb = new StringBuilder($"PawnBioTranslationUtility {pawnBios.First().gender.ToString()}\n\n");
            foreach (NameTriple name in pawnBios.Select(p => p.name))
            {
                sb.Append(name.ToString());
                sb.Append(" | ");
            }
            Log.Message(sb.ToString());
#endif
        }

        private static void InjectIntoSolidNamesByInjections(IEnumerable<NameTriple> solidNames, IEnumerable<PawnBioInjection> pawnBioInjections)
        {
            foreach (NameTriple name in solidNames)
            {
                NameTriple pawnName = name;
                string identifier = pawnName.ToString();
                PawnBioInjection pawnBioInjection = null;
                foreach (PawnBioInjection current in pawnBioInjections)
                {
                    if (current.identifier == identifier)
                    {
                        pawnBioInjection = current;
                        break;
                    }
                }
                if (pawnBioInjection == null)
                {
                    countMissings++;
                    missings.Append(identifier);
                    missings.Append(" | ");
                }
                else
                {
                    try
                    {
                        name.SetValueByInjection(pawnBioInjection);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }
                }
            }
#if DEBUG
            StringBuilder sb = new StringBuilder($"PawnBioTranslationUtility SolidNames\n\n");
            foreach (NameTriple name in solidNames)
            {
                sb.Append(name.ToString());
                sb.Append(" | ");
            }
            Log.Message(sb.ToString());
#endif
        }
    }
}
