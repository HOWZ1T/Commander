using System;

namespace Commander.Errors
{
    /// <summary>
    /// An Error class representing Command Errors.
    /// </summary>
    public class CommandError : Exception
    {
        public readonly string CallString;

        public CommandError(string msg, string callString) : base(msg)
        {
            this.CallString = callString;
        }
    }
}