using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WinformRemoteControl.Wrappers
{
    public static class TreeViewExtensions
    {
        public static IEnumerable<TreeNode> FlattenTree(this TreeView tv)
        {
            return FlattenTree(tv.Nodes);
        }

        private static IEnumerable<TreeNode> FlattenTree(this TreeNodeCollection coll)
        {
            return coll.OfType<TreeNode>()
                .Concat(coll.OfType<TreeNode>()
                    .SelectMany(x => FlattenTree(x.Nodes)));
        }
    }
}