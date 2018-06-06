using System;

namespace CheatModule.API.Detours
{
    public class RevertFailException : Exception
    {
        public RevertFailException() : base("Failed to revert method")
        {
        }
    }
}