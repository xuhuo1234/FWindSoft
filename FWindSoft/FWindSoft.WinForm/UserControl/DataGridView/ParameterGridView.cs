using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace FWindSoft.WinForm
{
    /// <summary>
    /// 参数显示框
    /// </summary>
    public class ParameterGridView : DataGridView
    {
        private const int m_RowHeight=24;

        public ParameterGridView()
        {
            if (IsDesignMode())
                return;
            DataGridViewTextBoxColumn columnName = new DataGridViewTextBoxColumn();
            columnName.HeaderText = "名称";
            columnName.MinimumWidth = 40;
            columnName.Name = "ParamName";
            columnName.ReadOnly = true;
            columnName.FillWeight = 0.6f;
            this.Columns.Add(columnName);
            DataGridViewTextBoxColumn columnValue = new DataGridViewTextBoxColumn();
            columnValue.HeaderText = "值";
            columnValue.MinimumWidth = 60;
            columnValue.Name = "ParamValue";
            columnValue.DataPropertyName = "X";
            columnValue.ReadOnly = false;
            columnValue.FillWeight = 1;
            this.Columns.Add(columnValue);

            this.AutoGenerateColumns = false;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;

            this.AllowUserToResizeRows = false;
            this.MultiSelect = false;
            this.BackgroundColor = SystemColors.Window;
            this.BorderStyle=BorderStyle.None;
            this.EditMode = DataGridViewEditMode.EditOnEnter;
            this.RowHeadersVisible = false;
            this.ColumnHeadersVisible = false;
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.RowHeadersVisible = false;
            this.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.RowPostPaint += GridPointShow_RowPostPaint;
            this.DefaultCellStyle.SelectionBackColor = Color.White;
            this.DefaultCellStyle.SelectionForeColor = Color.Black;
            this.MouseLeave += ParameterGridView_MouseLeave;
        }
        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);
            #region 控制灰显的不能被选中
            try
            {
                if (this.CurrentCell != null)
                {
                    var row = this.Rows[this.CurrentCell.RowIndex];
                    ParameterItem item = row.Tag as ParameterItem;

                    if (item != null && !item.IsEnable)
                    {
                        this.CurrentCell = null;
                    }
                }
            }
            catch (Exception)
            {
            }
            #endregion
        }

        void ParameterGridView_MouseLeave(object sender, EventArgs e)
        {
            this.CurrentCell = null;
            this.EndEdit();
        }

        protected override void OnCurrentCellDirtyStateChanged(EventArgs e)
        {
            base.OnCurrentCellDirtyStateChanged(e);
            if (this.CurrentCell is DataGridViewCheckBoxCell)
            {
                this.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
          
        }

        protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
        {
            base.OnCellValueChanged(e);
            var cellEdit = this.Rows[e.RowIndex].Cells[e.ColumnIndex];
            ParameterItem item = cellEdit.Tag as ParameterItem;
            if (item != null)
            {
                item.Value = cellEdit.Value;
                UpdateEditor();
            }
        }

        public static bool IsDesignMode()
        {
            bool flag = false;
#if DEBUG
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                flag = true;
            }
            else if (Process.GetCurrentProcess().ProcessName.Equals("devenv"))
            {
                flag = true;
            }
#endif

            return flag;
        }

        private void GridPointShow_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //DataGridView view = sender as DataGridView;

            //if (view != null)
            //{
            //    Rectangle rec = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, view.RowHeadersWidth - 4, e.RowBounds.Height);
            //    TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), view.RowHeadersDefaultCellStyle.Font, rec, view.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
            //}
        }

        protected override void OnRowEnter(DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            base.OnRowEnter(e);
        }

        #region 选择属性

        private ParameterCollection m_Parameters;
        /// <summary>
        /// 关联Parameters
        /// </summary>
        public ParameterCollection Parameters
        {
            get { return this.m_Parameters; }
            set
            {
                this.m_Parameters = value;
                RefreshShow();
            }
        }
        /// <summary>
        /// 涉及到显示相关时调用【如可见性，可用性更改】
        /// </summary>
        public void RefreshShow()
        {
            //清除所有列
            this.Rows.Clear();
            ParameterCollection collection = this.m_Parameters;
            if (collection == null)
                return;
            foreach (var parameterItem in collection)
            {
                int tempIndex = this.Rows.Add();
                this.Rows[tempIndex].Height = m_RowHeight;
                object tempValue = parameterItem.Value;
                if (tempValue is bool)
                {
                    SetEditor(this.Rows[tempIndex],parameterItem,new DataGridViewCheckBoxCell());
                }
                else if (tempValue is Enum)
                {
                }
                else
                {
                    var tempControl = parameterItem.ShowControl;
                    if (tempControl != null&&tempControl.IsDropDown)
                    {
                        TSZComboBoxCellExtend cellControl = new TSZComboBoxCellExtend();
                        cellControl.Numeric = tempControl.IsNumber;
                        cellControl.Items.AddRange(tempControl.Items??new List<object>());
                        SetEditor(this.Rows[tempIndex], parameterItem, cellControl);
                    }
                    else
                    {
                        #region 没有设置按默认的显示
                        SetEditor(this.Rows[tempIndex], parameterItem, new DataGridViewTextBoxCell());
                        #endregion
                    }
                }
            }
        }

        private void SetEditor(DataGridViewRow row,ParameterItem parameter,DataGridViewCell secondCell)
        {
            row.Cells[1] = secondCell;
            secondCell.Tag = parameter;
            row.Tag = parameter;
            row.Visible = parameter.IsVisible;
            if (!parameter.IsEnable)
            {
                row.Cells[0].Style.ForeColor = Color.DarkGray;
                row.Cells[1].Style.ForeColor = Color.DarkGray;
                row.ReadOnly = true;
            }
           
            row.Cells[0].Value = parameter.DisplayName;            
            row.Cells[1].Value = parameter.Value;
        }

        private void UpdateUditor(DataGridViewRow row,ParameterItem parameter)
        {
            if (parameter == null)
                return;
            TSZComboBoxCellExtend extend = row.Cells[1] as TSZComboBoxCellExtend;
            if (extend != null)
            {
                var tempControl = parameter.ShowControl;
                if (tempControl != null && tempControl.IsDropDown)
                {
                    extend.Numeric = tempControl.IsNumber;
                    extend.Items.AddRange(tempControl.Items ?? new List<object>());
                }
            }

            row.Tag = parameter;
            row.Visible = parameter.IsVisible;
            if (!parameter.IsEnable)
            {
                row.Cells[0].Style.ForeColor = Color.DarkGray;
                row.Cells[1].Style.ForeColor = Color.DarkGray;
                row.ReadOnly = true;
            }
            else
            {
                row.Cells[0].Style.ForeColor = Color.Black;
                row.Cells[1].Style.ForeColor = Color.Black;
                row.ReadOnly = false;
            }

            row.Cells[0].Value = parameter.DisplayName;
            row.Cells[1].Value = parameter.Value;
        }

        public void UpdateEditor()
        {
            foreach (DataGridViewRow row in this.Rows)
            {
                UpdateUditor(row,row.Tag as ParameterItem);
            }
        }

        #endregion

        /// <summary>
        /// 设置参数列和值列的比例
        /// </summary>
        /// <param name="widths"></param>
        public void SetColumnsWidth(List<double> widths)
        {
            if (widths == null || widths.Count < 2)
            {
                return;
            }
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            for (int i = 0; i < widths.Count; i++)
            {
                this.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.Columns[i].FillWeight = (float)widths[i];
               
            }
        }
        /// <summary>
        /// 设置标签列的列宽
        /// </summary>
        /// <param name="width"></param>
        public void SeLableColumnWidth(double width)
        {
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            this.Columns[0].FillWeight = 1;
            this.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.Columns[0].Width = Width;
        }
    }
}
