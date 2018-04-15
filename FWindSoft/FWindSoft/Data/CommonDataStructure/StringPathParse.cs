using System;
using System.Collections.Generic;
using System.Linq;

namespace FWindSoft.Data
{
    public class StringPathParse
    {
        /// <summary>
        /// 解析属性结构
        /// </summary>
        /// <param name="listTuple"></param>
        /// <returns></returns>
        public static List<TNode> Parse(List<Tuple<string, object>> listTuple)
        {
            if (listTuple == null)
                throw new ArgumentNullException(string.Format("{0}不能为null",nameof(listTuple)));
            List<PathParse> parses = listTuple.Select(t => new PathParse(t.Item1) { RefObject = t.Item2 }).ToList();
            return Parse(parses);
        }
        public static List<TNode> Parse(List<string> listName)
        {
            if (listName == null)
                throw new ArgumentNullException(string.Format("{0}不能为null", nameof(listName)));
            List<PathParse> parses = listName.Select(t => new PathParse(t) { RefObject = null }).ToList();
            return Parse(parses);
        }
        #region 私有方法
        private static List<TNode> Parse(List<PathParse> items)
        {
            List<TNode> tNodes = new List<TNode>();
            for (int i = 0; i < items.Count; i++)
            {
                PathParse treeParse = items[i];
                if (string.IsNullOrEmpty(treeParse.Current))
                    continue;
                TNode currentNode = new TNode() { Name = treeParse.Current };
                tNodes.Add(currentNode);
                List<PathParse> subParses = new List<PathParse>();
                for (int j = i + 1; j < items.Count; j++)
                {
                    PathParse tempSubParse = items[j];
                    if (string.IsNullOrEmpty(tempSubParse.Current))
                        continue;
                    if (treeParse.Current != tempSubParse.Current)
                    {
                        break;
                    }
                    subParses.Add(tempSubParse);
                    i = j;
                }
                if (subParses.Count > 0)
                {
                    //类似递归，暂时不改了
                    subParses.Insert(0, treeParse);
                    subParses.ForEach(p => p.MoveNext());
                    var subNodes = Parse(subParses);
                    subNodes.ForEach(n => n.Parent = currentNode);
                    currentNode.AddChildren(subNodes);
                }
                else
                {
                    currentNode.Tag = treeParse.RefObject;
                }
            }
            return tNodes;
        } 
        #endregion
    }
    /// <summary>
    /// 路径解析相关类
    /// </summary>
    internal class PathParse
    {
        private int m_Index = 0;
        public PathParse(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(string.Format("{0}不能为空",nameof(path)));
            this.OriginalPath = path;
            this.Levles = OriginalPath.Split('-').ToList();
            this.Current = GetCurrent();
        }
        /// <summary>
        /// 原始路径
        /// </summary>
        public  string OriginalPath { get; private set; }
        /// <summary>
        /// 关联对象，可为null
        /// </summary>
        public object RefObject { get; set; }

        /// <summary>
        /// 路径级别集合
        /// </summary>
        internal List<string> Levles { get; private set; }
        /// <summary>
        /// 当前路径级别
        /// </summary>
        public string Current { get; private set; }
        #region 相关方法
        public void ReParse()
        {
            this.Levles = OriginalPath.Split('-').ToList();
        }
        private string GetCurrent()
        {
            return this.Levles.Count > m_Index ? this.Levles[m_Index] : "";
        }
        public void MoveNext()
        {
            this.m_Index++;
            this.Current = GetCurrent();
        } 
        #endregion

    }
}
