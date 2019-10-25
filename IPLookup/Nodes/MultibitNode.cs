using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPLookup.Nodes
{
    /// <summary>
    /// A node of a multibit trie.
    /// </summary>
    public class MultibitNode : Node
    {
        /// <summary>
        /// The amount of bits to use for the node's dictionary.
        /// </summary>
        public int Stride { get; set; } = 3;

        /// <summary>
        /// The dictionary that assigns to each Stride-long combination a next hop and / or a child node.
        /// </summary>
        public Dictionary<string, (string, MultibitNode)> Children { get; set; } = new Dictionary<string, (string, MultibitNode)>();

        /// <inheritdoc/>
        public override void AddChild(string prefix, string path)
        {
            // If the path is empty, interrupt the recursion
            if (path == "") return;

            // Split the path into chunks of at most Stride characters
            var chunks = Utils.SplitInChunks(path, Stride);


            var first = chunks.First();

            // If there is only 1 chunk
            if (chunks.Count() == 1)
            {
                // If it's the same length as the stride
                if (first.Length == Stride)
                {
                    Children[first] = (prefix, null);
                }

                // Otherwise expand it with all possible combinations
                else
                {
                    foreach (var comb in GetCombinations(Stride - first.Length))
                    {
                        Children[first + comb] = (prefix, null);
                    }
                }
            }

            // If there are more chunks
            else
            {
                // If the child does not exist, create it
                if (!Children.ContainsKey(first))
                {
                    Children[first] = ("", new MultibitNode() { Stride = this.Stride });
                }
                
                // If it exists but it doesn't have a pointer (leaf node)
                else if (Children[first].Item2 == null)
                {
                    Children[first] = (Children[first].Item1, new MultibitNode() { Stride = this.Stride });
                }

                // Call the method again on the child recursively
                Children[first].Item2.AddChild(prefix, path.Substring(Stride));
            }
        }

        /// <inheritdoc/>
        public override string Lookup(string address, string backtrack)
        {
            // If the address is shorter than the stride, return the backtrack
            if (address.Length < Stride) return backtrack;

            // Cut the first Stride bits of the address
            var first = address.Substring(0, Stride);

            // If the child does not exist, return the backtrack
            if (address.Length < Stride || !Children.ContainsKey(first)) return backtrack;

            // WE ARE HERE IF THE ADDRESS IS AT LEAST AS LONG AS THE STRIDE

            var child = Children[first];

            // If the address is as long as the stride or there is no pointer to another multibit node, return the prefix
            if (address.Length == Stride || child.Item2 == null)
            {
                return child.Item1;
            }
            // If it's longer than the stride and we have a pointer
            else
            {
                return child.Item2.Lookup(address.Substring(Stride), child.Item1 == "" ? backtrack : child.Item1);
            }
        }

        /// <inheritdoc/>
        public override string LookupNonRecursive(string address, string rootPrefix)
        {
            var backtrack = rootPrefix;
            var partialAddress = address;
            var node = this;

            while (node != null)
            {
                if (partialAddress.Length < Stride) return backtrack;

                var first = partialAddress.Substring(0, Stride);

                if (partialAddress.Length < Stride || !node.Children.ContainsKey(first)) return backtrack;

                var child = node.Children[first];
                node = child.Item2; // Added by Emanuele

                if (partialAddress.Length == Stride || child.Item2 == null)
                {
                    return child.Item1;
                }
                else
                {
                    partialAddress = partialAddress.Substring(Stride);
                    if (child.Item1 != "") backtrack = child.Item1;
                    node = child.Item2;
                }
            }

            return backtrack;
        }

        private IEnumerable<string> GetCombinations(int length)
        {
            var charSet = "01";
            var list = charSet.Select(x => x.ToString());
            for (int i = 1; i < length; i++)
                list = list.SelectMany(x => charSet, (x, y) => x + y);
            return list;
        }
    }
}
