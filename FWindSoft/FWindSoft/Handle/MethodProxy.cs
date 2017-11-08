using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace FWindSoft
{
    public sealed class MethodProxy
    {
        public T Proxy<T>(T t)
        {

            var primitive = t as Delegate;
            if (primitive == null)
                return t;
            MethodInfo method = primitive.Method;
            var parameters = method.GetParameters();
            List<ParameterExpression> listParameters = new List<ParameterExpression>();
            foreach (var item in parameters)
            {
                listParameters.Add(Expression.Parameter(item.ParameterType));
            }

            MethodInfo preMethod = (this.GetType()).GetMethod("Pre", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            MethodInfo postMethod = (this.GetType()).GetMethod("Post", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            var pre = Expression.Call(Expression.Constant(this), preMethod);
            var post = Expression.Call(Expression.Constant(this), postMethod);

            Expression instance = primitive.Target != null ? Expression.Constant(primitive.Target, primitive.Target.GetType()) : null;

            MethodCallExpression execute = Expression.Call(instance, method, listParameters.ToArray());

            bool hasReturn = method.ReturnType != typeof(void);
            BlockExpression block = null;
            if (hasReturn)
            {
                #region 有返回值处理
                LabelTarget lblTarget = Expression.Label(method.ReturnType, "return");
                ParameterExpression resultTemp = Expression.Parameter(method.ReturnType);
                BinaryExpression assigment = Expression.Assign(resultTemp, execute);

                GotoExpression returnEx = Expression.Return(lblTarget, resultTemp);
                object defualtValue = method.ReturnType.IsValueType ? Activator.CreateInstance(method.ReturnType) : null;
                //标签表达式写在最后，则捕获返回值
                LabelExpression lblEx = Expression.Label(lblTarget, Expression.Constant(defualtValue, method.ReturnType));//有返回值是必须设置默认值

                block = Expression.Block(new ParameterExpression[] { resultTemp }, pre, assigment, post, returnEx, lblEx);
                #endregion
            }
            else
            {
                #region 无返回值处理
                block = Expression.Block(pre, execute, post);
                #endregion
            }
            var lumbda = Expression.Lambda<T>(block, listParameters.ToArray());
            return lumbda.Compile();

        }

        public event Action PreMethodExecute;
        private void Pre()
        {
            if (PreMethodExecute != null)
            {
                PreMethodExecute();
            }
        }
        public event Action PostMethodExecute;
        private void Post()
        {
            if (PostMethodExecute != null)
            {
                PostMethodExecute();
            }
        }
    }
}
