using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;

namespace DiaryJournal.Net
{
    public partial class FormNodeList : Form
    {
        public List<myNode> allNodes = new List<myNode>();
        public List<NodeType> nodeTypes = new List<NodeType>();
        public bool checkMultipleNodes = true;
        public bool listDeletedNodes = false;

        public List<myNode> outCheckedNodes = new List<myNode>();
        public myNode? outSelectedNode = null;

        public FormNodeList()
        {
            InitializeComponent();
        }

        private void FormNodeList_Load(object sender, EventArgs e)
        {
            lvNodeList.MultiSelect = checkMultipleNodes;
            lvNodeList.CheckBoxes = checkMultipleNodes;
            buttonAll.Enabled = buttonNone.Enabled = checkMultipleNodes;

            refillList();
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvNodeList.CheckedItems)
            {
                myNode? node = (myNode?)listViewItem.Tag;
                if (node == null) continue;
                outCheckedNodes.Add(node);
            }

            if (lvNodeList.SelectedItems.Count > 0) 
            {
                myNode? node = (myNode?)lvNodeList.SelectedItems[0].Tag;
                outSelectedNode = node;
            }
        }

        public void refillList()
        {
            lvNodeList.Items.Clear();

            foreach (myNode? listedNode in allNodes)
            {
                myNode? node = listedNode;
                if (node == null) continue;

                foreach (NodeType? type in nodeTypes)
                {
                    if (type == null) continue;

                    // check if this node is required, if not then skip
                    if (type != NodeType.AnyOrAll)
                    {
                        // a node type is exclusively given, so the node must be the same node type, else skip this node
                        if (node.chapter.nodeType != type)
                            continue;
                    }

                    // if deleted nodes are not wanted then skip them
                    if ((!listDeletedNodes) && (node.chapter.IsDeleted))
                        continue;

                    // node is required by node type, so add it in listview control
                    System.Windows.Forms.ListViewItem item = new System.Windows.Forms.ListViewItem();
                    item.Name = node.chapter.Id.ToString();
                    item.Tag = node;

                    String entryPath = "";
                    List<myNode>? lineage = null;
                    if (entryMethods.GenerateLineagePath(ref node.lineage, ref node, out entryPath, out lineage))
                        item.Text = entryPath;
                    else
                        item.Text = "[path not found]";

                    // dates
                    String chapterDateTime = node.chapter.chapterDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
                    item.SubItems.Add(chapterDateTime);
                    item.SubItems.Add(node.chapter.creationDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
                    item.SubItems.Add(node.chapter.modificationDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
                    item.SubItems.Add(node.chapter.deletionDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss"));

                    // deleted status
                    if (node.chapter.IsDeleted)
                        item.SubItems.Add("trash can");
                    else
                        item.SubItems.Add("common");

                    // special node type
                    item.SubItems.Add(node.chapter.specialNodeType.ToString());

                    // node type
                    item.SubItems.Add(node.chapter.nodeType.ToString());

                    // node's parent id
                    item.SubItems.Add(node.chapter.parentId.ToString());

                    // node's id
                    item.SubItems.Add(node.chapter.Id.ToString());

                    // other details
                    item.SubItems.Add(node.chapter.Title);
                    lvNodeList.Items.Add(item);
                }
            }

        }

        private void buttonAll_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvNodeList.Items)
            {
                listViewItem.Checked = true;
                listViewItem.Selected = true;
            }
        }

        private void buttonNone_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvNodeList.Items)
            {
                listViewItem.Checked = false;
                listViewItem.Selected = false;
            }
        }
    }
}
