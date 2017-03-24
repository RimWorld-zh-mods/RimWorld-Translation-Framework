using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RimTrans.Framework.Injection
{
    public class BackstoryInjection : Def
    {
        public BackstorySlot slot;

        public string title;

        public string titleShort;

        public string baseDesc;

        public Dictionary<string, int> skillGains = new Dictionary<string, int>();

        public Dictionary<SkillDef, int> skillGainsResolved = new Dictionary<SkillDef, int>();

        public WorkTags workDisables;

        public WorkTags requiredWorkTags;

        public List<string> spawnCategories = new List<string>();

        [LoadAlias("bodyNameGlobal")]
        public BodyType bodyTypeGlobal;

        [LoadAlias("bodyNameFemale")]
        public BodyType bodyTypeFemale;

        [LoadAlias("bodyNameMale")]
        public BodyType bodyTypeMale;
    }
}
