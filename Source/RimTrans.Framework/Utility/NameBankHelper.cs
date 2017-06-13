using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;


namespace RimTrans.Framework.Utility {
    public static class NameBankHelper {
        private readonly static Type t_NameBank = typeof(NameBank);
        private readonly static FieldInfo f_names = t_NameBank.GetField("names", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void Clear(this NameBank instance) {
            foreach (List<string> namesList in (List<string>[,])f_names.GetValue(instance)) {
                if (namesList != null) namesList.Clear();
            }
        }

        public static List<string> NamesFor(this NameBank instance, PawnNameSlot slot, Gender gender) {
            return ((List<string>[,])f_names.GetValue(instance))[(int)gender, (int)slot];
        }
    }
}
