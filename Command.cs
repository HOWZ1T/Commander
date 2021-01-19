using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using Commander.Attributes;

namespace Commander
{
    public class Command
    {
        public readonly string Name, Description, Signature;
        public readonly string[] Examples;
        public readonly ArgPolicy Policy;
        
        private Dictionary<string, Command> _subcommands = new Dictionary<string, Command>();
        
        public Command(ArgPolicy policy)
        {
            // gather attributes
            var nameAttr = this.GetType().GetCustomAttribute(typeof(NameAttribute));
            if (nameAttr == null)
            {
                throw new AttributeError("missing name attribute");
            }
            this.Name = (nameAttr as NameAttribute).Name;
            
            var sigAttr = this.GetType().GetCustomAttribute(typeof(SignatureAttribute));
            if (sigAttr == null)
            {
                throw new AttributeError("missing signature attribute");
            }
            this.Signature = (sigAttr as SignatureAttribute).Signature;

            var descriptionAttr = this.GetType().GetCustomAttribute(typeof(DescriptionAttribute));
            if (descriptionAttr != null)
            {
                this.Description = (descriptionAttr as DescriptionAttribute).Description;
            }
            else
            {
                this.Description = "Not available.";
            }

            var exampleAttrs = this.GetType().GetCustomAttributes(typeof(ExampleAttribute)).ToImmutableArray();
            var examples = new string[exampleAttrs.Length];
            for (int i = 0; i < exampleAttrs.Length; i++)
            {
                examples[i] = (exampleAttrs[i] as ExampleAttribute).Example;
            }
            this.Examples = examples;

            this.Policy = policy;
        }

        public Command(ArgPolicy policy, Command[] subcommands) : this(policy)
        {
            foreach (var subcommand in subcommands)
            {
                this.RegisterSubcommand(subcommand);
            }
        }

        public Command RegisterSubcommand(Command subcommand)
        {
            if (this._subcommands.ContainsKey(subcommand.Name)) return this;
            this._subcommands.Add(subcommand.Name, subcommand);
            return this;
        }
        
        public virtual void ValidateArgs(string[] args)
        {
            if (!this.Policy.isValid(args.Length))
            {
                throw new CommandError("Invalid number of arguments!", this);
            }
        }

        private string subcommands()
        {
            if (_subcommands.Count() <= 0)
            {
                return "None.";
            }
            
            var str = "";
            foreach (var entry in _subcommands)
            {
                str += entry.Value.Name + "\n";
            }

            return str.Substring(0, str.Length - 1);
        }

        private string examples()
        {
            if (Examples.Length <= 0)
            {
                return Signature;
            }
            
            var str = "";
            foreach (var example in Examples)
            {
                str += example + "\n";
            }

            return str.Substring(0, str.Length - 1);
        }

        public virtual string Help(Program program)
        {
            var hlp = String.Format(
@"       Name: {0}
      Usage: {1}
Subcommands: 
{2}

Description: 
{3}
  
Examples: 
{4}
", this.Name, this.Signature, this.subcommands(), this.Description, this.examples());
            hlp = hlp.Replace("@p", program.ProgramName);
            return hlp;
        }
        
        public virtual void Run(string[] args)
        {
            throw new NotImplementedException();
        }

        public bool HasSubcommand(string[] args)
        {
            // check if should execute sub command
            if (args.Length >= 1)
            {
                // do not attempt to execute subcommand for the help command
                if (args[0] == "help") return false;
                
                if (this._subcommands.ContainsKey(args[0]))
                {
                    return true;
                }
            }

            return false;
        }

        // walks the command tree until the last command is found and then returns it
        public Command GetEndCommand(string[] args, Command cmd = null)
        {
            if (this.HasSubcommand(args))
            {
                string[] rest = Utils.SafeShiftArgsLeft(args);
                Command c = this._subcommands[args[0]];
                return c.GetEndCommand(rest, c);
            }
            else
            {
                return cmd;
            }
        }
        
        public void Execute(string[] args)
        {
            // check if should execute sub command
            if (this.HasSubcommand(args))
            {
                string[] rest = Utils.SafeShiftArgsLeft(args);
                this._subcommands[args[0]].Execute(rest);
                return;
            }

            // validate args
            this.ValidateArgs(args);
            Run(args);
        }
    }
}