
using System;
using System.Collections.Generic;

namespace FWindSoft.MVVM
{
    
    public class CommandAdapter
    {
        public object Tag { get; set; }
        /// <summary>
        /// 自定义事件BeforeCommand
        /// </summary>
        public event EventHandler<CommandEventArgs> BeforeCommand;
        /// <summary>
        /// 自定义事件AfterCommand
        /// </summary>
        public event EventHandler<CommandEventArgs> AfterCommand;
       /// <summary>
       /// 在命令执行之前执行的方法
       /// </summary>
       /// <param name="args"></param>
        public void DoBeforeCommand(CommandEventArgs args)
        {
            EventHandler<CommandEventArgs> handler = this.BeforeCommand;
            if (handler != null)
                handler(this, args);
        }
        /// <summary>
        /// 在命令执行之后执行的方法
        /// </summary>
        /// <param name="args"></param>
        public void DoAfterCommand(CommandEventArgs args)
        {
            EventHandler<CommandEventArgs> handler = this.AfterCommand;
            if (handler != null)
                handler(this, args);
        }

        internal List<BaseCommand> m_Commands = new List<BaseCommand>();
        public IEnumerable<BaseCommand> Commands { get { return m_Commands; } }

        /// <summary>
        /// 刷新所有的命令
        /// </summary>
        public void RefreshAllCommands()
        {
            foreach (BaseCommand itemCommand in Commands)
            {
                itemCommand.RaiseExecuteChanged();
            }

        }
        EventHandler m_RequerySuggested;
        /// <summary>
        /// 注册事件
        /// </summary>
        public CommandAdapter()
        {
            m_RequerySuggested = new EventHandler(CommandManager_RequerySuggested);
            System.Windows.Input.CommandManager.RequerySuggested += m_RequerySuggested;
        }

        void CommandManager_RequerySuggested(object sender, EventArgs e)
        {
            RefreshAllCommands();
        }
    }
}
