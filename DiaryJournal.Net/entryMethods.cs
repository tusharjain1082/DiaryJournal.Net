﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DiaryJournal.Net
{
    public static class entryMethods
    {
        public static String getFormattedJournalPathFileName(String path, Guid guid, Guid parentGuid, String title,
            DateTime dateTime, long exportIndex, EntryType entryType, out String entryNameOut)
        {
            String entryName = getFormattedJournalFileName(guid, parentGuid, title, dateTime, exportIndex, entryType, out entryNameOut);
            String file = Path.Combine(path, Path.GetFileName(entryName));
            file = @"\\?\" + file;
            return file;
        }
        public static String getFormattedJournalFileName(Guid guid, Guid parentGuid, String title,
            DateTime dateTime, long exportIndex, EntryType entryType, out String entryNameOut)
        {
            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            myDB.getEntryTypeFormats(entryType, ref ext, ref extComplete, ref extSearchPattern);

            // common chapter entry. we use proper journal format
            String modifiedTitle = ((title != "") ? title.Replace("--", "-") : "");
            String entryName = String.Format("{0}--{1}--{2}--{3}--{4}--.{5}", exportIndex, dateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff"),
                guid, parentGuid, modifiedTitle, ext);
            entryNameOut = String.Format("{0}--{1}--{2}--{3}--{4}--", exportIndex, dateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff"),
                guid, parentGuid, modifiedTitle);
            return entryName;
        }
        public static Chapter? importEntry(myContext ctx, String path, EntryType entryType, DateTime customDateTime = default(DateTime),
            bool freeStandingEntry = false, bool createNew = false)
        {
            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            myDB.getEntryTypeFormats(entryType, ref ext, ref extComplete, ref extSearchPattern);

            Chapter chapter = new Chapter();
            String rtf = "";

            // entry's identification in the application and in the database.
            chapter.guid = Guid.NewGuid();

            // load entry file
            if (!entryMethods.loadEntryFile(ref chapter, path, ref rtf, entryType, freeStandingEntry))
                return null;

            // now check if this entry doesn't already exists.
            if (!createNew)
            {
                Chapter? existingChapter = myDB.findDbChapterByGuid(ctx, chapter.guid);
                if (existingChapter != null)
                    return null;
            }

            // create an absolutely new entry
            if (createNew)
            {
                chapter.guid = Guid.NewGuid();
            }

            // if this is a free standing entry, we null out the parent attributes and give a new guid to it.
            if (freeStandingEntry)
            {
                chapter.guid = Guid.NewGuid();
            }

            if (chapter.chapterDateTime == default(DateTime))
                chapter.chapterDateTime = DateTime.Now;

            if (customDateTime != default(DateTime))
                chapter.chapterDateTime = customDateTime;

            // 1st import the chapter's data blob
            ChapterData chapterData = myDB.newChapterData(chapter.guid, rtf);
            if (!myDB.importNewDBChapterData(ctx, ref chapterData))
                return null;

            // 2nd now import the entry as a chapter into db
            if (!myDB.importNewDBChapter(ctx, ref chapter))
                return null;

            return chapter;
        }

        public static bool loadEntryFile(ref Chapter chapter, String file, ref String rtf, EntryType entryType,
            bool freeStandingEntry = false)
        {
            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            myDB.getEntryTypeFormats(entryType, ref ext, ref extComplete, ref extSearchPattern);

            // load chapter from the file
            switch (entryType)
            {
                case EntryType.Xml:
                    if (!xmlEntry.fromXml(ref chapter, file, ref rtf))
                        return false;

                    break;

                case EntryType.Rtf:
                    if (!rtfEntry.fromRtf(ref chapter, file, ref rtf, extComplete, freeStandingEntry))
                        return false;

                    break;

                case EntryType.Html:
                    break;

                default:
                    break;
            }

            // now load the xml entry config file if it is not an xml entry.
            if (entryType != EntryType.Xml)
            {
                String output = "";
                String configFile = String.Format(@"{0}.{1}", file, "xml");
                xmlEntry.fromXml(ref chapter, configFile, ref output);
            }


            return true;
        }

        public static bool validateExtractEntryFile(String file, ref long index, ref DateTime chapterDate,
            ref String title, ref Guid chapterGuid, ref Guid parentGuid)
        {
            if (file == "")
                return false;

            FileInfo fileInfo = new FileInfo(file);
            String filename = fileInfo.Name;

            String validationPattern = @"[0-9]*--\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d-\d\d\d--";
            Regex regex0 = new Regex(validationPattern, RegexOptions.IgnoreCase);
            MatchCollection matches0 = regex0.Matches(filename);

            String entryDateTimePattern = @"\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d-\d\d\d";
            Regex regex1 = new Regex(entryDateTimePattern, RegexOptions.IgnoreCase);
            MatchCollection matches1 = regex1.Matches(filename);

            String indexPattern = @"([0-9]*)--(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d-\d\d\d)--";
            Regex regex2 = new Regex(indexPattern, RegexOptions.IgnoreCase);
            MatchCollection matches2 = regex2.Matches(filename);

            //String completePattern = @"([0-9]*)(--)(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d-\d\d\d)(--)([a-zA-Z].*)(--)([a-zA-Z].*)(--)(.*)(--)(\.[a-zA-Z].*)";
            String completePattern = @"([0-9]*)(--)(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d-\d\d\d)(--)(.*)(--)(.*)(--)(.*)(--)(\..*)";
            Regex regex3 = new Regex(completePattern, RegexOptions.IgnoreCase);
            MatchCollection matches3 = regex3.Matches(filename);

            if (matches0.Count <= 0)
                return false; // error not this application's generated file.

            if (matches1.Count <= 0)
                return false; // error not valid date and time

            if (matches2.Count <= 0)
                return false; // error not valid date and time

            if (matches3.Count <= 0)
                return false; // error not valid date and time

            if (!long.TryParse(matches2[0].Groups[1].Value, out index))
                return false;

            try
            {
                chapterDate = DateTime.ParseExact(matches3[0].Groups[3].Value, "yyyy-MM-dd-HH-mm-ss-fff",
                                  System.Globalization.CultureInfo.InvariantCulture);
                chapterGuid = Guid.Parse(matches3[0].Groups[5].Value);
                parentGuid = Guid.Parse(matches3[0].Groups[7].Value);
                title = matches3[0].Groups[9].Value;
            }
            catch (FormatException)
            {
                return false;
            }


            return true;
        }
        public static bool convertEntryFilenameToChapter(ref Chapter chapter, String file)
        {
            DateTime chapterDate = DateTime.Now;
            String title = "";
            long index = -1;
            Guid parentGuid = Guid.Empty;
            Guid chapterGuid = Guid.Empty;

            if (!validateExtractEntryFile(file, ref index, ref chapterDate, ref title, ref chapterGuid, ref parentGuid))
                return false;

            chapter.chapterDateTime = chapterDate;
            chapter.Title = title;
            chapter.guid = chapterGuid;
            chapter.parentGuid = parentGuid;
            return true;
        }

        public static String findEntryFileByExportIndex(IEnumerable<String> files, long index)
        {
            DateTime chapterDate = DateTime.Now;
            String title = "";
            long foundIndex = -1;
            Guid parentGuid = Guid.Empty;
            Guid chapterGuid = Guid.Empty;

            foreach (String file in files)
            {
                if (!validateExtractEntryFile(file, ref foundIndex, ref chapterDate, ref title, ref chapterGuid, ref parentGuid))
                    continue;

                if (index == foundIndex)
                    return file; // found a matching file
            }
            // exception, no file found
            return "";
        }
        public static bool deleteChapterEntry(myContext ctx, Chapter? identifier, bool mark)
        {
            if (identifier == null)
                return false;

            if (!ctx.isDBOpen())
                return false;

            // finally delete the entry
            return myDB.markDBChapterDeletedRecursive(ctx, identifier.guid, true);
        }

        public static bool setEntryHighlightFont(myContext ctx, Chapter identifier, Color highlightFontColor, Font highlightFont)
        {
            identifier.HLFont = commonMethods.FontToString(highlightFont);
            identifier.HLFontColor = commonMethods.ColorToString(highlightFontColor);
            return myDB.updateChapterHLFontByGuid(ctx, identifier);
        }

        public static bool setEntryHighlightBackColor(myContext ctx, Chapter identifier, Color highlightBackColor)
        {
            identifier.HLBackColor = commonMethods.ColorToString(highlightBackColor);
            return myDB.updateChapterHLBackColorByGuid(ctx, identifier);
        }

        public static bool initCalenderNodes(myContext ctx, int year, int month, out myNode? yearNodeOut, out myNode? monthNodeOut)
        {
            // now create Year Node direct in DB if it doesn't exists. else load it from db for linking
            ChapterData? yearChapterData = null;
            List<myNode> yearNodes = myDB.findNodesByNodeType(ctx, NodeType.YearNode, year, -1);
            myNode? yearNode = null;
            if (yearNodes.Count() <= 0)
            {
                // this year doesn't exists in db, so create it.
                DateTime yearDateTime = new DateTime(year, 1, 1, 0, 0, 0, 0);
                yearNode = myDB.newNode(ctx, year, -1, NodeType.YearNode, ref yearChapterData, yearDateTime, null, true);
            }
            else
            {
                // this year node already exists, so directly load it to the object.
                yearNode = yearNodes[0];
            }

            // now create Month Node direct in DB if it doesn't exists. else load it from db for linking.
            // note Month node's parent must be the year node.
            ChapterData? monthChapterData = null;
            List<myNode> monthNodes = myDB.findNodesByNodeType(ctx, NodeType.MonthNode, year, month);
            myNode? monthNode = null;
            if (monthNodes.Count() <= 0)
            {
                // this month in the current chapter's year doesn't exists in db, so create it.
                DateTime monthDateTime = new DateTime(year, month, 1, 0, 0, 0, 0);
                monthNode = myDB.newNode(ctx, year, month, NodeType.MonthNode, ref monthChapterData, monthDateTime, yearNode.chapter, true);
            }
            else
            {
                // this month node in the current chapter's year already exists, so directly load it to the object.
                monthNode = monthNodes[0];
            }

            // output
            yearNodeOut = yearNode;
            monthNodeOut = monthNode;
            return true;
        }

    }
}
