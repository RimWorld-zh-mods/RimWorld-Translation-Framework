using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RimTrans.Framework.Injection
{
    public class PawnBioInjection : Def
    {
        public GenderPossibility gender;

        public NameTriple name;

        public BackstoryInjection childhood;

        public BackstoryInjection adulthood;

        public bool pirateKing;
    }
}
