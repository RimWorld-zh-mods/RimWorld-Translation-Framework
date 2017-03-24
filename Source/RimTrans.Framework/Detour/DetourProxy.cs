using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;

namespace RimTrans.Framework.Detour
{
    /// <summary>
    /// A tool for detouring methods
    /// </summary>
    public class DetourProxy
    {
        // keep track of detours
        private static readonly Dictionary<MethodInfo, MethodInfo> detours = new Dictionary<MethodInfo, MethodInfo>();

        public static unsafe bool TryDetour(MethodInfo source, MethodInfo destination)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (destination == null) throw new ArgumentNullException("destination");
            if (detours.ContainsKey(source)) throw new ArgumentException("RimTrans.Framework: The source method has already be detoured: " + destination.DeclaringType.FullName + "." + destination.Name + " @ 0x" + destination.MethodHandle.GetFunctionPointer().ToString("X" + (IntPtr.Size * 2)));

            detours.Add(source, destination);

            if (IntPtr.Size == sizeof(Int64))
            {
                // 64-bit systems use 64-bit absolute address and jumps
                // 12 byte destructive

                // Get function pointers
                long source_Base = source.MethodHandle.GetFunctionPointer().ToInt64();
                long destination_Base = destination.MethodHandle.GetFunctionPointer().ToInt64();

                // Native source address
                byte* pointer_Raw_Source = (byte*)source_Base;

                // Pointer to insert jump address into native code
                long* pointer_Raw_Address = (long*)(pointer_Raw_Source + 0x02);

                // Insert 64-bit absolute jump into native code (address in rax)
                // mov rax, immediate64
                // jmp [rax]
                *(pointer_Raw_Source + 0x00) = 0x48;
                *(pointer_Raw_Source + 0x01) = 0xB8;
                *pointer_Raw_Address = destination_Base; // ( Pointer_Raw_Source + 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 )
                *(pointer_Raw_Source + 0x0A) = 0xFF;
                *(pointer_Raw_Source + 0x0B) = 0xE0;
            }
            else
            {
                // 32-bit systems use 32-bit relative offset and jump
                // 5 byte destructive

                // Get function pointers
                int source_Base = source.MethodHandle.GetFunctionPointer().ToInt32();
                int destination_Base = destination.MethodHandle.GetFunctionPointer().ToInt32();

                // Native source address
                byte* pointer_Raw_Source = (byte*)source_Base;

                // Pointer to insert jump address into native code
                int* pointer_Raw_Address = (int*)(pointer_Raw_Source + 1);

                // Jump offset (less instruction size)
                int offset = (destination_Base - source_Base) - 5;

                // Insert 32-bit relative jump into native code
                *pointer_Raw_Source = 0xE9;
                *pointer_Raw_Address = offset;
            }

            // Done!
            return true;
        }

        // skip types that were already processed
        private static readonly HashSet<Type> seenTypes = new HashSet<Type>();

        public static void ProcessAllDetours()
        {
            foreach (var type in Assembly.GetAssembly(typeof(DetourProxy)).GetTypes())
            {
                if (seenTypes.Contains(type))
                    continue;

                seenTypes.Add(type);
                
                foreach (var memberInfo in type.GetMembers(DetourHelper.AllBindingFlags))
                {
                    try
                    {
                        DetourMethodAttribute detourMethodAttribute;
                        if (memberInfo.TryGetAttribute(out detourMethodAttribute))
                        {
                            DetourMethodByAttribute(memberInfo as MethodInfo, detourMethodAttribute);
                        }
                        DetourPropertyAttribute detourPropertyAttribute;
                        if (memberInfo.TryGetAttribute(out detourPropertyAttribute))
                        {
                            DetourPropertyByAttribute(memberInfo as PropertyInfo, detourPropertyAttribute);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error((memberInfo as MethodInfo).FullName() + "\n" + e.ToString());
                    }
                }
            }
        }

        public static void CompletedAndLog()
        {
            if (detours.Count == 0)
                return;

            StringBuilder message = new StringBuilder("Following methods have been detoured:\n\n");
            foreach (var kvp in detours)
            {
                message.Append(" - ");
                message.Append(kvp.Key.FullName());
                message.Append(" >>> ");
                message.Append(kvp.Value.FullName());
                message.Append('\n');
            }
            Log.Message(message.ToString());
        }

        private static void DetourMethodByAttribute(MethodInfo destination, DetourMethodAttribute detourMethodAttribute)
        {
            if (destination == null) throw new ArgumentNullException("desitination");
            if (detourMethodAttribute == null) throw new ArgumentNullException("detourMethodAttribute");
            
            var source = detourMethodAttribute.WasSetByMethodInfo
                ? detourMethodAttribute.sourceMethodInfo
                : DetourHelper.GetMatchingMethodInfo(detourMethodAttribute, destination);
            if (source == null)
                throw new NullReferenceException("MethodInfo source could not be found based on attribute");

            TryDetour(source, destination);
        }

        private static void DetourPropertyByAttribute(PropertyInfo destination, DetourPropertyAttribute detourPropertyAttribute)
        {
            if (destination == null) throw new ArgumentNullException("desitination");
            if (detourPropertyAttribute == null) throw new ArgumentNullException("detourPropertyAttribute");
            
            var source = detourPropertyAttribute.sourcePropertyInfo;
            if (source == null)
                throw new NullReferenceException("PropertyInfo source could not be found based on attribute");

            // if getter or both was flagged
            MethodInfo sourceGetter = null;
            MethodInfo destinationGetter = null;
            if ((detourPropertyAttribute.detourProperty & DetourProperty.Getter) == DetourProperty.Getter)
            {
                sourceGetter = source.GetGetMethod(true);
                destinationGetter = destination.GetGetMethod(true);

                // make sure we've got what we wanted.
                if (sourceGetter == null)
                    throw new NullReferenceException("MethodInfo sourceGetter could not be found based on attribute");
                if (destinationGetter == null)
                    throw new NullReferenceException("MethodInfo destinationGetter could not be found based on attribute");
            }

            // if setter or both was flagged
            MethodInfo sourceSetter = null;
            MethodInfo destinationSetter = null;
            if ((detourPropertyAttribute.detourProperty & DetourProperty.Setter) == DetourProperty.Setter)
            {
                sourceSetter = source.GetSetMethod(true);
                destinationSetter = destination.GetSetMethod(true);

                // make sure we've got what we wanted.
                if (sourceSetter == null)
                    throw new NullReferenceException("MethodInfo sourceSetter could not be found based on attribute");
                if (destinationSetter == null)
                    throw new NullReferenceException("MethodInfo destinationSetter could not be found based on attribute");
            }

            if ((detourPropertyAttribute.detourProperty & DetourProperty.Getter) == DetourProperty.Getter)
                TryDetour(sourceGetter, destinationGetter);

            if ((detourPropertyAttribute.detourProperty & DetourProperty.Setter) == DetourProperty.Setter)
                TryDetour(sourceSetter, destinationSetter);
        }
    }
}
