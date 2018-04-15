
namespace FWindSoft.Data
{
    /*
     * 继承自本身泛型扩展，提供强签名公共方法A:BaseContext<A>
     */
    public class BaseContext<T> : InitObejct where T : new()
    {
        /// <summary>
        /// 标识最近的执行过程是否成功
        /// </summary>
        public bool HasSuccess { get; set; }

      
    }
}
