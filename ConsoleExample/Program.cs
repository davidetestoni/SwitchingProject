using IPLookup;
using IPLookup.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the tries
            var binaryRoot = new BinaryNode() { NextHop = "Root" };
            var compressedRoot = new CompressedNode() { NextHop = "Root" };
            var multibitRoot = new MultibitNode() { Stride = 3 };

            // Generate IPs
            var gen = new AddressGenerator().GenerateAddress();
            var db = gen.Take(10).ToArray();

            // Add children to tries
            foreach (var ip in db)
            {
                binaryRoot.AddChild(ip.MaskedIPv4, ip.BinaryString);
                compressedRoot.AddChild(ip.MaskedIPv4, ip.BinaryString);
                multibitRoot.AddChild(ip.MaskedIPv4, ip.BinaryString);
            }
            compressedRoot.Compress();

            // Perform a lookup
            var tosearch = new Address("132.15.162.33/17");
            Console.WriteLine($"Binary: {binaryRoot.Lookup(tosearch.BinaryString)}");
            Console.WriteLine($"Compressed: {compressedRoot.Lookup(tosearch.BinaryString)}");
            Console.WriteLine($"Multibit: {multibitRoot.Lookup(tosearch.BinaryString, "Root")}");
            
            Console.ReadLine();
        }
    }
}
