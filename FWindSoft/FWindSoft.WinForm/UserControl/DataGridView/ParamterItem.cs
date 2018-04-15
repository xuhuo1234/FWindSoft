
///////////////////////////////////////////////////////////////////////////////
//Copyright (c) 2016, 北京探索者软件公司
//All rights reserved.       
//文件名称: ParamterItem.cs
//文件描述: 参数项目
//创 建 者: xls
//创建日期: 2016-11-23
//版 本 号：1.0.0.0
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FWindSoft.WinForm
{
    /// <summary>
    /// 参数集合
    /// </summary>
    public class ParameterCollection:ICollection<ParameterItem>
    {
        private List<ParameterItem> m_List;
        public ParameterCollection()
            : base()
        {
            m_List = new List<ParameterItem>();
        }
        /// <summary>
        /// 增加项目
        /// </summary>
        /// <param name="property"></param>
        public void Add(ParameterItem property)
        {
            if (property == null) return;

            property.Owner = this;
            bool exist = false;
            for (int i = 0; i < m_List.Count; i++)
            {
                ParameterItem item = m_List[i] as ParameterItem;
                if (item != null && item.Name.Equals(property.Name))
                {
                    exist = true;
                    m_List[i] = property;
                }
            }
            if (!exist)
            {
                m_List.Add(property);
            }
        }
        /// <summary>
        /// 移除项目
        /// </summary>
        /// <param name="property"></param>
        public bool Remove(ParameterItem property)
        {
            if (property != null && m_List.Count > 0)
            {
               return  m_List.Remove(property);
            }
            return false;
        }

        public ParameterItem this[string name]
        {
            get
            {
                for (int i = 0; i < m_List.Count; i++)
                {
                    ParameterItem item = m_List[i] as ParameterItem;
                    if (item != null && item.Name == name)
                    {
                        return item;
                    }
                }
                return null;
            }
        }


        #region 实现接口
        public void Clear()
        {
            this.m_List.Clear();
        }

        public bool Contains(ParameterItem item)
        {
            return this.m_List.Contains(item);
        }

        public void CopyTo(ParameterItem[] array, int arrayIndex)
        {
            this.m_List.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.m_List.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
        public IEnumerator<ParameterItem> GetEnumerator()
        {
            return this.m_List.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.m_List.GetEnumerator();
        } 
        #endregion
    }
    /// <summary>
    /// 参数项目
    /// </summary>
    public class ParameterItem
    {
        private object m_Value;
        public event EventHandler<ParameterValueChangedArgs> OnValueChanged;

        public ParameterItem()
        {
            this.Name = string.Empty;
            this.DisplayName = String.Empty;
            this.IsVisible = true;
            this.IsEnable = true;
        }

        public ParameterItem(string name, object value)
            : this()
        {
            this.Name = name;
            this.Value = value;
            this.DisplayName = name;
        }

        public ParameterCollection Owner { get; internal set; }

        #region 特殊处理属性值

        /// <summary>
        /// 属性值
        /// </summary>
        public object Value
        {
            get { return this.m_Value; }
            set
            {
                ParameterValueChangedArgs args = new ParameterValueChangedArgs(this.m_Value, value, this.Owner);
                bool changed = this.m_Value != value;
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
        public bool IsEnable { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// 属性名显示
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Description { get; set; }
        #endregion

        /// <summary>
        /// 显示控制
        /// </summary>
        public ParamShowControl ShowControl { get; set; }
        /// <summary>
        /// 扩展关联信息
        /// </summary>
        public object Tag { get; set; }
    }

    /// <summary>
    /// 显示控制
    /// </summary>
    public class ParamShowControl
    {
        public ParamShowControl()
        {
            this.CanEdit = true;
            this.IsDropDown = true;
            this.IsNumber = true;
            this.Items=new List<object>();
        }

        /// <summary>
        /// 如果是下拉框，标识下拉框是否可编辑
        /// </summary>
        public bool CanEdit { get; set; }

         /// <summary>
        /// 是否支持下拉框
        /// </summary>
        public bool IsDropDown { get; set; }
        /// <summary>
        /// 下拉框列表
        /// </summary>
        public List<object> Items { get; set; }
        /// <summary>
        /// 是否限制输入数字
        /// </summary>
        public bool IsNumber { get; set; }
    }

    /// <summary>
    /// 值参数变化事件参数
    /// </summary>
    public class ParameterValueChangedArgs : HandledEventArgs
    {
        public ParameterValueChangedArgs(object oldValue, object newValue, ParameterCollection collection)
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
        public ParameterCollection ReferenceItems { get; private set; }
    }

}
