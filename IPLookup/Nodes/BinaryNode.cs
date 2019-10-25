using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPLookup.Nodes
{
    /// <summary>
    /// A node of a binary trie.
    /// </summary>
    public class BinaryNode : Node
    {
        /// <summary>
        /// The left child.
        /// </summary>
        public BinaryNode Left { get; set; }

        /// <summary>
        /// The right child.
        /// </summary>
        public BinaryNode Right { get; set; }

        /// <summary>
        /// The next hop information in case this node has one.
        /// </summary>
        public string NextHop { get; set; } = "";

        /// <inheritdoc/>
        public override void AddChild(string prefix, string path)
        {
            switch (path.Length)
            {
                case 0:
                    break;

                case 1:
                    if (path == "0")
                    {
                        Left = new BinaryNode() { NextHop = prefix };
                    }
                    else {
                        Right = new BinaryNode() { NextHop = prefix };
                    }
                    break;

                default:
                    if (path.StartsWith("0"))
                    {
                        if (Left == null) Left = new BinaryNode();
                        Left.AddChild(prefix, path.Substring(1));
                    }
                    else
                    {
                        if (Right == null) Right = new BinaryNode();
                        Right.AddChild(prefix, path.Substring(1));
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override string Lookup(string address, string backtrack = "")
        {
            // If this is a prefix, set it as backtrack
            if (NextHop != "") backtrack = NextHop;

            if (address == "" || (Left == null && Right == null)) return backtrack;

            if (address.StartsWith("0"))
            {
                if (Left != null) return Left.Lookup(address.Substring(1), backtrack);
                else return backtrack;
            }
            else
            {
                if (Right != null) return Right.Lookup(address.Substring(1), backtrack);
                else return backtrack;
            }
        }

        /// <inheritdoc/>
        public override string LookupNonRecursive(string address, string rootPrefix = "")
        {
            var backtrack = "";
            var partialAddress = address;
            var node = this;

            while (node != null)
            {
                if (node.NextHop != "") backtrack = node.NextHop;

                if (partialAddress == "" || (node.Left == null && node.Right == null)) return backtrack;

                if (partialAddress.StartsWith("0"))
                {
                    if (node.Left != null)
                    {
                        partialAddress = partialAddress.Substring(1);
                        node = node.Left;
                    }
                    else return backtrack;
                }
                else
                {
                    if (node.Right != null)
                    {
                        partialAddress = partialAddress.Substring(1);
                        node = node.Right;
                    }
                    else return backtrack;
                }
            }

            return backtrack;
        }
    }
}
