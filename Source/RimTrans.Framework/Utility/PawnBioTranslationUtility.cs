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
#if DEBUG
            Log.Message("PawnBioTranslationUtility.InjectIntoAllBios()");
            int temp = 0;
            var debugPawnBioInjection = new PawnBioInjection();
            foreach (var pawnBio in SolidBioDatabase.allBios)
            {
                debugPawnBioInjection.first = $"起灵Debug{temp}";
                debugPawnBioInjection.nick = $"大魔王Debug{temp}";
                debugPawnBioInjection.last = $"风Debug{temp}";
                pawnBio.name.SetValueByInjection(debugPawnBioInjection);
                temp++;
            }
            foreach (var name in PawnNameDatabaseSolid.AllNames())
            {
                debugPawnBioInjection.first = $"起灵Debug{temp}";
                debugPawnBioInjection.nick = $"大魔王Debug{temp}";
                debugPawnBioInjection.last = $"风Debug{temp}";
                name.SetValueByInjection(debugPawnBioInjection);
                temp++;
            }
            //return;
#endif
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
        }
    }
}
