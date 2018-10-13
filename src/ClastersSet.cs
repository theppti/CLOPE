using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsCLOPE
{
    public class ClastersSet<T>: List<Claster<T>>
    {
        public override string ToString()
        {
            return "{" + string.Join(", ", this.Select(x => x.ToString())) + "}";
        }              
    }
}
