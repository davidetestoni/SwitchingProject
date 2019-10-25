using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPLookup.Nodes
{
    /// <summary>
    /// A node of a compressed trie.
    /// </summary>
    public class CompressedNode : Node
    {
        /// <summary>
        /// The left child.
        /// </summary>
        public CompressedNode Left { get; set; }

        /// <summary>
        /// The right child.
        /// </summary>
        public CompressedNode Right { get; set; }

        /// <summary>
        /// The next hop information in case this node has one.
        /// </summary>
        public string NextHop { get; set; } = "";

        /// <summary>
        /// How many bits have been compressed.
        /// </summary>
        public int Skip { get { return Segment.Length; } }

        /// <summary>
        /// The segment that has been compressed.
        /// </summary>
        public string Segment { get; set; } = "";
        
        /// <summary>
        /// Compresses the trie after all children have been added.
        /// </summary>
        /// <param name="segment">Leave blank</param>
        /// <returns>The compressed root.</returns>
        public CompressedNode Compress(string segment = "")
        {
            // If I don't have children, return myself
            if (Right == null && Left == null)
            {
                Segment = segment;
                return this;
            }

            // If I have 2 children, I cannot compress but I call the compression on my children, then I return myself.
            if (Right != null && Left != null)
            {
                Right = Right.Compress("");
                Left = Left.Compress("");
                Segment = segment;
                return this;
            }

            // IF WE GET HERE, IT MEANS WE ONLY HAVE ONE CHILD

            // If I'm a prefix, stop compressing the current chain and call the compression on the child (resetting the segment).
            if (NextHop != "")
            {
                Segment = segment;

                // Left child
                if (Left != null)
                {
                    // Call the compression on a new tree of which the left child is the root
                    Left = Left.Compress("");
                }

                // Right child
                else
                {
                    // Call the compression on a new tree of which the right child is the root
                    Right = Right.Compress("");
                }

                return this;
            }
            
            // If I'm not a prefix
            else
            {
                // Left child
                if (Left != null)
                {
                    // Return the compression result
                    return Left.Compress(segment + "0");
                }

                // Right child
                else
                {
                    // Return the compression result
                    return Right.Compress(segment + "1");
                }
            }
        }

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
                        Left = new CompressedNode() { NextHop = prefix };
                    }
                    else
                    {
                        Right = new CompressedNode() { NextHop = prefix };
                    }
                    break;

                default:
                    if (path.StartsWith("0"))
                    {
                        if (Left == null) Left = new CompressedNode();
                        Left.AddChild(prefix, path.Substring(1));
                    }
                    else
                    {
                        if (Right == null) Right = new CompressedNode();
                        Right.AddChild(prefix, path.Substring(1));
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override string Lookup(string address, string backtrack = "")
        {
            // If it's a prefix, set it as backtrack
            if (NextHop != "") backtrack = NextHop;

            if (address == "" || (Left == null && Right == null)) return backtrack;

            // If we have a 0
            if (address.StartsWith("0"))
            {
                // If the left child exists, the address is longer than the segment, and the segment matches or is empty
                if (Left != null && address.Length >= Left.Skip + 1 && (Left.Segment == "" || address.StartsWith("0" + Left.Segment)))
                {
                    return Left.Lookup(address.Substring(Left.Skip + 1), backtrack);
                }
                else return backtrack;
            }

            // If we have a 1
            else
            {
                // If the right child exists, the address is longer than the segment, and the segment matches or is empty
                if (Right != null && address.Length >= Right.Skip + 1 && (Right.Segment == "" || address.StartsWith("1" + Right.Segment)))
                {
                    return Right.Lookup(address.Substring(Right.Skip + 1), backtrack);
                }
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
                    if (node.Left != null && partialAddress.Length >= node.Left.Skip + 1 && (node.Left.Segment == "" || partialAddress.StartsWith("0" + node.Left.Segment)))
                    {
                        partialAddress = partialAddress.Substring(node.Left.Skip + 1);
                        node = node.Left;
                    }
                    else return backtrack;
                }
                else
                {
                    if (node.Right != null && partialAddress.Length >= node.Right.Skip + 1 && (node.Right.Segment == "" || partialAddress.StartsWith("0" + node.Right.Segment)))
                    {
                        partialAddress = partialAddress.Substring(node.Right.Skip + 1);
                        node = node.Right;
                    }
                    else return backtrack;
                }
            }

            return backtrack;
        }
    }
}
