using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FWindSoft.Handle
{
  
    public interface IValidate<in T>
    {
       /// <summary>
       /// 验证指定元素
       /// </summary>
       /// <param name="element"></param>
       /// <returns></returns>
        bool Validate(T element);
    }

    /// <summary>
    /// 验证代理类
    /// </summary>
    public class BaseValidateProxy<T> : IValidate<T>, IXmlSerializable
    {
        private IValidate<T> m_Validate;

        /// <summary>
        /// 序列化时使用的默认构造函数
        /// </summary>
        public BaseValidateProxy()
        {
        }

        public BaseValidateProxy(IValidate<T> validate)
        {
            m_Validate = validate;
        }


        public virtual bool Validate(T element)
        {
            bool flag = false;
            try
            {
                if (m_Validate != null)
                {
                    flag = m_Validate.Validate(element);
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
            IValidate<T> result = null;
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                result = assembly.CreateInstance(fullName) as IValidate<T>;
            }
            catch (Exception)
            {

            }
            reader.ReadEndElement();
            m_Validate = result;

        }

        public void WriteXml(XmlWriter writer)
        {
            string fullNmae = "";
            if (m_Validate != null)
                fullNmae = m_Validate.GetType().FullName;
            writer.WriteString(fullNmae);
            //writer.WriteEndElement();//必须写，序列化按流形式进行
        }

        #endregion
    }
}
