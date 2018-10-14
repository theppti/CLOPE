using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsCLOPE
{
    /// <summary>
    /// Кластер это подмножество D
    /// набор наборов транзацкий
    /// причем
    /// 1. элементы кластера не пусты, 
    /// 2. пересечение любых элементов кластера пусто.
    /// </summary>
    //[DebuggerDisplay("S={S} W={W} D={D}")]
    public class Claster<T>
    //  where T: IConvertible
    {
        /// <summary>
        /// надо определять ее ДО заливки данных
        /// </summary>
        private IDictionary<T, int> _Occ = new SortedList<T, int>(22);

        public Claster()
        {
        }

        public ICollection<T> Keys { get { return _Occ.Keys; } }

        /// <summary>
        /// Число вхождений объекта i в кластер C
        /// </summary>
        /// <param name="i"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public int Occ(T obj)
        {
            if (_Occ.ContainsKey(obj) == false)
                return 0;

            return _Occ[obj];

        }

        /// <summary>
        /// Размер кластера
        /// </summary>
        public int Size;

        /// <summary>
        /// размер коллекции уникальных объектов
        /// </summary>
        /// <returns></returns>
        public int W;

        /// <summary>
        /// Площадь кластерa
        /// </summary>
        public int Square;

        public void RemoveTransaction(Transaction<T> arg)
        {
            T current;
            for (int i = 0; i < arg.SetOfObejct.Count; i++)
            {
                current = arg.SetOfObejct[i];

                if (_Occ[current] > 0)
                    _Occ[current]--;

                if (_Occ[current] < 1)
                    _Occ.Remove(current);
            }
            Square -= arg.Length;
            Size--;
            W = _Occ.Count;
        }
        internal void AddTransaction(Transaction<T> arg)
        {
            foreach (var item in arg.SetOfObejct)
            {
                if (_Occ.ContainsKey(item) == false)
                    _Occ.Add(item, 0);
                _Occ[item]++;
            }

            Square += arg.Length;
            Size++;
            W = _Occ.Count;
        }
    }
}
