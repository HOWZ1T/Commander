namespace Commander
{
    public interface ISplitter
    {
        /// <summary>
        /// Splits the given string into parts.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <returns>A string array consisting of the parts generated from the input string based on the splitting algorithm.</returns>
        public string[] Split(string str);
    }
}