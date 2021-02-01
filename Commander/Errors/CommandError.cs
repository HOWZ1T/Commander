using System;

namespace Commander.Errors
{
    public class CommandError : Exception
    {
        public readonly string CallString;

        public CommandError(string msg, string callString) : base(msg)
        {
            this.CallString = callString;
        }
    }
}