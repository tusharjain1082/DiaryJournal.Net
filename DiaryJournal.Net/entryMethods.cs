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

        public static String getFormattedPathFileName(String path, UInt32 id, UInt32 parentId, String title,
            DateTime dateTime, long exportIndex, EntryType entryType, out String entryNameOut)
        {
            String entryName = getFormattedFileName(id, parentId, title, dateTime, exportIndex, entryType, out entryNameOut);
            String file = Path.Combine(path, Path.GetFileName(entryName));
            //file = @"\\?\" + file;
            return file;
        }
        public static String getFormattedFileName(UInt32 id, UInt32 parentId, String title,
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
                id, parentId, modifiedTitle, ext);
            entryNameOut = String.Format("{0}--{1}--{2}--{3}--{4}--", exportIndex, dateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff"),
                id, parentId, modifiedTitle);
            return entryName;
        }

        // this method creates a new id.
        public static UInt32 CreateNodeID(ref UInt32 sourceIndex)
        {
            if (sourceIndex == 0)
                sourceIndex = 1;

            UInt32 index = sourceIndex;
            sourceIndex++;
            return index;
        }

        // this method creates a new id which does not exists in the database.
        public static UInt32 CreateNodeID(ref myConfig cfg)        
        {
            UInt32 index = 0;

            if (cfg.radCfgUseOpenFileSystemDB)
            {
                if (cfg.ctx1.dbConfig.currentDBIndex == 0)
                    cfg.ctx1.dbConfig.currentDBIndex = 1;

                index = cfg.ctx1.dbConfig.currentDBIndex;
                cfg.ctx1.dbConfig.currentDBIndex++;
                return index;
            }
            else
            {
                if (cfg.ctx0.dbConfig.currentDBIndex == 0)
                    cfg.ctx0.dbConfig.currentDBIndex = 1;

                index = cfg.ctx0.dbConfig.currentDBIndex;
                cfg.ctx0.dbConfig.currentDBIndex++;
                return index;
            }
        }


        public static String getEntryLabel(myNode node)
        {
            String entryName = "";
            switch (node.chapter.nodeType)
            {
                case NodeType.SetNode:
                    entryName = String.Format(@"{0}:::Date({1}):::Time({2}:{3}:{4})", node.chapter.Title,
                        node.chapter.chapterDateTime.ToString("dd-MM-yyyy"),
                        node.chapter.chapterDateTime.Hour, node.chapter.chapterDateTime.Minute,
                        node.chapter.chapterDateTime.Second);
                    break;

                case NodeType.YearNode:
                    entryName = node.chapter.chapterDateTime.ToString("yyyy");
                    break;

                case NodeType.MonthNode:
                    entryName = node.chapter.chapterDateTime.ToString("MMMM");
                    break;

                case NodeType.EntryNode:
                    entryName = String.Format(@"Date({0}):::Time({1}:{2}:{3})Title({4})", node.chapter.chapterDateTime.ToString("dd-MM-yyyy"),
                        node.chapter.chapterDateTime.Hour, node.chapter.chapterDateTime.Minute,
                        node.chapter.chapterDateTime.Second, node.chapter.Title);
                    break;

                default:
                    break;

            }
            return entryName;
        }

        // everything is kept in sets. set node is the root node. all import and export of sets exist in set node which is their root node.
        // set node is root node and has no parent.
        public static myNode createSetNode(ref UInt32 currentIndex, String setName, DateTime setDateTime)
        {
            myNode node = new myNode(true);
            node.chapter.chapterDateTime = setDateTime;
            node.chapter.nodeID = CreateNodeID(ref currentIndex);
            node.chapter.nodeType = NodeType.SetNode;
            node.chapter.parentNodeID = 0;
            node.chapter.Title = setName;
            return node;
        }
        // everything is kept in sets. set node is the root node. all import and export of sets exist in set node which is their root node.
        // set node is root node and has no parent.
        public static myNode createSetNode(ref myConfig cfg, String setName, DateTime setDateTime)
        {
            myNode node = new myNode(true);
            node.chapter.chapterDateTime = setDateTime;
            node.chapter.nodeID = CreateNodeID(ref cfg);
            node.chapter.nodeType = NodeType.SetNode;
            node.chapter.parentNodeID = 0;
            node.chapter.Title = setName;
            return node;
        }

        // apply the set node to root nodes. this applies the set node to the entire 100% tree.
        public static void applySetNode(UInt32 setNodeID, ref List<myNode> tree)
        {
            if (setNodeID == 0)
                return;

            List<myNode> rootNodes = entryMethods.findRootNodes(ref tree, true, false);
            foreach (myNode rootNode in rootNodes)
                rootNode.chapter.parentNodeID = setNodeID;
        }

        // find all root nodes
        public static List<myNode> findRootNodes(ref List<myNode> srcNodes, bool sort = true, bool descending = false)
        {
            List<myNode> nodes = new List<myNode>();
            foreach (myNode srcNode in srcNodes)
            {
                myNode node = srcNode;

                // a valid format entry file found, process
                if (node.chapter.parentNodeID == 0)
                {
                    // found matching node, process

                    // add this found node to the list
                    nodes.Add(node);
                }
            }

            // sort by date and time
            if (sort)
                entryMethods.sortNodesByDateTime(ref nodes, descending);

            return nodes;
        }
        // this method finds all first level or direct children of the target parent node, non-recursive.
        public static List<myNode> findFirstLevelChildren(UInt32 parentId, ref List<myNode> srcNodes,
            bool sort = true, bool descending = false)
        {
            List<myNode> nodes = new List<myNode>();
            if (parentId == 0)
                return nodes;

            foreach (myNode srcNode in srcNodes)
            {
                myNode node = srcNode;

                // a valid format entry file found, process
                if (node.chapter.parentNodeID == parentId)
                {
                    // found matching node, process

                    // add this found node to the list
                    nodes.Add(node);
                }
            }

            // sort if required
            if (sort)
                entryMethods.sortNodesByDateTime(ref nodes, descending);

            return nodes;
        }
                
        public static myNode? loadNode(String path, EntryType entryType, ref String rtfOut, 
            DateTime customDateTime = default(DateTime))
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
        public static bool exportEntry(myConfig cfg, UInt32 id, String path,
            long exportIndex, EntryType OutputEntryType)
        {
            if (id == 0)
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
                node = OpenFileSystemDB.findLoadNode(cfg.ctx1, id, ref rtf, true);
            else
                node = SingleFileDB.findLoadNode(cfg.ctx0, id, ref rtf, true);

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
            path = entryMethods.getFormattedPathFileName(path, node.chapter.nodeID, node.chapter.parentNodeID, node.chapter.Title,
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
            ref String title, ref UInt32 id, ref UInt32 parentId)
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
            String completePattern = @"([0-9]*)(--)(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d-\d\d\d)(--)([0-9]*)(--)([0-9]*)(--)(.*)(--)(\..*)";
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
                id = UInt32.Parse(matches3[0].Groups[5].Value);
                parentId = UInt32.Parse(matches3[0].Groups[7].Value);
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
            UInt32 parentId = 0;
            UInt32 id = 0;

            if (!validateExtractEntryFile(file, ref index, ref chapterDate, ref title, ref id, ref parentId))
                return false;

            chapter.chapterDateTime = chapterDate;
            chapter.Title = title;
            chapter.nodeID = id;
            chapter.parentNodeID = parentId;
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
            UInt32 parentId = 0;
            UInt32 id = 0;

            foreach (String file in files)
            {
                if (!validateExtractEntryFile(file, ref foundIndex, ref chapterDate, ref title, ref id, ref parentId))
                    continue;

                if (index == foundIndex)
                    return file; // found a matching file
            }
            // exception, no file found
            return "";
        }

        public static bool setEntryHighlightFont(myConfig cfg, ref myNode node, Color highlightFontColor, Font highlightFont)
        {
            String HLFont = commonMethods.FontToString(highlightFont);
            String HLFontColor = commonMethods.ColorToString(highlightFontColor);
            return updateHLFont(cfg, ref node, HLFont, HLFontColor, null);
        }
        public static bool setEntryHighlightFont(myConfig cfg, UInt32 id, Color highlightFontColor, Font highlightFont)
        {
            String HLFont = commonMethods.FontToString(highlightFont);
            String HLFontColor = commonMethods.ColorToString(highlightFontColor);
            return updateHLFont(cfg, id, HLFont, HLFontColor, null);
        }
        public static bool setEntryHighlightBackColor(myConfig cfg, ref myNode node, Color highlightBackColor)
        {
            String HLBackColor = commonMethods.ColorToString(highlightBackColor);
            return updateHLBackColor(cfg, ref node, HLBackColor);
        }
        public static bool setEntryHighlightBackColor(myConfig cfg, UInt32 id, Color highlightBackColor)
        {
            String HLBackColor = commonMethods.ColorToString(highlightBackColor);
            return updateHLBackColor(cfg, id, HLBackColor);
        }
        public static bool updateHLBackColor(myConfig cfg, ref myNode node, String? HLBackColor = null)
        {
            // first find the node
            String rtf = "";

            // set properties/config
            if (HLBackColor != null)
                node.chapter.HLBackColor = HLBackColor;

            // update
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.updateNode(cfg.ctx1, ref node, "", false);
            else
                return SingleFileDB.updateNode(cfg.ctx0, ref node, "", false);
        }

        public static bool updateHLBackColor(myConfig cfg, UInt32 id, String? HLBackColor = null)
        {
            // first find the node
            String rtf = "";
            myNode? node = null;

            if (cfg.radCfgUseOpenFileSystemDB)
                node = OpenFileSystemDB.findLoadNode(cfg.ctx1, id, ref rtf, false);
            else
                node = SingleFileDB.findLoadNode(cfg.ctx0, id, ref rtf, false);

            if (node == null)
                return false; // error node not found


            // set properties/config
            if (HLBackColor != null)
                node.chapter.HLBackColor = HLBackColor;

            // update
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.updateNode(cfg.ctx1, ref node, "", false);
            else
                return SingleFileDB.updateNode(cfg.ctx0, ref node, "", false);
        }
        public static bool updateHLFont(myConfig cfg, ref myNode node,
            String? HLFont = null, String? HLFontColor = null, String? HLBackColor = null)
        {
            // first find the node
            String rtf = "";

            if (node == null)
                return false; // error node not found

            // set properties/config
            if (HLFont != null)
                node.chapter.HLFont = HLFont;

            if (HLFontColor != null)
                node.chapter.HLFontColor = HLFontColor;

            if (HLBackColor != null)
                node.chapter.HLBackColor = HLBackColor;

            // update
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.updateNode(cfg.ctx1, ref node, "", false);
            else
                return SingleFileDB.updateNode(cfg.ctx0, ref node, "", false);
        }
        public static bool updateHLFont(myConfig cfg, UInt32 id,
            String? HLFont = null, String? HLFontColor = null, String? HLBackColor = null)
        {
            // first find the node
            String rtf = "";
            myNode? node = null;

            if (cfg.radCfgUseOpenFileSystemDB)
                node = OpenFileSystemDB.findLoadNode(cfg.ctx1, id, ref rtf, false);
            else
                node = SingleFileDB.findLoadNode(cfg.ctx0, id, ref rtf, false);

            if (node == null)
                return false; // error node not found

            // set properties/config
            if (HLFont != null)
                node.chapter.HLFont = HLFont;

            if (HLFontColor != null)
                node.chapter.HLFontColor = HLFontColor;

            if (HLBackColor != null)
                node.chapter.HLBackColor = HLBackColor;

            // update
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.updateNode(cfg.ctx1, ref node, "", false);
            else
                return SingleFileDB.updateNode(cfg.ctx0, ref node, "", false);
        }


        // this method automatically loads calendar nodes. if they do not exist, the method creates them and loads them.
        public static bool initCalenderNodes(myConfig cfg, ref myNode setNode, int year, int month, out myNode? yearNodeOut, out myNode? monthNodeOut)
        {
            // now create Year Node direct in DB if it doesn't exists. else load it from db for linking
            List<myNode>? yearNodes = null;
            if (cfg.radCfgUseOpenFileSystemDB)
                yearNodes = OpenFileSystemDB.findNodesByDate(cfg.ctx1, NodeType.YearNode, year, month, -1);
            else
                yearNodes = SingleFileDB.findNodesByDate(cfg.ctx0, NodeType.YearNode, year, month, -1);

            myNode? yearNode = null;
            myNode? nullNode = null;
            if (yearNodes.Count() <= 0)
            {
                // this year doesn't exists in db, so create it.
                DateTime yearDateTime = new DateTime(year, 1, 1, 0, 0, 0, 0);
                if (cfg.radCfgUseOpenFileSystemDB)
                    yearNode = OpenFileSystemDB.newNode(ref cfg.ctx1, NodeType.YearNode, DomainType.Journal, ref nullNode,
                        yearDateTime, setNode.chapter.nodeID, true, "", true);
                else
                    yearNode = SingleFileDB.newNode(ref cfg.ctx0, NodeType.YearNode, DomainType.Journal, ref nullNode,
                        yearDateTime, setNode.chapter.nodeID, true, "", true);

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
                    monthNode = OpenFileSystemDB.newNode(ref cfg.ctx1, NodeType.MonthNode, DomainType.Journal, ref nullNode,
                        monthDateTime, yearNode.chapter.nodeID, true, "", true);
                else
                    monthNode = SingleFileDB.newNode(ref cfg.ctx0, NodeType.MonthNode, DomainType.Journal, ref nullNode,
                        monthDateTime, yearNode.chapter.nodeID, true, "", true);
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

        public static void sortNodesByDateTime(ref List<myNode> nodes, bool descending = false)
        {
            // descending = latest/top/max/last to earliest/bottom/least/first (-)
            // ascending = earliest/bottom/least/first to latest/top/max/last (+)

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

            if (descending)
                nodes.Reverse();

        }

        public static myNode? findNode(List<myNode> nodes, UInt32 id)
        {
            foreach(myNode node in nodes)
            {
                if (node.chapter == null)
                    continue;

                if (node.chapter.nodeID == id)
                    return node;
            }
            return null;
        }
    }
}
