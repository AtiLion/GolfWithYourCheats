using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CheatModule.API.Detours
{
    public static class DetourManager
    {
        #region Public Properties

        public static Dictionary<MethodInfo, DetourInfo> DetouredMethods { get; private set; } = new Dictionary<MethodInfo, DetourInfo>();

        #endregion Public Properties

        #region Public Functions

        public static bool IsDetoured(MethodInfo method) =>
            DetouredMethods.ContainsKey(method);

        public static bool Detour(MethodInfo original, MethodInfo modified)
        {
            if (IsDetoured(original))
                return false;

            // JIT the methods
            RuntimeHelpers.PrepareMethod(original.MethodHandle);
            RuntimeHelpers.PrepareMethod(modified.MethodHandle);

            // Load pointers
            IntPtr ptrOriginal = original.MethodHandle.GetFunctionPointer();
            IntPtr ptrModified = original.MethodHandle.GetFunctionPointer();

            // Run
            DetouredMethods.Add(original, new DetourInfo(original, modified));
            return RedirectionHelper.DetourFunction(ptrOriginal, ptrModified);
        }

        public static bool Revert(MethodInfo method)
        {
            if (!IsDetoured(method))
                return false;

            bool status = RedirectionHelper.RevertDetour(DetouredMethods[method].Offsets);

            if (status)
                DetouredMethods.Remove(method);
            return status;
        }

        public static object CallOriginal(MethodInfo method, object instance = null, params object[] args)
        {
            if (!IsDetoured(method))
                throw new NotDetouredException();
            DetourInfo info = DetouredMethods[method];
            if (!Revert(method))
                throw new RevertFailException();

            object result = null;
            try
            {
                result = method.Invoke(instance, args);
            }
            catch (Exception ex)
            {
                result = ex;
            }

            if (!Detour(info.Original, info.Modified))
                throw new DetourFailException();
            if (typeof(Exception).IsAssignableFrom(result.GetType()))
                throw (Exception)result;

            return result;
        }

        #endregion Public Functions

        #region Sub Classes

        private static class RedirectionHelper
        {
            public static bool DetourFunction(IntPtr ptrOriginal, IntPtr ptrModified)
            {
                try
                {
                    switch (IntPtr.Size)
                    {
                        case sizeof(Int32):
                            unsafe
                            {
                                byte* ptrFrom = (byte*)ptrOriginal.ToPointer();

                                *ptrFrom = 0x68; // PUSH
                                *((uint*)(ptrFrom + 2)) = (uint)ptrModified.ToInt32(); // Pointer
                                *(ptrFrom + 8) = 0xC3; // RETN
                            }
                            break;

                        case sizeof(Int64):
                            unsafe
                            {
                                byte* ptrFrom = (byte*)ptrOriginal.ToPointer();

                                *ptrFrom = 0x48; // REX.W
                                *(ptrFrom + 1) = 0xB8; // MOV
                                *((ulong*)(ptrFrom + 2)) = (ulong)ptrModified.ToInt64(); // Pointer
                                *(ptrFrom + 10) = 0xFF; // INC 1
                                *(ptrFrom + 11) = 0xE0; // LOOPE
                            }
                            break;

                        default:
                            return false;
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logging.LogError("Error adding detour!", ex);
                    return false;
                }
            }

            public static bool RevertDetour(OffsetBackup backup)
            {
                try
                {
                    unsafe
                    {
                        byte* ptrOriginal = (byte*)backup.Method.ToPointer();

                        *ptrOriginal = backup.A;
                        *(ptrOriginal + 1) = backup.B;
                        *(ptrOriginal + 10) = backup.C;
                        *(ptrOriginal + 11) = backup.D;
                        *(ptrOriginal + 12) = backup.E;
                        if (IntPtr.Size == sizeof(Int32))
                        {
                            *((uint*)(ptrOriginal + 2)) = backup.F32;
                            *(ptrOriginal + 8) = backup.G;
                        }
                        else
                        {
                            *((ulong*)(ptrOriginal + 2)) = backup.F64;
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logging.LogError("Error reverting detour!", ex);
                    return false;
                }
            }
        }

        public class OffsetBackup
        {
            #region Variables

            public IntPtr Method;

            public byte A, B, C, D, E, G;
            public ulong F64;
            public uint F32;

            #endregion Variables

            public OffsetBackup(IntPtr method)
            {
                Method = method;

                unsafe
                {
                    byte* ptrMethod = (byte*)method.ToPointer();

                    A = *ptrMethod;
                    B = *(ptrMethod + 1);
                    C = *(ptrMethod + 10);
                    D = *(ptrMethod + 11);
                    E = *(ptrMethod + 12);
                    if (IntPtr.Size == sizeof(Int32))
                    {
                        F32 = *((uint*)(ptrMethod + 2));
                        G = *(ptrMethod + 8);
                    }
                    else
                    {
                        F64 = *((ulong*)(ptrMethod + 2));
                    }
                }
            }
        }

        public class DetourInfo
        {
            public OffsetBackup Offsets;
            public MethodInfo Modified;
            public MethodInfo Original;

            public DetourInfo(MethodInfo original, MethodInfo modified)
            {
                Modified = modified;
                Original = original;

                Offsets = new OffsetBackup(Original.MethodHandle.GetFunctionPointer());
            }
        }

        #endregion Sub Classes
    }
}