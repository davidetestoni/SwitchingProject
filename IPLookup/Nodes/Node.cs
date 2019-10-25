using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPLookup.Nodes
{
    /// <summary>
    /// A node of a generic trie.
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// Adds a node to the trie.
        /// </summary>
        /// <param name="prefix">The next hop information</param>
        /// <param name="path">The address as a string of 0s and 1s.</param>
        public virtual void AddChild(string prefix, string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the last node with a valid next hop that matches the given address.
        /// </summary>
        /// <param name="address">The address for which to find the next hop</param>
        /// <param name="backtrack">The last visited node with a valid next hop, leave blank if not a multibit trie</param>
        /// <returns>The next hop</returns>
        public virtual string Lookup(string address, string backtrack)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the last node with a valid next hop that matches the given address without recursion.
        /// </summary>
        /// <param name="address">The address for which to find the next hop</param>
        /// <param name="rootPrefix">Leave blank</param>
        /// <returns>The next hop</returns>
        public virtual string LookupNonRecursive(string address, string rootPrefix)
        {
            throw new NotImplementedException();
        }
    }
}
