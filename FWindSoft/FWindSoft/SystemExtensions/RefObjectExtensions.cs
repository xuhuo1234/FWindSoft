using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OBR.DotNet.SystemTypeExtensions
{
    /// <summary>
    /// 之所以如此命名，而不直接是ObjecExtensions。是要时刻提醒考虑string的特殊情况
    /// </summary>
    public static class RefObjectExtensions
    {
        public static T Clone<T>(this T obj)
        {
            try
            {
                Type type = typeof(T);
                var memberwiseClone = type.GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                return (T)memberwiseClone.Invoke(obj, null);
            }
            catch (Exception)
            {
                throw new Exception("复制对象异常");
            }
        }
    }
}
