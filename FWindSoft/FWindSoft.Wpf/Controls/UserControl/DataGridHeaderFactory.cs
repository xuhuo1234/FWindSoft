using FWindSoft.Data;
using FWindSoft.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace FWindSoft.Wpf
{
    public class DataGridHeaderFactory
    {
        private List<TNode> m_TNodes;
        private DataGrid m_RefDataGrid;
        private List<Tuple<ColumnDefinition, DataGridColumn,double>> m_RelationColumns;//加额外的修正值
        private double m_SplitBorder;
        public DataGridHeaderFactory(DataGrid dataGrid)
        {
            m_RefDataGrid = dataGrid;
            if (dataGrid == null)
            {
                throw new ArgumentNullException(nameof(dataGrid) + "不能为null");
            }

            m_RelationColumns = new List<Tuple<ColumnDefinition, DataGridColumn,double>>();
           
            var tuples = m_RefDataGrid.Columns.Select(c => new Tuple<string, object>(c.Header.ToString(), c)).ToList();
            m_TNodes = TreePathParse.Parse(tuples);
            m_SplitBorder = 1;
        }
        /// <summary>
        /// 创建表头
        /// </summary>
        /// <returns></returns>
        public Grid CreateHeader()
        {
            if (this.m_TNodes == null)
                throw new ArgumentNullException();
            Grid grid = new Grid();
            CreateHeader(grid, this.m_TNodes);
            return grid;
        }

        private void CreateHeader(Grid grid, List<TNode> tNodes)
        {
            int curentColumnIndex = 0;
            for (int i = 0; i < tNodes.Count; i++)
            {
                TNode node = tNodes[i];
                if (node.IsLeaf)
                {

                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = GetWidthEx(node, i == 0 ? -m_SplitBorder : -2 * m_SplitBorder);//new GridLength(1, GridUnitType.Star);
                    column.MinWidth = 20;
                    grid.ColumnDefinitions.Add(column);
                    TextBlock textBlock = CreateTextBlock(node.Name);
                    Grid.SetColumn(textBlock, curentColumnIndex++);
                    grid.Children.Add(textBlock);

                    DataGridColumn dataColumn = node.Tag as DataGridColumn;
                    if (dataColumn != null)
                    {
                        this.m_RelationColumns.Add(new Tuple<ColumnDefinition, DataGridColumn,double>(column, dataColumn,this.m_SplitBorder));
                        DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(ColumnDefinition.WidthProperty, typeof(ColumnDefinition));
                        //EventHandler handler = (sender, e) =>
                        //{
                        //    //dataColumn.Width = new DataGridLength(column.ActualWidth);
                        //};
                        descriptor.AddValueChanged(column, UpdateDataGridColumnsSize);
                    }
                }
                else
                {
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = GetWidthEx(node, i == 0 ? -m_SplitBorder : -2 * m_SplitBorder); //new GridLength(1, GridUnitType.Star);
                    grid.ColumnDefinitions.Add(column);

                    Grid gridColumn = new Grid();
                    Grid.SetColumn(gridColumn, curentColumnIndex++);
                    grid.Children.Add(gridColumn);

                    RowDefinition row1 = new RowDefinition();
                    row1.Height = new GridLength(1, GridUnitType.Star);
                    gridColumn.RowDefinitions.Add(row1);
                    TextBlock textBlock = CreateTextBlock(node.Name);
                    Grid.SetRow(textBlock, 0);
                    gridColumn.Children.Add(textBlock);

                    RowDefinition row2 = new RowDefinition();
                    row2.Height = GridLength.Auto;
                    gridColumn.RowDefinitions.Add(row2);
                    GridSplitter splitter = CreateHSplitter();
                    Grid.SetRow(splitter, 1);
                    gridColumn.Children.Add(splitter);

                    RowDefinition row3 = new RowDefinition();
                    row3.Height = new GridLength(1, GridUnitType.Star);
                    gridColumn.RowDefinitions.Add(row3);
                    Grid gridChild = new Grid();
                    grid.HorizontalAlignment = HorizontalAlignment.Stretch;
                    grid.VerticalAlignment = VerticalAlignment.Stretch;
                    Grid.SetRow(gridChild, 2);
                    gridColumn.Children.Add(gridChild);

                    CreateHeader(gridChild, node.Nodes);
                }
                if (i != tNodes.Count - 1)
                {
                    ColumnDefinition columnSplit = new ColumnDefinition();
                    columnSplit.Width = GridLength.Auto;
                    grid.ColumnDefinitions.Add(columnSplit);
                    GridSplitter splitter = CreateVSplitter();
                    Grid.SetColumn(splitter, curentColumnIndex++);
                    grid.Children.Add(splitter);
                }
            }
        }
        /// <summary>
        /// 创建表头
        /// </summary>
        /// <returns></returns>
        public Grid CreateHeaderEx()
        {
            if (this.m_TNodes == null)
                throw new ArgumentNullException();
            Grid grid = new Grid();
            List<int> depthes = new List<int>();
                 
           
            #region 取所有叶子节点
            List<TNode> tempNodes = new List<TNode>(this.m_TNodes);
            for (int i = 0; i < tempNodes.Count; i++)
            {
                TNode node = tempNodes[i];             
                if (node.IsLeaf)
                {
                    depthes.Add(node.Level);
                }
                else
                {
                    tempNodes.AddRange(node.Nodes);
                }
            }
            List<TNode> leaves = TNode.GetLeaves(this.m_TNodes);
            #endregion
            depthes=depthes.Distinct().ToList();
            int rowCount = MathUtil.MinCommonMultiple(depthes);
            int columnCount = this.m_TNodes.Sum(n => n.LeafCount);
            int rowExtend = rowCount * 2 - 1;
            int columnExtend = columnCount * 2 - 1+2;//为了使最后列能拖动
            for (int i = 0; i < columnExtend; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(1, GridUnitType.Star); //GetWidthEx(node, i == 0 ? -m_SplitBorder : -2 * m_SplitBorder);//
                grid.ColumnDefinitions.Add(column);

                
                if (i % 2 == 1)
                {
                    column.Width = GridLength.Auto;
                    //GridSplitter splitter = CreateVSplitter();
                    //Grid.SetColumn(splitter, i);
                    //Grid.SetRowSpan(splitter, rowExtend);
                    //grid.Children.Add(splitter);
                }
                else
                {
                    int leaveIndex = i/2;
                    if (leaveIndex < leaves.Count)
                    {
                        DataGridColumn dataColumn = leaves[leaveIndex].Tag as DataGridColumn;
                        if (dataColumn != null)
                        {
                            column.Width = GetWidthEx(leaves[leaveIndex], i == 0 ? -m_SplitBorder : -2 * m_SplitBorder);
                            this.m_RelationColumns.Add(new Tuple<ColumnDefinition, DataGridColumn, double>(column, dataColumn, 2 * this.m_SplitBorder));
                            DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(ColumnDefinition.WidthProperty, typeof(ColumnDefinition));
                            //EventHandler handler = (sender, e) =>
                            //{
                            //    //dataColumn.Width = new DataGridLength(column.ActualWidth);
                            //};
                            descriptor.AddValueChanged(column, UpdateDataGridColumnsSize);
                        }
                    }                  
                }
            }
            for (int i = 0; i < rowExtend; i++)
            {
                RowDefinition rowdefinition = new RowDefinition();
                rowdefinition.Height = new GridLength(1, GridUnitType.Star);
                grid.RowDefinitions.Add(rowdefinition);

                if (i % 2 == 1)
                {
                    rowdefinition.Height = GridLength.Auto;
                    //GridSplitter splitter = CreateHSplitter();
                    //Grid.SetRow(splitter, i);
                  
                    //Grid.SetColumnSpan(splitter, columnExtend);
                    //grid.Children.Add(splitter);
                }
            }


            CreateHeadeExr(grid, rowCount, 0, 0, this.m_TNodes);
            return grid;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="rowCount"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <param name="tNodes"></param>
        private void CreateHeadeExr(Grid grid, int rowCount, int rowIndex, int columnIndex, List<TNode> tNodes)
        {
            for (int i = 0; i < tNodes.Count;i++)
            {
                TNode node = tNodes[i];

                #region 处理列信息
                int columnSpan = node.LeafCount * 2 - 1;
                #endregion

                #region 处理行信息
                int rowUnit = rowCount / node.Depth;//改树形分支剩余的行数
                int parentdepth = node.Level + node.Depth - 1;
                int rowSpan = rowUnit * 2 - 1; 
                #endregion

                TextBlock textBlock= CreateTextBlock(node.Name);
                Grid.SetColumn(textBlock, columnIndex);
                Grid.SetColumnSpan(textBlock, columnSpan);
                Grid.SetRow(textBlock, rowIndex);
                Grid.SetRowSpan(textBlock, rowSpan);
                grid.Children.Add(textBlock);
                if (!node.IsLeaf)
                {
                    int tempRowCount=rowCount - rowUnit;
                    int tempRowIndex = rowIndex + rowSpan;

                    GridSplitter hSplitter = CreateHSplitter();
                    Grid.SetColumn(hSplitter, columnIndex);
                    Grid.SetRow(hSplitter, tempRowIndex);
                    Grid.SetColumnSpan(hSplitter, columnSpan);
                    grid.Children.Add(hSplitter);

                    CreateHeadeExr(grid, tempRowCount, tempRowIndex + 1, columnIndex, node.Nodes);
                }
               
                #region 创建竖向分割符
                if (true||i != tNodes.Count - 1)
                {
                    GridSplitter splitter = CreateVSplitter();
                    Grid.SetColumn(splitter, columnIndex + columnSpan);
                    if (node.Level > 1 && i == tNodes.Count - 1)
                    {
                        Grid.SetRow(splitter, rowIndex-1);
                        Grid.SetRowSpan(splitter, rowSpan+1);
                    }
                    else
                    {
                        Grid.SetRow(splitter, rowIndex);
                        Grid.SetRowSpan(splitter, rowSpan);
                    }
                  
                   
                    grid.Children.Add(splitter);
                }
               
                
                #endregion
                columnIndex = columnIndex + columnSpan + 1;//深入不换列，平铺不换行
            }
        }

        public void UpdateDataGridColumnsSize(object sender,EventArgs args)
        {
            foreach (Tuple<ColumnDefinition,DataGridColumn,double> item in m_RelationColumns)
            {
                try
                {
                    item.Item2.Width = new DataGridLength(item.Item1.ActualWidth+item.Item3);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 创建textBlock标签
        /// </summary>
        /// <param name="text">textBlock显示内容</param>
        /// <returns></returns>
        private  TextBlock CreateTextBlock(string text)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            return textBlock;
        }
        /// <summary>
        /// 创建竖向分割条
        /// </summary>
        /// <returns></returns>
        private GridSplitter CreateVSplitter()
        {
            GridSplitter splitter = new GridSplitter();
            splitter.BorderThickness = new Thickness(m_SplitBorder);
            splitter.BorderBrush = Brushes.Gray;
            splitter.HorizontalAlignment = HorizontalAlignment.Center;
            splitter.VerticalAlignment = VerticalAlignment.Stretch;
            return splitter;
        }
        /// <summary>
        /// 创建横向分割条
        /// </summary>
        /// <returns></returns>
        private GridSplitter CreateHSplitter()
        {
            GridSplitter splitter = CreateVSplitter();
            splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
            splitter.VerticalAlignment =VerticalAlignment.Center;
            return splitter;
        }

        /// <summary>
        /// 获取node的实际宽度
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private double GetWidth(TNode node)
        {
            double width = 0;
            List<TNode> list = new List<TNode>() { node };
            for (int i = 0; i < list.Count; i++)
            {
                TNode tempNode = list[i];
                if (tempNode.IsLeaf)
                {
                    DataGridColumn column = tempNode.Tag as DataGridColumn;
                    if (column != null)
                        width += column.ActualWidth;
                }
                else
                {
                    list.AddRange(tempNode.Nodes);
                }
            }
            return width;
        }
        /// <summary>
        /// 获取node的实际宽度
        /// </summary>
        /// <param name="node"></param>
        /// <param name="cof">修正值</param>
        /// <returns></returns>
        private GridLength GetWidthEx(TNode node,double cof)
        {
            double width = GetWidth(node);
            return Math.Abs(width - 1) < 0.0001 ? new GridLength(1, GridUnitType.Star) : new GridLength(width+cof);

        }
    }
}
