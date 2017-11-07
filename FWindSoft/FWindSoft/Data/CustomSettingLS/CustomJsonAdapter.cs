using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;

namespace OBR.DotNet.ConfigSetting
{
    /// <summary>
    /// all Items Operate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IJsonDal<T> where T :  new()
    {
        List<T> Reader(string strData);
        string Writer(List<T> datas);
    }

    /// <summary>
    /// 通用json适配器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommonJsonAdapter<T> : CustomSettingLS<T> where T : new()
    {
        private IJsonDal<T> m_JsonDal;
        /// <summary>
        /// 初始化table数据设配器
        /// </summary>
        /// <param name="dataStore">数据存储类</param>
        /// <param name="jsonDal">数据操作接口</param>
        public CommonJsonAdapter(IDataStore dataStore, IJsonDal<T> jsonDal)
            : base(dataStore)
        {
            if (jsonDal == null)
                throw new ArgumentNullException("数据操作接口不能为空");
            this.m_JsonDal = jsonDal;
        }

        public override void Load(Collection<T> colection)
        {
            string strData = m_DataStore.GetString();
            if (string.IsNullOrEmpty(strData))
                return;
            var datas = this.m_JsonDal.Reader(strData);
            if (null != datas)
            {
                datas.ForEach(data => colection.Add((T)data));

            }
        }

        public override void Save(Collection<T> colection)
        {
            string strData = this.m_JsonDal.Writer(colection.ToList());
            m_DataStore.SaveString(strData);
        }
    }


    public class DataContractJsonSerializerJsonDal<T>:IJsonDal<T> where T:new ()
    {
        private List<Type> types = null;
        public DataContractJsonSerializerJsonDal(List<Type> types)
        {
            this.types = types;
            m_Serializer = new DataContractJsonSerializer(typeof(List<T>), types);
        }

        private DataContractJsonSerializer m_Serializer;
        public List<T> Reader(string strData)
        {
            List<T> datas;
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strData)))
            {
                datas = m_Serializer.ReadObject(ms) as List<T>;
            }
            return datas ?? new List<T>();
        }

        public string Writer(List<T> datas)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                m_Serializer.WriteObject(ms, datas);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
