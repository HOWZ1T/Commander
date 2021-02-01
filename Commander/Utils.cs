using System;
using System.Diagnostics;

namespace Commander
{
    /// <summary>
    /// Utility class to provide utility methods.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Shifts the elements of the given string array one to the left.
        /// </summary>
        /// <param name="args">The string array to be shifted.</param>
        public static void ShiftArgs(ref string[] args)
        {
            for (int i = 0; i < args.Length-1; i++)
            {
                args[i] = args[i + 1];
            }
                
            Array.Resize(ref args, args.Length-1);
        }

        /// <summary>
        /// Writes out debug messages for the debug build only.
        /// </summary>
        /// <param name="msg">The string to be written out.</param>
        [ConditionalAttribute("DEBUG")]
        public static void Debug(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}