
using System;
using System.Collections;
using System.Windows.Forms;

namespace FWindSoft.WinForm
{
    //定制该扩展列的单元格 
    public class TSZComboBoxCellExtend : DataGridViewTextBoxCell
    {
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            DataGridViewComboBoxEditingControlExtend clt = this.DataGridView.EditingControl as DataGridViewComboBoxEditingControlExtend;

            DataGridViewComboBoxColumnExtend col =this.OwningColumn as DataGridViewComboBoxColumnExtend;
            if (col != null)
            {
                #region 存在列兼容
                clt.DataSource = this.DataSource ?? col.DataSource;
                if (clt.DataSource == null)
                {
                    clt.Items.Clear();
                    ArrayList listItems = this.m_Items ?? col.Items;
                    foreach (object o in listItems)
                    {
                        clt.Items.Add(o);
                    }
                }
                clt.DisplayMember = this.DisplayMember ?? col.DisplayMember;
                clt.ValueMember = this.ValueMember ?? col.ValueMember;
                if (this.AutoComplete || col.AutoComplete)
                {
                    clt.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                }
                else
                {
                    clt.AutoCompleteMode = AutoCompleteMode.None;
                }
                clt.SelectedIndex = -1;
                clt.Numeric = this.Numeric.HasValue ?
                    this.Numeric.Value :
                    (col.Numeric.HasValue ? col.Numeric.Value : false);
                clt.Text = Convert.ToString(this.Value); 
                #endregion
            }
            else
            {
                clt.DataSource = this.DataSource;
                if (clt.DataSource == null)
                {
                    clt.Items.Clear();
                    ArrayList listItems = this.m_Items;
                    foreach (object o in listItems)
                    {
                        clt.Items.Add(o);
                    }
                }
                clt.DisplayMember = this.DisplayMember;
                clt.ValueMember = this.ValueMember;
                if (this.AutoComplete)
                {
                    clt.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                }
                else
                {
                    clt.AutoCompleteMode = AutoCompleteMode.None;
                }
                clt.SelectedIndex = -1;
                clt.Numeric = this.Numeric.HasValue;
                clt.Text = Convert.ToString(this.Value); 
            }
            
        }

        public override Type EditType
        {
            get { return typeof(DataGridViewComboBoxEditingControlExtend); }
        }

        public override Type ValueType
        {
            get { return typeof(string); }
        }

        public override object DefaultNewRowValue
        {
            get { return string.Empty; }
        }

        private object dataSoruce = null;
        public object DataSource
        {
            get { return dataSoruce; }
            set { dataSoruce = value; }
        }

        private string valueMember;
        public string ValueMember
        {
            get { return valueMember; }
            set { valueMember = value; }
        }

        private string displayMember;
        public string DisplayMember
        {
            get { return displayMember; }
            set { displayMember = value; }
        }

        private bool m_AutoComplete;
        public bool AutoComplete
        {
            get { return m_AutoComplete; }
            set { m_AutoComplete = value; }
        }

        private bool? m_Numeric;
        public bool? Numeric
        {
            get { return m_Numeric; }
            set { m_Numeric = value; }
        }

        private ArrayList m_Items;
        public ArrayList Items
        {
            get
            {
                if (m_Items == null)
                {
                    m_Items = new ArrayList();
                }
                return m_Items;
            }
        }
    }
}
