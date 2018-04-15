using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace FWindSoft.MVVM
{
    [DataContract]
    public abstract  class BasePropertyChanged : INotifyPropertyChanged
    {
       
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        /// <summary>
        /// 提交变更
        /// </summary>
        /// <param name="propertyName">变更属性名称</param>
        protected void RaisePropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {

                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (null != propertyExpression)
            {
                var memberExpression = propertyExpression.Body as MemberExpression;
                if (memberExpression != null)
                    if(this.PropertyChanged!=null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs(memberExpression.Member.Name));
                    }
                   
            }
            else
            {
                throw new Exception();
            }

        }

    }
}
