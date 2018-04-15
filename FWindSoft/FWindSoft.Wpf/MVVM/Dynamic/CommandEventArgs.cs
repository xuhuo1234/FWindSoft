
using System;

namespace FWindSoft.MVVM
{
    /// <summary>
    /// 事件参数类
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        private CommandAdapter m_Adapter;

        public CommandAdapter Adapter { get { return m_Adapter; } }

        private CommandInfo m_Command;
        public CommandInfo Command { get { return m_Command; } }

        public CommandEventArgs(CommandAdapter adapter, CommandInfo command)
        {
            m_Adapter = adapter;
            m_Command = command;
        }
    }
}
