using System;

namespace CheatModule.API.Detours
{
    public class DetourFailException : Exception
    {
        public DetourFailException() : base("Failed to detour method")
        {
        }
    }
}