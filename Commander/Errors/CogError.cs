using System;

namespace Commander.Errors
{
    /// <summary>
    ///     An Error class representing Cog Errors.
    /// </summary>
    public class CogError : Exception
    {
        public CogError(string msg) : base(msg)
        {
        }
    }
}