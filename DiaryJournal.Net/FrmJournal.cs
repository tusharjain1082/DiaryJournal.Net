using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace DiaryJournal.Net
{
    public partial class FrmJournal : Form
    {
        public myContext ctx = new myContext();
        public bool stateChanged = false;
        public textFormatting formatting = null;
        public String previousRtf = "";
        public FormFind? myFormFind = null;
        bool properExit = false;



        // delegates
        public delegate void __initTreeViewYearEntryDelegate(Chapter chapter);
        public __initTreeViewYearEntryDelegate initTreeViewYearEntry;
        public delegate void __initTreeViewMonthEntryDelegate(Chapter chapter);
        public __initTreeViewMonthEntryDelegate initTreeViewMonthEntry;
        public delegate TreeNode __initTreeViewDayTimeBasedEntryDelegate(Chapter chapter);
        public __initTreeViewDayTimeBasedEntryDelegate initTreeViewDayTimeBasedEntry;
        public delegate TreeNode __initTreeViewChildEntryDelegate(Chapter chapter);
        public __initTreeViewChildEntryDelegate initTreeViewChildEntry;
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
        public delegate void __insertLvTrashCanItemDelegate(Chapter chapter);
        public __insertLvTrashCanItemDelegate insertLvTrashCanItem;
        public delegate void __treeViewBeginUpdateDelegate(TreeView tv);
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


        public FrmJournal()
        {
            InitializeComponent();
        }

        private void FrmJournal_Load(object sender, EventArgs e)
        {
            // Add a reference to the NuGet package System.Text.Encoding.CodePages for .Net core only
            // important initialization for RtfPipe Library:
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            AppContext.SetSwitch("Switch.System.Windows.Forms.DoNotLoadLatestRichEditControl", false);

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

            String strDateTimeTemplate = DiaryJournal.Net.Properties.Resources.BuildDateTime;
            DateTime buildDateTime = DateTime.Parse(strDateTimeTemplate);
            String strBuildDateTime = buildDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            this.Text = "Tushar Jain's " + this.Text + " Version " + Application.ProductVersion + ", Compiled/Built on: " + strBuildDateTime;

            // setup delegates
            initTreeViewYearEntry = new __initTreeViewYearEntryDelegate(__initTreeViewYearEntry);
            initTreeViewMonthEntry = new __initTreeViewMonthEntryDelegate(__initTreeViewMonthEntry);
            initTreeViewDayTimeBasedEntry = new __initTreeViewDayTimeBasedEntryDelegate(__initTreeViewDayTimeBasedEntry);
            updateProgressStatus = new __updateProgressStatusDelegate(__updateProgressStatus);
            toggleForm = new __toggleFormDelegate(__toggleForm);
            showMessageBox = new __showMessageBoxDelegate(__showMessageBox);
            setCalendarHighlightEntry = new __setCalendarHighlightEntryDelegate(__setCalendarHighlightEntry);
            gotoEntry = new __gotoEntryDelegate(__gotoEntry);
            gotoTodaysEntry = new __gotoTodaysEntryDelegate(__gotoTodaysEntry);
            updateTotalEntriesStatus = new __updateTotalEntriesStatusDelegate(__updateTotalEntriesStatus);
            resetTrashCan = new __resetTrashCanDelegate(__resetTrashCan);
            insertLvTrashCanItem = new __insertLvTrashCanItemDelegate(__insertLvTrashCanItem);
            initTreeViewChildEntry = new __initTreeViewChildEntryDelegate(__initTreeViewChildEntry);
            treeViewBeginUpdate = new __treeViewBeginUpdateDelegate(__treeViewBeginUpdate);
            treeViewEndUpdate = new __treeViewEndUpdateDelegate(__treeViewEndUpdate);
            LvTrashCanUpdate = new __LvTrashCanUpdateDelegate(__LvTrashCanUpdate);
            resetLVSearch = new __resetLVSearchDelegate(__resetLVSearch);
            LvSearchUpdate = new __LvSearchUpdateDelegate(__LvSearchUpdate);
            processSearch = new __processSearchDelegate(__processSearch);

            // now load config file and setup
            myConfigMethods.autoCreateLoadConfigFile(ctx, false);
            applyConfig(ctx);
            
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

            //            File.WriteAllText(@"C:\0--2022-08-02-09-46-10-000----result.rtf", theJournalMethods.fixTheJournalRtfEntry(File.ReadAllText(@"C:\0--2022-08-02-09-46-10-000----.rtf")));
            //File.WriteAllText(@"Q:\Chapters - Sunday, 31 July, 2022 - 1145 - result.rtf", theJournalMethods.fixTheJournalRtfEntry(File.ReadAllText(@"Q:\Chapters - Sunday, 31 July, 2022 - 1145.rtf")));

            //long i = 0;
            //DateTime dateTime = DateTime.Now;
            //String title = "";
            //entryMethods.validateExtractEntryFile(@"C:\95--2019-05-28-12-50-00-000--sdsd s. sdsdsdds(1 sds)ds dsdssdldj--.rtf", ref i, ref dateTime, ref title);

            /*
            //Chapter? chapter = new Chapter();
            //entryMethods.convertEntryFilenameToChapter(ref chapter, @"Q:\320--2022-08-04-16-59-24-574--b9d241cb-03a5-4cf5-83ee-591ce71d2ead--ab2db20e-f88b-4517-81e1-096bfa22ad75--(1)--.rtf");
            String text = @"Q:\fdfdjf mdmdkf jkdnfdndkfndnf1q1#$$#^%fg Earth ds sds3$$f fe.4554 is One dkfdndkjnf Earth sdlmd is One dkfdnfdkjfwdkf 320--2022-08-04-16- abd2 59-24-574--b9d abd2 241cb-03a5-4c aggggg12d2 f5-83ee-591cab acd2 2e71d2ead--ab2db20e-f88b-4517- abbbcb2 81e1-096bfa22ad75--(1)--.rtf";
            // whole word but with any number of any characters which is separated from all the string
            String pattern0 = String.Format(@"({0})", @"\bEa\w+One\b");
            Regex regex0 = new Regex(pattern0);
            MatchCollection matches0 = regex0.Matches(text);
            // a word anywhere in string which is a substring
            String pattern1 = @"(ab2)";
            Regex regex1 = new Regex(pattern1);
            MatchCollection matches1 = regex1.Matches(text);
            // match case, ignore case option not enabled
            String pattern2 = @"(Ab2)";
            Regex regex2 = new Regex(pattern2);
            MatchCollection matches2 = regex2.Matches(text);
            // ignore case option and explicit capture with indexing used
            String pattern3 = @"(?<1>Ab2)";
            Regex regex3 = new Regex(pattern3, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
            MatchCollection matches3 = regex3.Matches(text);
            return;
            */

           /* 
           //  * multiple substrings of wild card pattern  match
            String input = "dsklfjfdwfn abcdeflkndwnfdndlfdn" +
                " dfmdsfdsmdsfdn  ds07:21 PM\r\nFriday, January 29, 2021dfmdlf dmfdmfdmfdfmdlf" +
                "ldsnl dlnfdlfn lnfabcdef342rln43lt30r gnm43$#6t vlt3n 3";
            String pattern0 = @"(07\:21 PM(\r+\n+)Friday, January 29, 2021)";
            String output0 = "";
            String replacement0 = "11111111111111";
            Regex regex0 = new Regex(pattern0);
            MatchCollection matches0 = regex0.Matches(input);
            String result = Regex.Replace(input, pattern0, replacement0);
            return;
            */


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
            saveCloseDB();

            base.OnClosing(e);

        }

        private void FrmJournal_Shown(object? sender, EventArgs e)
        {
            // take final actions as per the stored configuration
            if (ctx.config.chkCfgAutoLoadCreateDefaultDB)
                autoCreateLoadDefaultDB(false);


        }

        public void applyConfig(myContext ctx)
        {
            chkCfgAutoLoadCreateDefaultDB.Checked = ctx.config.chkCfgAutoLoadCreateDefaultDB;

            // set editor's flow limit
            rtbEntry.RightMargin = ctx.config.cmbCfgRtbEntryRMValue;
            rtbViewEntry.RightMargin = ctx.config.cmbCfgRtbViewEntryRMValue;
            int index = cmbCfgRtbEntryRM.FindString(rtbEntry.RightMargin.ToString());
            cmbCfgRtbEntryRM.SelectedIndex = index;
            index = cmbCfgRtbViewEntryRM.FindString(rtbViewEntry.RightMargin.ToString());
            cmbCfgRtbViewEntryRM.SelectedIndex = index;

        }

        private void LvTrashCan_DoubleClick(object? sender, EventArgs e)
        {
            viewSelectedTrashCanEntry();

        }

        public void OpenSelectedSearchedEntry()
        {
            if (!ctx.isDBOpen())
                return;

            if (lvSearch.SelectedItems.Count == 0)
                return;

            ListViewItem listViewItem = lvSearch.SelectedItems[0];
            if (listViewItem == null)
                return;

            Guid entryGuid = (Guid)listViewItem.Tag;
            if (entryGuid == null)
                return;

            Chapter? chapter = myDB.findDbChapterByGuid(ctx, entryGuid);
            if (chapter == null)
                return;

            if (chapter.IsDeleted)
                return;

            tabControlJournal.SelectedIndex = 0;

            __gotoEntryByGuid(chapter.guid);
        }

        public void viewSelectedTrashCanEntry()
        {
            if (!ctx.isDBOpen())
                return;

            if (lvTrashCan.SelectedItems.Count == 0)
                return;

            ListViewItem listViewItem = lvTrashCan.SelectedItems[0];
            if (listViewItem == null)
                return;

            Chapter? id = (Chapter)listViewItem.Tag;
            if (id == null)
                return;

            // 1st load chapter's data blob
            ChapterData? chapterData = myDB.loadDBChapterData(ctx,  id.guid);
            if (chapterData == null)
                return;

            String rtf = chapterData.data;
            rtf = commonMethods.Base64Decode(rtf);
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
            if (!ctx.isDBOpen())
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

            // first save the entry
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
            if (!ctx.isDBOpen())
                return;

            // create new chapter direct
            Chapter? chapter = myDB.newChapter(ctx, ref ctx.totalEntries);
            if (chapter == null)
                return;

            // finally update tree view because entry was imported successfully
            initTreeViewYearEntry(chapter);
            initTreeViewMonthEntry(chapter);
            TreeNode newNode = initTreeViewDayTimeBasedEntry(chapter);
            tvEntries.SelectedNode = newNode;

            // update
            ctx.totalEntries++;
            tsslblTotalEntries.Text = ctx.totalEntries.ToString();
            __setCalendarHighlightEntry(chapter.chapterDateTime);
        }

        public void setupNewEntry(DateTime dateTime)
        {
            if (!ctx.isDBOpen())
                return;

            // create new chapter direct
            Chapter? chapter = myDB.newChapter(ctx, ref ctx.totalEntries, dateTime, true);
            if (chapter == null)
                return;

            // finally update tree view because entry was imported successfully
            initTreeViewYearEntry(chapter);
            initTreeViewMonthEntry(chapter);
            TreeNode newNode = initTreeViewDayTimeBasedEntry(chapter);
            tvEntries.SelectedNode = newNode;

            // update
            ctx.totalEntries++;
            tsslblTotalEntries.Text = ctx.totalEntries.ToString();

            __setCalendarHighlightEntry(chapter.chapterDateTime);
        }

        private void newDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sfdDB.InitialDirectory = Application.StartupPath;
            if (sfdDB.ShowDialog() != DialogResult.OK)
                return;

            // first reset everything
            reset();


            // todo
            if (!myDB.CreateLoadDB(sfdDB.FileName, ref ctx, true))
            {
                reset();
                this.Invoke(showMessageBox, "errror creating/loading db", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // setup ui
            txtDBFile.Text = ctx.dbpath;

        }

        private void closeDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ctx.isDBOpen())
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
            if (!ctx.isDBOpen())
                return;

            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            bgWorkerTJEntries.RunWorkerAsync();
        }

        public void importEntries(EntryType entryType)
        {
            // run in background worker
            bgWorkerImportEntries.RunWorkerAsync(entryType);

        }

        public void __setCalendarHighlightEntry(DateTime dateTime)
        {
            DateTime day = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
            CalendarEntries.AddBoldedDate(day);
            CalendarEntries.UpdateBoldedDates();

        }

        public void __treeViewBeginUpdate(TreeView tv)
        {
            tv.BeginUpdate();
        }
        public void __treeViewEndUpdate(TreeView tv)
        {
            tv.EndUpdate();
        }

        public void importEntries(String path, EntryType entryType)
        {
            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            myDB.getEntryTypeFormats(entryType, ref ext, ref extComplete, ref extSearchPattern);

            long filesDone = 0;
            IEnumerable<String> files = Directory.EnumerateFiles(path, extSearchPattern);//.OrderBy(filename => filename);
            long totalFiles = files.LongCount();
            if (totalFiles <= 0)
                return;

            this.Invoke(toggleForm, false);

            // 1st phase, import all entries direct in db 
            for (long i = 0; i < totalFiles; i++)
            {
                // find the file by export index
                String file = entryMethods.findEntryFileByExportIndex(files, i);
                if (file.Length <= 0)
                    continue; // file not found, so do not insert it and skip

                // matching file found, import direct
                Chapter? chapter = null;
                chapter = entryMethods.importEntry(ctx, file, entryType);
                // if error, then skip this file
                if (chapter == null)
                    continue;

                this.Invoke(updateProgressStatus, filesDone++, totalFiles);
            }

            // 2nd phase, load the entires into treeview
            this.Invoke(updateProgressStatus, 0, 0);
            this.Invoke(toggleForm, true);
            loadEntries(ctx);
//            this.Invoke(showMessageBox, "total entries imported:" + filesDone, "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void importTheJournalRtfEntries(String path)
        {
            long filesDone = 0;
            long totalFiles = Directory.EnumerateFiles(path, "*.rtf").LongCount();
            if (totalFiles <= 0)
                return;


            this.Invoke(toggleForm, false);

            RichTextBox richTextBox = new RichTextBox();

            IEnumerable<String> files = Directory.EnumerateFiles(path, "*.rtf").OrderBy(filename => filename);//Directory.EnumerateFiles(path, "*.rtf"))
            foreach (string file in files)
            {
                Chapter? chapter = theJournalMethods.convertFilenameToChapter(file);
                if (chapter == null)
                    continue;

                // get rtf and update
                String rtf = File.ReadAllText(file);
                rtf = theJournalMethods.fixTheJournalRtfEntry(rtf);
                richTextBox.Rtf = rtf;
                rtf = commonMethods.Base64Encode(richTextBox.Rtf);

                // entry's identification in the application and in the database.
                chapter.guid = Guid.NewGuid();

                // 1st import the chapter's data blob
                ChapterData? chapterData = myDB.newChapterData(chapter.guid, rtf);
                myDB.importNewDBChapterData(ctx, chapterData);

                // 2nd now import the entry as a chapter into db
                if (!myDB.importNewDBChapter(ctx, ref chapter))
                    continue;

                this.Invoke(updateProgressStatus, filesDone++, totalFiles);
            }

            // 2nd phase, load the entires into treeview
            this.Invoke(updateProgressStatus, 0, 0);
            this.Invoke(toggleForm, true);
            loadEntries(ctx);
            //            this.Invoke(showMessageBox, "total entries imported:" + filesDone, "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void UpdateAllOnce(Chapter chapter, long ChaptersCount)
        {
            // update tree view because entry was imported successfully
            this.Invoke(initTreeViewYearEntry, chapter);
            this.Invoke(initTreeViewMonthEntry, chapter);
            this.Invoke(initTreeViewDayTimeBasedEntry, chapter);

            // update
            ctx.totalEntries = ChaptersCount;
            tsslblTotalEntries.Text = ChaptersCount.ToString();
            this.Invoke(setCalendarHighlightEntry, chapter.chapterDateTime);
        }

        public bool exportEntry(Chapter chapter, String path, long exportIndex, EntryType entryType)
        {
            if (chapter == null)
                return false;

            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            myDB.getEntryTypeFormats(entryType, ref ext, ref extComplete, ref extSearchPattern);

            // 1st load chapter's data blob
            String rtf = "";
            ChapterData? chapterData = myDB.loadDBChapterData(ctx, chapter.guid);
            if (chapterData != null)
                rtf = chapterData.data;

            String fileData = "";
            switch (entryType)
            {
                case EntryType.Xml:
                    fileData = xmlEntry.toXml(chapter, rtf);
                    break;

                case EntryType.Rtf:
                    fileData = rtfEntry.toRtf(rtf);
                    break;

                case EntryType.Html:
                    fileData = htmlEntry.toHtml(rtf);
                    break;

                default:
                    break;
            }


            // output
            String modifiedTitle = ((chapter.Title != null) ? chapter.Title.Replace("--", "-") : "");
            String entryName = String.Format("{0}--{1}--{2}--{3}--{4}--.{5}", exportIndex, chapter.chapterDateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff"),
                chapter.guid, chapter.parentGuid, modifiedTitle, ext);
            String file = Path.Combine(path, Path.GetFileName(entryName));
            file = @"\\?\" + file;
            File.WriteAllText(file, fileData);
            return true;
        }

        public void exportEntries(String path, EntryType entryType)
        {
            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            myDB.getEntryTypeFormats(entryType, ref ext, ref extComplete, ref extSearchPattern);

            // first get the total number of chapters which exist in db
            long ChaptersCount = myDB.ChaptersCount(ctx);
            long filesDone = 0;
            long exportIndex = 0;
            long totalFiles = ChaptersCount;
            if (totalFiles <= 0)
                return;

            this.Invoke(toggleForm, false);

            ctx.identifiers = myDB.FindAllChapters(ctx).ToList();

            foreach (Chapter chapter in ctx.identifiers)
            {
                if (!exportEntry(chapter, path, exportIndex, entryType))
                    continue;

                // update
                filesDone += 1;
                exportIndex += 1;
                this.Invoke(updateProgressStatus, filesDone, totalFiles);
            }

            // done
            this.Invoke(updateProgressStatus, 0, 0);
            this.Invoke(toggleForm, true);
            this.Invoke(showMessageBox, "total entries exported:" + filesDone, "done", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        public void __initTreeViewYearEntry(Chapter chapter)
        {
            String path = chapter.chapterDateTime.Year.ToString() + @"\";
            TreeNode[] nodes = tvEntries.Nodes.Find(path, false);
            if (nodes.Length == 0) // no node with this entry exists, so create it
                tvEntries.Nodes.Add(path, chapter.chapterDateTime.Year.ToString());

        }

        public void __initTreeViewMonthEntry(Chapter chapter)
        {
            String path = chapter.chapterDateTime.Year.ToString() + @"\";
            TreeNode[] nodes = tvEntries.Nodes.Find(path, false);
            if (nodes.Length > 0)
            {
                // found year entry
                // process month entry
                path = chapter.chapterDateTime.Month.ToString() + @"\";
                // find if month node doesn't exists in the parent year then create it
                TreeNode[] monthNodes = nodes[0].Nodes.Find(path, false);
                if (monthNodes.Length == 0) // month entry node does not exists, so create it
                    nodes[0].Nodes.Add(path, chapter.chapterDateTime.ToString("MMMM"));
            }

        }

        public TreeNode? __initTreeViewDayTimeBasedEntry(Chapter chapter)
        {
            String path = chapter.chapterDateTime.Year.ToString() + @"\";
            TreeNode[] nodes = tvEntries.Nodes.Find(path, false);
            if (nodes.Length > 0)
            {
                // found year entry
                // process month entry
                path = chapter.chapterDateTime.Month.ToString() + @"\";
                // find if month node doesn't exists in the parent year then create it
                TreeNode[] monthNodes = nodes[0].Nodes.Find(path, false);
                if (monthNodes.Length == 0)
                    return null; // month node never existed, abort

                // parent path found, create the entry node.
                path = String.Format(@"{0}\", chapter.guid);
                String entryName = String.Format(@"Day({0}):::Time({1}:{2}:{3})Title({4})", chapter.chapterDateTime.Day, 
                    chapter.chapterDateTime.Hour, chapter.chapterDateTime.Minute, chapter.chapterDateTime.Second, chapter.Title);
                TreeNode newNode = monthNodes[0].Nodes.Add(path, entryName);
                newNode.Tag = chapter; // this is the actual identification in both this form and in the database.
                if (chapter.HLFont.Length >= 1)
                    newNode.NodeFont = commonMethods.StringToFont(chapter.HLFont);

                if (chapter.HLFontColor.Length >= 1)
                    newNode.ForeColor = commonMethods.StringToColor(chapter.HLFontColor);

                if (chapter.HLBackColor.Length >= 1)
                    newNode.BackColor = commonMethods.StringToColor(chapter.HLBackColor);

                return newNode;
            }
            else
            {
                return null;
            }
        }

        public TreeNode? __initTreeViewChildEntry(Chapter chapter)
        {
            String path = chapter.parentGuid.ToString() + @"\";
            TreeNode[] nodes = tvEntries.Nodes.Find(path, true);
            if (nodes.Length > 0)
            {
                // parent path found, create the entry node.
                path = String.Format(@"{0}\", chapter.guid);
                String entryName = String.Format(@"Date({0}):::Time({1}:{2}:{3})Title({4})", chapter.chapterDateTime.ToString("dd-MM-yyyy"),
                    chapter.chapterDateTime.Hour, chapter.chapterDateTime.Minute, chapter.chapterDateTime.Second, chapter.Title);
                TreeNode newNode = nodes[0].Nodes.Add(path, entryName);
                newNode.Tag = chapter; // this is the actual identification in both this form and in the database.
                if (chapter.HLFont.Length >= 1)
                    newNode.NodeFont = commonMethods.StringToFont(chapter.HLFont);

                if (chapter.HLFontColor.Length >= 1)
                    newNode.ForeColor = commonMethods.StringToColor(chapter.HLFontColor);

                if (chapter.HLBackColor.Length >= 1)
                    newNode.BackColor = commonMethods.StringToColor(chapter.HLBackColor);

                return newNode;
            }
            return null;
        }

        public void loadEntries(myContext ctx)
        {
            // first get the total number of chapters which exist in db
            long ChaptersCount = myDB.ChaptersCount(ctx);
            long filesDone = 0;
            long totalFiles = ChaptersCount;
            if (totalFiles <= 0)
                return;

            this.Invoke(toggleForm, false);

            ctx.totalEntries = ChaptersCount;

            ctx.identifiers = myDB.FindAllChapters(ctx).ToList();

            foreach (Chapter chapter in ctx.identifiers)
            {
                if (chapter.IsDeleted)
                    continue;

                // finally update tree view because entry was imported successfully
                this.Invoke(initTreeViewYearEntry, chapter);
                this.Invoke(initTreeViewMonthEntry, chapter);

                // if this is a child entry, then install it as a child in it's parent.
                if (chapter.parentGuid != Guid.Empty)
                    this.Invoke(initTreeViewChildEntry, chapter);
                else
                    this.Invoke(initTreeViewDayTimeBasedEntry, chapter);

                // update
                filesDone += 1;
                ctx.totalEntries = filesDone;
                this.Invoke(updateProgressStatus, filesDone, totalFiles);
                this.Invoke(setCalendarHighlightEntry, chapter.chapterDateTime);
                this.Invoke(updateTotalEntriesStatus, ctx.totalEntries);
            }
            this.Invoke(updateProgressStatus, 0, 0);
            this.Invoke(toggleForm, true);
            this.Invoke(gotoTodaysEntry);
            this.Invoke(updateTotalEntriesStatus, ctx.totalEntries);
        }

        public void loadDB(String path)
        {
            if (path == "")
                return;

            // firstly save entry
            saveEntry();

            // reset everything
            reset();

            // todo
            if (!myDB.CreateLoadDB(path, ref ctx, false))
            {
                reset();
                this.Invoke(showMessageBox, "errror creating/loading db", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // setup ui
            txtDBFile.Text = ctx.dbpath;

            // run in background worker
            bgWorkerLoadDB.RunWorkerAsync();
        }

        private void loadExistingDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofdDB.InitialDirectory = Application.StartupPath;
            if (ofdDB.ShowDialog() != DialogResult.OK)
                return;

            loadDB(ofdDB.FileName);
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
            ctx.close();
        }

        private void TvEntries_BeforeSelect(object? sender, TreeViewCancelEventArgs e)
        {
            // now first save the previous state if the state was changed.
            saveEntry();
        }

        private void tvEntries_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // get identifier to identify the chapter in db
            Chapter identifier = (Chapter)e.Node.Tag;
            if (identifier == null)
                return;
 
            loadSelectedEntry(identifier);
        }

        public void loadSelectedEntry(Chapter identifier)
        {
            // get the chapter from db by identifier
            Chapter? dbChapter = myDB.findDbChapterByGuid(ctx, identifier.guid);
            if (dbChapter == null)
                return;

            // 1st load chapter's data blob
            ChapterData? chapterData = myDB.loadDBChapterData(ctx, dbChapter.guid);
            String rtf = chapterData.data;

            // setup rtf
            // get rtf and update
            rtf = commonMethods.Base64Decode(rtf);
            resetRtb(rtbEntry, true, true);
            rtbEntry.Rtf = rtf;
            tsslblEntryTitle.Text = dbChapter.Title;
            CalendarEntries.SelectionStart = dbChapter.chapterDateTime;
            CalendarEntries.SelectionEnd = dbChapter.chapterDateTime;
        }

        private void newEntryNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setupNowEntry();
        }

        private void saveEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveEntry();
        }

        public void saveEntry()
        {
            if (!ctx.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            if (!stateChanged)
                return;

            // get identifier from node
            Chapter chapter = (Chapter)tvEntries.SelectedNode.Tag;
            if (chapter == null)
                return;

            // get original database chapter using identifier
            Chapter? dbChapter = myDB.findDbChapterByGuid(ctx, chapter.guid);
            if (dbChapter == null)
                return;

            // finall save/update
            myDB.UpdateChapterAndData(ctx, dbChapter, rtbEntry.Rtf);
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

            // get identifier from node
            Chapter? identifier = (Chapter)tvEntries.SelectedNode.Tag;
            if (identifier == null)
                return;

            string? input = identifier.Title;
            if (userInterface.ShowInputDialog("input title for entry", ref input) != DialogResult.OK)
                return;

            // finally set the title
            bool isChild = ((identifier.parentGuid != Guid.Empty) ? true : false);
            setupEntryTitle(input, isChild);
        }

        private void entryTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeEntryTitle();
        }

        public void setupEntryTitle(String title, bool childEntry)
        {
            if (tvEntries.SelectedNode == null)
                return;

            // get identifier from node
            Chapter identifier = (Chapter)tvEntries.SelectedNode.Tag;
            if (identifier == null)
                return;

            if (!myDB.updateChapterTitleByIDChapter(ctx, identifier, title))
            {
                MessageBox.Show("error updating the entry in db.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // resetup tree node
            String path = String.Format(@"{0}\", identifier.guid);
            String entryName = "";
            if (!childEntry)
                entryName = String.Format(@"Day({0}):::Time({1}:{2}:{3})Title({4})", identifier.chapterDateTime.Day, 
                    identifier.chapterDateTime.Hour, identifier.chapterDateTime.Minute, identifier.chapterDateTime.Second, title);
            else
                entryName = String.Format(@"Date({0}):::Time({1}:{2}:{3})Title({4})", identifier.chapterDateTime.ToString("dd-MM-yyyy"),
                    identifier.chapterDateTime.Hour, identifier.chapterDateTime.Minute, identifier.chapterDateTime.Second, title);

            tvEntries.SelectedNode.Text = entryName;
            tvEntries.SelectedNode.Name = path;
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
            loadEntries(ctx);
        }

        private void bgWorkerImportEntries_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            EntryType entryType = (EntryType)e.Argument;
            importEntries(browseFolder.SelectedPath, entryType);
        }

        private void bgWorkerExportEntries_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            EntryType entryType = (EntryType)e.Argument;
            exportEntries(browseFolder.SelectedPath, entryType);
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
            if (!ctx.isDBOpen())
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
            if (!ctx.isDBOpen())
                return;

            // get original database chapter using identifier
            Chapter? dbChapter = myDB.findDbChapterByDateTime(ctx, dateTime.Year, dateTime.Month, dateTime.Day);
            if (dbChapter != null)
            {
                String path = String.Format(@"{0}\", dbChapter.guid);
                TreeNode[] nodes = tvEntries.Nodes.Find(path, true);
                if (nodes.Count() <= 0)
                    return;

                // found one node in tree view
                tvEntries.SelectedNode = nodes[0];
            }

        }
        public void __gotoEntryByGuid(Guid guid)
        {
            if (!ctx.isDBOpen())
                return;

            // get original database chapter using identifier
            Chapter? dbChapter = myDB.findDbChapterByGuid(ctx, guid);
            if (dbChapter != null)
            {
                String path = String.Format(@"{0}\", dbChapter.guid);
                TreeNode[] nodes = tvEntries.Nodes.Find(path, true);
                if (nodes.Count() <= 0)
                    return;

                // found one node in tree view
                tvEntries.SelectedNode = nodes[0];
            }
        }

        public void __gotoTodaysEntry()
        {
            gotoEntry(DateTime.Now);
        }

        private void gotoDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ctx.isDBOpen())
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

        public void exportEntries(bool toxml)
        {
            // run in background worker
            bgWorkerExportEntries.RunWorkerAsync(toxml);
        }


        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (!ctx.isDBOpen())
                return;

            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            bgWorkerExportEntries.RunWorkerAsync(EntryType.Xml);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (!ctx.isDBOpen())
                return;

            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            importEntries(EntryType.Xml);

        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            importEntryFile(EntryType.Xml);
        }

        public void importEntryFile(EntryType entryType)
        {
            if (!ctx.isDBOpen())
                return;

            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            myDB.getEntryTypeFormats(entryType, ref ext, ref extComplete, ref extSearchPattern);

            ofdEntry.Filter = String.Format("{0} files *.{1}|*.{2}", ext, ext, ext);
            ofdEntry.FilterIndex = 1;
            ofdEntry.DefaultExt = extSearchPattern;
            ofdEntry.InitialDirectory = Application.StartupPath;
            if (ofdEntry.ShowDialog() != DialogResult.OK)
                return;

            Chapter? chapter = entryMethods.importEntry(ctx, ofdEntry.FileName, entryType);
            if (chapter == null)
                return;

            // finally update tree view because entry was imported successfully
            this.Invoke(initTreeViewYearEntry, chapter);
            this.Invoke(initTreeViewMonthEntry, chapter);
            this.Invoke(initTreeViewDayTimeBasedEntry, chapter);

            ctx.totalEntries += 1;
            this.Invoke(setCalendarHighlightEntry, chapter.chapterDateTime);
            this.Invoke(updateTotalEntriesStatus, ctx.totalEntries);

            // now load the entry from the tree view
            __gotoEntryByGuid(chapter.guid);
        }

        private void newEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newEntry();
        }

        public void newEntry()
        {
            if (!ctx.isDBOpen())
                return;

            DateTime inputDate = DateTime.Now;
            if (userInterface.ShowDateTimeDialog("new entry's date and time", ref inputDate, CalendarEntries.BoldedDates) != DialogResult.OK)
                return;

            setupNewEntry(inputDate);
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

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            exportSelectedEntry(EntryType.Xml);
        }

        private void bgWorkerTJEntries_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            importTheJournalRtfEntries(browseFolder.SelectedPath);
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
            if (!ctx.isDBOpen())
                return;

            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            bgWorkerExportEntries.RunWorkerAsync(EntryType.Html);
        }

        private void tsbuttonStrikeout_Click(object sender, EventArgs e)
        {
            if (rtbEntry.SelectionFont != null)
                formatting.formatStrikeout(rtbEntry, (ToolStripButton)sender);

        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            exportSelectedEntry(EntryType.Html);
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            if (!ctx.isDBOpen())
                return;

            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            bgWorkerExportEntries.RunWorkerAsync(EntryType.Rtf);

        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            exportSelectedEntry(EntryType.Rtf);
        }

        public void exportSelectedEntry(EntryType entryType)
        {
            if (!ctx.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            if (stateChanged)
                saveEntry();

            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            // get identifier from node
            Chapter chapter = (Chapter)tvEntries.SelectedNode.Tag;
            if (chapter == null)
                return;

            // get original database chapter using identifier
            Chapter? dbChapter = myDB.findDbChapterByGuid(ctx, chapter.guid);
            if (dbChapter == null)
                return;

            // finally export the chapter
            exportEntry(dbChapter, browseFolder.SelectedPath, 0, entryType);
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            if (!ctx.isDBOpen())
                return;

            if (browseFolder.ShowDialog() != DialogResult.OK)
                return;

            importEntries(EntryType.Rtf);
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            importEntryFile(EntryType.Rtf);

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
            ctx.config.chkCfgAutoLoadCreateDefaultDB = chkCfgAutoLoadCreateDefaultDB.Checked;
            ctx.config.cmbCfgRtbEntryRMValue = rtbEntryRightMargin;
            ctx.config.cmbCfgRtbViewEntryRMValue = rtbViewEntryRightMargin;
            myConfigMethods.saveConfigFile(myConfigMethods.getConfigPathFile(), ctx, false);
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

            // direct auto create/load default db
            if (!myDB.autoLoadCreateDefaultDB(ctx, overwrite))
            {
                MessageBox.Show("error creating/loading the default db. aborted.", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            // setup ui
            txtDBFile.Text = ctx.dbpath;

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
            loadDB(ctx.dbpath);
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
            ofdFile.Filter = @"*.bmp|*.bmp|*.jpg|*.jpg|*.jpeg|*.jpeg|*.gif|*.gif|*.tiff|*.tiff|*.png|*.png|all files *.*|*.*";
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
            if (!ctx.isDBOpen())
                return;

            // first get the total number of chapters which exist in db
            long ChaptersCount = myDB.ChaptersCount(ctx);
            ctx.totalEntries= ChaptersCount;
            MessageBox.Show("total number of " + ChaptersCount + " entries actually exist in db.", "status", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void tsbuttonDeleteEntry_Click(object sender, EventArgs e)
        {
            deleteEntries();   
        }

        public void deleteEntries()
        {
            if (!ctx.isDBOpen())
                return;

            // first save the entry
            saveEntry();

            ctx.identifiers = myDB.FindAllChapters(ctx).ToList();
            ctx.totalEntries = ctx.identifiers.LongCount();

            List<TreeNode> deletedNodes = new List<TreeNode>();

            foreach (Chapter identifier in ctx.identifiers)
            {
                if (identifier.IsDeleted)
                    continue;

                // find entry in treeview by identifier
                String path = String.Format(@"{0}\", identifier.guid);
                TreeNode[] nodes = tvEntries.Nodes.Find(path, true);
                if (nodes.Count() <= 0)
                    return;

                TreeNode matchedNode = nodes[0];

                // see if this node is checked
                if (!matchedNode.Checked)
                    continue;

                // checked node, delete the node's chapter in db
                if (!entryMethods.deleteChapterEntry(ctx, identifier, true))
                    continue;

                deletedNodes.Add(matchedNode);
                ctx.totalEntries--;
            }

            // finally remove all deleted nodes
            foreach(TreeNode node in deletedNodes)
                tvEntries.Nodes.Remove(node);

            // update ui
            tsslblTotalEntries.Text = ctx.totalEntries.ToString();
            //resetRtb(rtbEntry, true, true);
        }

        private void tsbuttonRefreshTrashcan_Click(object sender, EventArgs e)
        {
            if (!ctx.isDBOpen())
                return;

            bgWorkerRefreshTrashCan.RunWorkerAsync();
        }

        private void bgWorkerRefreshTrashCan_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!ctx.isDBOpen())
                return;

            refreshTrashcan(ctx);
        }

        public void refreshTrashcan(myContext ctx)
        {
            this.Invoke(toggleForm, false);
            this.Invoke(resetTrashCan);
            this.Invoke(LvTrashCanUpdate, true);
            foreach (Chapter chapter in myDB.findDeletedMarkedChapters(ctx))
                this.Invoke(insertLvTrashCanItem, chapter);

            this.Invoke(LvTrashCanUpdate, false);
            this.Invoke(toggleForm, true);
        }

        public void __resetTrashCan()
        {
            lvTrashCan.Items.Clear();   
        }

        public void __insertLvTrashCanItem(Chapter chapter)
        {
            ListViewItem item = new ListViewItem();
            item.Tag = chapter;
            String chapterDateTime = chapter.chapterDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            item.Text = chapterDateTime;
            item.SubItems.Add(chapter.Title);
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
            if (!ctx.isDBOpen())
                return;

            foreach (ListViewItem listViewItem in lvTrashCan.CheckedItems)
            {
                Chapter chapter = listViewItem.Tag as Chapter;
                myDB.pureDBChapterRecursive(ctx, chapter.guid);
            }

            // refresh trash can after deleting
            refreshTrashcan(ctx);
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
            if (!ctx.isDBOpen())
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
            if (!ctx.isDBOpen())
                return;

            foreach (ListViewItem listViewItem in lvTrashCan.CheckedItems)
            {
                Chapter chapter = listViewItem.Tag as Chapter;
                myDB.markDBChapterDeletedRecursive(ctx, chapter.guid, false);
            }

            // refresh trash can after deleting
            refreshTrashcan(ctx);

        }

        private void tsButtonRestoreDeletedEntry_Click(object sender, EventArgs e)
        {
            restoreTrashCan();
        }

        private void bgWorkerRebuildDB_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!ctx.isDBOpen())
                return;

            this.Invoke(toggleForm, false);
            myDB.rebuildDB(ctx);
            this.Invoke(toggleForm, true);

        }

        private void rebuildDbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ctx.isDBOpen())
                return;

            bgWorkerRebuildDB.RunWorkerAsync();    
        }

        private void toolStripMenuItem22_Click(object sender, EventArgs e)
        {
            doNewChildEntry();
        }

        public void doNewChildEntry()
        {
            if (!ctx.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            Chapter? parent = tvEntries.SelectedNode.Tag as Chapter;
            setupNewChildEntry(parent);
        }

        public void setupNewChildEntry(Chapter parent)
        {
            if (!ctx.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            // create new chapter direct
            Chapter? chapter = myDB.newChapter(ctx, ref ctx.totalEntries, DateTime.Now, true, parent);
            if (chapter == null)
                return;

            // finally update tree view because entry was imported successfully
            TreeNode newNode = initTreeViewChildEntry(chapter);
            tvEntries.SelectedNode = newNode;

            // update
            ctx.totalEntries++;
            tsslblTotalEntries.Text = ctx.totalEntries.ToString();

            __setCalendarHighlightEntry(chapter.chapterDateTime);
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
                var currentNode = queue.Dequeue();
                TreeNodeCollection children = currentNode.Nodes;// Directory.GetDirectories(currentPath);

                foreach (TreeNode childNode in children)
                {
                    queue.Enqueue(childNode);
                    childNode.Checked = set;
                }

                //paths.AddRange(Directory.GetFiles(currentPath, searchPattern).ToList());
            }
        }

        private void tsbuttonRestoreTrashCan_Click(object sender, EventArgs e)
        {
            restoreAllTrashcan();
        }

        public void restoreAllTrashcan()
        {
            if (!ctx.isDBOpen())
                return;

            checkEntireTrashCan(true);
            restoreTrashCan();
        }

        private void highlightCheckedEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highlightCheckedEntries();
        }

        public void highlightEntry(TreeNode node)
        {
            if (!ctx.isDBOpen())
                return;

            FontDialog fontDialog = new FontDialog();
            fontDialog.ShowColor = true; //color pnael
            fontDialog.ShowApply = true; //show apply button
            fontDialog.ShowEffects = true; //after appling, label style will chance
            fontDialog.Font = node.NodeFont;
            fontDialog.Color = node.ForeColor;

            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;

            Chapter? identifier = node.Tag as Chapter;
            if (identifier == null)
                return;

            node.ForeColor = fontDialog.Color;
            node.NodeFont = fontDialog.Font;
            entryMethods.setEntryHighlightFont(ctx, identifier, node.ForeColor, node.NodeFont);
        }

        public void highlightCheckedEntries()
        {
            if (!ctx.isDBOpen())
                return;

            if (tvEntries.Nodes.Count <= 0)
                return;

            FontDialog fontDialog = new FontDialog();
            fontDialog.ShowColor = true; //color pnael
            fontDialog.ShowApply = true; //show apply button
            fontDialog.ShowEffects = true; //after appling, label style will chance

            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;

            foreach (TreeNode node in tvEntries.Nodes)
                highlightTreeViewNodeRecursively(node, fontDialog.Font, fontDialog.Color);

        }

        public void highlightTreeViewNodeRecursively(TreeNode parentNode, Font font, Color color)
        {
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(parentNode);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                TreeNodeCollection children = currentNode.Nodes;// Directory.GetDirectories(currentPath);

                foreach (TreeNode childNode in children)
                {
                    queue.Enqueue(childNode);

                    if (childNode.Checked)
                    {
                        Chapter? identifier = childNode.Tag as Chapter;
                        if (identifier == null)
                            continue;

                        childNode.ForeColor = color;
                        childNode.NodeFont = font;
                        entryMethods.setEntryHighlightFont(ctx, identifier, childNode.ForeColor, childNode.NodeFont);
                    }
                }
            }
        }

        private void highlightSelectedEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ctx.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            highlightEntry(tvEntries.SelectedNode);
        }

        private void highlightBackColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ctx.isDBOpen())
                return;

            if (tvEntries.SelectedNode == null)
                return;

            highlightEntryBackColor(tvEntries.SelectedNode);
        }

        public void highlightEntryBackColor(TreeNode node)
        {
            if (!ctx.isDBOpen())
                return;

            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = node.BackColor;
            
            if (colorDialog.ShowDialog() != DialogResult.OK)
                return;

            Chapter? identifier = node.Tag as Chapter;
            if (identifier == null)
                return;

            node.BackColor = colorDialog.Color;
            entryMethods.setEntryHighlightBackColor(ctx, identifier, node.BackColor);

        }

        private void tsbuttonSearch_Click(object sender, EventArgs e)
        {
            if (!ctx.isDBOpen())
                return;

            if (rtbSearchPattern.TextLength <= 0)
                return;

            bgWorkerSearch.RunWorkerAsync();
        }

        private void bgWorkerSearch_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!ctx.isDBOpen())
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
            return journalSearchFramework.searchEntries(ctx, lvSearch, tsSearchProgressBar, dtpickerSearchFrom.Value, dtpickerSearchThrough.Value,
                chkSearchUseDateRange.Checked, rtbSearchPattern.Text, rtbSearchReplace.Text, chkSearchAll.Checked,
                chkSearchTrashCan.Checked, chkSearchMatchCase.Checked, chkSearchMatchWholeWord.Checked,
                chkSearchReplace.Checked, chkSearchMultiline.Checked, chkSearchExplicitCaptures.Checked);
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
        }

        public void restoreSearchedList()
        {
            if (!ctx.isDBOpen())
                return;

            foreach (ListViewItem listViewItem in lvSearch.CheckedItems)
            {
                Guid entryGuid = (Guid)listViewItem.Tag;
                if (entryGuid == null)
                    continue;

                Chapter? chapter = myDB.findDbChapterByGuid(ctx, entryGuid);
                if (chapter == null)
                    continue;

                if (chapter.IsDeleted)
                    myDB.markDBChapterDeletedRecursive(ctx, chapter.guid, false);

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
            if (!ctx.isDBOpen())
                return;

            checkAllSearchedItems(true);
            restoreSearchedList();
        }

        public void deleteSearchedList()
        {
            if (!ctx.isDBOpen())
                return;

            foreach (ListViewItem listViewItem in lvSearch.CheckedItems)
            {
                Guid entryGuid = (Guid)listViewItem.Tag;
                if (entryGuid == null)
                    continue;

                Chapter? chapter = myDB.findDbChapterByGuid(ctx, entryGuid);
                if (chapter == null)
                    continue;

                if (!chapter.IsDeleted)
                {
                    // checked node, delete the node's chapter in db
                    if (!entryMethods.deleteChapterEntry(ctx, chapter, true))
                        continue;
                }

                // find entry in treeview by identifier
                String path = String.Format(@"{0}\", chapter.guid);
                TreeNode[] nodes = tvEntries.Nodes.Find(path, true);
                if (nodes.Count() == 1)
                {
                    TreeNode matchedNode = nodes[0];
                    tvEntries.Nodes.Remove(matchedNode);
                }

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
            if (!ctx.isDBOpen())
                return;

            findAndReplace();
        }

        public void findAndReplace()
        {
            if (!ctx.isDBOpen())
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
            saveCloseDB();
            Application.Exit();
        }

        public void saveCloseDB()
        {
            saveEntry();
            closeContext();
        }
    }

}