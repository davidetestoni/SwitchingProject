using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchingProject.Nodes
{
    public class CompressedNode : Node
    {
        public CompressedNode Left { get; set; }
        public CompressedNode Right { get; set; }
        public string NextHop { get; set; } = "";
        public int Skip { get { return Segment.Length; } }
        public string Segment { get; set; } = "";
        
        public CompressedNode Compress(string segment = "")
        {
            // Se non ho figli, ritorna me stesso
            if (Right == null && Left == null)
            {
                Segment = segment;
                return this;
            }

            // Se ho due figli, non posso comprimere ma chiamo la compressione sui figli, poi ritorno me stesso
            if (Right != null && Left != null)
            {
                Right = Right.Compress("");
                Left = Left.Compress("");
                Segment = segment;
                return this;
            }

            // SEZIONE: Se ho un solo figlio

            // Se sono un prefix, smetto di comprimere la catena corrente e chiamo la compressione sul figlio con segment vuoto
            if (NextHop != "")
            {
                Segment = segment;

                // Se il figlio è il sinistro
                if (Left != null)
                {
                    // Setto il mio sinistro come il risultato della compressione del sinistro
                    Left = Left.Compress("");
                }

                // Se il figlio è il destro
                else
                {
                    // Setto il mio destro come il risultato della compressione del destro
                    Right = Right.Compress("");
                }

                return this;
            }
            // Se non sono un prefix
            else
            {
                // Se il figlio è il sinistro
                if (Left != null)
                {
                    // Ritorno il risultato della compressione
                    return Left.Compress(segment + "0");
                }

                // Se il figlio è il destro
                else
                {
                    // Ritorno il risultato della compressione
                    return Right.Compress(segment + "1");
                }
            }
        }

        /// <summary>
        /// Aggiunge un nodo al compressed trie.
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

            // Se l'address inizia con 0
            if (address.StartsWith("0"))
            {
                // Se il figlio sinistro esiste, l'address è più lungo del segment, e il segment è vuoto o matcha
                if (Left != null && address.Length >= Left.Skip + 1 && (Left.Segment == "" || address.StartsWith("0" + Left.Segment)))
                {
                    return Left.Lookup(address.Substring(Left.Skip + 1), backtrack);
                }
                else return backtrack;
            }
            else
            {
                // Se il figlio destro esiste, l'address è più lungo del segment, e il segment è vuoto o matcha
                if (Right != null && address.Length >= Right.Skip + 1 && (Right.Segment == "" || address.StartsWith("1" + Right.Segment)))
                {
                    return Right.Lookup(address.Substring(Right.Skip + 1), backtrack);
                }
                else return backtrack;
            }
        }
    }
}
