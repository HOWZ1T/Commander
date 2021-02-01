using System;

namespace Commander.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class Example : Attribute
    {
        /*
         * Special characters for example:
         * @p = program name
         * @n = command name
         * @c = call string
         */
        public readonly string Val;

        public Example(string example)
        {
            this.Val = example;
        }
    }
}