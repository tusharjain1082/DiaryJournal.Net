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
        public static bool searchEntries(myContext ctx, ListView lv, ToolStripProgressBar tsProgressBar,
            DateTime inputFrom, DateTime inputThrough, bool useDateTimeRange,
            String searchPattern, String replacement, bool searchAll,
            bool searchTrash, bool matchCase, bool matchWholeWord,
            bool replace, bool multiline, bool explicitCaptures)
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

            // use user's date and time range if required
            if (useDateTimeRange)
            {
                // choose user's given date time range if available
                if (inputFrom != default(DateTime))
                    from = new DateTime(inputFrom.Year, inputFrom.Month, inputFrom.Day, 0, 0, 0);

                // choose user's given date time range if available
                if (inputThrough != default(DateTime))
                    through = new DateTime(inputThrough.Year, inputThrough.Month, inputThrough.Day, 0, 0, 0);

                int result0 = DateTime.Compare(from, through);
                if (result0 > 0)
                    return false; // error, invalid date time input
            }

            // successfully prepared all config
            // now load regex with pattern and options
            Regex regex = new Regex(searchPattern, regexOptions);

            // now process all records
            // first get the total number of chapters which exist in db
            long ChaptersCount = myDB.ChaptersCount(ctx);
            if (ChaptersCount <= 0)
                return false;

            AdvRichTextBox rtb = new AdvRichTextBox();
            rtb.WordWrap = false;
            rtb.Multiline = true;
            rtb.SuspendLayout();
            rtb.BeginUpdate();
            ctx.identifiers = myDB.FindAllChapters(ctx).ToList();
            long chaptersDone = 0;
            foreach (Chapter chapter in ctx.identifiers)
            {

                chaptersDone++;
                tsProgressBar.Value = (int)Math.Round((double)(100 * chaptersDone) / ChaptersCount);

                // if this chapter is deleted but user doesn't wants to get deleted entries, so skip this chapter.
                if (chapter.IsDeleted && !searchTrash)
                    continue;

                // if both options are off, so quit
                if (!searchAll && !searchTrash)
                    return false;

                // first load the chapter's data
                ChapterData? dbChapterData = myDB.loadDBChapterData(ctx, chapter.guid);
                if (dbChapterData == null)
                    continue; // error

                String decodedRtf = commonMethods.Base64Decode(dbChapterData.data);
                if (decodedRtf == null || decodedRtf.Length <= 0)
                    continue; // no data or 0 length string

                rtb.Rtf = decodedRtf;
                rtb.Select(0, 0);
                rtb.ScrollToCaret();

                if (searchAll)
                {
                    // if this chapter is deleted but user doesn't wants to get deleted entries, so skip this chapter.
                    if (chapter.IsDeleted && !searchTrash)
                        continue;

                }
                else if (searchTrash)
                {
                    if (!chapter.IsDeleted)
                        continue; // user unchecked search all entries but deleted, this entry isn't deleted, so skip it.

                }
                else
                {
                    // no option chosen, return empty list
                    rtb.Clear();
                    rtb.Dispose();
                    rtb = null;
                    GC.Collect();
                    return false;
                }

                // date and time range check
                DateTime chapterDate = new DateTime(chapter.chapterDateTime.Year, chapter.chapterDateTime.Month,
                    chapter.chapterDateTime.Day, 0, 0, 0, 0);
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
                // finally search the entry with regex
                MatchCollection matches = regex.Matches(rtb.Text);//decodedRtf);
                if (matches.LongCount() > 0)
                {
                    // finally commit replace if user requires right here
                    if (replace)
                    {
                        // we cannot modify rtf. unicode characters are stored as unicode char opcodes in rtf, so it's impossible to replace them with string.
                        // so we use RichTextBox itself.
                        for (int i = 0; i < matches.Count; i++)
                        {
                            Match match = matches[i];
                            //AdvRichTextBox.ReplaceSelectedUnicodeText(rtb, match.Value, match.Index, match.Length);
                            bool result = rtb.FindReplaceUnicodeText(rtb, match.Value, 0, -1, replacement);//rtb.FindUnicodeText(rtb, match.Value, 0, rtb.TextLength);
                            if (!result)
                                continue;

                            // working =
                            //rtb.SelectionStart = index;//matchIndex;
                            //rtb.SelectionLength = match.Value.Length;
                            //rtb.SelectedText = replacement;
                        }
                        bool result3 = myDB.updateChapterByGuid(ctx, chapter.guid, rtb.Rtf);
                        if (result3 == false)
                            continue;
                    }
                    __insertLvSearchItem(lv, chapter, matches.LongCount());
                }
            }
            tsProgressBar.Value = 0;
            rtb.EndUpdate();
            rtb.ResumeLayout();
            rtb.Clear();
            rtb.Dispose();
            rtb = null;
            GC.Collect();
            return true;
        }
    }
}
