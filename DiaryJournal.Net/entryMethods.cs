using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DiaryJournal.Net
{
    public static class entryMethods
    {
        public const String xmlExt = "xml";
        public const String rtfExt = "rtf";
        public const String htmlExt = "html";
        public const String txtExt = "txt";
        public const String xmlExtComplete = ".xml";
        public const String rtfExtComplete = ".rtf";
        public const String htmlExtComplete = ".html";
        public const String txtExtComplete = ".txt";
        public const String xmlExtSearchPattern = "*.xml";
        public const String rtfExtSearchPattern = "*.rtf";
        public const String htmlExtSearchPattern = "*.html";
        public const String txtExtSearchPattern = "*.txt";


        public static void getEntryTypeFormats(EntryType entryType, ref String extOut, ref String extCompleteOut, ref String extSearchPatternOut)
        {
            switch (entryType)
            {
                case EntryType.Xml:
                    extOut = xmlExt;
                    extCompleteOut = xmlExtComplete;
                    extSearchPatternOut = xmlExtSearchPattern;
                    break;
                case EntryType.Rtf:
                    extOut = rtfExt;
                    extCompleteOut = rtfExtComplete;
                    extSearchPatternOut = rtfExtSearchPattern;
                    break;
                case EntryType.Html:
                    extOut = htmlExt;
                    extCompleteOut = htmlExtComplete;
                    extSearchPatternOut = htmlExtSearchPattern;
                    break;
                case EntryType.Txt:
                    extOut = txtExt;
                    extCompleteOut = txtExtComplete;
                    extSearchPatternOut = txtExtSearchPattern;
                    break;
                default:
                    extOut = "";
                    extCompleteOut = "";
                    extSearchPatternOut = "";
                    break;
            }
        }

        public static String getFormattedPathFileName(String path, Guid guid, Guid parentGuid, String title,
            DateTime dateTime, long exportIndex, EntryType entryType, out String entryNameOut)
        {
            String entryName = getFormattedFileName(guid, parentGuid, title, dateTime, exportIndex, entryType, out entryNameOut);
            String file = Path.Combine(path, Path.GetFileName(entryName));
            //file = @"\\?\" + file;
            return file;
        }
        public static String getFormattedFileName(Guid guid, Guid parentGuid, String title,
            DateTime dateTime, long exportIndex, EntryType entryType, out String entryNameOut)
        {
            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            entryMethods.getEntryTypeFormats(entryType, ref ext, ref extComplete, ref extSearchPattern);

            // common chapter entry. we use proper journal format
            String modifiedTitle = ((title != "") ? title.Replace("--", "-") : "");
            String entryName = String.Format("{0}--{1}--{2}--{3}--{4}--.{5}", exportIndex, dateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff"),
                guid, parentGuid, modifiedTitle, ext);
            entryNameOut = String.Format("{0}--{1}--{2}--{3}--{4}--", exportIndex, dateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff"),
                guid, parentGuid, modifiedTitle);
            return entryName;
        }
        public static myNode? loadNode(String path, EntryType entryType, ref String rtfOut, 
            DateTime customDateTime = default(DateTime), bool createNew = true)
        {
            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            entryMethods.getEntryTypeFormats(entryType, ref ext, ref extComplete, ref extSearchPattern);

            Chapter chapter = new Chapter();
            String rtf = "";

            // load entry file
            if (!entryMethods.loadEntryFile(ref chapter, path, ref rtf, entryType))
                return null;

            // entry's identification in the application and in the database.
            if (createNew)
                chapter.guid = Guid.NewGuid();

            // we auto initialize entry with latest date and time if the entry has no date and time
            if (chapter.chapterDateTime == default(DateTime))
                chapter.chapterDateTime = DateTime.Now;

            // use custom date and time if user provides it
            if (customDateTime != default(DateTime))
                chapter.chapterDateTime = customDateTime;

            myNode node = new myNode(ref chapter);  
            rtfOut = rtf;
            return node;
        }
        public static bool loadEntryFile(ref Chapter chapter, String file, ref String rtf, EntryType entryType)
        {
            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            entryMethods.getEntryTypeFormats(entryType, ref ext, ref extComplete, ref extSearchPattern);

            // load chapter from the file
            switch (entryType)
            {
                case EntryType.Xml:
                    if (!xmlEntry.fromXml(ref chapter, file, ref rtf, true))
                        return false;

                    break;

                case EntryType.Rtf:
                    if (!rtfEntry.fromRtf(ref chapter, file, ref rtf, extComplete))
                        return false;

                    break;

                case EntryType.Txt:
                    if (!txtEntry.fromTxt(ref chapter, file, ref rtf, extComplete))
                        return false;

                    break;

                case EntryType.Html:
                    return false;

                default:
                    return false;
            }

            // now load the xml entry config file if it is not an xml entry.
            if (entryType != EntryType.Xml)
            {
                String output = "";
                String configFile = String.Format(@"{0}.{1}", file, "xml");

                // config file is mandatory part of the pair which consists of xml config file and the entry file.
                // without config file, we cannot know the properties of this entry file. this results in an unknown orphan entry.
                if (!File.Exists(configFile))
                    return false;

                // config file found, load all config into this entry's chapter.
                xmlEntry.fromXml(ref chapter, configFile, ref output);
            }


            return true;
        }
        public static bool exportEntry(myConfig cfg, Guid guid, String path,
            long exportIndex, EntryType OutputEntryType)
        {
            if (guid == Guid.Empty)
                return false;

            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            entryMethods.getEntryTypeFormats(OutputEntryType, ref ext, ref extComplete, ref extSearchPattern);

            // 1st load chapter's data blob
            String rtf = "";
            myNode? node = null;
            if (cfg.radCfgUseOpenFileSystemDB)
                node = OpenFileSystemDB.findLoadNode(cfg.ctx1, guid, ref rtf, true);
            else
                node = SingleFileDB.findLoadNode(cfg.ctx0, guid, ref rtf, true);

            String fileData = "";
            switch (OutputEntryType)
            {
                case EntryType.Xml:
                    fileData = xmlEntry.toXml(ref node.chapter, rtf, true);
                    break;

                case EntryType.Rtf:
                    fileData = rtfEntry.toRtf(rtf);
                    break;

                case EntryType.Html:
                    fileData = htmlEntry.toHtml(rtf);
                    break;

                case EntryType.Txt:
                    fileData = txtEntry.toTxt(rtf);
                    break;

                default:
                    break;
            }

            // make sure path is available
            if (path.Length <= 0)
                return false;

            // output
            String entryName = "";
            path = entryMethods.getFormattedPathFileName(path, node.chapter.guid, node.chapter.parentGuid, node.chapter.Title,
                node.chapter.chapterDateTime, exportIndex, OutputEntryType, out entryName);

            // make sure to mark the file path as long file name
            if (path.IndexOf(@"\\?\") < 0)
                path = @"\\?\" + path;

            try
            {
                // try to write rtf text
                File.WriteAllText(path, fileData);
            }
            catch (Exception)
            {
                // there are some unicode characters in data, so write with unicode writer.
                using (StreamWriter writer = new StreamWriter(path, false, Encoding.Unicode))
                {
                    writer.Write(fileData);
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
            }

            // now output xml entry config file if it is not an xml entry.
            // note we cannot write configuration of this entry in the entry file itself when it isn't 
            // xml file. so we write the configuration in a 2nd file which is xml file.
            // entry is a pair of files, one is xml config file, and the 2nd file is entry data file.
            if (OutputEntryType != EntryType.Xml)
            {
                String configFile = String.Format(@"{0}.{1}", path, "xml");
                String xmlConfigString = xmlEntry.toXml(ref node.chapter, "");
                File.WriteAllText(configFile, xmlConfigString);
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

        public static bool convertEntryFilenameToNode(ref myNode node, String file)
        {
            node.chapter = new Chapter();
            return convertEntryFilenameToChapter(ref node.chapter, file);
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
        public static bool deleteChapterEntry(myConfig cfg, Chapter? identifier, bool mark)
        {
            if (identifier == null)
                return false;

            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return false;

            // finally delete the entry
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.DeleteOrPurgeRecursive(cfg.ctx1, identifier.guid, true, false);
            else
                return SingleFileDB.DeleteOrPurgeRecursive(cfg.ctx0, identifier.guid, true, false);
        }

        public static bool setEntryHighlightFont( 
            myConfig cfg, Chapter identifier, Color highlightFontColor, Font highlightFont)
        {
            identifier.HLFont = commonMethods.FontToString(highlightFont);
            identifier.HLFontColor = commonMethods.ColorToString(highlightFontColor);
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.updateHLFont(cfg.ctx1, identifier);
            else
                return SingleFileDB.updateHLFont(cfg.ctx0, identifier);
        }

        public static bool setEntryHighlightBackColor(
            myConfig cfg, Chapter identifier, Color highlightBackColor)
        {
            identifier.HLBackColor = commonMethods.ColorToString(highlightBackColor);
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.updateHLBackColor(cfg.ctx1, identifier);
            else
                return SingleFileDB.updateHLBackColor(cfg.ctx0, identifier);
        }

        // this method automatically loads calendar nodes. if they do not exist, the method creates them and loads them.
        public static bool initCalenderNodes(myConfig cfg,
            int year, int month, out myNode? yearNodeOut, out myNode? monthNodeOut)
        {
            // now create Year Node direct in DB if it doesn't exists. else load it from db for linking
            List<myNode>? yearNodes = null;
            if (cfg.radCfgUseOpenFileSystemDB)
                yearNodes = OpenFileSystemDB.findNodesByDate(cfg.ctx1, NodeType.YearNode, year, month, -1);
            else
                yearNodes = SingleFileDB.findNodesByDate(cfg.ctx0, NodeType.YearNode, year, month, -1);

            myNode? yearNode = null;
            if (yearNodes.Count() <= 0)
            {
                // this year doesn't exists in db, so create it.
                DateTime yearDateTime = new DateTime(year, 1, 1, 0, 0, 0, 0);
                if (cfg.radCfgUseOpenFileSystemDB)
                    yearNode = OpenFileSystemDB.newNode(ref cfg.ctx1, NodeType.YearNode, yearDateTime, default(Guid), true, "");
                else
                    yearNode = SingleFileDB.newNode(ref cfg.ctx0, NodeType.YearNode, yearDateTime, default(Guid), true, "");

            }
            else
            {
                // this year node already exists, so directly load it to the object.
                yearNode = yearNodes[0];
            }

            // now create Month Node direct in DB if it doesn't exists. else load it from db for linking.
            // note Month node's parent must be the year node.
            List<myNode>? monthNodes = null;
            if (cfg.radCfgUseOpenFileSystemDB)
                monthNodes = OpenFileSystemDB.findNodesByDate(cfg.ctx1, NodeType.MonthNode, year, month, -1);
            else
                monthNodes = SingleFileDB.findNodesByDate(cfg.ctx0, NodeType.MonthNode, year, month, -1);

            myNode? monthNode = null;
            if (monthNodes.Count() <= 0)
            {
                // this month in the current chapter's year doesn't exists in db, so create it.
                DateTime monthDateTime = new DateTime(year, month, 1, 0, 0, 0, 0);
                if (cfg.radCfgUseOpenFileSystemDB)
                    monthNode = OpenFileSystemDB.newNode(ref cfg.ctx1, NodeType.MonthNode, monthDateTime, yearNode.chapter.guid, true, "");
                else
                    monthNode = SingleFileDB.newNode(ref cfg.ctx0, NodeType.MonthNode, monthDateTime, yearNode.chapter.guid, true, "");
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

        public static void sortNodesByDateTime(ref List<myNode> nodes, bool ascending = false)
        {
            nodes = nodes.Select(d => new
            {
                d.chapter.chapterDateTime.Year,
                d.chapter.chapterDateTime.Month,
                d.chapter.chapterDateTime.Day,
                d.chapter.chapterDateTime.Hour,
                d.chapter.chapterDateTime.Minute,
                d.chapter.chapterDateTime.Second,
                d.chapter.chapterDateTime.Millisecond,
                x = d
            })
            .Distinct()
            .OrderBy(d => d.Year)
            .ThenBy(d => d.Month)
            .ThenBy(d => d.Day)
            .ThenBy(d => d.Hour)
            .ThenBy(d => d.Minute)
            .ThenBy(d => d.Second)
            .ThenBy(d => d.Millisecond)
            .Select(d => d.x).ToList();

            if (ascending)
                nodes.Reverse();

        }
    }
}
