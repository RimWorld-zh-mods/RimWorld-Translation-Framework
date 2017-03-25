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
    public static class NameTripleHelper
    {
        private readonly static Type t_NameTriple = typeof(NameTriple);
        private readonly static FieldInfo f_firstInt = t_NameTriple.GetField("firstInt", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly static FieldInfo f_nickInt = t_NameTriple.GetField("nickInt", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly static FieldInfo f_lastInt = t_NameTriple.GetField("lastInt", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void SetValueByInjection(this NameTriple instance, PawnBioInjection pawnBioInjection)
        {
            string first = pawnBioInjection.name.first;
            if (!string.IsNullOrEmpty(first))
                f_firstInt.SetValue(instance, first);

            string last = pawnBioInjection.name.last;
            if (!string.IsNullOrEmpty(last))
                f_lastInt.SetValue(instance, last);

            string nick = pawnBioInjection.name.nick;
            if (!string.IsNullOrEmpty(nick))
                f_nickInt.SetValue(instance, nick);
        }
    }
}
