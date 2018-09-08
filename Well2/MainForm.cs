﻿using System;
using System.Collections.Generic;
using System.Drawing;
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
            wellsTree.Nodes[0].Name = well.WellPath;
            //добавление методов в скважину (в дереве)
            foreach (string method in well.WellMethods())
            {
                wellsTree.Nodes[Well.count - 1].Nodes.Add(method);
            }
            wellsTree.ExpandAll();

            //создание новой ячейки tablePanel
            panel.RowCount = panel.RowCount + 1;
        }

        // создание List со скважинами в проекте  

        //создание tablePanel для последующего добавления в нее скважин
        TableLayoutPanel panel = new TableLayoutPanel();
        Controller controller = null;
        Dictionary <string, Chart> charts = new Dictionary <string, Chart>();


        public MainForm()
        {
            InitializeComponent();
            WellsArea wellsArea = new WellsArea();
            controller = new Controller(wellsArea);
            panel.Parent = splitContainer1.Panel2;
            panel.Dock = DockStyle.Fill;
            panel.AutoScroll = true;
        }

        // открытие нового файла из главного меню
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // проверка, если нажата отмена
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            else
            {
                string filePath = openFileDialog1.FileName;
                controller.AddWells(filePath);
                DrawTree(controller.GetWell(filePath));
            }
        }

        private Chart CreateChart(string wellName, TableLayoutPanel panel)
        {
            //Well well = controller.GetWell(wellName);
            int colCount = panel.ColumnCount -1;            
            Chart chartNew = new Chart();
            chartNew.Name = wellName;
            chartNew.Dock = DockStyle.Fill;
            panel.Controls.Add(chartNew, colCount, 0);
            panel.ColumnCount += 1;
            return chartNew;
        }
        private ChartArea CreateChartArea (string wellMethod, Chart chart)
        {
            ChartArea chartArea = new ChartArea(wellMethod);        
            chartArea.AxisY.IsReversed = true;
            if (chart.ChartAreas.Count != 0)
            {
                chartArea.Position.X = chart.ChartAreas.Last().Position.Right;                
            }
            chartArea.Position.Y = 0;
            chart.ChartAreas.Add(chartArea);
            return chartArea;
        }
        //событе нажатие на узел в дереве скважин
        private void wellsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            Chart chart = null;
            if (charts.ContainsKey(e.Node.Parent.Text))
            {
                chart = charts[e.Node.Parent.Text];
            }
            else
            {
                chart = CreateChart(e.Node.Parent.Text, panel);
                charts.Add(e.Node.Parent.Text, chart);
            }
            CreateChartArea(e.Node.Text, chart);
            
          


           
            
            //    //Series ser = new Series(chartAreaName);
            //    //ser.ChartType = SeriesChartType.Line;
            //    //ser.XAxisType = AxisType.Secondary;
            //    //ser.ChartArea = chartAreaName;

            //    //try
            //    //{
            //    //    //chartNew.ChartAreas[i].Position.Auto = true;
            //    //    chartNew.ChartAreas[i].Position.X = chartNew.ChartAreas[i - 1].Position.Right;
            //    //    chartNew.ChartAreas[i].Position.Y = 0;
            //    //}
            //    //catch (Exception)
            //    //{
            //    //    //chartNew.ChartAreas[i].Position.Auto = true;
            //    //    chartNew.ChartAreas[i].Position.X = 0;
            //    //    chartNew.ChartAreas[i].Position.Y = 0;
            //    //}
            //    //chartNew.ChartAreas[i].Position.Width = 20;//panel.Width/countMethods;
            //    //chartNew.ChartAreas[i].Position.Height = graphHeight;

            //    //string depthCol = well.WellData(WellPath).Keys.First();                 //fix

            //    //foreach (decimal depthValue in well.WellData(WellPath)[depthCol])    //fix
            //    //{
            //    //    int x = 0; decimal y = depthValue;
            //    //    ser.Points.AddXY(x, y);
            //    //}
            //    //chartNew.Series.Add(ser);
            //}

        }
        //проверка - выбран ли хоть 1 из методов скважины в дереве
        //private void checkCheckboxNodes(TreeNode ParentNode)
        //{
        //    int checkedNodes = 0;
        //    for (int i = 0; i < ParentNode.Nodes.Count; i++)
        //    {                
        //        if (ParentNode.Nodes[i].Checked == true)
        //        {                   
        //            checkedNodes++;
        //        }
        //    }
        //    if (checkedNodes == 0)
        //        ParentNode.Checked = false;
        //    else
        //        ParentNode.Checked = true;
        //}
        //private void SelectTreeNode(TreeNode node)
        //{
        //    if (node.Level == 1)
        //    {
        //        checkCheckboxNodes(node.Parent);
        //    }
        //}

        private void wellsTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            
            //e.Node.Tag = e.Node.Checked;
            //    if (!e.Node.Checked)
            //    {
            //        e.Node.Checked = true;                
            //    }            
            //    else
            //    {
            //        e.Node.Checked = false;
            //    }
            //    SelectTreeNode(e.Node);
        }

        private bool check = false;
        private void wellsTree_AfterCheck(object sender, TreeViewEventArgs e)
        {  
            if (check == true)
            {
                return;
            }
            check = true;
            SelectParents(e.Node, e.Node.Checked);
            check = false;
            //if (e.Node.Tag != null)
            //{
            //    e.Node.Checked = (bool)e.Node.Tag;
            //    e.Node.Tag = null;
            //}
            //MessageBox.Show(e.Node.Checked + "включен");
            //if (!e.Node.Checked)
            //{
            //    e.Node.Checked = true;
            //}
            //else
            //{
            //    e.Node.Checked = false;
            //}
            //SelectTreeNode(e.Node);
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

        private void wellsTree_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            //MessageBox.Show(e.Node.Checked + "включен");
            //if (!e.Node.Checked)
            //{
            //    e.Node.Checked = true;
            //}
            //else
            //{
            //    e.Node.Checked = false;
            //}
            //SelectTreeNode(e.Node);
        }

        private void wellsTree_DoubleClick(object sender, EventArgs e)
        {
            int a = 0;
        }

        private void wellsTree_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int a = 0;
        }
    }

}
