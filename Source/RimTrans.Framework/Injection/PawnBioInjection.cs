using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RimTrans.Framework.Injection
{
    public class TripleName
    {
        public string first;

        public string last;

        public string nick;
    }

    public class PawnBioInjection : Def
    {
        public string identifier;

        public GenderPossibility gender;

        public TripleName name;

        public BackstoryInjection childhood;

        public BackstoryInjection adulthood;

        public bool pirateKing;
    }
}
