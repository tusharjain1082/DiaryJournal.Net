#define UNICODE

using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System;
using RtfPipe.Tokens;
//using MigraDoc.DocumentObjectModel;
//using PdfSharp.Drawing.BarCodes;
using System.Data;
using System.Reflection;
using System.Net;
using RtfPipe.Model;
using System.Drawing.Printing;
using System.Collections.Generic;
using System.Windows.Forms.Integration;
using Net.Sgoliver.NRtfTree.Core;
//using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;
using static DiaryJournal.Net.FindReplaceFramework;
using System.Xml.Linq;
using MigraDoc.DocumentObjectModel.Tables;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.IO.Packaging;
using PdfSharp.Drawing;

namespace DiaryJournal.Net
{

    public partial class FrmJournal : Form
    {

        public bool stateChanged = false;
        public textFormatting formatting = null;

        public String xamlState0 = "";
        public String xamlState1 = "";
        public String xamlState2 = "";
        public String xamlState3 = "";
        public String xamlState4 = "";
        public String xamlState5 = "";
        public String xamlState6 = "";
        public String xamlState7 = "";
        public String xamlState8 = "";
        public String xamlState9 = "";

        public String xamlState = "";
        public String previousXamlState = "";
        public FormFind? myFormFind = null;
        public MatchesForm? myMatchesForm = null;

        bool properExit = false;
        public List<myNode> allNodes = new List<myNode>();
        PrintRichTextBoxEx printerRtb = new PrintRichTextBoxEx();
        //      CoreManaged.CoreManaged cppCore = new CoreManaged.CoreManaged();

        public myConfig cfg = new myConfig();

        System.Windows.Controls.WpfRichTextBoxEx rtbEntry = new System.Windows.Controls.WpfRichTextBoxEx();
        System.Windows.Controls.PrintDialog pd = new System.Windows.Controls.PrintDialog();
        //        System.Windows.Controls.Viewbox viewboxPrimary = new System.Windows.Controls.Viewbox();
        ElementHost host1 = new ElementHost();
        ElementHost host2 = new ElementHost();

        // TreeView drag and drop config
        TreeNode? targetDDNode = null;

        // delegates
        public delegate void __toggleFormDelegate(bool toggle);
        public __toggleFormDelegate toggleForm;
        public delegate void __showMessageBoxDelegate(String text, String title, MessageBoxButtons buttons, MessageBoxIcon icon);
        public __showMessageBoxDelegate showMessageBox;
        public delegate void __setCalendarHighlightEntryDelegate(DateTime dateTime);
        public __setCalendarHighlightEntryDelegate setCalendarHighlightEntry;
        public delegate void __gotoEntryDelegate(DateTime dateTime);
        public __gotoEntryDelegate gotoEntry;
        public delegate void __gotoTodaysEntryDelegate();
        public __gotoTodaysEntryDelegate gotoTodaysEntry;
        public delegate void __updateTotalEntriesStatusDelegate(long totalEntries);
        public __updateTotalEntriesStatusDelegate updateTotalEntriesStatus;
        public delegate void __resetTrashCanDelegate();
        public __resetTrashCanDelegate resetTrashCan;
        public delegate void __insertLvTrashCanItemDelegate(ref myNode node);
        public __insertLvTrashCanItemDelegate insertLvTrashCanItem;
        public delegate void __treeViewBeginUpdateDelegate(TreeView tv, bool clear);
        public __treeViewBeginUpdateDelegate treeViewBeginUpdate;
        public delegate void __treeViewEndUpdateDelegate(TreeView tv);
        public __treeViewEndUpdateDelegate treeViewEndUpdate;
        public delegate void __LvTrashCanUpdateDelegate(bool set);
        public __LvTrashCanUpdateDelegate LvTrashCanUpdate;
        public delegate void __resetLVSearchDelegate();
        public __resetLVSearchDelegate resetLVSearch;
        public delegate void __LvSearchUpdateDelegate(bool set);
        public __LvSearchUpdateDelegate LvSearchUpdate;
        public delegate bool __processSearchDelegate();
        public __processSearchDelegate processSearch;
        public delegate void __loadEntriesDelegate();
        public __loadEntriesDelegate loadEntries;
        public delegate void __saveEntryDeletage();
        public __saveEntryDeletage saveEntry;
        public delegate TreeNode? __initTreeViewEntryDelegate(ref myNode nodeEntry);
        public __initTreeViewEntryDelegate initTreeViewEntry;
        public delegate void __loadRootToBottomNodesDelegate(ref List<myNode> nodes, bool reverse = true);
        public __loadRootToBottomNodesDelegate loadRootToBottomNodes;
        public delegate List<myNode> __getHighestCheckedTreeViewItemsDBNodesDelegate(TreeView? tv);
        public __getHighestCheckedTreeViewItemsDBNodesDelegate getHighestCheckedTreeViewItemsDBNodes;
        public delegate void __initTreeViewRootNodeDelegate(TreeNode? node);
        public __initTreeViewRootNodeDelegate initTreeViewRootNode;
        public delegate List<myNode> __loadTreeDelegate(ref List<myNode> srcNodes);
        public __loadTreeDelegate loadTree;
        public delegate void __hideFormDelegate(bool toggle);
        public __hideFormDelegate hideForm;
        public delegate void __showFormDelegate();
        public __showFormDelegate showForm;
        public delegate void __updateSearchProgressPathDelegate(String path);
        public __updateSearchProgressPathDelegate updateSearchProgressPath;
        public delegate void __gotoEntryByAttributeDelegate(bool lm, bool lc, bool tc, bool byID, Int64 id);
        public __gotoEntryByAttributeDelegate gotoEntryByAttribute;


        public FrmJournal()
        {
            InitializeComponent();
        }

        private void FrmJournal_Load(object sender, EventArgs e)
        {
            // Add a reference to the NuGet package System.Text.Encoding.CodePages for .Net core only
            // important initialization for RtfPipe Library:
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // initialize all formatting config
            formatting = new textFormatting();

            //toolStripContainer2.ContentPanel.Size = new Size(10000, 10000);

            host1.Dock = DockStyle.Fill;
            rtbEntry.BitmapEffect = null;
            rtbEntry.BitmapEffectInput = null;
            host1.AutoSize = true;
            host1.Child = rtbEntry;
            rtbEntry.HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
            rtbEntry.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;

            //host1.Size = new Size(10000, 10000);
            //rtbEntry.Height = 10000;
            //rtbEntry.Width = 10000;
            toolStripContainer2.ContentPanel.Controls.Add(host1);

            formatting.formatLineSpacing(rtbEntry, 0.5);
            formatting.formatLineSpacing(rtbEntry.dummy, 0.5);

            // this setting stops empty strings to be set to null
            LiteDB.BsonMapper.Global.EmptyStringToNull = false;

            rtbEntry.TextChanged += rtbEntry_TextChanged;
            rtbEntry.SelectionChanged += RtbEntry_SelectionChanged;
            tvEntries.BeforeSelect += TvEntries_BeforeSelect;
            tvEntries.AfterCheck += TvEntries_AfterCheck;
            CalendarEntries.DateSelected += CalendarEntries_DateSelected;
            tabControlJournal.Selected += TabControlJournal_Selected;
            cmbFonts.SelectedIndexChanged += CmbFonts_SelectedIndexChanged;
            cmbSize.SelectedIndexChanged += CmbSize_SelectedIndexChanged;
            lvSearch.DoubleClick += LvSearch_DoubleClick;
            lvTrashCan.DoubleClick += LvTrashCan_DoubleClick;
            this.Shown += FrmJournal_Shown;
            this.FormClosing += FrmJournal_FormClosing;
            cmbSize.DropDownClosed += CmbSize_DropDownClosed;
            cmbFonts.DropDownClosed += CmbFonts_DropDownClosed;

            cmbFonts.MouseUp += CmbFonts_MouseUp;
            cmbSize.MouseUp += CmbSize_MouseUp;

            // Add event handlers for the required drag events.
            tvEntries.ItemDrag += TvEntries_ItemDrag;
            tvEntries.DragEnter += TvEntries_DragEnter;
            tvEntries.DragOver += TvEntries_DragOver;
            tvEntries.DragDrop += TvEntries_DragDrop;
            tvEntries.DragLeave += TvEntries_DragLeave;
            // setup treeview
            tvEntries.ImageList = new ImageList();
            tvEntries.ImageList.ImageSize = new Size(12, 12);
            tvEntries.Font = cfg.tvEntriesFont;// new System.Drawing.Font("Arial", 8, FontStyle.Regular);
            tvEntries.ImageList.Images.Add(DiaryJournal.Net.Properties.Resources.text_file_7);
            tvEntries.ImageList.Images.Add(DiaryJournal.Net.Properties.Resources.closed_book_1);
            tvEntries.ImageList.Images.Add(DiaryJournal.Net.Properties.Resources.opened_book_1);
            tvEntries.ImageList.Images.Add(DiaryJournal.Net.Properties.Resources.calendar_node_2);
            tvEntries.ImageList.Images.Add(DiaryJournal.Net.Properties.Resources.set_node_1);
            tvEntries.ImageList.Images.Add(DiaryJournal.Net.Properties.Resources.label_node_1);


            String strDateTimeTemplate = DiaryJournal.Net.Properties.Resources.BuildDateTime;
            DateTime buildDateTime = DateTime.Parse(strDateTimeTemplate);
            String strBuildDateTime = buildDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            this.Text = "Tushar Jain's " + this.Text + " Version " + Application.ProductVersion + ", uses .Net 8.0, Compiled/Built on: " + strBuildDateTime;

            for (int size = 6; size <= 300; size++)
                cmbSize.Items.Add(size);

            // setup delegates
            toggleForm = new __toggleFormDelegate(__toggleForm);
            showMessageBox = new __showMessageBoxDelegate(__showMessageBox);
            setCalendarHighlightEntry = new __setCalendarHighlightEntryDelegate(__setCalendarHighlightEntry);
            gotoEntry = new __gotoEntryDelegate(__gotoEntry);
            gotoTodaysEntry = new __gotoTodaysEntryDelegate(__gotoTodaysEntry);
            updateTotalEntriesStatus = new __updateTotalEntriesStatusDelegate(__updateTotalEntriesStatus);
            resetTrashCan = new __resetTrashCanDelegate(__resetTrashCan);
            insertLvTrashCanItem = new __insertLvTrashCanItemDelegate(__insertLvTrashCanItem);
            treeViewBeginUpdate = new __treeViewBeginUpdateDelegate(__treeViewBeginUpdate);
            treeViewEndUpdate = new __treeViewEndUpdateDelegate(__treeViewEndUpdate);
            LvTrashCanUpdate = new __LvTrashCanUpdateDelegate(__LvTrashCanUpdate);
            resetLVSearch = new __resetLVSearchDelegate(__resetLVSearch);
            LvSearchUpdate = new __LvSearchUpdateDelegate(__LvSearchUpdate);
            processSearch = new __processSearchDelegate(__processSearch);
            loadEntries = new __loadEntriesDelegate(__loadEntries);
            saveEntry = new __saveEntryDeletage(__saveEntry);
            initTreeViewEntry = new __initTreeViewEntryDelegate(__initTreeViewEntry);
            loadRootToBottomNodes = new __loadRootToBottomNodesDelegate(__loadRootToBottomNodes);
            getHighestCheckedTreeViewItemsDBNodes = new __getHighestCheckedTreeViewItemsDBNodesDelegate(__getHighestCheckedTreeViewItemsDBNodes);
            initTreeViewRootNode = new __initTreeViewRootNodeDelegate(__initTreeViewRootNode);
            loadTree = new __loadTreeDelegate(__loadTree);
            hideForm = new __hideFormDelegate(__hideForm);
            showForm = new __showFormDelegate(__showForm);
            updateSearchProgressPath = new __updateSearchProgressPathDelegate(__updateSearchProgressPath);
            gotoEntryByAttribute = new __gotoEntryByAttributeDelegate(__gotoEntryByAttribute);

            // setup config ui
            for (int itemHeight = 16; itemHeight <= 60; itemHeight++)
                cmbCfgTVEntriesItemHeight.Items.Add(itemHeight);

            for (int indent = 16; indent <= 60; indent++)
                cmbCfgTVEntriesIndent.Items.Add(indent);

            // now load config file and setup
            myConfigMethods.autoCreateLoadConfigFile(ref cfg, false);
            applyConfig();

            splitContainerEntryTree.Cursor = Cursors.Default;
            splitContainerH.Cursor = Cursors.Default;
            splitContainerV.Cursor = Cursors.Default;

            splitContainerSearch.Cursor = Cursors.Default;

            cmbFonts.Items.AddRange(formatting.fontNames.ToArray());

            // config
            resetRtb(rtbEntry);

            // listing all web colors in toolstrip drop down split buttons so that the user can select them.
            var webColors = commonMethods.getWebColors();// typeof(Color));
            foreach (System.Drawing.Color knownColor in webColors)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = knownColor.ToString();
                item.BackColor = knownColor; //Color.FromKnownColor(knownColor);
                item.Tag = knownColor;
                tssplitbuttonFontColors.DropDownItems.Add(item);
                item.Click += ToolStripFontColorMenuItem_Click;

                ToolStripMenuItem item2 = new ToolStripMenuItem();
                item2.Text = knownColor.ToString();
                item2.BackColor = knownColor; //Color.FromKnownColor(knownColor);
                item2.Tag = knownColor;
                tssplitbuttonBackColors.DropDownItems.Add(item2);
                item2.Click += ToolStripBackColorMenuItem_Click;
            }

            dtpickerSearchFrom.Value = DateTime.Now;
            dtpickerSearchThrough.Value = DateTime.Now;
            dtpickerSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerSearchThroughTime.Value = DateTime.Parse("23:59:59");
            dtpickerCDSearchFrom.Value = DateTime.Now;
            dtpickerCDSearchThrough.Value = DateTime.Now;
            dtpickerCDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerCDSearchThroughTime.Value = DateTime.Parse("23:59:59");
            dtpickerMDSearchFrom.Value = DateTime.Now;
            dtpickerMDSearchThrough.Value = DateTime.Now;
            dtpickerMDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerMDSearchThroughTime.Value = DateTime.Parse("23:59:59");
            dtpickerDDSearchFrom.Value = DateTime.Now;
            dtpickerDDSearchThrough.Value = DateTime.Now;
            dtpickerDDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerDDSearchThroughTime.Value = DateTime.Parse("23:59:59");

            // configure
            //rtbEntry.HideSelection = rtbViewEntry.HideSelection = false;

            // configure print document and printer stuff
            printerRtb.printDoc = pdRtbEntry;
            pdRtbEntry.BeginPrint += new PrintEventHandler(printerRtb.printDoc_BeginPrint);
            pdRtbEntry.PrintPage += new PrintPageEventHandler(printerRtb.printDoc_PrintPage);
            pdRtbEntry.EndPrint += new PrintEventHandler(printerRtb.printDoc_EndPrint);

        }


        private void CmbFonts_DropDownClosed(object? sender, EventArgs e)
        {
            ToolStripComboBox comboBox = (ToolStripComboBox)sender;
            if (comboBox.SelectedItem == null)
                return;

            formatting.formatFont(rtbEntry, (String)comboBox.SelectedItem);
        }

        private void CmbSize_MouseUp(object? sender, MouseEventArgs e)
        {
        }

        public void __updateSearchProgressPath(String path)
        {
            txtSearchProgressFullPath.Text = path;
            txtSearchProgressFullPath.Update();
            this.Update();

        }

        private void CmbFonts_MouseUp(object? sender, MouseEventArgs e)
        {
        }

        private void RtbEntry_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            tsbuttonUnderline.Checked = rtbEntry.Underline;
            tsbuttonStrikeout.Checked = rtbEntry.Strikeout;
            tsbuttonBold.Checked = rtbEntry.Bold;
            tsbuttonItalics.Checked = rtbEntry.Italic;

            int caretPosition = rtbEntry.SelectionStartOffset;
            int lineIndex = rtbEntry.LineIndex;
            tsslabelCaretPosition.Text = caretPosition.ToString();
            tsslabelLineIndex.Text = lineIndex.ToString();

            formatting.selStartIndex = rtbEntry.SelectionStartOffset;
            formatting.selLength = rtbEntry.SelectionLength;
            if (rtbEntry.SelectionFont != null)
            {
                int index = cmbFonts.FindString(rtbEntry.SelectionFont.Name);
                cmbFonts.SelectedIndex = index;
            }
            else
            {
                cmbFonts.SelectedItem = null;
            }

            if (rtbEntry.SelectionFont != null)
            {
                double doubleSelFontSize = rtbEntry.SelectionFontSize;
                int intSelFontSize = (int)doubleSelFontSize;
                cmbSize.SelectedIndex = cmbSize.FindString(intSelFontSize.ToString());
            }

            if (rtbEntry.SelectionAlignment == System.Windows.TextAlignment.Left)
                tsbuttonLeftJustify.Checked = true;
            else
                tsbuttonLeftJustify.Checked = false;

            if (rtbEntry.SelectionAlignment == System.Windows.TextAlignment.Right)
                tsbuttonRightJustify.Checked = true;
            else
                tsbuttonRightJustify.Checked = false;

            if (rtbEntry.SelectionAlignment == System.Windows.TextAlignment.Justify)
                tsbuttonJustify.Checked = true;
            else
                tsbuttonJustify.Checked = false;

            if (rtbEntry.SelectionAlignment == System.Windows.TextAlignment.Center)
                tsbuttonCenterJustify.Checked = true;
            else
                tsbuttonCenterJustify.Checked = false;

            if (rtbEntry.SelectionBullets)
                tsbuttonBullets.Checked = true;
            else
                tsbuttonBullets.Checked = false;

            if (rtbEntry.SelectionNumbering)
                tsbuttonNumberedList.Checked = true;
            else
                tsbuttonNumberedList.Checked = false;


