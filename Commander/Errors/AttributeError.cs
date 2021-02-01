using System;

namespace Commander.Errors
{
    /// <summary>
    /// An Error class representing Attribute Errors.
    /// </summary>
    public class AttributeError : Exception
    {
        public AttributeError(string msg) : base(msg) {}
    }
}