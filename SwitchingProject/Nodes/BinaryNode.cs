using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchingProject.Nodes
{
    public class BinaryNode : Node
    {
        public BinaryNode Left { get; set; }
        public BinaryNode Right { get; set; }
        public string NextHop { get; set; } = "";

        /// <summary>
        /// Aggiunge un nodo al binary trie.
        /// </summary>
        /// <param name="prefix">Il nome del nodo</param>
        /// <param name="path">L'address come stringa di 0 e 1</param>
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

        /// <summary>
        /// Ritorna l'ultimo prefix di cui è figlio l'address.
        /// </summary>
        /// <param name="address">L'address per cui trovare il prefix</param>
        /// <param name="backtrack">L'ultimo nodo prefix visitato, lasciare vuoto</param>
        /// <returns>Il prefix</returns>
        public override string Lookup(string address, string backtrack = "")
        {
            // Se questo è un prefix, settalo come nodo di backtrack
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
    }
}
