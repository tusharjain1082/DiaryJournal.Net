using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiaryJournal.Net
{
    public partial class FormTree : Form
    {
        public List<myNode> allNodes = new List<myNode>();

        public FormTree()
        {
            InitializeComponent();
        }

        private void FormTree_Load(object sender, EventArgs e)
        {
            __loadEntries();
        }
        public void __loadEntries()
        {
            this.Enabled = false;
            tvEntries.Nodes.Clear();
            tvEntries.BeginUpdate();

            foreach (myNode listedNode in allNodes)
            {
                myNode node = listedNode;

                if (node.chapter.IsDeleted)
                    continue;

                __initTreeViewEntry(node);
            }

            tvEntries.ExpandAll();
            tvEntries.EndUpdate();
            this.Enabled = true;
        }

        // this method sets up the node in the treeview
        public TreeNode? __initTreeViewEntry(myNode nodeEntry)
        {
            String entryName = "";

            if (nodeEntry.chapter.parentId == 0)
            {
                // this is root node because it has no parent. so insert it directly as the root.
                String path = String.Format(@"{0}", nodeEntry.chapter.Id);
                TreeNode[] matchingNodes = tvEntries.Nodes.Find(path, true);
                if (matchingNodes.Length == 0) // node does not exists, so create it
                {
                    entryName = entryMethods.getEntryLabel(nodeEntry);
                    TreeNode newNode = tvEntries.Nodes.Add(path, entryName);
                    newNode.Name = path;
                    loadNodeHighlight(newNode, ref nodeEntry);
                    return newNode;
                }
            }
            else
            {
                // this node has a parent. so first find parent, if it exists, then insert the node into it.
                String path = String.Format(@"{0}", nodeEntry.chapter.parentId);
                TreeNode[] parentNodes = tvEntries.Nodes.Find(path, true);
                if (parentNodes.Length > 0)
                {
                    // parent path found, check if this child node exists, else create new
                    path = String.Format(@"{0}", nodeEntry.chapter.Id);
                    TreeNode[] matchingNodes = parentNodes[0].Nodes.Find(path, true);
                    if (matchingNodes.Length == 0) // node does not exists, so create it
                    {
                        entryName = entryMethods.getEntryLabel(nodeEntry);
                        TreeNode newNode = parentNodes[0].Nodes.Add(path, entryName);
                        newNode.Name = path;
                        loadNodeHighlight(newNode, ref nodeEntry);
                        return newNode;
                    }
                }
            }
            return null;
        }
        public void loadNodeHighlight(TreeNode treeNode, ref myNode node)
        {
            if (node.chapter.HLFont.Length >= 1)
                treeNode.NodeFont = commonMethods.StringToFont(node.chapter.HLFont);

            if (node.chapter.HLFontColor.Length >= 1)
                treeNode.ForeColor = commonMethods.StringToColor(node.chapter.HLFontColor);

            if (node.chapter.HLBackColor.Length >= 1)
                treeNode.BackColor = commonMethods.StringToColor(node.chapter.HLBackColor);
        }

    }
}
