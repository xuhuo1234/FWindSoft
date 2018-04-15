using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FWindSoft.Revit.Table
{
    /// <summary>
    /// TSheet数据标识属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public sealed class TSheetFileAttribute : Attribute
    {
        public TSheetFileAttribute(string fileName)
        {
            this.FileName = fileName;
        }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string FileName { get; private set; }
    }
    #region 表格使用类
      /// <summary>
    /// 行设置
    /// </summary>
    public class WordSetting
    {
        /// <summary>
        /// 行高
        /// </summary>
        public double RowHeight { get; set; }
        /// <summary>
        /// 字高
        /// </summary>
        public double WordHeight { get; set; }
    }
    /// <summary>
    /// 列设置
    /// </summary>
    public class ColumnSetting
    {
        public ColumnSetting()
        {
        }

        public ColumnSetting(string content)
        {
            Name = content;
            Display = content;
            TextAlignment=TextAlignment.Middle;
            IsShow = true;
        }
        /// <summary>
        /// 列排序索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 列显示内容
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 列头显示名称
        /// </summary>
        public string Display { get; set; }
        /// <summary>
        /// 列宽
        /// </summary>
        public double ColomnWidth { get; set; }
        /// <summary>
        /// 文本对齐
        /// </summary>
        public TextAlignment TextAlignment { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; }

        public void SetValue(ColumnSetting setting)
        {
            Index = setting.Index;
            Display = setting.Display;
            ColomnWidth = setting.ColomnWidth;
            TextAlignment = setting.TextAlignment;
            IsShow = setting.IsShow;
        }
    }
    /// <summary>
    /// 内容排版
    /// </summary>
    public enum ContentArragne
    {
        [Description("横向排列")]
        Landscape,
        [Description("纵向排列")]
        Portrait
    }
    /// <summary>
    /// 文字对齐方式
    /// </summary>
    public enum TextAlignment
    {
        [Description("左对齐")]
        Left,
        [Description("居中")]
        Middle,
        [Description("右对齐")]
        Right,
    }
    #endregion

    /// <summary>
    /// 表格设置
    /// </summary>
    public  class TSheet
    {
        /// <summary>
        /// 构造函数不能私有化，是因为需要序列化和反序列化
        /// </summary>
        public TSheet()
        {
            this.FontString = "宋体";
            this.IsBottom = false;
            this.IsShowTitleName = false;
            this.TitleName = string.Format("表格名称");
            this.TitleWordSetting = new WordSetting() { RowHeight = 8, WordHeight = 5 };
            this.ColumnSettings = new List<ColumnSetting>();
          
            this.AutoFit = false;

            this.SheetHeaderWordSetting = new WordSetting() { RowHeight = 5, WordHeight = 3.5 };
            this.SheetRowWordSetting = new WordSetting() { RowHeight = 5, WordHeight = 3.5 };
            this.IsUsePagination = false;
            this.Arrange = ContentArragne.Landscape;
            this.RowCountPrePage = 5;
        }

       #region 属性控制维护
      
		 /// <summary>
        /// 表格字体
        /// </summary>
        public string FontString { get; set; }
        /// <summary>
        /// 表头是否置底
        /// </summary>
        public bool IsBottom{get;set;}

            /*
         * 为了分层明确，也为了更适合通用型，属于mode层的类尽量做到能不做属性通知就不做属性通知
         */
        #region 标题内容
        
        #endregion
       
        #region 标题内容
        /// <summary>
        /// 是否显示标题名字
        /// </summary>
        public bool IsShowTitleName { get; set; }
        /// <summary>
        /// 标题名称
        /// </summary>
        public string TitleName { get; set; }
        /// <summary>
        /// 标题文字设置
        /// </summary>
        public WordSetting TitleWordSetting { get; set; }

        #endregion

        #region 列设置
        /// <summary>
        /// 列设置
        /// </summary>
        public List<ColumnSetting> ColumnSettings { get;private set; }

        /// <summary>
        /// 列宽自适应
        /// </summary>
        public bool AutoFit { get; set; }

        #endregion

        #region 表头文字设置
        /// <summary>
        /// 表头文字设置
        /// </summary>
        public WordSetting SheetHeaderWordSetting { get; set; }
        #endregion

        #region 表格文字设置
        /// <summary>
        /// 表格文字设置
        /// </summary>
        public WordSetting SheetRowWordSetting { get; set; }
        #endregion

        #region 分页设置
        /// <summary>
        /// 是否使用分页
        /// </summary>
        public bool IsUsePagination { get; set; }
        /// <summary>
        /// 每页行数
        /// </summary>
        public int RowCountPrePage { get; set; }
        /// <summary>
        /// 内容排版格式
        /// </summary>
        public ContentArragne Arrange { get; set; }

        #endregion 
        #region 控制序号
        /// <summary>
        /// 是否动态生成序号
        /// </summary>
        public bool AutoBuildNo { get; set; }
        /// <summary>
        /// 序号列名称
        /// </summary>
        public string NoColumnName { get; set; }
        #endregion
	#endregion

        /// <summary>
        /// 添加列信息
        /// </summary>
        /// <param name="column"></param>
        public void AddColumns(ColumnSetting column)
        {
            if (column == null)
                return;
            this.ColumnSettings.Add(column);
        }

        public void Draw(ITSheetContext context)
        {
            context.Draw(this);
        }

        #region 取数据
        public static List<Dictionary<string, object>> GetSheetData(object o)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            ITSheetData getData = o as ITSheetData;
            if (getData != null)
            {
                result.AddRange(getData.CreateSheetData());
                return result;
            }
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Type type = o.GetType();
            var properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                try
                {
                    if ((property.PropertyType.IsClass && property.PropertyType != typeof(string)) || property.PropertyType.IsAbstract)
                        continue;
                    TSheetFileAttribute[] attrs =
                            property.GetCustomAttributes(typeof(TSheetFileAttribute), true) as TSheetFileAttribute[];
                    if (attrs.Any())
                    {
                        dic[attrs[0].FileName] = property.GetValue(o);
                    }
                }
                catch (Exception)
                {
                }
            }
            result.Add(dic);
            return result;
        }
        /// <summary>
        /// 单一类型集合，如果Object类型不一致，请手动使用GetSheetData(object o)
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> GetSheetData(List<object> objects)
        {

            List<Dictionary<string, object>> dic = new List<Dictionary<string, object>>();
            if (!objects.Any())
                return dic;
            Type type = objects[0].GetType();
            var properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            Dictionary<string, PropertyInfo> pros = new Dictionary<string, PropertyInfo>();
            foreach (var property in properties)
            {
                try
                {
                    if ((property.PropertyType.IsClass&&property.PropertyType!=typeof(string))|| property.PropertyType.IsAbstract)
                        continue;
                    TSheetFileAttribute[] attrs =
                            property.GetCustomAttributes(typeof(TSheetFileAttribute), true) as TSheetFileAttribute[];
                    if (attrs.Any())
                    {
                        pros[attrs[0].FileName] = property;
                    }
                }
                catch (Exception)
                {
                }
            }
            for (int i = 0; i < objects.Count; i++)
            {
                object tempObject = objects[i];
                ITSheetData iTSheet = tempObject as ITSheetData;
                if (iTSheet != null)
                {
                    dic.AddRange(iTSheet.CreateSheetData());
                    continue;
                }
                Dictionary<string, object> tempDicObject = new Dictionary<string, object>();
                foreach (var propertyInfo in pros)
                {
                    tempDicObject.Add(propertyInfo.Key, propertyInfo.Value.GetValue(tempObject));
                }
                dic.Add(tempDicObject);
            }
            return dic;
        }

        public static int GetRowCount(object o)
        {
            //其他情况都是1，继承自ITSheetData时自动获取
            //ITSheetData tData = o as ITSheetData;
            return GetSheetData(o).Count;

        } 
        #endregion
    }


    public class TCell
    {
        public TCell()
        {
            this.RowSpan = 1;
            this.ColumnSpan = 1;
        }

        /// <summary>
        /// 列索引
        /// </summary>
        public int ColumnIndex { get; set; }
        
        /// <summary>
        ///行索引
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 行跨度
        /// </summary>
        public int RowSpan { get; set; }
        /// <summary>
        /// 列跨度
        /// </summary>
        public int ColumnSpan { get; set; }

        /// <summary>
        /// 单元格值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 标签扩展
        /// </summary>
        public object Tag { get; set; }
    }

    /// <summary>
    /// 绘图逻辑
    /// </summary>
    public interface ITSheetContext
    {
        void Draw(TSheet sheet);
    }

    /// <summary>
    /// 获取表格数据
    /// </summary>
    public interface ITSheetData
    {
        /// <summary>
        /// 创建表格数据
        /// </summary>
        /// <returns></returns>
        List<Dictionary<string, object>> CreateSheetData();
    }

}
