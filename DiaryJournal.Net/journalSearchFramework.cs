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
        public static void __insertLvSearchItem(ListView lv, ref myNode node, long totalMatches)
        {
            if (node == null)
                return;

            ListViewItem item = new ListViewItem();
            item.Name = node.chapter.Id.ToString();
            String chapterDateTime = node.chapter.chapterDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            item.Text = chapterDateTime;

            // dates
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
            item.SubItems.Add(totalMatches.ToString());
            item.SubItems.Add(node.chapter.Title);
            lv.Items.Add(item);
        }

        public static bool searchEntries(myConfig cfg, ref List<myNode> allNodes, ListView lv, ToolStripProgressBar tsProgressBar,
            DateTime inputFrom, DateTime inputFromTime, DateTime inputThrough, DateTime inputThroughTime, bool useDateTimeRange,
            DateTime inputCDFrom, DateTime inputCDFromTime, DateTime inputCDThrough, DateTime inputCDThroughTime, bool useCreationDateTimeRange,
            DateTime inputMDFrom, DateTime inputMDFromTime, DateTime inputMDThrough, DateTime inputMDThroughTime, bool useModificationDateTimeRange,
            DateTime inputDDFrom, DateTime inputDDFromTime, DateTime inputDDThrough, DateTime inputDDThroughTime, bool useDeletionDateTimeRange,
            String searchPattern, String replacement, bool searchAll,
            bool searchTrash, bool matchCase, bool matchWholeWord,
            bool replace, bool multiline, bool explicitCaptures,
            bool searchReplaceTitle, bool searchEmptyString)
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

            // prepare entry date and time range
            DateTime from = DateTime.MinValue;
            DateTime through = DateTime.MaxValue;

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

            // prepare entry creation date and time range
            DateTime CDfrom = DateTime.MinValue;
            DateTime CDthrough = DateTime.MaxValue;

            // use user's date and time range if required
            if (useCreationDateTimeRange)
            {
                // choose user's given creation date time range if available
                if (inputCDFrom != default(DateTime))
                {
                    CDfrom = new DateTime(inputCDFrom.Year, inputCDFrom.Month, inputCDFrom.Day, inputCDFromTime.Hour, inputCDFromTime.Minute,
                        inputCDFromTime.Second, 0);
                }
                // choose user's given creation date time range if available
                if (inputCDThrough != default(DateTime))
                {
                    CDthrough = new DateTime(inputCDThrough.Year, inputCDThrough.Month, inputCDThrough.Day, inputCDThroughTime.Hour,
                        inputCDThroughTime.Minute, inputCDThroughTime.Second, 0);
                }
                int result0 = DateTime.Compare(CDfrom, CDthrough);
                if (result0 > 0)
                    return false; // error, invalid date time input

            }

            // prepare entry modification date and time range
            DateTime MDfrom = DateTime.MinValue;
            DateTime MDthrough = DateTime.MaxValue;

            // use user's date and time range if required
            if (useModificationDateTimeRange)
            {
                // choose user's given modification date time range if available
                if (inputMDFrom != default(DateTime))
                {
                    MDfrom = new DateTime(inputMDFrom.Year, inputMDFrom.Month, inputMDFrom.Day, inputMDFromTime.Hour, inputMDFromTime.Minute,
                        inputMDFromTime.Second, 0);
                }
                // choose user's given modification date time range if available
                if (inputMDThrough != default(DateTime))
                {
                    MDthrough = new DateTime(inputMDThrough.Year, inputMDThrough.Month, inputMDThrough.Day, inputMDThroughTime.Hour,
                        inputMDThroughTime.Minute, inputMDThroughTime.Second, 0);
                }
                int result0 = DateTime.Compare(MDfrom, MDthrough);
                if (result0 > 0)
                    return false; // error, invalid date time input

            }

            // prepare entry deletion date and time range
            DateTime DDfrom = DateTime.MinValue;
            DateTime DDthrough = DateTime.MaxValue;

            // use user's date and time range if required
            if (useDeletionDateTimeRange)
            {
                // choose user's given deletion date time range if available
                if (inputDDFrom != default(DateTime))
                {
                    DDfrom = new DateTime(inputDDFrom.Year, inputDDFrom.Month, inputDDFrom.Day, inputDDFromTime.Hour, inputDDFromTime.Minute,
                        inputDDFromTime.Second, 0);
                }
                // choose user's given deletion date time range if available
                if (inputDDThrough != default(DateTime))
                {
                    DDthrough = new DateTime(inputDDThrough.Year, inputDDThrough.Month, inputDDThrough.Day, inputDDThroughTime.Hour,
                        inputDDThroughTime.Minute, inputDDThroughTime.Second, 0);
                }
                int result0 = DateTime.Compare(DDfrom, DDthrough);
                if (result0 > 0)
                    return false; // error, invalid date time input

            }

            // successfully prepared all config
            // verify if the pattern is valid.
            if (!searchEmptyString)
            {
                if (!commonMethods.IsValidRegex(searchPattern))
                    return false;
            }

            // now load regex with pattern and options
            Regex regex = new Regex(searchPattern, regexOptions);

            AdvRichTextBox rtb = new AdvRichTextBox();
            rtb.WordWrap = false;
            rtb.Multiline = true;
            rtb.SuspendLayout();
            rtb.BeginUpdate();

            long nodesCount = allNodes.LongCount();
            if (nodesCount <= 0)
                return false;

            long nodesDone = 0;

            foreach (myNode listNode in allNodes)
            {
                myNode node = listNode;
                String? rtf = "";
                if (!searchEmptyString)
                {
                    if (cfg.radCfgUseOpenFileSystemDB)
                        rtf = OpenFileSystemDB.loadNodeData(cfg.ctx1, node.DirectorySectionID, node.chapter.Id);
                    else
                        rtf = SingleFileDB.loadNodeData(cfg.ctx0, node.chapter.Id);
                }
                nodesDone++;
                tsProgressBar.Value = (int)Math.Round((double)(100 * nodesDone) / nodesCount);

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

                // entry date and time range check
                DateTime chapterDate = new DateTime(node.chapter.chapterDateTime.Year, node.chapter.chapterDateTime.Month,
                    node.chapter.chapterDateTime.Day, node.chapter.chapterDateTime.Hour,
                    node.chapter.chapterDateTime.Minute, node.chapter.chapterDateTime.Second, 0);
                int result1 = DateTime.Compare(chapterDate, from);
                int result2 = DateTime.Compare(chapterDate, through);
                if (result1 < 0)
                    continue; // date mismatch

                if (result2 > 0)
                    continue; // date mismatch

                // entry true creation date and time range check
                DateTime chapterCreationDate = new DateTime(node.chapter.creationDateTime.Year, node.chapter.creationDateTime.Month,
                    node.chapter.creationDateTime.Day, node.chapter.creationDateTime.Hour,
                    node.chapter.creationDateTime.Minute, node.chapter.creationDateTime.Second, 0);
                result1 = DateTime.Compare(chapterCreationDate, CDfrom);
                result2 = DateTime.Compare(chapterCreationDate, CDthrough);
                if (result1 < 0)
                    continue; // date mismatch

                if (result2 > 0)
                    continue; // date mismatch

                // entry modification date and time range check
                DateTime chapterModificationDate = new DateTime(node.chapter.modificationDateTime.Year, node.chapter.modificationDateTime.Month,
                    node.chapter.modificationDateTime.Day, node.chapter.modificationDateTime.Hour,
                    node.chapter.modificationDateTime.Minute, node.chapter.modificationDateTime.Second, 0);
                result1 = DateTime.Compare(chapterModificationDate, MDfrom);
                result2 = DateTime.Compare(chapterModificationDate, MDthrough);
                if (result1 < 0)
                    continue; // date mismatch

                if (result2 > 0)
                    continue; // date mismatch

                // entry deletion date and time range check
                DateTime chapterDeletionDate = new DateTime(node.chapter.deletionDateTime.Year, node.chapter.deletionDateTime.Month,
                    node.chapter.deletionDateTime.Day, node.chapter.deletionDateTime.Hour,
                    node.chapter.deletionDateTime.Minute, node.chapter.deletionDateTime.Second, 0);
                result1 = DateTime.Compare(chapterDeletionDate, DDfrom);
                result2 = DateTime.Compare(chapterDeletionDate, DDthrough);
                if (result1 < 0)
                    continue; // date mismatch

                if (result2 > 0)
                    continue; // date mismatch

                bool matchesFound = false;
                long totalMatches = 0;

                // user demands empty string meaning he demands to find entries only by their configuration parameters
                // like creation date modification date and so and so.
                if (searchEmptyString)
                {
                    // no text to find and therefore nothing to replace. so directly list this entry/node because other 
                    // search parameters are valid at this place.
                    __insertLvSearchItem(lv, ref node, totalMatches);
                    continue; // bypass regex text search and replace.
                }

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
                    if (replace)
                        entryMethods.DBUpdateNode(cfg, ref node, rtb.Rtf, true);
                    
                    __insertLvSearchItem(lv, ref node, totalMatches);
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
