using System.Collections.Generic;
using System.Linq;

namespace FWindSoft
{
    public class TComboxItem
    {
        public static TComboxItem NullItem=new TComboxItem("","Null");
        private string m_Name;
        private object m_Value;

        public TComboxItem(string name,object value)
        {
            this.m_Name = name;
            if (this.m_Name == null) this.m_Name = "";
            this.m_Value = value;
        }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string Name {
            get { return this.m_Name; }
            set { this.m_Name = value; }
        }
        /// <summary>
        /// 实际值
        /// </summary>
        public object Value {
            get { return this.m_Value; }
            set { this.m_Value = value; }
        }
        #region 相等方法，后期检索用
        /// <summary>
        /// 名称是否相等
        /// </summary>
        /// <param name="comboxItem"></param>
        /// <returns></returns>
        public bool NameEquals(TComboxItem comboxItem)
        {
            return this.Name == comboxItem.Name;
        }
        /// <summary>
        /// 名称是否相等
        /// </summary>
        /// <param name="name">指定名称</param>
        /// <returns></returns>
        public bool NameEquals(string name)
        {
            return this.Name == name;
        }
        /// <summary>
        /// 值是否相等
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ValueEqulas(object value)
        {
            if (this.Value == null)
                return false;
            return this.Value.Equals(value);
        } 
        #endregion

        public bool IsNull
        {
            get { return this == NullItem; }
        }
    }

    /// <summary>
    /// comboxItem相关集合
    /// </summary>
    public class TComboxItems:List<TComboxItem>
    {
        public TComboxItem GetItemByName(string name)
        {
           return this.FirstOrDefault(c => c.NameEquals(name));
        }
        /// <summary>
        /// 通过值取项目
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TComboxItem GetItemByValue(object value)
        {
            return this.FirstOrDefault(c => c.ValueEqulas(value));
        }
        /// <summary>
        /// 添加项目
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, object value)
        {
            this.Add(new TComboxItem(name,value));
        }
        /// <summary>
        /// 在制定位置插入选项
        /// </summary>
        /// <param name="name">选项名称</param>
        /// <param name="value">值</param>
        /// <param name="index">位置</param>
        public void Insert(string name, object value,int index)
        {
            if (index > this.Count)
            {
                index = this.Count;
            }
            this.Insert(index,new TComboxItem(name,value));
        }
    }
}
