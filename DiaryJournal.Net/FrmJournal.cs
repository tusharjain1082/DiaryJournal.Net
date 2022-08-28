#define UNICODE

using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace DiaryJournal.Net
{

    public partial class FrmJournal : Form
    {

        public bool stateChanged = false;
        public textFormatting formatting = null;
        public String previousRtf = "";
        public FormFind? myFormFind = null;
        bool properExit = false;

        public myConfig cfg = new myConfig();

        // delegates
        public delegate void __updateProgressStatusDelegate(long progess, long total);
        public __updateProgressStatusDelegate updateProgressStatus;
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
        public delegate void __insertLvTrashCanItemDelegate(myNode node);
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
        public delegate TreeNode? __initTreeViewEntryDelegate(myNode nodeEntry);
        public __initTreeViewEntryDelegate initTreeViewEntry;
        public delegate void __loadRootToBottomNodesDelegate(List<myNode> nodes, bool reverse = true);
        public __loadRootToBottomNodesDelegate loadRootToBottomNodes;
        public delegate List<myNode> __getHighestCheckedTreeViewItemsDBNodesDelegate();
        public __getHighestCheckedTreeViewItemsDBNodesDelegate getHighestCheckedTreeViewItemsDBNodes;

        public FrmJournal()
        {
            InitializeComponent();
        }

        private void FrmJournal_Load(object sender, EventArgs e)
        {
            // Add a reference to the NuGet package System.Text.Encoding.CodePages for .Net core only
            // important initialization for RtfPipe Library:
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //AppContext.SetSwitch("Switch.System.Windows.Forms.DoNotLoadLatestRichEditControl", false);

            // this setting stops empty strings to be set to null
            LiteDB.BsonMapper.Global.EmptyStringToNull = false;

            rtbEntry.LostFocus += RtbEntry_LostFocus;
            rtbEntry.TextChanged += rtbEntry_TextChanged;
            tvEntries.BeforeSelect += TvEntries_BeforeSelect; ;
            tvEntries.AfterCheck += TvEntries_AfterCheck;
            CalendarEntries.DateSelected += CalendarEntries_DateSelected;
            tabControlJournal.Selected += TabControlJournal_Selected;
            rtbEntry.SelectionChanged += RtbEntry_SelectionChanged;
            cmbFonts.SelectedIndexChanged += CmbFonts_SelectedIndexChanged;
            cmbSize.SelectedIndexChanged += CmbSize_SelectedIndexChanged;
            lvSearch.DoubleClick += LvSearch_DoubleClick;
            lvTrashCan.DoubleClick += LvTrashCan_DoubleClick;
            this.Shown += FrmJournal_Shown;
            this.FormClosing += FrmJournal_FormClosing;

            // Add event handlers for the required drag events.
            tvEntries.ItemDrag += TvEntries_ItemDrag;
            tvEntries.DragEnter += TvEntries_DragEnter;
            tvEntries.DragOver += TvEntries_DragOver;
            tvEntries.DragDrop += TvEntries_DragDrop;


            String strDateTimeTemplate = DiaryJournal.Net.Properties.Resources.BuildDateTime;
            DateTime buildDateTime = DateTime.Parse(strDateTimeTemplate);
            String strBuildDateTime = buildDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            this.Text = "Tushar Jain's " + this.Text + " Version " + Application.ProductVersion + ", Compiled/Built on: " + strBuildDateTime;

            // setup delegates
            updateProgressStatus = new __updateProgressStatusDelegate(__updateProgressStatus);
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


        // now load config file and setup
        myConfigMethods.autoCreateLoadConfigFile(ref cfg, false);
            applyConfig();

            splitContainerEntryTree.Cursor = Cursors.Default;
            splitContainerH.Cursor = Cursors.Default;
            splitContainerV.Cursor = Cursors.Default;

            splitContainerSearch.Cursor = Cursors.Default;

            // initialize all formatting config
            formatting = new textFormatting();
            cmbFonts.Items.AddRange(formatting.fontNames.ToArray());

            // config
            resetRtb(rtbEntry);

            // listing all web colors in toolstrip drop down split buttons so that the user can select them.
            var webColors = commonMethods.getWebColors();// typeof(Color));
            foreach (Color knownColor in webColors)
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
            dtPickerSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerSearchThroughTime.Value = DateTime.Parse("23:59:59");

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

            // Retrieve the node at the drop location.
            TreeNode targetTreeNode = tvEntries.GetNodeAt(targetPoint);
            if (targetTreeNode == null) return;

            // load node
            String rtf = "";
            Int64 targetNodeID = Int64.Parse(targetTreeNode.Name);
            myNode? targetNode = entryMethods.DBFindLoadNode(cfg, targetNodeID, ref rtf, false);
            if (targetNode == null)
                return;

            // Retrieve the node that was dragged.
            TreeNode draggedTreeNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
            if (draggedTreeNode == null) return;

            // load node
            Int64 draggedNodeID = Int64.Parse(draggedTreeNode.Name);
            myNode? draggedNode = entryMethods.DBFindLoadNode(cfg, draggedNodeID, ref rtf, false);
            if (draggedNode == null)
                return;

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
                    myNode? updatedNode = null;
                    if (!entryMethods.DBSetNodeParent(cfg, draggedNode.chapter.Id, ref updatedNode, targetNode.chapter.Id))
                        return;

                    draggedTreeNode.Remove();
                    targetTreeNode.Nodes.Add(draggedTreeNode);
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

        private void TvEntries_DragOver(object? sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse position.
            Point targetPoint = tvEntries.PointToClient(new Point(e.X, e.Y));

            // Select the node at the mouse position.
            tvEntries.SelectedNode = tvEntries.GetNodeAt(targetPoint);
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
            base.OnClosing(e);

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

            // set editor's flow limit
            rtbEntry.RightMargin = cfg.cmbCfgRtbEntryRMValue;
            rtbViewEntry.RightMargin = cfg.cmbCfgRtbViewEntryRMValue;
            int index = cmbCfgRtbEntryRM.FindString(rtbEntry.RightMargin.ToString());
            cmbCfgRtbEntryRM.SelectedIndex = index;
            index = cmbCfgRtbViewEntryRM.FindString(rtbViewEntry.RightMargin.ToString());
            cmbCfgRtbViewEntryRM.SelectedIndex = index;
            radCfgUseOpenFileSystemDB.Checked = cfg.radCfgUseOpenFileSystemDB;
            radCfgUseSingleFileDB.Checked = cfg.radCfgUseSingleFileDB;
        }

        private void LvTrashCan_DoubleClick(object? sender, EventArgs e)
        {
            viewSelectedTrashCanEntry();

        }

        public void OpenSelectedSearchedEntry()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (lvSearch.SelectedItems.Count == 0)
                return;

            ListViewItem listViewItem = lvSearch.SelectedItems[0];
            if (listViewItem == null)
                return;

            Int64 id = Int64.Parse(listViewItem.Name);
            myNode? node = null;
            String rtf = "";
            node = entryMethods.DBFindLoadNode(cfg, id, ref rtf, true);
            if (node == null)
                return;

            if (node.chapter.IsDeleted)
            {
                rtbViewEntry.Rtf = rtf;
                tabControlJournal.SelectedIndex = tabControlJournal.TabPages.IndexOfKey("TabPageViewEntry");
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

            ListViewItem listViewItem = lvTrashCan.SelectedItems[0];
            if (listViewItem == null)
                return;

            Int64 id = Int64.Parse(listViewItem.Name);
            myNode? node = null;
            String rtf = "";
            node = entryMethods.DBFindLoadNode(cfg, id, ref rtf, true);
            if (node == null)
                return;

            rtbViewEntry.Rtf = rtf;
            tabControlJournal.SelectedIndex = tabControlJournal.TabPages.IndexOfKey("TabPageViewEntry");
        }


        private void LvSearch_DoubleClick(object? sender, EventArgs e)
        {
            OpenSelectedSearchedEntry();
        }

        private void ToolStripFontColorMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            if (menuItem == null)
                return;

            Color color = (Color)menuItem.Tag;
            if (color == null)
                return;

            formatting.formatFontColor(rtbEntry, color);
        }

        private void ToolStripBackColorMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            if (menuItem == null)
                return;

            Color color = (Color)menuItem.Tag;
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

        public void resetRtb(AdvRichTextBox rtb, bool clear = true, bool resetSaveState = false)
        {

            // config
            if (clear)
                rtb.Clear();

            //this.ActiveControl = rtb;
            rtb.Select(0, 0);
            rtb.Font = new Font("Times New Roman", 14.0f, FontStyle.Regular);
            rtb.SelectionFont = new Font("Times New Roman", 14.0f, FontStyle.Regular);
            rtb.ScrollToCaret();
            //rtb.Focus();
            //rtb.Select();

            if (resetSaveState)
            {
                stateChanged = false;
                tsslblStateChanged.Text = " ";
                tsbuttonSave.Checked = false;
                tsbuttonSave.BackColor = SystemColors.Control;
            }
        }

        private void CmbSize_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ToolStripComboBox comboBox = (ToolStripComboBox)sender;
            if (comboBox.SelectedItem == null)
                return;

            int value = 0;
            if (!int.TryParse((String)comboBox.SelectedItem, out value))
                return;

            formatting.formatFontSize(rtbEntry, value); 
        }

        private void CmbFonts_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ToolStripComboBox comboBox = (ToolStripComboBox)sender;
            if (comboBox.SelectedItem == null)
                return;

            formatting.formatFont(rtbEntry, (String)comboBox.SelectedItem);
        }

        private void RtbEntry_SelectionChanged(object? sender, EventArgs e)
        {
            int caretPosition = rtbEntry.SelectionStart;
            int lineIndex = rtbEntry.GetLineFromCharIndex(caretPosition);
            tsslabelCaretPosition.Text = caretPosition.ToString();
            tsslabelLineIndex.Text = lineIndex.ToString();

            //int line = rtbEntry.GetLineFromCharIndex(index);
            formatting.selStartIndex = rtbEntry.SelectionStart;
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
                tsbuttonBold.Checked = rtbEntry.SelectionFont.Bold;
                tsbuttonItalics.Checked = rtbEntry.SelectionFont.Italic;
                tsbuttonUnderline.Checked = rtbEntry.SelectionFont.Underline;
                tsbuttonStrikeout.Checked = rtbEntry.SelectionFont.Strikeout;
            }

            if (rtbEntry.SelectionFont != null)
            {
                //int index = cmbSize.FindString(rtbEntry.SelectionFont.Size.ToString());
                //cmbSize.SelectedIndex = index;
                cmbSize.Text = rtbEntry.SelectionFont.Size.ToString();
            }

            if (rtbEntry.SelectionAlignment == TextAlign.Left)
                tsbuttonLeftJustify.Checked = true;
            else
                tsbuttonLeftJustify.Checked = false;

            if (rtbEntry.SelectionAlignment == TextAlign.Right)
                tsbuttonRightJustify.Checked = true;
            else
                tsbuttonRightJustify.Checked = false;

            if (rtbEntry.SelectionAlignment == TextAlign.Justify)
                tsbuttonJustify.Checked = true;
            else
                tsbuttonJustify.Checked = false;

            if (rtbEntry.SelectionAlignment == TextAlign.Center)
                tsbuttonCenterJustify.Checked = true;
            else
                tsbuttonCenterJustify.Checked = false;

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

            // validation
            myNode? parentNode = null;
            String rtf = "";
            if (parentId != 0)
            {
                parentNode = entryMethods.DBFindLoadNode(cfg, parentId, ref rtf, false);
                if (parentNode == null)
                    return;
            }
            // initialize calender nodes
            myNode? yearNode = null;
            myNode? monthNode = null;

            // init calendar nodes
            mySystemNodes? systemNodes = null;
            entryMethods.initCalenderNodesSystem(cfg, dateTime.Year, dateTime.Month, ref yearNode, ref monthNode, ref systemNodes);
            __loadSystemNodes(ref systemNodes);

            // find if user asks for new child entry, else it is common year month day based entry.
            List<myNode> lineageChain = null;

            if (parentId == 0)
                parentId = monthNode.chapter.Id;

            myNode node = new myNode();
            node.chapter.parentId = parentId;
            node.chapter.chapterDateTime = dateTime;
            node.chapter.nodeType = nodeType;
            node.chapter.Title = title;
            if (!entryMethods.DBCreateNode(ref cfg, ref node, "", true, true))
                return;

            List<myNode>? allNodes = entryMethods.DBFindAllNodes(cfg, false, false);
            allNodes = entryMethods.findAllNodesTreeSequence(ref allNodes, true, false);
            lineageChain = entryMethods.findBottomToRootNodesRecursive(ref allNodes, node.chapter.Id, false, false);
            cfg.totalNodes = allNodes.LongCount();

            if (lineageChain.Count <= 0)
                return;

            // now insert the entire chain from root to bottom most child
            this.Invoke(loadRootToBottomNodes, lineageChain, true);

            // validate the newly created node and load it
            TreeNode? newTreeNode = null;
            String path = String.Format(@"{0}", node.chapter.Id);
            TreeNode[] nodes = tvEntries.Nodes.Find(path, true);
            if (nodes.Length == 0) 
                return;

            newTreeNode = nodes[0];

            // update ui
            __setCalendarHighlightEntry(node.chapter.chapterDateTime);
            tsslblTotalEntries.Text = cfg.totalNodes.ToString();
            tvEntries.SelectedNode = newTreeNode;
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
            entryMethods.autoCreateLoadSystemNodes(ref cfg, out systemNodes);

        }

        public void __createNewDB_SingleFileDB()
        {
            sfdDB.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
            if (sfdDB.ShowDialog() != DialogResult.OK)
                return;

            // first reset everything
            reset();

            // todo
            if (!SingleFileDB.CreateLoadDB(sfdDB.FileName, ref cfg.ctx0, true))
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
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            String message =
                @"DiaryJournal.Net can import .rtf document entries exported by the Journal 7 and 8 software.All the entries must exist in a single folder. this folder must not contain any alien or unwanted file.you should export the documents in .rtf format from the Journal software with export template format/ pattern: ""yyyy-mm-dd-hh-mm-ss"" for example ""2016-10-25-16-20-00.rtf"" is the exported file name." + Environment.NewLine +
                @"if it is a loose-leaf entry, it shall be imported as common calendar entry in the year and month calendar nodes like all the rest of entries." + Environment.NewLine +
                @"""export entries to document"" dialog =" + Environment.NewLine +
                @"1.please set the export option to .rtf in the Journal 7 / 8." + Environment.NewLine +
                @"2. ""file names"" settings:" + Environment.NewLine +
                @"category file name = %c" + Environment.NewLine +
                @"entry file name = %e" + Environment.NewLine +
                @"check mark the ""include full path in loose-leaf entry names"" option." + Environment.NewLine +
                @"export path separator = ""-""" + Environment.NewLine +
                @"date format: %yyyy-%mm-%dd-%time" + Environment.NewLine +
                @"all other settings remain default.";

            MessageBox.Show(this, message, "import instructions", MessageBoxButtons.OK, MessageBoxIcon.Information);

            browseFolder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            bgWorkerTJEntries.RunWorkerAsync();
        }
        public void __setCalendarHighlightEntry(DateTime dateTime)
        {
            DateTime day = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
            CalendarEntries.AddBoldedDate(day);
            CalendarEntries.UpdateBoldedDates();

        }

        public void __treeViewBeginUpdate(TreeView tv, bool clear)
        {
            tv.BeginUpdate();
            tv.SuspendLayout();
            tv.Nodes.Clear();
            CalendarEntries.SuspendLayout();
            CalendarEntries.Hide();

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

        public void __treeViewEndUpdate(TreeView tv)
        {
            tv.EndUpdate();
            tv.ResumeLayout();
            CalendarEntries.ResumeLayout();
            CalendarEntries.Show();
        }

        public void __importTheJournalRtfEntries(String path)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            long filesDone = 0;
            long totalFiles = Directory.EnumerateFiles(path, "*.rtf").LongCount();
            if (totalFiles <= 0)
                return;

            this.Invoke(toggleForm, false);

            RichTextBox richTextBox = new RichTextBox();

            //            IEnumerable <String> files = Directory.EnumerateFiles(path, "*.rtf").OrderBy(filename => filename);//Directory.EnumerateFiles(path, "*.rtf"))
            IEnumerable<FileInfo> files = commonMethods.EnumerateFiles(path, EntryType.Rtf);

            // first we need to create a set node. we cannot import anything at all without a set node.
            myNode? setNode = entryMethods.createSetNode(ref cfg, "the_journal_set", DateTime.Now);

            // import set node
            if (!entryMethods.DBCreateNode(ref cfg, ref setNode, "", false, true))
                return;

            List<myNode> allNodes = new List<myNode>();// entryMethods.DBFindAllChildrenRecursive(cfg, setNode.chapter.Id, true, false);

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
                entryMethods.initCalenderNodesNonSystemSet(cfg, ref allNodes, ref setNode, chapter.chapterDateTime.Year, 
                    chapter.chapterDateTime.Month, out yearNode, out monthNode, false);

                // now setup the chapter with the year and month config
                chapter.parentId = monthNode.chapter.Id;

                // create new node into db
                myNode? newNode = new myNode(ref chapter);
                newNode = entryMethods.DBNewNode(ref cfg,
                    SpecialNodeType.None, NodeType.EntryNode, DomainType.Journal,
                    ref newNode, chapter.chapterDateTime, chapter.parentId, true, "", rtf,
                    true, false);

                if (newNode == null)
                    continue;

                // done
                this.Invoke(updateProgressStatus, filesDone++, totalFiles);
            }

            // finally update the db index in file.
            entryMethods.DBWriteIndexing(ref cfg);

            // 2nd phase, load the entires into treeview
            this.Invoke(updateProgressStatus, 0, 0);
            this.Invoke(loadEntries);
            this.Invoke(toggleForm, true);
        }

        public void __showMessageBox(String text, String title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            MessageBox.Show(this, text, title, buttons, icon);
        }

        public void __toggleForm(bool toggle)
        {
            this.Enabled = toggle;
        }

        public void __updateTotalEntriesStatus(long totalEntries)
        {
            tsslblTotalEntries.Text = totalEntries.ToString();
        }

        public void __updateProgressStatus(long files, long total)
        {
            if (total > 0)
            {
                tsProgressBar.Value = (int)Math.Round((double)(100 * files) / total);
                tsslblFilesDone.Text = files.ToString();
            }
            else
            {
                tsProgressBar.Value = 0;
                tsslblFilesDone.Text = "";
            }
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
                    this.Invoke(setCalendarHighlightEntry, nodeEntry.chapter.chapterDateTime);
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
                        this.Invoke(setCalendarHighlightEntry, nodeEntry.chapter.chapterDateTime);
                        return newNode;
                    }
                }
            }
            return null;
        }

        public void __loadEntries()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // first save the entry
            this.Invoke(saveEntry);

            this.Invoke(toggleForm, false);
            this.Invoke(treeViewBeginUpdate, tvEntries, true);

            // collect all nodes
            List<myNode>? srcNodes = entryMethods.DBFindAllNodes(cfg, false, false);
            //srcNodes = entryMethods.findAllNodesTreeSequence(ref srcNodes, true, false);

            // first get the total number of nodes which exist in db
            long nodesCount = srcNodes.LongCount();
            long filesDone = 0;

            // build tree
            myTreeDom treeDom = new myTreeDom();
            treeDom.buildTree(ref srcNodes, true, false);
            List<myTreeDomNode> tree = treeDom.ToList();

            foreach (myTreeDomNode listedNode in tree)
            {
                myNode node = listedNode.self;

                if (node.chapter.IsDeleted)
                    continue;

                this.Invoke(initTreeViewEntry, node);

                // update ui
                this.Invoke(updateProgressStatus, filesDone++, nodesCount);
            }

            // update ui
            //this.Invoke(treeViewSort, tvEntries);

            // update
            cfg.totalNodes = filesDone;
            this.Invoke(updateProgressStatus, 0, 0);
            this.Invoke(treeViewEndUpdate, tvEntries);
            this.Invoke(updateTotalEntriesStatus, cfg.totalNodes);
            this.Invoke(toggleForm, true);
            this.Invoke(gotoTodaysEntry);
        }

        public void __loadSystemNodes(ref mySystemNodes systemNodes)
        {
            // load journal node
            this.Invoke(initTreeViewEntry, systemNodes.JournalSystemNode);

            // load library node
            this.Invoke(initTreeViewEntry, systemNodes.LibrarySystemNode);

            // load all year nodes
            __loadRootToBottomNodes(systemNodes.YearNodes, false);

            // load all month nodes
            __loadRootToBottomNodes(systemNodes.MonthNodes, false);


        }
        public void __loadRootToBottomNodes(List<myNode> nodes, bool reverse = true)
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
        public void loadDB(String path)
        {
            if (path == "")
                return;

            // firstly save entry
            saveEntry();

            // reset everything
            reset();

            if (cfg.radCfgUseOpenFileSystemDB)
            {
                if (!OpenFileSystemDB.LoadDB(ref cfg.ctx1, path))
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
                if (!SingleFileDB.CreateLoadDB(path, ref cfg.ctx0, false))
                {
                    reset();
                    this.Invoke(showMessageBox, "errror creating/loading db", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // setup ui
                txtDBFile.Text = cfg.ctx0.dbpath;

            }

            // run in background worker
            bgWorkerLoadDB.RunWorkerAsync();
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

            // auto create/load system nodes
            mySystemNodes? systemNodes = null;
            entryMethods.autoCreateLoadSystemNodes(ref cfg, out systemNodes);

        }

        public void reset()
        {

            // reset user interface
            resetRtb(rtbEntry);
            tvEntries.Nodes.Clear();
            CalendarEntries.RemoveAllBoldedDates();
            tsslblTotalEntries.Text = "0";
            tsslblEntryTitle.Text = "n/a";
            tsslblStateChanged.Text = " ";
            stateChanged = false;
            txtDBFile.Text = "";
            tsbuttonSave.Checked = false;
            tsbuttonSave.BackColor = SystemColors.Control;
            tsslblFilesDone.Text = "";

            // close the db
            cfg.ctx0.close();
            cfg.ctx1.close();
        }

        private void TvEntries_BeforeSelect(object? sender, TreeViewCancelEventArgs e)
        {
            // now first save the previous state if the state was changed.
            saveEntry();
        }

        private void tvEntries_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            Int64 id = Int64.Parse(e.Node.Name);
            loadSelectedEntry(id);

        }

        public TreeNode? initTreeNodeTitle(Int64 id)
        {
            TreeNode? treeNode = null;
            String path = String.Format(@"{0}", id);
            TreeNode[] nodes = tvEntries.Nodes.Find(path, true);
            if (nodes.Length == 0) // no node with this entry exists, so create it
                return null;

            treeNode = nodes[0];

            myNode? node = null;
            String rtf = "";
            node = entryMethods.DBFindLoadNode(cfg, id, ref rtf, false);
            if (node == null)
                return null;

            // setup/resetup tree node's title
            String entryName = entryMethods.getEntryLabel(node);
            treeNode.Text = entryName;
            return treeNode;    
        }

        public void loadSelectedEntry(Int64 id)
        {
            String rtf = "";
            myNode? node = null;
            node = entryMethods.DBFindLoadNode(cfg, id, ref rtf, true);
            if (node == null)
                return;

            rtbEntry.Rtf = rtf;
            resetRtb(rtbEntry, false, true);
            tsslblEntryTitle.Text = node.chapter.Title;
            CalendarEntries.SelectionStart = node.chapter.chapterDateTime;
            CalendarEntries.SelectionEnd = node.chapter.chapterDateTime;
            // setup/resetup node's title
            //initTreeNodeTitle(id);

        }

        private void newEntryNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setupNowEntry();
        }

        private void saveEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveEntry();
        }

        public void __saveEntry()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            if (!stateChanged)
                return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            myNode? node = null;
            String rtf = rtbEntry.Rtf;
            node = entryMethods.DBFindLoadNode(cfg, id, ref rtf, false);
            if (node == null)
                return;

            // save the entry
            entryMethods.DBUpdateNode(cfg, ref node, rtf, true);

            stateChanged = false;
            tsslblStateChanged.Text = " ";
            tsbuttonSave.Checked = false;
            tsbuttonSave.BackColor = SystemColors.Control;
        }


        private void RtbEntry_LostFocus(object? sender, EventArgs e)
        {
            
        }

        public void undoRtbEntry()
        {
            rtbEntry.Undo();
        }
        public void redoRtbEntry()
        {
            rtbEntry.Redo();
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoRtbEntry();
        }
        private void undoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            undoRtbEntry();
        }

        public void changeEntryTitle()
        {
            if (tvEntries.SelectedNode == null)
                return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            myNode? node = null;
            String rtf = "";
            node = entryMethods.DBFindLoadNode(cfg, id, ref rtf, false);
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
            if (!entryMethods.DBUpdateNodeTitle(cfg, id, title))
                return;

            myNode? node = null;
            String rtf = "";
            node = entryMethods.DBFindLoadNode(cfg, id, ref rtf, false);
            if (node == null)
                return;

            // resetup tree node
            String entryName = "";
            entryName = entryMethods.getEntryLabel(node);
            tvEntries.SelectedNode.Text = entryName;
            tsslblEntryTitle.Text = title;
        }

        private void splitContainerV_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void titleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeEntryTitle();
        }

        private void bgWorkerLoadDB_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // now load all entries
            __loadEntries();
        }

        public void pasteRtbEntry()
        {
            rtbEntry.Paste();
        }
        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            pasteRtbEntry();
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pasteRtbEntry();
        }

        private void rtbEntry_TextChanged(object? sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            stateChanged = true;
            tsslblStateChanged.Text = "*";
            tsbuttonSave.BackColor = Color.Orange;
//            tsbuttonSave.Checked = true;
        }

        private void gotoTodaysEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            __gotoTodaysEntry();
        }

        public void __gotoEntry(DateTime dateTime)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            myNode? node = null;
            
            List<myNode> allNodes = entryMethods.DBFindAllNodes(cfg, true, false);
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
            //TreeNode treeNode = findTreeNode(node.chapter.guid);
            //if (treeNode == null)
            //   return;

            // found one node in tree view
            tvEntries.SelectedNode = treeNodes[0];
        }

        public TreeNode? findTreeNode(Int64 id)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return null;

            // first save the entry
            saveEntry();

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

                    // add all children in queue, they will be processed in this same way in this same place: 1st parent node, 2nd children nodes.
                    foreach (TreeNode childNode in children)
                        queue.Enqueue(childNode);

                    String path = String.Format(@"{0}", id);
                    if (currentNode.Name == path)
                        return currentNode;
                }
            }
            return null;
        }

        public void __gotoEntryById(Int64 id)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            myNode? node = null;
            String rtf = "";
            node = entryMethods.DBFindLoadNode(cfg, id, ref rtf, false);
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

        public void copyRtbEntry()
        {
            rtbEntry.Copy();
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            copyRtbEntry();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyRtbEntry();
        }

        public void copyAllRtbEntry()
        {
            rtbEntry.SelectAll();
            rtbEntry.Copy();
        }

        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyAllRtbEntry();
        }

        private void copyAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            copyAllRtbEntry();
        }

        public void cutRtbEntry()
        {
            rtbEntry.Cut();
        }

        public void cutAllRtbEntry()
        {
            rtbEntry.SelectAll();
            rtbEntry.Cut();
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cutRtbEntry();
        }

        private void cutAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cutAllRtbEntry();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cutRtbEntry();
        }

        private void cutAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cutAllRtbEntry();
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

        private void bgWorkerTJEntries_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            __importTheJournalRtfEntries(browseFolder.SelectedPath);
        }

        private void cmbFonts_Click(object sender, EventArgs e)
        {
        }


        private void tsbuttonBold_Click(object sender, EventArgs e)
        {
            if (rtbEntry.SelectionFont != null)
                formatting.formatBold(rtbEntry, (ToolStripButton)sender);

        }

        private void tsbuttonItalics_Click(object sender, EventArgs e)
        {
            if (rtbEntry.SelectionFont != null)
                formatting.formatItalics(rtbEntry, (ToolStripButton)sender);

        }

        private void tsbuttonUnderline_Click(object sender, EventArgs e)
        {
            if (rtbEntry.SelectionFont != null)
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
            if (rtbEntry.SelectionFont != null)
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
            redoRtbEntry();
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            redoRtbEntry();
        }

        private void buttonApplyConfig1_Click(object sender, EventArgs e)
        {
            if (cfg.ctx0.isDBOpen() || cfg.ctx1.isDBOpen())
            {
                MessageBox.Show("error changing config. please save and close the opened db and retry.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            String err = "error configuration. retry after correcting it. aborted.";
            int rtbEntryRightMargin = 0;
            if (!int.TryParse(cmbCfgRtbEntryRM.Text, out rtbEntryRightMargin))
            {
                MessageBox.Show(err, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int rtbViewEntryRightMargin = 0;
            if (!int.TryParse(cmbCfgRtbViewEntryRM.Text, out rtbViewEntryRightMargin))
            {
                MessageBox.Show(err, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // in the final place, apply all configuration
            rtbEntry.RightMargin = rtbEntryRightMargin;
            rtbViewEntry.RightMargin = rtbViewEntryRightMargin;
            // update config
            cfg.chkCfgAutoLoadCreateDefaultDB = chkCfgAutoLoadCreateDefaultDB.Checked;
            cfg.cmbCfgRtbEntryRMValue = rtbEntryRightMargin;
            cfg.cmbCfgRtbViewEntryRMValue = rtbViewEntryRightMargin;
            cfg.chkCfgUseWinUserDocFolder = chkCfgUseWinUserDocFolder.Checked;
            cfg.radCfgUseSingleFileDB = radCfgUseSingleFileDB.Checked;
            cfg.radCfgUseOpenFileSystemDB = radCfgUseOpenFileSystemDB.Checked;
            myConfigMethods.saveConfigFile(myConfigMethods.getConfigPathFile(), ref cfg, false);
            MessageBox.Show("applied all configuration.", "done", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.ShowColor = true; //color pnael
            fontDialog.ShowApply = true; //show apply button
            fontDialog.ShowEffects = true; //after appling, label style will chance
            fontDialog.Font = rtbEntry.SelectionFont;
            fontDialog.Color = rtbEntry.SelectionColor;
            if (rtbEntry.SelectionFont != null)
                fontDialog.Font = rtbEntry.SelectionFont;

            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;

            rtbEntry.SelectionFont = fontDialog.Font;   
            rtbEntry.SelectionColor = fontDialog.Color;

        }

        private void cmbSize_Click(object sender, EventArgs e)
        {

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

            // auto create/load system nodes
            mySystemNodes? systemNodes = null;
            //List<myNode> allNodes = entryMethods.DBFindAllNodes(cfg, true, false);
            entryMethods.autoCreateLoadSystemNodes(ref cfg, out systemNodes);

            // run in background worker
            bgWorkerLoadDB.RunWorkerAsync();
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            autoCreateLoadDefaultDB(false);
        }

        private void tsbuttonBullets_Click(object sender, EventArgs e)
        {
            formatting.formatBullets(rtbEntry, tsbuttonBullets, tsbuttonNumberedList);

        }

        private void tsbuttonNumberedList_Click(object sender, EventArgs e)
        {
            formatting.formatNumberedList(rtbEntry, tsbuttonBullets, tsbuttonNumberedList);

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
            formatting.formatLineSpacing(rtbEntry, RichTextBoxEx.LineSpaceTypes.Single);
        }

        private void lineSpace1pt5_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, RichTextBoxEx.LineSpaceTypes.OneAndHalf);

        }

        private void lineSpace2_Click(object sender, EventArgs e)
        {
            formatting.formatLineSpacing(rtbEntry, RichTextBoxEx.LineSpaceTypes.Double);

        }

        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertTable();
        }

        public void insertTable()
        {
            FormInsertTable form = null;
            if (showInsertTableDialog(out form) != DialogResult.OK)
                return;

            if (form == null)
                return;

            formatting.formatInsertTable(rtbEntry, form.rows, form.columns, form.tableWidth, form.tableAlignment,
                form.tableOuterBorder, form.tableOuterBorderSize, form.tableInnerBorder, form.tableInnerBorderSize,
                form.rowHeight, form.columnWidth, form.fontName, form.fontSize, form.cellVAlign, form.cellAlignment);

            form.Dispose();
        }

        public static DialogResult showInsertTableDialog(out FormInsertTable formObect)
        {
            FormInsertTable formInsertTable = new FormInsertTable();
            formInsertTable.ShowDialog();
            formObect = formInsertTable;
            return formInsertTable.myResult;

        }

        private void tsbuttonClear_Click(object sender, EventArgs e)
        {
            clear();   
        }

        public void clear()
        {
            previousRtf = rtbEntry.Rtf;
            rtbEntry.Clear();
        }

        public void revertClear()
        {
            String history = rtbEntry.Rtf;
            rtbEntry.Rtf = previousRtf;
            previousRtf = history;
        }
        private void revertClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            revertClear();
        }

        private void revertClearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            revertClear();
        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            clear();
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

            // auto create/load system nodes
            mySystemNodes? systemNodes = null;
            //List<myNode> allNodes = entryMethods.DBFindAllNodes(cfg, true, false);
            entryMethods.autoCreateLoadSystemNodes(ref cfg, out systemNodes);

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

            Image image = Image.FromFile(ofdFile.FileName);
            formatting.formatInsertImage(rtbEntry, image);

        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setFontColor();
        }

        public void setFontColor()
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() != DialogResult.OK)    
                return;

            formatting.formatFontColor(rtbEntry, colorDialog.Color);
        }
        public void setBackColor()
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() != DialogResult.OK)
                return;

            formatting.formatBackColor(rtbEntry, colorDialog.Color);
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setBackColor();
        }

        private void tsbuttonUndo_Click(object sender, EventArgs e)
        {
            undoRtbEntry();
        }

        private void tsbuttonRedo_Click(object sender, EventArgs e)
        {
            redoRtbEntry();
        }

        private void tsbuttonRevertClear_Click(object sender, EventArgs e)
        {
            revertClear();
        }

        private void findTotalEntriesInDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            findTotalEntriesInDB();   
        }

        public void findTotalEntriesInDB()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            List<myNode> list = entryMethods.DBFindAllNodes(cfg, false, false);
            cfg.totalNodes = list.LongCount();
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

            List<myNode> nodes = __getCheckedTreeViewItemsDBNodes(); //__getHighestCheckedTreeViewItemsDBNodes();
            List<myNode>? allNodes = entryMethods.DBFindAllNodes(cfg, false, false);
            allNodes = entryMethods.findAllNodesTreeSequence(ref allNodes, true, false);
            entryMethods.DBDeleteOrPurgeListRecursive(cfg, ref allNodes, ref nodes, true, false);
            foreach (myNode node in nodes)
            {
                // system nodes cannot be touched.
                if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                    continue;

                TreeNode? treeNode = null;
                if (__findTreeNode(node.chapter.Id, out treeNode))
                    treeNode.Remove();

            }
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

            List<myNode> nodes = entryMethods.DBFindDeletedNodes(cfg);
            foreach (myNode node in nodes)
                this.Invoke(insertLvTrashCanItem, node);

            this.Invoke(LvTrashCanUpdate, false);
            this.Invoke(toggleForm, true);
        }

        public void __resetTrashCan()
        {
            lvTrashCan.Items.Clear();   
        }

        public void __insertLvTrashCanItem(myNode node)
        {
            ListViewItem item = new ListViewItem();
            item.Name = node.chapter.Id.ToString();
            String chapterDateTime = node.chapter.chapterDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            item.Text = chapterDateTime;
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

            List<myNode>? allNodes = entryMethods.DBFindAllNodes(cfg, false, false);
            allNodes = entryMethods.findAllNodesTreeSequence(ref allNodes, true, false);
            foreach (ListViewItem listViewItem in lvTrashCan.CheckedItems)
            {
                Int64 id = Int64.Parse(listViewItem.Name);
                myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
                if (node == null)
                    continue;

                entryMethods.DBDeleteOrPurgeNodeRecursive(cfg, ref allNodes, ref node, false, true);
            }

            // refresh trash can after deleting
            refreshTrashcan();
        }

        public void checkEntireTrashCan(bool check)
        {
            foreach (ListViewItem listViewItem in lvTrashCan.Items)
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

            checkEntireTrashCan(true);
            purgeTrashCan(); 
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

            List<myNode>? allNodes = entryMethods.DBFindAllNodes(cfg, false, false);
            allNodes = entryMethods.findAllNodesTreeSequence(ref allNodes, true, false);
            foreach (ListViewItem listViewItem in lvTrashCan.CheckedItems)
            {
                Int64 id = Int64.Parse(listViewItem.Name);
                myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
                if (node == null)
                    continue;

                entryMethods.DBDeleteOrPurgeNodeRecursive(cfg, ref allNodes, ref node, false, false);
            }
            // refresh trash can after deleting
            refreshTrashcan();

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
        public void doNewLabelEntry()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            string? input = "";
            if (userInterface.ShowInputDialog("input entry label (required)", ref input) != DialogResult.OK)
                return;

            if (input.Length <= 0)
                return;

            Int64 parentId = Int64.Parse(tvEntries.SelectedNode.Name);
            setupNewEntry(DateTime.Now, parentId, NodeType.LabelNode, input);
        }

        private void newChildEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doNewChildEntry();
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

                //currentNode.Checked = set;
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

            checkEntireTrashCan(true);
            restoreTrashCan();
        }

        private void highlightCheckedEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highlightCheckedEntries();
        }

        public void highlightEntry(TreeNode treeNode, ref myNode node)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            FontDialog fontDialog = new FontDialog();
            fontDialog.ShowColor = true; //color pnael
            fontDialog.ShowApply = true; //show apply button
            fontDialog.ShowEffects = true; //after appling, label style will chance
            fontDialog.Font = treeNode.NodeFont;
            fontDialog.Color = treeNode.ForeColor;

            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;

            entryMethods.setEntryHighlightFont(cfg, ref node, fontDialog.Color, fontDialog.Font);
            loadNodeHighlight(treeNode, ref node);
        }

        public void loadNodeHighlight(TreeNode treeNode)
        {
            // first find the node
            Int64 id = Int64.Parse(treeNode.Name);
            myNode? node = null;
            String rtf = "";
            node = entryMethods.DBFindLoadNode(cfg, id, ref rtf, false);
            if (node == null)
                return;

            if (node.chapter.HLFont.Length >= 1)
                treeNode.NodeFont = commonMethods.StringToFont(node.chapter.HLFont);

            if (node.chapter.HLFontColor.Length >= 1)
                treeNode.ForeColor = commonMethods.StringToColor(node.chapter.HLFontColor);

            if (node.chapter.HLBackColor.Length >= 1)
                treeNode.BackColor = commonMethods.StringToColor(node.chapter.HLBackColor);
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

        public void highlightCheckedEntries()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (tvEntries.Nodes.Count <= 0)
                return;

            FontDialog fontDialog = new FontDialog();
            fontDialog.ShowColor = true; //color pnael
            fontDialog.ShowApply = true; //show apply button
            fontDialog.ShowEffects = true; //after appling, label style will chance

            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;

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

                    highlightTreeViewNode(currentNode, fontDialog.Font, fontDialog.Color);
                    //highlightTreeViewNodeRecursively(currentNode, fontDialog.Font, fontDialog.Color);
                }
            }
            // refresh 
            loadEntries();
        }

        public void highlightTreeViewNode(TreeNode treeNode, Font font, Color color)
        {
            Int64 id = Int64.Parse(treeNode.Name);
            treeNode.ForeColor = color;
            treeNode.NodeFont = font;
            entryMethods.setEntryHighlightFont(cfg, id, color, font);
            loadNodeHighlight(treeNode);

        }
        private void highlightSelectedEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            Int64 id = Int64.Parse(tvEntries.SelectedNode.Name);
            myNode? node = null;
            String rtf = "";
            node = entryMethods.DBFindLoadNode(cfg, id, ref rtf, false);
            if (node == null)
                return;

            highlightEntry(tvEntries.SelectedNode, ref node);
        }

        private void highlightBackColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            highlightEntryBackColor(tvEntries.SelectedNode);
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
            entryMethods.setEntryHighlightBackColor(cfg, id, colorDialog.Color);
            loadNodeHighlight(treeNode);
        }

        private void tsbuttonSearch_Click(object sender, EventArgs e)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (rtbSearchPattern.TextLength <= 0)
                return;

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

        public bool __processSearch()
        {
            // init search
            return journalSearchFramework.searchEntries(cfg, lvSearch, tsSearchProgressBar,
                dtpickerSearchFrom.Value, dtPickerSearchFromTime.Value, dtpickerSearchThrough.Value, dtpickerSearchThroughTime.Value,
                chkSearchUseDateRange.Checked, rtbSearchPattern.Text, rtbSearchReplace.Text, chkSearchAll.Checked,
                chkSearchTrashCan.Checked, chkSearchMatchCase.Checked, chkSearchMatchWholeWord.Checked,
                chkSearchReplace.Checked, chkSearchMultiline.Checked, chkSearchExplicitCaptures.Checked,
                chkSearchReplaceTitle.Checked);
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
            OpenSelectedSearchedEntry();

        }

        private void toolStripSearch_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        public void checkAllSearchedItems(bool check)
        {
            foreach (ListViewItem listViewItem in lvSearch.Items)
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
            rtbSearchPattern.Clear();
            rtbSearchReplace.Clear();
            dtPickerSearchFromTime.Value = DateTime.Parse("0:00:00");
            dtpickerSearchThroughTime.Value = DateTime.Parse("23:59:59");
        }

        public void restoreSearchedList()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            List<myNode>? allNodes = entryMethods.DBFindAllNodes(cfg, false, false);
            allNodes = entryMethods.findAllNodesTreeSequence(ref allNodes, true, false);
            foreach (ListViewItem listViewItem in lvSearch.CheckedItems)
            {
                // restore checked nodes
                Int64 id = Int64.Parse(listViewItem.Name);
                myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
                if (node == null)
                    continue;

                entryMethods.DBDeleteOrPurgeNodeRecursive(cfg, ref allNodes, ref node, false, false);

                listViewItem.Checked = false;
                listViewItem.Selected = true;
            }
        }

        private void tsbuttonRestoreSearchedEntry_Click(object sender, EventArgs e)
        {
            restoreSearchedList();
        }
        public void restoreAllSearchedList()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            checkAllSearchedItems(true);
            restoreSearchedList();
        }

        public void deleteSearchedList()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            List<myNode>? allNodes = entryMethods.DBFindAllNodes(cfg, false, false);
            allNodes = entryMethods.findAllNodesTreeSequence(ref allNodes, true, false);
            foreach (ListViewItem listViewItem in lvSearch.CheckedItems)
            {
                // delete checked nodes
                Int64 id = Int64.Parse(listViewItem.Name);
                myNode? node = entryMethods.FindNodeInList(ref allNodes, id);
                if (node == null)
                    continue;

                entryMethods.DBDeleteOrPurgeNodeRecursive(cfg, ref allNodes, ref node, true, false);

                listViewItem.Checked = false;
                listViewItem.Selected = true;
            }
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
            RichTextBox rtb = (RichTextBox)cms.SourceControl;
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
            RichTextBox rtb = (RichTextBox)cms.SourceControl;
            rtb.SelectAll();

        }

        private void pasteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            RichTextBox rtb = (RichTextBox)cms.SourceControl;
            rtb.Paste();
        }

        private void toolStripMenuItem23_Click(object sender, EventArgs e)
        {
            var tsItem = (ToolStripMenuItem)sender;
            var cms = (ContextMenuStrip)tsItem.Owner;
            String sourceControl = cms.SourceControl.Name;
            RichTextBox rtb = (RichTextBox)cms.SourceControl;
            rtb.Cut();

        }

        private void generateTestEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbEntry.SelectedText = commonMethods.RandomString(1048576);
        }

        private void lvTrashCan_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void increaseFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formatting.formatIncreaseFontSize(rtbEntry);
        }

        public void increaseFontSize()
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
            myFormFind.rtbSearchPattern.Text = rtbEntry.SelectedText;
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

        public void exportSelectedEntry(EntryType entryType)
        {
            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            entryMethods.getEntryTypeFormats(entryType, ref ext, ref extComplete, ref extSearchPattern);

            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            saveEntry();

            TreeNode treeNode = tvEntries.SelectedNode;

            Int64 id = Int64.Parse(treeNode.Name);
            myNode? node = null;
            String rtf = "";
            node = entryMethods.DBFindLoadNode(cfg, id, ref rtf, false);
            if (node == null)
                return;

            browseFolder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            // finally export
            // todo tushar
            //entryMethods.exportEntry(cfg, id, browseFolder.SelectedPath, 0, entryType);
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
            dtPickerSearchFromTime.Value = DateTime.Parse("0:00:00");
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
            exportSet();
        }

        public void exportSet()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (browseFolder.ShowDialog(this) != DialogResult.OK)
                return;

            string? input = "";
            if (userInterface.ShowInputDialog("input set name/title", ref input) != DialogResult.OK)
                return;

            if (input.Length <= 0)
                return;

            bgWorkerExportSet.RunWorkerAsync(input);

        }

        private void bgWorkerExportSet_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            String dbName = (String)e.Argument;
            __exportSet(browseFolder.SelectedPath, dbName, EntryType.Rtf);

        }

        public void __exportSet(String path, String dbName, EntryType entryType)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // first save the entry
            this.Invoke(saveEntry);

            this.Invoke(toggleForm, false);

            // collect all source tree and nodes from currently active db
            List<myNode>? srcNodes = entryMethods.DBFindAllNodes(cfg, false, false);
            //srcNodes = entryMethods.findAllNodesTreeSequence(ref srcNodes, true, false);

            // first get the total number of chapters which exist in db
            long nodesCount = srcNodes.LongCount();
            Int64 exportIndex = 1; // 0 means no parent node id or node id

            // init new destination open file system db            
            OpenFSDBContext destctx = new OpenFSDBContext();
            OpenFileSystemDB.CreateLoadDB(path, dbName, ref destctx, true, true);

            // first we need to create a set node. we cannot export without a new set node
            // 1 is preset as set node's id. set node is 1st to be indexed before all nodes most recursively.
            myNode? setNode = entryMethods.createSetNode(ref exportIndex, dbName, DateTime.Now);

            // create set node entry in the destination
            OpenFileSystemDB.createNode(destctx, ref setNode, "", false);

            // load tree document object model structure
            myTreeDom treeDom = new myTreeDom();
            treeDom.buildTree(ref srcNodes, true, false);
            // we must nullify system node type in the export set because the export set is a clone/copy.
            // we import clones, not anything original. so system node type cannot be used.
            treeDom.nullSpecialNodeType();
            // nullify the entire tree with nulled non-db relative indexing.
            // we cannot use db indexing in an exported set.
            treeDom.reindexTree(ref exportIndex);
            treeDom.applySetNode(setNode.chapter.Id);
            List<myTreeDomNode> tree = treeDom.ToList();
            exportIndex = 0; // reset for using as a progress counter

            foreach (myTreeDomNode listedNode in tree)
            {
                // load the rtf from current node
                myNode? node = listedNode.self;
                String rtf = "";
                rtf = entryMethods.DBLoadNodeData(cfg, listedNode.previousID);

                // finally write the node and it's data into destination
                OpenFileSystemDB.createNode(destctx, ref node, rtf, false);

                // update
                this.Invoke(updateProgressStatus, exportIndex++, nodesCount);
            }
            // entire tree structure export completed. now final update and exit.
            this.Invoke(updateProgressStatus, 0, 0);
            this.Invoke(toggleForm, true);
            this.Invoke(showMessageBox, "total entries exported:" + exportIndex, "done", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        public void __exportCustomSet(String path, String dbName, EntryType entryType)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // first save the entry
            this.Invoke(saveEntry);

            this.Invoke(toggleForm, false);

            // collect all source tree and nodes from currently active db
            List<myNode>? srcNodes = entryMethods.DBFindAllNodes(cfg, true, false);
            srcNodes = entryMethods.findAllNodesTreeSequence(ref srcNodes, true, false);

            // first get the total number of chapters which exist in db
            long nodesCount = srcNodes.LongCount();
            Int64 exportIndex = 1; // 0 means no parent node id or node id

            // init new destination open file system db            
            OpenFSDBContext destctx = new OpenFSDBContext();
            OpenFileSystemDB.CreateLoadDB(path, dbName, ref destctx, true, true);

            // first we need to create a set node. we cannot export without a new set node
            // 1 is preset as set node's id. set node is 1st to be indexed before all nodes most recursively.
            myNode? setNode = entryMethods.createSetNode(ref exportIndex, dbName, DateTime.Now);

            // create set node entry in the destination
            OpenFileSystemDB.createNode(destctx, ref setNode, "", false);

            // now get the custom checked root nodes
            List<myNode> checkedNodes = (List<myNode>)this.Invoke(getHighestCheckedTreeViewItemsDBNodes);

            // load tree document object model structure
            myTreeDom treeDom = new myTreeDom();
            treeDom.buildCustomTree(ref srcNodes, ref checkedNodes, true, false);
            // nullify the entire tree with nulled non-db relative indexing.
            // we cannot use db indexing in an exported set.
            treeDom.reindexTree(ref exportIndex);
            treeDom.applySetNode(setNode.chapter.Id);
            List<myTreeDomNode> tree = treeDom.ToList();
            exportIndex = 0; // reset for using as a progress counter

            foreach (myTreeDomNode listedNode in tree)
            {
                // load the rtf from current node
                myNode? node = listedNode.self;
                String rtf = "";
                rtf = entryMethods.DBLoadNodeData(cfg, listedNode.previousID);

                // finally write the node and it's data into destination
                OpenFileSystemDB.createNode(destctx, ref node, rtf, false);

                // update
                this.Invoke(updateProgressStatus, exportIndex++, nodesCount);
            }
            // entire tree structure export completed. now final update and exit.
            this.Invoke(updateProgressStatus, 0, 0);
            this.Invoke(toggleForm, true);
            this.Invoke(showMessageBox, "total entries exported:" + exportIndex, "done", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        public List<TreeNode> __createAncestorList(TreeNode treeNode)
        {
            List<TreeNode> list = new List<TreeNode>();

            while (treeNode.Parent != null)
            {
                list.Add(treeNode.Parent);
                treeNode = treeNode.Parent;
            }
            return list;
        }

        public bool __isHighestCheckedTreeViewItem(TreeNode treeNode)
        {
            List<TreeNode> list = __createAncestorList(treeNode);
            
            foreach (TreeNode node in list)
            {
                if (node.Checked)
                    return false;
            }
            return true;
        }
        public List<TreeNode> __getCheckedTreeViewItems()
        {
            List<TreeNode> list = new List<TreeNode>();
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
        public List<myNode> __getCheckedTreeViewItemsDBNodes()
        {
            List<TreeNode> treeviewItems = __getCheckedTreeViewItems();
            List<myNode> list = new List<myNode>();

            foreach (TreeNode treeNode in treeviewItems)
            {
                myNode? node = null;
                String rtf = "";
                Int64 nodeID = Int64.Parse(treeNode.Name);
                node = entryMethods.DBFindLoadNode(cfg, nodeID, ref rtf, false);
                if (node != null)
                    list.Add(node);
            }
            return list;
        }


        public List<TreeNode> __getHighestCheckedTreeViewItems()
        {
            List<TreeNode> list = new List<TreeNode>();
            Queue<TreeNode> queue = new Queue<TreeNode>();

            foreach(TreeNode node in tvEntries.Nodes)
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
        public List<myNode> __getHighestCheckedTreeViewItemsDBNodes()
        {
            List<TreeNode> treeviewItems = __getHighestCheckedTreeViewItems();
            List<myNode> list = new List<myNode> ();

            foreach (TreeNode treeNode in treeviewItems)
            {
                myNode? node = null;
                String rtf = "";
                Int64 nodeID = Int64.Parse(treeNode.Name);
                node = entryMethods.DBFindLoadNode(cfg, nodeID, ref rtf, false);
                if (node != null)
                    list.Add(node);
            }
            return list;
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            importSet();
        }

        public void importSet()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            browseFolder.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Application.StartupPath;
            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            // load source db and get config
            OpenFSDBContext ctx = new OpenFSDBContext();
            if (!OpenFileSystemDB.LoadDB(ref ctx, browseFolder.SelectedPath))
            {
                reset();
                this.Invoke(showMessageBox, "error loading set db or invalid (non-db) path.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string? input = ctx.dbName;

            // close the db
            ctx.close();

            if (userInterface.ShowInputDialog("input new clone set name/title", ref input) != DialogResult.OK)
                return;

            if (input.Length <= 0)
                return;

            bgWorkerImportSet.RunWorkerAsync(input);

        }

        public void __importSet(String path, String importCloneName)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // first save the entry
            this.Invoke(saveEntry);

            // load source db
            OpenFSDBContext ctx = new OpenFSDBContext();
            if (!OpenFileSystemDB.LoadDB(ref ctx, path))
            {
                reset();
                this.Invoke(showMessageBox, "error loading set db or invalid (non-db) path.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.Invoke(toggleForm, false);

            // collect all tree list from source db
            List<myNode>? srcNodes = OpenFileSystemDB.findAllNodes(ctx, false, false);
            //srcNodes = entryMethods.findAllNodesTreeSequence(ref srcNodes, true, false);
            if (srcNodes.Count <= 0)
                return;

            long nodesCount = srcNodes.LongCount();
            long nodeIndex = 0;

            myTreeDom treeDom = new myTreeDom();
            treeDom.buildTree(ref srcNodes, true, false);

            // we import the set with an additional new clone set node so that
            // the original export set node and it's date and time and settings remain preserved upon import.
            myNode? setNode = null;
            if (cfg.radCfgUseOpenFileSystemDB)
                setNode = entryMethods.createSetNode(ref cfg.ctx1.dbIndexing.currentDBIndex, importCloneName, DateTime.Now);
            else
                setNode = entryMethods.createSetNode(ref cfg.ctx0.dbIndexing.currentDBIndex, importCloneName, DateTime.Now);

            // create set node entry in the destination
            if (!entryMethods.DBCreateNode(ref cfg, ref setNode, "", false, true))
                return;

            // we must nullify system node type
            // we import clones, not anything original. so system node type cannot be used.
            treeDom.nullSpecialNodeType();

            // only a clone shall be imported. one and same set cannot be imported repeatedly because it
            // corrupts the db. therefore a set always shall be imported as a new clone.
            // it is user's own effort to manage the imported entries.
            // prepare the tree with proper current db's indexing
            if (cfg.radCfgUseOpenFileSystemDB)
                treeDom.reindexTree(ref cfg.ctx1.dbIndexing.currentDBIndex);
            else
                treeDom.reindexTree(ref cfg.ctx0.dbIndexing.currentDBIndex);

            // apply clone set node
            treeDom.applySetNode(setNode.chapter.Id);

            // create a newly changed tree list from the tree dom structure.
            List<myTreeDomNode> tree = treeDom.ToList();

            // now direct import
            foreach (myTreeDomNode listedNode in tree)
            {
                String rtf = "";
                OpenFileSystemDB.findLoadNode(ctx, listedNode.previousID, ref rtf, true);
                if (!entryMethods.DBCreateNode(ref cfg, ref listedNode.self, rtf, false, false))
                    continue;

                // update ui
                this.Invoke(updateProgressStatus, nodeIndex++, nodesCount);
            }

            // finally update the db index in file.
            entryMethods.DBWriteIndexing(ref cfg);

            // update
            this.Invoke(updateProgressStatus, 0, 0);
            this.Invoke(toggleForm, true);
            __loadEntries();

        }

        private void bgWorkerImportSet_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            String importCloneName = (String)e.Argument;
            __importSet(browseFolder.SelectedPath, importCloneName);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            exportCustomSet();
        }
        public void exportCustomSet()
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            if (browseFolder.ShowDialog(this) != DialogResult.OK)
                return;

            string? input = "";
            if (userInterface.ShowInputDialog("input set name/title", ref input) != DialogResult.OK)
                return;

            if (input.Length <= 0)
                return;

            bgWorkerExportCustomSet.RunWorkerAsync(input);

        }
        private void bgWorkerExportCustomSet_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            String dbName = (String)e.Argument;
            __exportCustomSet(browseFolder.SelectedPath, dbName, EntryType.Rtf);

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
            if (!entryMethods.autoCreateLoadSystemNodes(ref cfg, out systemNodes))
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

        private void bgWorkerExportEntries_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            EntryType entryType = (EntryType)e.Argument;

            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return;

            // first save the entry
            this.Invoke(saveEntry);

            this.Invoke(toggleForm, false);

            // collect all source tree and nodes from currently active db
            List<myNode>? srcNodes = entryMethods.DBFindAllNodes(cfg, false, false);

            // now get the custom checked root nodes
            List<myNode> checkedNodes = (List<myNode>)this.Invoke(getHighestCheckedTreeViewItemsDBNodes);

            // load tree document object model structure
            myTreeDom treeDom = new myTreeDom();
            treeDom.buildCustomTree(ref srcNodes, ref checkedNodes, true, false);
            List<myTreeDomNode> tree = treeDom.ToList();
            long nodesCount = tree.LongCount();
            Int64 exportIndex = 0; 

            foreach (myTreeDomNode listedNode in tree)
            {
                // load the rtf from current node
                myNode? node = listedNode.self;

                // straightaway export the entry
                entryMethods.exportEntry(cfg, node.chapter.Id, browseFolder.SelectedPath, exportIndex, entryType);

                // update
                this.Invoke(updateProgressStatus, exportIndex++, nodesCount);
            }
            // entire tree structure export completed. now final update and exit.
            this.Invoke(updateProgressStatus, 0, 0);
            this.Invoke(toggleForm, true);
            this.Invoke(showMessageBox, "total entries exported:" + exportIndex, "done", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            doExportCheckedEntries(EntryType.Txt);
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            doExportCheckedEntries(EntryType.Rtf);

        }
    }

}