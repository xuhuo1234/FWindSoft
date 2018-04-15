using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWindSoft.WinForm
{
    public  class ComboxTypeConvert<T>: TypeConverter
    {
        private List<T> m_Mylist = null;
        public ComboxTypeConvert(List<T> list)
        {
            this.CanEdit = true;
            this.CanDrop = true;
            m_Mylist = list;
        }
        #region 属性
        /// <summary>
        /// 是否支持下拉框
        /// </summary>
        public bool CanDrop { get; set; }
        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool CanEdit { get; set; }
        #endregion
 
        //是否支持选择列表的编辑
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return CanDrop;
        }
        //重写Combobox的选择列表
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new TypeConverter.StandardValuesCollection(m_Mylist);
        }

        //判断转换器中否可以工作
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(T))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        //重写转换器，将选择列表中的值 转换到该类型的值
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return value;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }
        //将该类型的值转换到选择列表中
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return !CanEdit;
        }
    }
}
