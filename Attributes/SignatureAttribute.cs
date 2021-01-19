using System;

namespace Commander.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SignatureAttribute : Attribute
    {
    public readonly string Signature;

    public SignatureAttribute(string signature)
    {
        this.Signature = signature;
    }
    }
}