using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FWindSoft.SystemExtensions
{
    /// <summary>
    /// xml属性扩展
    /// </summary>
    public static class XmlAttributeExtensions
    {
        /// <summary>
        /// 在已知属性集合中获取给点键值的属性值，如果不存在返回""
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetAttributeValue(this XmlAttributeCollection attributes, string name)
        {
            if (attributes == null)
                return "";
            if (string.IsNullOrWhiteSpace(name))
                return "";
            var attribute = attributes[name];
            if (attribute == null)
                return "";
            return attribute.Value;

        }
    }
}
