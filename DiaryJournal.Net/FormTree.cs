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

        public delegate void __treeViewBeginUpdateDelegate(TreeView tv, bool clear);
        public __treeViewBeginUpdateDelegate treeViewBeginUpdate;
        public delegate void __treeViewEndUpdateDelegate(TreeView tv);
        public __treeViewEndUpdateDelegate treeViewEndUpdate;
        public delegate List<myNode> __loadTreeDelegate(ref List<myNode> srcNodes);
        public __loadTreeDelegate loadTree;
        public delegate void __initTreeViewRootNodeDelegate(TreeNode? node);
        public __initTreeViewRootNodeDelegate initTreeViewRootNode;

        public FormTree()
        {
            InitializeComponent();
        }

        private void FormTree_Load(object sender, EventArgs e)
        {
            treeViewBeginUpdate = new __treeViewBeginUpdateDelegate(__treeViewBeginUpdate);
            treeViewEndUpdate = new __treeViewEndUpdateDelegate(__treeViewEndUpdate);
            initTreeViewRootNode = new __initTreeViewRootNodeDelegate(__initTreeViewRootNode);
            loadTree = new __loadTreeDelegate(__loadTree);

            // setup treeview
            tvEntries.ImageList = new ImageList();
            tvEntries.ImageList.Images.Add(DiaryJournal.Net.Properties.Resources.text_file_7);
            tvEntries.ImageList.Images.Add(DiaryJournal.Net.Properties.Resources.closed_book_1);
            tvEntries.ImageList.Images.Add(DiaryJournal.Net.Properties.Resources.opened_book_1);
            tvEntries.ImageList.Images.Add(DiaryJournal.Net.Properties.Resources.calendar_node_2);
            tvEntries.ImageList.Images.Add(DiaryJournal.Net.Properties.Resources.set_node_1);
            tvEntries.ImageList.Images.Add(DiaryJournal.Net.Properties.Resources.label_node_1);

            __loadEntries();
        }
        public void __loadEntries()
        {
            this.Enabled = false;

            // load entire tree
            List<myNode> worklist = allNodes;
            allNodes = (List<myNode>)this.Invoke(loadTree, worklist);

            this.Enabled = true;
        }
        public void __treeViewBeginUpdate(TreeView tv, bool clear)
        {
            tv.BeginUpdate();
            tv.SuspendLayout();
            tv.Nodes.Clear();
        }
        public void __treeViewEndUpdate(TreeView tv)
        {
            tv.EndUpdate();
            tv.ResumeLayout();
        }
        // this method sets up the prepared root node in the treeview
        public void __initTreeViewRootNode(TreeNode? node)
        {
            tvEntries.Nodes.Add(node);
        }
        public List<myNode> __loadTree(ref List<myNode> srcNodes)
        {
            this.Invoke(treeViewBeginUpdate, tvEntries, true);

            // build tree
            List<myNode> outputTree = new List<myNode>();
            List<TreeNode> tree = entryMethods.buildTreeViewTree(ref srcNodes, ref outputTree, true, false, null, false);
            srcNodes = outputTree;

            // load all tree 
            foreach (TreeNode node in tree)
                this.Invoke(initTreeViewRootNode, node);

            this.Invoke(treeViewEndUpdate, tvEntries);
            return outputTree;
        }
    }
}
