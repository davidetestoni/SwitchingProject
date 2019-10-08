using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchingProject.Nodes
{
    public class MultibitNode : Node
    {
        // Stride = 3 by default
        public int Stride { get; set; } = 3;
        // Il data structure è un dizionario dove c'è una chiave es. 001 e un valore che è composto dal prefix e, se esiste, dal prossimo Multibit node
        public Dictionary<string, (string, MultibitNode)> Children { get; set; } = new Dictionary<string, (string, MultibitNode)>();

        /// <summary>
        /// Aggiunge un prefix al multibit trie ed eventualmente genera ulteriori nodi necessari.
        /// </summary>
        /// <param name="prefix">Il nome del nodo</param>
        /// <param name="path">L'address come stringa di 0 e 1</param>
        public override void AddChild(string prefix, string path)
        {
            if (path == "") return;
            var chunks = Split(path);
            var first = chunks.First();

            // Se è l'unico chunk
            if (chunks.Count() == 1)
            {
                // Se è esattamente lungo come la stride
                if (first.Length == Stride)
                {
                    Children[first] = (prefix, null);
                }
                // Altrimenti espandi il chunk con tutte le combinazioni possibili
                else
                {
                    foreach (var comb in GetCombinations(Stride - first.Length))
                    {
                        Children[first + comb] = (prefix, null);
                    }
                }
            }
            // Altrimenti, se ci sono ulteriori chunks
            else
            {
                // Se non esiste, crealo, altrimenti setta solamente il puntatore
                if (!Children.ContainsKey(first)) Children[first] = ("", new MultibitNode());
                else Children[first] = (Children[first].Item1, new MultibitNode());

                // Chiama di nuovo il metodo sul figlio
                Children[first].Item2.AddChild(prefix, path.Substring(Stride));
            }
        }

        /// <summary>
        /// Ritorna l'ultimo prefix di cui è figlio l'address.
        /// </summary>
        /// <param name="address">L'address per cui trovare il prefix</param>
        /// <param name="backtrack">L'ultimo nodo prefix visitato, lasciare vuoto</param>
        /// <returns>Il prefix</returns>
        public override string Lookup(string address, string backtrack)
        {
            // Se abbiamo l'address piu piccolo della stride, ritorna l'ultimo nodo visitato
            if (address.Length < Stride) return backtrack;

            // Tagliamo l'address
            var first = address.Substring(0, Stride);

            // Se non esiste il figlio, ritorna l'ultimo nodo visitato
            if (address.Length < Stride || !Children.ContainsKey(first)) return backtrack;

            // Prendimo la reference in modo da non doverla cercare ogni volta nel dizionario tramite chiave
            var child = Children[first];

            // (Qui ci arriviamo quando l'address è almeno lungo quanto la stride)
            // Se è lungo quanto la stride, o non esiste un puntatore ad un altro nodo, ritorna il prefix
            if (address.Length == Stride || child.Item2 == null)
            {
                return child.Item1;
            }
            // Se invece è più lungo della stride e abbiamo un puntatore
            else
            {
                return child.Item2.Lookup(address.Substring(Stride), child.Item1 == "" ? backtrack : child.Item1);
            }
        }

        /// <summary>
        /// Ritorna l'ultimo prefix di cui è figlio l'address, senza ricorsione (più veloce).
        /// </summary>
        /// <param name="address">L'address per cui trovare il prefix</param>
        /// <param name="rootPrefix">Il nome del prefisso del nodo root</param>
        /// <returns>Il prefix</returns>
        public override string LookupNonRecursive(string address, string rootPrefix)
        {
            var backtrack = rootPrefix;
            var partialAddress = address;
            var node = this;

            // Ciclo finchè l'address non si svuota
            while (node != null)
            {
                if (partialAddress.Length < Stride) return backtrack;

                var first = partialAddress.Substring(0, Stride);

                if (partialAddress.Length < Stride || !node.Children.ContainsKey(first)) return backtrack;

                var child = node.Children[first];

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

        private string[] Split(string address)
        {
            if (address.Length < Stride) return new string[] { address };
            return Enumerable.Range(0, (int)Math.Ceiling((double)address.Length / (double)Stride))
                .Select(i => address.Substring(i * Stride, Math.Min(address.Length - i * Stride, Stride)))
                .ToArray();
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
