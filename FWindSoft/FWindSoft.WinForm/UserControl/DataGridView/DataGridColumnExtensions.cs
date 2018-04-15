using System;
using System.Collections;
using System.Windows.Forms;

namespace FWindSoft.WinForm
{
    public class DataGridViewComboBoxEditingControlExtend : ComboBox, IDataGridViewEditingControl
    {
        protected int rowIndex;
        protected DataGridView dataGridView;
        protected bool valueChanged = false;
        public DataGridViewComboBoxEditingControlExtend()
            : base()
        { }
        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.Escape:
                case Keys.Enter:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return false;
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (this.Numeric.HasValue && this.Numeric.Value)
            {               
                TextControl textControl = new TextControl(this);
                if (textControl.ValidateNumeric(e))
                    return;
            }
            base.OnKeyPress(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            NotifyDataGridViewOfValueChange();
        }

        private void NotifyDataGridViewOfValueChange()
        {
            valueChanged = true;
            dataGridView.NotifyCurrentCellDirty(true);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            NotifyDataGridViewOfValueChange();
        }

        public Cursor EditingPanelCursor
        {
            get { return Cursors.IBeam; }
        }

        public DataGridView EditingControlDataGridView
        {
            get { return dataGridView; }
            set { dataGridView = value; }
        }

        public object EditingControlFormattedValue
        {
            get { return this.Text; }
            set
            {
                this.Text = value.ToString();
                NotifyDataGridViewOfValueChange();
            }
        }

        public virtual object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return this.Text;
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            if (selectAll)
            {
                this.SelectAll();
            }
            else
            {
                this.SelectionStart = this.ToString().Length;
            }
        }

        public virtual bool RepositionEditingControlOnValueChange
        {
            get { return false; }
        }

        public int EditingControlRowIndex
        {
            get { return this.rowIndex; }
            set { this.rowIndex = value; }
        }

        private bool? m_Numeric;
        public bool? Numeric
        {
            get { return m_Numeric; }
            set { m_Numeric = value; }
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.BackColor = dataGridViewCellStyle.BackColor;
        }

        public bool EditingControlValueChanged
        {
            get { return valueChanged; }
            set { this.valueChanged = value; }
        }
    }
    //定制该扩展列的单元格 
    public class DataGridViewComboBoxCellExtend : DataGridViewTextBoxCell
    {
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            DataGridViewComboBoxEditingControlExtend clt = this.DataGridView.EditingControl as DataGridViewComboBoxEditingControlExtend;
            DataGridViewComboBoxColumnExtend col = (DataGridViewComboBoxColumnExtend)this.OwningColumn;
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
    //定制该扩展列 
    public class DataGridViewComboBoxColumnExtend : DataGridViewColumn
    {
        public DataGridViewComboBoxColumnExtend()
            : base(new DataGridViewComboBoxCellExtend())
        { }
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

        public override DataGridViewCell CellTemplate
        {
            get { return base.CellTemplate; }
            set
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewComboBoxCellExtend)))
                {
                    throw new InvalidCastException();
                }
                base.CellTemplate = value;
            }
        }

        private DataGridViewComboBoxCellExtend ComboBoxCellTemplate
        {
            get { return (DataGridViewComboBoxCellExtend)this.CellTemplate; }
        }
    }
    
    public class DataGridViewDateTimeEditingControl : DateTimePicker, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private bool valueChanged = false;
        private int rowIndex;

        public DataGridViewDateTimeEditingControl()
        {
            this.Format = DateTimePickerFormat.Short;
        }

        public object EditingControlFormattedValue
        {
            get
            {
                return this.Value.ToShortDateString();
            }
            set
            {
                if (value is String)
                {
                    this.Value = DateTime.Parse((String)value);
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
            this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }

        public int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }

        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }

        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }

        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }

        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        protected override void OnValueChanged(EventArgs eventargs)
        {
            valueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }
    }

    public class DataGridViewDateTimeCell : DataGridViewTextBoxCell
    {
        public DataGridViewDateTimeCell()
            : base()
        {
            this.Style.Format = "yyyy-MM-dd";
        }

        public override void InitializeEditingControl(int rowIndex,
            object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            DataGridViewDateTimeEditingControl ctl = DataGridView.EditingControl as DataGridViewDateTimeEditingControl;
            if (this.Value == null || this.Value.ToString().Trim().Length <= 0)
                ctl.Value = DateTime.Now;
            else
                ctl.Value = Convert.ToDateTime(this.Value);
        }

        public override Type EditType
        {
            get
            {
                return typeof(DataGridViewDateTimeEditingControl);
            }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(DateTime);
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                return string.Empty;
            }
        }
    }

    public class DataGridViewDateTimeColumn : DataGridViewColumn
    {
        public DataGridViewDateTimeColumn()
            : base(new DataGridViewDateTimeCell())
        { }
        public override DataGridViewCell CellTemplate
        {
            get { return base.CellTemplate; }
            set
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewDateTimeCell)))
                {
                    throw new InvalidCastException();
                }
                base.CellTemplate = value;
            }
        }
    }
}
