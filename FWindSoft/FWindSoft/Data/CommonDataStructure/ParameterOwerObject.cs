using System;
using System.Collections.Generic;

namespace FWindSoft.Data
{
    /// <summary>
    /// 参数类类型管理
    /// </summary>
    public class ParameterOwnerObject
    {

        private Dictionary<string, object> m_DicValue = new Dictionary<string, object>();

        public virtual object GetValue(ParameterProperty property)
        {
            Type type = GetType();
            string key = property.CreateKey(type);
            object data;
            if (m_DicValue.TryGetValue(key, out data))
            {
                return data;
            }
            if (!ParameterProperty.GetValue(property, type, out data))
            {
                //throw new Exception("属性没有进行注册");
            }
            return data;
        }

        public virtual void SetValue(ParameterProperty property, object value)
        {
            if (!property.IsValidValue(value))
            {
                throw new ArgumentException("值类型不匹配");
            }
            Type type = GetType();
            string key = property.CreateKey(type);
            m_DicValue[key] = value;
        }
    }
}
