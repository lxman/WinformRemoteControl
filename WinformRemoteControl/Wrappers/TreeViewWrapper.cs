using System;
using System.Linq;
using System.Windows.Forms;

namespace WinformRemoteControl.Wrappers
{
    public class TreeViewWrapper : IControlWrapper
    {
        public Guid Identifier { get; }
        
        public string Name { get; }
        
        public Control Control { get; private set; }
        
        private TreeView TreeView { get; set; }

        public TreeViewWrapper(TreeView tv)
        {
            Name = tv.Name;
            Control = tv;
            TreeView = tv;
        }

        public void NodeSelectByText(string text)
        {
            if (TreeView.InvokeRequired)
            {
                TreeView.Invoke(new Action(() => SelectNode(text)));
            }
            else SelectNode(text);
        }

        private void SelectNode(string text)
        {
            TreeView.Select();
            TreeNode node = TreeView.FlattenTree().FirstOrDefault(n => n.Text == text);
            if (!(node is null))
            {
                TreeView.Focus();
                TreeView.CollapseAll();
                TreeView.SelectedNode = node;
            }
        }
        
        public void Dispose()
        {
            Control = null;
            TreeView = null;
        }
    }
}