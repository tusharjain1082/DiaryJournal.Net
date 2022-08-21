using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DiaryJournal.Net

{
    public static class journalSearchFramework
    {
        public static void __insertLvSearchItem(ListView lv, Chapter? chapter, long totalMatches)
        {
            if (chapter == null)
                return;

            ListViewItem item = new ListViewItem();
            item.Tag = chapter.guid;
            String chapterDateTime = chapter.chapterDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            item.Text = chapterDateTime;
            if (chapter.IsDeleted)
                item.SubItems.Add("trash can entry");
            else
                item.SubItems.Add("common valid entry");

            item.SubItems.Add(totalMatches.ToString());
            item.SubItems.Add(chapter.Title);
            lv.Items.Add(item);
        }

        public static bool searchEntries(myConfig cfg, ListView lv, ToolStripProgressBar tsProgressBar,
            DateTime inputFrom, DateTime inputFromTime, DateTime inputThrough, DateTime inputThroughTime, bool useDateTimeRange,
            String searchPattern, String replacement, bool searchAll,
            bool searchTrash, bool matchCase, bool matchWholeWord,
            bool replace, bool multiline, bool explicitCaptures,
            bool searchReplaceTitle)
        {
            // prepare regex
            RegexOptions regexOptions = new RegexOptions();
            if (!matchCase)
                regexOptions |= RegexOptions.IgnoreCase;

            if (explicitCaptures)
                regexOptions |= RegexOptions.ExplicitCapture;

            if (multiline)
                regexOptions |= RegexOptions.Multiline;
            else
                regexOptions |= RegexOptions.Singleline;

            // prepare to match whole word if user requires. but we can use it through manual pattern as well.
            if (matchWholeWord)
                searchPattern = String.Format(@"\b{0}\b", searchPattern);

            // prepare date and time range
            DateTime from = DateTime.MinValue;
            DateTime through = DateTime.MaxValue;
            //DateTime fromTime = DateTime.MinValue;
            //DateTime throughTime = DateTime.MaxValue;

            // use user's date and time range if required
            if (useDateTimeRange)
            {
                // choose user's given date time range if available
                if (inputFrom != default(DateTime))
                {
                    from = new DateTime(inputFrom.Year, inputFrom.Month, inputFrom.Day, inputFromTime.Hour, inputFromTime.Minute,
                        inputFromTime.Second, 0);
                    //fromTime = new DateTime(inputFrom.Year, inputFrom.Month, inputFrom.Day, inputFromTime.Hour, inputFromTime.Minute,
                    //  inputFromTime.Second, 0);
                }
                // choose user's given date time range if available
                if (inputThrough != default(DateTime))
                {
                    through = new DateTime(inputThrough.Year, inputThrough.Month, inputThrough.Day, inputThroughTime.Hour,
                        inputThroughTime.Minute, inputThroughTime.Second, 0);
                    //throughTime = new DateTime(inputThrough.Year, inputThrough.Month, inputThrough.Day, inputThroughTime.Hour,
                    //    inputThroughTime.Minute, inputThroughTime.Second, 0);
                }
                int result0 = DateTime.Compare(from, through);
                if (result0 > 0)
                    return false; // error, invalid date time input

            }

            // successfully prepared all config
            // verify if the pattern is valid.
            if (!commonMethods.IsValidRegex(searchPattern))
                return false;

            // now load regex with pattern and options
            Regex regex = new Regex(searchPattern, regexOptions);

            // now process all records
            // first get the total number of chapters which exist in db
            long nodesCount = 0;
            if (cfg.radCfgUseOpenFileSystemDB)
                nodesCount = OpenFileSystemDB.NodesCount(cfg.ctx1);
            else
                nodesCount = SingleFileDB.NodesCount(cfg.ctx0);

            if (nodesCount <= 0)
                return false;

            AdvRichTextBox rtb = new AdvRichTextBox();
            rtb.WordWrap = false;
            rtb.Multiline = true;
            rtb.SuspendLayout();
            rtb.BeginUpdate();
            List<myNode> nodes = null;
            
            if (cfg.radCfgUseOpenFileSystemDB)
                nodes = OpenFileSystemDB.findAllNodes(cfg.ctx1, true, false);
            else
                nodes = SingleFileDB.findAllNodes(cfg.ctx0, true, false);

            long nodesDone = 0;

            foreach (myNode listNode in nodes)
            {
                myNode node = listNode;
                String? rtf = "";

                if (cfg.radCfgUseOpenFileSystemDB)
                    rtf = OpenFileSystemDB.loadNodeData(cfg.ctx1, node.chapter.guid);
                else
                    rtf = SingleFileDB.loadNodeData(cfg.ctx0, node.chapter.guid);

                nodesDone++;
                tsProgressBar.Value = (int)Math.Round((double)(100 * nodesDone) / nodesCount);

                if (node == null)
                    continue; // some invalid file, skip

                if (rtf == null)
                    continue;

                if (rtf.Length <= 0)
                    continue;

                // if this chapter is deleted but user doesn't wants to get deleted entries, so skip this chapter.
                if (node.chapter.IsDeleted && !searchTrash)
                    continue;

                // if both options are off, so quit
                if (!searchAll && !searchTrash)
                    return false;

                rtb.Rtf = rtf;
                rtb.Select(0, 0);
                rtb.ScrollToCaret();

                if (searchAll)
                {
                    // if this chapter is deleted but user doesn't wants to get deleted entries, so skip this chapter.
                    if (node.chapter.IsDeleted && !searchTrash)
                        continue;

                }
                else if (searchTrash)
                {
                    if (!node.chapter.IsDeleted)
                        continue; // user unchecked search all entries but deleted, this entry isn't deleted, so skip it.

                }
                else
                {
                    // no option chosen, return empty list
                    rtb.Clear();
                    rtb.Dispose();
                    GC.Collect();
                    return false;
                }

                // date and time range check
                DateTime chapterDate = new DateTime(node.chapter.chapterDateTime.Year, node.chapter.chapterDateTime.Month,
                    node.chapter.chapterDateTime.Day, node.chapter.chapterDateTime.Hour,
                    node.chapter.chapterDateTime.Minute, node.chapter.chapterDateTime.Second, 0);
                int result1 = DateTime.Compare(chapterDate, from);
                int result2 = DateTime.Compare(chapterDate, through);
                if (result1 < 0)
                {
                    continue; // date mismatch
                }

                if (result2 > 0)
                {
                    continue; // date mismatch
                }

                bool matchesFound = false;
                long totalMatches = 0;

                // both node's title and node's body must not be searched and replaced simultaneusly.
                // this is illogical because it confuses the user and also mistakenly
                // replaces the title when it must not be replaced without user's special requirement.
                // therefore node's title and node's body shall be separately searched and replaced.
                if (!searchReplaceTitle)
                {
                    // node's body

                    // finally search the entry with regex
                    MatchCollection matches0 = regex.Matches(rtb.Text);//decodedRtf);
                    if (matches0.LongCount() > 0)
                    {
                        // finally commit replace if user requires right here
                        if (replace)
                        {
                            // we cannot modify rtf. unicode characters are stored as unicode char opcodes in rtf, so it's impossible to replace them with string.
                            // so we use RichTextBox itself.
                            for (int i = 0; i < matches0.Count; i++)
                            {
                                Match match = matches0[i];
                                //AdvRichTextBox.ReplaceSelectedUnicodeText(rtb, match.Value, match.Index, match.Length);
                                bool result = rtb.FindReplaceUnicodeText(rtb, match.Value, 0, -1, replacement);//rtb.FindUnicodeText(rtb, match.Value, 0, rtb.TextLength);
                                if (!result)
                                    continue;
                            }
                        }
                        matchesFound = true;
                        totalMatches += matches0.LongCount();
                    }
                }

                if (searchReplaceTitle)
                {
                    // node's title

                    // check and update node's title
                    MatchCollection matches1 = regex.Matches(node.chapter.Title);
                    if (matches1.LongCount() > 0)
                    {
                        if (replace)
                            node.chapter.Title = regex.Replace(node.chapter.Title, replacement);

                        matchesFound = true;
                        totalMatches += matches1.LongCount();
                    }
                }

                // finally update node in db
                if (matchesFound)
                {
                    if (cfg.radCfgUseOpenFileSystemDB)
                        OpenFileSystemDB.updateNode(cfg.ctx1, ref node, rtb.Rtf, true);
                    else
                        SingleFileDB.updateNode(cfg.ctx0, ref node, rtb.Rtf, true);

                    __insertLvSearchItem(lv, node.chapter, totalMatches);
                }
            }
            tsProgressBar.Value = 0;
            rtb.EndUpdate();
            rtb.ResumeLayout();
            rtb.Clear();
            rtb.Dispose();
            GC.Collect();
            return true;
        }

    }
}
