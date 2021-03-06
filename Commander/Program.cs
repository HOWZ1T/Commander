using System;
using System.Collections.Generic;
using System.Linq;
using Commander.Convertors;
using Commander.Errors;

namespace Commander
{
    /// <summary>
    ///     Program provides a class that encapsulates cogs and provides the ability to process a given command string.
    /// </summary>
    public class Program
    {
        public readonly bool IsCaseSensitive;
        public readonly string Name;
        public readonly bool UseProgramNamePrefix;

        private readonly Dictionary<string, Cog> _cogs;
        private readonly Dictionary<Type, Convertor> _convertors;
        public IHelp Help;

        public ISplitter Splitter;

        public Program(string name, bool isCaseSensitive = false, bool useProgramNamePrefix = true)
        {
            Utils.Debug("initializing program...");
            _cogs = new Dictionary<string, Cog>();
            // Adding default convertors
            _convertors = new Dictionary<Type, Convertor>();
            _convertors.Add(typeof(bool), new BooleanConvertor());
            _convertors.Add(typeof(byte), new Convertor(typeof(byte)));
            _convertors.Add(typeof(sbyte), new Convertor(typeof(sbyte)));
            _convertors.Add(typeof(char), new Convertor(typeof(char)));
            _convertors.Add(typeof(decimal), new Convertor(typeof(decimal)));
            _convertors.Add(typeof(double), new Convertor(typeof(double)));
            _convertors.Add(typeof(float), new Convertor(typeof(float)));
            _convertors.Add(typeof(int), new Convertor(typeof(int)));
            _convertors.Add(typeof(uint), new Convertor(typeof(uint)));
            _convertors.Add(typeof(long), new Convertor(typeof(long)));
            _convertors.Add(typeof(ulong), new Convertor(typeof(ulong)));
            _convertors.Add(typeof(short), new Convertor(typeof(short)));
            _convertors.Add(typeof(ushort), new Convertor(typeof(ushort)));

            Name = name;
            IsCaseSensitive = isCaseSensitive;
            UseProgramNamePrefix = useProgramNamePrefix;
            Splitter = new DefaultSplitter();
            Help = new DefaultHelp();
            Utils.Debug(
                $"Program: {Name}\nIsCaseSensitive: {IsCaseSensitive.ToString()}" +
                $"\nUseProgramNamePrefix: {UseProgramNamePrefix.ToString()}"
            );
        }

        /// <summary>
        ///     Adds the given cog to the program.
        /// </summary>
        /// <param name="cog">The cog to register in the program.</param>
        public void Register(Cog cog)
        {
            var name = IsCaseSensitive ? cog.Name : cog.Name.ToLower();
            if (_cogs.ContainsKey(name)) return;
            _cogs.Add(name, cog);
            Utils.Debug($"added cog: {name}");
        }

        /// <summary>
        ///     Removes the given cog from the program.
        /// </summary>
        /// <param name="cog">The cog to remove from the program.</param>
        public void Deregister(Cog cog)
        {
            var name = IsCaseSensitive ? cog.Name : cog.Name.ToLower();
            if (!_cogs.ContainsKey(name)) return;
            _cogs.Remove(name);
            Utils.Debug($"removed cog: {name}");
        }

        /// <summary>
        ///     Finds and returns a cog based on the given name.
        /// </summary>
        /// <param name="name">The name of the cog to find.</param>
        /// <returns>The Cog if found, otherwise null</returns>
        public Cog GetCog(string name)
        {
            var n = IsCaseSensitive ? name : name.ToLower();
            if (_cogs.ContainsKey(n)) return _cogs[n];

            return null;
        }

        /// <summary>
        ///     Determines whether or not a cog with the given name exists within the program.
        /// </summary>
        /// <param name="name">The name of the cog.</param>
        /// <returns>A bool representing if the cog exists within the program.</returns>
        public bool HasCog(string name)
        {
            var n = IsCaseSensitive ? name : name.ToLower();
            return _cogs.ContainsKey(n);
        }

        public Cog[] AllCogs()
        {
            return _cogs.Values.ToArray();
        }

