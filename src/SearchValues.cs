using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsCLOPE
{
    public class SearchValues<T>
    {
        public Claster<T> Claster;
        public double Value;
        public int Index = -1;

        public SearchValues(Claster<T> c, int v)
        {
            this.Claster = c;
            this.Value = v;
        }

        internal void Reset()
        {
            Claster = null;
            Value = 0;
            Index = -1;
        }
    }
}
