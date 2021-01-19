using System;

namespace Commander
{
    public class Utils
    {
        public static string[] ShiftArgsLeft(string[] arg)
        {
            // shift array left
            // 0, 1, 2, 3
            // 1, 2, 3
            for (int i = 0; i < arg.Length-1; i++)
            {
                arg[i] = arg[i + 1];
            }
                
            Array.Resize(ref arg, arg.Length-1);
            return arg;
        }
        
        public static string[] SafeShiftArgsLeft(string[] arg)
        {
            string[] argCopy = new string[arg.Length];
            Array.Copy(arg, argCopy, arg.Length);

            return ShiftArgsLeft(argCopy);
        }
    }
}