using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using Verse;
using HugsLib;
using RimTrans.Framework.Core;

namespace RimTrans.Framework.Injector {
    public static class SolidBioDatabaseInjector {
        private readonly static Type t_NameTriple = typeof(NameTriple);
        private readonly static FieldInfo f_firstInt = t_NameTriple.GetField("firstInt", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly static FieldInfo f_nickInt = t_NameTriple.GetField("nickInt", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly static FieldInfo f_lastInt = t_NameTriple.GetField("lastInt", BindingFlags.NonPublic | BindingFlags.Instance);

        public static List<string> missingNames = new List<string>();

        public static void Inject() {
            foreach (PawnBio curPawnBio in DirectXmlLoader.LoadXmlDataInResourcesFolder<PawnBio>("Backstories/Solid")) {
                TransPawnBioName(curPawnBio);
                curPawnBio.name.ResolveMissingPieces(null);
                if (curPawnBio.childhood == null || curPawnBio.adulthood == null) {
                    PawnNameDatabaseSolid.AddPlayerContentName(curPawnBio.name, curPawnBio.gender);
                } else {
                    curPawnBio.PostLoad();
                    curPawnBio.ResolveReferences();
                    foreach (string curMessage in curPawnBio.ConfigErrors()) {
                        Log.Error(curMessage);
                    }
                    SolidBioDatabase.allBios.Add(curPawnBio);
                    curPawnBio.childhood.shuffleable = false;
                    curPawnBio.childhood.slot = BackstorySlot.Childhood;
                    curPawnBio.adulthood.shuffleable = false;
                    curPawnBio.adulthood.slot = BackstorySlot.Adulthood;
                    BackstoryDatabase.AddBackstory(curPawnBio.childhood);
                    BackstoryDatabase.AddBackstory(curPawnBio.adulthood);
                }
            }
            if (missingNames.Count > 0) {
                RimTransFrameworkMod.Instance.Logger.Warning("[RimWorld Translation Framework] Missing pawn name translations:\n"
                    + string.Join("; ", missingNames.ToArray()));
            }
        }

        public static void TransPawnBioName(PawnBio bio) {
            string identifier = bio.gender.ToString() + NameToIdentifier(bio.name);
            SolidNameDef pawnNameDef = DefDatabase<SolidNameDef>.GetNamed(identifier, false);
            if (pawnNameDef != null) {
                TransName(bio.name, pawnNameDef);
            } else {
                missingNames.Add(bio.name.ToString());
            }
        }

        public static string NameToIdentifier(NameTriple name) {
            StringBuilder sb = new StringBuilder();
            sb.Append("_");
            if (!string.IsNullOrEmpty(name.First)) {
                sb.Append(name.First.Trim());
            } else {
                sb.Append("EMPTY");
            }
            sb.Append("_");
            if (!string.IsNullOrEmpty(name.Last.Trim())) {
                sb.Append(name.Last);
            } else {
                sb.Append("EMPTY");
            }
            sb.Append("_");
            if (!string.IsNullOrEmpty(name.Nick.Trim())) {
                sb.Append(name.Nick);
            } else {
                sb.Append("EMPTY");
            }
            return sb.ToString();
        }

        public static void TransName(NameTriple name, SolidNameDef def) {
            f_firstInt.SetValue(name, def.first);
            f_lastInt.SetValue(name, def.first);
            f_nickInt.SetValue(name, def.nick);
        }
    }
}
