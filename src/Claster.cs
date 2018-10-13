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
        //public const int __SIZE = 300;

        //private Dictionary<T, int> _Occ = new Dictionary<T, int>();
        //ArrayList _Occ1;// надо гдето инициализировать при заливке данных
        //int _Occ1_Size = 0;

        private IDictionary<T, int> _Occ = new SortedList<T, int>(22);

        public Claster()
        {
            //_Occ1 = ArrayList.Repeat(null, __SIZE);
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
            //int ind = Convert.ToInt32(obj);

            //return (_Occ1[ind] == null) ? 0: Convert.ToInt32(_Occ1[ind]);

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
                //int ind = Convert.ToInt32(current);

                //if ((int)_Occ1[ind] > 0)
                //    _Occ1[ind] = (int)_Occ1[ind] - 1;

                //if ((int)_Occ1[ind] < 1)
                //{
                //    _Occ1[ind] = null;
                //    _Occ1_Size--;
            }            
            Square -= arg.Length;
            Size--;
            W = _Occ.Count;
            //W = _Occ1_Size;
        }
        internal void AddTransaction(Transaction<T> arg)
        {
            foreach (var item in arg.SetOfObejct)
            {
            if (_Occ.ContainsKey(item) == false)
                _Occ.Add(item, 0);
            _Occ[item]++;

            //int ind = Convert.ToInt32(item);

            //    if (_Occ1[ind] == null)
            //    {
            //        _Occ1[ind] = 0;
            //        _Occ1_Size++;
            //    }
            //    _Occ1[ind] = (int)_Occ1[ind] + 1;
            }

        Square += arg.Length;
            Size++;
            W = _Occ.Count;
            //W = _Occ1_Size;
        }
    }
}
