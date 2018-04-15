
///////////////////////////////////////////////////////////////////////////////
//Copyright (c) 2016, 北京探索者软件公司
//All rights reserved.       
//文件名称: RevitSheetDraw.cs
//文件描述: 绘图逻辑
//创 建 者: xls
//创建日期: 2017-4-19
//版 本 号：1.0.0.0
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace FWindSoft.Revit.Table
{
    /// <summary>
    /// Revit绘图逻辑
    /// </summary>
    public class RevitSheetDraw:ITSheetContext
    {
        public RevitSheetDraw()
        {
            this.Location = XYZ.Zero;
        }

        /// <summary>
        /// 绘图数据源
        /// </summary>
        public List<object> Items { get; set; }
        /// <summary>
        /// 绘图视图
        /// </summary>
        public View DrawView { get; set; }

        /// <summary>
        /// 绘图起始点
        /// </summary>
        public XYZ Location { get; set; }
        public void Draw(TSheet sheet)
        {
            if (Items==null||Items.Count == 0||DrawView==null||Location==null)
                return;
            //获取列信息
            var columns = sheet.ColumnSettings.OrderBy(s => s.Index).Where(s => s.IsShow).ToList();
            int showColumnCount = columns.Count;
            if (showColumnCount == 0)
                return;

            View currentView = DrawView;
            int scale = DrawView.Scale;
            string fontString = sheet.FontString;
            bool isBottom = sheet.IsBottom;

            TextNoteType ntypeSheet = currentView.Document.GetTextNodeType(fontString, sheet.TitleWordSetting.WordHeight, 0.7);
            TextNoteType ntypeHead = currentView.Document.GetTextNodeType(fontString, sheet.SheetHeaderWordSetting.WordHeight, 0.7);
            TextNoteType ntypeContent = currentView.Document.GetTextNodeType(fontString, sheet.SheetRowWordSetting.WordHeight, 0.7);

            #region 分页
            List<List<object>> pages = new List<List<object>>();
            if (sheet.IsUsePagination && sheet.RowCountPrePage > 1)
            {
                int count = 0;
                List<object> pageElement = new List<object>();
                int groupCount = this.Items.Count;
                for (int i = 0; i < groupCount; i++)
                {
                    count++;

                    if (count < sheet.RowCountPrePage && i != groupCount - 1)
                    {
                        pageElement.Add(this.Items[i]);
                    }
                    else if (count == sheet.RowCountPrePage || i == groupCount - 1)//相等
                    {
                        pageElement.Add(this.Items[i]);
                        pages.Add(pageElement);
                        pageElement = new List<object>();
                        count = 0;
                    }
                }
            }
            else
            {
                pages.Add(this.Items);
            }
            #endregion
            #region 处理表头树
            var tuples = columns.Select(c => new Tuple<string, object>(c.Display.ToString(), c)).ToList();
            List<TNode> headerNotes = TreePathParse.Parse(tuples);
            int headerRows = headerNotes.Max(h => h.Depth);
            #endregion

            #region 写数据
            Document doc = currentView.Document;
            Dictionary<int, double> columnsWidth = null;
            bool isAutoFit = sheet.AutoFit;
            int refCount = TSheet.GetRowCount(this.Items[0]);//一个元素对应多少行revit表格
            List<RevitTalbe> tables = new List<RevitTalbe>();
            //每一页重新编号
            //int num = 0;
            foreach (var page in pages)
            {
                //num =1;
                if (isAutoFit)
                {
                    columnsWidth = new Dictionary<int, double>();
                    for (int i = 0; i < showColumnCount; i++)
                    {
                        columnsWidth[i] = 0;
                    }
                }
                int currentRowCount = page.Count * refCount + headerRows;
                if (sheet.IsShowTitleName)
                    currentRowCount++;
                RevitTalbe table = new RevitTalbe(currentRowCount, showColumnCount);
                int currentRowIndex = 0;
                Func<int> currentRowIndexMove = () => currentRowIndex++;
                if (isBottom)
                {
                    currentRowIndex = currentRowCount - 1;
                    currentRowIndexMove = () => currentRowIndex--;
                }

                #region 表格维护

                {
                    if (sheet.IsShowTitleName)
                    {
                        var data = new TextNoteData(currentView, XYZ.Zero, XYZ.BasisX, XYZ.BasisY, TextAlignFlags.TEF_ALIGN_CENTER, sheet.TitleName, ntypeSheet);
                        TextNote tn = data.Create(currentView.Document);
                        table.SetValueMerge(currentRowIndex, 0, currentRowIndex, showColumnCount - 1, tn);
                        table.Rows[currentRowIndex].Height = sheet.TitleWordSetting.RowHeight * scale;
                        currentRowIndexMove(); 
                    }
                }

                #endregion

                #region 表头处理

                {
                    List<TCell> cells = GetHeaderCells(ntypeHead, headerRows, headerNotes, 0, 0);
                    int k = 0;
                    foreach (TCell cell in cells)
                    {
                        table.SetValueMerge(cell.RowIndex,cell.ColumnIndex, cell.RowIndex+cell.RowSpan-1,cell.ColumnIndex+cell.ColumnSpan-1,cell.Value);
                        if (isAutoFit && cell.ColumnSpan == 1)
                        {
                            double tempWidth =Convert.ToDouble(cell.Tag);
                            columnsWidth[k] = columnsWidth[k].IsThanEq(tempWidth) ? columnsWidth[k] : tempWidth;
                            k++;
                        }
                    }
                    for (int i = 0; i < headerRows; i++)
                    {
                        table.Rows[currentRowIndex].Height = sheet.SheetHeaderWordSetting.RowHeight * scale;
                        currentRowIndexMove();
                    }
                   
                }
                #endregion

                #region 表格处理

                {
                    List<Dictionary<string, object>> tempPage = TSheet.GetSheetData(page);
                    for (int no = 0; no < tempPage.Count; no++)
                    {
                        var item = tempPage[no];
                        #region 自动编号
                        if (sheet.AutoBuildNo && !string.IsNullOrEmpty(sheet.NoColumnName) && no % refCount == 0)
                        {
                            item[sheet.NoColumnName] = no / refCount + 1;
                        } 
                        #endregion
                        for (int i = 0; i < showColumnCount; i++)
                        {
                            //图例暂时不处理
                            object tempValue = item.GetValue(columns[i].Name);
                           
                            string text = tempValue==null?"":item.GetValue(columns[i].Name).ToString();
                            var data = new TextNoteData(currentView, XYZ.Zero, XYZ.BasisX, XYZ.BasisY, BaseDataSupply.ConvertToTextNodeAlgin(columns[i].TextAlignment), text, ntypeContent);
                            TextNote tn = data.Create(currentView.Document);
                           
                            table.SetValue(currentRowIndex, i, tn);
                            if (isAutoFit)
                            {
                                double tempWidth = doc.ComputeTextNoteWidth(currentView, text, ntypeContent).FromApi() * 0.7;
                                columnsWidth[i] = columnsWidth[i].IsThanEq(tempWidth) ? columnsWidth[i] : tempWidth;
                            }
                        }
                        table.Rows[currentRowIndex].Height = sheet.SheetRowWordSetting.RowHeight * scale;
                        currentRowIndexMove();

                    }
                }
                #endregion
                #region 样式设置

                if (isAutoFit)
                {
                    //自适应列宽
                    foreach (var d in columnsWidth)
                    {
                        table.Columns[d.Key].Width = d.Value * scale;
                    }

                }
                else
                {
                    for (int i = 0; i < Math.Min(table.Columns.Count, columns.Count); i++)
                    {
                        table.Columns[i].Width = columns[i].ColomnWidth * scale;
                    }

                }
                tables.Add(table);
                #endregion

            }
            #endregion

            #region 放置表格
            List<XYZ> locations = new List<XYZ>();
            double sapce = 2000d;// 2000d.ToApi();
            XYZ baseLocation = Location;
            XYZ currentLocation = baseLocation.AddX(0);
            for (int i = 0; i < tables.Count; i++)
            {

                if (sheet.Arrange == ContentArragne.Landscape)
                {
                    if (i != 0)
                    {
                        double tempWidth = tables[i - 1].Columns.Sum(c => c.Width);
                        tempWidth = tempWidth + sapce;
                        locations.Add(currentLocation = currentLocation.AddX(tempWidth.ToApi()));
                    }
                    else
                    {
                        locations.Add(currentLocation = currentLocation.AddX(0));
                    }

                }
                else
                {
                    if (i != 0)
                    {
                        double tempHeight = tables[i - 1].Rows.Sum(r => r.Height);
                        tempHeight = tempHeight + sapce;
                        locations.Add(currentLocation = currentLocation.OffsetPoint(-XYZ.BasisY, tempHeight.ToApi()));
                    }
                    else
                    {
                        //double tempHeight = tables[i].Rows.Sum(r => r.Height);
                        //tempHeight = tempHeight;
                        //locations.Add(currentLocation=currentLocation.OffsetPoint(-XYZ.BasisY, tempHeight.ToApi()));
                        locations.Add(currentLocation = currentLocation.AddY(0));
                    }
                }
            }
            for (int i = 0; i < tables.Count; i++)
            {
                var revitTalbe = tables[i];
                revitTalbe.DrawTable(locations[i], currentView);
            }
            #endregion

        }

        #region 可合并表头创建逻辑

        /// <summary>
        /// 整合表头信息
        /// </summary>
        /// <param name="noteType">贯穿信息（递归过程中国年保持不变）</param>
        /// <param name="rowCount">剩余的行数</param>
        /// <param name="nodes"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private List<TCell> GetHeaderCells(TextNoteType noteType, int rowCount, List<TNode> nodes, int rowIndex,
            int columnIndex)
        {
            List<TCell> cells = new List<TCell>();
            View currentView = this.DrawView;
            Document doc = currentView.Document;
            int currentColumn = columnIndex;
            int currentRow = rowIndex;
            for (int k = 0; k < nodes.Count; k++)
            {
                TNode currentNode = nodes[k];
                int columnSpan = currentNode.LeafCount;

                int rowUnit = rowCount/currentNode.Depth; //改树形分支剩余的行数
                int rowSpan = rowUnit;

                string text = currentNode.Name;
                var leaves=currentNode.GetLeaves();
                ColumnSetting column = leaves[0].Tag as ColumnSetting;
                if (column != null)
                {
                    var data = new TextNoteData(currentView, XYZ.Zero, XYZ.BasisX, XYZ.BasisY,
                        BaseDataSupply.ConvertToTextNodeAlgin(column.TextAlignment), text, noteType);
                    TextNote tn = data.Create(currentView.Document);
                    TCell tCell = null;
                    cells.Add(tCell = new TCell()
                    {
                        RowIndex = currentRow,
                        ColumnIndex = currentColumn,
                        RowSpan = rowSpan,
                        ColumnSpan = columnSpan,
                        Value = tn
                    });

                    //if (isAutoFit)
                    //{
                    double tempWidth = doc.ComputeTextNoteWidth(currentView, text, noteType).FromApi() * 0.7;
                    tCell.Tag = tempWidth;
                    //    columnsWidth[k] = columnsWidth[k].IsThanEq(tempWidth) ? columnsWidth[k] : tempWidth;
                    //}
                }

                if (!currentNode.IsLeaf)
                {

                    cells.AddRange(GetHeaderCells(noteType, rowCount - rowSpan, currentNode.Nodes, currentRow + rowSpan,
                        currentColumn));
                }
                currentColumn = currentColumn + columnSpan;
            }
            return cells;
        }

        #endregion
    }
}
