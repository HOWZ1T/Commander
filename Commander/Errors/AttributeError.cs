using System;

namespace Commander.Errors
{
    public class AttributeError : Exception
    {
        public AttributeError(string msg) : base(msg) {}
    }
}