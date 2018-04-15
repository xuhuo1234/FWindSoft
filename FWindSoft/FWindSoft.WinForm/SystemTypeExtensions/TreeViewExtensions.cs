using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace FWindSoft.WinForm
{
    public static class TreeViewExtensions
    {
        /// <summary>
        /// 获取指定节点的叶子节点
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns></returns>
        public static List<TreeNode> GetLeaves(this TreeNode treeNode)
        {
            List<TreeNode> result = new List<TreeNode>();
            List<TreeNode> nodes = new List<TreeNode>() { treeNode };

            for (int i = 0; i < nodes.Count; i++)
            {
                TreeNode tempNode = nodes[i];
                if (tempNode.Nodes.Count > 0)
                {
                    nodes.AddRange(tempNode.Nodes.OfType<TreeNode>().ToList());
                    continue;
                }
                else
                {
                    result.Add(tempNode);
                }
            }
            return result;
        }
        /// <summary>
        /// 获取指定节点的叶子节点
        /// </summary>
        /// <param name="treeView"></param>
        /// <returns></returns>
        public static List<TreeNode> GetLeaves(this TreeView treeView)
        {
            List<TreeNode> result = new List<TreeNode>();
            List<TreeNode> nodes = new List<TreeNode>(treeView.Nodes.OfType<TreeNode>().ToList());
            for (int i = 0; i < nodes.Count; i++)
            {
                TreeNode tempNode = nodes[i];
                if (tempNode.Nodes.Count > 0)
                {
                    nodes.AddRange(tempNode.Nodes.OfType<TreeNode>().ToList());
                    continue;
                }
                else
                {
                    result.Add(tempNode);
                }
            }
            return result;
        }
    }
}
