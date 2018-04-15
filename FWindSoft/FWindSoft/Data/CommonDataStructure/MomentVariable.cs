using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWindSoft.Data
{

    /// <summary>
    /// 瞬态变量，保持一个变量不变，直到该值被使用过一次
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MomentVariable<T>
    {
        private readonly Queue<T> m_Queue = new Queue<T>();
        private T m_DefaultValue;

        public MomentVariable()
        {
            m_DefaultValue = default(T);
        }
        /// <summary>
        /// 瞬态变量的默认值
        /// </summary>
        /// <param name="defaultValue"></param>
        public MomentVariable(T defaultValue)
        {
            m_DefaultValue = defaultValue;
        }
        /// <summary>
        /// 瞬态变量值
        /// </summary>
        public T Value
        {
            set
            {
                //不需判断是否与默认值相同
                lock (m_Queue)
                {
                    m_Queue.Clear();
                    m_Queue.Enqueue(value);
                }
            }
            get
            {
                lock (m_Queue)
                {
                    if (m_Queue.Any())
                    {
                        return m_Queue.Dequeue();
                    }
                    return m_DefaultValue;
                }
            }
        }
    }
}
