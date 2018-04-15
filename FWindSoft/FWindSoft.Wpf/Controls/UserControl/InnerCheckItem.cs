using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

using FWindSoft.MVVM;
namespace FWindSoft.Wpf
{
    /*
     * 点击select选中项操作：true,该选中项以下的项目，全选;该项目以上的选项 全选
     *                       false,该选中项以下的项目，全不选；该项目以上的项目重新检测，是否选中，如果出现一个选中则  在往上全选
     */
    /// <summary>
    /// 内部使用勾选类
    /// </summary>
    internal class InnerCheckItem : BasePropertyChanged
    {
        private bool m_IsChecked;
        private string m_Display;
        private object m_RefObject;
        public InnerCheckItem(object o,string namePath)
        {

            this.m_RefObject = o;
            if (namePath == null)
            {
                this.Display = o.ToString();
                return;
            }
            PropertyInfo propertyInfo = o.GetType().GetProperty(namePath);
            if (propertyInfo == null)
            {
                this.Display = o.ToString();
                return;
                // throw new Exception("绑定类型必须包含Name属性");
            }
            
            this.Display = propertyInfo.GetValue(o,null).ToString();
        }
        #region 属性
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked
        {
            get { return this.m_IsChecked; }
            set
            {
                this.m_IsChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }
        /// <summary>
        /// 标签显示
        /// </summary>
        public string Display
        {
            get { return this.m_Display; }
            set
            {
                this.m_Display = value;
                RaisePropertyChanged("IsDisplay");
            }
        }

        public object RefObject
        {
            get { return this.m_RefObject; }
        }

        public InnerCheckItemCollection Prarent { get; set; }

        #endregion

        public virtual void CheckedChanged()
        {
            ValidateParentChecked(this.Prarent);
        }
      

        private void ValidateParentChecked(InnerCheckItemCollection parent)
        {
            if (parent == null)
                return;
            parent.IsChecked=parent.AnyChildChecked();
            ValidateParentChecked(parent.Prarent);
        }
    }
    /// <summary>
    /// 内部使用勾选类集合
    /// </summary>
    internal class InnerCheckItemCollection : InnerCheckItem
    {
        #region 集合类扩展集合属性
        private ObservableCollection<InnerCheckItem> m_Items;
        public ObservableCollection<InnerCheckItem> Items
        {
            get { return m_Items; }
        }

        public void AddItem(InnerCheckItem item)
        {
            item.Prarent = this;
            this.Items.Add(item);
        }

        #endregion
        public InnerCheckItemCollection(object o, string namePath)
            : base(o, namePath)
        {
            m_Items=new ObservableCollection<InnerCheckItem>();
        }
        #region 选中逻辑处理
        /*
         * 父元素回溯一步一步检测递归
         * 子元素集合迭代复制处理
         * 
         */
        public override void CheckedChanged()
        {
            bool currentChecked=this.IsChecked;
            //迭代处理子元素
            List<InnerCheckItem> tempItems=new List<InnerCheckItem>();
            tempItems.AddRange(this.Items);
            for (int i = 0; i < tempItems.Count; i++)
            {
                InnerCheckItem tempItem = tempItems[i];
                tempItem.IsChecked = currentChecked;

                InnerCheckItemCollection tempCollection = tempItem as InnerCheckItemCollection;
                if (tempCollection != null)
                {
                    tempItems.AddRange(tempCollection.Items);
                }
            }

            base.CheckedChanged();
        }

        public  bool AnyChildChecked()
        {
            return this.Items.Any(t => t.IsChecked);
        } 
        #endregion
    }
}
