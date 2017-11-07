using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OBR.DotNet.ConfigSetting
{
    public interface ITableDal<T> where T : new()
    {
        T Reader(DataRow row);
        Dictionary<string, object> Writer(T data);

        /// <summary>
        /// 表结构
        /// </summary>
        DataTable TableStructure { get; }
    }
    /// <summary>
    /// 通用表结构存储适配器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommonTableAdapter<T> : CustomSettingLS<T> where T : new()
    {
        private readonly ITableDal<T> m_TableDal;

        /// <summary>
        /// 初始化table数据设配器
        /// </summary>
        /// <param name="dataStore">文件存储接口</param>
        /// <param name="tableDal">数据操作接口</param>
        public CommonTableAdapter(IDataStore dataStore, ITableDal<T> tableDal)
            : base(dataStore)
        {
            if (tableDal == null)
                throw new ArgumentNullException("数据操作接口不能为空");
            this.m_TableDal = tableDal;
        }

        public override void Load(Collection<T> colection)
        {
            string strData = m_DataStore.GetString();
            StringReader strReader = new StringReader(strData);
            XmlReader xmlReader = XmlReader.Create(strReader);
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            DataTable datas = serializer.Deserialize(xmlReader) as DataTable;
            foreach (DataRow row in datas.Rows)
            {
                colection.Add(this.m_TableDal.Reader(row));
            }
        }

        public override void Save(Collection<T> colection)
        {
            DataTable cloneTable = this.m_TableDal.TableStructure.Clone();
            foreach (var tempT in colection)
            {
                var dicValues = this.m_TableDal.Writer(tempT);
                DataRow row = cloneTable.NewRow();
                foreach (var dicValue in dicValues)
                {
                    row[dicValue.Key] = dicValue.Value;
                }
                //cloneTable.ImportRow(row);
                cloneTable.Rows.Add(row);
            }
            //必须写名字
            cloneTable.TableName = "Serializer";
            StringBuilder strData = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(strData);
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            serializer.Serialize(writer, cloneTable);
            writer.Close();
            m_DataStore.SaveString(strData.ToString());
        }
    }
}