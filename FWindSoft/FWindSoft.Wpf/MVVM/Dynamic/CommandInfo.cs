
using System;
using System.Windows.Input;

namespace FWindSoft.MVVM
{
    /// <summary>
    /// 命令的信息类
    /// </summary>
   public  struct CommandInfo
    {
       /// <summary>
       /// 命令的引用实例
       /// </summary>
        private ICommand m_CommandReference;

        /// <summary>
        /// 命令的引用实例
        /// </summary>
        public ICommand CommandReference
        {
            get { return m_CommandReference; }
            set { m_CommandReference = value; }
        }
       /// <summary>
       /// 命令的名字
       /// </summary>
        private String m_CommandName;

        /// <summary>
        /// 命令的名字
        /// </summary>
        public String CommandName
        {
            get { return m_CommandName; }
            set { m_CommandName = value; }
        }
    }
}
