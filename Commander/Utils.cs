using System;
using System.Diagnostics;

namespace Commander
{
    public class Utils
    {
        public static void ShiftArgs(ref string[] args)
        {
            // shift array left
            // 0, 1, 2, 3
            // 1, 2, 3
            for (int i = 0; i < args.Length-1; i++)
            {
                args[i] = args[i + 1];
            }
                
            Array.Resize(ref args, args.Length-1);
        }

        [ConditionalAttribute("DEBUG")]
        public static void Debug(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}