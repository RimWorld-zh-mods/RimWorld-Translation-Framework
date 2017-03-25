using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;
using RimTrans.Framework.Detour;
using RimTrans.Framework.Utility;

namespace RimTrans.Framework
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        private const string ModIndentifier = "RimTrans.Framwork";

        static Startup()
        {
            if (GameObject.Find(ModIndentifier) != null)
            {
                Log.Error("'RimWorld Translation Framework' has already been activated.");
                return;
            }
            GameObject.DontDestroyOnLoad(new GameObject(ModIndentifier));

            string version = string.Empty;
            try
            {
                var asm = Assembly.GetAssembly(typeof(Startup));
                var verAttr = asm.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false).FirstOrDefault() as AssemblyFileVersionAttribute;
                version = verAttr.Version;
            }
            catch (Exception)
            {
            }

            Log.Message("RimWorld Translation Framework "/* + version*/);

            DetourProxy.ProcessAllDetours(); // Detour
#if DEBUG
            DetourProxy.CompletedAndLog();
#endif
            UtilityHelper.Initialize();
        }
    }
}
