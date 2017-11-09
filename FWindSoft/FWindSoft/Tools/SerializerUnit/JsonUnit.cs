using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Json;
using System.Text;
using System;
using System.Collections.Generic;

namespace FWindSoft.Tools
{
    public class JsonUnit
    {
        #region 实例相关
        private readonly DataContractJsonSerializer m_Serializer;
        public JsonUnit()
        { 
        }
        public JsonUnit(Type type, IEnumerable<Type> knownTypes)
        {
            m_Serializer = new DataContractJsonSerializer(type, knownTypes);
        }
        public JsonUnit(DataContractJsonSerializer serializer)
        {
            m_Serializer = serializer;
        }

        public string ObjectToJsonStr(object obj)
        {
            string jsonStr = null;
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    m_Serializer.WriteObject(stream, obj);
                    jsonStr = Encoding.UTF8.GetString(stream.ToArray());
                }
                catch
                { 
                }
            }
            return jsonStr ?? string.Empty;
        }
        public object JsonStrToObject(string jsonStr)
        {
            object result = null;
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr)))
            {
                try
                {
                    result = m_Serializer.ReadObject(stream);
                }
                catch
                {
                }
            }
            return result;
        }
        #endregion
        /// <summary>
        /// 将对象转化成Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToJson(object obj)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                ser.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// 将字符串转化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T JsonToObj<T>(string str) where T : class
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                return (T)ser.ReadObject(ms);
            }
        }
    }
}
