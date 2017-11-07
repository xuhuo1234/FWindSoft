using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace FWindSoft.SystemInterface
{
    /// <summary>
    /// 转换简单list对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListConverter<T> : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null || value.GetType() != typeof(string))
                return null;
            if (string.IsNullOrWhiteSpace(value.ToString()))
            {
                return null;
            }
            List<T> result = new List<T>();
            var list = value.ToString().Trim(',').Split(',');
            foreach (var s in list)
            {
                try
                {
                    var tempValue = (T)Convert.ChangeType(s, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
                    if (tempValue != null)
                    {
                        result.Add(tempValue);
                    }
                }
                catch (Exception)
                {
                }
            }
            return result;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
                return "";
            if (value == null || value.GetType().GetGenericTypeDefinition() != typeof(List<>))
                return "";
            List<T> list = value as List<T>;
            if (list == null || list.Count == 0)
                return "";
            return string.Join(",", list);
        }
    }
    /*
     * 1、序列化中，可以标注类型转换类
     * 2、类型转换类不能使用泛型形式
     */
    public class DoulbeListConvert : ListConverter<double>
    {
    }
    public class StringListConvert : ListConverter<string>
    {
    }
}
