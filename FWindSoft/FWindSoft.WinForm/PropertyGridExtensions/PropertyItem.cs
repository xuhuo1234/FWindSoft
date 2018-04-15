using System;
using System.ComponentModel;

namespace FWindSoft.WinForm
{
    public class PropertyItem
    {
        private object m_Value;
        public event EventHandler<ValueChangedArgs> OnValueChanged;
        public PropertyItem()
        {
            this.Name = string.Empty;
            this.DisplayName = String.Empty;
            this.Category = string.Empty;
            this.IsVisible = true;
            this.IsReadOnly = false;
        }

        public PropertyItem(string name, object value)
            : this()
        {
            this.Name = name;
            this.Value = value;
            this.DisplayName = name;
        }

        public PropertyCollection Owner { get; internal set; }
        #region 特殊处理属性值
        /// <summary>
        /// 属性值
        /// </summary>
        public object Value
        {
            get { return this.m_Value; }
            set
            {
                ValueChangedArgs args = new ValueChangedArgs(this.m_Value, value, this.Owner);
                bool changed = this.m_Value!=value;
                this.m_Value = value;
                if (changed && OnValueChanged != null)
                {
                    OnValueChanged(this, args);
                }
            }
        } 
        #endregion
        #region 属性维护
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }
      
        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool IsReadOnly { get; set; }
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 编辑器
        /// </summary>
        public virtual object Editor { get; set; }
        /// <summary>
        /// 属性名显示
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 类型转换
        /// </summary>
        public  TypeConverter Convert { get; set; }
        #endregion
    }

    public class ValueChangedArgs : HandledEventArgs
    {
        public ValueChangedArgs(object oldValue,object newValue,PropertyCollection collection)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.ReferenceItems = collection;
        }

        /// <summary>
        /// 旧值
        /// </summary>
        public object OldValue { get; private set; }
        /// <summary>
        /// 新值
        /// </summary>
        public object NewValue { get; private set; }
        /// <summary>
        /// 关联项目集合
        /// </summary>
        public PropertyCollection ReferenceItems { get; private set; }
    }
}
