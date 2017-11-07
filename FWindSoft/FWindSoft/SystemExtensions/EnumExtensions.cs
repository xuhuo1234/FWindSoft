using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FWindSoft.SystemExtensions
{
    /// <summary>
    /// 枚举类型扩展显示类
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取指定枚举类型的描述值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="en">给定枚举</param>
        /// <returns></returns>
        public static string GetDisplay<T>(this T t) where T :struct
        {
            string display = t.ToString();
            FieldInfo fieldInfo = t.GetType().GetField(display);
            object[] attributes = fieldInfo.GetCustomAttributes(typeof (DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                DescriptionAttribute info = attributes[0] as DescriptionAttribute;
                if (info != null)
                {
                    display = info.Description;
                }

            }
            return display;
        }

        /// <summary>
        /// 获取枚举的条目列表
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns></returns>
        public static List<EnumItem<T>> GetEnumItems<T>() where T : struct
        {
            List<EnumItem<T>> items = new List<EnumItem<T>>();
            FieldInfo[] fieldInfos = typeof (T).GetFields();
            foreach (FieldInfo field in fieldInfos)
            {
                if (field.FieldType == typeof (T))
                {
                    object[] attributes = field.GetCustomAttributes(typeof (DescriptionAttribute), false);
                    if (attributes.Length > 0)
                    {
                        DescriptionAttribute info = attributes[0] as DescriptionAttribute;
                        if (info != null)
                        {

                            T tValue = (T) Convert.ChangeType(field.GetValue(field), field.FieldType);
                            items.Add(new EnumItem<T>(tValue, info.Description));
                        }

                    }
                }


            }
            return items;
        }

        /// <summary>
        /// 根据指定描述获取枚举条目
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="display">枚举类型描述</param>
        /// <returns></returns>
        public static EnumItem<T> GetEnumItem<T>(string display) where T : struct
        {

            FieldInfo[] fieldInfos = typeof (T).GetFields();
            foreach (FieldInfo field in fieldInfos)
            {
                if (field.FieldType == typeof (T))
                {
                    object[] attributes = field.GetCustomAttributes(typeof (DescriptionAttribute), false);
                    if (attributes.Length > 0)
                    {
                        DescriptionAttribute info = attributes[0] as DescriptionAttribute;
                        if (info != null && info.Description == display)
                        {

                            T tValue = (T) Convert.ChangeType(field.GetValue(field), field.FieldType);
                            return new EnumItem<T>(tValue, info.Description);
                        }
                    }
                }
            }
            return null;
        }
    }

    /// <summary>
    /// 枚举描述类
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    public class EnumItem<T> where T : struct
    {
        private T m_Item;
        private string m_Display;

        public EnumItem(T item)
        {
            this.m_Item = item;
            this.m_Display = m_Item.GetDisplay();
        }

        public EnumItem(T item, string display)
        {
            this.m_Item = item;
            this.m_Display = display;
        }

        public EnumItem(string display)
        {
            this.m_Item = EnumExtensions.GetEnumItem<T>(display).Item;
            this.m_Display = display;
        }

        public T Item
        {
            set { m_Item = value; }
            get { return m_Item; }
        }

        public string Display
        {
            set { m_Display = value; }
            get { return m_Display; }
        }

        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj == null)
                return flag;
            if (this.GetType() == obj.GetType())
            {
                flag = this.Item.Equals(((EnumItem<T>) obj).Item);
            }
            return flag;
        }

        public override int GetHashCode()
        {
            return m_Item.GetHashCode();
        }
    }
}
