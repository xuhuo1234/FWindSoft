using System;
using System.Collections.Generic;

namespace FWindSoft.Data
{
    public class ParameterProperty
    {
        /// <summary>
        /// 各个属性默认值（定义成静态的，一是为了节约存储空间，二是为了统一管理基本属性和附加属性【没关联的属性】）
        /// </summary>
        private  readonly static Dictionary<string, object> m_DicDefault = new Dictionary<string, object>();
        private ParameterProperty()
        {
        }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 参数所属类型
        /// </summary>
        public Type OwerType { get; private set; }

        public Type ValueType { get; private set; }
        public static ParameterProperty Register(Type ownertype, string name, Type valueType,object defaultValue)
        {
            ParameterProperty property = new ParameterProperty();
            property.Name = name;
            property.OwerType = ownertype;
            property.ValueType = valueType;
            if (property.CheckedKey())
            {
                throw new ArgumentException("该名称属性已经被注册");
            }
            if (!property.IsValidValue(defaultValue))
            {
                throw new ArgumentException("默认值类型不匹配");
            }
            string key = property.CreateKey(ownertype);
            m_DicDefault[key] = defaultValue;
            return property;
        }

        public static bool GetValue(ParameterProperty property,Type ownerType,out object value)
        {
            value = null;
            if (ownerType.FullName == property.OwerType.FullName)
            {
                string key = property.CreateKey(ownerType);
                return m_DicDefault.TryGetValue(key, out value);
            }
            else
            {
                string key = property.CreateKey(ownerType);
                if (!m_DicDefault.TryGetValue(key, out value))
                {
                    key = property.CreateKey();
                    return m_DicDefault.TryGetValue(key, out value);
                }
            }          
            return true;
        }
        public static void SetValue(ParameterProperty property, Type ownerType, object value)
        {
            string key = property.CreateKey(ownerType);
            object data;
            if (m_DicDefault.TryGetValue(key, out data))
            {
                m_DicDefault[key] = value;
            }
            else
            {
                throw new Exception("属性没有进行注册");
            }

        }
        public  string CreateKey(Type type)
        {
            return string.Format("{0}[a]{1}",type.FullName,this.Name);
        }
        public string CreateKey()
        {
            return CreateKey(this.OwerType);
        }
        /// <summary>
        /// 验证值是否是属性的有效值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsValidValue(object value)
        {
            var defaultType = this.ValueType;
            if (value == null)
            {
                return defaultType.IsClass;
            }

            var valueType = value.GetType();
            if (defaultType.IsClass)
            {
                return defaultType == valueType || valueType.IsSubclassOf(defaultType);
            }
            else
            {
                return defaultType == valueType;
            }

            //return value.GetType() == this.ValueType;
        }
        
        /// <summary>
        /// 验证属性是否被注册过
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CheckedKey(Type type)
        {
            string key = this.CreateKey(type);
            return m_DicDefault.ContainsKey(key);
        }
        /// <summary>
        /// 验证属性是否被注册过
        /// </summary>
        /// <returns></returns>
        public bool CheckedKey()
        {
            return CheckedKey(this.OwerType);
        }
    }
}
