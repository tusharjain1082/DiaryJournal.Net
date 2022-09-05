﻿using System;
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
        public delegate void __loadTreeDelegate(ref List<myNode> srcNodes);
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

            __loadEntries();
        }
        public void __loadEntries()
        {
            this.Enabled = false;

            // load entire tree
            this.Invoke(loadTree, allNodes);

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
        public void __loadTree(ref List<myNode> srcNodes)
        {
            this.Invoke(treeViewBeginUpdate, tvEntries, true);

            // build tree
            List<TreeNode> tree = entryMethods.buildTreeViewTree(ref srcNodes, true, false, null, false);

            // load all tree 
            foreach (TreeNode node in tree)
                this.Invoke(initTreeViewRootNode, node);

            this.Invoke(treeViewEndUpdate, tvEntries);
        }
    }
}
