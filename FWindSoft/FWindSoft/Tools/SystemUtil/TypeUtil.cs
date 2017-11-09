using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FWindSoft.Tools.SystemUnit
{
    public class TypeUtil
    {
        /// <summary>
        /// 将输入值转换成指定类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object ChangeType(object value, Type targetType)
        {

            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                NullableConverter nullableConverter = new NullableConverter(targetType);
                Type convertType = nullableConverter.UnderlyingType;
                return Convert.ChangeType(value, convertType);
            }
            if (value == null && targetType.IsGenericType)
            {
                return Activator.CreateInstance(targetType);
            }
            if (value == null)
            {
                return null;
            }
            if (targetType == value.GetType())
            {
                return value;
            }
            if (targetType.IsEnum)
            {
                if (value is string)
                {
                    return Enum.Parse(targetType, value as string);
                }
                else
                {
                    return Enum.ToObject(targetType, value);
                }
            }
            if (!targetType.IsInterface && targetType.IsGenericType)
            {
                Type innerType = targetType.GetGenericArguments()[0];
                object innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(targetType, new object[] { innerValue });
            }
            if (value is string && targetType == typeof(Guid))
            {
                return new Guid(value as string);
            }
            if (value is string && targetType == typeof(Version))
            {
                return new Version(value as string);
            }
            if (!(value is IConvertible))
            {
                return value;
            }
            return Convert.ChangeType(value, targetType);
        }

        /// <summary>
        /// 获取指定类型的默认值
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object DefaultValue(Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        /// <summary>
        /// 将iEnumerable类型的变量转化成指定类型的List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<T> ConvertToList<T>(object obj)
        {
            List<T> result = null;
            IEnumerable enumerable = obj as IEnumerable;
            if (enumerable != null)
            {
                result = enumerable.OfType<T>().ToList();
            }
            return result ?? new List<T>();
        }
    }
}
