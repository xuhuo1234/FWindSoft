using System;
using System.Threading;

namespace FWindSoft.MVVM
{
    public class MouseDoubleClickCommand : BaseCommand
    {
        private int m_Interval;
        private object m_PreviousParam;
        private readonly object o = new object();
        private Timer m_Tirmer;
        public MouseDoubleClickCommand(Action<object> execute, Predicate<object> canExecute)
            : base(execute, canExecute)
        {
            m_Interval = 500;
            this.m_Tirmer=new Timer(Callback,o,int.MaxValue,this.m_Interval);
            
        }
        public MouseDoubleClickCommand(Action<object> execute, Predicate<object> canExecute, int interval)
            : base(execute, canExecute)
        {
            m_Interval = interval;
            this.m_Tirmer = new Timer(Callback, o, int.MaxValue, this.m_Interval);

        }
        private void Callback(object ob)
        {
            lock (o)
            {
                this.m_PreviousParam = null;
                this.m_Tirmer.Change(int.MaxValue, this.m_Interval);
            }
        }

        public override void Execute(object parameter)
        {
            if (m_PreviousParam == null)
            {               
                m_PreviousParam = parameter;
                this.m_Tirmer.Change(this.m_Interval, this.m_Interval);
                return;                             
            }
            else
            {
                lock (o)
                {
                    if (m_PreviousParam != null && m_PreviousParam.Equals(parameter))
                    {
                        base.Execute(parameter);
                        this.m_PreviousParam = null;
                        this.m_Tirmer.Change(int.MaxValue, this.m_Interval);
                    }
                    else
                    {
                        m_PreviousParam = parameter;
                        this.m_Tirmer.Change(this.m_Interval, this.m_Interval);
                    }
                }                    
            }
        }
    }
}
