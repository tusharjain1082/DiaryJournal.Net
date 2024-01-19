using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using RtfPipe.Model;
using System.Xml.Linq;
using System.IO;
using System.Windows.Documents;
using System.Reflection;
using System.Data;

namespace DiaryJournal.Net
{
    public static class entryMethods
    {
        public const String xmlExt = "xml";
        public const String rtfExt = "rtf";
        public const String htmlExt = "html";
        public const String txtExt = "txt";
        public const String cfgExt = "cfg";
        public const String pdfExt = "pdf";
        public const String xmlExtComplete = ".xml";
        public const String rtfExtComplete = ".rtf";
        public const String htmlExtComplete = ".html";
        public const String txtExtComplete = ".txt";
        public const String cfgExtComplete = ".cfg";
        public const String pdfExtComplete = ".pdf";
        public const String xmlExtSearchPattern = "*.xml";
        public const String rtfExtSearchPattern = "*.rtf";
        public const String htmlExtSearchPattern = "*.html";
        public const String txtExtSearchPattern = "*.txt";
        public const String cfgExtSearchPattern = "*.cfg";
        public const String pdfExtSearchPattern = "*.pdf";

        public static bool getEntryTypeFormatsByFileName(String file, ref EntryType entryTypeOut, 
            ref String extOut, ref String extCompleteOut, ref String extSearchPatternOut)
        {
            if (file.Length <= 0)
                return false;

            FileInfo fileInfo = new FileInfo(file);
            String extension = fileInfo.Extension;
            if (extension.Length <= 0)
                return false;

            switch (extension)
            {
                case ".xml":
                    entryTypeOut = EntryType.Xml;
                    getEntryTypeFormats(EntryType.Xml, ref extOut, ref extCompleteOut, ref extSearchPatternOut);
                    break;

                case ".rtf":
                    entryTypeOut = EntryType.Rtf;
                    getEntryTypeFormats(EntryType.Rtf, ref extOut, ref extCompleteOut, ref extSearchPatternOut);
                    break;

                case ".txt":
                    entryTypeOut = EntryType.Txt;
                    getEntryTypeFormats(EntryType.Txt, ref extOut, ref extCompleteOut, ref extSearchPatternOut);
                    break;

                case ".html":
                    entryTypeOut = EntryType.Html;
                    getEntryTypeFormats(EntryType.Html, ref extOut, ref extCompleteOut, ref extSearchPatternOut);
                    break;

                case ".pdf":
                    entryTypeOut = EntryType.Pdf;
                    getEntryTypeFormats(EntryType.Pdf, ref extOut, ref extCompleteOut, ref extSearchPatternOut);
                    break;

                case ".cfg":
                    entryTypeOut = EntryType.Cfg;
                    getEntryTypeFormats(EntryType.Cfg, ref extOut, ref extCompleteOut, ref extSearchPatternOut);
                    break;

                default:
                    return false;
            }
            return true;
        }
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
                case EntryType.Pdf:
                    extOut = pdfExt;
                    extCompleteOut = pdfExtComplete;
                    extSearchPatternOut = pdfExtSearchPattern;
                    break;
                case EntryType.Cfg:
                    extOut = cfgExt;
                    extCompleteOut = cfgExtComplete;
                    extSearchPatternOut = cfgExtSearchPattern;
                    break;
                default:
                    extOut = "";
                    extCompleteOut = "";
                    extSearchPatternOut = "";
                    break;
            }
        }

        public static String getFormattedPathFileName(String path, Int64 id, Int64 parentId, String title,
            DateTime dateTime, long exportIndex, EntryType entryType, out String entryNameOut)
        {
            String entryName = getFormattedFileName(id, parentId, title, dateTime, exportIndex, entryType, out entryNameOut);
            String file = Path.Combine(path, Path.GetFileName(entryName));
            //file = @"\\?\" + file;
            return file;
        }
        public static String getFormattedFileName(Int64 id, Int64 parentId, String title,
            DateTime dateTime, long exportIndex, EntryType entryType, out String entryNameOut)
        {
            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            entryMethods.getEntryTypeFormats(entryType, ref ext, ref extComplete, ref extSearchPattern);

            // common chapter entry. we use proper journal format
            String modifiedTitle = ((title != "") ? title.Replace("--", "-") : "");
            modifiedTitle = title.Replace(":", "-");
            String entryName = String.Format("{0}--{1}--{2}--{3}--{4}--.{5}", exportIndex, dateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff"),
                id, parentId, modifiedTitle, ext);
            entryNameOut = String.Format("{0}--{1}--{2}--{3}--{4}--", exportIndex, dateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff"),
                id, parentId, modifiedTitle);
            return entryName;
        }

        // this method creates a new id.
        public static Int64 CreateNodeID(ref Int64 sourceIndex)
        {
            if (sourceIndex == 0)
                sourceIndex = 1;

            Int64 index = sourceIndex;
            sourceIndex++;
            return index;
        }

        // this method creates a new id which does not exists in the database.
        public static Int64 CreateNodeID(ref myConfig cfg)        
        {
            Int64 index = 0;

            if (cfg.radCfgUseOpenFileSystemDB)
            {
                if (cfg.ctx1.dbIndexing.currentDBIndex == 0)
                    cfg.ctx1.dbIndexing.currentDBIndex = 1;

                index = cfg.ctx1.dbIndexing.currentDBIndex;
                cfg.ctx1.dbIndexing.currentDBIndex++;
                return index;
            }
            else
            {
                if (cfg.ctx0.dbIndexing.currentDBIndex == 0)
                    cfg.ctx0.dbIndexing.currentDBIndex = 1;

                index = cfg.ctx0.dbIndexing.currentDBIndex;
                cfg.ctx0.dbIndexing.currentDBIndex++;
                return index;
            }
        }

        public static String removeAllInvalidPathCharacters(String value)
        {
            string illegal = value;//"//\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            value = r.Replace(illegal, "");
            return value;
        }
        public static String getEntryLabel(myNode node, bool removeInvalidPathChars)
        {
            String entryName = "";
            switch (node.chapter.nodeType)
            {
                /*
                case NodeType.Journal:
                    entryName = String.Format(@"{0}:(ID {1})", mySystemNodes.JournalSystemNodeName, node.chapter.Id.ToString());
                    break;

                case NodeType.Library:
                    entryName = String.Format(@"{0}:(ID {1})", mySystemNodes.LibrarySystemNodeName, node.chapter.Id.ToString());
                    break;
                */
                case NodeType.Label:
                    entryName = String.Format(@"{0}:(ID {1})", node.chapter.Title, node.chapter.Id.ToString());
                    break;

                case NodeType.NonCalendarEntry:
                    entryName = String.Format(@"{0}:(ID {1})", node.chapter.Title, node.chapter.Id.ToString());
                    break;

                case NodeType.Set:
                    entryName = String.Format(@"CloneSet:({0}):({1}):({2}:{3}:{4}:{5}):(ID {6})", node.chapter.Title,
                        node.chapter.chapterDateTime.ToString("dd-MM-yyyy"),
                        node.chapter.chapterDateTime.Hour, node.chapter.chapterDateTime.Minute,
                        node.chapter.chapterDateTime.Second,
                        node.chapter.chapterDateTime.Millisecond,
                        node.chapter.Id.ToString());
                    break;

                case NodeType.Year:
                    entryName = String.Format("{0}:(ID {1})", node.chapter.chapterDateTime.ToString("yyyy"), node.chapter.Id.ToString());
                    break;

                case NodeType.Month:
                    entryName = String.Format("{0}:(ID {1})", node.chapter.chapterDateTime.ToString("MMMM"), node.chapter.Id.ToString());
                    break;

                case NodeType.Entry:
                    entryName = String.Format(@"({0}):({1}:{2}:{3}:{4}):({5}):(ID {6})", node.chapter.chapterDateTime.ToString("dd-MM-yyyy"),
                        node.chapter.chapterDateTime.Hour, node.chapter.chapterDateTime.Minute,
                        node.chapter.chapterDateTime.Second, node.chapter.chapterDateTime.Millisecond, node.chapter.Title, node.chapter.Id.ToString());
                    break;

                default:
                    if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                        entryName = String.Format(@"{0}:(ID {1})", mySystemNodes.getSystemNodeName(node.chapter.nodeType), node.chapter.Id.ToString());
                    else
                        entryName = String.Format(@"{0}:(ID {1})", node.chapter.Title, node.chapter.Id.ToString());

                    break;

            }
            if (removeInvalidPathChars)
                entryName = removeAllInvalidPathCharacters(entryName);

            return entryName;
        }

        // this method auto loads system nodes. if they don't exist, the method auto creates them and loads them
        public static bool autoCreateLoadSystemNodes(ref myConfig cfg, ref List<myNode> allNodes, out mySystemNodes? systemNodesOut)
        {
            // first try to find and load system nodes
            mySystemNodes systemNodes = new mySystemNodes();
            if (!createCoreSystemNodes(ref cfg, ref allNodes, ref systemNodes))
            {
                // critical error abort
                systemNodesOut = null;
                return false;
            }

            // load all other system nodes such as year and month nodes if they exist from the source list
            loadOtherSystemNodesCollection(ref allNodes, ref systemNodes);

            // return output
            systemNodesOut = systemNodes;
            return true;
        }

        // find all system nodes
        public static List<myNode> findSystemNodes(ref List<myNode> srcNodes, bool coreSystemNodes = true, bool calendarNodes = false, bool sort = true, bool descending = false)
        {
            List<myNode> nodes = new List<myNode>();
            foreach (myNode srcNode in srcNodes)
            {
                myNode node = srcNode;

                if (node.chapter.specialNodeType != SpecialNodeType.SystemNode)
                    continue; // not system node so skip

                if (!calendarNodes)
                {
                    if (node.chapter.nodeType == NodeType.Year || node.chapter.nodeType == NodeType.Month)
                        continue; // calendar system node not wanted so skip it
                }

                if (!coreSystemNodes)
                {
                    // core system nodes not wanted so skip
                    if (mySystemNodes.isCoreSystemNode(node.chapter.nodeType))
                        continue;
                }
                // finally add this system node
                nodes.Add(node);
            }

            // sort by date and time
            if (sort)
                entryMethods.sortNodesByDateTime(ref nodes, descending);

            return nodes;
        }

        // find all system nodes
        public static List<myNode> findNodesByTypes(ref List<myNode> srcNodes, SpecialNodeType specialNodeType,
            NodeType nodeType, bool sort = true, bool descending = false)
        {
            List<myNode> nodes = new List<myNode>();
            foreach (myNode srcNode in srcNodes)
            {
                myNode node = srcNode;

                // a valid format entry file found, process
                if (node.chapter.specialNodeType != SpecialNodeType.AnyOrAll)
                {
                    // find only by special type
                    if (node.chapter.specialNodeType != specialNodeType)
                        continue;

                    // found matching node, process
                    if (nodeType != NodeType.AnyOrAll)
                    {
                        // 2nd condition required by user
                        if (node.chapter.nodeType == nodeType)
                            nodes.Add(node);
                    }
                    else
                    {
                        // no node type requirement by user, so add all matching nodes of node type
                        nodes.Add(node);
                    }
                }
                else
                {
                    // find all special types

                    if (nodeType != NodeType.AnyOrAll)
                    {
                        // find only by node type
                        // 2nd condition required by user
                        if (node.chapter.nodeType == nodeType)
                            nodes.Add(node);
                    }
                    else
                    {
                        // find all node types
                        // no node type requirement by user, so add all matching nodes of node type
                        nodes.Add(node);
                    }

                }
            }

            // sort by date and time
            if (sort)
                entryMethods.sortNodesByDateTime(ref nodes, descending);

            return nodes;
        }
        // this method finds and loads all system nodes into proper places in the system nodes collection
        public static void loadOtherSystemNodesCollection(ref List<myNode> allNodes, ref mySystemNodes systemNodes)
        {
            // reset
            systemNodes.YearNodes.Clear();
            systemNodes.MonthNodes.Clear();

            List<myNode> otherNodes = findSystemNodes(ref allNodes, false, true, true, false);

            // now find items and fill in appropirate places
            foreach (myNode node in otherNodes)
            {
                if (node.chapter.nodeType == NodeType.Year)
                    systemNodes.YearNodes.Add(node);
                else if (node.chapter.nodeType == NodeType.Month)
                    systemNodes.MonthNodes.Add(node);
            }
        }
        // this method creates new system nodes
        public static bool createCoreSystemNodes(ref myConfig cfg, ref List<myNode> allNodes, ref mySystemNodes systemNodes)
        {
            List<myNode> coreNodesCollection = findSystemNodes(ref allNodes, true, false, true, false);

            foreach (String systemNodeName in mySystemNodes.SystemNodesNames)
            {
                NodeType nodeType = commonMethods.convertToEnum<NodeType>(systemNodeName);

                // skip this system node if it exists in db
                myNode? found = null;
                if (coreNodesCollection.Count() > 0) found = coreNodesCollection.FirstOrDefault(s => s.chapter.nodeType == nodeType);
                if (found != null) systemNodes.setSystemNode(nodeType, ref found);
                if (found != null) continue;

                // this node does not exists in db, so auto create it and load it in collections
                myNode newNode = new myNode(true);
                newNode.chapter.specialNodeType = SpecialNodeType.SystemNode;
                newNode.chapter.nodeType = nodeType;
                newNode.chapter.chapterDateTime = DateTime.Now;
                newNode.chapter.Title = systemNodeName;

                // finally create this new node
                if (!DBCreateNode(ref cfg, ref newNode, "", true, true, true, true, true, true))
                    return false; // critical error abort

                // finally add the new node and update the collection
                systemNodes.setSystemNode(nodeType, ref newNode);
                allNodes.Add(newNode);
            }
            return true;
        }

        // find all deleted marked nodes in db
        public static List<myNode> DBFindDeletedNodes(ref List<myNode> allNodes)
        {
            List<myNode> nodes = new List<myNode>();
            foreach (myNode node in allNodes)
            {
                if (node.chapter.IsDeleted)
                    nodes.Add(node);
            }
            return nodes;
        }
        // this promotes the node to one level up in tree structure.
        public static bool DBPromoteNode(myConfig cfg, ref List<myNode> allNodes, ref myNode? node)
        {
            if (node == null)
                return false; // error node not found

            // validate
            if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return false;

            if (node.chapter.parentId == 0)
                return false;

            // first find the parent
            myNode? parentNode = FindNodeInList(ref allNodes, node.chapter.parentId);
            if (parentNode == null)
                return false; // error node not found

            // now set the parent's parent as this child node's parent.
            // this will promote this child node in one level up.
            node.chapter.parentId = parentNode.chapter.parentId;

            // now update the node
            return DBUpdateNode(cfg, ref node, "", false, false);
        }

        // this promotes the node to root of the tree structure
        public static bool DBPromoteNodeToRoot(myConfig cfg, ref myNode? node)
        {
            if (node == null)
                return false; // error node not found

            // validate
            if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return false;

            if (node.chapter.parentId == 0)
                return false;

            // now clear the parent id of this node to move it to root of the tree.
            node.chapter.parentId = 0;

            // now update the node
            return DBUpdateNode(cfg, ref node, "", false, false);
        }

        // this method sets or unsets a parent for a target node by guid
        public static bool DBSetNodeParent(myConfig cfg, ref myNode? node, Int64 parentId)
        {
            if (node == null)
                return false; // error node not found

            // validate
            if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return false;

            // set parent id: 0 for no parent, 1+ for a valid parent
            node.chapter.parentId = parentId;

            // now update the node
            return DBUpdateNode(cfg, ref node, "", false, false);
        }

        // load the entry data through entry's node
        public static String DBLoadNodeData(myConfig cfg, Int64 id, Int64 sectionId = 0)
        {
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.loadNodeData(cfg.ctx1, sectionId, id);
            else
                return SingleFileDB.loadNodeData(cfg.ctx0, id);
        }

        public static bool DBCheckpoint(ref myConfig cfg)
        {
            if (cfg.radCfgUseSingleFileDB)
                return SingleFileDB.Checkpoint(cfg.ctx0);

            return false;
        }
        // this method auto selects the db core and creates a new node with rtf into it
        public static bool DBCreateNode(ref myConfig cfg, ref myNode node, String rtf,
            bool resetCD, bool resetMD, bool resetDD,
            bool newID, bool writeDBIndexingFile, bool checkpoint)
        {
            bool result = false;

            if (cfg.radCfgUseOpenFileSystemDB)
                result = OpenFileSystemDB.createNode(cfg.ctx1, ref node, rtf, resetCD, resetMD, resetDD, newID);
            else
                result = SingleFileDB.createNode(cfg.ctx0, ref node, rtf, resetCD, resetMD, resetDD, newID, checkpoint);

            // write config file if user demands
            if (result)
            {
                if (writeDBIndexingFile)
                    DBWriteIndexing(ref cfg);
            }
            return result;
        }
        // this method auto selects the db core and creates new node with given config and rtf
        public static myNode? DBNewNode(ref myConfig cfg,
            SpecialNodeType specialNodeType, NodeType nodeType, DomainType domainType, ref myNode? initialNode,
            bool resetCD, bool resetMD, bool resetDD,
            DateTime nodeDateTime = default(DateTime), Int64 parentId = 0, bool DBImport = true,
            String title = "", String rtf = "", bool newID = true, bool writeDBIndexingFile = true,
             bool checkpoint = true)
        {
            myNode? node = null;
            if (cfg.radCfgUseOpenFileSystemDB)
                node = OpenFileSystemDB.newNode(ref cfg.ctx1,
                    specialNodeType, nodeType, domainType,
                    ref initialNode, resetCD, resetMD, resetDD, nodeDateTime, parentId, DBImport, title, rtf, newID);
            else
                node = SingleFileDB.newNode(ref cfg.ctx0,
                    specialNodeType, nodeType, domainType,
                    ref initialNode, resetCD, resetMD, resetDD, nodeDateTime, parentId, DBImport, title, rtf, newID, checkpoint);

            // write config file if user demands
            if (node != null)
            {
                if (writeDBIndexingFile)
                    DBWriteIndexing(ref cfg);
            }
            return node;
        }
        // this method auto selects the db core writes it's indexing file
        public static void DBWriteIndexing(ref myConfig cfg)
        {
            if (cfg.radCfgUseOpenFileSystemDB)
                DatabaseIndexing.toFile(ref cfg.ctx1.dbIndexing, cfg.ctx1.dbIndexingFile);
            else
                DatabaseIndexing.toFile(ref cfg.ctx0.dbIndexing, cfg.ctx0.dbIndexingFile);
        }
        // this method auto selects the db core sets the index
        public static void DBSetIndexingIndex(ref myConfig cfg, DatabaseType dbType, long index)
        {
            if (dbType == DatabaseType.OpenFSDB)
                cfg.ctx1.dbIndexing.currentDBIndex = index;
            else if (dbType == DatabaseType.SingleFileDB)
                cfg.ctx0.dbIndexing.currentDBIndex = index;
        }
        // this method copies the db indexing from one db to another
        public static void DBCopyIndexingIndex(ref myConfig cfgSrc, ref myConfig cfgDest, DatabaseType srcDBType, DatabaseType destDBType)
        {
            if (srcDBType == DatabaseType.SingleFileDB)
            {
                if (destDBType == DatabaseType.SingleFileDB)
                {
                    cfgDest.ctx0.dbIndexing.currentDBIndex = cfgSrc.ctx0.dbIndexing.currentDBIndex;
                }
                else if (destDBType == DatabaseType.OpenFSDB)
                {
                    cfgDest.ctx1.dbIndexing.currentDBIndex = cfgSrc.ctx0.dbIndexing.currentDBIndex;
                }
            }
            else if (srcDBType == DatabaseType.OpenFSDB)
            {
                if (destDBType == DatabaseType.SingleFileDB)
                {
                    cfgDest.ctx0.dbIndexing.currentDBIndex = cfgSrc.ctx1.dbIndexing.currentDBIndex;
                }
                else if (destDBType == DatabaseType.OpenFSDB)
                {
                    cfgDest.ctx1.dbIndexing.currentDBIndex = cfgSrc.ctx1.dbIndexing.currentDBIndex;
                }
            }
        }

        // this method auto selects the db core writes it's config file
        public static String DBWriteConfig(ref myConfig cfg)
        {
            String xml = "";
            if (cfg.radCfgUseOpenFileSystemDB)
                xml = DatabaseConfig.toXmlFile(ref cfg.ctx1.dbConfig, cfg.ctx1.dbConfigFile);
            else
                xml = DatabaseConfig.toXmlFile(ref cfg.ctx0.dbConfig, cfg.ctx0.dbConfigFile);

            return xml;
        }
        // auto select db and retrieve entire tree structure sequential list most recursively
        public static List<myNode> DBFindAllNodesTreeSequence(ref List<myNode> allNodes,
            bool sort = true, bool descending = false)
        {
            return findAllNodesTreeSequence(ref allNodes, sort, descending);
        }

        // auto select db and find all nodes
        public static List<myNode> DBFindAllNodes(myConfig cfg, bool sort = true, bool descending = false)
        {
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.findAllNodes(cfg.ctx1, sort, descending);
            else
                return SingleFileDB.findAllNodes(cfg.ctx0, sort, descending);
        }
        // auto select db and find a node by id
        public static myNode? DBFindLoadNode(myConfig cfg, Int64 id, ref String rtf, bool loadData = false, Int64 sectionId = 0)
        {
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.findLoadNode(cfg.ctx1, sectionId, id, ref rtf, loadData);
            else
                return SingleFileDB.findLoadNode(cfg.ctx0, id, ref rtf, loadData);
        }
        // auto select db and find a node by id
        public static myNode? FindNodeInList(ref List<myNode> allNodes, Int64 id)
        {
            foreach (myNode node in allNodes)
            {
                if (node.chapter.Id == id)
                    return node;
            }
            return null;
        }
        // find node by title
        public static myNode? FindNodeInListByTitle(ref List<myNode> allNodes, String title)
        {
            foreach (myNode node in allNodes)
            {
                if (node.chapter.Title == title)
                    return node;
            }
            return null;
        }
        // auto select db and find a node by title
        public static myNode? FindNodeByTitleInList(ref List<myNode> allNodes, String title)
        {
            foreach (myNode node in allNodes)
            {
                if (node.chapter.Title == title)
                    return node;
            }
            return null;
        }
        // auto select db and find a node by title
        public static myNode? BuildPathNodes(ref Int64 startingNodeIndex, String path, String delimiter, ref List<myNode> listOut)
        {
            List<myNode> list = new List<myNode>();

            if (path.Length == 0)
                return null;

            if (delimiter.Length == 0)
                return null;

            String[] values = path.Split(delimiter);
            if (values.Length == 0)
                return null;

            myNode? parentNode = null;
            foreach (String value in values)
            {
                myNode node = new myNode(true);
                node.chapter.Id = startingNodeIndex++;
                if (parentNode != null)
                    node.chapter.parentId = parentNode.chapter.Id;

                node.chapter.Title = value;
                node.chapter.chapterDateTime = DateTime.Now;
                listOut.Add(node);
                parentNode = node;
            }
            return null;
        }

        // this method finds all lineage chain of a parent node most recursively
        public static List<myNode> FindAllChildrenRecursiveInList(ref List<myNode> allNodes,
            ref myNode node, bool sort = true, bool descending = false, bool addParentNode = false)
        {
            List<myNode> list = new List<myNode>();

            if (node == null)
                return list; // error node not found

            Queue<myNode> queue = new Queue<myNode>();
            queue.Enqueue(node);

            // add parent node at index 0 if demanded
            if (addParentNode)
                list.Add(node);

            while (queue.Count > 0)
            {
                myNode currentNode = queue.Dequeue();

                List<myNode> children = findFirstLevelChildren(currentNode.chapter.Id, ref allNodes, sort, descending);

                foreach (myNode childNode in children)
                    queue.Enqueue(childNode);

                if (currentNode.chapter.Id != node.chapter.Id)
                    list.Add(currentNode);
            }
            return list;
        }
        // this method finds all lineage chain of all parent nodes most recursively
        public static List<myNode> FindSelectedNodesAllChildrenRecursiveInList(ref List<myNode> allNodes,
            ref List<myNode> selNodes, bool sort = true, bool descending = false, bool addParentNode = false)
        {
            List<myNode> list = new List<myNode>();

            if (selNodes.Count == 0)
                return list; // error node not found

            foreach (myNode selNode in selNodes)
            {
                Queue<myNode> queue = new Queue<myNode>();
                queue.Enqueue(selNode);

                // add parent node at index 0 if demanded
                if (addParentNode)
                    list.Add(selNode);

                while (queue.Count > 0)
                {
                    myNode currentNode = queue.Dequeue();

                    List<myNode> children = findFirstLevelChildren(currentNode.chapter.Id, ref allNodes, sort, descending);

                    foreach (myNode childNode in children)
                        queue.Enqueue(childNode);

                    if (currentNode.chapter.Id != selNode.chapter.Id)
                        list.Add(currentNode);
                }
            }
            return list;
        }

        public static bool GenerateLineagePath(ref List<myNode> allNodes, ref myNode srcNode, out String outFormatted, out List<myNode> outLineage)
        {
            List<myNode> lineage = new List<myNode>();
            if ((srcNode == null) || (allNodes.Count() <= 0))
            {
                outFormatted = "";
                outLineage = lineage;
                return false;
            }

            // get lineage
            lineage = findBottomToRootNodesRecursive(ref allNodes, ref srcNode, false, false, true);

            // get lineage formatted full path
            outFormatted = "";
            foreach (myNode? node in lineage)
                outFormatted += @"\\" + getEntryLabel(node, false);

            // set and return
            outLineage = lineage;
            return true;
        }
        // this method first finds & loads the current node by it's guid, then recursively finds & loads all it's parents and ancestors
        // right to the root ancestor which has no parent of it's own.
        public static List<myNode> findBottomToRootNodesRecursive(ref List<myNode> allNodes, ref myNode srcNode, bool deleted = false,
            bool useDeletedParam = false, bool topToBottom = false, bool includeNonDeleted = false)
        {
            // todo tushar: srcNode

            List<myNode> nodes = new List<myNode>();

            if (srcNode == null)
                return nodes; // error node not found

            // if user demands to find deleted nodes, then validate and add accordingly
            if (useDeletedParam)
            {
                if (srcNode.chapter.IsDeleted == deleted)
                    nodes.Add(srcNode); // matching node either deleted marked or not deleted marked as demanded by user

                // include non deleted nodes if demanded
                if (includeNonDeleted && !srcNode.chapter.IsDeleted)
                    nodes.Add(srcNode);
                    
            }
            else
            {
                // add this node at index 0 of the list
                nodes.Add(srcNode);
            }

            if (nodes.Count <= 0)
                return nodes;

            myNode? node = srcNode;
            while (true)
            {
                if (node == null)
                    break;

                if (node.chapter.parentId == 0)
                    break;

                // find and load all parent nodes recursively from bottom to root
                node = FindNodeInList(ref allNodes, node.chapter.parentId);
                if (node == null)
                    break; // no more parents found, this is end of loop

                // a parent found

                // if user demands to find deleted nodes, then validate and add accordingly
                if (useDeletedParam)
                {
                    if (node.chapter.IsDeleted == deleted)
                        nodes.Add(node); // matching node either deleted marked or not deleted marked as demanded by user

                    // include non deleted nodes if demanded
                    if (includeNonDeleted && !srcNode.chapter.IsDeleted)
                        nodes.Add(srcNode);

                }
                else
                {
                    // add this parent node in the list
                    nodes.Add(node);
                }
            }

            // from top ancestor to bottom most child if demanded
            if (topToBottom)
                nodes.Reverse();

            return nodes;
        }
        // this method automatically checks if a node is an ancestor of source node
        public static int IsAncestorNode(ref List<myNode> allNodes, Int64 nodeToCheckId, myNode srcNode, bool deleted = false,
            bool useDeletedParam = false)
        {
            if (srcNode == null) return -1;

            myNode? node = srcNode;
            List<myNode> ancestors = findBottomToRootNodesRecursive(ref allNodes, ref srcNode, deleted, useDeletedParam);
            myNode? found = FindNodeInList(ref ancestors, nodeToCheckId);
            if (found == null)
                return 0;
            else
                return 1;
        }


        // this method purges the old unusable files and replaces them with new update, and updates the node accordingly. 
        public static bool DBUpdateNode(myConfig cfg, ref myNode node, String rtf = "", bool storeData = false, bool updateModificationDate = true,
            bool checkpoint = true)
        {
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.updateNode(cfg.ctx1, ref node, rtf, storeData, updateModificationDate);
            else
                return SingleFileDB.updateNode(cfg.ctx0, ref node, rtf, storeData, updateModificationDate, checkpoint);
        }

        // erases and purges the node's files
        public static bool DBPurgeNode(myConfig cfg, ref myNode node, bool checkpoint = true)
        {
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.purgeNode(cfg.ctx1, ref node);
            else
                return SingleFileDB.purgeNode(cfg.ctx0, ref node, checkpoint);
        }

        // deletes or restores the node and restores all the affected parent nodes if the child node is restored
        public static bool DBDeleteOrPurgeNode(myConfig cfg, ref List<myNode> allNodes, ref myNode node, bool mark = true, bool purge = false,
            bool checkpoint = true)
            
        {
            // first get the node
            if (node == null)
                return false; // no more parents found, this is end of loop

            if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
            {
                // system nodes cannot be deleted/purged
                allNodes.Remove(node);
                return false;
            }

            // purge the marked node if demanded
            if (purge)
            {
                allNodes.Remove(node);
                return DBPurgeNode(cfg, ref node, false);
            }

            if (!mark)
            {
                // user demands to restore this node, so all it's parents which were marked deleted,
                // must also be restored so that this node is restored to completion.
                List<myNode> effectedNodes = findBottomToRootNodesRecursive(ref allNodes, ref node, true, true);

                // restore all effected nodes
                foreach (myNode listedNode in effectedNodes)
                {
                    myNode effectedNode = listedNode;
                    effectedNode.chapter.IsDeleted = false; // restore all the affected nodes from bottom to the top
                    effectedNode.chapter.deletionDateTime = default(DateTime);
                    DBUpdateNode(cfg, ref effectedNode, "", false, false, false);
                    allNodes.Remove(listedNode); // parent node processed, remove them
                }
            }
            else
            {
                // mark delete
                node.chapter.IsDeleted = true;
                node.chapter.deletionDateTime = DateTime.Now;

                // finally update the node
                DBUpdateNode(cfg, ref node, "", false, false, false);
            }

            // this node was processed, so remove it from list
            allNodes.Remove(node);

            return true;

        }

        public static bool DBDeleteOrPurgeListRecursive(myConfig cfg, ref List<myNode> allNodes,
            ref List<myNode> nodes, bool mark = true, bool purge = false, bool checkpoint = true)
        {
            foreach (myNode listedNode in nodes)
            {
                myNode node = listedNode;

                // if it is delete/purge then recursion list is used, otherwise there is no recursion list.
                // in restore we process every list node and only restore it and only it's effected ancestors right to top.
                if ((mark || purge) || (mark && purge))
                {
                    // process all children of this node through a recursion list
                    List<myNode> children = FindAllChildrenRecursiveInList(ref allNodes, ref node, true, false);
                    foreach (myNode listedChild in children)
                    {
                        myNode child = listedChild;
                        DBDeleteOrPurgeNode(cfg, ref allNodes, ref child, mark, purge, checkpoint);
                    }
                }

                // finally process this node
                DBDeleteOrPurgeNode(cfg, ref allNodes, ref node, mark, purge, checkpoint);
            }

            return true;
        }

        public static bool DBDeleteOrPurgeNodeRecursive(myConfig cfg, ref List<myNode> allNodes, ref myNode node, bool mark = true, bool purge = false,
            bool checkpoint = true)
        {
            if (node == null)
                return false;

            // if it is delete/purge then recursion list is used, otherwise there is no recursion list.
            // in restore we process every list node and only restore it and only it's effected ancestors right to top.
            if ((mark || purge) || (mark && purge))
            {
                // process all children of this node through a recursion list
                List<myNode> children = FindAllChildrenRecursiveInList(ref allNodes, ref node, true, false);
                foreach (myNode listedChild in children)
                {
                    myNode child = listedChild;
                    DBDeleteOrPurgeNode(cfg, ref allNodes, ref child, mark, purge, checkpoint);
                }
            }

            // finally process this node
            DBDeleteOrPurgeNode(cfg, ref allNodes, ref node, mark, purge, checkpoint);

            return true;
        }
        // everything is kept in sets. set node is the root node. all import and export of sets exist in set node which is their root node.
        // set node is root node and has no parent.
        public static myNode createSetNode(ref Int64 currentIndex, String setName, DateTime setDateTime)
        {
            myNode node = new myNode(true);
            node.chapter.chapterDateTime = setDateTime;
            node.chapter.creationDateTime = setDateTime;
            node.chapter.modificationDateTime = setDateTime;
            node.chapter.Id = CreateNodeID(ref currentIndex);
            node.chapter.nodeType = NodeType.Set;
            node.chapter.parentId = 0;
            node.chapter.Title = setName;
            return node;
        }
        // everything is kept in sets. set node is the root node. all import and export of sets exist in set node which is their root node.
        // set node is root node and has no parent.
        public static myNode createSetNode(ref myConfig cfg, String setName, DateTime setDateTime)
        {
            myNode node = new myNode(true);
            node.chapter.chapterDateTime = setDateTime;
            node.chapter.creationDateTime = setDateTime;
            node.chapter.modificationDateTime = setDateTime;
            node.chapter.Id = CreateNodeID(ref cfg);
            node.chapter.nodeType = NodeType.Set;
            node.chapter.parentId = 0;
            node.chapter.Title = setName;
            return node;
        }

        // apply the set node to root nodes. this applies the set node to the entire 100% tree.
        public static void applySetNode(Int64 setNodeID, ref List<myNode> tree)
        {
            if (setNodeID == 0)
                return;

            List<myNode> rootNodes = entryMethods.findRootNodes(ref tree, SpecialNodeType.AnyOrAll, true, false);
            foreach (myNode rootNode in rootNodes)
                rootNode.chapter.parentId = setNodeID;
        }

        // find all root nodes
        public static List<myNode> findRootNodes(ref List<myNode> srcNodes, SpecialNodeType specialNodeType,
            bool sort = true, bool descending = false)
        {
            List<myNode> nodes = new List<myNode>();
            foreach (myNode srcNode in srcNodes)
            {
                myNode node = srcNode;

                if (specialNodeType == SpecialNodeType.AnyOrAll)
                {
                    // process all nodes
                }
                else if (specialNodeType == SpecialNodeType.NonSystemNode)
                {
                    // user demands non system node type
                    if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                        continue; 
                }
                else if (specialNodeType == SpecialNodeType.SystemNode)
                {
                    // user demands system node type
                    if (node.chapter.specialNodeType != SpecialNodeType.SystemNode)
                        continue;
                }
                else if (specialNodeType == SpecialNodeType.None)
                {
                    // user demands non system node type
                    if (node.chapter.specialNodeType != SpecialNodeType.None)
                        continue;
                }

                // a valid format entry file found, process
                if (node.chapter.parentId == 0)
                {
                    // found matching node, process

                    // add this found node to the list
                    nodes.Add(node);
                }
            }

            // sort by date and time
            if (sort)
                entryMethods.sortNodes(ref nodes, false, true, true, false, false, descending);
                //entryMethods.sortNodesByIdThenDateTime(ref nodes, descending);

            return nodes;
        }
        // dom method: this method sets the new id to a node. if demanded resets the children's parent node id to new parent id
        public static void setNodeID(ref List<myNode> allNodes, myNode node, Int64 newID, bool setChildrenParentID = true)
        {
            node.previousID = node.chapter.Id;
            node.chapter.Id = newID;
            if (setChildrenParentID)
            {
                List<myNode> children = findFirstLevelChildren(node.previousID, ref allNodes, false, false);
                foreach (myNode child in children)
                    child.chapter.parentId = newID;
            }
        }

        // sets node common date and time
        public static bool DBSetNodeCommonDateTime(ref myConfig cfg, ref List<myNode> allNodes, Int64 ID, DateTime newDateTime)
        {
            myNode? node = entryMethods.FindNodeInList(ref allNodes, ID);
            if (node == null)
                return false;

            return DBSetNodeCommonDateTime(ref cfg, ref node, newDateTime);
        }

        // sets node common date and time
        public static bool DBSetNodeCommonDateTime(ref myConfig cfg, ref myNode? node, DateTime newDateTime)
        {
            if (node == null)
                return false;

            // configure node
            node.chapter.chapterDateTime = newDateTime;

            // update
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.updateNode(cfg.ctx1, ref node, "", false, false);
            else
                return SingleFileDB.updateNode(cfg.ctx0, ref node, "", false, false);

        }

        // this method finds all first level or direct children of the target parent node, non-recursive.
        public static List<myNode> findFirstLevelChildren(Int64 parentId, ref List<myNode> srcNodes,
            bool sort = true, bool descending = false)
        {
            List<myNode> nodes = new List<myNode>();
            //if (parentId == 0)
            //    return nodes;

            foreach (myNode srcNode in srcNodes)
            {
                myNode node = srcNode;

                // a valid format entry file found, process
                if (node.chapter.parentId == parentId)
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
        // this method exports a node to common exported human readable format document.
        // this method does not export importable set or file. non-importable document.
        public static bool exportEntry(myConfig cfg, ref myNode? node, String path, bool useCustomPathFileName,
            Int64 exportIndex, EntryType OutputEntryType)
        {
            if (node == null)
                return false;

            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            entryMethods.getEntryTypeFormats(OutputEntryType, ref ext, ref extComplete, ref extSearchPattern);

            // 1st load chapter's data blob
            String rtf = entryMethods.DBLoadNodeData(cfg, node.chapter.Id, node.DirectorySectionID);

            String fileData = "";
            byte[]? fileDataBytes = null;
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

                case EntryType.Pdf:
                    fileDataBytes = pdfEntry.toPDF(rtf);
                    break;

                default:
                    break;
            }

            // make sure path is available
            if (path.Length <= 0)
                return false;

            String oldPath = path;
            if (!useCustomPathFileName)
            {
                // no custom file name given, so auto generate filename based on database node/entry attributes.
                String entryName = "";
                path = entryMethods.getFormattedPathFileName(path, node.chapter.Id, node.chapter.parentId, node.chapter.Title,
                    node.chapter.chapterDateTime, exportIndex, OutputEntryType, out entryName);
            }
            else
            {
                // custom path and filename have been given, so use them.
            }

            // make sure to mark the file path as long file name
            if (path.IndexOf(@"\\?\") < 0)
                path = @"\\?\" + path;

            if (OutputEntryType != EntryType.Pdf)
            {
                // export non pdf text file
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
            }
            else
            {
                // export pdf file
                try
                {
                    File.WriteAllBytes(path, fileDataBytes);
                }
                catch (Exception){ }
            }
            return true;
        }

        public static bool validateExtractEntryFile(String file, ref long index, ref DateTime chapterDate,
            ref String title, ref Int64 id, ref Int64 parentId)
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
                id = Int64.Parse(matches3[0].Groups[5].Value);
                parentId = Int64.Parse(matches3[0].Groups[7].Value);
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
            Int64 parentId = 0;
            Int64 id = 0;

            if (!validateExtractEntryFile(file, ref index, ref chapterDate, ref title, ref id, ref parentId))
                return false;

            chapter.chapterDateTime = chapterDate;
            chapter.Title = title;
            chapter.Id = id;
            chapter.parentId = parentId;
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
            Int64 parentId = 0;
            Int64 id = 0;

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
        // this method configures node's document width
        public static bool DBSetNodeDocumentWidth(myConfig cfg, ref myNode? node, int width)
        {
            if (node == null)
                return false; // error node not found

            // configure
            node.chapter.documentWidth = width;

            // now update the node
            return DBUpdateNode(cfg, ref node, "", false, false, true);
        }
        public static bool setEntryHighlightFontCompleteStrings(myConfig cfg, ref myNode? node,
            String HLFontColor,
            String HLBackColor,
            String HLFont)
        {
            return updateHLFont(cfg, ref node, HLFont, HLFontColor, HLBackColor);
        }
        public static bool setEntryHighlightFontComplete(myConfig cfg, ref myNode? node,
            Color highlightFontColor,
            Color highlightBackColor,
            Font highlightFont)
        {
            String HLFont = commonMethods.FontToString(highlightFont);
            String HLFontColor = commonMethods.ColorToString(highlightFontColor);
            String HLBackColor = commonMethods.ColorToString(highlightBackColor);
            return updateHLFont(cfg, ref node, HLFont, HLFontColor, HLBackColor);
        }
        public static bool setEntryHighlightFont(myConfig cfg, ref myNode? node, Color highlightFontColor, Font highlightFont)
        {
            String HLFont = commonMethods.FontToString(highlightFont);
            String HLFontColor = commonMethods.ColorToString(highlightFontColor);
            return updateHLFont(cfg, ref node, HLFont, HLFontColor, null);
        }
        public static bool setEntryClearHighlightFont(myConfig cfg, ref myNode? node)
        {
            String HLFont = "";
            String HLFontColor = "";
            return updateHLFont(cfg, ref node, HLFont, HLFontColor, null);
        }

        public static bool setEntryHighlightBackColor(myConfig cfg, ref myNode? node, Color highlightBackColor)
        {
            String HLBackColor = commonMethods.ColorToString(highlightBackColor);
            return updateHLBackColor(cfg, ref node, HLBackColor);
        }
        public static bool setEntryClearBackColor(myConfig cfg, ref myNode? node)
        {
            String HLBackColor = "";
            return updateHLBackColor(cfg, ref node, HLBackColor);
        }
        public static bool setEntryClearHighlight(myConfig cfg, ref myNode? node)
        {
            return setEntryHighlightFontCompleteStrings(cfg, ref node, "", "", "");
        }
        public static bool updateHLBackColor(myConfig cfg, ref myNode? node, String? HLBackColor = null)
        {
            if (node == null)
                return false; // error node not found

            // set properties/config
            if (HLBackColor != null)
                node.chapter.HLBackColor = HLBackColor;

            // update
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.updateNode(cfg.ctx1, ref node, "", false, false);
            else
                return SingleFileDB.updateNode(cfg.ctx0, ref node, "", false, false);
        }
        public static bool updateHLFont(myConfig cfg, ref myNode? node,
            String? HLFont = null, String? HLFontColor = null, String? HLBackColor = null)
        {
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
                return OpenFileSystemDB.updateNode(cfg.ctx1, ref node, "", false, false);
            else
                return SingleFileDB.updateNode(cfg.ctx0, ref node, "", false, false);
        }

        // clone the entry/node
        public static myNode? DBCloneNode(ref myConfig cfg, ref List<myNode> allNodes, ref myNode? node, Int64 locationId,
            bool writeDBIndexingFile = true, bool checkpoint = true, bool buildLineageList = true)
        {
            if (node == null)
                return null; // error node not found

            if (node.chapter == null)
                return null; // error node without chapter

            // validate
            if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                return null; // error 

            String rtf = "";
            myNode? clone = node.DeepCopy();
            rtf = entryMethods.DBLoadNodeData(cfg, node.chapter.Id, node.DirectorySectionID);

            // configure
            if (locationId != -1)
                clone.chapter.parentId = locationId;

            // finally clone the node
            if (entryMethods.DBCreateNode(ref cfg, ref clone, rtf, false, false, false, true, writeDBIndexingFile, checkpoint))
            {
                // add the clone into the global work list
                allNodes.Add(clone);
                cfg.totalNodes++;

                // rebuild lineage if required by user
                if (buildLineageList)
                    clone.lineage = entryMethods.findBottomToRootNodesRecursive(ref allNodes, ref clone, false, false, true, false);

                return clone;
            }
            else
            {
                // error creating node return error
                return null;
            }
        }

        // load new tree node with configuration

        public static TreeNode? InitializeNewTreeNode(ref myNode? node, Font defaultFont)
        {
            if (node == null) return null;
            if (node.chapter == null) return null;
            System.Drawing.Font? nodeFont = null;
            String path = String.Format(@"{0}", node.chapter.Id);
            String entryName = getEntryLabel(node, false);
            TreeNode newTreeNode = new TreeNode(entryName);
            newTreeNode.Name = path;
            loadNodeHighlight(newTreeNode, node, defaultFont, Color.White, Color.Black);
            return newTreeNode;                
        }

        // sets node state cursor position
        public static bool DBUpdateCaretConfig(ref myConfig cfg, ref List<myNode> allNodes, Int64 ID, Int32 caretIndex, Int32 careSelLength)
        {
            myNode? node = entryMethods.FindNodeInList(ref allNodes, ID);
            if (node == null)
                return false;

            return DBUpdateCaretConfig(ref cfg, ref node, caretIndex, careSelLength);
        }

        // sets node state cursor position
        public static bool DBUpdateCaretConfig(ref myConfig cfg, ref myNode? node, Int32 caretIndex, Int32 careSelLength)
        {
            if (node == null)
                return false; // error node not found

            if (caretIndex < 0) return false;
            if (careSelLength < 0) return false;

            // configure
            node.chapter.caretIndex = caretIndex;
            node.chapter.caretSelectionLength = careSelLength;

            // update
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.updateNode(cfg.ctx1, ref node, "", false, false);
            else
                return SingleFileDB.updateNode(cfg.ctx0, ref node, "", false, false);
        }

        // this method selects the db core and updates the node's title
        public static bool DBUpdateNodeTitle(myConfig cfg, ref myNode? node, String title)
        {
            if (node == null)
                return false; // error node not found

            // set properties/config
            node.chapter.Title = title;

            // update
            return DBUpdateNode(cfg, ref node, "", false);
        }
        // find all nodes by type and date
        public static List<myNode> findNodesByNodeTypeDate(ref List<myNode> allNodes,
            SpecialNodeType specialNodeType, NodeType nodeType,
            int year, int month, int day)
        {
            List<myNode> nodes = new List<myNode>();

            foreach (myNode listNode in allNodes)
            {
                myNode node = listNode;

                if (specialNodeType == SpecialNodeType.AnyOrAll)
                {
                    // process all nodes
                }
                else if (specialNodeType == SpecialNodeType.NonSystemNode)
                {
                    // user demands non system node type
                    if (node.chapter.specialNodeType == SpecialNodeType.SystemNode)
                        continue;
                }
                else if (specialNodeType == SpecialNodeType.SystemNode)
                {
                    // user demands system node type
                    if (node.chapter.specialNodeType != SpecialNodeType.SystemNode)
                        continue;
                }
                else if (specialNodeType == SpecialNodeType.None)
                {
                    // user demands non system node type
                    if (node.chapter.specialNodeType != SpecialNodeType.None)
                        continue;
                }

                switch (nodeType)
                {
                    case NodeType.Year:
                        if (node.chapter.nodeType == NodeType.Year && node.chapter.chapterDateTime.Year == year)
                            nodes.Add(node);

                        break;
                    case NodeType.Month:
                        if (node.chapter.nodeType == NodeType.Month && node.chapter.chapterDateTime.Year == year && node.chapter.chapterDateTime.Month == month)
                            nodes.Add(node);

                        break;
                    case NodeType.Entry:
                        if (node.chapter.nodeType == NodeType.Entry && node.chapter.chapterDateTime.Year == year && node.chapter.chapterDateTime.Month == month &&
                            node.chapter.chapterDateTime.Day == day)
                            nodes.Add(node);

                        break;
                    case NodeType.AnyOrAll:
                        if (node.chapter.chapterDateTime.Year == year && node.chapter.chapterDateTime.Month == month &&
                            node.chapter.chapterDateTime.Day == day)
                            nodes.Add(node);

                        break;
                    default:
                        break;
                }
            }
            return nodes;
        }
        // this method sets the highlights and font for a given tree node
        public static void loadNodeHighlight(TreeNode treeNode, myNode node, Font defaultFont, Color defaultBackColor, Color defaultForeColor)
        {
            String defaultFontString = commonMethods.FontToString(defaultFont);
            String defaultBCString = commonMethods.ColorToString(defaultBackColor);
            String defaultFCString = commonMethods.ColorToString(defaultForeColor);

            if ((node.chapter.HLFont.Length > 0 ) && (node.chapter.HLFont != defaultFontString))
                treeNode.NodeFont = commonMethods.StringToFont(node.chapter.HLFont);

            if ((node.chapter.HLFontColor.Length > 0) && (node.chapter.HLFontColor != defaultFCString))
                treeNode.ForeColor = commonMethods.StringToColor(node.chapter.HLFontColor);

            if ((node.chapter.HLBackColor.Length > 0) && (node.chapter.HLBackColor != defaultBCString))
                treeNode.BackColor = commonMethods.StringToColor(node.chapter.HLBackColor);
            
        }
        public static void setCalendarHighlightEntry(MonthCalendar CalendarEntries, DateTime dateTime)
        {
            DateTime day = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
            CalendarEntries.AddBoldedDate(day);
            CalendarEntries.UpdateBoldedDates();
        }

        // this method finds all nodes ordered by first parent and then it's children and so and so
        // and builds a treeview tree struture
        public static List<TreeNode> buildTreeViewTree(ref List<myNode> srcNodes, ref List<myNode> outTree, 
            Font defaultFont, bool addTreeNodes = true, bool nullmyNodeTag = true,
            bool sort = true, bool descending = false, MonthCalendar? CalendarEntries = null,
            bool insertDeletedTreeNode = false)
        {
            List<TreeNode> tree = new List<TreeNode>();
            outTree = new List<myNode>();
            Queue<TreeNode> queue = new Queue<TreeNode>();

            // system nodes are first to be indexed at index 0 before all the rest of nodes.
            List<myNode> rootNodes = new List<myNode>();
            rootNodes.AddRange(entryMethods.findRootNodes(ref srcNodes, SpecialNodeType.SystemNode, sort, descending));
            // non system nodes must exist after the system nodes.
            rootNodes.AddRange(entryMethods.findRootNodes(ref srcNodes, SpecialNodeType.NonSystemNode, sort, descending));

            // first enqueue all root nodes
            foreach (myNode rootNode in rootNodes)
            {
                myNode node = rootNode;
                String path = String.Format(@"{0}", node.chapter.Id);
                String entryName = getEntryLabel(node, false);
                TreeNode newTreeNode = new TreeNode(entryName);
                newTreeNode.Name = path;
                newTreeNode.Tag = node;

                if (CalendarEntries != null)
                    setCalendarHighlightEntry(CalendarEntries, node.chapter.chapterDateTime);

                if (!insertDeletedTreeNode)
                {
                    if (!node.chapter.IsDeleted)
                        tree.Add(newTreeNode);
                }
                else
                {
                    tree.Add(newTreeNode);
                }

                // add both common and deleted node into tree
                outTree.Add(node); // add this node in the output tree list.
                queue.Enqueue(newTreeNode);
            }
            // now build a perfect sequentially ordered queue of all parents first, then 2nd their children most recursively.
            // in the year loop, 1st all years are enqueued. and all months of each year are enqueued into the queue.
            // the 2nd is month loop. when all years and their months added, then all children of each month are enqueued.
            // this is most sequential and recursive layer by layer processing.
            while (queue.Count > 0)
            {
                TreeNode currentTreeNode = queue.Dequeue();
                if (currentTreeNode == null) continue;

                myNode currentNode = (myNode)currentTreeNode.Tag;

                // fetch this node's children
                List<myNode> children = entryMethods.findFirstLevelChildren(currentNode.chapter.Id, ref srcNodes, sort, descending);

                // 2nd in sequence is parent's children, so children are added 2nd to parent in sequence.
                foreach (myNode childNode in children)
                {
                    myNode node = childNode;
                    String path = String.Format(@"{0}", node.chapter.Id);
                    String entryName = getEntryLabel(node, false);
                    TreeNode newTreeNode = new TreeNode(entryName);
                    newTreeNode.Name = path;
                    newTreeNode.Tag = node;

                    if (addTreeNodes)
                    {
                        if (!insertDeletedTreeNode)
                        {
                            if (!node.chapter.IsDeleted)
                                currentTreeNode.Nodes.Add(newTreeNode);
                        }
                        else
                        {
                            currentTreeNode.Nodes.Add(newTreeNode);
                        }
                    }

                    outTree.Add(node); // add this node in the output tree list.
                    queue.Enqueue(newTreeNode);
                }

                // setup tree node icons
                if (currentNode.chapter.specialNodeType == SpecialNodeType.SystemNode)
                {
                    currentTreeNode.ImageIndex = 1;
                    currentTreeNode.SelectedImageIndex = 2;
                }

                // setup treeview icons
                if (currentNode.chapter.nodeType == NodeType.Year || currentNode.chapter.nodeType == NodeType.Month)
                {
                    currentTreeNode.ImageIndex = 3;
                    currentTreeNode.SelectedImageIndex = 3;
                }
                else if (currentNode.chapter.nodeType == NodeType.Set)
                {
                    currentTreeNode.ImageIndex = 4;
                    currentTreeNode.SelectedImageIndex = 4;
                }
                else if (currentNode.chapter.nodeType == NodeType.Label)
                {
                    currentTreeNode.ImageIndex = 5;
                    currentTreeNode.SelectedImageIndex = 5;
                }

                // setup calendar highlight
                if (CalendarEntries != null)
                    setCalendarHighlightEntry(CalendarEntries, currentNode.chapter.chapterDateTime);

                // highlight
                loadNodeHighlight(currentTreeNode, currentNode, defaultFont, Color.Black, Color.White);

                // null the processed tree node's tag so that the resource is released and memory freed.
                if (nullmyNodeTag) currentTreeNode.Tag = null;
            }

            return tree;
        }

        // this method finds all nodes ordered by first parent and then it's children and so and so
        public static List<myNode> findAllNodesTreeSequence(ref List<myNode> srcNodes, 
            bool sort = true, bool descending = false)
        {
            List<myNode> nodes = new List<myNode>();
            Queue<myNode> queue = new Queue<myNode>();

            // system nodes are first to be indexed at index 0 before all the rest of nodes.
            List<myNode> rootNodes = new List<myNode>();
            rootNodes.AddRange(entryMethods.findRootNodes(ref srcNodes, SpecialNodeType.SystemNode, sort, descending));
            // non system nodes must exist after the system nodes.
            rootNodes.AddRange(entryMethods.findRootNodes(ref srcNodes, SpecialNodeType.NonSystemNode, sort, descending));

            // first enqueue all root nodes
            foreach (myNode rootNode in rootNodes)
                queue.Enqueue(rootNode);

            // now build a perfect sequentially ordered queue of all parents first, then 2nd their children most recursively.
            // in the year loop, 1st all years are enqueued. and all months of each year are enqueued into the queue.
            // the 2nd is month loop. when all years and their months added, then all children of each month are enqueued.
            // this is most sequential and recursive layer by layer processing.
            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                if (currentNode == null)
                    continue;

                // fetch this node's children
                List<myNode> children = entryMethods.findFirstLevelChildren(currentNode.chapter.Id, ref srcNodes, sort, descending);

                // 2nd in sequence is parent's children, so children are added 2nd to parent in sequence.
                foreach (myNode childNode in children)
                    queue.Enqueue(childNode);

                // 1st in sequence is the parent node, so parent node is added 1st in sequence before all children.
                nodes.Add(currentNode);
            }
            return nodes;
        }

        // this method automatically loads system calendar nodes. if they do not exist, the method creates them and loads them.
        public static bool initCalenderNodesSystem(myConfig cfg, ref List<myNode> allNodes, int year, int month, ref myNode? yearNodeOut, ref myNode? monthNodeOut,
            ref mySystemNodes? systemNodesOut)
        {
            // auto create/load system nodes
            mySystemNodes? systemNodes = null;
            if (!entryMethods.autoCreateLoadSystemNodes(ref cfg, ref allNodes, out systemNodes))
                return false;

            systemNodesOut = systemNodes;

            // now create Year Node direct in DB if it doesn't exists. else load it from db for linking
            List<myNode>? yearNodes = findNodesByNodeTypeDate(ref systemNodes.YearNodes,
                SpecialNodeType.SystemNode, NodeType.Year,
                year, month, -1);

            myNode? yearNode = null;
            if (yearNodes.Count() <= 0)
            {
                // this year doesn't exists in db, so create it.
                DateTime yearDateTime = new DateTime(year, 1, 1, 0, 0, 0, 0);
                String yearLabel = yearDateTime.ToString("yyyy");
                yearNode = new myNode(true);
                yearNode.chapter.nodeType = NodeType.Year;
                yearNode.chapter.specialNodeType = SpecialNodeType.SystemNode;
                yearNode.chapter.parentId = systemNodes.getSystemNode(NodeType.Journal).chapter.Id;
                yearNode.chapter.chapterDateTime = yearDateTime;
                yearNode.chapter.Title = yearLabel;
                if (!DBCreateNode(ref cfg, ref yearNode, "", true, true, true, true, true, true))
                    return false; // error

                // add into context session work list
                allNodes.Add(yearNode);
            }
            else
            {
                // this year node already exists, so directly load it to the object.
                yearNode = yearNodes[0];
            }

            // now create Month Node direct in DB if it doesn't exists. else load it from db for linking.
            // note Month node's parent must be the year node.
            List<myNode>? monthNodes = findNodesByNodeTypeDate(ref systemNodes.MonthNodes,
                SpecialNodeType.SystemNode, NodeType.Month,
                year, month, -1);

            myNode? monthNode = null;
            if (monthNodes.Count() <= 0)
            {
                // this month in the current chapter's year doesn't exists in db, so create it.
                DateTime monthDateTime = new DateTime(year, month, 1, 0, 0, 0, 0);
                String monthLabel = monthDateTime.ToString("MMMM");
                monthNode = new myNode(true);
                monthNode.chapter.nodeType = NodeType.Month;
                monthNode.chapter.specialNodeType = SpecialNodeType.SystemNode;
                monthNode.chapter.parentId = yearNode.chapter.Id;
                monthNode.chapter.chapterDateTime = monthDateTime;
                monthNode.chapter.Title = monthLabel;
                if (!DBCreateNode(ref cfg, ref monthNode, "", true, true, true, true, true, true))
                    return false; // error

                // add into context session work list
                allNodes.Add(monthNode);
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

        // this method automatically loads calendar nodes. if they do not exist, the method creates them and loads them.
        public static bool initCalenderNodesNonSystemSet(myConfig cfg, ref List<myNode> allNodes,
            ref myNode setNode, int year, int month, out myNode? yearNodeOut, out myNode? monthNodeOut, 
            bool writeDBIndexingFile = true)
        {
            // now create Year Node direct in DB if it doesn't exists. else load it from db for linking
            List<myNode>? yearNodes = null;
            yearNodes = findNodesByNodeTypeDate(ref allNodes, SpecialNodeType.None, NodeType.Year, year, month, -1);

            myNode? yearNode = null;
            if (yearNodes.Count() <= 0)
            {
                // this year doesn't exists in db, so create it.
                DateTime yearDateTime = new DateTime(year, 1, 1, 0, 0, 0, 0);
                String yearLabel = yearDateTime.ToString("yyyy");
                yearNode = DBNewNode(ref cfg,
                    SpecialNodeType.None, NodeType.Year, DomainType.Journal,
                    ref yearNode, true, true, true, yearDateTime, setNode.chapter.Id, true, yearLabel, "",
                    true, writeDBIndexingFile);

                // add the newly created node in the realtime context session work list so that it is identified throughout session.
                allNodes.Add(yearNode);
            }
            else
            {
                // this year node already exists, so directly load it to the object.
                yearNode = yearNodes[0];
            }

            // now create Month Node direct in DB if it doesn't exists. else load it from db for linking.
            // note Month node's parent must be the year node.
            List<myNode>? monthNodes = null;
            monthNodes = findNodesByNodeTypeDate(ref allNodes, SpecialNodeType.None, NodeType.Month, year, month, -1);

            myNode? monthNode = null;
            if (monthNodes.Count() <= 0)
            {
                // this month in the current chapter's year doesn't exists in db, so create it.
                DateTime monthDateTime = new DateTime(year, month, 1, 0, 0, 0, 0);
                String monthLabel = monthDateTime.ToString("MMMM");
                monthNode = DBNewNode(ref cfg,
                    SpecialNodeType.None, NodeType.Month, DomainType.Journal,
                    ref monthNode, true, true, true, monthDateTime, yearNode.chapter.Id, true, monthLabel,
                    "", true, writeDBIndexingFile);

                // add the newly created node in the realtime context session work list so that it is identified throughout session.
                allNodes.Add(monthNode);

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
            
            if (nodes.Count() <= 0)
                return;

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

        public static void sortNodes(ref List<myNode> nodes, bool sortById, bool sortByIdExtra,
            bool sortByCommonDateTime, bool sortByCreationDateTime, bool sortByModificationDateTime,
            bool descending = false)
        {
            // descending = latest/top/max/last to earliest/bottom/least/first (-)
            // ascending = earliest/bottom/least/first to latest/top/max/last (+)

            if (nodes.Count() <= 0)
                return;

            if (sortById)
            {
                nodes = nodes.Select(d => new
                {
                    d.chapter.Id,
                    x = d
                })
                .Distinct()
                .OrderBy(d => d.Id)
                .Select(d => d.x).ToList();
            }
            else if (sortByIdExtra)
            {
                // sort by id

                if (sortByCommonDateTime)
                {
                    nodes = nodes.Select(d => new
                    {
                        d.chapter.Id,
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
                    .OrderBy(d => d.Id)
                    .ThenBy(d => d.Year)
                    .ThenBy(d => d.Month)
                    .ThenBy(d => d.Day)
                    .ThenBy(d => d.Hour)
                    .ThenBy(d => d.Minute)
                    .ThenBy(d => d.Second)
                    .ThenBy(d => d.Millisecond)
                    .Select(d => d.x).ToList();
                }
                else if (sortByCreationDateTime)
                {
                    nodes = nodes.Select(d => new
                    {
                        d.chapter.Id,
                        d.chapter.creationDateTime.Year,
                        d.chapter.creationDateTime.Month,
                        d.chapter.creationDateTime.Day,
                        d.chapter.creationDateTime.Hour,
                        d.chapter.creationDateTime.Minute,
                        d.chapter.creationDateTime.Second,
                        d.chapter.creationDateTime.Millisecond,
                        x = d
                    })
                    .Distinct()
                    .OrderBy(d => d.Id)
                    .ThenBy(d => d.Year)
                    .ThenBy(d => d.Month)
                    .ThenBy(d => d.Day)
                    .ThenBy(d => d.Hour)
                    .ThenBy(d => d.Minute)
                    .ThenBy(d => d.Second)
                    .ThenBy(d => d.Millisecond)
                    .Select(d => d.x).ToList();
                }
                else if (sortByModificationDateTime)
                {
                    nodes = nodes.Select(d => new
                    {
                        d.chapter.Id,
                        d.chapter.modificationDateTime.Year,
                        d.chapter.modificationDateTime.Month,
                        d.chapter.modificationDateTime.Day,
                        d.chapter.modificationDateTime.Hour,
                        d.chapter.modificationDateTime.Minute,
                        d.chapter.modificationDateTime.Second,
                        d.chapter.modificationDateTime.Millisecond,
                        x = d
                    })
                    .Distinct()
                    .OrderBy(d => d.Id)
                    .ThenBy(d => d.Year)
                    .ThenBy(d => d.Month)
                    .ThenBy(d => d.Day)
                    .ThenBy(d => d.Hour)
                    .ThenBy(d => d.Minute)
                    .ThenBy(d => d.Second)
                    .ThenBy(d => d.Millisecond)
                    .Select(d => d.x).ToList();
                }

            }
            else
            {
                // do not sort by id

                if (sortByCommonDateTime)
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
                }
                else if (sortByCreationDateTime)
                {
                    nodes = nodes.Select(d => new
                    {
                        d.chapter.creationDateTime.Year,
                        d.chapter.creationDateTime.Month,
                        d.chapter.creationDateTime.Day,
                        d.chapter.creationDateTime.Hour,
                        d.chapter.creationDateTime.Minute,
                        d.chapter.creationDateTime.Second,
                        d.chapter.creationDateTime.Millisecond,
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
                }
                else if (sortByModificationDateTime)
                {
                    nodes = nodes.Select(d => new
                    {
                        d.chapter.modificationDateTime.Year,
                        d.chapter.modificationDateTime.Month,
                        d.chapter.modificationDateTime.Day,
                        d.chapter.modificationDateTime.Hour,
                        d.chapter.modificationDateTime.Minute,
                        d.chapter.modificationDateTime.Second,
                        d.chapter.modificationDateTime.Millisecond,
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
                }

            }

            if (descending)
                nodes.Reverse();

        }

        // this method is universal, clones from source db to destination db.
        public static bool CloneDB(FrmJournal? parentForm,
            ref myConfig cfgSrc, ref myConfig cfgDest, DatabaseType srcDBType, DatabaseType destDBType,
            bool loadOperationForm = false, bool reindex = false)
        {
            // operations status form
            FormOperation? formOperation = null;
            if (loadOperationForm)
                formOperation = FormOperation.showForm(parentForm, "please wait. doing operation...", 0, 100, 0, 0);

            // collect all nodes from source
            List<myNode>? allNodes = entryMethods.DBFindAllNodes(cfgSrc, false, false);

            // first get the total number of chapters which exist in db
            long total = allNodes.LongCount();
            long index = 0;

            // load tree document object model structure
            long nodeIndex = 0;
            myTreeDom treeDom = new myTreeDom();
            treeDom.buildTree(ref allNodes, true, false);

            if (reindex)
            {
                // user demands reindex the destination db
                treeDom.reindexTree(ref nodeIndex);
            }
            
            List<myTreeDomNode > tree = treeDom.ToList();
            foreach (myTreeDomNode listedNode in tree)
            {
                // load the source node
                myNode? node = listedNode.self;

                String rtf = "";
                if (reindex)
                    rtf = entryMethods.DBLoadNodeData(cfgSrc, listedNode.previousID, listedNode.self.DirectorySectionID);
                else
                    rtf = entryMethods.DBLoadNodeData(cfgSrc, listedNode.self.chapter.Id, listedNode.self.DirectorySectionID);

                // finally write the node and it's data into destination
                entryMethods.DBCreateNode(ref cfgDest, ref node, rtf, false, false, false, false, false, false);

                // update ui
                if (loadOperationForm)
                {
                    formOperation.updateProgressBar(index, total);
                    formOperation.updateFilesStatus(index, total);
                }

                // update
                index++;
            }

            // done cloning

            // checkpoint
            entryMethods.DBCheckpoint(ref cfgDest);

            // because this is direct to direct clone, so we just copy everything 1:1
            // direct copy the indexing from source to destination db
            if (!reindex)
                DBCopyIndexingIndex(ref cfgSrc, ref cfgDest, srcDBType, destDBType);
            else
                DBSetIndexingIndex(ref cfgDest, destDBType, nodeIndex); // not 1:1 cloning, so reindexing has been done, so we set the final processed index.

            // finally update the destination db index
            entryMethods.DBWriteIndexing(ref cfgDest);

            // close db
            cfgSrc.close();
            cfgDest.close();

            if (loadOperationForm)
                formOperation.close();

            // done
            return true;
        }

        // this method is universal, exports a set from source db
        public static bool ExportSet(FrmJournal? parentForm, ref myConfig cfgSrc, ref List<myNode> allNodes, ref List<myNode> parentNodes, String dbName, String dstPath,
            DatabaseType destDBType, bool loadOperationForm = false)
        {
            myConfig cfgDest = new myConfig();
            if (allNodes.Count == 0) return false;
            if (parentNodes.Count == 0) return false;

            switch (destDBType)
            {
                case DatabaseType.OpenFSDB:
                    {
                        cfgDest.radCfgUseSingleFileDB = false;
                        cfgDest.radCfgUseOpenFileSystemDB = true;
                        if (!OpenFileSystemDB.CreateLoadDB(dstPath, dbName, ref cfgDest.ctx1, true, true))
                            return false;

                        break;
                    }
                case DatabaseType.SingleFileDB:
                    {
                        cfgDest.radCfgUseSingleFileDB = true;
                        cfgDest.radCfgUseOpenFileSystemDB = false;
                        if (!SingleFileDB.CreateLoadDB(dstPath, dbName, ref cfgDest.ctx0, true, true))
                            return false;

                        break;
                    }
                default:
                    {
                        return false;
                    }
            }

            // operations status form
            FormOperation? formOperation = null;
            if (loadOperationForm)
                formOperation = FormOperation.showForm(parentForm, "please wait. doing operation...", 0, 100, 0, 0);

            long index = 0;

            // first we need to create a set node. we cannot export without a new set node
            // 1 is preset as set node's id. set node is 1st to be indexed before all nodes most recursively.
            myNode? setNode = entryMethods.createSetNode(ref index, dbName, DateTime.Now);

            // create set node entry in the destination
            DBCreateNode(ref cfgDest, ref setNode, "", false, false, false, false, true, true);

            // load tree document object model structure
            myTreeDom treeDom = new myTreeDom();
            treeDom.buildCustomTree(ref allNodes, ref parentNodes, true, false);
            // nullify the entire tree with nulled non-db relative indexing.
            // we cannot use db indexing in an exported set.
            treeDom.reindexTree(ref index);
            treeDom.applySetNode(setNode.chapter.Id);
            List<myTreeDomNode> tree = treeDom.ToList();
            index = 0; // reset for using as a progress counter

            // get the total number of nodes to export
            long total = tree.LongCount();

            foreach (myTreeDomNode listedNode in tree)
            {
                // load the rtf from current node
                myNode? node = listedNode.self;
                String rtf = "";
                rtf = entryMethods.DBLoadNodeData(cfgSrc, listedNode.previousID, listedNode.self.DirectorySectionID);

                // finally write the node and it's data into destination
                entryMethods.DBCreateNode(ref cfgDest, ref node, rtf, false, false, false, false, false, false);

                // update ui
                if (loadOperationForm)
                {
                    formOperation.updateProgressBar(index, total);
                    formOperation.updateFilesStatus(index, total);
                }

                // update
                index++;
            }

            // done

            // checkpoint
            entryMethods.DBCheckpoint(ref cfgDest);

            // finally update the db index in file.
            entryMethods.DBWriteIndexing(ref cfgDest);

            // now close the database
            cfgDest.close();

            if (loadOperationForm)
                formOperation.close();

            // done
            return true;
        }

        // this method is universal, imports a set from source db
        public static bool ImportSet(FrmJournal? parentForm, ref myConfig cfgDest, ref List<myNode> allNodes,
            String srcPath, String dbName, DatabaseType srcDBType, bool loadOperationForm = false)
        {
            myConfig cfgSrc = new myConfig();

            // load source db
            switch (srcDBType)
            {
                case DatabaseType.OpenFSDB:
                    {
                        cfgSrc.radCfgUseSingleFileDB = false;
                        cfgSrc.radCfgUseOpenFileSystemDB = true;
                        if (!OpenFileSystemDB.CreateLoadDB(srcPath, "", ref cfgSrc.ctx1, false, false))
                            return false;

                        break;
                    }
                case DatabaseType.SingleFileDB:
                    {
                        cfgSrc.radCfgUseSingleFileDB = true;
                        cfgSrc.radCfgUseOpenFileSystemDB = false;
                        if (!SingleFileDB.CreateLoadDB(srcPath, "", ref cfgSrc.ctx0, false, false))
                            return false;

                        break;
                    }
                default:
                    {
                        return false;
                    }
            }

            // operations status form
            FormOperation? formOperation = null;
            if (loadOperationForm)
                formOperation = FormOperation.showForm(parentForm, "please wait. doing operation...", 0, 100, 0, 0);

            // collect all tree list from source db
            List<myNode>? srcNodes = DBFindAllNodes(cfgSrc, false, false);

            // build tree from source nodes
            myTreeDom treeDom = new myTreeDom();
            treeDom.buildTree(ref srcNodes, true, false);

            // we import the set with an additional new clone set node so that
            // the original export set node and it's date and time and settings remain preserved upon import.
            myNode? setNode = entryMethods.createSetNode(ref cfgDest, dbName, DateTime.Now);

            // create set node entry in the destination
            entryMethods.DBCreateNode(ref cfgDest, ref setNode, "", false, false, false, false, true, true);
            
            // we must nullify system node type
            // we import clones, not anything original. so system node type cannot be used.
            treeDom.nullSpecialNodeType();

            // only a clone shall be imported. one and same set cannot be imported repeatedly because it
            // corrupts the db. therefore a set always shall be imported as a new clone.
            // it is user's own effort to manage the imported entries.
            // prepare the tree with proper current db's indexing
            if (cfgDest.radCfgUseOpenFileSystemDB)
                treeDom.reindexTree(ref cfgDest.ctx1.dbIndexing.currentDBIndex);
            else
                treeDom.reindexTree(ref cfgDest.ctx0.dbIndexing.currentDBIndex);

            // apply clone set node
            treeDom.applySetNode(setNode.chapter.Id);

            // create a newly changed tree list from the tree dom structure.
            List<myTreeDomNode> tree = treeDom.ToList();
            List<myNode> setList = new List<myNode>();
            long total = tree.LongCount();
            long index = 0;

            // now direct import
            foreach (myTreeDomNode listedNode in tree)
            {
                // load source node
                String rtf = "";
                rtf = DBLoadNodeData(cfgSrc, listedNode.previousID, listedNode.self.DirectorySectionID);

                // write destination node
                // clone set node is created with latest creation date. all other config and data is 1:1 cloned except the id.
                if (!entryMethods.DBCreateNode(ref cfgDest, ref listedNode.self, rtf, true, false, false, false, false, false))
                    continue;

                // add newly created node to set's list
                setList.Add(listedNode.self);

                // update ui
                if (loadOperationForm)
                {
                    formOperation.updateProgressBar(index, total);
                    formOperation.updateFilesStatus(index, total);
                }

                // update
                index++;
            }

            // done

            // checkpoint
            entryMethods.DBCheckpoint(ref cfgDest);

            // finally update the db index in file.
            entryMethods.DBWriteIndexing(ref cfgDest);

            // now first add set node
            allNodes.Add(setNode);

            // now add all set list into the global session work list
            allNodes.AddRange(setList);

            // now close the database
            cfgSrc.close();

            if (loadOperationForm)
                formOperation.close();

            // done
            return true;

        }

        // this function customizes the nodes or resets them to default properties
        public static bool DBCustomizeTreeNodesRecursive(ref myConfig cfg, ref List<myNode> nodes, bool set, bool setFont, bool setFontSize, bool setItalics, bool setBold, bool setStrikeout,
            bool setUnderline, bool setBackColor, bool setForeColor, Color backColor, Color foreColor, String fontName = "", float size = -1)
        {
            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return false;

            // phase 2: format all nodes

            foreach (myNode? node in nodes)
            {
                // reformat the node

                myNode? listedNode = node;
                System.Drawing.Font? font = commonMethods.StringToFont(listedNode.chapter.HLFont);
                if (font == null)
                    font = cfg.tvEntriesFont;

                if (fontName == "")
                    fontName = font.Name;

                if (size <= 0)
                    size = font.Size;

                if (set)
                {
                    // customize to custom properties

                    if (setFont)
                        font = CustomFontDialog.getNewFontWithStyle(fontName, font.Size, font.Bold, font.Italic, font.Strikeout, font.Underline);

                    if (setFontSize)
                        font = CustomFontDialog.getNewFontWithStyle(font, size, font.Bold, font.Italic, font.Strikeout, font.Underline);

                    if (setBold)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, true, font.Italic, font.Strikeout, font.Underline);

                    if (setItalics)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, font.Bold, true, font.Strikeout, font.Underline);

                    if (setStrikeout)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, font.Bold, font.Italic, true, font.Underline);

                    if (setUnderline)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, font.Bold, font.Italic, font.Strikeout, true);
                }
                else
                {
                    // reset to default values

                    if (setFontSize)
                        size = cfg.tvEntriesFont.Size;

                    if (setFont)
                        font = CustomFontDialog.getNewFontWithStyle(cfg.tvEntriesFont, font.Size, font.Bold, font.Italic, font.Strikeout, font.Underline);

                    if (setFontSize)
                        font = CustomFontDialog.getNewFontWithStyle(font, size, font.Bold, font.Italic, font.Strikeout, font.Underline);

                    if (setBold)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, false, font.Italic, font.Strikeout, font.Underline);

                    if (setItalics)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, font.Bold, false, font.Strikeout, font.Underline);

                    if (setStrikeout)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, font.Bold, font.Italic, false, font.Underline);

                    if (setUnderline)
                        font = CustomFontDialog.getNewFontWithStyle(font, font.Size, font.Bold, font.Italic, font.Strikeout, false);
                }

                // update node
                listedNode.chapter.HLFont = commonMethods.FontToString(font);

                // now configure colors

                if (backColor == Color.Empty)
                    backColor = cfg.tvEntriesBackColor;

                if (foreColor == Color.Empty)
                    foreColor = cfg.tvEntriesForeColor;

                if (setBackColor)
                    listedNode.chapter.HLBackColor = commonMethods.ColorToString(backColor);

                if (setForeColor)
                    listedNode.chapter.HLFontColor = commonMethods.ColorToString(foreColor);

                // finally update the node in db

                entryMethods.DBUpdateNode(cfg, ref listedNode, "", false, false, false);
            }

            // phase 3: completion

            // commit all
            entryMethods.DBWriteConfig(ref cfg);
            entryMethods.DBCheckpoint(ref cfg);
            entryMethods.DBWriteIndexing(ref cfg);
            return true;
        }

        public static bool DBNullOrEmptyNodes(ref myConfig cfg, ref List<myNode> nodes, out long processed, FormOperation? formop = null)
        {
            processed = 0;

            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return false;

            // first get the total number of chapters which exist in db
            long allNodesCount = nodes.LongCount();
            long index = 0;

            foreach (myNode? listedNode in nodes)
            {
                myNode? node = listedNode;

                node.chapter.HLFont = "";//commonMethods.FontToString(tvEntries.Font);
                node.chapter.HLFontColor = "";//commonMethods.ColorToString(Color.Black);
                node.chapter.HLBackColor = "";// commonMethods.ColorToString(Color.White);

                entryMethods.DBUpdateNode(cfg, ref node, "", false, false, false);

                if (formop != null)
                {
                    formop.updateProgressBar(index, allNodesCount);
                    formop.updateFilesStatus(index, allNodesCount);
                }
                index++;
            }

            // checkpoint
            entryMethods.DBCheckpoint(ref cfg);

            // finally update the db index in file.
            entryMethods.DBWriteIndexing(ref cfg);

            processed = index;
            return true;
        }
        public static bool DBFixUpgradeOldDB(ref myConfig cfg, out long processed, FormOperation? formop = null)
        {
            processed = 0;

            if (!cfg.ctx0.isDBOpen() && !cfg.ctx1.isDBOpen())
                return false;

            // collect all nodes from source
            List<myNode>? allNodes = entryMethods.DBFindAllNodes(cfg, false, false);

            // first get the total number of chapters which exist in db
            long total = allNodes.LongCount();
            long index = 0;

            foreach (myNode? listedNode in allNodes)
            {
                myNode? node = listedNode;

                entryMethods.DBUpdateNode(cfg, ref node, "", false, false, false);

                if (formop != null)
                {
                    formop.updateProgressBar(index, total);
                    formop.updateFilesStatus(index, total);
                }
                index++;
            }

            // checkpoint
            entryMethods.DBCheckpoint(ref cfg);

            // finally update the db index in file.
            entryMethods.DBWriteIndexing(ref cfg);

            processed = index;
            return true;
        }

        // this method deletes the loaded db
        public static bool DBDeleteDatabase(ref myConfig cfg)
        {
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.deleteDB(ref cfg.ctx1);
            else if (cfg.radCfgUseSingleFileDB)
                return SingleFileDB.deleteDB(ref cfg.ctx0);

            return true;
        }

        // this method copies or moves the loaded db
        public static bool DBCopyDatabase(ref myConfig cfg, String dest, bool copy, bool overwrite)
        {
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.CopyDB(ref cfg.ctx1, dest, copy, overwrite);
            else if (cfg.radCfgUseSingleFileDB)
                return SingleFileDB.CopyDB(ref cfg.ctx0, dest, copy, overwrite);

            return true;
        }

        public static myConfig? DBSelectOpenLoadDB(DatabaseType dbType, ref String outDBName, Form? parentForm = null)
        {
            myConfig cfg = new myConfig();

            // load source db and setup myConfig
            switch (dbType)
            {
                case DatabaseType.OpenFSDB:
                    {
                        FolderBrowserDialog browseFolder = new FolderBrowserDialog();
                        if (browseFolder.ShowDialog(parentForm) != DialogResult.OK)
                            return null;

                        cfg.radCfgUseSingleFileDB = false;
                        cfg.radCfgUseOpenFileSystemDB = true;
                        if (!OpenFileSystemDB.CreateLoadDB(browseFolder.SelectedPath, "", ref cfg.ctx1, false, false))
                            return null;

                        outDBName = cfg.ctx1.dbName;

                        break;
                    }
                case DatabaseType.SingleFileDB:
                    {
                        OpenFileDialog ofdDB = new OpenFileDialog();
                        ofdDB.Filter = "database files *.db|*.db";
                        ofdDB.FilterIndex = 1;
                        if (ofdDB.ShowDialog(parentForm) != DialogResult.OK)
                            return null;

                        cfg.radCfgUseSingleFileDB = true;
                        cfg.radCfgUseOpenFileSystemDB = false;
                        if (!SingleFileDB.CreateLoadDB(ofdDB.FileName, "", ref cfg.ctx0, false, false))
                            return null;

                        outDBName = cfg.ctx0.dbName;

                        break;
                    }
                default:
                    {
                        return null;
                    }
            }
            return cfg;
        }

        public static myConfig? DBSelectOpenLoadDestinationDB(DatabaseType dbType, String dbName, Form? parentForm = null,
            bool create = true, bool overwrite = true)
        {
            myConfig cfg = new myConfig();

            // destination clone db
            switch (dbType)
            {
                case DatabaseType.OpenFSDB:
                    FolderBrowserDialog browseFolder = new FolderBrowserDialog();
                    if (browseFolder.ShowDialog(parentForm) != DialogResult.OK)
                        return null;

                    cfg.radCfgUseSingleFileDB = false;
                    cfg.radCfgUseOpenFileSystemDB = true;
                    if (!OpenFileSystemDB.CreateLoadDB(browseFolder.SelectedPath, dbName, ref cfg.ctx1, overwrite, create))
                    {
                        cfg.close();
                        return null;
                    }

                    break;

                case DatabaseType.SingleFileDB:
                    SaveFileDialog sfdDB = new SaveFileDialog();
                    sfdDB.Filter = "database files *.db|*.db";
                    sfdDB.FilterIndex = 1;
                    if (sfdDB.ShowDialog(parentForm) != DialogResult.OK)
                        return null;

                    cfg.radCfgUseSingleFileDB = true;
                    cfg.radCfgUseOpenFileSystemDB = false;
                    if (!SingleFileDB.CreateLoadDB(sfdDB.FileName, dbName, ref cfg.ctx0, overwrite, create))
                    {
                        cfg.close();
                        return null;
                    }

                    break;
                default:
                    return null;
            }
            return cfg;
        }

    }
}
