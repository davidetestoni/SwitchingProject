using SwitchingProject.Nodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchingProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> db = new Dictionary<string, string>()
            {
                { "P2", "1*" },
                { "P3", "00*" },
                { "P4", "101*" },
                { "P5", "111*" },
                { "P6", "1000*" },
                { "P7", "11101*" },
                { "P8", "111001*" },
                { "P9", "1000011*" }
            };

            // Assicurati che il db sia ordinato per lunghezza
            db = db.OrderBy(e => e.Value.Length).ToDictionary(e => e.Key, e => e.Value);

            // Per evitare di stampare i trie grafici, settare su false
            var printGraph = false;

            // Inizializzo il binary trie
            BinaryNode binaryRoot = new BinaryNode() { NextHop = "P1" };
            Console.WriteLine("Initialized BINARY TRIE");
            foreach (var entry in db)
            {
                binaryRoot.AddChild(entry.Key, entry.Value.Replace("*", ""));
            }
            if (printGraph)
            {
                Console.WriteLine();
                BTreePrinter.Print(binaryRoot);
            } 

            // Inizializzo il compressed trie
            CompressedNode compressedRoot = new CompressedNode() { NextHop = "P1" };
            Console.WriteLine("Initialized COMPRESSED TRIE");
            foreach (var entry in db)
            {
                compressedRoot.AddChild(entry.Key, entry.Value.Replace("*", ""));
            }
            compressedRoot.Compress();
            if (printGraph)
            {
                Console.WriteLine();
                CTreePrinter.Print(compressedRoot);
            }

            // Inizializzo il multibit trie
            MultibitNode multibitRoot = new MultibitNode() { Stride = 3 };
            Console.WriteLine("Initialized MULTIBIT TRIE");
            foreach (var entry in db)
            {
                multibitRoot.AddChild(entry.Key, entry.Value.Replace("*", ""));
            }

            /*
             * LOOKUP IP PREDEFINITI
             * 
             * */

            var defaultIPs = new string[] {
                "0", // P1
                "01", // P1
                "100", // P2
                "111", // P5
                "10101", // P4
                "11100", // P5
                "100001111" // P9
            };

            Console.WriteLine();
            foreach (var ip in defaultIPs)
            {
                Console.WriteLine($"Address: {ip}");
                Console.WriteLine($"Binary: {binaryRoot.Lookup(ip)}");
                Console.WriteLine($"Compressed: {compressedRoot.Lookup(ip)}");
                Console.WriteLine($"Multibit: {multibitRoot.Lookup(ip, "P1")}");
                Console.WriteLine();
            }

            /*
             * LOOKUP IP CASUALI
             * 
             * */
            Console.WriteLine();
            Console.WriteLine("CASUAL IPs:");

            // Generiamo N address casuali
            var n = 10000;
            var ips = new AddressGenerator().Generate().Take(n);
            Console.WriteLine($"Generated {n} IPs");
            
            // Il risultato del lookup (per non istanziare una nuova variabile ogni volta)
            var lookupResult = "";

            // Il cronometro per le tempistiche dei metodi
            Stopwatch watch = new Stopwatch();

            // BINARY TRIE
            watch.Reset();
            watch.Start();
            foreach (var ip in ips)
            {
                lookupResult = binaryRoot.Lookup(ip);
            }
            watch.Stop();
            Console.WriteLine($"Binary: {watch.ElapsedMilliseconds} ms");

            // COMPRESSED TRIE
            watch.Reset();
            watch.Start();
            foreach (var ip in ips)
            {
                lookupResult = compressedRoot.Lookup(ip);
            }
            watch.Stop();
            Console.WriteLine($"Compressed: {watch.ElapsedMilliseconds} ms");

            // MULTIBIT
            watch.Reset();
            watch.Start();
            foreach (var ip in ips)
            {
                lookupResult = multibitRoot.Lookup(ip, "P1");
            }
            watch.Stop();
            Console.WriteLine($"Multibit: {watch.ElapsedMilliseconds} ms");

            Console.ReadLine();
        }
    }
}
