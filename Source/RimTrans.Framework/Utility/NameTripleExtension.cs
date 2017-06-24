using System;
using System.Text;
using RimWorld;
using Verse;

namespace RimTrans.Framework.Utility {
    public static class NameTripleExtension {
        public static string ToDefName(this NameTriple name, GenderPossibility gender) {
            StringBuilder sb = new StringBuilder();
            sb.Append(gender.ToString());
            sb.Append("_");
            if (!string.IsNullOrEmpty(name.First.Trim())) {
                sb.Append(name.First.Replace("'", "").Replace(" ", ""));
            } else {
                sb.Append("EMPTY");
            }
            sb.Append("_");
            if (!string.IsNullOrEmpty(name.Last.Trim())) {
                sb.Append(name.Last.Replace("'", "").Replace(" ", ""));
            } else {
                sb.Append("EMPTY");
            }
            sb.Append("_");
            if (!string.IsNullOrEmpty(name.Nick.Trim())) {
                sb.Append(name.Nick.Replace("'", "").Replace(" ", ""));
            } else {
                sb.Append("EMPTY");
            }
            return sb.ToString();
        }
    }
}
