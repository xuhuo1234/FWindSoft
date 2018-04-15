/*
 * 目标元素的两种组织方式
 * 1、强组合。将扩展属性集中到目标类中
 * 2、弱组合。将目标类型单独关联到目标类中
 */

namespace FWindSoft.Handle
{
    public class TargetObject<T,S>
    {
        /// <summary>
        /// 分类
        /// </summary>
        public T Category { get; set; }
        /// <summary>
        /// 关联源元素
        /// </summary>
        public S RefSourceObject { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsUsable{get;set;}
    }

    public class TargetObject<T, S,G>:TargetObject<T,S>
    {

        /// <summary>
        /// 关联目标元素
        /// </summary>
        public G RefTargetObject { get; set; }
    }
}
