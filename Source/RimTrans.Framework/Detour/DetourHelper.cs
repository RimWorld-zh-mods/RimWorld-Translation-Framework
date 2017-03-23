using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RimTrans.Framework.Detour
{
    public static class DetourHelper
    {
        public const BindingFlags AllBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.Public |
            BindingFlags.NonPublic;

        internal static MethodInfo GetMatchingMethodInfo(DetourMethodAttribute sourceAttribute, MethodInfo destinationInfo)
        {
            // we should only ever get here in case the attribute was not defined with a sourceMethodInfo, but let's check just in case.
            if (sourceAttribute.WasSetByMethodInfo)
                return sourceAttribute.sourceMethodInfo;

            // aight, let's search by name
            MethodInfo[] candidates =
                sourceAttribute.sourceType.GetMethods(AllBindingFlags)
                    .Where(mi => mi.Name == sourceAttribute.sourceMethodName).ToArray();

            // if we only get one result, we've got our method info - if the length is zero, the method doesn't exist.
            if (candidates.Length == 0)
                return null;
            if (candidates.Length == 1)
                return candidates.First();

            // this is where things get slightly complicated, we'll have to search by parameters.
            var destinationParameterTypes = destinationInfo.GetParameters().Select(pi => pi.ParameterType).ToList();
            var extensionMethodType = GetExtensionMethodType(destinationInfo);
            if (extensionMethodType != null)
            {
                // the "this" parameter should not be used in the parameter sequence comparison
                destinationParameterTypes.RemoveAt(0);
            }

            candidates = candidates.Where(mi =>
                mi.ReturnType == destinationInfo.ReturnType &&
                (extensionMethodType == null || mi.DeclaringType == extensionMethodType) &&
                mi.GetParameters().Select(pi => pi.ParameterType).SequenceEqual(destinationParameterTypes)).ToArray();

            // if we only get one result, we've got our method info - if the length is zero, the method doesn't exist.
            if (candidates.Length == 0)
                return null;
            if (candidates.Length == 1)
                return candidates.First();

            // if we haven't returned anything by this point there were still multiple candidates. This is theoretically impossible,
            // unless I missed something.
            return null;
        }

        internal static string FullName(this MethodInfo methodInfo)
        {
            if (methodInfo == null) return "[null reference]";
            if (methodInfo.DeclaringType == null) return methodInfo.Name;
            return methodInfo.DeclaringType.FullName + "." + methodInfo.Name;
        }

        // returns the type of the first parameter is the method is an extension method, null otherwise
        internal static Type GetExtensionMethodType(MethodInfo method)
        {
            if (method.TryGetAttributeSafely<ExtensionAttribute>() != null)
            {
                return method.GetParameters().Select(p => p.ParameterType).FirstOrDefault();
            }
            return null;
        }

        internal static T TryGetAttributeSafely<T>(this MemberInfo member) where T : Attribute
        {
            try
            {
                var attrs = member.GetCustomAttributes(typeof(T), false);
                if (attrs.Length > 0) return (T)attrs[0];
            }
            catch
            {
                //mods could include attributes from libraries that are not loaded, which would throw an exception
            }
            return null;
        }
    }
}
