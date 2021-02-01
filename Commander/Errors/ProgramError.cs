using System;

namespace Commander.Errors
{
    public class ProgramError : Exception
    {
        public ProgramError(string msg) : base(msg) {}
    }
}