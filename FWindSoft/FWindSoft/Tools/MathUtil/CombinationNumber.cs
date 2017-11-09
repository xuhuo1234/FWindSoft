
using System.Collections.Generic;
using System.Linq;

namespace FWindSoft.Tools
{
    /// <summary>
    /// 组合数获取
    /// </summary>
    public class CombinationNumber
    {
        /// <summary>
        /// 获取指定不同字符串列表的组合数
        /// </summary>
        /// <param name="listData">需要操作的集合</param>
        /// <param name="count">组合条目数量</param>
        /// <returns></returns>
        public static List<List<string>> GetResults(List<string> listData, int count)
        {
            List<List<string>> finnalyResult = new List<List<string>>();
            List<string> temp = new List<string>();
            GetList(listData, count, ref temp,ref finnalyResult);
            return finnalyResult;
        }
        private static void GetList(List<string> listData, int count, ref List<string> tempResult,ref List<List<string>> finnalyResult)
        {
            if (listData.Count < count)
            {
                return;
            }
            if (count == 0)
            {
                List<string> listTemp = new List<string>();
                listTemp.AddRange(tempResult);
                if (!finnalyResult.Any(list => list.Intersect(listTemp).Count() == list.Count))
                {
                    finnalyResult.Add(listTemp);
                }
                return;
            }
            for (int i = 0; i < listData.Count; i++)
            {
                tempResult.Add(listData[i]);
                List<string> subList = new List<string>();
                subList.AddRange(listData.FindAll(d => !d.Equals(listData[i])));

                GetList(subList, (count - 1), ref tempResult,ref finnalyResult);
                tempResult.RemoveRange(tempResult.Count - 1, 1);
            }
        }
    }
}
