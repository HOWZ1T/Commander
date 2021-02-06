using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Commander.Attributes;
using Commander.Errors;

namespace Commander
{
    /// <summary>
    /// Cog provides an class that encapsulates commands.
    /// </summary>
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
                this.Description = StringProcessor.Process(cmdGrp.Description, prog);
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
                    this.Description = StringProcessor.Process(description, prog);
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
                cmd.Description = StringProcessor.Process(cmd.Description, prog, cmd);
                for(int i = 0; i < cmd.Examples.Length; i++)
                {
                    cmd.Examples[i] = StringProcessor.Process(cmd.Examples[i], prog, cmd);
                }
                
                if (group != null && parent == null)
                {
                    group.AddChild(cmd);
                }
                else if (parent == null)
                {
                    Commands.Add(cmdName, cmd);
                }
            }
        }

        /// <summary>
        /// Finds and returns a command based on the given name.
        /// </summary>
        /// <param name="name">The name of the command to find.</param>
        /// <returns>The command if found otherwise null</returns>
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

        /// <summary>
        /// Processes and executes the given input based on the command name and argument list.
        /// </summary>
        /// <param name="prog">The program that this cog belongs to.</param>
        /// <param name="name">The name of the first specified command from the arguments.</param>
        /// <param name="args">The rest of the arguments to pass along to the command, includes subcommands.</param>
        /// <returns>A string containing the result of the execution.</returns>
        /// <exception cref="CogError"></exception>
        /// <seealso cref="CommandObj.Invoke"/>
        internal string Execute(Program prog, string name, string[] args)
        {
            // process  the command(s) down the command tree.
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