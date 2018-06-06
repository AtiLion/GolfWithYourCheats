using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace CheatModule.API
{
    public static class DllInfo
    {
        #region Private Info

        private static Assembly dllAssembly = Assembly.GetExecutingAssembly();
        private static FileVersionInfo dllFileVersionInfo = FileVersionInfo.GetVersionInfo(dllAssembly.Location);

        #endregion Private Info

        #region Public Info

        public static readonly string Name = dllFileVersionInfo.ProductName;
        public static readonly string Creator = dllFileVersionInfo.CompanyName;
        public static readonly string Version = dllFileVersionInfo.ProductVersion;
        public static readonly string Description = dllFileVersionInfo.FileDescription;
        public static readonly bool IsDebugBuild = dllFileVersionInfo.IsDebug;
        public static readonly string Location = Path.GetDirectoryName(Uri.EscapeDataString((new UriBuilder(dllAssembly.CodeBase).Path)));

        #endregion Public Info
    }
}