using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;
using Harmony;
using RimTrans.Framework.Utility;

namespace RimTrans.Framework {
    public class RimTransFrameworkMod : Mod {

        public static readonly string ModIdentifier = "RimTransFramework";

        #region Construtor

        public RimTransFrameworkMod(ModContentPack content) : base(content) {
            // Harmony patches
            HarmonyInstance harmony = HarmonyInstance.Create(ModIdentifier);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        #endregion
    }
}
