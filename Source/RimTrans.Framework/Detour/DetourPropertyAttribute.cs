using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RimTrans.Framework.Detour
{
    [Flags]
    public enum DetourProperty
    {
        None = 0x0,
        Getter = 0x1,
        Setter = 0x2,
        Both = Getter | Setter
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class DetourPropertyAttribute : Attribute
    {
        public readonly DetourProperty detourProperty;
        // store references needed for detours
        public readonly PropertyInfo sourcePropertyInfo;

        // disable default constructor
        private DetourPropertyAttribute() { }

        // get by name
        public DetourPropertyAttribute(Type sourceType, string sourcePropertyName,
                                        DetourProperty detourProperty = DetourProperty.Both)
        {
            sourcePropertyInfo = sourceType.GetProperty(sourcePropertyName, DetourHelper.AllBindingFlags);
            this.detourProperty = detourProperty;
        }

        // get by propertyInfo
        public DetourPropertyAttribute(PropertyInfo sourcePropertyInfo,
                                        DetourProperty detourProperty = DetourProperty.Both)
        {
            this.sourcePropertyInfo = sourcePropertyInfo;
            this.detourProperty = detourProperty;
        }
    }
}
