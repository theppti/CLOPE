using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsCLOPE
{
    public class ClopeAlg<T>
    {
        public static double DeltaAdd(Claster<T> cluster, Transaction<T> t, double r)
        {
            var res1 = DeltaAdd1(cluster, t, r);
            var res2 = DeltaAdd2(cluster, t, r);
            return res1;
        }
        /// <summary>
        /// цена добавления транзакции
        /// </summary>
        /// <param name="t"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static double DeltaAdd1(Claster<T> cluster, Transaction<T> t, double r)
        {
            // для пустого кластера вычисляем вот так 
            if (cluster == null)
                return t.Length / Math.Pow(t.Length, r);

            int widthNew = cluster.W;

            foreach (var item in t.SetOfObejct.Where(x => cluster.Occ(x) == 0))
            {
                //if (cluster.Occ(item) == 0)
                widthNew++;
            }

            return (cluster.Square + t.Length) * (cluster.Size + 1) / Math.Pow(widthNew, r) - cluster.Square * cluster.Size / Math.Pow(cluster.W, r);
        }
        public static double DeltaAdd2(Claster<T> cluster, Transaction<T> t, double r)
        {
            if (cluster == null)
                return t.Length / Math.Pow(t.Length, r);

            int widthNew = cluster.W;

            foreach (var item in t.SetOfObejct.Except(cluster.Keys))
            {
                widthNew++;
            }

            return (cluster.Square + t.Length) * (cluster.Size + 1) / Math.Pow(widthNew, r) - cluster.Square * cluster.Size / Math.Pow(cluster.W, r);
        }
       
        /// <summary>
        /// цена вычитания транзакции
        /// </summary>
        /// <param name="t"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static double DeltaRemove(Claster<T> cluster, Transaction<T> t, double r)
        {
            int widthNew = cluster.W;

            foreach (var item in t.SetOfObejct.Where(x => cluster.Occ(x) == 1))
            {
                //if (cluster.Occ(item) == 1)
                widthNew--;
            }

            return (cluster.Square - t.Length) * (cluster.Size - 1) / Math.Pow(widthNew, r) - cluster.Square * cluster.Size / Math.Pow(cluster.W, r);
        }

        /// <summary>
        /// начальная инциализация
        /// </summary>
        /// <param name="treader"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static ClastersSet<T> Initialiazation(IClopeReader<Transaction<T>> treader, double r)
        {
            var res = new ClastersSet<T>();

            //
            // 1. начальная инициализация набора кластеров
            //
            // по порядку перебираются все транзакции, и для каждой из них происходит вычисление цены ее добавления в существующие кластеры или же в новый, изначально пустой, кластер.
            // Исходя из вычисленных значений, каждой транзакции назначается кластер(какой - либо из уже существующих или новый)
            // выбирается кластер(при необходимости создается новый), для которого было получено максимальное значение цены добавления данной транзакции. 
            // Таким образом происходит начальное распределение транзакций по кластерам(инициализация кластеризации).
            int iteration = 0;
            var max_deltaadd = new SearchValues<T>(null, 0);
            double res_add;
            Transaction<T> transaction; // не будем каждый раз выделять под нее память.
            Claster<T> claster_new; // создадимо его только если он будет нужен
            Claster<T> claster_cur; // в цикле не будет инициализироваться новая память

            treader.BeginRead();

            while (!treader.EndOF)
            {
                iteration++;
                transaction = treader.ReadNext();

                // начальные значения для поиска (возможно быстрее прям здесь по порядку обнулить поля, чем вызывать функцию)
                max_deltaadd.Reset();

                res_add = 0;

                for (int i = 0; i < res.Count; i++)
                {
                    claster_cur = res[i];
                    res_add = DeltaAdd(claster_cur, transaction, r); // 48% занимает от времени инициализации

                    if (max_deltaadd.Value < res_add)
                    {
                        max_deltaadd.Claster = claster_cur;
                        max_deltaadd.Value = res_add;
                        max_deltaadd.Index = i;
                    }
                }

                res_add = DeltaAdd(null, transaction, r);
                if (max_deltaadd.Value < res_add)
                {
                    claster_new = new Claster<T>();

                    max_deltaadd.Claster = claster_new;
                    max_deltaadd.Value = res_add;
                    max_deltaadd.Index = res.Count;

                    // если выбран новый кластер, то добавляем его в массив кластеров
                    res.Add(claster_new);
                }

                // добавляем транзакцию в максимально увеличивающий дельту кластер
                max_deltaadd.Claster.AddTransaction(transaction);
                treader.Write(transaction, max_deltaadd.Index);
            }

            treader.EndRead();

            return res;
        }

        /// <summary>
        /// 2. Уточняющие итерации
        /// </summary>
        /// <param name="treader"></param>
        /// <param name="r"></param>
        /// <param name="res"></param>
        internal static void Clasterization(IClopeReader<Transaction<T>> treader, double r, ClastersSet<T> res)
        {
            // число перемещений
            int replace_count;
            double res_add;
            var max_deltaadd = new SearchValues<T>(null, 0); // ициниализация из образца
            Claster<T> claster_for;
            Claster<T> claser_new;

            // начало цикла repeat
            do
            {
                replace_count = 0;
                treader.BeginRead();

                while (!treader.EndOF)
                {
                    var transaction = treader.ReadNext();

                    var current_claster = res[transaction.ClusterNumber];

                    var deltaremove = DeltaRemove(current_claster, transaction, r);

                    max_deltaadd.Reset();// = new SearchValues<T>(null, 0); // ициниализация из образца

                    //foreach (var claster in res .Except(new Claster<T>[] { current_claster }) )
                    for (int i = 0; i < res.Count; i++)
                    {
                        claster_for = res[i];
                        if (claster_for == current_claster)
                            continue;

                        res_add = DeltaAdd(claster_for, transaction, r); // 62% занимает при кластеризации

                        //если для транзакции найден кластер с большей ценой перемещения, сохраняем эту информацию
                        if (res_add + deltaremove > max_deltaadd.Value)
                        {
                            max_deltaadd.Claster = claster_for;
                            max_deltaadd.Value = res_add;
                            max_deltaadd.Index = i;
                        }
                    }

                    res_add = DeltaAdd(null, transaction, r);
                    if (res_add + deltaremove > max_deltaadd.Value)
                    {
                        claser_new = new Claster<T>();

                        max_deltaadd.Claster = claser_new;
                        max_deltaadd.Value = res_add;
                        max_deltaadd.Index = res.Count;

                        // если выбран новый кластер, то добавляем его в массив кластеров
                        res.Add(claser_new);
                    }

                    // было найдено выгодное перемещение
                    if (max_deltaadd.Claster != null && current_claster != max_deltaadd.Claster)
                    {
                        current_claster.RemoveTransaction(transaction);
                        max_deltaadd.Claster.AddTransaction(transaction);

                        treader.Write(transaction, max_deltaadd.Index);
                        replace_count++;
                    }
                }

                treader.EndRead();

                // удаляем пустые кластеры
                foreach (var item in res.Where(x => x.Size == 0 || x.W == 0).ToArray())
                {
                    res.Remove(item);
                }
            }
            while (replace_count != 0);
        }
    }
}
