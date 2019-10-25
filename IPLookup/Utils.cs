using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IPLookup
{
    /// <summary>
    /// Utility class.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// The valid regex pattern for an IPv4 (1.1.1.1)
        /// </summary>
        public const string IPRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";

        /// <summary>
        /// The valid regex pattern for a masked IPv4 (1.1.1.1/24)
        /// </summary>
        public const string MaskedIPRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])/([1-9]|1[0-9]|2[0-9]|3[0-2])$";

        /// <summary>
        /// The valid regex pattern for a binary string that is 1 to 32 characters long.
        /// </summary>
        public const string BinStringRegex = @"^[01]{1,32}$";

        /// <summary>
        /// Converts an IP string to a binary string.
        /// </summary>
        /// <param name="input">The IP string formatted like 1.1.1.1/24</param>
        /// <returns>A string of 0s and 1s</returns>
        public static string IPToBinString(string input)
        {
            if (!Regex.Match(input, MaskedIPRegex).Success)
            {
                throw new FormatException("The masked IP format was wrong");
            }

            var split = input.Split('/');
            var output = string.Join("", (split[0].Split('.').Select(x => Convert.ToString(int.Parse(x), 2).PadLeft(8, '0'))).ToArray());

            if (split.Count() > 1) // If masked
            {
                return output.Substring(0, int.Parse(split[1]));
            }
            else
            {
                return output;
            }
        }

        /// <summary>
        /// Splits a string into chunks that have a maximum size.
        /// </summary>
        /// <remarks>This function is slow and should not be used during a lookup.</remarks>
        /// <param name="input">The string to split</param>
        /// <param name="size">The maximum chunk size</param>
        /// <returns>The array of string chunks</returns>
        public static string[] SplitInChunks(string input, int size)
        {
            if (input.Length < size) return new string[] { input };
            return Enumerable.Range(0, (int)Math.Ceiling((double)input.Length / (double)size))
                .Select(i => input.Substring(i * size, Math.Min(input.Length - i * size, size)))
                .ToArray();
        }
    }
}
