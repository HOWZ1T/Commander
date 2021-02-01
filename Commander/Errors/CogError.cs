using System;

namespace Commander.Errors
{
    public class CogError : Exception
    {
        public CogError(string msg) : base(msg) { }
    }
}