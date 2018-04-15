
using System;
using System.Linq;
using System.Reflection;

namespace FWindSoft.MVVM
{
    public class BaseViewModel : BasePropertyChanged
    {
      /*此所谓适配器，为了提供统一的访问接口
       * 其充当的角色不过是一个抽象引用。
       * 依托于wpf的反射机制，动态调用vm中方法的名称
       * 其本质无所谓适配器 
      */
       private CommandAdapter m_Commands;
       /// <summary>
       /// 返回所有的命令
       /// </summary>
        public CommandAdapter Commands
        {
            get
            {              
                if (m_Commands == null)
                {
                    Type t = typeof(CommandAdapterFactory<>).MakeGenericType(new Type[] { this.GetType() });
                    m_Commands = (CommandAdapter)(t.GetMember("Create").FirstOrDefault() as MethodInfo)
                        .Invoke(null, new object[] { this });
                }
                return m_Commands;
            }
        }
    }
}
