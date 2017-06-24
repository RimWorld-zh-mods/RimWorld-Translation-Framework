using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using Verse;

namespace RimTrans.Framework.Utility {
    public static class NameBankExtension {
        //FieldInfo of NameBank.names
        private static readonly FieldInfo f_names = typeof(NameBank).GetField("names", BindingFlags.Instance | BindingFlags.NonPublic);

        #region Methods

        public static List<string>[,] AllNames(this NameBank nameBank) {
            return f_names.GetValue(nameBank) as List<string>[,];
        }

        public static void Clear(this NameBank nameBank) {
            foreach (List<string> namesList in nameBank.AllNames()) {
                if (namesList != null)
                    namesList.Clear();
            }
        }

        public static List<string> NamesFor(this NameBank nameBank, PawnNameSlot slot, Gender gender) {
            return nameBank.AllNames()[(int)gender, (int)slot];
        }

        public static void AddNamesDedup(this NameBank nameBank, PawnNameSlot slot, Gender gender, IEnumerable<string> namesToAdd) {
            List<string> names = nameBank.NamesFor(slot, gender);
            foreach (string curName in namesToAdd) {
                if (!names.Contains(curName)) {
                    names.Add(curName);
                }
            }
        }

        #endregion
    }
}
