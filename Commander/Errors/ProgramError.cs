using System;

namespace Commander.Errors
{
    /// <summary>
    ///     An Error class representing Program Errors.
    /// </summary>
    public class ProgramError : Exception
    {
        public ProgramError(string msg) : base(msg)
        {
        }
    }
}