using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchingProject.Nodes
{
    public abstract class Node
    {
        public virtual void AddChild(string prefix, string path)
        {
            throw new NotImplementedException();
        }

        public virtual string Lookup(string address, string backtrack)
        {
            throw new NotImplementedException();
        }
    }
}
