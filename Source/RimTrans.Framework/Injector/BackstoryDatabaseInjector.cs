using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RimTrans.Framework.Injector {
    public static class BackstoryDatabaseInjector {
        public static void LoadAllBackstories() {
            foreach (Backstory curBackstory in DirectXmlLoader.LoadXmlDataInResourcesFolder<Backstory>("Backstories/Shuffled")) {
                curBackstory.PostLoad();
                curBackstory.ResolveReferences();
                foreach (string curMessage in curBackstory.ConfigErrors(false)) {
                    Log.Error(curBackstory.Title + ": " + curMessage);
                }
                BackstoryDatabase.AddBackstory(curBackstory);
            }
        }
    }
}
