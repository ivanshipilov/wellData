using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace well
{
    public partial class MainForm : Form
    {
        // создание List со скважинами в проекте  
        List<Well> listWells = new List<Well>();

        public MainForm()
        {
            InitializeComponent();

            //Load += MainForm_Load;
            //Panel p2 = splitContainer1.Panel2;
            //p2.Paint += new PaintEventHandler(Panel2Draw);
        }

        //private void Panel2Draw(object sender, PaintEventArgs e)
        //{
        //    Graphics g = e.Graphics;
        //    Panel p = sender as Panel;

        //    Pen pen = new Pen(Brushes.Green, 20f);
        //    Point p1 = new Point(10, 10);
        //    Point p2 = new Point(this.Width, this.Height);
        //    g.DrawLine(pen, p1, p2);
        //}

        //private void MainForm_Load(object sender, EventArgs e)
        //{

        //}

        protected override void OnPaint(PaintEventArgs e)
        {
            //Graphics g = Graphics.FromHwnd(Handle);
            //Pen pen = new Pen(Brushes.Green, 20f);
            //Point p1 = new Point(10,10);
            //Point p2 = new Point(this.Width, this.Height);
            //g.DrawLine(pen, p1, p2);
        }

        //private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        //{

        //}

        // открытие нового файла из главного меню
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // проверка, если нажата отмена
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            else
            {
                //открытие файла - перенести в метод well?
                string filePath = openFileDialog1.FileName;
                string fileContent = System.IO.File.ReadAllText(filePath);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                //создается новая скважина                  
                Well WellNew = new Well(fileName, filePath);

                //скважина добавляется в список скважин - пока не используется
                listWells.Add(WellNew);

                //в дерево скважин добавляется новая скважина   
                wellsTree.Nodes.Add(listWells[Well.count - 1].WellName);
                //добавление методов в скважину (в дереве)
                foreach (string method in WellNew.WellMethods(filePath))
                {
                    wellsTree.Nodes[Well.count - 1].Nodes.Add(method);
                }
                wellsTree.ExpandAll();


                //List<decimal> temp = new List<decimal>();
                //temp = 

                //MessageBox.Show(WellNew.WellData(filePath).Keys.First());

                // MessageBox.Show(WellNew.WellData(filePath).Count.ToString());




                string сhartName = WellNew.WellName + Well.count;
                Chart chartNew = new Chart();
                chartNew.Parent = splitContainer1.Panel2;
                // chartNew.Dock = DockStyle.Fill;
                chartNew.ChartAreas.Add(new ChartArea(сhartName));
                Series ser1 = new Series(сhartName);
                ser1.ChartType = SeriesChartType.Line;
                ser1.ChartArea = сhartName;
                string depthCol = WellNew.WellData(filePath).Keys.First();

                for (int i = 0; i < WellNew.WellData(filePath).Count; i++) //fix всего 6 значений
                {
                    foreach (decimal depthValue in WellNew.WellData(filePath)[depthCol])
                    {
                        int x = 0; decimal y = depthValue;
                        ser1.Points.AddXY(x, y);
                    }
                 //   chartNew.Series.Add(ser1);
                 //проверка репозитория
                }

                //for (double x = -Math.PI; x <= Math.PI; x += Math.PI / 10.0)
                //{
                //    ser1.Points.AddXY(x, Math.Sin(x));
                //}
                //chartNew.Series.Add(ser1);
                //foreach (KeyValuePair<string, List<decimal>> pair in WellNew.WellData(filePath))
                //{
                //foreach (decimal val in pair.Value)
                //{ s =/* s +*/ "key: " + pair.Key + "val= " + val + Environment.NewLine; }
                //}
                // MessageBox.Show(s);




            }
        }        
    }
}