            if (rtbEntry.SelectionIndent > 0)
            {
                tsbuttonIndentLeft.Checked = false;
                tsbuttonIndentRight.Checked = true;
            }
            else
            {
                tsbuttonIndentLeft.Checked = true;
                tsbuttonIndentRight.Checked = false;
            }
        }

        public void __showForm()
        {
            this.Activate();
            this.Focus();
            this.BringToFront();
            this.Show();
        }
        // Determine whether one node is a parent 
        // or ancestor of a second node.
        private bool ContainsNode(TreeNode node1, TreeNode node2)
        {
            // Check the parent node of the second node.
            if (node2 == null) return false;
            if (node2.Parent == null) return false;
            if (node2.Parent.Equals(node1)) return true;

            // If the parent node is not null or equal to the first node, 
            // call the ContainsNode method recursively using the parent of 
            // the second node.
            return ContainsNode(node1, node2.Parent);
        }

        private void TvEntries_DragDrop(object? sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the drop location.
            Point targetPoint = tvEntries.PointToClient(new Point(e.X, e.Y));

            if (targetDDNode != null)
            {
                targetDDNode.BackColor = tvEntries.BackColor;
                targetDDNode = null;
            }
            // Retrieve the node at the drop location.
            TreeNode targetTreeNode = tvEntries.GetNodeAt(targetPoint);
            if (targetTreeNode == null) return;

            // load node
            Int64 targetNodeID = Int64.Parse(targetTreeNode.Name);
            myNode? targetNode = entryMethods.FindNodeInList(ref allNodes, targetNodeID);
            if (targetNode == null) return;

            // Retrieve the node that was dragged.
            TreeNode draggedTreeNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
            if (draggedTreeNode == null) return;

            // load node
            Int64 draggedNodeID = Int64.Parse(draggedTreeNode.Name);
            myNode? draggedNode = entryMethods.FindNodeInList(ref allNodes, draggedNodeID);
            if (draggedNode == null) return;

            if (draggedNode.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            // Confirm that the node at the drop location is not 
            // the dragged node or a descendant of the dragged node.
            if (!draggedTreeNode.Equals(targetTreeNode) && !ContainsNode(draggedTreeNode, targetTreeNode))
            {
                // If it is a move operation, remove the node from its current 
                // location and add it to the node at the drop location.
                if (e.Effect == DragDropEffects.Move)
                {
                    if (!entryMethods.DBSetNodeParent(cfg, ref draggedNode, targetNode.chapter.Id))
                        return;

                    draggedTreeNode.Remove();
                    targetTreeNode.Nodes.Add(draggedTreeNode);

                    // rebuild lineage
                    draggedNode.lineage = entryMethods.findBottomToRootNodesRecursive(ref allNodes, ref draggedNode, false, false, true, false);
                }

                // If it is a copy operation, clone the dragged node 
                // and add it to the node at the drop location.
                // else if (e.Effect == DragDropEffects.Copy)
                //{
                //    targetNode.Nodes.Add((TreeNode)draggedNode.Clone());
                // }

                // Expand the node at the location 
                // to show the dropped node.
                targetTreeNode.Expand();
            }
        }

        private void TvEntries_DragLeave(object? sender, EventArgs e)
        {
            //            TreeNode? treeNode = (TreeNode?)sender;

            // Retrieve the client coordinates of the mouse position.
            //Point targetPoint = tvEntries.PointToClient(new Point(e.X, e.Y));

            // Select the node at the mouse position.
            //TreeNode? targetNode = tvEntries.GetNodeAt(targetPoint);
            //            treeNode.BackColor = SystemColors.Control; // This will work
        }

        private void TvEntries_DragOver(object? sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the drop location.
            Point targetPoint = tvEntries.PointToClient(new Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            TreeNode targetNode = tvEntries.GetNodeAt(targetPoint);

            // target node must be highlighted
            if (targetNode != null)
                targetNode.BackColor = SystemColors.Highlight;

            // reset highlight of previous node and set it to target node if it is not target node
            if (targetDDNode != targetNode)
            {
                if (targetDDNode != null)
                    targetDDNode.BackColor = tvEntries.BackColor;

                targetDDNode = targetNode;
            }
            e.Effect = e.AllowedEffect;
        }

        private void TvEntries_DragEnter(object? sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void TvEntries_ItemDrag(object? sender, ItemDragEventArgs e)
        {
            // Move the dragged node when the left mouse button is used.
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }

            // Copy the dragged node when the right mouse button is used.
            else if (e.Button == MouseButtons.Right)
            {
                DoDragDrop(e.Item, DragDropEffects.Copy);
            }
        }

        private void FrmJournal_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!properExit)
            {
                //                var res = MessageBox.Show(this, "you cannot directly close this form. please click on exit button to close the form.", "error",
                //                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                //e.Cancel = true;
            }
            else
            {
                //e.Cancel = false;
            }

            // first save the entry, then exit
            if (stateChanged)
            {
                if (MessageBox.Show(this, "do you wish to save the currently active changed entry?", "question",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    saveCloseDB();
            }

            try
            {
                reset();
            }
            catch { }

            try
            {
                List<Control>? list = ControlHelper.GetAllChildControls(this); //ControlHelper.GetAll(this, typeof(Control)))
                if (list != null)
                {
                    foreach (Control c in list)
                        c.Dispose();
                }
                this.Controls.Clear();
            }
            catch { }

            try
            {
                base.OnClosing(e);
            }
            catch { }

        }

        private void FrmJournal_Shown(object? sender, EventArgs e)
        {
            // take final actions as per the stored configuration
            if (cfg.chkCfgAutoLoadCreateDefaultDB)
                autoCreateLoadDefaultDB(false);
        }

        public void applyConfig()
        {
            chkCfgAutoLoadCreateDefaultDB.Checked = cfg.chkCfgAutoLoadCreateDefaultDB;
            chkCfgUseWinUserDocFolder.Checked = cfg.chkCfgUseWinUserDocFolder;

            rtbViewEntry.RightMargin = cfg.cmbCfgRtbViewEntryRMValue;
            int index = cmbCfgRtbViewEntryRM.FindString(rtbViewEntry.RightMargin.ToString());
            cmbCfgRtbViewEntryRM.SelectedIndex = index;
            radCfgUseOpenFileSystemDB.Checked = cfg.radCfgUseOpenFileSystemDB;
            radCfgUseSingleFileDB.Checked = cfg.radCfgUseSingleFileDB;
            radCfgLMNode.Checked = cfg.radCfgLMNode;
            radCfgLCNode.Checked = cfg.radCfgLCNode;
            radCfgTCNode.Checked = cfg.radCfgTCNode;

            if (cfg.tvEntriesItemHeight <= 0) cfg.tvEntriesItemHeight = myConfig.default_tvEntriesItemHeight;
            if (cfg.tvEntriesIndent <= 0) cfg.tvEntriesIndent = myConfig.default_tvEntriesIndent;

            tvEntries.ItemHeight = cfg.tvEntriesItemHeight;
            index = cmbCfgTVEntriesItemHeight.FindString(tvEntries.ItemHeight.ToString());
            if (index >= 0)
                cmbCfgTVEntriesItemHeight.SelectedIndex = index;
            else
                cmbCfgTVEntriesItemHeight.Text = index.ToString();

            tvEntries.Indent = cfg.tvEntriesIndent;
            index = cmbCfgTVEntriesItemHeight.FindString(tvEntries.Indent.ToString());
            if (index >= 0)
                cmbCfgTVEntriesIndent.SelectedIndex = index;
            else
                cmbCfgTVEntriesIndent.Text = index.ToString();

            if (cfg.tvEntriesFont == null) cfg.tvEntriesFont = myConfig.default_tvEntriesFont;
            tvEntries.Font = cfg.tvEntriesFont;
            linkCfgTVEntriesFont.Text = commonMethods.FontToString(cfg.tvEntriesFont);

            if (cfg.tvEntriesBackColor == Color.Empty) cfg.tvEntriesBackColor = myConfig.default_tvEntriesBackColor;
            if (cfg.tvEntriesForeColor == Color.Empty) cfg.tvEntriesForeColor = myConfig.default_tvEntriesForeColor;
            tvEntries.BackColor = cfg.tvEntriesBackColor;
            tvEntries.ForeColor = cfg.tvEntriesForeColor;
            linkCfgTVEntriesFont.BackColor = cfg.tvEntriesBackColor;
            linkCfgTVEntriesFont.ForeColor = cfg.tvEntriesForeColor;

        }

        private void LvTrashCan_DoubleClick(object? sender, EventArgs e)
        {
            viewSelectedTrashCanEntry();

        }

        public void viewEntry(myConfig cfg, ref myNode node)
        {
            String rtf = entryMethods.DBLoadNodeData(cfg, node.chapter.Id, node.DirectorySectionID);
            rtbViewEntry.Rtf = rtf;
            tabControlJournal.SelectedIndex = tabControlJournal.TabPages.IndexOfKey("TabPageViewEntry");

            // now setup caret config
            rtbViewEntry.Select(node.chapter.caretIndex, 0);
            if (node.chapter.caretSelectionLength != 0)
                rtbViewEntry.Select(node.chapter.caretIndex, node.chapter.caretSelectionLength);

            rtbViewEntry.ScrollToCaret();

        }
        public void OpenSearchedEntry(Int64 id, bool openParentNode = false)
        {
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            if (openParentNode)
            {
                node = entryMethods.FindNodeInList(ref allNodes, node.chapter.parentId);
                if (node == null)
                    return;
            }

            if (node.chapter.IsDeleted)
            {
                viewEntry(cfg, ref node);
            }
            else
            {
                tabControlJournal.SelectedIndex = 0;
                __gotoEntryById(node.chapter.Id);
            }
        }

        public void viewSelectedTrashCanEntry()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (lvTrashCan.SelectedItems.Count == 0)
                return;

            System.Windows.Forms.ListViewItem listViewItem = lvTrashCan.SelectedItems[0];
            if (listViewItem == null)
                return;

            Int64 id = Int64.Parse(listViewItem.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            viewEntry(cfg, ref node);
        }

        public long getSelectedListViewNodeId(System.Windows.Forms.ListView lv)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return -1;

            if (lv.SelectedItems.Count == 0)
                return -1;

            System.Windows.Forms.ListViewItem listViewItem = lv.SelectedItems[0];
            Int64 id = Int64.Parse(listViewItem.Name);

            return id;
        }

        private void LvSearch_DoubleClick(object? sender, EventArgs e)
        {
            Int64 id = getSelectedListViewNodeId(lvSearch);
            OpenSearchedEntry(id, false);
        }

        private void ToolStripFontColorMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            if (menuItem == null)
                return;

            System.Drawing.Color color = (System.Drawing.Color)menuItem.Tag;
            if (color == null)
                return;

            formatting.formatFontColor(rtbEntry, color);
        }

        private void ToolStripBackColorMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            if (menuItem == null)
                return;

            System.Drawing.Color color = (System.Drawing.Color)menuItem.Tag;
            if (color == null)
                return;

            formatting.formatBackColor(rtbEntry, color);
        }

        private void TvEntries_AfterCheck(object? sender, TreeViewEventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            TreeView tv = (TreeView)sender;
            if (tv.Nodes.Count == 0)
                return;

            bool set = e.Node.Checked;
            CheckTreeViewNodeRecursively(e.Node, set);
        }

        public void resetRtb(System.Windows.Controls.WpfRichTextBoxEx rtb, bool clear = true, bool resetSaveState = false)
        {

            // config
            if (clear)
            {
                // configure rtb document
                configureRtbDocument(rtbEntry, myConfig.defaultDocumentWidth, true);

                rtb.Rtf = "";
                rtb.SelectionFont = new System.Drawing.Font("Times New Roman", 24.0f, FontStyle.Regular);
            }

            if (resetSaveState)
            {
                stateChanged = false;
                tsslblStateChanged.Text = " ";
                tsbuttonSave.Checked = false;
                tsbuttonSave.BackColor = SystemColors.Control;
                tsslblStateChanged.BackColor = SystemColors.Control;

            }
        }

        private void CmbSize_SelectedIndexChanged(object? sender, EventArgs e)
        {
        }

        private void CmbFonts_SelectedIndexChanged(object? sender, EventArgs e)
        {
        }

        private void TabControlJournal_Selected(object? sender, TabControlEventArgs e)
        {
        }

        public void setupNowEntry()
        {
            setupNewEntry(DateTime.Now, 0);
        }

        public void setupNewEntry(DateTime dateTime, Int64 parentId = 0, NodeType nodeType = NodeType.EntryNode,
            String title = "")
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // initialize calender nodes
            myNode? yearNode = null;
            myNode? monthNode = null;

            // init calendar nodes
            mySystemNodes? systemNodes = null;
            entryMethods.initCalenderNodesSystem(cfg, ref allNodes, dateTime.Year, dateTime.Month, ref yearNode, ref monthNode, ref systemNodes);
            __loadSystemNodes(ref systemNodes);

            // validation
            myNode? parentNode = null;
            if (parentId > 0)
            {
                parentNode = entryMethods.FindNodeInList(ref allNodes, parentId);
                if (parentNode == null)
                    return;
            }
            else if (parentId == 0)
            {
                parentId = monthNode.chapter.Id;
            }
            else
            {
                // root node demanded through -1, set parent to 0 meaning invalid
                parentId = 0;
            }

            myNode node = new myNode();
            node.chapter.parentId = parentId;
            node.chapter.chapterDateTime = dateTime;
            node.chapter.nodeType = nodeType;
            node.chapter.Title = title;
            if (!entryMethods.DBCreateNode(ref cfg, ref node, "", true, true, true, true, true, true))
                return;

            // add the newly created node in the context session work list
            allNodes.Add(node);

            List<myNode> lineageChain = entryMethods.findBottomToRootNodesRecursive(ref allNodes, ref node, false, false);
            if (lineageChain.Count <= 0)
                return;

            // now insert the entire chain from root to bottom most child
            this.Invoke(loadRootToBottomNodes, lineageChain, true);

            // validate the newly created node
            TreeNode? newTreeNode = null;
            if (!__findTreeNode(node.chapter.Id, out newTreeNode))
                return;

            // update ui
            cfg.totalNodes = allNodes.LongCount();
            __setCalendarHighlightEntry(node.chapter.chapterDateTime);
            tsslblTotalEntries.Text = cfg.totalNodes.ToString();
            tvEntries.SelectedNode = newTreeNode;
            lineageChain.Reverse();
            node.lineage = lineageChain;
        }
        public bool __findTreeNode(String IdString, out TreeNode? treeNodeOut)
        {
            Int64 Id = Int64.Parse(IdString);
            return __findTreeNode(Id, out treeNodeOut);
        }
        public bool __findTreeNode(Int64 Id, out TreeNode? treeNodeOut)
        {
            TreeNode? treeNode = null;
            String path = String.Format(@"{0}", Id);
            TreeNode[] nodes = tvEntries.Nodes.Find(path, true);
            if (nodes.Length != 0)
            {
                treeNode = nodes[0];
                treeNodeOut = treeNode;
                return true;
            }
            treeNodeOut = null;
            return false;
        }
        private void newDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            __createNewDB();
        }

        public void __createNewDB()
        {
            if (cfg.radCfgUseOpenFileSystemDB)
            {
                __createNewDB_OpenFileSystemDB();
            }
            else
            {
                __createNewDB_SingleFileDB();
            }

            // auto create/load system nodes
            mySystemNodes? systemNodes = null;
            reloadAll(true, true, true, ref systemNodes);
        }

        public void reloadAll(bool autoAllNodes, bool autoSystemNodes, bool autoTreeView, ref mySystemNodes? systemNodesOut)
        {
            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            if (autoAllNodes)
            {
                allNodes = entryMethods.DBFindAllNodes(cfg, false, false);
                //allNodes = entryMethods.DBFindAllNodesTreeSequence(cfg, ref allNodes, true, false);

                foreach (myNode? node in allNodes)
                {
                    myNode? listedNode = node;
                    listedNode.lineage = entryMethods.findBottomToRootNodesRecursive(ref allNodes, ref listedNode, false, false, true, false);
                }

            }

            if (autoSystemNodes)
            {
                mySystemNodes? systemNodes = null;
                entryMethods.autoCreateLoadSystemNodes(ref cfg, ref allNodes, out systemNodes);
                systemNodesOut = systemNodes;
            }

            if (autoTreeView)
                this.Invoke(loadEntries);

            this.Invoke(toggleForm, true);

            formOperation.close();
        }
        public void __createNewDB_SingleFileDB()
        {
            sfdDB.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
            if (sfdDB.ShowDialog() != DialogResult.OK)
                return;

            string? input = "";
            if (userInterface.ShowInputDialog("input new database name/title", ref input) != DialogResult.OK)
                return;

            // first reset everything
            reset();

            // create new db
            if (!SingleFileDB.CreateLoadDB(sfdDB.FileName, input, ref cfg.ctx0, true, true))
            {
                reset();
                this.Invoke(showMessageBox, "errror creating/loading db", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // setup ui
            txtDBFile.Text = cfg.ctx0.dbpath;

        }
        public void __createNewDB_OpenFileSystemDB()
        {
            if (browseFolder.ShowDialog(this) != DialogResult.OK)
                return;

            string? input = "";
            if (userInterface.ShowInputDialog("input new database name/title", ref input) != DialogResult.OK)
                return;

            // first reset everything
            reset();

            // todo
            if (!OpenFileSystemDB.CreateLoadDB(browseFolder.SelectedPath, input, ref cfg.ctx1, true, true))
            {
                reset();
                this.Invoke(showMessageBox, "errror creating/loading db", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // setup ui
            txtDBFile.Text = cfg.ctx1.dbBasePath;
        }

        private void closeDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cfg.ctx0.isDBOpen() || cfg.ctx1.isDBOpen())
            {
                saveEntry();
                closeContext();
            }
        }

        public void closeContext()
        {
            reset();
        }

        private void TheJournalImportrtfEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doImportRtfEntries(false);
        }
        public void doImportRtfEntries(bool alienEntries)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            browseFolder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            string? input = "the journal set";
            if (alienEntries)
                input = "other calendar import set";

            if (userInterface.ShowInputDialog("input new clone set name/title", ref input) != DialogResult.OK)
                return;

            if (input.Length <= 0)
                return;

            __importTheJournalRtfEntries(browseFolder.SelectedPath, input, true);
        }
        private void toolStripMenuItem70_Click(object sender, EventArgs e)
        {
            doImportRtfEntries(true);
        }

        public void __setCalendarHighlightEntry(DateTime dateTime)
        {
            DateTime day = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
            CalendarEntries.AddBoldedDate(day);

        }

        public void __treeViewBeginUpdate(TreeView tv, bool clear)
        {
            tv.Hide();
            tv.SuspendLayout();
            //tv.BeginUpdate();

            if (clear)
                tv.Nodes.Clear();

            CalendarEntries.SuspendLayout();
            CalendarEntries.Hide();
        }
        public void __treeViewEndUpdate(TreeView tv)
        {
            CalendarEntries.ResumeLayout();
            CalendarEntries.Show();

            //tv.EndUpdate();
            tv.ResumeLayout();
            tv.Show();
        }

        /*
        public void __treeViewSort(TreeView tv)
        {
            List<myNode>? nodes = null;
            if (cfg.radCfgUseOpenFileSystemDB)
                nodes = OpenFileSystemDB.findAllNodes(cfg.ctx1, true, false);
            else
                nodes = SingleFileDB.findAllNodes(cfg.ctx0, true, false);
            
            if (nodes == null)
                return;

            var defaultSorter = tv.TreeViewNodeSorter;
            tv.TreeViewNodeSorter = new NodeSorter(ref nodes);
            tv.Sort();
            tv.TreeViewNodeSorter = defaultSorter;
        }
        */

        public void __importTheJournalRtfEntries(String path, String importSetName, bool loadOperationForm)
        {
            // first save the entry
            this.Invoke(saveEntry);

            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            if (loadOperationForm)
                formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            System.Windows.Forms.RichTextBox richTextBox = new System.Windows.Forms.RichTextBox();

            IEnumerable<FileInfo> files = commonMethods.EnumerateFiles(path, EntryType.Rtf);
            long index = 0;
            long total = files.LongCount();

            // first we need to create a set node. we cannot import anything at all without a set node.
            myNode? setNode = entryMethods.createSetNode(ref cfg, importSetName, DateTime.Now);

            // import set node
            entryMethods.DBCreateNode(ref cfg, ref setNode, "", false, false, false, false, true, true);

            // this set's own context session work list
            List<myNode> setList = new List<myNode>();

            foreach (FileInfo file in files)
            {
                // everything inside the set and the set node is virtual and relative, not based on anything outside the set and set node.

                Chapter? chapter = theJournalMethods.convertFilenameToChapter(file.FullName);
                if (chapter == null)
                    continue;

                // get rtf and update
                // richtextbox automatically cleans and fixes a corrupted rtf and makes it valid.
                // so we first load the imported rtf into a richtextbox object, then retrieve the cleaned and fixed
                // rtf from it and only then store it in db.
                String rtf = File.ReadAllText(file.FullName);
                rtf = theJournalMethods.fixTheJournalRtfEntry(rtf);
                richTextBox.Rtf = rtf;
                rtf = richTextBox.Rtf;

                // by default all entries imported from "The Journal" are root entries aligned in Year and Month Nodes.
                // entry's properties
                chapter.nodeType = NodeType.EntryNode;

                // initialize calender nodes
                myNode? yearNode = null;
                myNode? monthNode = null;

                // initialize set's own virtual calendar nodes
                entryMethods.initCalenderNodesNonSystemSet(cfg, ref setList, ref setNode, chapter.chapterDateTime.Year,
                    chapter.chapterDateTime.Month, out yearNode, out monthNode, true);

                // now setup the chapter with the year and month config
                chapter.parentId = monthNode.chapter.Id;

                // create new node into db
                myNode? newNode = new myNode(ref chapter);
                newNode = entryMethods.DBNewNode(ref cfg,
                    SpecialNodeType.None, NodeType.EntryNode, DomainType.Journal,
                    ref newNode, true, true, true, chapter.chapterDateTime, chapter.parentId, true, chapter.Title, rtf,
                    true, true, false);

                if (newNode == null)
                    continue;

                // node op success, load the node into the global context session work list
                setList.Add(newNode);

                // update ui
                if (loadOperationForm)
                {
                    formOperation.updateProgressBar(index, total);
                    formOperation.updateFilesStatus(index, total);
                }

                // update
                index++;
            }

            // checkpoint
            entryMethods.DBCheckpoint(ref cfg);

            // finally update the db index in file.
            entryMethods.DBWriteIndexing(ref cfg);

            // now first add set node
            allNodes.Add(setNode);

            // now add all set list into the global session work list
            allNodes.AddRange(setList);

            this.Invoke(toggleForm, true);

            if (loadOperationForm)
                formOperation.close();

            mySystemNodes? systemNodes = null;
            reloadAll(true, true, true, ref systemNodes);
        }

        public void __showMessageBox(String text, String title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            MessageBox.Show(this, text, title, buttons, icon);
        }

        public void __toggleForm(bool toggle)
        {
            this.Enabled = toggle;
            //this.Show();
            //this.BringToFront();
            //this.Focus();
        }
        public void __hideForm(bool toggle)
        {
            if (toggle)
                this.Hide();
            else
                this.Show();
        }

        public void __updateTotalEntriesStatus(long totalEntries)
        {
            tsslblTotalEntries.Text = totalEntries.ToString();
        }
        private static void PopulateTreeView(TreeView treeView, IEnumerable<string> paths, char pathSeparator)
        {
            TreeNode lastNode = null;
            string subPathAgg;
            foreach (string path in paths)
            {
                subPathAgg = string.Empty;
                foreach (string subPath in path.Split(pathSeparator))
                {
                    subPathAgg += subPath + pathSeparator;
                    TreeNode[] nodes = treeView.Nodes.Find(subPathAgg, true);
                    if (nodes.Length == 0)
                        if (lastNode == null)
                            lastNode = treeView.Nodes.Add(subPathAgg, subPath);
                        else
                            lastNode = lastNode.Nodes.Add(subPathAgg, subPath);
                    else
                        lastNode = nodes[0];
                }
            }
        }
        // this method sets up the node in the treeview
        public TreeNode? __initTreeViewEntry(ref myNode nodeEntry)
        {
            String entryName = "";
            TreeNode newNode = null;

            if (nodeEntry.chapter.parentId == 0)
            {
                // this is root node because it has no parent. so insert it directly as the root.
                String path = String.Format(@"{0}", nodeEntry.chapter.Id);
                TreeNode[] matchingNodes = tvEntries.Nodes.Find(path, true);
                if (matchingNodes.Length == 0) // node does not exists, so create it
                {
                    entryName = entryMethods.getEntryLabel(nodeEntry, false);
                    newNode = tvEntries.Nodes.Add(path, entryName);
                    newNode.Name = path;
                    loadNodeHighlight(cfg, newNode, ref nodeEntry);
                    this.Invoke(setCalendarHighlightEntry, nodeEntry.chapter.chapterDateTime);
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
                        entryName = entryMethods.getEntryLabel(nodeEntry, false);
                        newNode = parentNodes[0].Nodes.Add(path, entryName);
                        newNode.Name = path;
                        loadNodeHighlight(cfg, newNode, ref nodeEntry);
                        this.Invoke(setCalendarHighlightEntry, nodeEntry.chapter.chapterDateTime);
                    }
                }
            }

            if (newNode != null)
            {
                // setup tree node icons
                if (nodeEntry.chapter.specialNodeType == SpecialNodeType.SystemNode && (nodeEntry.chapter.nodeType == NodeType.JournalNode ||
                    nodeEntry.chapter.nodeType == NodeType.LibraryNode))
                {
                    newNode.ImageIndex = 1;
                    newNode.SelectedImageIndex = 2;
                }
                // setup treeview icons
                else if (nodeEntry.chapter.nodeType == NodeType.YearNode || nodeEntry.chapter.nodeType == NodeType.MonthNode)
                {
                    newNode.ImageIndex = 3;
                    newNode.SelectedImageIndex = 3;
                }
                else if (nodeEntry.chapter.nodeType == NodeType.SetNode)
                {
                    newNode.ImageIndex = 4;
                    newNode.SelectedImageIndex = 4;
                }
                else if (nodeEntry.chapter.nodeType == NodeType.LabelNode)
                {
                    newNode.ImageIndex = 5;
                    newNode.SelectedImageIndex = 5;
                }
            }
            return newNode;
        }

        // this method sets up the prepared root node in the treeview
        public void __initTreeViewRootNode(TreeNode? node)
        {
            tvEntries.Nodes.Add(node);
        }

        public void __loadEntries()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // first save the entry
            this.Invoke(saveEntry);

            // load entire tree
            List<myNode> worklist = allNodes;
            allNodes = (List<myNode>)this.Invoke(loadTree, worklist);

            // update ui
            this.Invoke(gotoEntryByAttribute, cfg.radCfgLMNode, cfg.radCfgLCNode, cfg.radCfgTCNode, false, -1);
        }

        public List<myNode> __loadTree(ref List<myNode> srcNodes)
        {
            this.Invoke(treeViewBeginUpdate, tvEntries, true);

            // build tree
            List<myNode> outputTree = new List<myNode>();
            List<TreeNode> tree = entryMethods.buildTreeViewTree(ref srcNodes, ref outputTree, true, false, CalendarEntries, false);
            srcNodes = outputTree;

            // load all tree 
            foreach (TreeNode node in tree)
                this.Invoke(initTreeViewRootNode, node);

            cfg.totalNodes = srcNodes.LongCount();
            this.Invoke(treeViewEndUpdate, tvEntries);
            this.Invoke(updateTotalEntriesStatus, cfg.totalNodes);
            return outputTree;
        }

        public void __loadSystemNodes(ref mySystemNodes systemNodes)
        {
            // load journal node
            this.Invoke(initTreeViewEntry, systemNodes.JournalSystemNode);

            // load library node
            this.Invoke(initTreeViewEntry, systemNodes.LibrarySystemNode);

            // load all year nodes
            __loadRootToBottomNodes(ref systemNodes.YearNodes, false);

            // load all month nodes
            __loadRootToBottomNodes(ref systemNodes.MonthNodes, false);
        }
        public void __loadRootToBottomNodes(ref List<myNode> nodes, bool reverse = true)
        {
            // first reverse the nodes so that root node which is last is first, and all parents align before the children upto the bottom node.
            if (reverse)
                nodes.Reverse();

            foreach (myNode node in nodes)
            {
                // we cannot load a deleted node and all it's children recursively. they are all then deleted. so we skip them all.
                if (node.chapter.IsDeleted)
                    continue;

                // process current node first so that the children nodes are processed after it's processing.
                String path = String.Format(@"{0}", node.chapter.Id);
                TreeNode[] treeNodes0 = tvEntries.Nodes.Find(path, true);
                if (treeNodes0.Count() >= 1)
                    continue; // this node had been processed before. so skip it's reprocessing.

                // this is a new node. process it with 1st level (core) priority.
                this.Invoke(initTreeViewEntry, node); // common child entry present in month.
            }
        }
        public void loadDB(String path, bool save = true)
        {
            if (path == "")
                return;


            // firstly save entry
            if (save) saveEntry();

            // reset everything
            reset();

            if (cfg.radCfgUseOpenFileSystemDB)
            {
                if (!OpenFileSystemDB.CreateLoadDB(path, "", ref cfg.ctx1, false, false))
                {
                    reset();
                    this.Invoke(showMessageBox, "error loading db or invalid (non-db) path.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // setup ui
                txtDBFile.Text = cfg.ctx1.dbBasePath;
            }
            else
            {
                if (!SingleFileDB.CreateLoadDB(path, "", ref cfg.ctx0, false, false))
                {
                    reset();
                    this.Invoke(showMessageBox, "errror creating/loading db", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // setup ui
                txtDBFile.Text = cfg.ctx0.dbpath;

            }

            // reload/load
            mySystemNodes? systemNodes = null;
            reloadAll(true, true, true, ref systemNodes);
        }
        private void loadExistingDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doLoadDB();
        }

        public void doLoadDB()
        {
            if (cfg.radCfgUseOpenFileSystemDB)
            {
                browseFolder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
                if (browseFolder.ShowDialog() == DialogResult.OK)
                    loadDB(browseFolder.SelectedPath);
            }
            else
            {
                ofdDB.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
                if (ofdDB.ShowDialog() == DialogResult.OK)
                    loadDB(ofdDB.FileName);
            }
        }

        public void reset()
        {
            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // first clear tree view
            tvEntries.SuspendLayout();
            tvEntries.Hide();
            tvEntries.Nodes.Clear();
            tvEntries.Show();
            tvEntries.ResumeLayout();

            // reset user interface
            resetRtb(rtbEntry);
            CalendarEntries.RemoveAllBoldedDates();
            tsslblTotalEntries.Text = "0";
            txtEntryTitle.Text = "unique title (mandatory for identification)";
            txtBoxEntryPath.Text = "full path of the entry";
            tsslblStateChanged.Text = " ";
            stateChanged = false;
            txtDBFile.Text = "";
            tsbuttonSave.Checked = false;
            tsbuttonSave.BackColor = SystemColors.Control;
            tsslblStateChanged.BackColor = SystemColors.Control;

            // close the db
            cfg.ctx0.close();
            cfg.ctx1.close();

            this.Invoke(toggleForm, true);

            formOperation.close();

        }

        private void TvEntries_BeforeSelect(object? sender, TreeViewCancelEventArgs e)
        {
            // now first save the previous state if the state was changed.
            saveEntry();
        }

        private void tvEntries_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;

            rtbEntry.sel = null;

            // load another node
            Int64 id = Int64.Parse(e.Node.Name);
            loadSelectedEntry(id);
        }

        public void loadSelectedEntry(Int64 id)
        {
            timerSetRtbEntry.Tag = id;
            timerSetRtbEntry.Enabled = true;
            timerSetRtbEntry.Start();
        }


        private void newEntryNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setupNowEntry();
        }

        private void saveEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveEntry();
        }

        public void __rotateInsertState()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            String newXamlState = rtbEntry.Xaml;
            xamlState0 = xamlState1;
            xamlState1 = xamlState2;
            xamlState2 = xamlState3;
            xamlState3 = xamlState4;
            xamlState4 = xamlState5;
            xamlState5 = xamlState6;
            xamlState6 = xamlState7;
            xamlState7 = xamlState8;
            xamlState8 = xamlState9;
            xamlState9 = newXamlState;
        }

        public void __saveEntry()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null) return;

            // save current caret position
            entryMethods.DBUpdateCaretConfig(ref cfg, ref node, rtbEntry.SelectionStartOffset, rtbEntry.SelectionLength);

            // check if body was changed
            if (!stateChanged)
                return;

            // first rotate and save new state for emergency restore
            __rotateInsertState();

            // save the entry
            entryMethods.DBUpdateNode(cfg, ref node, rtbEntry.Rtf, true);

            stateChanged = false;
            tsslblStateChanged.Text = " ";
            tsslabelLMD.Text = node.chapter.modificationDateTime.ToString("HH:mm:ss:fff, dddd, dd MMMM yyyy");
            tsbuttonSave.Checked = false;
            tsbuttonSave.BackColor = SystemColors.Control;
            tsslblStateChanged.BackColor = SystemColors.Control;

        }



        public void undortbEntry()
        {
            rtbEntry.Undo();
        }
        public void redortbEntry()
        {
            rtbEntry.Redo();
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undortbEntry();
        }
        private void undoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            undortbEntry();
        }

        public void changeEntryTitle()
        {
            if (tvEntries.SelectedNode == null)
                return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            // system node title cannot be modified
            if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return;

            string? input = node.chapter.Title;
            if (userInterface.ShowInputDialog("input title for entry", ref input) != DialogResult.OK)
                return;

            setupEntryTitle(input);
        }

        private void entryTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeEntryTitle();
        }

        public void setupEntryTitle(String title)
        {
            if (tvEntries.SelectedNode == null)
                return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            if (!entryMethods.DBUpdateNodeTitle(cfg, ref node, title))
                return;

            // resetup tree node
            String entryName = "";
            entryName = entryMethods.getEntryLabel(node, false);
            tvEntries.SelectedNode.Text = entryName;
            txtEntryTitle.Text = title;
            String entryPath = "";
            List<myNode>? lineage = null;
            if (entryMethods.GenerateLineagePath(ref allNodes, ref node, out entryPath, out lineage))
                txtBoxEntryPath.Text = entryPath;

        }

        private void splitContainerV_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void titleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeEntryTitle();
        }

        public void pastertbEntry()
        {
            rtbEntry.Paste();
        }
        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            pastertbEntry();
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pastertbEntry();
        }

        private void rtbEntry_TextChanged(object? sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            stateChanged = true;
            tsslblStateChanged.Text = "*";
            tsslblStateChanged.BackColor = System.Drawing.Color.Red;
            tsbuttonSave.BackColor = System.Drawing.Color.Red;
            //            tsbuttonSave.Checked = true;
        }

        private void gotoTodaysEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            __gotoTodaysEntry();
        }

        public void __gotoParentByEntryID(Int64 id)
        {
            if (allNodes == null) return;
            if (allNodes.Count() <= 0) return;

            // catch the node from work list
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            // goto custom entry
            __gotoEntryByAttribute(false, false, false, true, node.chapter.parentId);
        }

        public void __gotoEntry(DateTime dateTime)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            myNode? node = null;

            List<myNode> list = entryMethods.findNodesByNodeTypeDate(ref allNodes,
                SpecialNodeType.AnyOrAll, NodeType.AnyOrAll,
                dateTime.Year, dateTime.Month, dateTime.Day);

            if (list.Count <= 0)
                return;

            entryMethods.sortNodesByDateTime(ref list, true);
            node = list[0];

            if (node == null)
                return;

            String path = String.Format(@"{0}", node.chapter.Id);
            TreeNode[] treeNodes = tvEntries.Nodes.Find(path, true);
            if (treeNodes.Count() <= 0)
                return;

            // found one node in tree view
            tvEntries.SelectedNode = treeNodes[0];
        }
        public void __gotoEntryById(Int64 id)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            String path = String.Format(@"{0}", node.chapter.Id);
            TreeNode[] treeNodes = tvEntries.Nodes.Find(path, true);
            if (treeNodes.Count() <= 0)
                return;

            // found one node in tree view
            tvEntries.SelectedNode = treeNodes[0];
        }

        //public void gotoEntryByAttribute(bool lm, bool lc)
        // {
        //     __gotoEntryByAttribute(lm, lc);
        //}
        public void __gotoEntryByAttribute(bool lm, bool lc, bool tc, bool byID, Int64 id)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (allNodes.Count == 0) return;

            myNode? node = null;

            if (lm)
                node = allNodes.FirstOrDefault(x => x.chapter.modificationDateTime == allNodes.Max(x => x.chapter.modificationDateTime));
            else if (lc)
                node = allNodes.FirstOrDefault(x => x.chapter.creationDateTime == allNodes.Max(x => x.chapter.creationDateTime));
            else if (tc)
                node = allNodes.FirstOrDefault(x => x.chapter.chapterDateTime == allNodes.Max(x => x.chapter.chapterDateTime));
            else if (byID)
            {
                if (id <= 0) return;
                node = entryMethods.FindNodeInList(ref allNodes, id);
                if (node == null)
                    return;
            }

            if (node == null)
                return;

            String path = String.Format(@"{0}", node.chapter.Id);
            TreeNode[] treeNodes = tvEntries.Nodes.Find(path, true);
            if (treeNodes.Count() <= 0)
                return;

            // found one node in tree view
            tvEntries.SelectedNode = treeNodes[0];

        }
        public void __gotoTodaysEntry()
        {
            gotoEntry(DateTime.Now);
        }

        private void gotoDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            DateTime inputDate = DateTime.Now;
            if (userInterface.ShowDateTimeDialog("input entry's date and time", ref inputDate, CalendarEntries.BoldedDates) != DialogResult.OK)
                return;

            gotoEntry(inputDate);
        }

        public void copyrtbEntry()
        {
            rtbEntry.Copy();
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            copyrtbEntry();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyrtbEntry();
        }

        public void copyAllrtbEntry()
        {
            rtbEntry.SelectAll();
            rtbEntry.Copy();
        }

        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyAllrtbEntry();
        }

        private void copyAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            copyAllrtbEntry();
        }

        public void cutrtbEntry()
        {
            rtbEntry.Cut();
        }

        public void cutAllrtbEntry()
        {
            rtbEntry.SelectAll();
            rtbEntry.Cut();
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cutrtbEntry();
        }

        private void cutAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cutAllrtbEntry();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cutrtbEntry();
        }

        private void cutAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cutAllrtbEntry();
        }

        private void newEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newEntry();
        }

        public void newEntry()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            DateTime inputDate = DateTime.Now;
            if (userInterface.ShowDateTimeDialog("new entry's date and time", ref inputDate, CalendarEntries.BoldedDates) != DialogResult.OK)
                return;

            setupNewEntry(inputDate, 0);
        }

        private void CalendarEntries_DateChanged(object sender, DateRangeEventArgs e)
        {
        }

        private void CalendarEntries_DateSelected(object? sender, DateRangeEventArgs e)
        {
            this.Invoke(gotoEntry, e.Start);
        }

        private void tsbuttonSave_Click(object sender, EventArgs e)
        {
            saveEntry();
        }

        private void cmbFonts_Click(object sender, EventArgs e)
        {
        }


        private void tsbuttonBold_Click(object sender, EventArgs e)
        {
            formatting.formatBold(rtbEntry, (ToolStripButton)sender);
        }

        private void tsbuttonItalics_Click(object sender, EventArgs e)
        {
            formatting.formatItalics(rtbEntry, (ToolStripButton)sender);
        }

        private void tsbuttonUnderline_Click(object sender, EventArgs e)
        {
            formatting.formatUnderline(rtbEntry, (ToolStripButton)sender);
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            doExportCheckedEntries(EntryType.Html);
        }

        public void doExportCheckedEntries(EntryType entryType)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            browseFolder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            bgWorkerExportEntries.RunWorkerAsync(entryType);
        }
        private void tsbuttonStrikeout_Click(object sender, EventArgs e)
        {
            formatting.formatStrikeout(rtbEntry, (ToolStripButton)sender);
        }

        private void tsbuttonLeftJustify_Click(object sender, EventArgs e)
        {
            formatting.formatLeftJustify(rtbEntry, (ToolStripButton)sender);
        }

        private void tsbuttonRightJustify_Click(object sender, EventArgs e)
        {
            formatting.formatRightJustify(rtbEntry, (ToolStripButton)sender);
        }

        private void tsbuttonJustify_Click(object sender, EventArgs e)
        {
            formatting.formatJustify(rtbEntry, (ToolStripButton)sender);
        }

        private void tsbuttonCenterJustify_Click(object sender, EventArgs e)
        {
            formatting.formatCenterJustify(rtbEntry, (ToolStripButton)sender);
        }

        private void tsbuttonNewEntry_Click(object sender, EventArgs e)
        {
            newEntry();
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            redortbEntry();
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            redortbEntry();
        }

        private void buttonApplyConfig1_Click(object sender, EventArgs e)
        {
            String err = "error configuration. retry after correcting it. aborted.";
            int rtbViewEntryRightMargin = 0;
            if (!int.TryParse(cmbCfgRtbViewEntryRM.Text, out rtbViewEntryRightMargin))
            {
                MessageBox.Show(err, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // first save config which is allowed to be saved while db is loaded.
            rtbViewEntry.RightMargin = rtbViewEntryRightMargin;
            cfg.cmbCfgRtbViewEntryRMValue = rtbViewEntryRightMargin;

            // now set other 1st level config
            cfg.radCfgLMNode = radCfgLMNode.Checked;
            cfg.radCfgLCNode = radCfgLCNode.Checked;
            cfg.radCfgTCNode = radCfgTCNode.Checked;

            int tvEntriesItemHeight = 16;
            if (!int.TryParse(cmbCfgTVEntriesItemHeight.Text, out tvEntriesItemHeight))
            {
                MessageBox.Show(err, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            cfg.tvEntriesItemHeight = tvEntriesItemHeight;
            tvEntries.ItemHeight = tvEntriesItemHeight;

            int tvEntriesIndent = 20;
            if (!int.TryParse(cmbCfgTVEntriesIndent.Text, out tvEntriesIndent))
            {
                MessageBox.Show(err, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            cfg.tvEntriesIndent = tvEntriesIndent;
            tvEntries.Indent = tvEntriesIndent;

            System.Drawing.Font tvEntriesFont = commonMethods.StringToFont(linkCfgTVEntriesFont.Text);
            if (tvEntriesFont == null) tvEntriesFont = myConfig.default_tvEntriesFont;
            cfg.tvEntriesFont = tvEntriesFont;
            tvEntries.Font = tvEntriesFont;

            System.Drawing.Color tvEntriesBackColor = linkCfgTVEntriesFont.BackColor;
            System.Drawing.Color tvEntriesForeColor = linkCfgTVEntriesFont.ForeColor;
            cfg.tvEntriesBackColor = tvEntriesBackColor;
            cfg.tvEntriesForeColor = tvEntriesForeColor;
            tvEntries.BackColor = tvEntriesBackColor;
            tvEntries.ForeColor = tvEntriesForeColor;

            myConfigMethods.saveConfigFile(myConfigMethods.getConfigPathFile(), ref cfg, false);

            // message
            MessageBox.Show("1st level configuration is set.", "set 1st level config", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // abort if no more config can be saved if a db is loaded.
            if (cfg.ctx0.isDBOpen() || cfg.ctx1.isDBOpen())
            {
                MessageBox.Show("error changing 2nd level config. please save and close the opened db and retry.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // in the final place, apply all configuration when db is closed.
            // update config
            cfg.chkCfgAutoLoadCreateDefaultDB = chkCfgAutoLoadCreateDefaultDB.Checked;
            cfg.chkCfgUseWinUserDocFolder = chkCfgUseWinUserDocFolder.Checked;
            cfg.radCfgUseSingleFileDB = radCfgUseSingleFileDB.Checked;
            cfg.radCfgUseOpenFileSystemDB = radCfgUseOpenFileSystemDB.Checked;
            myConfigMethods.saveConfigFile(myConfigMethods.getConfigPathFile(), ref cfg, false);
            MessageBox.Show("applied all levels of configurations.", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {

            CustomFontDialog fontDialog = new CustomFontDialog();
            fontDialog.font = rtbEntry.SelectionFont;
            fontDialog.size = (int)rtbEntry.SelectionFontSize;
            fontDialog.bold = rtbEntry.Bold;
            fontDialog.italic = rtbEntry.Italic;
            fontDialog.underline = rtbEntry.Underline;
            fontDialog.strikeout = rtbEntry.Strikeout;
            fontDialog.fontColor = rtbEntry.SelectionColor;
            fontDialog.fontBackColor = rtbEntry.SelectionBackColor;

            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;

            rtbEntry.SelectionFont = fontDialog.font;
            rtbEntry.SelectionColor = fontDialog.fontColor;
            rtbEntry.SelectionBackColor = fontDialog.fontBackColor;
            rtbEntry.SelectionFontSize = fontDialog.size;
            rtbEntry.Bold = fontDialog.bold;
            rtbEntry.Italic = fontDialog.italic;
            rtbEntry.toggleSelectionAllDecorations(rtbEntry.Selection, fontDialog.underline, fontDialog.strikeout);
            //rtbEntry.Strikeout = fontDialog.strikeout;
            //rtbEntry.Underline = fontDialog.underline;

        }

        private void cmbSize_Click(object sender, EventArgs e)
        {
        }

        private void CmbSize_DropDownClosed(object? sender, EventArgs e)
        {
            if (cmbSize.SelectedItem == null)
                return;

            int value = (int)cmbSize.SelectedItem;

            formatting.formatFontSize(rtbEntry, value);
        }

        public void autoCreateLoadDefaultDB(bool overwrite)
        {
            // first save the entry if was modified.
            saveEntry();

            // first reset everything
            reset();

            if (cfg.radCfgUseOpenFileSystemDB)
            {
                // direct auto create/load default db
                if (!OpenFileSystemDB.autoLoadCreateDefaultDB(ref cfg.ctx1, cfg, overwrite))
                {
                    MessageBox.Show("error creating/loading the default db. aborted.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // setup ui
                txtDBFile.Text = cfg.ctx1.dbBasePath;
            }
            else
            {
                // direct auto create/load default db
                if (!SingleFileDB.autoLoadCreateDefaultDB(ref cfg.ctx0, cfg, overwrite))
                {
                    MessageBox.Show("error creating/loading the default db. aborted.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // setup ui
                txtDBFile.Text = cfg.ctx0.dbpath;
            }

            // reload/load
            mySystemNodes? systemNodes = null;
            reloadAll(true, true, true, ref systemNodes);
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            autoCreateLoadDefaultDB(false);
        }

        private void tsbuttonBullets_Click(object sender, EventArgs e)
        {
            formatting.formatBullets(rtbEntry);

        }

        private void tsbuttonNumberedList_Click(object sender, EventArgs e)
        {
            formatting.formatNumberedList(rtbEntry);

        }

        private void toolStripContainer2_ContentPanel_Load(object sender, EventArgs e)
        {

        }

        private void tsbuttonIndentLeft_Click(object sender, EventArgs e)
        {
            formatting.formatIndentLeft(rtbEntry, tsbuttonIndentLeft, tsbuttonIndentRight);
        }

        private void tsbuttonIndentRight_Click(object sender, EventArgs e)
        {
            formatting.formatIndentRight(rtbEntry, tsbuttonIndentLeft, tsbuttonIndentRight);
        }

        private void lineSpace1_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, 1.0);
        }

        private void lineSpace1pt5_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, 1.5);

        }

        private void lineSpace2_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, 2.0);

        }
        private void toolStripMenuItem60_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, 4.0);
        }

        private void toolStripMenuItem61_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, 6.0);

        }
        private void toolStripMenuItem62_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, 10.0);
        }

        private void toolStripMenuItem63_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, 14.0);
        }

        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertTable();
        }

        public void insertTable()
        {
            formatting.formatInsertTable(rtbEntry);
        }

        private void tsbuttonStore_Click(object sender, EventArgs e)
        {
            store();
        }

        public void store()
        {
            String xaml = rtbEntry.Xaml;
            if (xaml == "") return;

            xamlState = xaml;
        }


        public void restore()
        {
            if (xamlState == "") return;

            previousXamlState = rtbEntry.Xaml;
            rtbEntry.Xaml = xamlState;
        }
        public void undoRestore()
        {
            if (previousXamlState == "") return;

            rtbEntry.Xaml = previousXamlState;
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restore();
        }

        private void restoreToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            restore();
        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {
            store();
        }

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            store();
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {

        }

        private void tsbuttonIncreaseFontSize_Click(object sender, EventArgs e)
        {
            formatting.formatIncreaseFontSize(rtbEntry);
        }

        private void tsbuttonDecreaseFontSize_Click(object sender, EventArgs e)
        {
            formatting.formatDecreaseFontSize(rtbEntry);
        }

        private void toolStripMenuItem19_Click(object sender, EventArgs e)
        {
            autoCreateLoadDefaultDB(true);
        }

        private void toolStripMenuItem20_Click(object sender, EventArgs e)
        {
            if (cfg.radCfgUseOpenFileSystemDB)
                loadDB(cfg.ctx1.dbBasePath);
            else
                loadDB(cfg.ctx0.dbpath);

        }

        private void forceSetBoldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetBold(rtbEntry, (ToolStripMenuItem)sender, true);
        }

        private void forceUnsetBoldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetBold(rtbEntry, (ToolStripMenuItem)sender, false);
        }

        private void forceSetItalicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetItalics(rtbEntry, (ToolStripMenuItem)sender, true);
        }

        private void forceUnsetItalicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetItalics(rtbEntry, (ToolStripMenuItem)sender, false);
        }

        private void forceSetUnderlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetUnderline(rtbEntry, (ToolStripMenuItem)sender, true);
        }

        private void forceUnsetUnderlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetUnderline(rtbEntry, (ToolStripMenuItem)sender, false);
        }

        private void forceSetStrikeoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetStrikeout(rtbEntry, (ToolStripMenuItem)sender, true);
        }

        private void forceUnsetStrikeoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatForceSetUnsetStrikeout(rtbEntry, (ToolStripMenuItem)sender, false);
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertImage();
        }

        private void tsbuttonInsertImage_Click(object sender, EventArgs e)
        {
            insertImage();
        }

        public void insertImage()
        {
            ofdFile.Filter = @"*.bmp;*.jpg;*.jpeg;*.gif;*.tiff;*.png|*.bmp;*.jpg;*.jpeg;*.gif;*.tiff;*.png|all files *.*|*.*";
            ofdFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
            if (ofdFile.ShowDialog() != DialogResult.OK)
                return;

            System.Drawing.Image image = System.Drawing.Image.FromFile(ofdFile.FileName);
            formatting.formatInsertImage(rtbEntry, image);

        }

        private void tsbuttonUndo_Click(object sender, EventArgs e)
        {
            undortbEntry();
        }

        private void tsbuttonRedo_Click(object sender, EventArgs e)
        {
            redortbEntry();
        }

        private void tsbuttonRestore_Click(object sender, EventArgs e)
        {
            restore();
        }
        private void undoRestorereadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoRestore();
        }

        private void tsbuttonUndoRestore_Click(object sender, EventArgs e)
        {
            undoRestore();
        }

        private void findTotalEntriesInDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            findTotalEntriesInDB();
        }

        public void findTotalEntriesInDB()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            cfg.totalNodes = allNodes.LongCount();
            MessageBox.Show("total number of " + cfg.totalNodes + " validated/valid entries actually exist in db.", "status", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tsbuttonDeleteEntry_Click(object sender, EventArgs e)
        {
            deleteEntries();
        }

        public void deleteEntries()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // first save the entry
            saveEntry();

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            List<myNode> nodes = __getCheckedTreeViewItemsDBNodes(tvEntries); //__getHighestCheckedTreeViewItemsDBNodes();
            List<myNode>? worklist = new List<myNode>();
            worklist.AddRange(allNodes);
            entryMethods.DBDeleteOrPurgeListRecursive(cfg, ref worklist, ref nodes, true, false, false);

            // checkpoint
            entryMethods.DBCheckpoint(ref cfg);

            foreach (myNode node in nodes)
            {
                // system nodes cannot be touched.
                if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                    continue;

                TreeNode? treeNode = null;
                if (__findTreeNode(node.chapter.Id, out treeNode))
                    treeNode.Remove();

            }

            formOperation.close();

            // auto create/load all nodes
            mySystemNodes? systemNodes = null;
            reloadAll(true, true, false, ref systemNodes);
        }

        private void tsbuttonRefreshTrashcan_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            refreshTrashcan();
        }

        public void refreshTrashcan()
        {
            this.Invoke(toggleForm, false);
            this.Invoke(resetTrashCan);
            this.Invoke(LvTrashCanUpdate, true);

            List<myNode> nodes = entryMethods.DBFindDeletedNodes(ref allNodes);
            foreach (myNode node in nodes)
                this.Invoke(insertLvTrashCanItem, node);

            this.Invoke(LvTrashCanUpdate, false);
            this.Invoke(toggleForm, true);
        }

        public void __resetTrashCan()
        {
            lvTrashCan.Items.Clear();
        }

        public void __insertLvTrashCanItem(ref myNode? node)
        {
            System.Windows.Forms.ListViewItem item = new System.Windows.Forms.ListViewItem();
            item.Name = node.chapter.Id.ToString();
            String chapterDateTime = node.chapter.chapterDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            item.Text = chapterDateTime;

            // dates
            item.SubItems.Add(node.chapter.creationDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
            item.SubItems.Add(node.chapter.modificationDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
            item.SubItems.Add(node.chapter.deletionDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss"));

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
            lvTrashCan.Items.Add(item);
        }

        public void __LvTrashCanUpdate(bool set)
        {
            if (set)
                lvTrashCan.BeginUpdate();
            else
                lvTrashCan.EndUpdate();
        }

        private void tsButtonPurgeDeletedEntry_Click(object sender, EventArgs e)
        {
            purgeTrashCan();
        }

        public void purgeTrashCan()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            List<myNode>? worklist = new List<myNode>();
            worklist.AddRange(allNodes);
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvTrashCan.CheckedItems)
            {
                Int64 id = Int64.Parse(listViewItem.Name);
                myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
                if (node == null)
                    return;

                entryMethods.DBDeleteOrPurgeNodeRecursive(cfg, ref worklist, ref node, false, true, false);
            }

            // checkpoint
            entryMethods.DBCheckpoint(ref cfg);

        }

        public void checkEntireTrashCan(bool check)
        {
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvTrashCan.Items)
                listViewItem.Checked = check;
        }

        private void tsbuttonChkAllTrash_Click(object sender, EventArgs e)
        {
            checkEntireTrashCan(true);
        }

        private void tsbuttonUnchkAllTrash_Click(object sender, EventArgs e)
        {
            checkEntireTrashCan(false);
        }

        public void emptyTrashcan()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            checkEntireTrashCan(true);
            purgeTrashCan();

            formOperation.close();

            // auto create/load all nodes
            mySystemNodes? systemNodes = null;
            reloadAll(true, true, false, ref systemNodes);

            // refresh trash can after deleting
            refreshTrashcan();

        }
        private void tsbuttonEmptyTrashCan_Click(object sender, EventArgs e)
        {
            emptyTrashcan();
        }

        private void tsProgressBar_Click(object sender, EventArgs e)
        {

        }

        public void restoreTrashCan()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            List<myNode>? worklist = new List<myNode>();
            worklist.AddRange(allNodes);
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvTrashCan.CheckedItems)
            {
                Int64 id = Int64.Parse(listViewItem.Name);
                myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
                if (node == null)
                    return;

                entryMethods.DBDeleteOrPurgeNodeRecursive(cfg, ref worklist, ref node, false, false, false);
            }

            // checkpoint
            entryMethods.DBCheckpoint(ref cfg);

        }

        private void tsButtonRestoreDeletedEntry_Click(object sender, EventArgs e)
        {
            restoreTrashCan();
        }

        private void bgWorkerRebuildDB_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!cfg.ctx0.isDBOpen())
                return;

            this.Invoke(toggleForm, false);
            SingleFileDB.rebuildDB(cfg.ctx0);
            this.Invoke(toggleForm, true);

        }

        private void rebuildDbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen())
                return;

            if (!cfg.radCfgUseSingleFileDB)
                return;

            // note that this function is meant only for single file db

            bgWorkerRebuildDB.RunWorkerAsync();
        }

        private void toolStripMenuItem22_Click(object sender, EventArgs e)
        {
            doNewChildEntry();
        }

        public void doNewChildEntry()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            Int64 parentId = Int64.Parse(tvEntries.SelectedNode.Name);
            setupNewEntry(DateTime.Now, parentId);
        }
        public void doNewLabelEntry(bool root = false)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            string? input = "";
            if (userInterface.ShowInputDialog("input entry label (required)", ref input) != DialogResult.OK)
                return;

            if (input.Length <= 0)
                return;

            if (!root)
            {
                if (tvEntries.SelectedNode == null)
                    return;

                Int64 parentId = Int64.Parse(tvEntries.SelectedNode.Name);
                setupNewEntry(DateTime.Now, parentId, NodeType.LabelNode, input);
            }
            else
            {
                setupNewEntry(DateTime.Now, -1, NodeType.LabelNode, input);
            }
        }

        private void newChildEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doNewChildEntry();
        }

        public void CheckEntireTreeView(bool set = true)
        {
            foreach (TreeNode node in tvEntries.Nodes)
                node.Checked = set;

        }
        public static void CheckTreeViewNodeRecursively(TreeNode parentNode, bool set)
        {
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(parentNode);

            while (queue.Count > 0)
            {
                TreeNode currentNode = queue.Dequeue();
                TreeNodeCollection children = currentNode.Nodes;// Directory.GetDirectories(currentPath);

                foreach (TreeNode childNode in children)
                {
                    queue.Enqueue(childNode);
                    childNode.Checked = set;
                }
            }
        }

        private void tsbuttonRestoreTrashCan_Click(object sender, EventArgs e)
        {
            restoreAllTrashcan();
        }

        public void restoreAllTrashcan()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            checkEntireTrashCan(true);
            restoreTrashCan();

            formOperation.close();

            // auto create/load system nodes
            mySystemNodes? systemNodes = null;
            reloadAll(true, true, true, ref systemNodes);

            // refresh trash can after deleting
            refreshTrashcan();

        }

        private void highlightCheckedEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highlightCheckedEntries(false);
        }

        public void highlightEntry(TreeNode treeNode, ref myNode? node)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            CustomFontDialog fontDialog = new CustomFontDialog();

            if (treeNode.NodeFont == null)
                treeNode.NodeFont = cfg.tvEntriesFont;//new System.Drawing.Font("Arial", 8);

            fontDialog.font = treeNode.NodeFont;
            fontDialog.fontColor = treeNode.ForeColor;
            fontDialog.fontBackColor = treeNode.BackColor;
            fontDialog.size = (int)treeNode.NodeFont.Size;
            fontDialog.bold = treeNode.NodeFont.Bold;
            fontDialog.italic = treeNode.NodeFont.Italic;
            fontDialog.underline = treeNode.NodeFont.Underline;
            fontDialog.strikeout = treeNode.NodeFont.Strikeout;
            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;

            entryMethods.setEntryHighlightFontComplete(cfg, ref node, fontDialog.fontColor, fontDialog.fontBackColor, fontDialog.font);
            loadNodeHighlight(cfg, treeNode, ref node);
        }

        // this method sets the highlights and font for a given tree node
        public static void loadNodeHighlight(myConfig? cfg, TreeNode treeNode, ref myNode? node)
        {
            TreeNode tmpNode = new TreeNode();
            tmpNode.NodeFont = cfg.tvEntriesFont;// new System.Drawing.Font("Arial", 8, FontStyle.Regular);

            if (node.chapter.HLFont.Length >= 1)
                treeNode.NodeFont = commonMethods.StringToFont(node.chapter.HLFont);
            else
                treeNode.NodeFont = tmpNode.NodeFont;

            if (node.chapter.HLFontColor.Length >= 1)
                treeNode.ForeColor = commonMethods.StringToColor(node.chapter.HLFontColor);
            else
                treeNode.ForeColor = tmpNode.ForeColor;

            if (node.chapter.HLBackColor.Length >= 1)
                treeNode.BackColor = commonMethods.StringToColor(node.chapter.HLBackColor);
            else
                treeNode.BackColor = tmpNode.BackColor;

        }

        public void loadNodeHighlight(TreeNode treeNode)
        {
            // first find the node
            Int64 id = Int64.Parse(treeNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            if (node.chapter.HLFont.Length >= 1)
                treeNode.NodeFont = commonMethods.StringToFont(node.chapter.HLFont);

            if (node.chapter.HLFontColor.Length >= 1)
                treeNode.ForeColor = commonMethods.StringToColor(node.chapter.HLFontColor);

            if (node.chapter.HLBackColor.Length >= 1)
                treeNode.BackColor = commonMethods.StringToColor(node.chapter.HLBackColor);
        }


        public void highlightCheckedEntries(bool reset)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (tvEntries.Nodes.Count <= 0)
                return;

            CustomFontDialog fontDialog = new CustomFontDialog();
            // create a temporary tree node and extract it's default values
            TreeNode treeNode = new TreeNode();
            fontDialog.font = treeNode.NodeFont;
            fontDialog.fontColor = treeNode.ForeColor;
            fontDialog.fontBackColor = treeNode.BackColor;
            if (treeNode.NodeFont == null)
                treeNode.NodeFont = cfg.tvEntriesFont;// new System.Drawing.Font("Arial", 8);
            fontDialog.size = (int)treeNode.NodeFont.Size;
            fontDialog.bold = treeNode.NodeFont.Bold;
            fontDialog.italic = treeNode.NodeFont.Italic;
            fontDialog.underline = treeNode.NodeFont.Underline;
            fontDialog.strikeout = treeNode.NodeFont.Strikeout;
            if (!reset)
            {
                // if user selected a tree node it should be the default value
                if (tvEntries.SelectedNode != null)
                {
                    fontDialog.fontColor = tvEntries.SelectedNode.ForeColor;
                    fontDialog.fontBackColor = tvEntries.SelectedNode.BackColor;
                    if (tvEntries.SelectedNode.NodeFont != null)
                    {
                        fontDialog.font = tvEntries.SelectedNode.NodeFont;
                        fontDialog.size = (int)tvEntries.SelectedNode.NodeFont.Size;
                        fontDialog.bold = tvEntries.SelectedNode.NodeFont.Bold;
                        fontDialog.italic = tvEntries.SelectedNode.NodeFont.Italic;
                        fontDialog.underline = tvEntries.SelectedNode.NodeFont.Underline;
                        fontDialog.strikeout = tvEntries.SelectedNode.NodeFont.Strikeout;
                    }
                }
                if (fontDialog.ShowDialog() != DialogResult.OK)
                    return;
            }

            TreeNodeCollection rootNodes = tvEntries.Nodes;
            foreach (TreeNode rootNode in rootNodes)
            {
                Queue<TreeNode> queue = new Queue<TreeNode>();
                queue.Enqueue(rootNode);
                while (queue.Count > 0)
                {
                    // the first node is dequeued and processed first, then it's children are processed level by level.
                    TreeNode currentNode = queue.Dequeue();

                    // get children of this node.
                    TreeNodeCollection children = currentNode.Nodes;

                    // add all children is queue, they will be processed in this same way in this same place: 1st parent node, 2nd children nodes.
                    foreach (TreeNode childNode in children)
                        queue.Enqueue(childNode);

                    if (!currentNode.Checked)
                        continue;

                    highlightTreeViewNode(currentNode, fontDialog.font, fontDialog.fontColor, fontDialog.fontBackColor, reset);
                }
            }

            //if (cfg.ctx0.isDBOpen())
            //    SingleFileDB.Checkpoint(cfg.ctx0);

        }

        public void highlightTreeViewNode(TreeNode treeNode, System.Drawing.Font font, System.Drawing.Color color, System.Drawing.Color backColor, bool reset)
        {
            Int64 id = Int64.Parse(treeNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            treeNode.ForeColor = color;
            treeNode.BackColor = backColor;
            treeNode.NodeFont = font;
            if (reset)
                entryMethods.setEntryClearHighlight(cfg, ref node);
            else
                entryMethods.setEntryHighlightFontComplete(cfg, ref node, color, backColor, font);

            loadNodeHighlight(treeNode);

        }
        private void highlightSelectedEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            highlightEntry(tvEntries.SelectedNode, ref node);
        }

        public void highlightEntryBackColor(TreeNode treeNode)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = treeNode.BackColor;

            if (colorDialog.ShowDialog() != DialogResult.OK)
                return;

            Int64 id = Int64.Parse(treeNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            entryMethods.setEntryHighlightBackColor(cfg, ref node, colorDialog.Color);
            loadNodeHighlight(treeNode);
        }

        private void tsbuttonSearch_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            saveEntry();

            bgWorkerSearch.RunWorkerAsync();
        }

        private void bgWorkerSearch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            this.Invoke(toggleForm, false);
            this.Invoke(resetLVSearch);
            this.Invoke(LvSearchUpdate, true);
            bool searchResult = (bool)this.Invoke(processSearch);
            this.Invoke(LvSearchUpdate, false);
            this.Invoke(toggleForm, true);

        }

        public List<myNode> lvItemsToMyNodes(ref ListView? lv)
        {
            List<myNode> list = new List<myNode>();
            foreach (ListViewItem item in lv.Items)
            {
                myNode? node = (myNode)item.Tag;
                list.Add(node);
            }
            return list;
        }
        public bool __processSearch()
        {
            // init search
            List<myNode>? locations = lvItemsToMyNodes(ref lvSearchWhere);
            return journalSearchFramework.searchEntries(cfg, ref allNodes, this, txtSearchProgressFullPath, lvSearch, tsSearchProgressBar,
                dtpickerSearchFrom.Value, dtpickerSearchFromTime.Value, dtpickerSearchThrough.Value, dtpickerSearchThroughTime.Value, chkSearchUseDateRange.Checked,
                dtpickerCDSearchFrom.Value, dtpickerCDSearchFromTime.Value, dtpickerCDSearchThrough.Value, dtpickerCDSearchThroughTime.Value, chkSearchUseCreationDateRange.Checked,
                dtpickerMDSearchFrom.Value, dtpickerMDSearchFromTime.Value, dtpickerMDSearchThrough.Value, dtpickerMDSearchThroughTime.Value, chkSearchUseModificationDateRange.Checked,
                dtpickerDDSearchFrom.Value, dtpickerDDSearchFromTime.Value, dtpickerDDSearchThrough.Value, dtpickerDDSearchThroughTime.Value, chkSearchUseDeletionDateRange.Checked,
                rtbSearch.Text, rtbSearchReplace.Text, chkSearchAll.Checked,
                chkSearchTrashCan.Checked, chkSearchMatchCase.Checked, chkSearchMatchWholeWord.Checked,
                chkSearchReplace.Checked, chkSearchReplaceTitle.Checked, chkSearchEmptyString.Checked, ref locations);
        }

        public void __resetLVSearch()
        {
            lvSearch.Items.Clear();
        }


        public void __LvSearchUpdate(bool set)
        {
            if (set)
                lvSearch.BeginUpdate();
            else
                lvSearch.EndUpdate();
        }

        private void lvSearch_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tsbuttonOpenSearchedEntry_Click(object sender, EventArgs e)
        {
            Int64 id = getSelectedListViewNodeId(lvSearch);
            OpenSearchedEntry(id, false);
        }

        private void toolStripSearch_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        public void checkAllSearchedItems(bool check)
        {
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvSearch.Items)
                listViewItem.Checked = check;
        }

        private void tsbuttonChkAllSearchedEntries_Click(object sender, EventArgs e)
        {
            checkAllSearchedItems(true);
        }

        private void tsbuttonUnchkAllSearchedEntries_Click(object sender, EventArgs e)
        {
            checkAllSearchedItems(false);
        }

        private void tsbuttonResetSearch_Click(object sender, EventArgs e)
        {
            __resetLVSearch();
            rtbSearch.Clear();
            rtbSearchReplace.Clear();
            dtpickerSearchFrom.Value = DateTime.Now;
            dtpickerSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerSearchThrough.Value = DateTime.Now;
            dtpickerSearchThroughTime.Value = DateTime.Parse("23:59:59");
            dtpickerCDSearchFrom.Value = DateTime.Now;
            dtpickerCDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerCDSearchThrough.Value = DateTime.Now;
            dtpickerCDSearchThroughTime.Value = DateTime.Parse("23:59:59");
            dtpickerMDSearchFrom.Value = DateTime.Now;
            dtpickerMDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerMDSearchThrough.Value = DateTime.Now;
            dtpickerMDSearchThroughTime.Value = DateTime.Parse("23:59:59");
            dtpickerDDSearchFrom.Value = DateTime.Now;
            dtpickerDDSearchThrough.Value = DateTime.Now;
            dtpickerDDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerDDSearchThroughTime.Value = DateTime.Parse("23:59:59");
            lvSearchWhere.Tag = null;
            lvSearchWhere.Items.Clear();
        }

        public void restoreSearchedList()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            List<myNode>? worklist = new List<myNode>();
            worklist.AddRange(allNodes);
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvSearch.CheckedItems)
            {
                // restore checked nodes
                Int64 id = Int64.Parse(listViewItem.Name);
                myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
                if (node == null)
                    return;

                entryMethods.DBDeleteOrPurgeNodeRecursive(cfg, ref worklist, ref node, false, false, false);

                listViewItem.Checked = false;
                listViewItem.Selected = true;
            }

            // checkpoint
            entryMethods.DBCheckpoint(ref cfg);

            // auto create/load system nodes
            mySystemNodes? systemNodes = null;
            reloadAll(true, true, true, ref systemNodes);

        }

        private void tsbuttonRestoreSearchedEntry_Click(object sender, EventArgs e)
        {
            restoreSearchedList();
        }
        public void restoreAllSearchedList()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            saveEntry();

            checkAllSearchedItems(true);
            restoreSearchedList();
        }

        public void deleteSearchedList()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            saveEntry();

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            List<myNode>? worklist = new List<myNode>();
            worklist.AddRange(allNodes);
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvSearch.CheckedItems)
            {
                // delete checked nodes
                Int64 id = Int64.Parse(listViewItem.Name);
                myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
                if (node == null)
                    return;

                entryMethods.DBDeleteOrPurgeNodeRecursive(cfg, ref worklist, ref node, true, false, false);

                listViewItem.Checked = false;
                listViewItem.Selected = true;
            }

            // checkpoint
            entryMethods.DBCheckpoint(ref cfg);

            formOperation.close();

            // auto create/load all nodes
            mySystemNodes? systemNodes = null;
            reloadAll(true, true, true, ref systemNodes);
        }

        private void tsbuttonDeleteSearchedEntry_Click(object sender, EventArgs e)
        {
            deleteSearchedList();
        }

        private void tsbuttonRestoreAllSearchedEntries_Click(object sender, EventArgs e)
        {
            restoreAllSearchedList();
        }

        private void copyToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.Copy();
        }

        private void rtbSearchPattern_TextChanged(object sender, EventArgs e)
        {

        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.SelectAll();

        }

        private void pasteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.Paste();
        }

        private void toolStripMenuItem23_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)cms.SourceControl;
            rtb.Cut();

        }

        private void generateTestEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbEntry.Selection.Text = commonMethods.RandomString(1048576);
        }

        private void lvTrashCan_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void increaseFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatIncreaseFontSize(rtbEntry);
        }

        private void decreaseFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatDecreaseFontSize(rtbEntry);
        }

        private void toolStripMenuItem24_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            findAndReplace();
        }

        public void findAndReplace()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (myFormFind != null)
                return;

            myFormFind = new FormFind();
            myFormFind.myParentForm = this;
            myFormFind.rtb = rtbEntry;
            myFormFind.rtbSearchPattern.Text = rtbEntry.Selection.Text;
            myFormFind.Show(this);
        }

        private void searchAllEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlJournal.SelectedIndex = tabControlJournal.TabPages.IndexOfKey("TabPageSearch");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (stateChanged)
            {
                if (MessageBox.Show(this, "do you wish to save the currently active changed entry?", "question",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    saveCloseDB();
            }

            Application.Exit();
        }

        public void saveCloseDB()
        {
            saveEntry();
            closeContext();
        }

        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            if (menuItem.Checked)
                menuItem.Checked = false;
            else
                menuItem.Checked = true;

            userInterface.GoFullscreen(this, menuItem.Checked);

        }

        private void buttonSearchResetDates_Click(object sender, EventArgs e)
        {
            dtpickerSearchFrom.Value = DateTime.Now;
            dtpickerSearchThrough.Value = DateTime.Now;
            dtpickerSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerSearchThroughTime.Value = DateTime.Parse("23:59:59");
        }

        private void toolStripMenuItem33_Click(object sender, EventArgs e)
        {
        }

        private void bgWorkerSingleDBToFSDB_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            exportSet(DatabaseType.SingleFileDB, false);
        }

        public void exportSet(DatabaseType dbType, bool checkedNodesSet)
        {
            // firstly save entry
            saveEntry();

            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            String destPath = "";
            String dbName = "";
            if (userInterface.ShowInputDialog("input set name/title", ref dbName) != DialogResult.OK) return;
            if (dbName.Length <= 0) return;

            // destination set db
            switch (dbType)
            {
                case DatabaseType.OpenFSDB:
                    if (browseFolder.ShowDialog(this) != DialogResult.OK)
                        return;

                    destPath = browseFolder.SelectedPath;

                    break;

                case DatabaseType.SingleFileDB:
                    sfdDB.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    if (sfdDB.ShowDialog() != DialogResult.OK)
                        return;

                    destPath = sfdDB.FileName;

                    break;
                default:
                    return;
            }

            this.Invoke(toggleForm, false);

            // what to export
            List<myNode>? selectedNodes = null;
            if (checkedNodesSet)
                selectedNodes = (List<myNode>)this.Invoke(getHighestCheckedTreeViewItemsDBNodes, tvEntries);
            else
                selectedNodes = getRootNodes();

            // export selected nodes set
            entryMethods.ExportSet(this, ref cfg, ref allNodes, ref selectedNodes, dbName, destPath, dbType, true);

            // reload without saving currently selected node because it was saved previously.
            reloadDB(false);

            this.Invoke(toggleForm, true);
        }

        public void reloadDB(bool save)
        {
            if (cfg.radCfgUseOpenFileSystemDB)
                loadDB(cfg.ctx1.dbBasePath, save);
            else
                loadDB(cfg.ctx0.dbpath, save);
        }
        public List<myNode> getRootNodes()
        {
            List<myNode> list = new List<myNode>();

            foreach (TreeNode treeNode in tvEntries.Nodes)
            {

                // load node
                Int64 id = Int64.Parse(treeNode.Name);
                myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
                if (node == null) return list;

                list.Add(node);
            }
            return list;
        }
        public List<TreeNode> __createAncestorList(TreeNode? treeNode)
        {
            List<TreeNode> list = new List<TreeNode>();
            if (treeNode == null) return list;

            while (treeNode.Parent != null)
            {
                list.Add(treeNode.Parent);
                treeNode = treeNode.Parent;
            }
            return list;
        }

        public bool __isHighestCheckedTreeViewItem(TreeNode? treeNode)
        {
            List<TreeNode> list = __createAncestorList(treeNode);

            foreach (TreeNode node in list)
            {
                if (node.Checked)
                    return false;
            }
            return true;
        }
        public List<TreeNode> __getTreeNodeChildrenRecursive(TreeNode? treeNode)
        {
            List<TreeNode> list = new List<TreeNode>();
            if (treeNode == null) return list;
            Queue<TreeNode> queue = new Queue<TreeNode>();

            foreach (TreeNode node in treeNode.Nodes)
                queue.Enqueue(node);

            while (queue.Count > 0)
            {
                TreeNode currentNode = queue.Dequeue();
                TreeNodeCollection children = currentNode.Nodes;

                foreach (TreeNode childNode in children)
                    queue.Enqueue(childNode);

                list.Add(currentNode);
            }
            return list;
        }
        public List<TreeNode> __getCheckedTreeViewItems(TreeView? tv)
        {
            List<TreeNode> list = new List<TreeNode>();
            if (tv == null) return list;
            Queue<TreeNode> queue = new Queue<TreeNode>();

            foreach (TreeNode node in tvEntries.Nodes)
                queue.Enqueue(node);

            while (queue.Count > 0)
            {
                TreeNode currentNode = queue.Dequeue();
                TreeNodeCollection children = currentNode.Nodes;

                foreach (TreeNode childNode in children)
                    queue.Enqueue(childNode);

                if (currentNode.Checked)
                    list.Add(currentNode);
            }
            return list;
        }
        public List<myNode> __getCheckedTreeViewItemsDBNodes(TreeView? tv)
        {
            List<myNode> list = new List<myNode>();
            if (tv == null) return list;
            List<TreeNode> treeviewItems = __getCheckedTreeViewItems(tv);

            foreach (TreeNode treeNode in treeviewItems)
            {
                Int64 id = Int64.Parse(treeNode.Name);
                myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
                if (node != null)
                    list.Add(node);
            }
            return list;
        }


        public List<TreeNode> __getHighestCheckedTreeViewItems(TreeView? tv)
        {
            List<TreeNode> list = new List<TreeNode>();
            if (tv == null) return list;
            Queue<TreeNode> queue = new Queue<TreeNode>();

            foreach (TreeNode node in tv.Nodes)
                queue.Enqueue(node);

            while (queue.Count > 0)
            {
                TreeNode currentNode = queue.Dequeue();
                TreeNodeCollection children = currentNode.Nodes;

                foreach (TreeNode childNode in children)
                    queue.Enqueue(childNode);

                if (currentNode.Checked)
                {
                    if (__isHighestCheckedTreeViewItem(currentNode))
                        list.Add(currentNode);
                }
            }
            return list;
        }
        public List<myNode> __getHighestCheckedTreeViewItemsDBNodes(TreeView? tv)
        {
            List<TreeNode> treeviewItems = __getHighestCheckedTreeViewItems(tv);
            List<myNode> list = new List<myNode>();

            foreach (TreeNode treeNode in treeviewItems)
            {
                Int64 id = Int64.Parse(treeNode.Name);
                myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
                if (node != null)
                    list.Add(node);
            }
            return list;
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            importSet(DatabaseType.SingleFileDB);
        }

        public void importSet(DatabaseType dbType)
        {
            // firstly save entry
            saveEntry();

            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // source db
            String srcPath = "";
            string? dbName = "";
            switch (dbType)
            {
                case DatabaseType.OpenFSDB:
                    {
                        if (browseFolder.ShowDialog(this) != DialogResult.OK)
                            return;

                        // load source db and get config
                        OpenFSDBContext ctx = new OpenFSDBContext();
                        if (!OpenFileSystemDB.CreateLoadDB(browseFolder.SelectedPath, "", ref ctx, false, false))
                        {
                            this.Invoke(showMessageBox, "error loading set db or invalid (non-db) path.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // assign found db/set name as default value in input box
                        dbName = ctx.dbConfig.setName;

                        // close the db
                        ctx.close();

                        if (userInterface.ShowInputDialog("input new clone set name/title", ref dbName) != DialogResult.OK)
                            return;

                        if (dbName.Length <= 0) return;
                        srcPath = browseFolder.SelectedPath;

                        break;
                    }
                case DatabaseType.SingleFileDB:
                    {
                        if (ofdDB.ShowDialog() != DialogResult.OK)
                            return;

                        // load source db and get config
                        SingleFileDBContext ctx = new SingleFileDBContext();
                        if (!SingleFileDB.CreateLoadDB(ofdDB.FileName, "", ref ctx, false, false))
                        {
                            this.Invoke(showMessageBox, "error loading set db or invalid (non-db) path.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // assign found db/set name as default value in input box
                        dbName = ctx.dbConfig.setName;

                        // close the db
                        ctx.close();

                        if (userInterface.ShowInputDialog("input new clone set name/title", ref dbName) != DialogResult.OK)
                            return;

                        if (dbName.Length <= 0) return;
                        srcPath = ofdDB.FileName;

                        break;
                    }
                default:
                    return;
            }

            // import source set db
            this.Invoke(toggleForm, false);
            entryMethods.ImportSet(this, ref cfg, ref allNodes, srcPath, dbName, dbType, true);

            // reload without saving currently selected node because it was saved previously.
            reloadDB(false);

            this.Invoke(toggleForm, true);

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            exportSet(DatabaseType.SingleFileDB, true);
        }

        private void newLabelNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doNewLabelEntry();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            gotoSystemNode(NodeType.LibraryNode);
        }

        public void gotoSystemNode(NodeType nodeType)
        {
            // auto create/load system nodes
            mySystemNodes? systemNodes = null;
            //List<myNode> allNodes = entryMethods.DBFindAllNodes(cfg, false, false);
            if (!entryMethods.autoCreateLoadSystemNodes(ref cfg, ref allNodes, out systemNodes))
                return;

            switch (nodeType)
            {
                case NodeType.LibraryNode:
                    gotoTreeNodeByDBNode(ref systemNodes.LibrarySystemNode);
                    break;

                case NodeType.JournalNode:
                    gotoTreeNodeByDBNode(ref systemNodes.JournalSystemNode);
                    break;

                default:
                    break;

            }
        }
        public void gotoTreeNodeByDBNode(ref myNode? node)
        {
            if (node == null)
                return;

            String path = String.Format(@"{0}", node.chapter.Id);
            TreeNode[] matchingNodes = tvEntries.Nodes.Find(path, true);
            if (matchingNodes.Length <= 0)
                return;

            TreeNode treeNode = matchingNodes[0];

            tvEntries.Focus();
            tvEntries.SelectedNode = treeNode;
            treeNode.ExpandAll();
            treeNode.EnsureVisible();
        }

        private void tsbuttonLibrary_Click(object sender, EventArgs e)
        {
            gotoSystemNode(NodeType.LibraryNode);

        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            gotoSystemNode(NodeType.JournalNode);

        }

        private void tsbuttonJournal_Click(object sender, EventArgs e)
        {
            gotoSystemNode(NodeType.JournalNode);

        }

        public void __exportEntries(EntryType entryType, bool loadOperationForm)
        {
            // first save the entry
            this.Invoke(saveEntry);

            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            if (loadOperationForm)
                formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // first get the total number of chapters which exist in db
            long allNodesCount = allNodes.LongCount();

            // now get the custom checked root nodes
            List<myNode> checkedNodes = (List<myNode>)this.Invoke(getHighestCheckedTreeViewItemsDBNodes, tvEntries);

            // load tree document object model structure
            myTreeDom treeDom = new myTreeDom();
            treeDom.buildCustomTree(ref allNodes, ref checkedNodes, true, false);
            List<myTreeDomNode> tree = treeDom.ToList();
            long nodesCount = tree.LongCount();
            Int64 exportIndex = 0;

            foreach (myTreeDomNode listedNode in tree)
            {
                // load the rtf from current node
                myNode? node = listedNode.self;

                // straightaway export the entry
                entryMethods.exportEntry(cfg, ref node, browseFolder.SelectedPath, false, exportIndex, entryType);

                // update ui
                if (loadOperationForm)
                {
                    formOperation.updateProgressBar(exportIndex, nodesCount);
                    formOperation.updateFilesStatus(exportIndex, nodesCount);
                }

                // update
                exportIndex++;
            }

            // entire tree structure export completed. now final update and exit.
            this.Invoke(toggleForm, true);

            if (loadOperationForm)
                formOperation.close();

            this.Invoke(showMessageBox, "total entries exported:" + exportIndex, "done", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        private void bgWorkerExportEntries_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            EntryType entryType = (EntryType)e.Argument;
            __exportEntries(entryType, true);
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            doExportCheckedEntries(EntryType.Txt);
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            doExportCheckedEntries(EntryType.Rtf);
        }

        private void promoteNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            promoteNode();
        }

        public void promoteNode()
        {
            if (tvEntries.SelectedNode == null)
                return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            if (!entryMethods.DBPromoteNode(cfg, ref allNodes, ref node))
                return;

            // rebuild lineage
            node.lineage = entryMethods.findBottomToRootNodesRecursive(ref allNodes, ref node, false, false, true, false);

            TreeNode treeNode = tvEntries.SelectedNode;
            TreeNode parentTreeNode = treeNode.Parent;
            if (parentTreeNode == null) // node is already in root of tree.
                return;

            TreeNode ancestorTreeNode = parentTreeNode.Parent;
            treeNode.Remove();
            if (ancestorTreeNode == null)
            {
                // ancestor is root, so move in root
                tvEntries.Nodes.Add(treeNode);
            }
            else
            {
                // ancestor is some tree node, so move in it.
                ancestorTreeNode.Nodes.Add(treeNode);
            }

            tvEntries.SelectedNode = treeNode;
            treeNode.ExpandAll();
            treeNode.EnsureVisible();
        }
        public void promoteNodeToRoot()
        {
            if (tvEntries.SelectedNode == null)
                return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            if (!entryMethods.DBPromoteNodeToRoot(cfg, ref node))
                return;

            // rebuild lineage
            node.lineage = entryMethods.findBottomToRootNodesRecursive(ref allNodes, ref node, false, false, true, false);

            TreeNode treeNode = tvEntries.SelectedNode;
            TreeNode parentTreeNode = treeNode.Parent;
            if (parentTreeNode == null) // node is already in root of tree.
                return;

            // move to root of tree
            treeNode.Remove();
            tvEntries.Nodes.Add(treeNode);
            tvEntries.SelectedNode = treeNode;
            treeNode.ExpandAll();
            treeNode.EnsureVisible();

        }

        private void moveNodeToRootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            promoteNodeToRoot();
        }

        private void promoteNodeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            promoteNode();
        }

        private void moveNodeToRootToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            promoteNodeToRoot();
        }

        public void __moveNodeTo(TreeNode? subjectTreeNode)
        {
            FormTree form = new FormTree();
            form.allNodes = allNodes;
            if (form.ShowDialog() != DialogResult.OK)
                return;

            // resetup newly updated tree into the primary tree work list.
            allNodes = form.allNodes;

            TreeNode? targetTreeNode = form.tvEntries.SelectedNode;
            if (targetTreeNode == null) // no node selected, abort.
                return;

            Int64 targetNodeId = Int64.Parse(targetTreeNode.Name);
            myNode? targetNode = entryMethods.FindNodeInList(ref allNodes, targetNodeId);
            if (targetNode == null)
                return;

            Int64 subjectNodeId = Int64.Parse(subjectTreeNode.Name);
            myNode? subjectNode = entryMethods.FindNodeInList(ref allNodes, subjectNodeId);
            if (subjectNode == null)
                return;

            if (subjectNodeId == targetNodeId)
                return; // same target node selected as the subject node, so abort.

            if (!entryMethods.DBSetNodeParent(cfg, ref subjectNode, targetNodeId))
                return;

            // rebuild lineage
            subjectNode.lineage = entryMethods.findBottomToRootNodesRecursive(ref allNodes, ref subjectNode, false, false, true, false);

            if (__findTreeNode(targetNodeId, out targetTreeNode))
            {
                // target tree node found in current tree nodes
                subjectTreeNode.Remove();
                targetTreeNode.Nodes.Add(subjectTreeNode);
                tvEntries.SelectedNode = subjectTreeNode;
                subjectTreeNode.ExpandAll();
                subjectTreeNode.EnsureVisible();
            }
        }
        public void doMoveNodeTo()
        {
            if (tvEntries.SelectedNode == null)
                return;

            TreeNode? subjectTreeNode = tvEntries.SelectedNode;

            __moveNodeTo(subjectTreeNode);
        }
        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            doMoveNodeTo();
        }

        private void moveNodeToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doMoveNodeTo();
        }

        private void exportEntryAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode? treeNode = tvEntries.SelectedNode;
            if (treeNode == null)
                return;

            Int64 id = Int64.Parse(treeNode.Name);

            doExportCustomEntry(id);
        }

        public void doExportCustomEntry(Int64 id)
        {
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            sfdFile.Filter = "*.rtf|*.rtf|*.txt|*.txt|*.html|*.html|*.pdf|*.pdf";
            sfdFile.FilterIndex = 1;
            sfdFile.Title = "save as";
            sfdFile.FileName = entryMethods.getEntryLabel(node, true);

            if (sfdFile.ShowDialog() != DialogResult.OK)
                return;

            if (sfdFile.FileName.Length <= 0)
                return;

            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            EntryType entryType = EntryType.Rtf;
            entryMethods.getEntryTypeFormatsByFileName(sfdFile.FileName, ref entryType, ref ext, ref extComplete, ref extSearchPattern);

            // straightaway export the entry
            if (entryMethods.exportEntry(cfg, ref node, sfdFile.FileName, true, 0, entryType))
                MessageBox.Show(this, "entry exported", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            TreeNode? treeNode = tvEntries.SelectedNode;
            if (treeNode == null)
                return;

            Int64 id = Int64.Parse(treeNode.Name);


            doExportCustomEntry(id);
        }

        private void timeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatInsertTime(rtbEntry, DateTime.Now);
        }

        private void dateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatInsertDate(rtbEntry, DateTime.Now);
        }

        private void toolStripMenuItem25_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null) return;

            clearHighlight(tvEntries.SelectedNode, ref node);
        }

        public void clearHighlight(TreeNode treeNode, ref myNode? node)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            entryMethods.setEntryClearHighlight(cfg, ref node);
            loadNodeHighlight(cfg, treeNode, ref node);
        }

        private void toolStripMenuItem26_Click(object sender, EventArgs e)
        {
            highlightCheckedEntries(true);
        }

        private void checkmarkAllNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckEntireTreeView(true);
        }

        private void uncheckAllNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckEntireTreeView(false);
        }

        private void checkAllNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckEntireTreeView(true);
        }

        private void uncheckAllNodesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CheckEntireTreeView(false);
        }

        private void toolStripMenuItem27_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            DateTime inputDate = DateTime.Now;
            if (userInterface.ShowDateTimeDialog("custom time", ref inputDate, CalendarEntries.BoldedDates) != DialogResult.OK)
                return;

            formatting.formatInsertTime(rtbEntry, inputDate);

        }

        private void toolStripMenuItem28_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            DateTime inputDate = DateTime.Now;
            if (userInterface.ShowDateTimeDialog("custom date", ref inputDate, CalendarEntries.BoldedDates) != DialogResult.OK)
                return;

            formatting.formatInsertDate(rtbEntry, inputDate);


        }

        private void toolStripMenuItem29_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            DateTime inputDate = DateTime.Now;
            if (userInterface.ShowDateTimeDialog("custom date and time", ref inputDate, CalendarEntries.BoldedDates) != DialogResult.OK)
                return;

            formatting.formatInsertDateTime(rtbEntry, inputDate);


        }

        private void checkpointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // checkpoint
            entryMethods.DBCheckpoint(ref cfg);

            // finally update the db index in file.
            entryMethods.DBWriteIndexing(ref cfg);

            MessageBox.Show("checkpoint done", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripMenuItem30_Click(object sender, EventArgs e)
        {
            formatting.formatForceRemoveAllFormatting(rtbEntry);
        }

        private void toolStripMenuItem31_Click(object sender, EventArgs e)
        {
            doNewLabelEntry(true);
        }

        private void toolStripMenuItem32_Click(object sender, EventArgs e)
        {
            doNewLabelEntry();
        }

        private void toolStripMenuItem33_Click_1(object sender, EventArgs e)
        {
            doNewLabelEntry(true);
        }

        private void openDatabaseLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (cfg.radCfgUseOpenFileSystemDB)
            {
                // opens the folder in explorer
                System.Diagnostics.Process.Start("explorer.exe", cfg.ctx1.dbBasePath);
            }
            else
            {
                // opens the folder in explorer
                System.Diagnostics.Process.Start("explorer.exe", cfg.ctx0.dbBasePath);
            }
        }

        private void upgradeOldVersionDbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myConfig cfg = new myConfig();

            browseFolder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
            if (browseFolder.ShowDialog() == DialogResult.OK)
            {
                if (!OpenFileSystemDB.LoadDB(ref cfg.ctx1, browseFolder.SelectedPath))
                {
                    this.Invoke(showMessageBox, "error loading db or invalid (non-db) path.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                return;
            }

            cfg.radCfgUseOpenFileSystemDB = true;
            cfg.radCfgUseSingleFileDB = false;

            // collect all source tree and nodes from currently active db
            List<myNode>? allNodes = entryMethods.DBFindAllNodes(cfg, false, false);

            // first get the total number of chapters which exist in db
            long nodesCount = allNodes.LongCount();

            // load tree document object model structure
            myTreeDom treeDom = new myTreeDom();
            treeDom.buildTree(ref allNodes, true, false);
            List<myTreeDomNode> tree = treeDom.ToList();

            foreach (myTreeDomNode listedNode in tree)
                OpenFileSystemDB.updateNode(cfg.ctx1, ref listedNode.self, "", false, false);

            this.Invoke(showMessageBox, "done.", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void toolStripMenuItem34_Click(object sender, EventArgs e)
        {
            doExportCheckedEntries(EntryType.Pdf);
        }

        private void closeAllTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeOpenAllTree(false);
        }

        public void closeOpenAllTree(bool expand = true)
        {
            if (expand)
                tvEntries.ExpandAll();
            else
                tvEntries.CollapseAll();
        }

        private void expandAllTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeOpenAllTree(true);
        }

        private void expandSelectedNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeOpenNode(tvEntries.SelectedNode, true);
        }

        public void closeOpenNode(TreeNode? node, bool expand = true)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (node == null)
                return;

            if (expand)
                node.ExpandAll();
            else
                node.Collapse();
        }

        private void toolStripMenuItem36_Click(object sender, EventArgs e)
        {
            closeOpenNode(tvEntries.SelectedNode, false);
        }

        private void closeAllTreeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            closeOpenAllTree(false);
        }

        private void expandAllTreeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            closeOpenAllTree(true);
        }

        private void closeSelectedNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeOpenNode(tvEntries.SelectedNode, false);
        }

        private void expandSelectedNodeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            closeOpenNode(tvEntries.SelectedNode, true);
        }

        private void buttonSearchResetCDates_Click(object sender, EventArgs e)
        {
            dtpickerCDSearchFrom.Value = DateTime.Now;
            dtpickerCDSearchThrough.Value = DateTime.Now;
            dtpickerCDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerCDSearchThroughTime.Value = DateTime.Parse("23:59:59");
        }

        private void buttonSearchResetMDates_Click(object sender, EventArgs e)
        {
            dtpickerMDSearchFrom.Value = DateTime.Now;
            dtpickerMDSearchThrough.Value = DateTime.Now;
            dtpickerMDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerMDSearchThroughTime.Value = DateTime.Parse("23:59:59");
        }

        private void tsbuttonOpenSEParent_Click(object sender, EventArgs e)
        {
            Int64 id = getSelectedListViewNodeId(lvSearch);
            OpenSearchedEntry(id, true);
        }

        private void toolStripMenuItem37_Click(object sender, EventArgs e)
        {
            setSelectedNodeCommonDateTime();
        }

        public void setSelectedNodeCommonDateTime()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            DateTime inputDate = node.chapter.chapterDateTime;
            if (userInterface.ShowDateTimeDialog("edit entry's date and time", ref inputDate, CalendarEntries.BoldedDates) != DialogResult.OK)
                return;

            // update node in db
            entryMethods.DBSetNodeCommonDateTime(ref cfg, ref node, inputDate);

            // resetup tree node
            String entryName = "";
            entryName = entryMethods.getEntryLabel(node, false);
            tvEntries.SelectedNode.Text = entryName;

        }

        private void entryCommonDateAndTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setSelectedNodeCommonDateTime();
        }

        public void sortRootNodes(TreeView? tv, TreeNodeCollection collection, bool sortById, bool sortByIdExtra,
        bool sortByCommonDateTime, bool sortByCreationDateTime, bool sortByModificationDateTime,
        bool descending = false)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // first save the entry
            saveEntry();

            if (tv == null) return;
            if (collection == null) return;
            if (collection.Count == 0) return;

            List<myNode> nodes = getNodesFromTreeNodes(collection, ref allNodes);
            if (nodes.Count == 0) return;

            // now sort nodes
            entryMethods.sortNodes(ref nodes, sortById, sortByIdExtra, sortByCommonDateTime, sortByCreationDateTime, sortByModificationDateTime, descending);

            // finally get treenodes sequence based on nodes sequence
            List<TreeNode> affectedTreeNodes = getTreeNodeListFromNodes(collection, ref nodes);
            if (affectedTreeNodes.Count == 0) return;

            tv.BeginUpdate();

            // now remove all affected tree nodes
            foreach (TreeNode child in affectedTreeNodes)
                child.Remove();

            // now finally refill affected tree nodes into the treeview node based on the sorted sequence
            foreach (TreeNode child in affectedTreeNodes)
                tv.Nodes.Add(child);

            tv.EndUpdate();
        }

        public void sortTreeNodeFirstLevelChildren(TreeView? tv, TreeNode? treeNode, bool sortById, bool sortByIdExtra,
            bool sortByCommonDateTime, bool sortByCreationDateTime, bool sortByModificationDateTime,
            bool descending = false)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // first save the entry
            saveEntry();

            if (tv == null) return;
            if (treeNode == null) return;

            List<myNode> nodes = getNodesFromTreeNodes(treeNode.Nodes, ref allNodes);
            if (nodes.Count == 0) return;

            // now sort nodes
            entryMethods.sortNodes(ref nodes, sortById, sortByIdExtra, sortByCommonDateTime, sortByCreationDateTime, sortByModificationDateTime, descending);

            // finally get treenodes sequence based on nodes sequence
            List<TreeNode> affectedTreeNodes = getTreeNodeListFromNodes(treeNode.Nodes, ref nodes);
            if (affectedTreeNodes.Count == 0) return;

            tv.BeginUpdate();

            // now remove all affected tree nodes
            foreach (TreeNode child in affectedTreeNodes)
                child.Remove();

            // now finally refill affected tree nodes into the treeview node based on the sorted sequence
            foreach (TreeNode child in affectedTreeNodes)
                treeNode.Nodes.Add(child);

            tv.EndUpdate();
        }

        public List<myNode> getNodesFromTreeNodes(TreeNodeCollection collection, ref List<myNode> allNodes)
        {
            List<myNode> nodes = new List<myNode>();

            if (collection == null) return nodes;
            if (collection.Count == 0) return nodes;

            foreach (TreeNode child in collection)
            {
                Int64 id = Int64.Parse(child.Name);
                myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
                if (node != null) nodes.Add(node);
            }
            return nodes;
        }

        public List<TreeNode> getTreeNodeListFromNodes(TreeNodeCollection collection, ref List<myNode> nodes)
        {
            List<TreeNode> treenodes = new List<TreeNode>();

            if (collection == null) return treenodes;
            if (collection.Count == 0) return treenodes;

            foreach (myNode? node in nodes)
            {
                TreeNode? foundNode = findTreeNodeByNodeId(collection, node.chapter.Id);
                if (foundNode == null) continue;

                // treenode found
                treenodes.Add(foundNode);
            }
            return treenodes;
        }

        public TreeNode? findTreeNodeByNodeId(TreeNodeCollection collection, Int64 nodeId)
        {
            if (collection == null) return null;
            if (collection.Count == 0) return null;

            foreach (TreeNode child in collection)
            {
                Int64 id = Int64.Parse(child.Name);
                if (id == nodeId)
                    return child; // tree node found by node id
            }
            // not found
            return null;
        }

        private void toolStripMenuItem38_Click(object sender, EventArgs e)
        {
            FormSortOptions form = new FormSortOptions();
            if (form.ShowDialog() != DialogResult.OK) return;

            doSortTreeNodeFirstLevelChildren(tvEntries, tvEntries.SelectedNode, ref form);
        }

        public void doSortTreeNodeFirstLevelChildren(TreeView? tv, TreeNode? treeNode, ref FormSortOptions? form)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // first save the entry
            saveEntry();

            if (form == null) return;
            if (tv == null) return;
            if (treeNode == null) return;

            sortTreeNodeFirstLevelChildren(tv, treeNode, form.sortById, form.sortByIdExtra,
                form.sortByCommonDateTime, form.sortByCreationDateTime, form.sortByModificationDateTime,
                form.descending);
        }
        public void doSortTreeNodeRecursive(TreeView? tv, TreeNode? treeNode, ref FormSortOptions? form)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // first save the entry
            saveEntry();

            if (form == null) return;
            if (tv == null) return;
            if (treeNode == null) return;

            // first sort the parent treenode's first level children
            sortTreeNodeFirstLevelChildren(tv, treeNode, form.sortById, form.sortByIdExtra,
                form.sortByCommonDateTime, form.sortByCreationDateTime, form.sortByModificationDateTime,
                form.descending);

            // 2ndly sort all the tree of the parent treenode
            List<TreeNode> children = __getTreeNodeChildrenRecursive(treeNode);
            if (children.Count == 0) return;

            // sort entire tree into all children
            foreach (TreeNode childTreeNode in children)
            {
                sortTreeNodeFirstLevelChildren(tv, childTreeNode, form.sortById, form.sortByIdExtra,
                    form.sortByCommonDateTime, form.sortByCreationDateTime, form.sortByModificationDateTime,
                    form.descending);
            }
        }

        private void sortNodesChildrenRecursivelyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSortOptions form = new FormSortOptions();
            if (form.ShowDialog() != DialogResult.OK) return;

            doSortTreeNodeRecursive(tvEntries, tvEntries.SelectedNode, ref form);
        }

        public void doSortFirstLevelRootNodes(TreeView? tv, ref FormSortOptions? form)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // first save the entry
            saveEntry();

            if (form == null) return;
            if (tv == null) return;

            // sort all root nodes
            sortRootNodes(tv, tv.Nodes, form.sortById, form.sortByIdExtra,
                form.sortByCommonDateTime, form.sortByCreationDateTime, form.sortByModificationDateTime,
                form.descending);
        }
        private void sortFirstLevelRootNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSortOptions form = new FormSortOptions();
            if (form.ShowDialog() != DialogResult.OK) return;

            doSortFirstLevelRootNodes(tvEntries, ref form);
        }

        public void doSortRootNodesRecursive(TreeView? tv, ref FormSortOptions? form)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // first save the entry
            saveEntry();

            if (form == null) return;
            if (tv == null) return;

            // sort all root nodes
            sortRootNodes(tv, tv.Nodes, form.sortById, form.sortByIdExtra,
                form.sortByCommonDateTime, form.sortByCreationDateTime, form.sortByModificationDateTime,
                form.descending);

            // 2ndly sort all the root tree recursive
            foreach (TreeNode tn in tv.Nodes)
                doSortTreeNodeRecursive(tv, tn, ref form);
        }

        private void sortAllRootNodesRecursivelyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSortOptions form = new FormSortOptions();
            if (form.ShowDialog() != DialogResult.OK) return;

            doSortRootNodesRecursive(tvEntries, ref form);
        }

        public void doSortCheckedNodesFirstChildren(TreeView? tv, ref FormSortOptions? form)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // first save the entry
            saveEntry();

            if (form == null) return;
            if (tv == null) return;

            List<TreeNode> treeNodes = __getCheckedTreeViewItems(tv);

            foreach (TreeNode treeNode in treeNodes)
                doSortTreeNodeFirstLevelChildren(tv, treeNode, ref form);
        }

        private void sortCheckedNodesFirstChildrenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSortOptions form = new FormSortOptions();
            if (form.ShowDialog() != DialogResult.OK) return;

            doSortCheckedNodesFirstChildren(tvEntries, ref form);
        }
        public void doSortCheckedNodesRecursive(TreeView? tv, ref FormSortOptions? form)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // first save the entry
            saveEntry();

            if (form == null) return;
            if (tv == null) return;

            List<TreeNode> treeNodes = __getHighestCheckedTreeViewItems(tv);

            foreach (TreeNode treeNode in treeNodes)
                doSortTreeNodeRecursive(tv, treeNode, ref form);
        }

        private void sortCheckedNodesRecursivelyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSortOptions form = new FormSortOptions();
            if (form.ShowDialog() != DialogResult.OK) return;

            doSortCheckedNodesRecursive(tvEntries, ref form);
        }

        private void toolStripMenuItem40_Click(object sender, EventArgs e)
        {
            FormSortOptions form = new FormSortOptions();
            if (form.ShowDialog() != DialogResult.OK) return;

            doSortTreeNodeFirstLevelChildren(tvEntries, tvEntries.SelectedNode, ref form);
        }

        private void toolStripMenuItem41_Click(object sender, EventArgs e)
        {
            FormSortOptions form = new FormSortOptions();
            if (form.ShowDialog() != DialogResult.OK) return;

            doSortTreeNodeRecursive(tvEntries, tvEntries.SelectedNode, ref form);
        }

        private void toolStripMenuItem42_Click(object sender, EventArgs e)
        {
            FormSortOptions form = new FormSortOptions();
            if (form.ShowDialog() != DialogResult.OK) return;

            doSortFirstLevelRootNodes(tvEntries, ref form);
        }

        private void toolStripMenuItem43_Click(object sender, EventArgs e)
        {
            FormSortOptions form = new FormSortOptions();
            if (form.ShowDialog() != DialogResult.OK) return;

            doSortRootNodesRecursive(tvEntries, ref form);
        }

        private void toolStripMenuItem44_Click(object sender, EventArgs e)
        {
            FormSortOptions form = new FormSortOptions();
            if (form.ShowDialog() != DialogResult.OK) return;

            doSortCheckedNodesFirstChildren(tvEntries, ref form);
        }

        private void toolStripMenuItem45_Click(object sender, EventArgs e)
        {
            FormSortOptions form = new FormSortOptions();
            if (form.ShowDialog() != DialogResult.OK) return;

            doSortCheckedNodesRecursive(tvEntries, ref form);
        }

        private void buttonSearchResetDDates_Click(object sender, EventArgs e)
        {
            dtpickerDDSearchFrom.Value = DateTime.Now;
            dtpickerDDSearchThrough.Value = DateTime.Now;
            dtpickerDDSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerDDSearchThroughTime.Value = DateTime.Parse("23:59:59");
        }

        private void singleFileDbToOpenFilesystemDbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloneDB(DatabaseType.SingleFileDB, DatabaseType.OpenFSDB);
        }
        private void openFilesystemDbToSingleFileDbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloneDB(DatabaseType.OpenFSDB, DatabaseType.SingleFileDB);
        }

        public void CloneDB(DatabaseType srcDBType, DatabaseType destDBType, bool reindex = false)
        {
            // firstly save entry
            saveEntry();

            // source db
            String srcPath = "";
            String dbName = "";
            switch (srcDBType)
            {
                case DatabaseType.OpenFSDB:
                    {
                        if (browseFolder.ShowDialog(this) != DialogResult.OK)
                            return;

                        // load source db and get config
                        OpenFSDBContext ctx = new OpenFSDBContext();
                        if (!OpenFileSystemDB.CreateLoadDB(browseFolder.SelectedPath, "", ref ctx, false, false))
                        {
                            this.Invoke(showMessageBox, "error loading set db or invalid (non-db) path.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // assign found db/set name as default value in input box
                        dbName = ctx.dbConfig.setName;

                        // close the db
                        ctx.close();

                        if (userInterface.ShowInputDialog("input new clone db name/title", ref dbName) != DialogResult.OK)
                            return;

                        if (dbName.Length <= 0) return;
                        srcPath = browseFolder.SelectedPath;

                        break;
                    }
                case DatabaseType.SingleFileDB:
                    {
                        if (ofdDB.ShowDialog() != DialogResult.OK)
                            return;

                        // load source db and get config
                        SingleFileDBContext ctx = new SingleFileDBContext();
                        if (!SingleFileDB.CreateLoadDB(ofdDB.FileName, "", ref ctx, false, false))
                        {
                            this.Invoke(showMessageBox, "error loading set db or invalid (non-db) path.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // assign found db/set name as default value in input box
                        dbName = ctx.dbConfig.setName;

                        // close the db
                        ctx.close();

                        if (userInterface.ShowInputDialog("input new clone db name/title", ref dbName) != DialogResult.OK)
                            return;

                        if (dbName.Length <= 0) return;
                        srcPath = ofdDB.FileName;

                        break;
                    }
                default:
                    return;
            }

            // destination clone db
            String destPath = "";
            switch (destDBType)
            {
                case DatabaseType.OpenFSDB:
                    if (browseFolder.ShowDialog(this) != DialogResult.OK)
                        return;

                    destPath = browseFolder.SelectedPath;

                    break;

                case DatabaseType.SingleFileDB:
                    sfdDB.FileName = dbName;
                    if (sfdDB.ShowDialog() != DialogResult.OK)
                        return;

                    destPath = sfdDB.FileName;

                    break;
                default:
                    return;
            }

            // finally clone db from src to dest
            this.Invoke(toggleForm, false);
            entryMethods.CloneDB(this, srcPath, destPath, dbName, srcDBType, destDBType, true, reindex);
            this.Invoke(toggleForm, true);
        }

        private void create30000TestNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // first save the current entry
            saveEntry();

            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // operations status form
            FormOperation? formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);
            int index = 0;
            int total = 30000;

            for (index = 0; index < total; index++)
            {
                myNode node = new myNode();
                node.chapter.parentId = 0;
                node.chapter.chapterDateTime = DateTime.Now;
                node.chapter.nodeType = NodeType.LabelNode;
                node.chapter.Title = index.ToString();
                entryMethods.DBCreateNode(ref cfg, ref node, "", true, true, true, true, false, false);

                formOperation.updateProgressBar(index, total);
                formOperation.updateFilesStatus(index, total);

            }

            // finally update the db index in file.
            entryMethods.DBWriteIndexing(ref cfg);

            // checkpoint
            entryMethods.DBCheckpoint(ref cfg);

            formOperation.close();

            // now reload
            if (cfg.radCfgUseOpenFileSystemDB)
                loadDB(cfg.ctx1.dbBasePath);
            else
                loadDB(cfg.ctx0.dbpath);

        }

        private void toolStripMenuItem46_Click(object sender, EventArgs e)
        {
            exportSet(DatabaseType.OpenFSDB, false);
        }

        private void toolStripMenuItem47_Click(object sender, EventArgs e)
        {
            exportSet(DatabaseType.OpenFSDB, true);
        }

        private void toolStripMenuItem48_Click(object sender, EventArgs e)
        {
            importSet(DatabaseType.OpenFSDB);
        }

        private void exportAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            System.Windows.Forms.ListView lv = (System.Windows.Forms.ListView)cms.SourceControl;
            if (lv == null) return;
            if (lv.SelectedItems.Count == 0) return;
            System.Windows.Forms.ListViewItem lvitem = lv.SelectedItems[0];

            // load node
            Int64 id = Int64.Parse(lvitem.Name);
            //            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            //          if (node == null) return;

            //        Int64 id = Int64.Parse(treeNode.Name);

            doExportCustomEntry(id);

        }

        private void lvTrashCan_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem49_Click(object sender, EventArgs e)
        {
            doImportNonCalendarRtfEntries(false);
        }
        public void doImportNonCalendarRtfEntries(bool alienEntries)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            browseFolder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            string? input = "the journal non-calendar set";
            if (alienEntries)
                input = "other non-calendar import set";

            if (userInterface.ShowInputDialog("input new clone set name/title", ref input) != DialogResult.OK)
                return;

            if (input.Length <= 0)
                return;


            __importTheJournalNonCalendarRtfEntriesNew(browseFolder.SelectedPath, input, true);

        }

        public void __importTheJournalNonCalendarRtfEntriesNew(String path, String importSetName, bool loadOperationForm)
        {
            // first save the entry
            this.Invoke(saveEntry);

            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            if (loadOperationForm)
                formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            AdvRichTextBox rtb = new AdvRichTextBox();
            rtb.WordWrap = false;
            rtb.Multiline = true;
            rtb.SuspendLayout();
            rtb.BeginUpdate();

            IEnumerable<String> files = Directory.EnumerateFiles(path, "*.rtf");
            long index = 0;
            long total = files.LongCount();

            // first we need to create a set node. we cannot import anything at all without a set node.
            myNode? setNode = entryMethods.createSetNode(ref cfg, importSetName, DateTime.Now);

            // import set node
            entryMethods.DBCreateNode(ref cfg, ref setNode, "", false, false, false, false, true, true);

            // this set's own context session work list
            List<myNode> setList = new List<myNode>();

            foreach (String file in files)
            {
                // notice: any tree node level in entire tree cannot have more than 1 nodes with exactly same name/title.
                // this leads to destruction of nodes of same name and level while importing.
                // so you must ensure there is no duplicate named/titled node in any tree level.
                // if there is any duplicate node, you must give it a unique name/title which differs in all children of a tree node level.
                // also, each and every entry which is to be imported, must have a unique name/title in it's filename.

                // auto create direct all nodes line by node names which exist in the file name itself.
                List<String> nodeNames = theJournalMethods.partitionEntryFileIntoNodes(file);
                List<myNode> nodesLine = theJournalMethods.initNodesLineTJNC(ref cfg, ref setList, ref nodeNames, setNode.chapter.Id);
                myNode? targetNode = nodesLine.Last();

                // get rtf and update
                // richtextbox automatically cleans and fixes a corrupted rtf and makes it valid.
                // so we first load the imported rtf into a richtextbox object, then retrieve the cleaned and fixed
                // rtf from it and only then store it in db.
                String rtf = File.ReadAllText(file);
                rtf = theJournalMethods.fixTheJournalRtfEntry(rtf);
                rtb.Rtf = rtf;
                rtf = rtb.Rtf;

                // finally write body of the node in db
                entryMethods.DBUpdateNode(cfg, ref targetNode, rtf, true, true);

                // update ui
                if (loadOperationForm)
                {
                    formOperation.updateProgressBar(index, total);
                    formOperation.updateFilesStatus(index, total);
                }

                // update
                index++;
            }

            // checkpoint
            entryMethods.DBCheckpoint(ref cfg);

            // finally update the db index in file.
            entryMethods.DBWriteIndexing(ref cfg);

            // now first add set node
            allNodes.Add(setNode);

            // now add all set list into the global session work list
            allNodes.AddRange(setList);

            this.Invoke(toggleForm, true);

            if (loadOperationForm)
                formOperation.close();

            mySystemNodes? systemNodes = null;
            reloadAll(true, true, true, ref systemNodes);
        }

        private void toolStripMenuItem50_Click(object sender, EventArgs e)
        {
            doNewCustomNode(NodeType.EntryNode, false);
        }
        public void doNewCustomNode(NodeType nodeType, bool root = false)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            string? input = "";
            if (userInterface.ShowInputDialog("input node title (required)", ref input) != DialogResult.OK)
                return;

            DateTime inputDate = DateTime.Now;
            if (userInterface.ShowDateTimeDialog("new node's date and time", ref inputDate, CalendarEntries.BoldedDates) != DialogResult.OK)
                return;

            if (input.Length <= 0)
                return;

            if (!root)
            {
                if (tvEntries.SelectedNode == null)
                    return;

                Int64 parentId = Int64.Parse(tvEntries.SelectedNode.Name);
                setupNewEntry(inputDate, parentId, nodeType, input);
            }
            else
            {
                setupNewEntry(inputDate, -1, nodeType, input);
            }
        }

        private void toolStripMenuItem51_Click(object sender, EventArgs e)
        {
            doNewCustomNode(NodeType.EntryNode, true);
        }

        private void toolStripMenuItem52_Click(object sender, EventArgs e)
        {
            printEntry(ref rtbEntry, false);
        }

        public void printEntry(ref System.Windows.Controls.WpfRichTextBoxEx? rtb, bool printPreview = false)
        {
            // first save the entry
            this.Invoke(saveEntry);

            if (rtb == null) return;
            if (rtb.Text.Length == 0) return;
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen()) return;


            // init and configure
            printerRtb.Clear();
            printerRtb.Rtf = rtb.Rtf;

            // first page setup
            if (pdlgRtbEntry.ShowDialog() != DialogResult.OK) return;

            // 2nd print preview dialog
            if (printPreview)
                if (ppDlgRtbEntry.ShowDialog() != DialogResult.OK) return;

            // finally print
            //if ((pd.ShowDialog() == true))
            // {
            //     pd.PrintDocument((((IDocumentPaginatorSource)rtbEntry.Document).DocumentPaginator),
            //        "Print Document");
            // }

            printerRtb.PrintRichTextContents();
        }

        private void toolStripMenuItem53_Click(object sender, EventArgs e)
        {
            printEntry(ref rtbEntry, true);
        }

        private void toolStripMenuItem54_Click(object sender, EventArgs e)
        {
            CloneDB(DatabaseType.SingleFileDB, DatabaseType.OpenFSDB, true);
        }

        private void toolStripMenuItem55_Click(object sender, EventArgs e)
        {
            CloneDB(DatabaseType.OpenFSDB, DatabaseType.SingleFileDB, true);
        }

        private void toolStripMenuItem56_Click(object sender, EventArgs e)
        {
            CloneDB(DatabaseType.SingleFileDB, DatabaseType.SingleFileDB, true);
        }

        private void toolStripMenuItem57_Click(object sender, EventArgs e)
        {
            CloneDB(DatabaseType.OpenFSDB, DatabaseType.OpenFSDB, true);
        }

        /// <summary>
        /// Gets the text pointer at the given character offset.
        /// Each line break will count as 2 chars.
        /// </summary>
        /// <param name="richTextBox">The rich text box.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The TextPointer at the given character offset</returns>
        public static TextPointer GetTextPointerAtOffset(System.Windows.Controls.RichTextBox richTextBox, int offset)
        {
            var navigator = richTextBox.Document.ContentStart;
            int cnt = 0;

            while (navigator.CompareTo(richTextBox.Document.ContentEnd) < 0)
            {
                switch (navigator.GetPointerContext(LogicalDirection.Forward))
                {
                    case TextPointerContext.ElementStart:
                        break;
                    case TextPointerContext.ElementEnd:
                        if (navigator.GetAdjacentElement(LogicalDirection.Forward) is Paragraph)
                            cnt += 2;
                        break;
                    case TextPointerContext.EmbeddedElement:
                        // TODO: Find out what to do here?
                        cnt++;
                        break;
                    case TextPointerContext.Text:
                        int runLength = navigator.GetTextRunLength(LogicalDirection.Forward);

                        if (runLength > 0 && runLength + cnt < offset)
                        {
                            cnt += runLength;
                            navigator = navigator.GetPositionAtOffset(runLength);
                            if (cnt > offset)
                                break;
                            continue;
                        }
                        cnt++;
                        break;
                }

                if (cnt > offset)
                    break;

                navigator = navigator.GetPositionAtOffset(1, LogicalDirection.Forward);

            } // End while.

            return navigator;
        }

        private void timerSetRtbEntry_Tick(object sender, EventArgs e)
        {
            String rtf = "";

            Int64 id = (Int64)timerSetRtbEntry.Tag;
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
            {
                timerSetRtbEntry.Stop();
                timerSetRtbEntry.Enabled = false;
                return;
            }

            msJournal.Enabled = false;
            this.Enabled = false;

            // configure rtb document
            configureRtbDocument(rtbEntry, node.chapter.documentWidth, true);

            // create a new flowdocument and set it
            rtf = entryMethods.DBLoadNodeData(cfg, id, node.DirectorySectionID);

            // configure richtextbox
            try
            {
                rtbEntry.Rtf = rtf;
            }
            catch (Exception)
            {
                MessageBox.Show("error loading this entry's rtf. rtf corrupted and or contains invalid data.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            resetRtb(rtbEntry, false, true);

            txtEntryTitle.Text = node.chapter.Title;
            String entryPath = "";
            List<myNode>? lineage = null;
            if (entryMethods.GenerateLineagePath(ref node.lineage, ref node, out entryPath, out lineage))
                txtBoxEntryPath.Text = entryPath;

            tsslabelLMD.Text = node.chapter.modificationDateTime.ToString("HH:mm:ss:fff, dddd, dd MMMM yyyy");
            tsslabelID.Text = node.chapter.Id.ToString();
            tsslabelPID.Text = node.chapter.parentId.ToString();
            CalendarEntries.SelectionStart = node.chapter.chapterDateTime;
            CalendarEntries.SelectionEnd = node.chapter.chapterDateTime;

            this.Enabled = true;
            msJournal.Enabled = true;

            this.ActiveControl = host1;
            host1.Focus();
            rtbEntry.Focus();

            // now setup caret config
            rtbEntry.SelectionStartOffset = node.chapter.caretIndex;
            if (node.chapter.caretSelectionLength != 0)
                rtbEntry.SelectionLength = node.chapter.caretSelectionLength;

            // show caret position in the middle of richtextbox
            if (rtbEntry.Selection != null)
            {
                if (rtbEntry.Selection.Start != null)
                {
                    if (rtbEntry.Selection.Start.Paragraph != null)
                    {
                        if (rtbEntry.Selection.Start.Paragraph.NextBlock != null)
                        {
                            Block selNextBlock = rtbEntry.Selection.Start.Paragraph.NextBlock;
                            if (selNextBlock != null)
                            {
                                var characterRect = selNextBlock.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                                rtbEntry.ScrollToVerticalOffset(rtbEntry.VerticalOffset + characterRect.Top - rtbEntry.ActualHeight / 2d);
                            }
                        }
                    }
                }
            }

            timerSetRtbEntry.Stop();
            timerSetRtbEntry.Enabled = false;
        }
        public void configureRtbDocument(System.Windows.Controls.WpfRichTextBoxEx rtb, int width, bool clear)
        {
            // create a new flowdocument and set it with new width
            List<Block> blocks = rtbEntry.Document.Blocks.ToList();
            rtbEntry.Document.Blocks.Clear();
            rtbEntry.SpellCheck.IsEnabled = false;
            FlowDocument flowDoc = new FlowDocument();// new Paragraph(new Run("")));
            flowDoc.PageWidth = (double)width;
            flowDoc.ColumnWidth = 999999.0;
            flowDoc.FontFamily = new System.Windows.Media.FontFamily("Times New Roman");
            flowDoc.FontSize = 24.0; /// original 14.0;
            rtbEntry.Document = flowDoc;
            if (!clear)
                rtbEntry.Document.Blocks.AddRange(blocks);
        }
        private void insertColumnLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatInsertTableColumn(rtbEntry, true);
        }

        private void insertColumnRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatInsertTableColumn(rtbEntry, false);
        }

        private void insertRowAboveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatInsertTableRow(rtbEntry, true);
        }

        private void insertRowBelowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatInsertTableRow(rtbEntry, false);
        }

        private void toolStripMenuItem59_Click(object sender, EventArgs e)
        {
            formatting.formatDeleteTableColumn(rtbEntry);
        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatDeleteTableRow(rtbEntry);

        }

        private void toolStripMenuItem64_Click(object sender, EventArgs e)
        {
            formatting.formatDeleteTable(rtbEntry);
        }

        private void columnWidthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatSetColumnWidth(rtbEntry, false);
        }

        private void toolStripSeparator20_Click(object sender, EventArgs e)
        {

        }

        private void formatRowsCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatCells(rtbEntry);
        }

        private void removeFormattingOfCellsOfRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatCellsRemoveAllFormatting(rtbEntry);
        }

        private void formatColumnsAndTheirContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatColumns(rtbEntry, false);
        }

        private void removeFormattingOfEntireTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatTable(rtbEntry, true);
        }

        private void toolStripMenuItem58_Click(object sender, EventArgs e)
        {
            formatting.formatTable(rtbEntry, false);
        }

        private void toolStripMenuItem65_Click(object sender, EventArgs e)
        {
            formatting.formatSetColumnWidth(rtbEntry, true);
        }

        private void selectTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.selectTable(rtbEntry);
        }

        private void toolStripMenuItem66_Click(object sender, EventArgs e)
        {
            formatting.formatRows(rtbEntry, false);
        }

        private void toolStripMenuItem67_Click(object sender, EventArgs e)
        {
            formatting.formatRows(rtbEntry, true);
        }

        private void toolStripMenuItem68_Click(object sender, EventArgs e)
        {
            formatting.formatColumns(rtbEntry, true);
        }

        private void toolStripMenuItem69_Click(object sender, EventArgs e)
        {
            doImportNonCalendarRtfEntries(true);
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            rtbEntry.SelectAll();
        }

        private void paragraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatInsertParagraph(rtbEntry);
        }

        private void recoverSavedEntryFromDbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String rtf = "";
            if (tvEntries.SelectedNode == null) return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            this.Enabled = false;

            resetRtb(rtbEntry, true, true);
            rtf = entryMethods.DBLoadNodeData(cfg, id, node.DirectorySectionID);
            rtbEntry.Rtf = rtf;
            resetRtb(rtbEntry, false, true);

            txtEntryTitle.Text = node.chapter.Title;
            String entryPath = "";
            List<myNode>? lineage = null;
            if (entryMethods.GenerateLineagePath(ref allNodes, ref node, out entryPath, out lineage))
                txtBoxEntryPath.Text = entryPath;

            tsslabelLMD.Text = node.chapter.modificationDateTime.ToString("HH:mm:ss:fff, dddd, dd MMMM yyyy");
            tsslabelID.Text = node.chapter.Id.ToString();
            tsslabelPID.Text = node.chapter.parentId.ToString();
            CalendarEntries.SelectionStart = node.chapter.chapterDateTime;
            CalendarEntries.SelectionEnd = node.chapter.chapterDateTime;

            this.Enabled = true;

            this.ActiveControl = host1;
            host1.Focus();
            rtbEntry.Focus();

            // now setup caret config
            rtbEntry.SelectionStartOffset = node.chapter.caretIndex;
            if (node.chapter.caretSelectionLength != 0)
                rtbEntry.SelectionLength = node.chapter.caretSelectionLength;
        }
        private void configureEntrysWidthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doConfigureEntryWidth();
        }

        public void doConfigureEntryWidth()
        {
            if (tvEntries.SelectedNode == null)
                return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);

            configureEntryWidth(rtbEntry, id);
        }

        public void configureEntryWidth(System.Windows.Controls.WpfRichTextBoxEx rtb, Int64 id)
        {
            // first save the current entry
            saveEntry();

            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // collect node from work list
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            // prepare input box
            List<Object> valueStrings = new List<Object>();
            int width = myConfig.defaultDocumentWidth;
            for (int i = 0; i < 60; i++)
            {
                valueStrings.Add((Object)width.ToString());
                width += 100;
            }

            // ask user
            String input = node.chapter.documentWidth.ToString();
            if (userInterface.ShowListInputDialog("configure entry document width", ref input, ref valueStrings, -1) != DialogResult.OK)
                return;

            // check validity
            int value = -1;
            if (!int.TryParse(input, out value))
                return;

            // firstly set document width in db
            if (!entryMethods.DBSetNodeDocumentWidth(cfg, ref node, value))
                return;

            // finally setup document width
            // configure rtb document
            configureRtbDocument(rtbEntry, node.chapter.documentWidth, false);
        }

        public void __selectSearchNode()
        {

            FormTree form = new FormTree();
            form.allNodes = allNodes;
            if (form.ShowDialog() != DialogResult.OK)
                return;

            // resetup newly updated tree into the primary tree work list.
            allNodes = form.allNodes;

            List<myNode> selNodes = getHighestCheckedTreeViewItemsDBNodes(form.tvEntries);
            if (selNodes.Count == 0) return;

            // user selected a location to search
            foreach (myNode node in selNodes)
            {
                myNode? listedNode = node;

                // first check if this node exists
                ListViewItem[] found = lvSearchWhere.Items.Find(node.chapter.Id.ToString(), false);
                if (found.Length != 0)
                    continue;

                // now check if this node's ancestor exists
                foreach (ListViewItem listedItem in lvSearchWhere.Items)
                {
                    Int64 listedItemId = Int64.Parse(listedItem.Name);
                    if (entryMethods.IsAncestorNode(ref allNodes, listedItemId, node, false, false) == 1)
                        goto __selectSearchNodeContinue;
                }
                goto __selectSearchNodeDo;

            __selectSearchNodeContinue:
                // node has an ancestor in list, so skip this node
                continue;

            __selectSearchNodeDo:
                System.Windows.Forms.ListViewItem item = new System.Windows.Forms.ListViewItem();
                item.Name = node.chapter.Id.ToString();

                String entryPath = "";
                List<myNode>? lineage = null;
                if (entryMethods.GenerateLineagePath(ref listedNode.lineage, ref listedNode, out entryPath, out lineage))
                    item.Text = entryPath;

                // node's parent id
                item.SubItems.Add(node.chapter.parentId.ToString());

                // node's id
                item.SubItems.Add(node.chapter.Id.ToString());

                // set tag
                item.Tag = node;

                lvSearchWhere.Items.Add(item);
            }
        }

        private void buttonSearchLocation_Click(object sender, EventArgs e)
        {
            __selectSearchNode();
        }

        private void buttonSearchLocationReset_Click(object sender, EventArgs e)
        {
            lvSearchWhere.Tag = null;
            lvSearchWhere.Items.Clear();
        }

        private void toolStripMenuItem71_Click(object sender, EventArgs e)
        {
            formatting.formatSelectTableRow(rtbEntry);
        }

        private void toolStripMenuItem72_Click(object sender, EventArgs e)
        {
            formatting.formatStrikeout(rtbEntry, (ToolStripButton)tsbuttonStrikeout);
        }

        private void buttonRemoveSearchLocation_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvSearchWhere.CheckedItems)
                item.Remove();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("DiaryJournal.Net\nopen source free diary and journal software, uses x64 platform, .Net 8.0 and Visual Studio 2022.\nCopyright © 2022 - 2025, Tushar Jain\nsole developer: Tushar Jain", "about", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chkSearchReplace_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSearchReplace.Checked)
            {
                if (MessageBox.Show("warning: please confirm if you want to replace all matching items?", "warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    chkSearchReplace.Checked = false;
            }
        }

        private void toolStripMenuItem73_Click(object sender, EventArgs e)
        {
            formatting.formatPasteRawText(rtbEntry);
        }

        private void convertEntryToRawTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatConvertToRawText(rtbEntry);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            showMatchesForm();
        }

        public void showMatchesForm()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (myMatchesForm != null)
                return;

            myMatchesForm = new MatchesForm();
            myMatchesForm.myParentForm = this;
            //myMatchesForm.rtb = rtbEntry;
            //myMatchesForm.rtbSearchPattern.Text = rtbEntry.Selection.Text;
            myMatchesForm.Show(this);
        }

        private void toolStripMenuItem74_Click(object sender, EventArgs e)
        {
            formatting.cutParagraphRaw(rtbEntry);
        }

        private void cloneEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tvEntries.SelectedNode == null)
                return;

            cloneEntry(tvEntries.SelectedNode, -1);
        }

        public bool cloneEntry(TreeNode? treeNode, Int64 locationId)
        {
            if (treeNode == null) return false;
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen()) return false;

            // firstly save entry
            __saveEntry();

            Int64 id = Int64.Parse(treeNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return false;

            myNode? clone = entryMethods.DBCloneNode(ref cfg, ref allNodes, ref node, locationId, true, true, true);
            if (clone == null)
                return false;

            // setup tree node
            TreeNode? newTreeNode = entryMethods.InitializeNewTreeNode(ref clone);
            if (clone.chapter.parentId >= 1)
            {
                // add new clone node at it's parent's location
                TreeNode? locationTreeNode = null;
                __findTreeNode(clone.chapter.parentId, out locationTreeNode);
                if (locationTreeNode == null) // no parent tree node found 
                    return false;

                locationTreeNode.Nodes.Add(newTreeNode);
            }
            else
            {
                // add into treeview root
                __initTreeViewRootNode(newTreeNode);
            }

            // update
            this.Invoke(updateTotalEntriesStatus, cfg.totalNodes);
            tvEntries.SelectedNode = newTreeNode;
            newTreeNode.ExpandAll();
            newTreeNode.EnsureVisible();
            return true;
        }

        private void cloneAtParentLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tvEntries.SelectedNode == null)
                return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            if (node == null)
                return;

            Int64 ancestorId = 0;
            myNode? parentNode = null;
            Int64 parentId = node.chapter.parentId;
            if (parentId >= 1)
            {
                parentNode = entryMethods.FindNodeInList(ref allNodes, parentId);
                if (parentNode == null)
                    return;

                // get id of parent of parent
                ancestorId = parentNode.chapter.parentId;
            }

            // create clone node at parent's level
            cloneEntry(tvEntries.SelectedNode, ancestorId);
        }

        private void cloneToOtherLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tvEntries.SelectedNode == null)
                return;

            FormTree form = new FormTree();
            form.allNodes = allNodes;
            if (form.ShowDialog() != DialogResult.OK)
                return;

            // resetup newly updated tree into the primary tree work list.
            allNodes = form.allNodes;

            TreeNode? targetTreeNode = form.tvEntries.SelectedNode;
            if (targetTreeNode == null) // no node selected, abort.
                return;

            Int64 locationId = Int64.Parse(targetTreeNode.Name);

            //Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            //myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
            //if (node == null)
            //    return;

            // create clone node at other location
            cloneEntry(tvEntries.SelectedNode, locationId);
        }

        private void toolStripMenuItem75_Click(object sender, EventArgs e)
        {
            if (tvEntries.SelectedNode == null)
                return;

            // create clone node at root level
            cloneEntry(tvEntries.SelectedNode, 0);

        }

        private void linkCfgTVEntriesFont_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CustomFontDialog fontDialog = new CustomFontDialog();
            System.Drawing.Font font = commonMethods.StringToFont(linkCfgTVEntriesFont.Text);
            fontDialog.font = font;
            fontDialog.size = (int)font.Size;
            fontDialog.bold = font.Bold;
            fontDialog.italic = font.Italic;
            fontDialog.underline = font.Underline;
            fontDialog.strikeout = font.Strikeout;
            fontDialog.fontBackColor = linkCfgTVEntriesFont.BackColor;
            fontDialog.fontColor = linkCfgTVEntriesFont.ForeColor;

            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;

            Color backColor = Color.Empty;
            Color foreColor = Color.Empty;
            font = fontDialog.getNewFontComplete(out backColor, out foreColor);
            if (backColor == Color.Empty) backColor = myConfig.default_tvEntriesBackColor;
            if (foreColor == Color.Empty) foreColor = myConfig.default_tvEntriesForeColor;
            linkCfgTVEntriesFont.Text = commonMethods.FontToString(font);
            linkCfgTVEntriesFont.BackColor = backColor;
            linkCfgTVEntriesFont.ForeColor = foreColor;

        }

        private void buttonResetConfig1_Click(object sender, EventArgs e)
        {
            cmbCfgRtbViewEntryRM.SelectedIndex = cmbCfgRtbViewEntryRM.FindString(myConfig.default_cmbCfgRtbViewEntryRMValue.ToString());
            cmbCfgTVEntriesItemHeight.SelectedIndex = cmbCfgTVEntriesItemHeight.FindString(myConfig.default_tvEntriesItemHeight.ToString());
            cmbCfgTVEntriesIndent.SelectedIndex = cmbCfgTVEntriesIndent.FindString(myConfig.default_tvEntriesIndent.ToString());
            linkCfgTVEntriesFont.Text = commonMethods.FontToString(myConfig.default_tvEntriesFont);
            linkCfgTVEntriesFont.BackColor = myConfig.default_tvEntriesBackColor;
            linkCfgTVEntriesFont.ForeColor = myConfig.default_tvEntriesForeColor;

        }

        private void resetCheckedNodesRecursiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // firstly save entry
            __saveEntry();

            FormTreeResetOptions form = new FormTreeResetOptions();
            if (form.ShowDialog() != DialogResult.OK) return;

            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // phase 1: get all checked nodes
            List<myNode> checkedNodes = __getCheckedTreeViewItemsDBNodes(tvEntries);

            // customize commit nodes
            entryMethods.DBCustomizeTreeNodesRecursive(ref cfg, ref checkedNodes, false,
                form.resetFont, form.resetFontSize, form.resetItalics, form.resetBold, form.resetStrikeout, form.resetUnderline,
                form.resetBackColor, form.resetForeColor, Color.Empty, Color.Empty, "", -1);

            this.Invoke(toggleForm, true);
            formOperation.close();

            // finally reload/load db
            mySystemNodes? systemNodes = null;
            reloadAll(true, true, true, ref systemNodes);

        }

        private void gotoLatestEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // goto custom entry
            __gotoEntryByAttribute(false, true, false, false, -1);

        }

        private void gotoLatestLastModifiedEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // goto custom entry
            __gotoEntryByAttribute(true, false, false, false, -1);
        }

        private void gotoTodaysCreatedLatestEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // goto custom entry
            __gotoEntryByAttribute(false, false, true, false, -1);
        }

        private void gotoEntryByIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // firstly save entry
            __saveEntry();

            // prepare input box
            Int64 id = 1;
            List<Object> items = new List<Object>();
            foreach (myNode? listedNode in allNodes)
                items.Add((Object)listedNode.chapter.Id);

            Int64 total = items.LongCount();

            // ask user
            String input = 1.ToString();
            if (userInterface.ShowListInputDialog("enter id of an existing entry/node", ref input, ref items, -1) != DialogResult.OK)
                return;

            // check validity
            Int64 value = -1;
            if (!Int64.TryParse(input, out value))
                return;

            if (value <= 0) return;

            // goto custom entry
            __gotoEntryByAttribute(false, false, false, true, value);

        }

        private void resetAllDbNodesRecursiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // firstly save entry
            __saveEntry();

            FormTreeResetOptions form = new FormTreeResetOptions();
            if (form.ShowDialog() != DialogResult.OK) return;

            this.Invoke(toggleForm, false);

            // operations status form
            FormOperation? formOperation = null;
            formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // phase 1: get all nodes
            allNodes = entryMethods.DBFindAllNodes(cfg, false, false);
            mySystemNodes? systemNodes = null;
            entryMethods.autoCreateLoadSystemNodes(ref cfg, ref allNodes, out systemNodes);

            // customize commit nodes
            entryMethods.DBCustomizeTreeNodesRecursive(ref cfg, ref allNodes, false,
                form.resetFont, form.resetFontSize, form.resetItalics, form.resetBold, form.resetStrikeout, form.resetUnderline,
                form.resetBackColor, form.resetForeColor, Color.Empty, Color.Empty, "", -1);

            this.Invoke(toggleForm, true);
            formOperation.close();

            // finally reload/load db
            systemNodes = null;
            reloadAll(true, true, true, ref systemNodes);

        }

        private void toolStripMenuItem76_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            String xml = entryMethods.DBWriteConfig(ref cfg);
            if (xml != "")
                MessageBox.Show("upgraded db config to current product version", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void gotoParentByChildEntryIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // firstly save entry
            __saveEntry();

            // prepare input box
            Int64 id = 1;
            List<Object> items = new List<Object>();
            foreach (myNode? listedNode in allNodes)
                items.Add((Object)listedNode.chapter.Id);

            Int64 total = items.LongCount();

            // ask user
            String input = 1.ToString();
            if (userInterface.ShowListInputDialog("enter id of an existing entry/node", ref input, ref items, -1) != DialogResult.OK)
                return;

            // check validity
            Int64 value = -1;
            if (!Int64.TryParse(input, out value))
                return;

            if (value <= 0) return;

            __gotoParentByEntryID(value);
        }

        private void testsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        public void restoreState(int index)
        {
            switch (index)
            {
                case 0:
                    rtbEntry.Xaml = xamlState0;
                    break;
                case 1:
                    rtbEntry.Xaml = xamlState1;
                    break;
                case 2:
                    rtbEntry.Xaml = xamlState2;
                    break;
                case 3:
                    rtbEntry.Xaml = xamlState3;
                    break;
                case 4:
                    rtbEntry.Xaml = xamlState4;
                    break;
                case 5:
                    rtbEntry.Xaml = xamlState5;
                    break;
                case 6:
                    rtbEntry.Xaml = xamlState6;
                    break;
                case 7:
                    rtbEntry.Xaml = xamlState7;
                    break;
                case 8:
                    rtbEntry.Xaml = xamlState8;
                    break;
                case 9:
                    rtbEntry.Xaml = xamlState9;
                    break;

            }
        }

        private void state0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(0);
        }

        private void state1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(1);

        }

        private void state2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(2);

        }

        private void state3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(3);

        }

        private void state4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(4);

        }

        private void state5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(5);

        }

        private void state6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(6);

        }

        private void state7ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(7);

        }

        private void state8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(8);

        }

        private void state9latestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreState(9);

        }

        private void copyDbToLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (browseFolder.ShowDialog() != DialogResult.OK) return;

            // firstly save entry
            saveEntry();

            // operations status form
            FormOperation? formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // now copy/move db
            if (entryMethods.DBCopyDatabase(ref cfg, browseFolder.SelectedPath, true, true))
            {
                formOperation.close();
                MessageBox.Show("successfully copied/moved db! please manually reload the db.", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                formOperation.close();
                MessageBox.Show("error occured!", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // reset everything
            reset();
        }


        private void moveDbToLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (browseFolder.ShowDialog() != DialogResult.OK) return;

            // firstly save entry
            saveEntry();

            // operations status form
            FormOperation? formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // now copy/move db
            if (entryMethods.DBCopyDatabase(ref cfg, browseFolder.SelectedPath, false, true))
            {

                formOperation.close();
                MessageBox.Show("successfully copied/moved db! please manually reload the db.", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                formOperation.close();
                MessageBox.Show("error occured!", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // reset everything
            reset();
        }

        private void toolStripMenuItem78_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (MessageBox.Show("warning: are you sure you want to delete the loaded db?", "warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            // firstly save entry
            saveEntry();

            // operations status form
            FormOperation? formOperation = FormOperation.showForm(this, "please wait. doing operation...", 0, 100, 0, 0);

            // now delete the db
            if (entryMethods.DBDeleteDatabase(ref cfg))
            {
                formOperation.close();
                MessageBox.Show("successfully deleted currently active db.", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                formOperation.close();

            // reset everything
            reset();

        }

    }
}