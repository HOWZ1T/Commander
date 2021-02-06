using System;
using System.Text;

namespace Commander
{
    public class DefaultHelp : IHelp
    {
        /// <summary>
        /// Generates help info for ourself, the help command.
        /// </summary>
        /// <returns>A string containing the help information.</returns>
        /// <seealso cref="Help(Commander.Program,Commander.Cog)"/>
        /// <seealso cref="Help(Commander.Program,Commander.CommandObj)"/>
        public virtual string Help()
        {
            /*
             * [help]
             * Description:
             * Provides help information for the given cog or command.
             *
             * Usage:
             * help <command/cog name [string]>
             */

            return "[help]\nDescription:\nProvides help information for the given cog or command.\n\nUsage:\nhelp <command/cog name [string]>";
        }
        
        /// <summary>
        /// Generates help info for the given command.
        /// </summary>
        /// <param name="prog">The program from which the help command is being run.</param>
        /// <param name="cmd">The command for which to generate the help info.</param>
        /// <returns>A string containing the help information.</returns>
        /// <seealso cref="Help()"/>
        /// <seealso cref="Help(Commander.Program,Commander.Cog)"/>
        public string Help(Program program, CommandObj cmd)
        {
            /*
             * [Command Name]
             * Description:
             * <description>
             * 
             * Usage:
             * <usage>
             *
             * Examples:
             * <examples>
             */

            var cmdName = (program.IsCaseSensitive) ? cmd.Name : cmd.Name.ToLower();
            StringBuilder info = new StringBuilder();
            info.Append('[').Append(cmdName).Append("]\n").Append("Description:\n").Append(cmd.Description);
            info.Append("\n\nUsage:\n").Append(cmd.Usage(program));

            if (cmd.Examples.Length > 0)
            {
                info.Append("\n\nExamples:");
                foreach (var example in cmd.Examples)
                {
                    info.Append(example).Append('\n');
                }
                
                // remove trailing \n char
                info.Remove(info.Length - 1, info.Length);
            }

            return info.ToString();
        }
        
        /// <summary>
        /// Generates help info for the given cog.
        /// </summary>
        /// <param name="prog">The program from which the help command is being run.</param>
        /// <param name="cog">The cog for which to generate the help info.</param>
        /// <returns>A string containing the help information.</returns>
        /// <seealso cref="Help()"/>
        /// <seealso cref="Help(Commander.Program,Commander.CommandObj)"/>
        public string Help(Program program, Cog cog)
        {
            /*
             * [Cog Name]
             * Description:
             * <description>
             * 
             * Commands:
             * <commands>
             */

            var cogName = program.IsCaseSensitive ? cog.Name : cog.Name.ToLower();
            StringBuilder info = new StringBuilder();
            info.Append('[').Append(cogName).Append("]\nDescription:\n").Append(cog.Description);

            if (cog.Commands.Count > 0)
            {
                info.Append("\n\nCommands:\n");
                foreach (var cmd in cog.Commands.Values)
                {
                    info.Append(cmd.Name).Append('\n');
                }

                // remove trailing \n char
                info.Remove(info.Length - 1, info.Length);
            }
            
            return info.ToString();
        }
    }
}