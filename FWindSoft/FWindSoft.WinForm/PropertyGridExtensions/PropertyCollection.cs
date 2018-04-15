using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace FWindSoft.WinForm
{
    /// <summary>
    /// 属性窗格绑定类
    /// </summary>
    public class PropertyCollection:ICustomTypeDescriptor
    {
        private readonly List<PropertyItem> m_List;
        public PropertyCollection() : base()
        {
            m_List=new List<PropertyItem>();
        }

        /// <summary>
        /// 增加项目
        /// </summary>
        /// <param name="property"></param>
        public void Add(PropertyItem property)
        {
            if (property == null) return;

            property.Owner = this;
            bool exist = false;
            for (int i = 0; i < m_List.Count; i++)
            {
                PropertyItem item = m_List[i] as PropertyItem;
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
        public void Remove(PropertyItem property)
        {
            if (property != null && m_List.Count > 0)
            {
                m_List.Remove(property);
            }
        }

        public PropertyItem this[string name]
        {
            get
            {
                for (int i = 0; i < m_List.Count; i++)
                {
                    PropertyItem item = m_List[i] as PropertyItem;
                    if(item!=null&&item.Name==name)
                    {
                        return item;
                    }
                }
                return null;
            }
        }

        #region 实现接口
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            ArrayList props = new ArrayList();
            for (int i = 0; i < this.m_List.Count; i++)
            {
                //判断属性是否显示
                if (m_List[i].IsVisible)
                {
                    PropDescriptor psd = new PropDescriptor(m_List[i], attributes);
                    props.Add(psd);
                }

            }
            PropertyDescriptor[] propArray = (PropertyDescriptor[])props.ToArray(typeof(PropDescriptor));
            return new PropertyDescriptorCollection(propArray);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        } 
        #endregion
    }
}
