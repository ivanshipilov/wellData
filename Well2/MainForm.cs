//свойство panel scale
//перемещение при удалении метода скважины
//масштабирование
//ошибка дерева
//str 81 никак не получается сделать подпись над графиком(
//str 107 - фильтрация

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Well2;

namespace well
{
    public partial class MainForm : Form
    {
        private void DrawTree(Well well)
        {
            wellsTree.Nodes.Add(well.WellName);
            wellsTree.Nodes[wellsTree.Nodes.Count-1].Name = well.WellPath;
            //добавление методов в скважину (в дереве)
            foreach (string method in well.WellMethods())
            {
                wellsTree.Nodes[Well.count - 1].Nodes.Add(method);
            }
            wellsTree.ExpandAll();
        }

        public const string extension = ".las";
        TableLayoutPanel panel = new TableLayoutPanel();
        Controller controller = null;
        WellsArea wellsArea = null;
        Dictionary <string, Chart> charts = new Dictionary <string, Chart>();

        public MainForm()
        {
            InitializeComponent();
            this.MouseWheel += Scale;
            wellsArea = new WellsArea();            
            controller = new Controller(wellsArea);
            panel.Parent = splitContainer1.Panel2;
            //panel.Height = splitContainer1.Panel2.Height;
            //panel.Width = splitContainer1.Panel2.Width;
            panel.Dock = DockStyle.Fill;
            panel.AutoScroll = true;
        }

        // открытие нового файла из главного меню
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            else
            {
                string filePath = openFileDialog1.FileName;
                OpenFile(filePath);                
            }
        }

        private void OpenFile(string filePath)
        {
            controller.AddWells(filePath);
            DrawTree(controller.GetWell(filePath));
        }

        //событе нажатие на узел в дереве скважин       
        private void wellsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)            
        {
            string strWellname = e.Node.Parent.Name;
            string strWell = e.Node.Parent.Text;
            string strMethod = e.Node.Text;

            //если node делается unchecked:
            if (e.Node.Checked == false)
            {   
                //удаляется набор данных serie для метода и charArea
                charts[strWell].Series.RemoveAt(charts[strWell].ChartAreas.IndexOf(strMethod));
                charts[strWell].ChartAreas.RemoveAt(charts[strWell].ChartAreas.IndexOf(strMethod));
                return;
            }            
            SelectParents(e.Node, e.Node.Checked);
            addMethodToScreen(strWell, strWellname, strMethod);
        }

        private void addMethodToScreen (string wellName, string wellPath, string strMethod)
        {
            Chart chart = null;
            ChartArea chartArea = null;
            Series serie = null;
            if (charts.ContainsKey(wellName))
            {
                chart = charts[wellName];
            }
            else
            {
                chart = CreateChart(wellName, panel);
                charts.Add(wellName, chart);
            }
            chartArea = CreateChartArea(strMethod, chart);
            serie = CreateSerie(wellPath, strMethod, chartArea);
            DrawGraphInChart(chart, chartArea, serie);
            ScaleSet(wellPath, chart);
        }

        private Chart CreateChart(string wellName, TableLayoutPanel panel)
        {
            int colCount = panel.ColumnCount - 1;
            Chart chartNew = new Chart();
            chartNew.Name = wellName;
            //chartNew.Dock = DockStyle.Fill;
            panel.Controls.Add(chartNew, colCount, 0);
            panel.ColumnCount += 1;            
            return chartNew;
        }
        private ChartArea CreateChartArea(string wellMethod, Chart chart)
        {
            ChartArea chartArea = new ChartArea(wellMethod);
            chartArea.AxisY.IsReversed = true;
            if (chart.ChartAreas.Count != 0)
            {
                chartArea.Position.X = chart.ChartAreas.Last().Position.Right;
            }

            chartArea.Position.Width = 30;
            chartArea.Position.Height = 100;

            chartArea.AxisY.Title = wellMethod;
            chartArea.AxisY.LabelStyle.IntervalOffset = 0;

            chart.ChartAreas.Add(chartArea);
            return chartArea;
        }
        private Series CreateSerie(string wellPath, string wellMethod, ChartArea chartArea)
        {
            Well well = controller.GetWell(wellPath);
            Dictionary<string, List<decimal>> wellData = well.WellData();
            string depthDataKey = wellData.First().Key;
            List<decimal> methodData = wellData[wellMethod];
            Series serie = new Series(chartArea.Name);
            int i = 0;
            //создается набор данных для чарта где глубина по X - т.к. фильтрация делается только по Y
            foreach (decimal depthValue in wellData[depthDataKey])
            {
                decimal y = methodData[i]; decimal x = depthValue;
                serie.Points.AddXY(x, y);
                i += 1;
            }         
            //фильтрация - fix! вынести в отдельный метод, значение передавать пока как параметр, в дальнейшем сделать для пользователя поле для ввода неактуального значения
            //еще далее добавить аналогичную фильтрацию для значений >x <x
            DataManipulator filter = new DataManipulator();
            filter.Filter(CompareMethod.EqualTo, -999.250, serie);
            Series serie1 = new Series(chartArea.Name);
            //меняются местами x и y для правильного отображения графиков
            foreach (var item in serie.Points)
            {
                double y = item.XValue;
                double x = item.YValues[0];
                serie1.Points.AddXY(x, y);
            }
            return serie1;
        }
        //настройка и отрисовка графика в чарте
        private void DrawGraphInChart(Chart chart, ChartArea chartArea, Series serie)
        {
            serie.ChartType = SeriesChartType.Line;
            serie.XAxisType = AxisType.Secondary;
            serie.ChartArea = chartArea.Name;
            chart.Series.Add(serie);            
        }
        //масштабирование 
        private void ScaleSet(string wellPath, Chart chart)
        {
            Well well = controller.GetWell(wellPath);
            decimal wellDepth = well.WellDepth;
            chart.Height = (int)(wellDepth / wellsArea.Scale);
        }

        private void SelectParents(TreeNode node, Boolean isChecked)
        {
            var parent = node.Parent;

            if (parent == null)
                return;

            if (isChecked)
            {
                parent.Checked = true; // we should always check parent
                SelectParents(parent, true);
            }
            else
            {
                if (parent.Nodes.Cast<TreeNode>().Any(n => n.Checked))
                    return; // do not uncheck parent if there other checked nodes
                parent.Checked = false;
                SelectParents(parent, false); // otherwise uncheck parent
            }
        }

        private void wellsTree_DragEnter(object sender, DragEventArgs e)
        {            
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                bool allow = true;
                foreach (string file in files)
                {                                        
                    if (Path.GetExtension(file).ToLower() != extension)
                    {                       
                        allow = false;
                        break;
                    }
                }
                e.Effect = (allow ? DragDropEffects.Copy : DragDropEffects.None);                
            }
        }

        private void wellsTree_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePathes = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in filePathes)
                {
                    OpenFile(file);
                }
            }
        }
        private void Scale (object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift)
            {
                if (e.Delta > 0) 
                  panel.Scale(new SizeF(1, ((float)1.2)));                   
                else
                    panel.Scale(new SizeF(1, ((float)0.8)));
            }                     
        }        
    }
}
