using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsCLOPE
{
    public class TransactionReader : IClopeReader<Transaction<int>>
    {
        const int rowLength = 80;
        const int beginPosition = -1;
        FileStream fileStream;
        int position = beginPosition;

        public TransactionReader()
        {
            StreamReader sr = new StreamReader(MashDataSet.file_normal);
            StreamWriter sw = new StreamWriter(MashDataSet.file_result);
            while (!sr.EndOfStream)
                sw.WriteLine((sr.ReadLine() + ",").PadRight(rowLength - 2, ' '));
            sr.Close();
            sw.Close();
        }
        public bool EndOF => fileStream.Position == fileStream.Length;

        public void BeginRead()
        {
            position = beginPosition;
            if (fileStream == null)
                fileStream = new FileStream(MashDataSet.file_result, FileMode.Open);
            fileStream.Seek(0, SeekOrigin.Begin);
        }

        public void EndRead()
        {
            fileStream.Close();
            fileStream = null;
        }

        public Transaction<int> ReadNext()
        {
            byte[] buffer = new byte[rowLength];
            fileStream.Read(buffer, 0, rowLength);
            position++;
            List<int> parameters = new List<int>();
            Encoding.ASCII.GetString(buffer, 0, rowLength - 2).Split(',').ToList().ForEach(new Action<string>((e) =>
            {
                int value;
                int.TryParse(e.Trim(), out value);
                parameters.Add(value);
            }));
                        
            var res = new Transaction<int>(parameters.Take(parameters.Count - 1));
            res.ClusterNumber = parameters[parameters.Count - 1];

            return res;
        }

        public void Write(Transaction<int> transaction, int cluster_number)
        {
            transaction.ClusterNumber = cluster_number;
            string result = "";
            foreach (var item in transaction.SetOfObejct)
                result += item + ",";            
            result += cluster_number;
            result = result.PadRight(rowLength - 2) + "\r\n";
            fileStream.Seek(position * rowLength, SeekOrigin.Begin);
            fileStream.Write(Encoding.ASCII.GetBytes(result), 0, result.Length);
        }        
    }
}
