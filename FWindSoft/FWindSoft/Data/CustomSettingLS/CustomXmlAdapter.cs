using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace OBR.DotNet.ConfigSetting
{
    public interface IXmlDal<T> where T :  new()
    {
        List<T> Reader(XmlDocument document);
        XmlElement Writer(XmlDocument document,T data);
    }
    /// <summary>
    /// 基础xml数据读取操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseXmlDal<T> : IXmlDal<T> where T :  new()
    {
        #region 实现数据操作接口
        public List<T> Reader(XmlDocument document)
        {
            List<T> holeInfos = new List<T>();
            StringBuilder xmlPath = new StringBuilder();
            xmlPath.Append(string.Format(@"//{0}", typeof(T).Name));
            var listNodes = document.SelectNodes(xmlPath.ToString());
            if (listNodes == null)
                return holeInfos;
            var collection = TypeDescriptor.GetProperties(typeof(T));
            foreach (XmlNode node in listNodes)
            {
                XmlAttributeCollection attributes;
                if (node != null && (attributes = node.Attributes) != null)
                {
                    T entity = new T();
                    foreach (PropertyDescriptor property in collection)
                    {
                        TypeConverter convert = property.Converter;
                        //添加为空判断的异常
                       XmlAttribute tempAttribute= attributes[property.Name];
                       if (null == tempAttribute)
                           continue;
                       string xmlValue = tempAttribute.Value ?? "";
                        if (convert == null)
                        {
                            property.SetValue(entity, xmlValue);
                        }
                        else
                        {
                            property.SetValue(entity, convert.ConvertFrom(xmlValue));
                        }
                    }
                    holeInfos.Add(entity);
                }
            }
            return holeInfos;
        }

        public XmlElement Writer(XmlDocument document, T data)
        {
            XmlElement xmlElement = document.CreateElement(data.GetType().Name);
            var collection = TypeDescriptor.GetProperties(data);
            foreach (PropertyDescriptor property in collection)
            {
                TypeConverter convert = property.Converter;
                object value = property.GetValue(data);
                if (value == null)
                {
                    xmlElement.SetAttribute(property.Name, "");
                    continue;
                }
                if (convert == null)
                {
                    xmlElement.SetAttribute(property.Name, value.ToString());
                }
                else
                {
                    var strValue = convert.ConvertTo(value, typeof(string));
                    xmlElement.SetAttribute(property.Name, strValue == null ? "" : strValue.ToString());
                }
            }
            return xmlElement;
        }
        #endregion
    }

    public class CommonXmlAdapter<T> : CustomSettingLS<T> where T :  new()
    {
        private IXmlDal<T> m_XmlDal;
        private XmlDocument m_Document;
        /// <summary>
        /// 初始化table数据设配器
        /// </summary>
        /// <param name="dataStore">数据存储接口</param>
        /// <param name="xmlDal">数据操作接口</param>
        public CommonXmlAdapter(IDataStore dataStore, IXmlDal<T> xmlDal)
            : base(dataStore)
        {
            if (xmlDal == null)
                throw new ArgumentNullException("数据操作接口不能为空");
            this.m_XmlDal = xmlDal;
        }
        /// <summary>
        /// 初始化table数据设配器
        /// </summary>
        /// <param name="dataStore">数据存储接口</param>
        /// <param name="xmlDal">数据操作接口</param>
        /// <param name="document">指定追加document</param>
        public CommonXmlAdapter(IDataStore dataStore, IXmlDal<T> xmlDal,XmlDocument document)
            : base(dataStore)
        {
            if (xmlDal == null)
                throw new ArgumentNullException("数据操作接口不能为空");
            this.m_XmlDal = xmlDal;
            m_Document = document;
        }
        public override void Load(Collection<T> colection)
        {
            try
            {
                string strData = m_DataStore.GetString();
                XmlDocument docment = new XmlDocument();
               
                docment.LoadXml(strData);
                var datas = this.m_XmlDal.Reader(docment);
                datas.ForEach(data => colection.Add((T)data));
            }
            catch (Exception)
            {
            }
        }

        public override void Save(Collection<T> colection)
        {
            string strData = null;
            XmlDocument document =this.m_Document??new XmlDocument();
            string rootName = string.Concat(typeof (T).Name, "s");
            if (!colection.GetType().IsGenericType)
            {
                rootName = colection.GetType().Name;
            }
            XmlElement rootElement = document.CreateElement(rootName);
            foreach (var tempT in colection)
            {
                rootElement.AppendChild(this.m_XmlDal.Writer(document, tempT));
            }
            document.AppendChild(rootElement);
          
            strData = document.InnerXml;
            m_DataStore.SaveString(strData);
        }
    }
}
