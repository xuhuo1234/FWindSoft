
using FWindSoft.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FWindSoft.Wpf.Controls
{
    /// <summary>
    /// ListCategory.xaml 的交互逻辑
    /// </summary>
    public partial class ListCategory : UserControl
    {
        public ListCategory()
        {
            InitializeComponent();
        }
    }
    public partial class ListCategory
    {
        /*
         * 绑定数据源必须存在Name的属性
         * 直接和界面中属性关联的  使用绑定进行关联
         * 如果公开的属性和界面中用到的属性经过中间处理，则通过使用空间名字的方法进行赋值
         */
        #region 依赖属性定义

        public static readonly DependencyProperty ItemsProperty;
        public static readonly DependencyProperty DisplayPathProperty;
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
            RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventHandler<object>), typeof(ListCategory));

        #endregion
        static ListCategory()
        {
            ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable), typeof(ListCategory), new PropertyMetadata(default(IEnumerable), new PropertyChangedCallback(PropertyChangedCallback)));
            DisplayPathProperty = DependencyProperty.Register("DisplayPath", typeof(string), typeof(ListCategory), new PropertyMetadata(default(string), new PropertyChangedCallback(PropertyChangedCallback)));

            m_SelectCommand = new RoutedCommand("SelectCommand", typeof(ListCategory));
            System.Windows.Input.CommandManager.RegisterClassCommandBinding(typeof(ListCategory), new CommandBinding(m_SelectCommand, OnSelectCommand, CanExecuteRoutedEventHandler));

            HeightProperty.OverrideMetadata(typeof(ListCategory), new FrameworkPropertyMetadata(120d, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(HeightPropertyChangedCallback)));
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
        #endregion
        #region 事件封装
        public event RoutedPropertyChangedEventHandler<object> SelectChanged
        {
            add { this.AddHandler(SelectChangedEvent,value); }
            remove { this.RemoveHandler(SelectChangedEvent,value);}
        }

        #endregion
        #region 相关静态方法
        #region 属性变化与界面关联
        private static void HeightPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ListCategory section = sender as ListCategory;
            if (section != null)
            {
                section.list.Height = (double)args.NewValue;
            }
        }
        private static void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ListCategory list = sender as ListCategory;
            if (list != null)
            {
                string propeertyName = args.Property.Name;
                switch (propeertyName)
                {
                    case "Items":
                    {
                        object tempObject = args.NewValue;
                        if (tempObject == null)
                        {
                            list.DisplayItemsSource = null;
                            list.SelectItems = null;
                            list.list.ItemsSource = null;
                            break;
                        }
                        var type=tempObject.GetType().GetInterface(typeof (IEnumerable).Name);
                        if (type == null)
                        {
                            throw new Exception("数据源需要实现IEnumerable接口");
                        }
                        IEnumerator enumerator = ((IEnumerable)tempObject).GetEnumerator();
                        try
                        {
                            var tempSource=new ObservableCollection<InnerCheckItem>();                           
                            while (enumerator.MoveNext())
                            {
                                var tempCurrent = enumerator.Current;
                                if (tempCurrent != null)
                                {
                                    var tempItem = new InnerCheckItem(tempCurrent,list.DisplayPath);
                                    tempSource.Add(tempItem);
                                }

                            }
                            list.DisplayItemsSource = tempSource;
                        }
                        finally
                        {
                            enumerator.Reset();
                        }                       
                        list.list.ItemsSource = list.DisplayItemsSource;
                        list.SelectItems = null;
                        break;
                    }
                    case "DisplayPath":
                    {
                        object tempObject = list.Items;
                        if (tempObject == null)
                        {
                            break;
                        }
                        var type = tempObject.GetType().GetInterface(typeof(IEnumerable).Name);
                        if (type == null)
                        {
                            break;
                        }
                        IEnumerator enumerator = ((IEnumerable)tempObject).GetEnumerator();
                        string newPath = args.NewValue.ToString();
                        try
                        {
                            var tempSource = new ObservableCollection<InnerCheckItem>();
                            while (enumerator.MoveNext())
                            {
                                var tempCurrent = enumerator.Current;
                                if (tempCurrent != null)
                                {
                                    var tempItem = new InnerCheckItem(tempCurrent, newPath);
                                    tempSource.Add(tempItem);
                                }

                            }
                            list.DisplayItemsSource = tempSource;
                        }
                        finally
                        {
                            enumerator.Reset();
                        }
                        list.list.ItemsSource = list.DisplayItemsSource;
                        list.SelectItems = null;
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
            ListCategory list = sender as ListCategory;
            if (list != null)
            {
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

        #region 公开selectChange事件

        private void ChangeSelectItems()
        {
            List<object> tempObjects=new List<object>();
            if (this.DisplayItemsSource != null)
            {
                foreach (var innerCheckItem in DisplayItemsSource)
                {
                    if (innerCheckItem.IsChecked)
                    {
                        tempObjects.Add(innerCheckItem.RefObject);
                    }                  
                }
            }
            object oldValue = this.SelectItems;
            this.SelectItems = tempObjects;
            OnSelectChanged(oldValue, this.SelectItems);
        }

        protected virtual void OnSelectChanged(object oldValue,object newValue )
        {
            RoutedPropertyChangedEventArgs<object> arg = new RoutedPropertyChangedEventArgs<object>(oldValue, newValue, SelectChangedEvent);
            this.RaiseEvent(arg);
        }

        #endregion
        /// <summary>
        /// 设置选中项
        /// </summary>
        /// <param name="o"></param>
        public void SetSelected(object o)
        {
            if (this.DisplayItemsSource == null)
                return;
            foreach (var innerCheckItem in DisplayItemsSource)
            {
                if (innerCheckItem.RefObject.Equals(o))
                {
                    innerCheckItem.IsChecked = true;
                }
            }
        }
       /// <summary>
       /// 设置选中集合
       /// </summary>
        /// <param name="selectList"></param>
        public void SetMultiSelected(IList selectList)
        {
            if (this.DisplayItemsSource == null)
                return;
            foreach (var innerCheckItem in DisplayItemsSource)
            {
                innerCheckItem.IsChecked = selectList.Contains(innerCheckItem.RefObject);
            }
            this.ChangeSelectItems();
        }      
    }
}
