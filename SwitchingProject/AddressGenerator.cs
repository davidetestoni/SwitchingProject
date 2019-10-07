using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchingProject
{
    public class AddressGenerator
    {
        public Random Rand { get; set; } = new Random();

        public IEnumerable<string> Generate()
        {
            while (true)
            {
                var length = Rand.Next(1, 32);
                var address = "";
                for (var i = 0; i < length; i++)
                {
                    address += (Rand.Next() % 2).ToString();
                }
                yield return address;
            }
        }
    }
}
