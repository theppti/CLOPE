using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsCLOPE
{
    /// <summary>
    /// Транзакция набор объектов i1....im
    /// </summary>
    public class Transaction<T>
    {
        private List<T> _set;

        public Transaction(IEnumerable<T> v)
        {
            _set = new List<T>(v);
        }

        public Transaction()
        {
            _set = new List<T>();
        }

        public IList<T> SetOfObejct { get { return _set; } }

        public override string ToString()
        {
            return string.Join(" ", SetOfObejct);
        }

        internal int Length
        {
            get
            {
                return SetOfObejct.Count;
            }
        }

        public int ClusterNumber { get; set; }
    }
}
