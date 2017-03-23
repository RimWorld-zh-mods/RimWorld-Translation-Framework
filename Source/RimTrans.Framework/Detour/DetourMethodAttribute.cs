using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RimTrans.Framework.Detour
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class DetourMethodAttribute : Attribute
    {
        public readonly MethodInfo sourceMethodInfo;
        public readonly string sourceMethodName;
        // store references needed for detours
        public readonly Type sourceType;

        // disable default constructor
        private DetourMethodAttribute() { }

        // get by name
        public DetourMethodAttribute(Type sourceType, string sourceMethodName)
        {
            this.sourceType = sourceType;
            this.sourceMethodName = sourceMethodName;
        }

        // get by methodInfo
        public DetourMethodAttribute(MethodInfo methodInfo)
        {
            sourceType = methodInfo.DeclaringType;
            sourceMethodInfo = methodInfo;
        }

        // check how this attribute was created
        public bool WasSetByMethodInfo
        {
            get { return sourceMethodInfo != null; }
        }
    }
}
