using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CheatModule.API
{
    public static class Logging
    {
        #region Info

        public static readonly string File_Current = Directory.GetCurrentDirectory() + "/" + CheatConfig.LogName + ".log";
        public static readonly string File_Old = Directory.GetCurrentDirectory() + "/" + CheatConfig.LogName + ".old.log";

        #endregion Info

        static Logging()
        {
            if (File.Exists(File_Old))
                File.Delete(File_Old);
            if (File.Exists(File_Current))
                File.Move(File_Current, File_Old);
        }

        #region Private Functions

        private static void Log(object log, Assembly asm, ConsoleColor color = ConsoleColor.White, string prefix = "[LOG]")
        {
            log = prefix + " " + asm.GetName().Name + " >> " + log;

            File.AppendAllText(File_Current, log + Environment.NewLine);
            if (CheatConfig.UseUnityLog)
                Debug.Log(log);
        }

        #endregion Private Functions

        #region Public Functions

        public static void Log(object log) =>
            Log(log, Assembly.GetCallingAssembly());

        public static void LogError(object log, Exception ex)
        {
            Log(log, Assembly.GetCallingAssembly(), ConsoleColor.Red, "[ERROR]");
            Log(ex, Assembly.GetCallingAssembly(), ConsoleColor.DarkRed, "[EXCEPTION]");
        }

        public static void LogWarning(object log) =>
            Log(log, Assembly.GetCallingAssembly(), ConsoleColor.Yellow, "[WARNING]");

        public static void LogImportant(object log) =>
            Log(log, Assembly.GetCallingAssembly(), ConsoleColor.Cyan, "[IMPORTANT]");

        #endregion Public Functions
    }
}