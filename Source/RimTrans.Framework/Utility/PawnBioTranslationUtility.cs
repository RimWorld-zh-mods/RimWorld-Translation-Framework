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
        private readonly static Dictionary<string, NameTriple> missings = new Dictionary<string, NameTriple>();

        public static void InjectIntoAllBios()
        {
#if DEBUG
            Log.Message("PawnBioTranslationUtility.InjectIntoAllBios()");
            int temp = 0;
            var debugPawnBioInjection = new PawnBioInjection();
            foreach (var pawnBio in SolidBioDatabase.allBios)
            {
                debugPawnBioInjection.name = new NameTriple($"起灵Debug{temp}", $"大魔王Debug{temp}", $"风Debug{temp}");
                pawnBio.name.SetValueByInjection(debugPawnBioInjection);
                temp++;
            }
            foreach (var name in PawnNameDatabaseSolid.AllNames())
            {
                debugPawnBioInjection.name = new NameTriple($"起灵Debug{temp}", $"大魔王Debug{temp}", $"风Debug{temp}");
                name.SetValueByInjection(debugPawnBioInjection);
                temp++;
            }
            //return;
#endif
            missings.Clear();
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

            if (missings.Count > 0)
            {
                StringBuilder message = new StringBuilder("Folloing PawnBios can not find matching PawnBioInjections:\n\n");
                foreach (var kvp in missings)
                {
                    message.Append(" - PawnBio: <");
                    message.Append(kvp.Value.ToString());
                    message.Append("> PawnBioInjection: <");
                    message.Append(kvp.Key);
                    message.Append(">\n");
                }
                Log.Warning(message.ToString());
            }
            missings.Clear();
        }

        private static void InjectIntoPawnBiosByInjections(IEnumerable<PawnBio> pawnBios, IEnumerable<PawnBioInjection> pawnBioInjections)
        {
            foreach (PawnBio pawnBio in pawnBios)
            {
                NameTriple pawnName = pawnBio.name;
                string defName = pawnName.First + "_" + pawnName.Last + "_" + pawnName.Nick;
                PawnBioInjection pawnBioDef = null;
                foreach (PawnBioInjection current in pawnBioInjections)
                {
                    if (current.defName == defName)
                    {
                        pawnBioDef = current;
                        break;
                    }
                }
                if (pawnBioDef == null)
                {
                    missings.Add(defName, pawnBio.name);
                }
                else
                {
                    pawnBio.name.SetValueByInjection(pawnBioDef);
                }
            }
        }

        private static void InjectIntoSolidNamesByInjections(IEnumerable<NameTriple> solidNames, IEnumerable<PawnBioInjection> pawnBioInjections)
        {
            foreach (NameTriple name in solidNames)
            {
                NameTriple pawnName = name;
                string defName = pawnName.First + "_" + pawnName.Last + "_" + pawnName.Nick;
                PawnBioInjection pawnBioDef = null;
                foreach (PawnBioInjection current in pawnBioInjections)
                {
                    if (current.defName == defName)
                    {
                        pawnBioDef = current;
                        break;
                    }
                }
                if (pawnBioDef == null)
                {
                    missings.Add(defName, name);
                }
                else
                {
                    name.SetValueByInjection(pawnBioDef);
                }
            }
        }
    }
}
