using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HugsLib;
using HugsLib.Utils;
using RimTrans.Framework.Utility;

namespace RimTrans.Framework.Core {
    public class RimTransFrameworkMod : ModBase {
        public static RimTransFrameworkMod Instance { get; private set; }

        internal new ModLogger Logger {
            get { return base.Logger; }
        }

        public RimTransFrameworkMod() {
            Instance = this;
        }

        public override string ModIdentifier {
            get { return "RimTrans-Framework"; }
        }

        public override void Initialize() {
            GlobalInjector.Inject();
            PawnBioAndBacktoryUtility.AddAllToDatabase();
        }
    }
}
