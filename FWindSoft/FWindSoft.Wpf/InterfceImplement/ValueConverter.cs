using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace FWindSoft.Wpf
{
    /// <summary>
    /// 枚举类型转换器
    /// </summary>
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object returnValue = false;
            if (Enum.IsDefined(value.GetType(), value) == false)
                returnValue = false;
            else
            {
                Object paramValue = Enum.Parse(value.GetType(), parameter.ToString(), true);
                returnValue = paramValue.Equals(value);
            }
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.Parse(targetType, parameter.ToString(), true);
        }

    }

    /// <summary>
    /// 布尔类型相反转换
    /// </summary>
    [ValueConversion(typeof (bool), typeof (bool))]
    public class BooleanOppositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return true;
            var isbool = (bool) value;
            return !isbool;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return true;
            var isbool = (bool) value;
            return !isbool;
        }
    }

    /// <summary>
    /// 布尔类型翻转对应可见性
    /// </summary>
    public class BooleanOppositionToVisibilityConverter : IValueConverter
    {
        private BooleanToVisibilityConverter m_tempConvert;

        public BooleanOppositionToVisibilityConverter()
        {
            m_tempConvert = new BooleanToVisibilityConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return m_tempConvert.Convert(!(System.Convert.ToBoolean(value)), targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(System.Convert.ToBoolean(m_tempConvert.ConvertBack(value, targetType, parameter, culture)));
        }
    }
    /// <summary>
    /// true\false 对布尔类型的转换
    /// </summary>
    public class BoolStringToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object returnValue = false;
            if (parameter != null)
            {
                bool tempValue;
                bool.TryParse(parameter.ToString(), out tempValue);
                returnValue = tempValue && System.Convert.ToBoolean(value);

            }
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool tempValue;
            bool.TryParse(parameter.ToString(), out tempValue);
            return tempValue;
        }
    }

    /// <summary>
    /// 多值转换
    /// </summary>
    public class MultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value.GetType().IsArray)
                return (object[])value;
            else
            {
                throw new Exception("转换失败");
            }
        }
    }

    /// <summary>
    /// 布尔类型翻转
    /// </summary>
    public class BoolTurnOver : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new ArgumentException("目标类型必须是bool类型");
            if (value.GetType() != typeof(bool))
                throw new ArgumentException("转换参数必须是bool类型");
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new ArgumentException("目标类型必须是bool类型");
            if (value.GetType() != typeof(bool))
                throw new ArgumentException("转换参数必须是bool类型");
            return !(bool)value;
        }
    }

    /// <summary>
    /// 特殊长度转换GridtoDataGrid
    /// </summary>
    public class DoubleToDgLengthConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(DataGridLength))
                throw new ArgumentException("目标类型必须是DataGridLength类型");
            if (value.GetType() != typeof(GridLength))
                throw new ArgumentException("转换参数必须是GridLength类型");
            return new DataGridLength(System.Convert.ToDouble(((GridLength)value).Value)); ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (targetType != typeof(bool))
            //    throw new ArgumentException("目标类型必须是bool类型");
            //if (value.GetType() != typeof(bool))
            //    throw new ArgumentException("转换参数必须是bool类型");
            //return !(bool)value;
            return 10d;
        }
    }
}
