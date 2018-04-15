using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FWindSoft.Wpf;

namespace OBR.Wpf.Controls
{
    /// <summary>
    /// TreeCategory.xaml 的交互逻辑
    /// </summary>
    public partial class TreeCategory : UserControl
    {
        public TreeCategory()
        {
            InitializeComponent();
        }
    }
    public partial class TreeCategory
    {
        /*
         * 绑定数据源必须存在Name的属性
         * 直接和界面中属性关联的  使用绑定进行关联
         * 如果公开的属性和界面中用到的属性经过中间处理，则通过使用空间名字的方法进行赋值
         */
        #region 依赖属性定义

        public static readonly DependencyProperty ItemsProperty;
        public static readonly DependencyProperty DisplayPathProperty;
        public static readonly DependencyProperty ChildrenPathProperty;
        #endregion
        #region 定义命令

        private static RoutedCommand m_SelectCommand;

        public static RoutedCommand SelectCommand
        {
            get { return m_SelectCommand; }
        }
        #endregion

        #region 定义事件

        public static readonly RoutedEvent SelectChangedEvent = EventManager.RegisterRoutedEvent("SelectChanged",
            RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventHandler<object>), typeof(TreeCategory));

        #endregion
        static TreeCategory()
        {
            ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable), typeof(TreeCategory), new PropertyMetadata(default(IEnumerable), new PropertyChangedCallback(PropertyChangedCallback)));
            DisplayPathProperty = DependencyProperty.Register("DisplayPath", typeof(string), typeof(TreeCategory), new PropertyMetadata(default(string), new PropertyChangedCallback(PropertyChangedCallback)));
            ChildrenPathProperty = DependencyProperty.Register("ChildrenPath", typeof(string), typeof(TreeCategory), new PropertyMetadata(default(string), new PropertyChangedCallback(PropertyChangedCallback)));

            m_SelectCommand = new RoutedCommand("SelectCommand", typeof(TreeCategory));
            System.Windows.Input.CommandManager.RegisterClassCommandBinding(typeof(TreeCategory), new CommandBinding(m_SelectCommand, OnSelectCommand, CanExecuteRoutedEventHandler));

            HeightProperty.OverrideMetadata(typeof(TreeCategory), new FrameworkPropertyMetadata(120d, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(HeightPropertyChangedCallback)));
        }
        #region 依赖属性封装
        /// <summary>
        /// 控件包好项目
        /// </summary>
        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
        /// <summary>
        /// 类型显示字段
        /// </summary>
        public string DisplayPath
        {
            get { return (string)GetValue(DisplayPathProperty); }
            set { SetValue(DisplayPathProperty, value); }
        }
        /// <summary>
        /// 控件选中项目
        /// </summary>
        public IList SelectItems { get; private set; }
        /// <summary>
        /// 子集合路径
        /// </summary>
        public string ChildrenPath
        {
            get { return (string)GetValue(ChildrenPathProperty); }
            set { SetValue(ChildrenPathProperty, value); }
        }
        #endregion
        #region 事件封装
        public event RoutedPropertyChangedEventHandler<object> SelectChanged
        {
            add { this.AddHandler(SelectChangedEvent, value); }
            remove { this.RemoveHandler(SelectChangedEvent, value); }
        }

        #endregion
        #region 相关静态方法
        #region 属性变化与界面关联
        private static void HeightPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TreeCategory section = sender as TreeCategory;
            if (section != null)
            {
                section.tree.Height = (double)args.NewValue;
            }
        }
        private static void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TreeCategory tree = sender as TreeCategory;
            if (tree != null)
            {
                string propeertyName = args.Property.Name;
                switch (propeertyName)
                {
                    case "Items":
                        {
                            object tempObject = args.NewValue;
                            if (tempObject == null)
                            {
                                tree.DisplayItemsSource = null;
                                tree.SelectItems = null;
                                tree.tree.ItemsSource = null;
                                break;
                            }
                            var type = tempObject.GetType().GetInterface(typeof(IEnumerable).Name);
                            if (type == null)
                            {
                                throw new Exception("数据源需要实现IEnumerable接口");
                            }
                            tree.InitSource((IEnumerable)tempObject,tree.DisplayPath, tree.ChildrenPath);
                            break;
                        }
                    case "ChildrenPath":
                    {
                        string tempObject = args.NewValue as string;
                        if (tree.Items == null)
                            break;
                        tree.InitSource(tree.Items, tree.DisplayPath, tempObject);
                        break;
                    }
                    case "DisplayPath":
                    {
                        string tempObject = args.NewValue as string;
                        if (tree.Items == null)
                            break;
                        tree.InitSource(tree.Items, tempObject, tree.ChildrenPath);
                        break;
                    }
                }
            }
        }
        #endregion
        #endregion

        #region 命令处理

        private static void OnSelectCommand(object sender, ExecutedRoutedEventArgs e)
        {
            TreeCategory list = sender as TreeCategory;
            if (list != null)
            {
                InnerCheckItem item = e.Parameter as InnerCheckItem;
                if (item != null)
                {
                    item.CheckedChanged();
                }
                list.ChangeSelectItems();
            }
        }

        private static void CanExecuteRoutedEventHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion
        #region 私有属性

        private ObservableCollection<InnerCheckItem> DisplayItemsSource { get; set; }

        #endregion
        #region 类型私有方法集合

        private void InitSource(IEnumerable source,string namePath, string childPath)
        {
            /*
             * 这种分两种情况考虑递归的方式，是因为，最顶层可能不存在根节点。所以顶层数据结构需要特殊处理
             */
            if (source == null)
            {
                this.DisplayItemsSource = null;
                return;
            }
            childPath = childPath ?? "";
            IEnumerator enumerator = ((IEnumerable)source).GetEnumerator();
            try
            {
                var tempSource = new ObservableCollection<InnerCheckItem>();
                while (enumerator.MoveNext())
                {
                    var tempCurrent = enumerator.Current;
                    if (tempCurrent != null)
                    {
                        Type type = tempCurrent.GetType();
                        PropertyInfo propertyInfo = type.GetProperty(childPath);
                        if (propertyInfo == null || propertyInfo.PropertyType.GetInterface(typeof(IEnumerable).Name) == null)
                        {
                            tempSource.Add(new InnerCheckItem(tempCurrent, namePath));
                        }
                        else
                        {
                            InnerCheckItemCollection tempCollection;
                            tempSource.Add(tempCollection = new InnerCheckItemCollection(tempCurrent, namePath));
                            object children = propertyInfo.GetValue(tempCurrent, null);
                            IEnumerator enumeratorCollection = ((IEnumerable)children).GetEnumerator();
                            try
                            {
                                while (enumeratorCollection.MoveNext())
                                {
                                    var tempCurrentCollection = enumeratorCollection.Current;
                                    if (tempCurrentCollection != null)
                                    {
                                        InitSource(tempCollection, tempCurrentCollection, namePath, childPath);
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }
                            finally
                            {
                                enumeratorCollection.Reset();
                            }
                        }
                    }

                }
                this.DisplayItemsSource = tempSource;
            }
            finally
            {
                enumerator.Reset();
            }
            this.tree.ItemsSource = this.DisplayItemsSource;
            this.SelectItems = null;
        }
        private void InitSource(InnerCheckItemCollection collection, object obj,string namePath, string childPath)
        {
            Type type = obj.GetType();
            PropertyInfo propertyInfo = type.GetProperty(childPath);
            if (propertyInfo == null || propertyInfo.PropertyType.GetInterface(typeof(IEnumerable).Name) == null)
            {
                collection.AddItem(new InnerCheckItem(obj, namePath));
            }
            else
            {
                InnerCheckItemCollection tempCollection;
                collection.AddItem(tempCollection = new InnerCheckItemCollection(obj, namePath));
                IEnumerator enumerator = ((IEnumerable)obj).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var tempCurrent = enumerator.Current;
                    if (tempCurrent != null)
                    {
                        InitSource(tempCollection, tempCurrent,namePath, childPath);
                    }
                }
            }
        }
        #endregion
        #region 公开selectChange事件

        private void ChangeSelectItems()
        {
            List<object> tempObjects = new List<object>();
            List<InnerCheckItem> tempSource=new List<InnerCheckItem>(this.DisplayItemsSource);
            for (int i = 0; i < tempSource.Count; i++)
            {
                InnerCheckItem tempItem = tempSource[i];
                if (tempItem is InnerCheckItemCollection)
                {
                    tempSource.AddRange(((InnerCheckItemCollection)tempItem).Items);
                    continue;
                }
                if (tempItem.IsChecked)
                {
                    tempObjects.Add(tempItem.RefObject);
                    continue;
                }
            }
            object oldValue = this.SelectItems;
            this.SelectItems = tempObjects;
            OnSelectChanged(oldValue, this.SelectItems);
        }

        protected virtual void OnSelectChanged(object oldValue, object newValue)
        {
            RoutedPropertyChangedEventArgs<object> arg = new RoutedPropertyChangedEventArgs<object>(oldValue, newValue, SelectChangedEvent);
            this.RaiseEvent(arg);
        }

        #endregion
        #region 选择实例方法
        /// <summary>
        /// 获取所有叶子节点
        /// </summary>
        /// <returns></returns>
        private List<InnerCheckItem> GetAllLeaf()
        {
            List<InnerCheckItem> leaves = new List<InnerCheckItem>();
            if (this.DisplayItemsSource == null)
            {
                return leaves;
            }
            foreach (var innerCheckItem in DisplayItemsSource)
            {
                GetAllLeaf(leaves, innerCheckItem);
            }
            return leaves;
        }
        private void GetAllLeaf(List<InnerCheckItem> list, InnerCheckItem item)
        {
            if (item == null) return;
            InnerCheckItemCollection tempCollection = item as InnerCheckItemCollection;
            if (tempCollection != null)
            {
                foreach (InnerCheckItem innerCheckItem in tempCollection.Items)
                {
                    GetAllLeaf(list, innerCheckItem);
                }
                return;
            }
            list.Add(item);
        }
        /// <summary>
        /// 设置选中项
        /// </summary>
        /// <param name="o"></param>
        public void SetSelected(object o)
        {
            var allLeaf = GetAllLeaf();
            foreach (var innerCheckItem in allLeaf)
            {
                if (innerCheckItem.RefObject.Equals(o))
                {
                    innerCheckItem.IsChecked = true;
                }
            }
            allLeaf.ForEach(leaf => leaf.CheckedChanged());
        }
        /// <summary>
        /// 设定选中元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="compare"></param>
        public void SetSelected<T>(T o, IEqualityComparer<T> compare) where T : class
        {
            var allLeaf = GetAllLeaf();
            foreach (var innerCheckItem in allLeaf)
            {
                T tempRef = innerCheckItem.RefObject as T;
                if (compare != null && tempRef != null && compare.Equals(tempRef, o))
                {
                    innerCheckItem.IsChecked = true;
                }
            }
            allLeaf.ForEach(leaf => leaf.CheckedChanged());
        }
        /// <summary>
        /// 设置选中集合
        /// </summary>
        /// <param name="selectList"></param>
        /// <param name="compare"></param>
        public void SetSelected<T>(IList<T> selectList, IEqualityComparer<T> compare) where T : class
        {
            var allLeaf = GetAllLeaf();
            foreach (var innerCheckItem in DisplayItemsSource)
            {
                T tempRef = innerCheckItem.RefObject as T;
                if (tempRef == null)
                    continue;
                foreach (T o in selectList)
                {
                    if (compare != null && compare.Equals(tempRef, o))
                    {
                        innerCheckItem.IsChecked = true;
                    }
                }
            }
            allLeaf.ForEach(leaf => leaf.CheckedChanged());
        }
        /// <summary>
        /// 设置选中集合
        /// </summary>
        /// <param name="selectList"></param>
        public void SetSelected(IList selectList)
        {
            var allLeaf = GetAllLeaf();
            foreach (var innerCheckItem in DisplayItemsSource)
            {
                if (selectList.Contains(innerCheckItem.RefObject))
                {
                    innerCheckItem.IsChecked = true;
                }
            }
            allLeaf.ForEach(leaf => leaf.CheckedChanged());
        }


        private void SetAllElementChecked(bool isChecked,bool turnChecked=false)
        {
            bool flag = isChecked;
            if (DisplayItemsSource == null)
                return;
            var tempSource = new List<InnerCheckItem>(this.DisplayItemsSource);

            for (int i = 0; i < tempSource.Count; i++)
            {
                var tempItem = tempSource[i];
                if (isChecked)
                {
                    flag = !tempItem.IsChecked;
                }
                tempItem.IsChecked = flag;
                InnerCheckItemCollection collection = tempItem as InnerCheckItemCollection;
                if (collection != null)
                {
                    tempSource.AddRange(collection.Items);
                }
            }
        }
        /// <summary>
        /// 选中所有元素
        /// </summary>
        public void CheckAll()
        {
            SetAllElementChecked(true);
            ChangeSelectItems();
        }
        /// <summary>
        /// 去除所有选中元素
        /// </summary>
        public void CheckNone()
        {
            SetAllElementChecked(false);
            ChangeSelectItems();
        }
        /// <summary>
        /// 反选
        /// </summary>
        public void CheckTurn()
        {
            SetAllElementChecked(false,true);
            var allLeaf = GetAllLeaf();
            allLeaf.ForEach(leaf => leaf.CheckedChanged());
            ChangeSelectItems();
        }

        #endregion
    } 
        
}
