using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Commander.Errors;

namespace Commander
{
    /// <summary>
    /// A internal representation of a command method.
    /// </summary>
    public class CommandObj
    {
        public readonly string Name;
        public readonly string Description;
        public readonly string[] Examples;
        public readonly bool Invokable;
        public readonly object _cog;
        
        private CommandObj _parent;
        private MethodInfo _methodInfo;

        protected Dictionary<string, CommandObj> Children;
        
        public CommandObj(object cog, MethodInfo methodInfo, bool invokable, string name, string description, string[] examples) : this(cog, null,  methodInfo, invokable, name, description, examples){}
        
        public CommandObj(object cog, CommandObj parent, MethodInfo methodInfo, bool invokable, string name, string description, string[] examples)
        {
            _cog = cog;
            _parent = null;

            Name = name;
            Description = description;
            Examples = examples;
            Invokable = invokable;
            _methodInfo = methodInfo;
            Children = new Dictionary<string, CommandObj>();

            parent?.AddChild(this);
        }

        public bool HasParent()
        {
            return _parent != null;
        }

        public CommandObj GetParent()
        {
            return _parent;
        }

        /// <summary>
        /// Generates the call string for the given command.
        /// </summary>
        /// <param name="cmd">The command for which to generate the call string.</param>
        /// <returns>The call string for the given command.</returns>
        protected static string CallString(CommandObj cmd)
        {
            var names = new Stack<string>();
            while (cmd != null)
            {
                names.Push(cmd.Name);
                cmd = cmd._parent;
            }

            var callstring = "";
            var name = "";
            while (names.TryPop(out name))
            {
                callstring += name + " ";
            }

            callstring = callstring.Substring(0, callstring.Length - 1);
            return callstring;
        }

        public void AddChild(CommandObj cmd)
        {
            if (Children.ContainsKey(cmd.Name)) return;
            Children.Add(cmd.Name, cmd);
            cmd._parent = this;
        }

        public CommandObj GetChild(string name)
        {
            if (Children.ContainsKey(name))
            {
                return Children[name];
            }

            return null;
        }

        public bool HasChild(string name)
        {
            return Children.ContainsKey(name);
        }

        public CommandObj[] AllChildren()
        {
            return Children.Values.ToArray();
        }

        /// <summary>
        /// Executes the command based on the given arguments.
        /// </summary>
        /// <param name="prog">The program this command belong to.</param>
        /// <param name="args">The arguments to be passed to the command.</param>
        /// <returns>A string containing the result of the execution of the command.</returns>
        /// <exception cref="CommandError"></exception>
        /// <seealso cref="Cog.Execute"/>
        public string Invoke(Program prog, string[] args)
        {
            if (!Invokable)
            {
                var callstring = CallString(this);
                throw new CommandError($"Attempted to invoke a non-invokable command: {Name}", callstring);
            }
            
            // check given args length vs expected number of args
            var minArgCount = 0;
            var maxArgCount = 0;
            foreach (var parameterInfo in _methodInfo.GetParameters())
            {
                if (parameterInfo.IsOptional)
                {
                    maxArgCount++;
                }
                else
                {
                    maxArgCount++;
                    minArgCount++;
                }
            }

            if (args.Length < minArgCount || args.Length > maxArgCount)
            {
                throw new CommandError("invalid number of arguments", CallString(this));
            }
            
            // start attempting to parse args based on function parameters
            var parameterInfos = new SortedList<int, ParameterInfo>();
            foreach (var parameterInfo in _methodInfo.GetParameters())
            {
                parameterInfos.Add(parameterInfo.Position, parameterInfo);
            }

            object[] parameters = new object[parameterInfos.Count];
            for (var i = 0; i < args.Length; i++)
            {
                // get convertor
                Type type = parameterInfos[i].ParameterType;
                var convertor = prog.GetConvertor(type);

                // added convertor null check to string type check to avoid override any user defined string convertors
                if (type == typeof(string) && convertor == null)
                {
                    parameters[i] = args[i];
                }
                else
                {
                    // attempt to convert argument
                    if (convertor == null)
                    {
                        throw new CommandError($"could not find convertor for type: {type}", CallString(this));
                    }
                    
                    object obj;
                    var result = convertor.TryConvert(type, args[i], out obj);
                    if (!result)
                    {
                        throw new CommandError($"could not convert: {args[i]} to type: {type}", CallString(this));
                    }

                    parameters[i] = obj;
                }
            }
            
            if (parameterInfos.Count > args.Length)
            {
                // check for unassigned parameters and handle optional parameters
                for (var i = args.Length; i < parameterInfos.Count; i++)
                {
                    if (!parameterInfos[i].IsOptional || !parameterInfos[i].HasDefaultValue)
                    {
                        throw new CommandError($"Missing required parameter: {parameterInfos[i].Name}",
                            CallString(this));
                    }

                    parameters[i] = parameterInfos[i].DefaultValue;
                }
            }
            
            return _methodInfo.Invoke(_cog, parameters) as string;
        }
    }
}