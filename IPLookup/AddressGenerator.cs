using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPLookup
{
    /// <summary>
    /// Generates random addresses.
    /// </summary>
    public class AddressGenerator
    {
        private Random Rand { get; set; } = new Random();

        /// <summary>
        /// Generates strings of 0s and 1s that can be 1 to 32 characters long.
        /// </summary>
        /// <returns>An IEnumerable with random binary strings.</returns>
        public IEnumerable<string> GenerateBin()
        {
            while (true)
            {
                var length = Rand.Next(1, 32);
                var address = "";
                for (var i = 0; i < length; i++)
                {
                    address += (Rand.Next() % 2).ToString();
                }
                yield return address;
            }
        }

        /// <summary>
        /// Generates a random IPv4 address with a 1 to 32 bit mask.
        /// </summary>
        /// <returns>An IEnumerable with random Addresses.</returns>
        public IEnumerable<Address> GenerateAddress(
            int minOctet1 = 1, int maxOctet1 = 255,
            int minOctet2 = 0, int maxOctet2 = 255,
            int minOctet3 = 0, int maxOctet3 = 255,
            int minOctet4 = 0, int maxOctet4 = 255,
            int minMask = 1, int maxMask = 32)
        {
            while (true)
            {
                yield return new Address(
                    Rand.Next(minOctet1, maxOctet1 + 1) + "." +
                    Rand.Next(minOctet2, maxOctet2 + 1) + "." +
                    Rand.Next(minOctet3, maxOctet3 + 1) + "." +
                    Rand.Next(minOctet4, maxOctet4 + 1) + "/" +
                    Rand.Next(minMask, maxMask));
            }
        }
    }
}
