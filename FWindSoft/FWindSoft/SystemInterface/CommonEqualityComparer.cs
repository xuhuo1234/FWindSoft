using System;
using System.Collections.Generic;
namespace FWindSoft.SystemInterface
{
    /// <summary>
    /// 无哈希值比较（哈希值均为0）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EqualityComparer<T> : IEqualityComparer<T>
    {
        private Func<T, T, bool> m_Func;

        public EqualityComparer(Func<T, T, bool> func)
        {
            if (func != null)
                this.m_Func = func;
        }

        public bool Equals(T x, T y)
        {
            bool flag = false;
            if (m_Func != null)
                flag = m_Func(x, y);
            return flag;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetType().GetHashCode();
        }
    }

    /// <summary>
    /// 有哈希值比较
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EqualityComparerWithHash<T> : IEqualityComparer<T>
    {
        private Func<T, T, bool> m_Func;

        public EqualityComparerWithHash(Func<T, T, bool> func)
        {
            if (func != null)
                this.m_Func = func;
        }

        public bool Equals(T x, T y)
        {
            bool flag = false;
            if (m_Func != null)
                flag = m_Func(x, y);
            return flag;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
