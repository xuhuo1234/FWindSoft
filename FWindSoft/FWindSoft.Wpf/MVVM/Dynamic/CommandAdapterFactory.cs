
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Input;

namespace FWindSoft.MVVM
{
    /// <summary>
    /// 泛型CommandAdapterFactory类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class CommandAdapterFactory<T> where T : class
    {
        private static readonly Type m_Type;

        /// <summary>
        /// 创建命令适配器（获取Execute方法和CanExecute方法为创建ICommand接口实例准备）
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static object Create(object sender)
        {
            CommandAdapter inst = (CommandAdapter) m_Type.GetConstructor(new Type[] {}).Invoke(new object[] {});
            Type thisType = typeof (T);
            MethodInfo[] methods = thisType.GetMethods();

            foreach (MethodInfo methodItem in methods)
            {
                CommandAttribute[] attrs =
                    methodItem.GetCustomAttributes(typeof (CommandAttribute), true) as CommandAttribute[];
                if ((attrs != null) && (attrs.Length != 0))
                {
                    string methodName = methodItem.Name;

                    string canDoMethodName = "Can" + methodName;
                    bool hasCanDoMethod = (from method in methods
                        where method.Name == canDoMethodName
                        select method).FirstOrDefault() != null;

                    #region 初始化命令
                    BaseCommand cmd = new BaseCommand(
                                 (x) =>
                                 {
                                     CommandEventArgs eargs = new CommandEventArgs(inst,
                                         new CommandInfo() { CommandName = methodName });
                                     inst.DoBeforeCommand(eargs);
                                     try
                                     {
                                         try
                                         {
                                             MethodInfo miLocal = thisType.GetMethod(methodName);
                                             ParameterInfo[] pi = miLocal.GetParameters();
                                             if (pi.Length == 0)
                                             {
                                                 thisType.InvokeMember(methodName, BindingFlags.Public, null, sender,
                                                     new object[] { });
                                             }
                                             else if (pi.Length == 1)
                                             {
                                                 thisType.InvokeMember(methodName,
                                                     BindingFlags.InvokeMethod, null,
                                                     sender, new object[] { x });

                                             }
                                         }
                                         catch
                                         {
                                             throw;
                                         }
                                     }
                                     finally
                                     {
                                         inst.DoAfterCommand(eargs);
                                     }
                                 }, hasCanDoMethod
                                     ? (Predicate<object>)
                                         (
                                             (x) =>
                                             {
                                                 return (bool)thisType.InvokeMember(canDoMethodName,
                                                     BindingFlags.InvokeMethod, null, sender, new object[] { x });
                                             }
                                             )
                                     : null

                                 ); 
                    #endregion

                    m_Type.InvokeMember(methodItem.Name, BindingFlags.SetProperty, null, inst,
                        new object[] {cmd});
                    inst.m_Commands.Add(cmd);
                }
            }
            return inst;
        }

        /// <summary>
        /// 动态创建属性
        /// </summary>
        static CommandAdapterFactory()
        {
            try
            {
                TypeBuilder tb = CodeGenerator.Module.DefineType(typeof (T).Name + "*Commands", TypeAttributes.Class,
                    typeof (CommandAdapter));

                Type thisType = typeof (T);
                MethodInfo[] methods = thisType.GetMethods();
                foreach (MethodInfo methodItem in methods)
                {
                    CommandAttribute[] attrs =
                        methodItem.GetCustomAttributes(typeof (CommandAttribute), true) as CommandAttribute[];
                    if ((attrs != null) && (attrs.Length != 0))
                    {
                        FieldBuilder cmdField = tb.DefineField("_" + methodItem.Name, typeof (ICommand),
                            FieldAttributes.Private);

                        MethodBuilder cmdGet = tb.DefineMethod("get_" + methodItem.Name, MethodAttributes.Public,
                            typeof (ICommand), new Type[] {});
                        ILGenerator cmdGetIL = cmdGet.GetILGenerator();
                        cmdGetIL.Emit(OpCodes.Ldarg_0);
                        cmdGetIL.Emit(OpCodes.Ldfld, cmdField);
                        cmdGetIL.Emit(OpCodes.Ret);

                        MethodBuilder cmdSet = tb.DefineMethod("set_" + methodItem.Name,
                            MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                            typeof (void), new Type[] {typeof (ICommand)});
                        ILGenerator cmdSetIL = cmdSet.GetILGenerator();
                        cmdSetIL.Emit(OpCodes.Ldarg_0);
                        cmdSetIL.Emit(OpCodes.Ldarg_1);
                        cmdSetIL.Emit(OpCodes.Stfld, cmdField);
                        cmdSetIL.Emit(OpCodes.Ret);


                        PropertyBuilder cmdProperty = tb.DefineProperty(methodItem.Name, PropertyAttributes.None,
                            typeof (ICommand),
                            new Type[] {});
                        cmdProperty.SetGetMethod(cmdGet);
                        cmdProperty.SetSetMethod(cmdSet);


                    }
                }

                m_Type = tb.CreateType();
            }
            catch
            {
               
            }
        }
    }
}