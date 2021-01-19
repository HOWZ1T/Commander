using System;

namespace Commander.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExampleAttribute : Attribute
    {
        public readonly string Example;

        public ExampleAttribute(string example)
        {
            this.Example = example;
        }
    }
}