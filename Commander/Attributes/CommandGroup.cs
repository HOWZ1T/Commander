using System;

namespace Commander.Attributes
{
    /// <summary>
    ///     CommandGroup attribute to specify that a class is a 'group' for commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandGroup : Attribute
    {
        private string _description;
        private string _name;

        public CommandGroup(string name)
        {
            _name = name;
            _description = "Not given.";
        }

        public virtual string Name
        {
            get => _name;
            set => _name = value;
        }

        public virtual string Description
        {
            get => _description;
            set => _description = value;
        }
    }
}