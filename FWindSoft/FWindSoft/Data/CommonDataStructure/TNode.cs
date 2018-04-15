
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FWindSoft.Data
{
    public class TNode
    {
        public TNode()
        {
            Nodes = new List<TNode>();
        }
        public TNode Parent { get; internal set; }
        /// <summary>
        /// 节点名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public List<TNode> Nodes { get; private set; }
        /// <summary>
        /// 节点关联信息
        /// </summary>
        public object Tag { get; set; }
        /// <summary>
        /// 是不是叶子节点
        /// </summary>
        public bool IsLeaf { get { return this.Nodes.Count == 0; } }

        public void AddChild(TNode node)
        {
            this.Nodes.Add(node);
        }
        public void AddChildren(List<TNode> nodes)
        {
            this.Nodes.AddRange(nodes);
        }
        /// <summary>
        /// 深度
        /// </summary>
        public int Depth
        {
            get
            {
                return GetDepth(this, 1);
            }
        }
        /// <summary>
        /// 获取node在竖向类别中的级别
        /// </summary>
        public int Level
        {
            get
            {
                int tempLevel = 1;
                for (TNode parent = this.Parent; parent != null; parent = parent.Parent)
                {
                    tempLevel++;
                }

                return tempLevel;
            }
        }
        /// <summary>
        /// 叶子的数量
        /// </summary>
        public int LeafCount
        {
            get
            {
                int count = 0;
                List<TNode> nodes = new List<TNode>() { this };
                for (int i = 0; i < nodes.Count; i++)
                {
                    TNode tempNode = nodes[i];
                    if (tempNode.IsLeaf)
                    {
                        count++;
                    }
                    else
                    {
                        nodes.AddRange(tempNode.Nodes);
                    }
                }
                return count;
            }
        }
        /// <summary>
        /// 获取指定节点的深度
        /// </summary>
        /// <param name="node"></param>
        /// <param name="currentDepth"></param>
        /// <returns></returns>
        private int GetDepth(TNode node, int currentDepth)
        {
            int depth = currentDepth;
            if (node.IsLeaf)
                return depth;
            foreach (TNode item in node.Nodes)
            {
                int tempdepth = GetDepth(item, currentDepth++);
                depth = Math.Max(tempdepth, depth);
            }
            return depth;
        }
        /// <summary>
        /// 获取所有叶子节点
        /// </summary>
        /// <returns></returns>
        public List<TNode> GetLeaves()
        {
            return GetLeaves(new List<TNode>() { this });
        }
        public static List<TNode> GetLeaves(List<TNode> nodes)
        {
            List<TNode> leaves = new List<TNode>();
            for (int i = 0; i < nodes.Count; i++)
            {
                TNode tempNode = nodes[i];
                if (tempNode.IsLeaf)
                {
                    leaves.Add(tempNode);
                }
                else
                {
                    leaves.AddRange(GetLeaves(tempNode.Nodes));
                }
            }
            return leaves;
        }
    } 
}
