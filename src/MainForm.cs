using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsCLOPE
{
    public partial class MainForm : Form
    {
        ClastersSet<int> clasterset;
        TransactionReader treader;

        public MainForm()
        {
            InitializeComponent();
        }
               
        private void button2_Click(object sender, EventArgs e)
        {
            var alg = new ClopeAlg<int>();
                       
            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += new DoWorkEventHandler(new Action<object, DoWorkEventArgs>((o, dwe) =>
            {
                BeginInvoke(new Action(() =>
                {
                    button2.Enabled = false;
                }));

                double r;
                if (!double.TryParse(tbRepulsion.Text, out r))
                {
                    MessageBox.Show("Коэффициент отталкивания введён в неправильном формате!");
                    return;
                }
                InitFirst(r);
               
            }));

            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(new Action<object, RunWorkerCompletedEventArgs>((o, wce) =>
            {
                BeginInvoke(new Action(() => { RefreshGrid();}));
            }));

            bw.RunWorkerAsync();
        }

        private void InitFirst(double r)
        {
            MashDataSet.CreateDataFile();
            treader = new TransactionReader();
            clasterset = ClopeAlg<int>.Initialiazation(treader, r);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var alg = new ClopeAlg<int>();
            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += new DoWorkEventHandler(new Action<object, DoWorkEventArgs>((o, dwe) =>
            {
                BeginInvoke(new Action(() =>
                {
                    button1.Enabled = false;
                }));

                double r;
                if (!double.TryParse(tbRepulsion.Text, out r))
                {
                    MessageBox.Show("Коэффициент отталкивания введён в неправильном формате!");
                    return;
                }

                ClopeAlg<int>.Clasterization(treader, r, clasterset);
            }));

            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(new Action<object, RunWorkerCompletedEventArgs>((o, wce) =>
            {
                BeginInvoke(new Action(() => { RefreshGrid(); }));
            }));

            bw.RunWorkerAsync();
        }

        private void RefreshGrid()
        {
            if (clasterset != null)
            {
                dataGridView1.Rows.Clear();

                StreamReader srSrc = new StreamReader(MashDataSet.file_source);
                StreamReader srRes = new StreamReader(MashDataSet.file_result);
                Dictionary<int, int> em = new Dictionary<int, int>();
                Dictionary<int, int> pm = new Dictionary<int, int>();
                while (!srSrc.EndOfStream)
                {
                    string type = srSrc.ReadLine().Split(',')[0];
                    string[] transaction = srRes.ReadLine().Split(',');
                    var val = transaction[transaction.Length - 1];
                    int clusterNumber = int.Parse(val);
                    if (!pm.ContainsKey(clusterNumber))
                        pm.Add(clusterNumber, 0);
                    if (!em.ContainsKey(clusterNumber))
                        em.Add(clusterNumber, 0);

                    if (type == "e")
                        em[clusterNumber] += 1;
                    else
                        pm[clusterNumber] += 1;
                }
                srSrc.Close();
                srRes.Close();

                int index = 1;
                for (int i = 0; i < clasterset.Count; i++)
                {
                    var c = clasterset[i];
                    //if (!em.ContainsKey(i) || !pm.ContainsKey(i))
                    //    continue;

                    dataGridView1.Rows.Add(index++, c.Square, c.W, c.Size, em[i], pm[i]);                
                }
            }
            button2.Enabled = true;
            button1.Enabled = true;
        }
    }
}
