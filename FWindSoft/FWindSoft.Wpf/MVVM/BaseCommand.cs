using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace FWindSoft.MVVM
{
    public class BaseCommand:ICommand
    {
        private  Action<object> m_Execute;
        private Predicate<object> m_CanExecute;

        public BaseCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.m_Execute = execute;
            this.m_CanExecute = canExecute;
        }
        
        public virtual bool CanExecute(object parameter)
        {
            return this.m_CanExecute == null || this.m_CanExecute(parameter);
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { System.Windows.Input.CommandManager.RequerySuggested += value; }
            remove { System.Windows.Input.CommandManager.RequerySuggested -= value; }
        }

        public virtual void Execute(object parameter)
        {
            if (this.m_Execute != null)
            {
                this.m_Execute(parameter);
            }
        }
        public void RaiseExecuteChanged()
        {
            //不完善
            //EventHandler handler = CanExecuteChanged;
            //if (handler != null)
            //    handler(this, new EventArgs());

        }
    }

    public class CommandManager
    {
        private readonly Dictionary<string,BaseCommand> m_Commands=new Dictionary<string,BaseCommand>();

        public void RegisterCommand(string name,BaseCommand baseCommand)
        {
            m_Commands[name] = baseCommand;
        }

        public void RegisterCommand(string name, Action<object> execute, Predicate<object> canExecute)
        {
            m_Commands[name] = new BaseCommand(execute, canExecute);
        }
        public BaseCommand GetCommand(string name)
        {
            BaseCommand command;
            m_Commands.TryGetValue(name, out command);
            return command;
        }
        public void RemoveCommand(string name)
        {
            m_Commands.Remove(name);
        }
        public void ClearCommand()
        {
           m_Commands.Clear();
        }
    }
}
