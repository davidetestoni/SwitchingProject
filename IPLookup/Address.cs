using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace IPLookup
{
    /// <summary>
    /// An IP address with mask.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// The IPv4 with the mask e.g. 1.1.1.1/24
        /// </summary>
        public string MaskedIPv4 { get { return $"{IPv4}/{Mask}"; } }

        /// <summary>
        /// The IPv4 without mask.
        /// </summary>
        public string IPv4 { get; set; }

        /// <summary>
        /// The subnet mask.
        /// </summary>
        public int Mask { get; set; }

        /// <summary>
        /// The IP as a binary string made of 0s and 1s.
        /// </summary>
        public string BinaryString { get { return Utils.IPToBinString(MaskedIPv4); } }

        /// <summary>
        /// Creates an address from a masked string.
        /// </summary>
        /// <param name="maskedIP">The masked IP string in this format: 1.1.1.1/24</param>
        public Address(string maskedIP)
        {
            if (!Regex.Match(maskedIP, Utils.MaskedIPRegex).Success)
            {
                throw new FormatException("The input masked IP is not in the correct format");
            }

            var split = maskedIP.Split('/');
            IPv4 = split[0];
            Mask = int.Parse(split[1]);
        }

        /// <summary>
        /// Creates an address from a string and a mask.
        /// </summary>
        /// <param name="ip">The address in this format: 1.1.1.1</param>
        /// <param name="mask">The mask (must be between 1 and 32)</param>
        public Address(string ip, int mask)
        {
            if (mask < 1 || mask > 32)
            {
                throw new ArgumentException("The mask must be between 1 and 32");
            }

            if (!Regex.Match(ip, Utils.IPRegex).Success)
            {
                throw new FormatException("The input IP is not in the correct format");
            }

            IPv4 = ip;
            Mask = mask;
        }
    }
}
