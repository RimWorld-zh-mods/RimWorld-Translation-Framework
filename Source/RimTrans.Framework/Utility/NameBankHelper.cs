﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using RimTrans.Framework.Detour;


namespace RimTrans.Framework.Utility
{
    public static class NameBankHelper
    {
        private readonly static Type t_NameBank = typeof(NameBank);
        private readonly static FieldInfo f_names = t_NameBank.GetField("names", BindingFlags.NonPublic | BindingFlags.Instance);

        public static List<string> NamesFor(this NameBank instance, PawnNameSlot slot, Gender gender)
        {
            List<string>[,] names = (List < string >[,])f_names.GetValue(instance);
            return names[(int)gender, (int)slot];
        }
    }
}