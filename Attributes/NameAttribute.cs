using System;

namespace Commander.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NameAttribute : Attribute
    {
        public readonly string Name;
        
        public NameAttribute(string name)
        {
            this.Name = name;
        }
    }
}