        public void AddConvertor(Type typ, Convertor convertor)
        {
            _convertors.Add(typ, convertor);
        }

        public Convertor GetConvertor(Type typ)
        {
            if (_convertors.ContainsKey(typ)) return _convertors[typ];
            return null;
        }

        /// <summary>
        ///     Processes the arguments and dispatches the input to the appropriate cog/command.
        /// </summary>
        /// <param name="args">The input arguments to the program.</param>
        /// <returns>The result of the execution from the program based on the given input arguments.</returns>
        /// <exception cref="ProgramError"></exception>
        private string DispatchCommand(string[] args)
        {
            if (args.Length == 0) throw new ProgramError("No command given.");

            // check for command group name
            var name = args[0];
            Utils.ShiftArgs(ref args);

            if (!IsCaseSensitive) name = name.ToLower();

            if (UseProgramNamePrefix)
            {
                var pn = IsCaseSensitive ? Name : Name.ToLower();
                if (name != pn) throw new ProgramError("Must prefix command with program name!");

                if (args.Length < 1) throw new ProgramError("No command given!");

                name = args[0];
                Utils.ShiftArgs(ref args);
                if (!IsCaseSensitive) name = name.ToLower();
            }

            // utility function for running the command tree
            CommandObj FindCommand(string cmdName)
            {
                foreach (var pair in _cogs)
                {
                    var obj = pair.Value.GetCommand(cmdName);
                    if (obj != null) return obj;
                }

                return null;
            }

            // handle help command
            if (name == "help")
            {
                // help called with no args
                if (args.Length <= 0)
                {
                    return Help.Help();
                }
                
                var cogName = args[0];
                var cg = GetCog(cogName);
                CommandObj cmd = null;
                if (cg != null)
                {
                    Utils.ShiftArgs(ref args);
                    if (args.Length <= 0) return Help.Help(this, cg);

                    cmd = cg.GetCommand(args.First());
                }

                // if cmd is still null, perform cog independent search
                if (cmd == null) cmd = FindCommand(args.First());

                if (cmd == null)
                {
                    // help was called on itself
                    if (args.First() == "help" || args.Last() == "help") return Help.Help();

                    throw new ProgramError($@"could not find any command called: {args.First()}");
                }

                return Help.Help(this, cmd);
            }

            var result = "";
            var cog = GetCog(name);
            if (cog != null)
            {
                result = cog.Execute(this, name, args);
            }
            else
            {
                Utils.Debug($"cog not found for name: {name}, attempting to find function...");

                // attempt to find function
                var cmd = FindCommand(name);
                if (cmd == null) throw new ProgramError($"could not find any command called: {name}");

                result = cmd.Invoke(this, args);
            }

            return result;
        }

        /// <summary>
        ///     Processes and executes the program based on the input arguments.
        /// </summary>
        /// <param name="args">The input arguments to the program.</param>
        /// <returns>The result of the execution from the program based on the given input arguments.</returns>
        /// <seealso cref="Run(string)" />
        public string Run(string[] args)
        {
            var result = "";

            try
            {
                result = DispatchCommand(args);
            }
            catch (Exception e)
            {
                if (e is ProgramError)
                {
                    result += "Program Error: " + e.Message + "\nUse `";
                    if (UseProgramNamePrefix) result += Name + " ";
                    result += "help` for more info.";
                }
                else if (e is CommandError)
                {
                    result += "Command Error: " + e.Message + "\nUse `";
                    if (UseProgramNamePrefix) result += Name + " ";
                    result += "help " + (e as CommandError).CallString + "` for more info.";
                }
                else if (e is AttributeError)
                {
                    result += "Attribute Error: " + e.Message;
                }
                else
                {
                    result += "Unhandled Error: " + e.Message;
                }
            }

            return result;
        }

        /// <summary>
        ///     Processes and executes the program based on the input argument.
        /// </summary>
        /// <param name="str">The input string to the program.</param>
        /// <returns>The result of the execution from the program based on the given input string.</returns>
        /// <seealso cref="Run(string[])" />
        public string Run(string str)
        {
            return Run(Splitter.Split(str));
        }
    }
}