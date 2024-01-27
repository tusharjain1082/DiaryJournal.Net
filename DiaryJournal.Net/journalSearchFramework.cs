using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Xml.Xsl;
using System.Xml;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Drawing;
using static System.Windows.Forms.Design.AxImporter;

namespace DiaryJournal.Net

{
    public static class journalSearchFramework
    {
        public static void __insertLvSearchItem(System.Windows.Forms.ListView lv, ref List<myNode> allNodes, ref myNode node, long totalMatches)
        {
            if (node == null)
                return;

            System.Windows.Forms.ListViewItem item = new System.Windows.Forms.ListViewItem();
            item.Name = node.chapter.Id.ToString();
            item.Text = totalMatches.ToString();

            String entryPath = "";
            List<myNode>? lineage = null;
            if (entryMethods.GenerateLineagePath(ref node.lineage, ref node, out entryPath, out lineage))
                item.SubItems.Add(entryPath);
            else
                item.SubItems.Add("[path not found]");

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
            lv.Items.Add(item);
        }

        /// <summary>
        /// Find the corresponding<see cref="TextRange"/> instance
        /// representing the input string given a specified text pointer position.
        /// </summary>
        /// <param name="position">the current text position</param>
        /// <param name="textToFind">input text</param>
        /// <param name="findOptions">the search option</param>
        /// <returns>An<see cref="TextRange"/> instance represeneting the matching string withing the text container.</returns>
        /// 
        public static bool searchEntries(myConfig cfg, ref List<myNode> allNodes,
            FrmJournal form, System.Windows.Forms.TextBox txtSearchProgressPath, System.Windows.Forms.ListView lv, ToolStripProgressBar tsProgressBar,
            DateTime inputFrom, DateTime inputFromTime, DateTime inputThrough, DateTime inputThroughTime, bool useDateTimeRange,
            DateTime inputCDFrom, DateTime inputCDFromTime, DateTime inputCDThrough, DateTime inputCDThroughTime, bool useCreationDateTimeRange,
            DateTime inputMDFrom, DateTime inputMDFromTime, DateTime inputMDThrough, DateTime inputMDThroughTime, bool useModificationDateTimeRange,
            DateTime inputDDFrom, DateTime inputDDFromTime, DateTime inputDDThrough, DateTime inputDDThroughTime, bool useDeletionDateTimeRange,
            String searchPattern, String replacement, bool searchAll,
            bool searchTrash, bool matchCase, bool matchWholeWord,
            bool replace, bool searchReplaceTitle, bool searchEmptyString, ref List<myNode> locations)
        {
            // prepare regex
            RegexOptions regexOptions = new RegexOptions();
            if (!matchCase)
                regexOptions |= RegexOptions.IgnoreCase;

            regexOptions |= RegexOptions.Singleline;

            // prepare to match whole word if user requires. but we can use it through manual pattern as well.
            if (matchWholeWord)
            {
                String flattened = Regex.Escape(searchPattern);
                searchPattern = @"\b" + flattened + @"";//[^.*\r*\n*]";
            }

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

            WpfRichTextBoxEx rtb = new WpfRichTextBoxEx();
            //System.Windows.Forms.RichTextBox rtbnative = new System.Windows.Forms.RichTextBox();

            // create work list
            List<myNode> worklist = allNodes;

            // select search location if user demands it
            if (locations.Count > 0) 
            {
                // find all children of this node recursively and create their list
                worklist = entryMethods.FindSelectedNodesAllChildrenRecursiveInList(ref allNodes, ref locations, true, false, true);
            }

            long nodesCount = worklist.LongCount();
            if (nodesCount <= 0) return false;
            long nodesDone = 0;

            // initialize a single regex for all operations
            // now load regex with pattern and options
            Regex regex = new Regex(searchPattern, regexOptions);

            foreach (myNode listNode in worklist)
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

                // show full path of current node
                String entryPath = "";
                List<myNode>? lineage = null;
                if (entryMethods.GenerateLineagePath(ref node.lineage, ref node, out entryPath, out lineage))
                {
                    form.Invoke(form.updateSearchProgressPath, entryPath);
                    
                }
                // user demands empty string meaning he demands to find entries only by their configuration parameters
                // like creation date modification date and so and so.
                if (searchEmptyString)
                {
                    // no text to find and therefore nothing to replace. so directly list this entry/node because other 
                    // search parameters are valid at this place.
                    __insertLvSearchItem(lv, ref allNodes, ref node, totalMatches);
                    continue; // bypass regex text search and replace.
                }

                // configure richtextbox
                //rtbnative.Rtf = rtf;
                try
                {
                    rtb.Rtf = rtf;// rtbnative.Rtf;
                }
                catch (Exception)
                {
                    continue;
                }

                // both node's title and node's body must not be searched and replaced simultaneusly.
                // this is illogical because it confuses the user and also mistakenly
                // replaces the title when it must not be replaced without user's special requirement.
                // therefore node's title and node's body shall be separately searched and replaced.
                if (!searchReplaceTitle)
                {
                    // node's body

                    // initialize search and generate collection
                    FindReplaceFramework.MatchedTextCollection col = FindReplaceFramework.MatchedTextCollection.initializeSearch(ref regexOptions,
                        rtb.Document, searchPattern);

                    // replace if demanded
                    if (col.Count > 0)
                    {
                        // update
                        matchesFound = true;
                        totalMatches += col.Count;

                        // finally commit replace if user requires right here
                        if (replace)
                        {
                            // begin with first or another node
                            col.Next(true);
                            while (col.current != null) 
                            {
                                // set selection and replace
                                TextRange selection = new TextRange(col.current.start, col.current.end);
                                selection.Text = replacement;
                                col.Remove(ref col.current);
                                col.Next(true); // to the next node
                            }
                        }
                    }
                }

                if (searchReplaceTitle)
                {
                    // node's title

                    // check and update node's title
                    MatchCollection matches1 = regex.Matches(node.chapter.Title);
                    if (matches1.Count > 0)
                    {
                        if (replace)
                            node.chapter.Title = regex.Replace(node.chapter.Title, replacement);

                        matchesFound = true;
                        totalMatches += matches1.Count;
                    }
                }

                // finally update node in db
                if (matchesFound)
                {
                    if (replace)
                        entryMethods.DBUpdateNode(cfg, ref node, rtb.Rtf, true);
                    

                    __insertLvSearchItem(lv, ref allNodes, ref node, totalMatches);
                }
            }
            tsProgressBar.Value = 0;
            //txtSearchProgressPath.Text = "";
            GC.Collect();
            return true;
        }
    }
}
