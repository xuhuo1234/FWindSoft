using System;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Reflection;


namespace FWindSoft.MVVM
{
    public class CallExtension : MarkupExtension
    {
        private string m_MethodName;
        public CallExtension(string methodName)
        {
            this.m_MethodName = methodName;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var t = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            return new CallCommand(t.TargetObject as FrameworkElement, m_MethodName);
        }
    }

    public class CallCommand : DependencyObject, ICommand
    {
        #region 私有字段
        private  FrameworkElement m_Element;
        private string m_MethodName;
        private MethodInfo m_Method;
        #endregion
        public CallCommand(FrameworkElement element, string methodName)
        {
            this.m_Element = element;
            this.m_MethodName = methodName;
            element.DataContextChanged += target_DataContextChanged;

            BindingOperations.SetBinding(this, CanCallProperty, new Binding("DataContext.Can" + methodName)
            {
                Source = element
            });

            GetMethod();
        }

        public static readonly DependencyProperty CanCallProperty =
            DependencyProperty.Register("CanCall", typeof(bool), typeof(CallCommand),
                                        new PropertyMetadata(true));
        public bool CanCall
        {
            get { return (bool)GetValue(CanCallProperty); }
            set { SetValue(CanCallProperty, value); }
        }

        public object DataContext
        {
            get { return m_Element.DataContext; }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == CanCallProperty)
            {
                RaiseCanExecuteChanged();
            }
        }

        void GetMethod()
        {
            m_Method = DataContext == null ? null : DataContext.GetType().GetMethod(m_MethodName, Type.EmptyTypes);
        }

        void target_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            GetMethod();
            RaiseCanExecuteChanged();
        }

        void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return m_Method != null && CanCall;
        }

        public void Execute(object parameter)
        {
            object[] arrays = new object[1] {parameter};
            m_Method.Invoke(DataContext,arrays);
        }

        #endregion
    }
}
