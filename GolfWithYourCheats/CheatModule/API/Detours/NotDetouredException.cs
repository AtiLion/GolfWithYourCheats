using System;

namespace CheatModule.API.Detours
{
    public class NotDetouredException : Exception
    {
        public NotDetouredException() : base("Method not detoured!")
        {
        }
    }
}