using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FWindSoft.MVVM
{
    /// <summary>
    /// 代码生成器类
    /// </summary>
    public static class CodeGenerator
    {
        public static readonly AssemblyName AssemblyName;
        public static readonly AssemblyBuilder Assembly;
        public static readonly ModuleBuilder Module;
        static CodeGenerator()
        {
            AssemblyName = new AssemblyName("{F92A88EB-5131-4447-A409-B0CF97894AE2}");
            Assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(AssemblyName,
                AssemblyBuilderAccess.Run);
            Module = Assembly.DefineDynamicModule("{FE71E98C-1DB2-41ec-AFAB-3AB15701E5A0}");

        }
    }
}
