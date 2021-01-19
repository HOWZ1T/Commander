using System;
using Commander.Attributes;

namespace Commander
{
    [Name("help")]
    [Signature("@p help <command: optional>")]
    [Description("Provides helpful information for the program or for the specified command.")]
    [Example("@p help")]
    [Example("@p help someCommand")]
    public class HelpCommand : Command
    {
        private readonly Program _program;

        public HelpCommand(Program program) : base(new ArgPolicy(Commander.Policy.Any))
        {
            this._program = program;
        }

        public override void Run(string[] args)
        {
            this.ValidateArgs(args);
            
            if (args.Length == 0)
            {
                Console.WriteLine(this.Help(this._program));
                return;
            }
            
            // go down command tree
            /*
             * e.g.:
             * - hash
             *      \
             *       -> hamming
             *                \
             *                 -> similarity
             */
            
            if (args[0] == "help")
            {
                Console.WriteLine(Help(this));
                return;
            }

            Command cmd = this._program.GetCommand(args[0]);
            if (cmd != null)
            {
                string[] rest = Utils.ShiftArgsLeft(args);
                Command c = cmd.GetEndCommand(rest, cmd);
                Console.WriteLine(Help(c));
                return;
            }
            
            throw new CommandError("Could not find the command: " + args[0], this);
        }

        public virtual new string Help(Program program)
        {
            // TODO cleaner commands listing
            var cmds = this._program.ListCommands();
            var hlp = String.Format(@"
       Name: {0}
Description: 
{1}
  
Commands: 
{2}
", this._program.ProgramName, this._program.Description, this._program.ListCommands());
            hlp = hlp.Replace("@p", program.ProgramName);
            return hlp;
        }

        public virtual string Help(Command command)
        {
            return command.Help(this._program);
        }
    }
}