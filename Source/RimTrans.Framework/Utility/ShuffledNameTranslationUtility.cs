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
    public static class ShuffledNameTranslationUtility
    {
        private readonly static Dictionary<string, StringBuilder> missings = new Dictionary<string, StringBuilder>();

        public static void InjectIntoAllNames()
        {
            missings.Clear();
            InjectIntoShuffledNamesByDefs(PawnNameSlot.First, Gender.Male, "First_Male");
            InjectIntoShuffledNamesByDefs(PawnNameSlot.First, Gender.Female, "First_Female");
            InjectIntoShuffledNamesByDefs(PawnNameSlot.Nick, Gender.Male, "Nick_Male");
            InjectIntoShuffledNamesByDefs(PawnNameSlot.Nick, Gender.Female, "Nick_Female");
            InjectIntoShuffledNamesByDefs(PawnNameSlot.Nick, Gender.None, "Nick_Unisex");
            InjectIntoShuffledNamesByDefs(PawnNameSlot.Last, Gender.None, "Last");

            if (missings.Count > 0)
            {
                StringBuilder message = new StringBuilder("Folloing ShuffledNames can not find matching ShuffledNamesInjections:\n\n");
                foreach (var kvp in missings)
                {
                    message.Append("defName:");
                    message.Append(kvp.Key);
                    message.Append("\n");
                    message.Append(kvp.Value);
                    message.Append("\n\n");
                }
                Log.Warning(message.ToString());
            }
            missings.Clear();
        }

        private static void InjectIntoShuffledNamesByDefs(PawnNameSlot slot, Gender gender, string defName)
        {
#if DEBUG
            var debugNames = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard).NamesFor(slot, gender);
            int count = debugNames.Count;
            debugNames.Clear();
            string temp = string.Empty;
            switch (slot)
            {
                case PawnNameSlot.First:
                    temp = $"起灵{gender}";
                    break;
                case PawnNameSlot.Last:
                    temp = $"风{gender}";
                    break;
                case PawnNameSlot.Nick:
                    temp = $"大魔王{gender}";
                    break;
                case PawnNameSlot.Only:
                    break;
                default:
                    break;
            }
            for (int i = 0; i < count; i++)
            {
                debugNames.Add(temp + i);
            }
#endif
            ShuffledNameInjection shuffledNameInjection = DefDatabase<ShuffledNameInjection>.GetNamed(defName);
            if (shuffledNameInjection == null)
                return;

            NameBank nameBank = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard);
            List<string> names = nameBank.NamesFor(slot, gender);
            List<NamePair> namePairs = shuffledNameInjection.namePairs;
            Dictionary<string, string> preNames = new Dictionary<string, string>();
            foreach (string name in names)
            {
                bool isMathced = false;
                foreach (NamePair namePair in namePairs)
                {
                    if (name == namePair.englishName)
                    {
                        preNames.Add(name, namePair.nativeName);
                        isMathced = true;
                        break;
                    }
                }
                if (!isMathced)
                {
                    preNames.Add(name, name);
                    AddToMissings(defName, name);
                }
            }
            names.Clear();
            names.AddRange(preNames.Values);
        }

        private static void AddToMissings(string defName, string name)
        {
            StringBuilder sb;
            if (missings.TryGetValue(defName, out sb))
            {
                sb.Append(", ");
                sb.Append(name);
            }
            else
            {
                missings.Add(defName, new StringBuilder(name));
            }
        }
    }
}
