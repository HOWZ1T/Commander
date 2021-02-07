using System;

namespace Commander.Attributes
{
    /// <summary>
    ///     Command attribute to specify that a method is a command within the framework.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class Command : Attribute
    {
        private string _description;

        public Command()
        {
            _description = "Not given.";
        }

        public virtual string Name { get; set; }

        public virtual string Description
        {
            get => _description;
            set => _description = value;
        }

        public virtual string Parent { get; set; }
    }
}