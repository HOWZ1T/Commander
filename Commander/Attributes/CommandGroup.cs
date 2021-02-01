using System;

namespace Commander.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandGroup : Attribute
    {
        private string _name;
        private string _description;

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