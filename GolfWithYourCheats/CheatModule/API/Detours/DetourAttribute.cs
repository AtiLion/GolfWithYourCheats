using System;
using System.Reflection;

namespace CheatModule.API.Detours
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DetourAttribute : Attribute
    {
        #region Public Properties

        public MethodInfo Modified { get; set; } = null;
        public MethodInfo Original { get; private set; } = null;

        #endregion Public Properties

        public DetourAttribute(MethodInfo original) =>
            Original = original;
    }
}