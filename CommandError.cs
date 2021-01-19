using System;

namespace Commander
{
    public class CommandError : Exception
    {
        public CommandError(string message, Command cmd) : base(message) { }
    }
}