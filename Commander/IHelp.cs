using System;

namespace Commander
{
    public interface IHelp
    {
        /// <summary>
        ///     Generates help info for ourself, the help command.
        /// </summary>
        /// <returns>A string containing the help information.</returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <seealso cref="Help(Commander.Program,Commander.Cog)" />
        /// <seealso cref="Help(Commander.Program,Commander.CommandObj)" />
        public virtual string Help()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Generates help info for the given command.
        /// </summary>
        /// <param name="prog">The program from which the help command is being run.</param>
        /// <param name="cmd">The command for which to generate the help info.</param>
        /// <returns>A string containing the help information.</returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <seealso cref="Help()" />
        /// <seealso cref="Help(Commander.Program,Commander.Cog)" />
        public virtual string Help(Program prog, CommandObj cmd)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Generates help info for the given cog.
        /// </summary>
        /// <param name="prog">The program from which the help command is being run.</param>
        /// <param name="cog">The cog for which to generate the help info.</param>
        /// <returns>A string containing the help information.</returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <seealso cref="Help()" />
        /// <seealso cref="Help(Commander.Program,Commander.CommandObj)" />
        public virtual string Help(Program prog, Cog cog)
        {
            throw new NotImplementedException();
        }
    }
}