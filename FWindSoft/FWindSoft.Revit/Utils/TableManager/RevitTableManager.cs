
///////////////////////////////////////////////////////////////////////////////
//Copyright (c) 2015, 北京探索者软件公司
//All rights reserved.       
//文件名称: RevitTableManager.cs
//文件描述: Revit表格管理
//创 建 者: xls
//创建日期: 2016-10-13
//版 本 号：1.0.0.0
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace FWindSoft.Revit.Table
{
    public class RevitTableManager
    {
    }
    #region 基础信息维护
    /// <summary>
    /// Revit表格
    /// </summary>
    public class RevitTalbe
    {
        private RevitCell[,] m_Cells;
        private List<RevitColumn> m_Columns;
        private List<RevitRow> m_Rows; 
        public RevitTalbe(int rowCount,int columnCount)
        {
           #region 初始化表格 column,row
            m_Columns=new List<RevitColumn>();
            m_Rows=new List<RevitRow>();
		    for (int i = 0; i < rowCount; i++)
            {
                m_Rows.Add(new RevitRow());
            }
            for (int i = 0; i < columnCount; i++)
            {
                m_Columns.Add(new RevitColumn());
            } 
	       #endregion
           #region 初始化表格 cell
            m_Cells = new RevitCell[rowCount, columnCount];
            double tempTop = 0;
            for (int i = 0; i < rowCount; i++)
            {
                if (i != 0)
                {
                    tempTop = tempTop + m_Rows[i - 1].Height;
                }
                double tempLeft = 0;
                for (int j = 0; j < columnCount; j++)
                {
                    if (j != 0)
                    {
                        tempLeft = tempLeft + m_Columns[j - 1].Width;
                    }
                    m_Cells[i, j] = new RevitCell(this) { BaseColumnIndex = j, BaseRowIndex = i, Top = tempTop, Left = tempTop };
                }
            } 
            #endregion
        }

        #region 属性维护
        /// <summary>
        /// 列集合
        /// </summary>
        public List<RevitColumn> Columns { get { return m_Columns; } }
        /// <summary>
        /// 行集合
        /// </summary>
        public List<RevitRow> Rows { get { return m_Rows; } }  
        #endregion

        #region 公开方法维护

        public void SetValue(int row, int column,object value)
        {
            m_Cells[row, column].Value = value;
        }
        public void SetValueMerge(int row1, int column1, int row2, int column2, object value)
        {
            int baseRowIndex = row1 <= row2 ? row1 : row2;
            int baseColumnIndex = column1 <= column2 ? column1 : column2;
           
            int rowRange = Math.Abs(row1 - row2);
            int columnRange = Math.Abs(column1 - column2);
            //暂时不修改成同一个引用对象
            for (int i = 0; i < rowRange+1; i++)
            {
                for (int j = 0; j < columnRange+1; j++)
                {
                     var tempValue = m_Cells[baseRowIndex+i, baseColumnIndex+j];
                     tempValue.BaseRowIndex = baseRowIndex;
                     tempValue.BaseColumnIndex = baseColumnIndex;
                     tempValue.Value = value;
                     tempValue.ExtendRowRange = rowRange;
                     tempValue.ExtendColumnRange = columnRange;
                }
            }
          


        }
        #endregion
        #region 私有方法，重新计算表格偏移量

        public void CalcCellsOFfset()
        {
            int rowCount = Rows.Count;
            int columnCount = Columns.Count;
            double tempTop = 0;
            for (int i = 0; i < rowCount; i++)
            {
                if (i != 0)
                {
                    tempTop = tempTop + m_Rows[i - 1].Height;
                }
                double tempLeft = 0;
                for (int j = 0; j < columnCount; j++)
                {
                    if (j != 0)
                    {
                        tempLeft = tempLeft + m_Columns[j - 1].Width;
                    }
                    var tempCell = m_Cells[i, j];
                    if (tempCell != null)
                    {
                        tempCell.Top = tempTop;
                        tempCell.Left = tempLeft;
                    }
                }
            } 
        }

        #endregion


        #region 绘制表格
        /// <summary>
        /// 表格所处位置
        /// </summary>
        public XYZ Location { get; set; }
        public void DrawTable(XYZ origion,View view)
        {
            CalcCellsOFfset();
            Location = origion;
            int rowCount = Rows.Count;
            int columnCount = Columns.Count;
            #region 表格线框

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    var tempCell = m_Cells[i, j];
                    if (tempCell == null)
                        continue;
                    if (tempCell.BaseRowIndex == i && tempCell.BaseColumnIndex == j)
                    {
                        var curves=tempCell.GetBorders();
                        curves[0].NewDetailCurveExt(view);
                        curves[1].NewDetailCurveExt(view);
                        if ((tempCell.BaseColumnIndex + tempCell.ExtendColumnRange) == columnCount - 1)
                        {
                            curves[2].NewDetailCurveExt(view);
                        }
                        if ((tempCell.BaseRowIndex+tempCell.ExtendRowRange) == rowCount - 1)
                        {
                            curves[3].NewDetailCurveExt(view);
                        }                     
                        tempCell.DrawValue();
                    }
                }
            }
            #endregion
        }

        #endregion
    }

    /// <summary>
    /// 表格列维护
    /// </summary>
    public class RevitColumn
    {
        public RevitColumn()
        {
            Width = 2000;
        }

        public RevitColumn(double width)
        {
            Width = width;
        }

        /// <summary>
        /// 列的宽度
        /// </summary>
        public double Width { get; set; }
    }
    /// <summary>
    /// 表格行维护
    /// </summary>
    public class RevitRow
    {
        public RevitRow()
        {
            Height = 2000;
        }

        public RevitRow(double height)
        {
            Height = height;
        }

        /// <summary>
        /// 列的高度
        /// </summary>
        public double Height { get; set; }
    }

    public class RevitCell
    {
        private RevitTalbe m_ReferenceTable;

        public RevitCell(RevitTalbe revitTalbe)
        {
            m_ReferenceTable = revitTalbe;
        }

        /// <summary>
        /// 相对于表格定位点的位置  上
        /// </summary>
        public double Top { get; set; }
        /// <summary>
        /// 相对于表格定位点的位置 左
        /// </summary>
        public double Left { get; set; }

        /// <summary>
        /// 当前cell的宽度
        /// </summary>
        public double Width
        {
            get
            {
                double tempWidth = 0;
                try
                {
                    for (int i = 0; i < ExtendColumnRange + 1; i++)
                    {
                        tempWidth += m_ReferenceTable.Columns[BaseColumnIndex + i].Width;
                    }
                }
                catch
                {
                    // ignored
                }
                return tempWidth;
            }
        }
        /// <summary>
        /// 当前cell的高度
        /// </summary>
        public double Height
        {
            get
            {
                double tempHeight = 0;
                try
                {
                    for (int i = 0; i < ExtendRowRange + 1; i++)
                    {
                        tempHeight += m_ReferenceTable.Rows[BaseRowIndex + i].Height;
                    }
                }
                catch
                {
                    // ignored
                }
                return tempHeight;
            }
        }
        /// <summary>
        /// 基准表格行位置
        /// </summary>
        public int BaseRowIndex { get; set; }
        /// <summary>
        /// 基准表格列位置
        /// </summary>
        public int BaseColumnIndex { get; set; }

        /// <summary>
        /// 向下合并行的数量
        /// </summary>
        public int ExtendRowRange { get; set; }
        /// <summary>
        /// 向右合并列的数量
        /// </summary>
        public int ExtendColumnRange { get; set; }
        /// <summary>
        /// 单元格的值
        /// </summary>
        public object Value { get; set; }
        #region 公开方法

        public XYZ GetLocation()
        {
            XYZ tempOrigion = m_ReferenceTable.Location??XYZ.Zero;
            return new XYZ(tempOrigion.X+Left.ToApi(),tempOrigion.Y-Top.ToApi(),tempOrigion.Z);
        }
        /// <summary>
        /// 获取cell Border,顺序左，上，右，下
        /// </summary>
        /// <returns></returns>
        public List<Curve> GetBorders()
        {
            List<Curve> curves=new List<Curve>();
            XYZ tempBase = GetLocation();
            XYZ leftBottom = tempBase.OffsetPoint(-XYZ.BasisY, Height.ToApi());
            XYZ rightTop = tempBase.OffsetPoint(XYZ.BasisX, Width.ToApi());
            XYZ rightBottom = rightTop.OffsetPoint(-XYZ.BasisY, Height.ToApi());
            curves.Add(tempBase.NewLine(leftBottom));
            curves.Add(tempBase.NewLine(rightTop));
            curves.Add(rightTop.NewLine(rightBottom));
            curves.Add(leftBottom.NewLine(rightBottom));
            return curves;
        }

        public void DrawValue()
        {
            TextNote textNote = Value as TextNote;
            XYZ tempOrigion = GetLocation();
            if (textNote != null)
            {
               View tempView= ExternalDataWrapper.Current.Doc.GetElement(textNote.OwnerViewId) as View;
                if (tempView == null)
                {
                    return;
                }
                
                 #region 处理标签宽度
		         double scale = tempView.Scale;
                 textNote.Width = Width.ToApi() / scale; 
                 #endregion
                
                //标签宽度，与表格同宽
                 #region 处理水平对齐偏移
                 double tempOffsetWidth = Width.ToApi() / 2;
                 int hashcode = textNote.GetParameterInteger(BuiltInParameter.TEXT_ALIGN_HORZ);
                 if (hashcode == TextAlignFlags.TEF_ALIGN_LEFT.GetHashCode())
                 {
                     tempOffsetWidth = 0;
                 }
                 else if (hashcode == TextAlignFlags.TEF_ALIGN_RIGHT.GetHashCode())
                 {
                     tempOffsetWidth = Width.ToApi();
                 }
                 XYZ result = tempOrigion.OffsetPoint(XYZ.BasisX, tempOffsetWidth); 
                 #endregion
                BoundingBoxXYZ bb = textNote.get_BoundingBox(tempView);
                if (bb != null)
                {
                    double textHeight = bb.Max.Y - bb.Min.Y;
                    double offset = (Height.ToApi() - textHeight) / 2;
                    result = result.OffsetPoint(-XYZ.BasisY, offset);
                }
                textNote.Document.MoveElementExt(textNote.Id, result.Subtract(textNote.Coord));
            }
        }

        #endregion
    }

    #endregion
}
