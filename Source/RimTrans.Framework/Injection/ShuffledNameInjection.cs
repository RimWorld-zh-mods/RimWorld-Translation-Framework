using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RimTrans.Framework.Injection
{
    public class NamePair
    {
        public string englishName;
        public string nativeName;
    }

    public class ShuffledNameInjection : Def
    {
        public List<NamePair> namePairs;
    }
}
