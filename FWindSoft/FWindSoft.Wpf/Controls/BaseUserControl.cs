
///////////////////////////////////////////////////////////////////////////////
//Copyright (c) 2015, 
//All rights reserved.       
//文件名称: BaseUserControl.cs
//文件描述: 保存加载历史值的组合控件（常规使用中，组合控件的绑定可能会覆盖load的赋值，原因未知；
//           故作特殊处理）
//创 建 者: xls
//创建日期: 2016-1-26
//版 本 号：1.0.0.0
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FWindSoft.Wpf
{
    /// <summary>
    /// 自定义空间基础类
    /// </summary>
    public class BaseUserControl:UserControl
    {
        private Window m_CurrentOwerWindow = null;
        public BaseUserControl()
        {
            this.Loaded += new RoutedEventHandler(BaseUserControl_Loaded);
            this.Unloaded += new RoutedEventHandler(BaseUserControl_Unloaded);
        }
     

        void BaseUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_CurrentOwerWindow != null)
            {
                this.SaveControlData(m_CurrentOwerWindow);
                m_CurrentOwerWindow = null;
            }
           
        }
       
        void BaseUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(SetControlDalta));
        }

        private void SetControlDalta()
        {
            Window window = this.GetOwnerWindow();
            m_CurrentOwerWindow = window;
            if(window!=null)
               this.SetControlData(window);
        }
    }
}
