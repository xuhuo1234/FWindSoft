using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FWindSoft.Data
{
    /// <summary>
    /// 初始化标志
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class InitAttribute : Attribute
    {
    }
    /// <summary>
    /// 自动初始化带标签的基类
    /// </summary>
    public class InitObejct
    {
        public InitObejct()
        {
            Init();
        }
        private void Init()
        {
            Type type = this.GetType();
            var properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                try
                {
                    if (!property.PropertyType.IsClass || property.PropertyType.IsAbstract)
                        continue;
                    InitAttribute[] attrs =
                            property.GetCustomAttributes(typeof(InitAttribute), true) as InitAttribute[];
                    if (attrs.Any())
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(this, string.Empty, null);
                            continue;
                        }
                        var cons = property.PropertyType.GetConstructor(Type.EmptyTypes);
                        if (cons != null)
                        {
                            property.SetValue(this, cons.Invoke(null), null);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
           
        }
    }
}
