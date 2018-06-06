using System;
using System.Collections.Generic;
using System.Reflection;

namespace CheatModule.API
{
    public static class Reflection
    {
        #region Public Properties

        public static Dictionary<string, FieldInfo> Fields { get; private set; } = new Dictionary<string, FieldInfo>();
        public static Dictionary<string, MethodInfo> Methods { get; private set; } = new Dictionary<string, MethodInfo>();

        #endregion Public Properties

        #region Public Functions

        public static bool IsFieldPrepared(string name) =>
            Fields.ContainsKey(name);

        public static bool IsFieldPrepared(FieldInfo field) =>
            Fields.ContainsValue(field);

        public static bool IsFieldPrepared(Type type, string name) =>
            IsFieldPrepared(type.FullName + "." + name);

        public static bool HasFieldFlags(FieldInfo field, BindingFlags flags)
        {
            if (flags.HasFlags(BindingFlags.Public) && !field.IsPublic)
            {
                if (!flags.HasFlags(BindingFlags.NonPublic))
                    return false;
            }
            else if (flags.HasFlags(BindingFlags.NonPublic) && field.IsPublic)
            {
                return false;
            }
            if (flags.HasFlags(BindingFlags.Static) && !field.IsStatic)
            {
                if (!flags.HasFlags(BindingFlags.Instance))
                    return false;
            }
            else if (flags.HasFlags(BindingFlags.Instance) && field.IsStatic)
            {
                return false;
            }

            return true;
        }

        public static bool IsMethodPrepared(string name) =>
            Methods.ContainsKey(name);

        public static bool IsMethodPrepared(MethodInfo method) =>
            Methods.ContainsValue(method);

        public static bool IsMethodPrepared(Type type, string name) =>
            IsMethodPrepared(type.FullName + "." + name);

        public static bool HasMethodFlags(MethodInfo method, BindingFlags flags)
        {
            if (flags.HasFlags(BindingFlags.Public) && !method.IsPublic)
            {
                if (!flags.HasFlags(BindingFlags.NonPublic))
                    return false;
            }
            else if (flags.HasFlags(BindingFlags.NonPublic) && method.IsPublic)
            {
                return false;
            }
            if (flags.HasFlags(BindingFlags.Static) && !method.IsStatic)
            {
                if (flags.HasFlags(BindingFlags.Instance))
                    return false;
            }
            else if (flags.HasFlags(BindingFlags.Instance) && method.IsStatic)
            {
                return false;
            }

            return true;
        }

        public static FieldInfo PrepareField(Type type, string name, BindingFlags flags) =>
            PrepareField(type.GetField(name, flags));

        public static FieldInfo PrepareField(FieldInfo field)
        {
            if (field == null)
                return null;
            if (IsFieldPrepared(field))
                return null;

            Fields.Add(field.DeclaringType.FullName + "." + field.Name, field);
            return field;
        }

        public static MethodInfo PrepareMethod(Type type, string name, BindingFlags flags) =>
            PrepareMethod(type.GetMethod(name, flags));

        public static MethodInfo PrepareMethod(MethodInfo method)
        {
            if (method == null)
                return null;
            if (IsMethodPrepared(method))
                return null;

            Methods.Add(method.DeclaringType.FullName + "." + method.Name, method);
            return method;
        }

        public static FieldInfo GetField(string field)
        {
            if (!IsFieldPrepared(field))
                return null;

            return Fields[field];
        }

        public static FieldInfo GetField(Type type, string name) =>
            GetField(type.FullName + "." + name);

        public static MethodInfo GetMethod(string method)
        {
            if (!IsMethodPrepared(method))
                return null;

            return Methods[method];
        }

        public static MethodInfo GetMethod(Type type, string name) =>
            GetMethod(type.FullName + "." + name);

        #endregion Public Functions

        #region Extension Functions

        public static bool HasFlags(this Enum tocheck, Enum checker)
        {
            if (tocheck.GetType() != checker.GetType())
                throw new Exception("Non matching enum types!");

            int iCheck = Convert.ToInt32(tocheck);
            int iChecker = Convert.ToInt32(checker);

            return (iChecker & iCheck) == iCheck;
        }

        public static FieldInfo Prepare(this FieldInfo field) =>
            PrepareField(field);

        public static MethodInfo Prepare(this MethodInfo method) =>
            PrepareMethod(method);

        public static MethodInfo GetMethodA(this Type type, string name, bool prepare = false) =>
            type.GetMethodB(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, prepare);

        public static MethodInfo GetMethodB(this Type type, string fullname) =>
            GetMethod(fullname);

        public static MethodInfo GetMethodB(this Type type, string name, BindingFlags flags, bool prepare = false)
        {
            MethodInfo mi;
            if (IsMethodPrepared(type, name))
                mi = GetMethod(type, name);
            else
                mi = type.GetMethod(name, flags);
            if (mi == null)
                return null;

            if (prepare && !IsMethodPrepared(mi))
                PrepareMethod(mi);
            return mi;
        }

        public static FieldInfo GetFieldA(this Type type, string name, bool prepare = false) =>
            type.GetFieldB(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, prepare);

        public static FieldInfo GetFieldB(this Type type, string fullname) =>
            GetField(fullname);

        public static FieldInfo GetFieldB(this Type type, string name, BindingFlags flags, bool prepare = false)
        {
            FieldInfo fi;
            if (IsFieldPrepared(type, name))
                fi = GetField(type, name);
            else
                fi = type.GetField(name, flags);
            if (fi == null)
                return null;

            if (prepare && !IsFieldPrepared(fi))
                PrepareField(fi);
            return fi;
        }

        #endregion Extension Functions
    }
}