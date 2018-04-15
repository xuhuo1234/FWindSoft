
using System;

namespace FWindSoft.MVVM
{
    /// <summary>
    /// 特性标记类 标记方法是否是Command方法,控制命令对象的动态生成
    /// </summary>
   [AttributeUsage(AttributeTargets.Method)]
   public  class CommandAttribute:Attribute
    {
    }
}
