using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RimTrans.Framework
{
    public class PawnBioDef : Def
    {
        public GenderPossibility gender;

        public NameTriple name;

        public string childhood;

        public string adulthood;

        public bool pirateKing;
    }
}
