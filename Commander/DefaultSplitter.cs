using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commander.Errors;

namespace Commander
{
    /// <summary>
    /// Provides implementation of string splitting based on splitting by whitespace.
    /// </summary>
    public class DefaultSplitter : ISplitter
    {
        /// <summary>
        /// Splits the input string into parts using whitespace as the delimeter.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <returns>A string array consisting of the parts obtained by splitting the input string by whitespace.</returns>
        public string[] Split(string str)
        {
            List<string> parts = new List<string>();
            StringBuilder part = new StringBuilder();
            bool inQuote = false;
            char quoteChar = ' ';
            foreach (var c in str)
            {
                switch (c)
                {
                    case ' ':
                        if (part.Length > 0 && !inQuote)
                        {
                            parts.Add(part.ToString());
                            part.Clear();
                        }
                        else if (inQuote)
                        {
                            part.Append(c);
                        }

                        break;

                    case '"':
                    case '\'':
                        inQuote = !inQuote;
                        
                        if (inQuote)
                        {
                            quoteChar = c;
                        }
                        
                        if (!inQuote)
                        {
                            if (c != quoteChar)
                            {
                                inQuote = true;
                                part.Append(c);
                            }
                            else
                            {
                                parts.Add(part.ToString());
                                part.Clear();
                                quoteChar = ' ';
                            }
                        }
                        break;
                    
                    default:
                        part.Append(c);
                        break;
                }
            }

            if (inQuote)
            {
                throw new ProgramError("malformed string quote");
            }

            if (part.Length > 0)
            {
                parts.Add(part.ToString());
            }

            return parts.ToArray();
        }
    }
}