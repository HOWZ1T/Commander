using System;

namespace Commander
{
    public class AttributeError : Exception
    {
        public AttributeError(string msg) : base(msg) { }
    }
}