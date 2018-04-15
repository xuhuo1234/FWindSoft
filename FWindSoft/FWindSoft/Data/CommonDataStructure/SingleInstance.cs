namespace FWindSoft.Data
{
    /*
    * 继承自本身泛型扩展，提供强签名公共方法A:SingleInstance<A>，不是单例，因为构造函数没有私有化。但可以提供一个公用的静态维护类
    */
    public class SingleInstance<T> where T : new()
    {
        private static T m_Instance;
        /// <summary>
        /// 系统级别的静态变量
        /// </summary>
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new T();
                return m_Instance;
            }
        }
    }
}
