using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Commander.Attributes;
using Commander.Errors;

namespace Commander
{
    public class Cog
    {
        public readonly string Name;
        public readonly string Description;
        public readonly bool IsGroup;
        public bool IsCaseSensitive = false;
        public readonly Dictionary<string, CommandObj> Commands;

        public Cog(Program prog) : this(prog, "", "") { }
        
        public Cog(Program prog, string name = "", string description = "")
        {
            IsCaseSensitive = prog.IsCaseSensitive;
            IsGroup = false;
            Commands = new Dictionary<string, CommandObj>();
            
            // process attributes
            // set name, description and IsGroup based on presence of attributes and variables
            var attr = this.GetType().GetCustomAttribute(typeof(CommandGroup));
            if (attr != null)
            {
                var cmdGrp = attr as CommandGroup;
                this.Name = (IsCaseSensitive) ? cmdGrp.Name.ToLower() : cmdGrp.Name;
                this.Description = cmdGrp.Description;
                this.IsGroup = true;
                Commands.Add(Name, new CommandObj(this, null, false, Name, Description, new string[] { }));
            }
            else
            {
                if (name.Length == 0)
                {
                    this.Name = this.GetType().Name;
                }
                else
                {
                    this.Name = name;
                }

                if (description.Length == 0)
                {
                    this.Description = "Not given.";
                }
                else
                {
                    this.Description = description;
                }
            }
            
            Utils.Debug($"Cog: {Name}, {Description}\nIsGroup: {IsGroup.ToString()}");
            
            CommandObj group = null;
            if (IsGroup)
            {
                group = Commands[Name];
            }
            
            // process functions
            var methodInfo = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methodInfo)
            {

                var rawAttrib = method.GetCustomAttribute(typeof(Command));
                if (rawAttrib == null) continue;

                var attrib = rawAttrib as Command;
                
                // process command
                
                // check that command function return value is string
                if (method.ReturnType != typeof(string))
                {
                    throw new AttributeError($"cog: {Name}.{method.Name}() does not return a string");
                }
                
                var cmdName = (attrib.Name == null) ? method.Name : attrib.Name;

                // parse examples
                var examples = new List<string>();
                var attribs = method.GetCustomAttributes(typeof(Example));
                foreach (var a in attribs)
                {
                    examples.Add((a as Example).Val);
                }

                CommandObj parent = null;
                
                // check if command is a subcommand
                if (attrib.Parent != null)
                {
                    var attrParent = (IsCaseSensitive) ? attrib.Parent : attrib.Parent.ToLower();
                    if (Commands.ContainsKey(attrParent))
                    {
                        parent = Commands[attrParent];
                    }
                    else
                    {
                        CommandObj FindParent(CommandObj command, string strParent)
                        {
                            var p = (IsCaseSensitive) ? strParent : strParent.ToLower();
                            var cName = (IsCaseSensitive) ? command.Name : command.Name.ToLower();
                            if (cName == p) return command;

                            return command.AllChildren().Select(commandObj => FindParent(commandObj, strParent)).FirstOrDefault();
                        }

                        parent = Commands.Values.Select(c => FindParent(c, attrParent)).FirstOrDefault();
                        if (parent == null)
                        {
                            throw new AttributeError($"could not find parent: {attrParent} in the scope of this cog");
                        }
                    }
                }

                CommandObj cmd = new CommandObj(this, parent, method, true, cmdName, attrib.Description, examples.ToArray());
                if (group != null && parent == null)
                {
                    group.AddChild(cmd);
                }
                else if (parent == null)
                {
                    Commands.Add(cmdName, cmd);
                }
            }


            void PrintCmds(CommandObj cmd)
            {
                Utils.Debug(cmd.Name);
                foreach (var commandObj in cmd.AllChildren())
                {
                    PrintCmds(commandObj);
                }
            }
            
            Utils.Debug($"Commands: ");
            foreach (var c in Commands.Values)
            {
                PrintCmds(c);
            }
        }

        public CommandObj GetCommand(string name)
        {
            CommandObj FindCommand(CommandObj cmd, string cmdName)
            {
                var cn = (IsCaseSensitive) ? cmdName : cmdName.ToLower();
                var c = (IsCaseSensitive) ? cmd.Name : cmd.Name.ToLower();

                Utils.Debug($"comparing: {c} vs {cn}");
                if (c == cn) return cmd;

                foreach (var child in cmd.AllChildren())
                {
                    CommandObj co = FindCommand(child, cmdName);
                    if (co != null) return co;
                }

                return null;
            }

            foreach (var cv in Commands.Values)
            {
                CommandObj c = FindCommand(cv, name);
                if (c != null) return c;
            }

            return null;
        }

        internal string Execute(Program prog, string name, string[] args)
        {
            string InternalExecute(string iName, string[] cArgs)
            {
                var next = "";
                if (cArgs.Length > 0)
                {
                    next = cArgs[0];
                }
            
                // check if we can find the current command
                CommandObj cmd = GetCommand(iName);
                if (cmd == null)
                {
                    throw new CogError($"could not find command: {iName}");
                }
                
                // check if next is subcommand
                CommandObj nextCmd = GetCommand(next);
                if (nextCmd == null)
                {
                    return cmd.Invoke(prog, cArgs);
                }
                
                // run down tree
                Utils.ShiftArgs(ref cArgs);
                return InternalExecute(nextCmd.Name, cArgs);
            }
            
            return InternalExecute(name, args);
        }
    }
}