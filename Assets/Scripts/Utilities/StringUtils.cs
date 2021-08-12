namespace Utilities
{
    public static class StringUtils
    {
        /// <summary>
        ///     Generates a tab formatted string to make the help output look pretty
        /// </summary>
        /// <param name="string1"> First string to be included in the formatted string </param>
        /// <param name="string2"> Second string to be included in the formatted string </param>
        /// <param name="string3"> Third string to be included in the formatted string </param>
        /// <returns> Formatted string containing the 3 input strings </returns>
        public static string FormatOutputString(string string1, string string2, string string3)
        {
            // Pads a given string with spaces to the desired segment length
            string AutoFormat(string baseString, int segmentLength)
            {
                var remainder = segmentLength - baseString.Length;
                return $"{baseString}{new string(' ', remainder)}";
            }

            return $"{AutoFormat(string1, 20)}" +
                   $"{AutoFormat(string2, 30)}" +
                   $"{AutoFormat(string3, 50)}";
        }
    }
}