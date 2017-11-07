using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace OBR.DotNet.ConfigSetting
{
    /// <summary>
    /// 自定义设置文件加载读取
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CustomSettingLS<T> where T :  new()
    {
        protected IDataStore m_DataStore;
        public CustomSettingLS(IDataStore dataStore)
        {
            this.m_DataStore = dataStore;
        }

        public abstract void Load(Collection<T> colection);
        public abstract void Save(Collection<T> colection);
    }

    public interface IDataStore
    {
        string GetString();
        bool SaveString(string strData);
    }

    public class FileDataStore : IDataStore
    {
        private readonly string m_FileName;

        public FileDataStore(string fileName)
        {
            this.m_FileName = fileName;

        }
        public string FileName
        {
            get { return this.m_FileName; }
        }

        #region 实现接口
        public string GetString()
        {
            if (!File.Exists(this.m_FileName))
                return "";
            FileInfo file = new FileInfo(this.m_FileName);
            StringBuilder strData=new StringBuilder();           
            using (StreamReader reader=file.OpenText())
            {
                strData.Append(reader.ReadToEnd());
            }
            return strData.ToString();
        }

        public bool SaveString(string strData)
        {
            string strDir = Path.GetDirectoryName(this.m_FileName);
            if (strDir == null)
                return false;
            if (!Directory.Exists(strDir))
            {
                Directory.CreateDirectory(strDir);
            }

            byte[] buffer = UnicodeEncoding.UTF8.GetBytes(strData);
            using (FileStream fileStream = File.Create(this.m_FileName, buffer.Count()))
            {
                fileStream.Position = 0;
                fileStream.Write(buffer,0,buffer.Count());
            }
            return true;
        } 
        #endregion
    }


    public class DelegateDataStore : IDataStore
    {
        private Func<string> m_Load;
        private Predicate<string> m_Save;

        public DelegateDataStore(Func<string> load, Predicate<string> save)
        {
            m_Load = load;
            m_Save = save;
        }

        public string GetString()
        {
            Func<string> tempLoad = this.m_Load;
            if (tempLoad == null)
                return "";
            return tempLoad();
        }

        public bool SaveString(string strData)
        {
            Predicate<string> tempSave = this.m_Save;
            if (tempSave == null)
                return false;
            return tempSave(strData);
        }
    }
}
