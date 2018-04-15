using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FWindSoft.Handle
{
    public interface IParse<in T, in U>
    {
        /// <summary>
        /// 解析操作
        /// </summary>
        /// <param name="target">解析的目标元素</param>
        /// <param name="source">解析的源始元素</param>
        /// <returns></returns>
        bool Parse(T target, U source);
    }
    /// <summary>
    /// 验证代理类
    /// </summary>
    public class BaseParseProxy<T,U> : IParse<T,U>, IXmlSerializable
    {
        private IParse<T, U> m_Parse;

        /// <summary>
        /// 序列化时使用的默认构造函数
        /// </summary>
        public BaseParseProxy()
        {
        }

        public BaseParseProxy(IParse<T, U> prase)
        {
            m_Parse = prase;
        }


        public virtual bool Parse(T target, U source)
        {
            bool flag = false;
            try
            {
                if (m_Parse != null)
                {
                    flag = m_Parse.Parse(target,source);
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception)
            {

                flag = false;
            }
            return flag;
        }

        #region xml序列化接口

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            string fullName = "";
            fullName = reader.ReadString() ?? "";
            IParse<T, U> result = null;
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                result = assembly.CreateInstance(fullName) as IParse<T, U>;
            }
            catch (Exception)
            {

            }
            reader.ReadEndElement();
            m_Parse = result;

        }

        public void WriteXml(XmlWriter writer)
        {
            string fullNmae = "";
            if (m_Parse != null)
                fullNmae = m_Parse.GetType().FullName;
            writer.WriteString(fullNmae);
            //writer.WriteEndElement();
        }

        #endregion
    }
}
