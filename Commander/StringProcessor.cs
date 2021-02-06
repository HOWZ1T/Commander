namespace Commander
{
    public class StringProcessor
    {
        /// <summary>
        /// Replaces special sequences within the string with the relevant data.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="program"></param>
        /// <param name="cmd"></param>
        /// <returns>The processed string.</returns>
        public static string Process(string str, Program program = null, CommandObj cmd = null)
        {
            var processedStr = str;
            /*
             * Special Sequences:
             * @p = program name
             * @n = command name
             * @c = call string
             * @u = usage
             */

            if (program != null)
            {
                var name = program.IsCaseSensitive ? program.Name : program.Name.ToLower();
                processedStr = processedStr.Replace("@p", name);
            }

            if (cmd != null)
            {
                var name = program.IsCaseSensitive ? cmd.Name : cmd.Name.ToLower();
                var callString = CommandObj.CallString(cmd);
                var usage = cmd.Usage(program);

                processedStr = processedStr.Replace("@n", name);
                processedStr = processedStr.Replace("@c", callString);
                processedStr = processedStr.Replace("@u", usage);
            }

            return processedStr;
        }
    }
}