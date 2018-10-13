using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsCLOPE
{
    public interface IClopeReader<T>
    {
        bool EndOF { get; }
        T ReadNext();

        void BeginRead();
        void EndRead();

        void Write(T transaction, int cluster_number);
    }
}
