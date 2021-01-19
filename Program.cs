using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Commander.Attributes;

namespace Commander
{
    public class Program
    {
        private Dictionary<string, Command> _commands = new Dictionary<string, Command>();
        public readonly string ProgramName;
        public readonly string Description;
        public HelpCommand HelpCommand;

        public Program()
        {
            var nameAttr = this.GetType().GetCustomAttribute(typeof(NameAttribute));
            if (nameAttr == null)
            {
                throw new AttributeError("missing name attribute");
            }
            
            var descriptionAttr = this.GetType().GetCustomAttribute(typeof(DescriptionAttribute));
            if (descriptionAttr != null)
            {
                this.Description = (descriptionAttr as DescriptionAttribute).Description;
            }
            else
            {
                this.Description = "Not available.";
            }

            this.ProgramName = (nameAttr as NameAttribute).Name;
            this.HelpCommand = new HelpCommand(this);
            Init();
        }

        public virtual void Init()
        {
            throw new NotImplementedException();
        }
        
        public void RegisterCommand(Command command)
        {
            this._commands.Add(command.Name, command);
        }

        public string ListCommands()
        {
            if (!_commands.Any())
            {
                return "This program has no commands.";
            }
            
            var s = "";
            foreach (var entry in _commands)
            {
                s += entry.Value.Signature + "\n";
            }

            return s.Substring(0, s.Length - 1);
        }

        public Command GetCommand(string name)
        {
            Command cmd = null;
            if (this._commands.TryGetValue(name, out cmd))
            {
                return cmd;
            }

            return null;
        }
        
        public void Run(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Invalid number of arguments\nUse '" + this.ProgramName +
                                  " help' for more information.");
                return;
            }

            string command = args[0];
            if (!this._commands.ContainsKey(command) && command != "help")
            {
                Console.WriteLine("Invalid command\nUse '" + this.ProgramName + " help' for more information.");
                return;
            }
            
            try
            {
                // rest of the args
                string[] rest = Utils.ShiftArgsLeft(args);
                DispatchCommand(command, rest);
            }
            catch (Exception e)
            {
                if (e is CommandError)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Use '" + ProgramName + " help " + command + "' for more info.");   
                }
                else
                {
                    Console.WriteLine("Unhandled error:");
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void DispatchCommand(string command, string[] rest)
        {
            if (command == "help")
            {
                this.HelpCommand.Run(rest);
                return;
            }
            
            if (!this._commands.ContainsKey(command)) return;
            this._commands[command].Execute(rest);
        }
    }
}