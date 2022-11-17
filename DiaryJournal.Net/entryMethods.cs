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

        public static String getEntryLabel(myNode node)
        {
            String entryName = "";
            switch (node.chapter.nodeType)
            {
                case NodeType.JournalNode:
                    entryName = String.Format(@"{0}", mySystemNodes.JournalSystemNodeName);
                    break;

                case NodeType.LibraryNode:
                    entryName = String.Format(@"{0}", mySystemNodes.LibrarySystemNodeName);
                    break;

                case NodeType.LabelNode:
                    entryName = String.Format(@"{0}", node.chapter.Title);
                    break;

                case NodeType.NonCalendarEntryNode:
                    entryName = String.Format(@"{0}", node.chapter.Title);
                    break;

                case NodeType.SetNode:
                    entryName = String.Format(@"CloneSet:({0}):({1}):({2}:{3}:{4})", node.chapter.Title,
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
                    entryName = String.Format(@"({0}):({1}:{2}:{3}):({4})", node.chapter.chapterDateTime.ToString("dd-MM-yyyy"),
                        node.chapter.chapterDateTime.Hour, node.chapter.chapterDateTime.Minute,
                        node.chapter.chapterDateTime.Second, node.chapter.Title);
                    break;

                default:
                    break;

            }
            return entryName;
        }

        // this method auto loads system nodes. if they don't exist, the method auto creates them and loads them
        public static bool autoCreateLoadSystemNodes(ref myConfig cfg, ref List<myNode> allNodes, out mySystemNodes? systemNodesOut)
        {
            // first try to find and load system nodes
            mySystemNodes systemNodes = new mySystemNodes();
            List<myNode> collection = findNodesByTypes(ref allNodes, SpecialNodeType.SystemNode, NodeType.AnyOrAll, true, false);
            if (collection.Count() <= 0) // system nodes do not exist, so create them
            {
                if (!createSystemNodes(ref cfg, ref systemNodes))
                {
                    systemNodesOut = null;
                    return false;
                }

                // successfully created system nodes, add them to prebuilt list
                collection = findSystemNodesList(ref systemNodes);
                allNodes.AddRange(collection);
            }
            else
            {
                // system nodes already exist, successfully loaded system nodes
                // load all other system nodes such as year and month nodes if they exist from the source list
                loadSystemNodesCollection(ref collection, ref systemNodes);
            }
            // return output
            systemNodesOut = systemNodes;
            return true;
        }
        // this method creates and returns a list with all existent system nodes
        public static List<myNode> findSystemNodesList(ref mySystemNodes systemNodes)
        {
            List<myNode> list = new List<myNode>();

            if (systemNodes == null)
                return list;

            list.Add(systemNodes.JournalSystemNode);
            list.Add(systemNodes.LibrarySystemNode);
            list.AddRange(systemNodes.YearNodes);
            list.AddRange(systemNodes.MonthNodes);
            return list;
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
        public static void loadSystemNodesCollection(ref List<myNode> list, ref mySystemNodes systemNodes)
        {
            // reset
            systemNodes.YearNodes.Clear();
            systemNodes.MonthNodes.Clear();

            // now find items and fill in appropirate places
            foreach (myNode node in list)
            {
                if (node.chapter.specialNodeType != SpecialNodeType.SystemNode)
                    continue;

                if (node.chapter.nodeType == NodeType.JournalNode)
                    systemNodes.JournalSystemNode = node;
                else if (node.chapter.nodeType == NodeType.LibraryNode)
                    systemNodes.LibrarySystemNode = node;
                else if (node.chapter.nodeType == NodeType.YearNode)
                    systemNodes.YearNodes.Add(node);
                else if (node.chapter.nodeType == NodeType.MonthNode)
                    systemNodes.MonthNodes.Add(node);
            }
        }
        // this method creates new system nodes
        public static bool createSystemNodes(ref myConfig cfg, ref mySystemNodes systemNodes)
        {
            // journal node
            myNode journalNode = new myNode(true);
            journalNode.chapter.specialNodeType = SpecialNodeType.SystemNode;
            journalNode.chapter.nodeType = NodeType.JournalNode;
            journalNode.chapter.chapterDateTime = DateTime.Now;
            journalNode.chapter.Title = mySystemNodes.JournalSystemNodeName;

            // library node
            myNode libraryNode = new myNode(true);
            libraryNode.chapter.specialNodeType = SpecialNodeType.SystemNode;
            libraryNode.chapter.nodeType = NodeType.LibraryNode;
            libraryNode.chapter.chapterDateTime = DateTime.Now;
            libraryNode.chapter.Title = mySystemNodes.LibrarySystemNodeName;

            // finally create nodes in db

            // create journal node
            if (!DBCreateNode(ref cfg, ref journalNode, "", true))
                return false;

            // create library node
            if (!DBCreateNode(ref cfg, ref libraryNode, "", true))
                return false;

            // output
            systemNodes.JournalSystemNode = journalNode;
            systemNodes.LibrarySystemNode = libraryNode;
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
        public static bool DBCreateNode(ref myConfig cfg, ref myNode node, String rtf, bool newID = true,
            bool writeDBIndexingFile = true, bool checkpoint = true)
        {
            bool result = false;
            if (cfg.radCfgUseOpenFileSystemDB)
                result = OpenFileSystemDB.createNode(cfg.ctx1, ref node, rtf, newID);
            else
                result = SingleFileDB.createNode(cfg.ctx0, ref node, rtf, newID, checkpoint);

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
            SpecialNodeType specialNodeType, NodeType nodeType, DomainType domainType,
            ref myNode? initialNode, DateTime nodeDateTime = default(DateTime), Int64 parentId = 0, bool DBImport = true,
            String title = "", String rtf = "", bool newID = true, bool writeDBIndexingFile = true,
             bool checkpoint = true)
        {
            myNode? node = null;
            if (cfg.radCfgUseOpenFileSystemDB)
                node = OpenFileSystemDB.newNode(ref cfg.ctx1,
                    specialNodeType, nodeType, domainType,
                    ref initialNode, nodeDateTime, parentId, DBImport, title, rtf, newID);
            else
                node = SingleFileDB.newNode(ref cfg.ctx0,
                    specialNodeType, nodeType, domainType,
                    ref initialNode, nodeDateTime, parentId, DBImport, title, rtf, newID, checkpoint);

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
            ref myNode node, bool sort = true, bool descending = false)
        {
            List<myNode> list = new List<myNode>();

            if (node == null)
                return list; // error node not found

            Queue<myNode> queue = new Queue<myNode>();
            queue.Enqueue(node);

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
        // this method first finds & loads the current node by it's guid, then recursively finds & loads all it's parents and ancestors
        // right to the root ancestor which has no parent of it's own.
        public static List<myNode> findBottomToRootNodesRecursive(ref List<myNode> allNodes, ref myNode srcNode, bool deleted = false,
            bool useDeletedParam = false)
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
                }
                else
                {
                    // add this parent node in the list
                    nodes.Add(node);
                }
            }
            return nodes;
        }

        // this method purges the old unusable files and replaces them with new update, and updates the node accordingly. 
        public static bool DBUpdateNode(myConfig cfg, ref myNode node, String rtf = "", bool storeData = false, bool updateModificationDate = true)
        {
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.updateNode(cfg.ctx1, ref node, rtf, storeData, updateModificationDate);
            else
                return SingleFileDB.updateNode(cfg.ctx0, ref node, rtf, storeData, updateModificationDate);
        }

        // erases and purges the node's files
        public static bool DBPurgeNode(myConfig cfg, ref myNode node)
        {
            if (cfg.radCfgUseOpenFileSystemDB)
                return OpenFileSystemDB.purgeNode(cfg.ctx1, ref node);
            else
                return SingleFileDB.purgeNode(cfg.ctx0, ref node);
        }

        // deletes or restores the node and restores all the affected parent nodes if the child node is restored
        public static bool DBDeleteOrPurgeNode(myConfig cfg, ref List<myNode> allNodes, ref myNode node, bool mark = true, bool purge = false)
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
                return DBPurgeNode(cfg, ref node);
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
                    DBUpdateNode(cfg, ref effectedNode, "", false, false);
                    allNodes.Remove(listedNode); // parent node processed, remove them
                }
            }
            else
            {
                // mark delete
                node.chapter.IsDeleted = true;

                // finally update the node
                DBUpdateNode(cfg, ref node, "", false, false);
            }

            // this node was processed, so remove it from list
            allNodes.Remove(node);

            return true;

        }

        public static bool DBDeleteOrPurgeListRecursive(myConfig cfg, ref List<myNode> allNodes,
            ref List<myNode> nodes, bool mark = true, bool purge = false)
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
                        DBDeleteOrPurgeNode(cfg, ref allNodes, ref child, mark, purge);
                    }
                }

                // finally process this node
                DBDeleteOrPurgeNode(cfg, ref allNodes, ref node, mark, purge);
            }

            return true;
        }

        public static bool DBDeleteOrPurgeNodeRecursive(myConfig cfg, ref List<myNode> allNodes, ref myNode node, bool mark = true, bool purge = false)
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
                    DBDeleteOrPurgeNode(cfg, ref allNodes, ref child, mark, purge);
                }
            }

            // finally process this node
            DBDeleteOrPurgeNode(cfg, ref allNodes, ref node, mark, purge);

            return true;
        }
        // everything is kept in sets. set node is the root node. all import and export of sets exist in set node which is their root node.
        // set node is root node and has no parent.
        public static myNode createSetNode(ref Int64 currentIndex, String setName, DateTime setDateTime)
        {
            myNode node = new myNode(true);
            node.chapter.chapterDateTime = setDateTime;
            node.chapter.Id = CreateNodeID(ref currentIndex);
            node.chapter.nodeType = NodeType.SetNode;
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
            node.chapter.Id = CreateNodeID(ref cfg);
            node.chapter.nodeType = NodeType.SetNode;
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
                entryMethods.sortNodesByDateTime(ref nodes, descending);

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

        // this method finds all first level or direct children of the target parent node, non-recursive.
        public static List<myNode> findFirstLevelChildren(Int64 parentId, ref List<myNode> srcNodes,
            bool sort = true, bool descending = false)
        {
            List<myNode> nodes = new List<myNode>();
            if (parentId == 0)
                return nodes;

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
                return OpenFileSystemDB.updateNode(cfg.ctx1, ref node, "", false);
            else
                return SingleFileDB.updateNode(cfg.ctx0, ref node, "", false);
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
                return OpenFileSystemDB.updateNode(cfg.ctx1, ref node, "", false);
            else
                return SingleFileDB.updateNode(cfg.ctx0, ref node, "", false);
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
                    case NodeType.YearNode:
                        if (node.chapter.nodeType == NodeType.YearNode && node.chapter.chapterDateTime.Year == year)
                            nodes.Add(node);

                        break;
                    case NodeType.MonthNode:
                        if (node.chapter.nodeType == NodeType.MonthNode && node.chapter.chapterDateTime.Year == year && node.chapter.chapterDateTime.Month == month)
                            nodes.Add(node);

                        break;
                    case NodeType.EntryNode:
                        if (node.chapter.nodeType == NodeType.EntryNode && node.chapter.chapterDateTime.Year == year && node.chapter.chapterDateTime.Month == month &&
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
        public static void loadNodeHighlight(TreeNode treeNode, ref myNode node)
        {
            if (node.chapter.HLFont.Length >= 1)
                treeNode.NodeFont = commonMethods.StringToFont(node.chapter.HLFont);

            if (node.chapter.HLFontColor.Length >= 1)
                treeNode.ForeColor = commonMethods.StringToColor(node.chapter.HLFontColor);

            if (node.chapter.HLBackColor.Length >= 1)
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
        public static List<TreeNode> buildTreeViewTree(ref List<myNode> srcNodes,
            bool sort = true, bool descending = false, MonthCalendar? CalendarEntries = null,
            bool addDeletedNode = false)
        {
            List<TreeNode> tree = new List<TreeNode>();
            Queue<TreeNode> queue = new Queue<TreeNode>();

            // system nodes are first to be indexed at index 0 before all the rest of nodes.
            List<myNode> rootNodes = new List<myNode>();
            rootNodes.AddRange(entryMethods.findRootNodes(ref srcNodes, SpecialNodeType.SystemNode, sort, descending));
            // non system nodes must exist after the system nodes.
            rootNodes.AddRange(entryMethods.findRootNodes(ref srcNodes, SpecialNodeType.NonSystemNode, sort, descending));

            // first enqueue all root nodes
            foreach (myNode rootNode in rootNodes)
            {
                if (!addDeletedNode)
                {
                    if (rootNode.chapter.IsDeleted)
                        continue;
                }
                myNode node = rootNode;
                String path = String.Format(@"{0}", node.chapter.Id);
                String entryName = getEntryLabel(node);
                TreeNode newTreeNode = new TreeNode(entryName);
                newTreeNode.Name = path;
                loadNodeHighlight(newTreeNode, ref node);
                newTreeNode.Tag = node;

                if (CalendarEntries != null)
                    setCalendarHighlightEntry(CalendarEntries, node.chapter.chapterDateTime);

                tree.Add(newTreeNode); 
                queue.Enqueue(newTreeNode);
            }
            // now build a perfect sequentially ordered queue of all parents first, then 2nd their children most recursively.
            // in the year loop, 1st all years are enqueued. and all months of each year are enqueued into the queue.
            // the 2nd is month loop. when all years and their months added, then all children of each month are enqueued.
            // this is most sequential and recursive layer by layer processing.
            while (queue.Count > 0)
            {
                TreeNode currentTreeNode = queue.Dequeue();
                if (currentTreeNode == null)
                    continue;

                myNode currentNode = (myNode)currentTreeNode.Tag;

                // fetch this node's children
                List<myNode> children = entryMethods.findFirstLevelChildren(currentNode.chapter.Id, ref srcNodes, sort, descending);

                // 2nd in sequence is parent's children, so children are added 2nd to parent in sequence.
                foreach (myNode childNode in children)
                {
                    if (!addDeletedNode)
                    {
                        if (childNode.chapter.IsDeleted)
                            continue;
                    }
                    myNode node = childNode;
                    String path = String.Format(@"{0}", node.chapter.Id);
                    String entryName = getEntryLabel(node);
                    TreeNode newTreeNode = new TreeNode(entryName);
                    newTreeNode.Name = path;
                    loadNodeHighlight(newTreeNode, ref node);
                    newTreeNode.Tag = node;

                    currentTreeNode.Nodes.Add(newTreeNode);
                    queue.Enqueue(newTreeNode);
                }

                // setup tree node icons
                if (currentNode.chapter.specialNodeType == SpecialNodeType.SystemNode && (currentNode.chapter.nodeType == NodeType.JournalNode ||
                    currentNode.chapter.nodeType == NodeType.LibraryNode))
                {
                    currentTreeNode.ImageIndex = 1;
                    currentTreeNode.SelectedImageIndex = 2;
                }
                // setup treeview icons
                else if (currentNode.chapter.nodeType == NodeType.YearNode || currentNode.chapter.nodeType == NodeType.MonthNode)
                {
                    currentTreeNode.ImageIndex = 3;
                    currentTreeNode.SelectedImageIndex = 3;
                }
                else if (currentNode.chapter.nodeType == NodeType.SetNode)
                {
                    currentTreeNode.ImageIndex = 4;
                    currentTreeNode.SelectedImageIndex = 4;
                }
                else if (currentNode.chapter.nodeType == NodeType.LabelNode)
                {
                    currentTreeNode.ImageIndex = 5;
                    currentTreeNode.SelectedImageIndex = 5;
                }

                // null the processed tree node's tag so that the resource is released and memory freed.
                currentTreeNode.Tag = null;
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
                SpecialNodeType.SystemNode, NodeType.YearNode,
                year, month, -1);

            myNode? yearNode = null;
            if (yearNodes.Count() <= 0)
            {
                // this year doesn't exists in db, so create it.
                DateTime yearDateTime = new DateTime(year, 1, 1, 0, 0, 0, 0);
                String yearLabel = yearDateTime.ToString("yyyy");
                yearNode = new myNode(true);
                yearNode.chapter.nodeType = NodeType.YearNode;
                yearNode.chapter.specialNodeType = SpecialNodeType.SystemNode;
                yearNode.chapter.parentId = systemNodes.JournalSystemNode.chapter.Id;
                yearNode.chapter.chapterDateTime = yearDateTime;
                yearNode.chapter.Title = yearLabel;
                if (!DBCreateNode(ref cfg, ref yearNode, "", true))
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
                SpecialNodeType.SystemNode, NodeType.MonthNode,
                year, month, -1);

            myNode? monthNode = null;
            if (monthNodes.Count() <= 0)
            {
                // this month in the current chapter's year doesn't exists in db, so create it.
                DateTime monthDateTime = new DateTime(year, month, 1, 0, 0, 0, 0);
                String monthLabel = monthDateTime.ToString("MMMM");
                monthNode = new myNode(true);
                monthNode.chapter.nodeType = NodeType.MonthNode;
                monthNode.chapter.specialNodeType = SpecialNodeType.SystemNode;
                monthNode.chapter.parentId = yearNode.chapter.Id;
                monthNode.chapter.chapterDateTime = monthDateTime;
                monthNode.chapter.Title = monthLabel;
                if (!DBCreateNode(ref cfg, ref monthNode, "", true))
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
            yearNodes = findNodesByNodeTypeDate(ref allNodes, SpecialNodeType.None, NodeType.YearNode, year, month, -1);

            myNode? yearNode = null;
            if (yearNodes.Count() <= 0)
            {
                // this year doesn't exists in db, so create it.
                DateTime yearDateTime = new DateTime(year, 1, 1, 0, 0, 0, 0);
                String yearLabel = yearDateTime.ToString("yyyy");
                yearNode = DBNewNode(ref cfg,
                    SpecialNodeType.None, NodeType.YearNode, DomainType.Journal,
                    ref yearNode, yearDateTime, setNode.chapter.Id, true, yearLabel, "",
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
            monthNodes = findNodesByNodeTypeDate(ref allNodes, SpecialNodeType.None, NodeType.MonthNode, year, month, -1);

            myNode? monthNode = null;
            if (monthNodes.Count() <= 0)
            {
                // this month in the current chapter's year doesn't exists in db, so create it.
                DateTime monthDateTime = new DateTime(year, month, 1, 0, 0, 0, 0);
                String monthLabel = monthDateTime.ToString("MMMM");
                monthNode = DBNewNode(ref cfg,
                    SpecialNodeType.None, NodeType.MonthNode, DomainType.Journal,
                    ref monthNode, monthDateTime, yearNode.chapter.Id, true, monthLabel,
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

            if (descending)
                nodes.Reverse();

        }
    }
}
