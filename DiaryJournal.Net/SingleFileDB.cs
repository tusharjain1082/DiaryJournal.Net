using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

namespace DiaryJournal.Net
{
    public class SingleFileDBContext
    {
        public LiteDatabase? db = null;
        public ILiteCollection<Chapter>? chapterCollection = null;
        public ILiteCollection<ChapterData>? chapterDataCollection = null;
        public DatabaseConfig dbConfig = new DatabaseConfig();
        public String dbConfigFile = "";
        public String dbpath = "";
        public String dbfile = "";

        public bool isDBOpen()
        {
            if (db == null)
                return false;

            return true;
        }

        public void close()
        {
            try
            {
                if (db != null)
                {
                    db.Checkpoint();
                    db.Dispose();
                }

            }
            catch (Exception)
            {
            }
            chapterCollection = null;
            chapterDataCollection = null;
            db = null;
            dbfile = dbpath = "";
        }

    }
    public static class SingleFileDB
    {
        public static String defaultDBPath = Application.StartupPath;
        public static String defaultDBPath_factory = Application.StartupPath;
        public static String defaultDBFileName = "myJournal";
        public static String defaultDBFileName_factory = "myJournal";
        public const string configFileName = ".dbConfig.xml";

        public const String DBExt = "db";
        public const String DBExtComplete = ".db";
        public const String DBExtSearchPattern = "*.db";


        public static bool CreateLoadDB(String file, ref SingleFileDBContext ctx, bool overwrite)
        {
            // if db exists load it, else create new.
            if (overwrite)
            {
                // overwrite create new direct
                if (!SingleFileDB.createDb(file))
                    return false;
            }
            else
            {
                // skip db creation/overwrite if it already exists
                if (!File.Exists(file))
                {
                    if (!SingleFileDB.createDb(file))
                        return false;
                }
            }

            // load the db
            ctx.db = SingleFileDB.loadDB(file);
            if (ctx.db == null)
                return false;

            // initialize all tables
            if (!SingleFileDB.initDB(ctx, file))
                return false;

            return true;
        }

        public static bool createDb(String file)
        {
            try
            {
                if (File.Exists(file))
                    commonMethods.SecureEraseFile(file, 1, true);
                //File.Delete(file);
            }
            catch (Exception)
            {
                return false;
            }

            try
            {
                FileStream fileStream = new FileStream(file, FileMode.Create);
                LiteDatabase db = new LiteDatabase(fileStream);
                db.Checkpoint();
                db.Dispose();
                fileStream.Close();
                fileStream.Dispose();

                // delete previous config file if it exists
                String dbConfigFile = file + configFileName;
                if (File.Exists(dbConfigFile))
                    File.Delete(dbConfigFile);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static LiteDatabase? loadDB(String file)
        {
            try
            {
                LiteDatabase db = new LiteDatabase(file);
                db.Checkpoint();
                return db;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static bool initDB(SingleFileDBContext ctx, String path)
        {
            bool result = initChaptersTable(ctx);
            result = initChapterDataTable(ctx);

            if (result)
            {
                ctx.dbpath = path;
                FileInfo fileInfo = new FileInfo(path);
                ctx.dbfile = fileInfo.Name;

                // create/load config file
                String dbConfigFile = path + configFileName;
                ctx.dbConfig = new DatabaseConfig();
                ctx.dbConfigFile = dbConfigFile;
                if (!File.Exists(dbConfigFile))
                    DatabaseConfig.toXmlFile(ref ctx.dbConfig, dbConfigFile);
                else
                    DatabaseConfig.fromXml(ref ctx.dbConfig, dbConfigFile);


            }
            return result;
        }

        public static bool autoLoadCreateDefaultDB(ref SingleFileDBContext ctx, myConfig cfg, bool overwrite)
        {
            if (ctx.db != null)
                ctx.close();

            if (cfg.chkCfgUseWinUserDocFolder)
                defaultDBPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            else
                defaultDBPath = Application.StartupPath;

            try
            {
                if (!Directory.Exists(defaultDBPath))
                    Directory.CreateDirectory(defaultDBPath);

                String fileName = String.Format("{0}{1}", defaultDBFileName, DBExtComplete);
                String file = Path.Combine(defaultDBPath, Path.GetFileName(fileName));

                // if db exists load it, else create new.
                if (overwrite)
                {
                    // overwrite create new direct
                    if (!SingleFileDB.createDb(file))
                        return false;
                }
                else
                {
                    // skip db creation/overwrite if it already exists
                    if (!File.Exists(file))
                    {
                        if (!SingleFileDB.createDb(file))
                            return false;
                    }
                }


                // load the db
                ctx.db = SingleFileDB.loadDB(file);
                if (ctx.db == null)
                    return false;

                // initialize all tables
                if (!SingleFileDB.initDB(ctx, file))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }

        }

        public static bool rebuildDB(SingleFileDBContext ctx)
        {
            if (ctx.db == null)
                return false;

            try
            {
                ctx.db.Rebuild();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static List<myNode> findAllNodes(SingleFileDBContext ctx, bool sort = true, bool descending = false)
        {
            List<myNode> nodes = new List<myNode>();
            IEnumerable<Chapter> chapters = ctx.chapterCollection.FindAll();
            if (chapters == null)
                return nodes;

            if (chapters.LongCount() <= 0)
                return nodes;

            foreach (Chapter dbChapter in chapters)
            {
                myNode node = new myNode();
                node.chapter = dbChapter;
                nodes.Add(node);
            }

            // sort by date and time
            if (sort)
                entryMethods.sortNodesByDateTime(ref nodes, descending);

            return nodes;
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

        // this method finds all nodes ordered by first parent and then it's children and so and so
        public static List<myNode> findAllNodesTreeSequence(SingleFileDBContext ctx, bool sort = true, bool descending = false)
        {
            List<myNode> nodes = new List<myNode>();
            Queue<myNode> queue = new Queue<myNode>();

            // we cannot rebuild and reloop all the nodes indefinately per node down below. so we create the list once and use it indefinately.
            List<myNode> srcNodes = findAllNodes(ctx, sort, descending);

            // first enqueue all root nodes
            foreach (myNode rootNodes in findRootNodes(ref srcNodes, sort, descending))
                queue.Enqueue(rootNodes);

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
                List<myNode> children = entryMethods.findFirstLevelChildren(currentNode.chapter.nodeID, ref srcNodes, sort, descending);

                // 2nd in sequence is parent's children, so children are added 2nd to parent in sequence.
                foreach (myNode childNode in children)
                    queue.Enqueue(childNode);

                // 1st in sequence is the parent node, so parent node is added 1st in sequence before all children.
                nodes.Add(currentNode);
            }
            return nodes;
        }

        public static IEnumerable<Chapter> FindAllChapters(SingleFileDBContext ctx)
        {
            return ctx.chapterCollection.FindAll();
        }
        public static IEnumerable<ChapterData> FindAllChapterData(SingleFileDBContext ctx)
        {
            return ctx.chapterDataCollection.FindAll();
        }

        public static bool initChaptersTable(SingleFileDBContext ctx)
        {
            try
            {
                ctx.chapterCollection = ctx.db.GetCollection<Chapter>("chapters");
                ctx.chapterCollection.EnsureIndex(x => x.Id, true);
                ctx.db.Checkpoint();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //public static ILiteCollection<ChapterData> initChapterDataTable(SingleFileDBContext ctx)
        public static bool initChapterDataTable(SingleFileDBContext ctx)
        {
            try
            {
                ctx.chapterDataCollection = ctx.db.GetCollection<ChapterData>("chapterdata");
                ctx.chapterDataCollection.EnsureIndex(x => x.Id, true);
                ctx.db.Checkpoint();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static long NodesCount(SingleFileDBContext ctx)
        {
            return ChaptersCount(ctx);
        }

        public static long ChaptersCount(SingleFileDBContext ctx)
        {
            try
            {
                ctx.chapterCollection = ctx.db.GetCollection<Chapter>("chapters");
                ctx.chapterCollection.EnsureIndex(x => x.Id, true);
                return ctx.chapterCollection.LongCount();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static myNode? newNode(ref SingleFileDBContext ctx, NodeType nodeType, DomainType domainType, ref myNode? initialNode,
            DateTime nodeDateTime = default(DateTime), UInt32 parentId = 0, bool DBImport = true,
            String rtf = "", bool newNodeID = true)
        {
            // prepare and configure
            myNode? node = initialNode;
            if (node == null)
                node = new myNode(true);

            node.chapter.parentNodeID = parentId;
            node.chapter.chapterDateTime = nodeDateTime;
            node.chapter.nodeType = nodeType;
            node.chapter.domainType = domainType;

            // now when all setup is done, import the entry and it's data object into the db if required
            if (DBImport)
            {
                if (!createNode(ctx, ref node, rtf, newNodeID))
                    return null;
            }
            return node;
        }
        // find all nodes by date
        public static List<myNode> findNodesByDate(SingleFileDBContext ctx, NodeType nodeType, int year = -1, int month = -1, int day = -1)
        {
            List<myNode> nodes = new List<myNode>();

            foreach (myNode listNode in findAllNodes(ctx, true, false))
            {
                myNode node = listNode;

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

        // find all nodes by node type
        public static List<myNode> findNodesByNodeType(SingleFileDBContext ctx, NodeType nodeType)
        {
            List<myNode> nodes = new List<myNode>();
            foreach (myNode listNode in findAllNodes(ctx, true, false))
            {
                myNode node = listNode;
                if (node.chapter.nodeType == nodeType)
                    nodes.Add(node);
            }
            return nodes;
        }

        // find all children chapters in db by parent
        public static List<myNode> findNodesByParentID(SingleFileDBContext ctx, UInt32 parentId)
        {
            List<myNode> nodes = new List<myNode>();
            IEnumerable<Chapter> chapters = ctx.chapterCollection.Find(x => x.parentNodeID == parentId);
            if (chapters == null)
                return nodes;

            if (chapters.LongCount() <= 0)
                return nodes;

            foreach (Chapter dbChapter in chapters)
            {
                myNode node = new myNode();
                node.chapter = dbChapter;
                nodes.Add(node);
            }
            return nodes;
        }
        public static ChapterData newChapterData(UInt32 id, String? data = "")
        {
            ChapterData chapterData = new ChapterData();
            chapterData.nodeID = id;
            chapterData.data = data;
            return chapterData;
        }

        public static bool importNewDBChapterData(SingleFileDBContext ctx, ref ChapterData chapterData)
        {
            // now import the entry into db
            try
            {
                ctx.chapterDataCollection.Insert(chapterData);
                ctx.db.Checkpoint();
                return true;
            }
            catch (Exception ex)
            {
                // error importing entry
                return false;
            }
        }
        public static bool importNewDBChapter(SingleFileDBContext ctx, ref Chapter dbChapter)
        {
            // now import the entry into db
            try
            {
                ctx.chapterCollection.Insert(dbChapter);
                ctx.db.Checkpoint();
                return true;
            }
            catch (Exception)
            {
                // error importing entry
                return false;
            }
        }

        // find chapter data in database by it's litedb id
        public static ChapterData? findDbChapterDataByLiteDBID(SingleFileDBContext ctx, long LiteDBID)
        {
            ChapterData? chapterData = ctx.chapterDataCollection.FindById(LiteDBID);
            if (chapterData == null)
                return null;

            return chapterData;
        }

        // find chapter data in database by it's guid
        public static ChapterData? findDbChapterData(SingleFileDBContext ctx, UInt32 id)
        {
            IEnumerable<ChapterData> results = ctx.chapterDataCollection.Find(x => x.nodeID == id);
            if (results.Count() <= 0)
                return null;

            return results.FirstOrDefault();
        }

        // find all deleted marked nodes in db
        public static List<myNode> findDeletedNodes(SingleFileDBContext ctx)
        {
            List<myNode> nodes = new List<myNode>();
            IEnumerable<Chapter> results = ctx.chapterCollection.Find(x => x.IsDeleted == true);
            foreach (Chapter chapter in results)
            {
                myNode node = new myNode();
                node.chapter = chapter;
                nodes.Add(node);
            }
            return nodes;
        }

        // find all children chapters in db by parent
        public static IEnumerable<Chapter> findChildrenByParentId(SingleFileDBContext ctx, UInt32 parentId)
        {
            return ctx.chapterCollection.Find(x => x.parentNodeID == parentId);
        }

        public static bool updateNodeTitleByLiteDBID(SingleFileDBContext ctx, long id, String title = "")
        {
            String rtf = "";
            myNode? node = findLoadNodeByLiteDBID(ctx, id, ref rtf, false);
            if (node == null)
                return false;

            // set title to the chapter
            node.chapter.Title = title;

            // chapter found, update it and it's data
            return updateNode(ctx, ref node);
        }

        public static bool updateNodeTitle(SingleFileDBContext ctx, UInt32 id, String title = "")
        {
            String rtf = "";
            myNode? node = findLoadNode(ctx, id, ref rtf, false);
            if (node == null)
                return false;

            // set title to the chapter
            node.chapter.Title = title;

            // chapter found, update it and it's data
            return updateNode(ctx, ref node);
        }

        public static bool updateChapter(SingleFileDBContext ctx, UInt32 id, String decodedRtf = "")
        {
            Chapter? dbChapter = findDbChapter(ctx, id);
            if (dbChapter == null)
                return false;

            // chapter found, update it and it's data
            return UpdateChapterAndData(ctx, ref dbChapter, decodedRtf);
        }

        public static bool updateDBChapterData(SingleFileDBContext ctx, ref ChapterData chapterData)
        {
            // found chapter data object, so update
            try
            {
                ctx.chapterDataCollection.Update(chapterData);
                ctx.db.Checkpoint();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool setNodeParent(SingleFileDBContext ctx, UInt32 id, ref myNode updatedOut, UInt32 parentId = 0)
        {
            String rtf = "";
            myNode? node = findLoadNode(ctx, id, ref rtf, false);
            if (node == null)
                return false;

            // configure
            node.chapter.parentNodeID = parentId;

            // finally update the node
            bool result = updateNode(ctx, ref node);
            updatedOut = node;
            return result;
        }

        public static bool updateNode(SingleFileDBContext ctx, ref myNode node)
        {
            if (node == null)
                return false;

            return updateDBChapter(ctx, ref node.chapter);
        }

        public static bool updateDBChapter(SingleFileDBContext ctx, ref Chapter dbChapter)
        {
            try
            {
                ctx.chapterCollection.Update(dbChapter);
                ctx.db.Checkpoint();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool createNode(SingleFileDBContext ctx, ref myNode node, String rtf, bool newNodeID)
        {

            if (newNodeID)
            {
                // generate new id if demanded
                node.chapter.nodeID = entryMethods.CreateNodeID(ref ctx.dbConfig.currentDBIndex);
            }

            return importNewChapterAndData(ctx, ref node.chapter, rtf);
        }

        public static bool importNewChapterAndData(SingleFileDBContext ctx, ref Chapter chapter, String rtf)
        {
            // 1st import the chapter's data blob
            ChapterData chapterData = SingleFileDB.newChapterData(chapter.nodeID, rtf);
            if (!SingleFileDB.importNewDBChapterData(ctx, ref chapterData))
                return false;

            // 2nd now import the entry as a chapter into db
            if (!SingleFileDB.importNewDBChapter(ctx, ref chapter))
                return false;

            return true;
        }

        public static bool updateNode(SingleFileDBContext ctx, ref myNode node, String rtf = "", bool storeData = false)
        {
            if (storeData)
                return UpdateChapterAndData(ctx, ref node.chapter, rtf);
            else
                return UpdateChapterAndData(ctx, ref node.chapter, null);
        }

        public static bool UpdateChapterAndData(SingleFileDBContext ctx, ref Chapter dbChapter, String? rtf)
        {
            if (rtf != null)
            {
                // 1st load chapter's data blob
                ChapterData? dbChapterData = SingleFileDB.loadDBChapterDataByLiteDBID(ctx, dbChapter.Id);
                if (dbChapterData == null)
                    return false;

                // 2nd set blob in chapterData and update
                dbChapterData.data = rtf;
                if (!SingleFileDB.updateDBChapterData(ctx, ref dbChapterData))
                    return false;
            }

            // finally update chapter
            return updateDBChapter(ctx, ref dbChapter);
        }

        // find & load a node by it's litedb id
        public static myNode? findLoadNodeByLiteDBID(SingleFileDBContext ctx, long LiteDBID, ref String rtf, bool loadData = false)
        {
            // load the node
            Chapter? dbChapter = findDbChapterByLiteDBID(ctx, LiteDBID);
            if (dbChapter == null)
                return null;

            if (loadData)
            {
                // load node's data
                ChapterData? dbChapterData = findDbChapterDataByLiteDBID(ctx, LiteDBID);
                if (dbChapterData != null)
                    rtf = dbChapterData.data;
            }
            myNode node = new myNode(ref dbChapter);
            return node;
        }

        // find & load a node by it's guid
        public static myNode? findLoadNode(SingleFileDBContext ctx, UInt32 id, ref String rtf, bool loadData = false)
        {
            // load the node
            Chapter? dbChapter = findDbChapter(ctx, id);
            if (dbChapter == null)
                return null;

            if (loadData)
            {
                // load node's data
                ChapterData? dbChapterData = findDbChapterDataByLiteDBID(ctx, dbChapter.Id);
                if (dbChapterData != null)
                    rtf = dbChapterData.data;
            }
            myNode node = new myNode(ref dbChapter);
            return node;
        }

        // load the entry data through entry's node
        public static String loadNodeData(SingleFileDBContext ctx, UInt32 id)
        {
            String rtf = "";

            // load node's data
            ChapterData? dbChapterData = findDbChapterData(ctx, id);
            if (dbChapterData == null)
                return "";

            rtf = dbChapterData.data;

            return rtf;
        }
        // load the entry data through entry's node
        public static String loadNodeDataByLiteDBID(SingleFileDBContext ctx, long LiteDBID)
        {
            String rtf = "";

            // load node's data
            ChapterData? dbChapterData = findDbChapterDataByLiteDBID(ctx, LiteDBID);    
            if (dbChapterData == null)
                return "";

            rtf = dbChapterData.data;

            return rtf;
        }

        public static ChapterData? loadDBChapterDataByLiteDBID(SingleFileDBContext ctx, long LiteDBID)
        {
            // get original database chapter data using identifier
            ChapterData? dbChapterData = findDbChapterDataByLiteDBID(ctx, LiteDBID);
            if (dbChapterData == null)
                return null;

            // success
            return dbChapterData;
        }

        public static ChapterData? loadDBChapterData(SingleFileDBContext ctx, UInt32 id)
        {
            // get original database chapter data using identifier
            ChapterData? dbChapterData = findDbChapterData(ctx, id);
            if (dbChapterData == null)
                return null;

            // success
            return dbChapterData;
        }

        // load the entry config through entry's node 
        public static bool loadNodeConfig(SingleFileDBContext ctx, UInt32 id, ref myNode nodeOut)
        {
            String rtf = "";
            myNode? node = findLoadNode(ctx, id, ref rtf, false);
            if (node == null)
                return false;

            nodeOut = node;
            return true;
        }


        public static bool purgeDBDataNode(SingleFileDBContext ctx, UInt32 id)
        {
            // 1st load chapter's data blob
            ChapterData? dbChapterData = SingleFileDB.loadDBChapterData(ctx, id);
            if (dbChapterData == null)
                return false;

            try
            {
                bool result = ctx.chapterDataCollection.Delete(dbChapterData.Id);
                ctx.db.Checkpoint();
                return result;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public static bool purgeNode(SingleFileDBContext ctx, UInt32 id)
        {
            String rtf = "";
            myNode? node = findLoadNode(ctx, id, ref rtf, false);
            if (node == null)
                return false; // no more parents found, this is end of loop

            // first purge this chapter's data
            if (!purgeDBDataNode(ctx, id))
                return false;

            try
            {
                // finally purge the chapter from db
                bool result = ctx.chapterCollection.Delete(node.chapter.Id);
                ctx.db.Checkpoint();
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // this method marks deleted a node and all it's children or purges the marked ones most recursively.
        public static bool DeleteOrPurgeRecursiveByLiteDBID(SingleFileDBContext ctx, long id, bool mark = true, bool purge = false)
        {
            // first get the root node
            String rtf = "";
            myNode? node = findLoadNodeByLiteDBID(ctx, id, ref rtf, false);
            if (node == null)
                return false; // no more parents found, this is end of loop

            // we cannot rebuild and reloop all the nodes indefinately per node down below. so we create the list once and use it indefinately.
            List<myNode> srcNodes = findAllNodes(ctx, false, false);

            Queue<myNode> queue = new Queue<myNode>();
            queue.Enqueue(node);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                if (currentNode == null)
                    continue;

                List<myNode> children = entryMethods.findFirstLevelChildren(currentNode.chapter.nodeID, ref srcNodes, false, false);

                foreach (myNode childNode in children)
                {
                    queue.Enqueue(childNode);
                }

                // finally delete or restore or purge current node
                DeleteOrPurgeNode(ctx, currentNode.chapter.nodeID, mark, purge);
            }
            return true;
        }

        // this method marks deleted a node and all it's children or purges the marked ones most recursively.
        public static bool DeleteOrPurgeRecursive(SingleFileDBContext ctx, UInt32 id, bool mark = true, bool purge = false)
        {
            // first get the root node
            String rtf = "";
            myNode? node = findLoadNode(ctx, id, ref rtf, false);
            if (node == null)
                return false; // no more parents found, this is end of loop

            // we cannot rebuild and reloop all the nodes indefinately per node down below. so we create the list once and use it indefinately.
            List<myNode> srcNodes = findAllNodes(ctx, false, false);

            Queue<myNode> queue = new Queue<myNode>();
            queue.Enqueue(node);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                if (currentNode == null)
                    continue;

                List<myNode> children = entryMethods.findFirstLevelChildren(currentNode.chapter.nodeID, ref srcNodes, false, false);

                foreach (myNode childNode in children)
                {
                    queue.Enqueue(childNode);
                }

                // finally delete or restore or purge current node
                DeleteOrPurgeNode(ctx, currentNode.chapter.nodeID, mark, purge);
            }
            return true;
        }
        // finds or selects the node from db
        public static myNode? selectNode(SingleFileDBContext ctx, UInt32 id, ref String rtf, bool loadData = false)
        {
            return findLoadNode(ctx, id, ref rtf, loadData);
        }

        // deletes or restores the node and restores all the affected parent nodes if the child node is restored
        public static bool DeleteOrPurgeNode(SingleFileDBContext ctx, UInt32 id, bool mark = true, bool purge = false)
        {
            // first get the node
            String rtf = "";
            myNode? node = findLoadNode(ctx, id, ref rtf, false);
            if (node == null)
                return false; // no more parents found, this is end of loop

            List<myNode> effectedNodes = null;
            if (!mark)
            {
                // user demands to restore this node, so all it's parents which were marked deleted,
                // must also be restored so that this node is restored to completion.
                effectedNodes = findBottomToRootNodesRecursive(ctx, node.chapter.nodeID, true, true);

                // restore all effected nodes
                foreach (myNode listedNode in effectedNodes)
                {
                    myNode effectedNode = listedNode;
                    effectedNode.chapter.IsDeleted = false; // restore all the affected nodes from bottom to the top
                    updateNode(ctx, ref effectedNode, null);
                }
            }
            else
            {
                //// we cannot delete year and month nodes, any node except common entry node.
                //if (node.chapter.nodeType == NodeType.EntryNode)
                //{
                // mark delete
                node.chapter.IsDeleted = true;

                // finally update the node
                updateNode(ctx, ref node, null);
                //}
            }

            // we cannot delete year and month nodes, any node except common entry node.
            //if (node.chapter.nodeType == NodeType.EntryNode)
            // {
            // purge the marked node if demanded
            if (purge)
                purgeNode(ctx, node.chapter.nodeID);
            // }

            return true;
        }

        // this method first finds & loads the current node by it's guid, then recursively finds & loads all it's parents and ancestors
        // right to the root ancestor which has no parent of it's own.
        public static List<myNode> findBottomToRootNodesRecursive(SingleFileDBContext ctx, UInt32 id, bool deleted = false,
            bool useDeletedParam = false)
        {
            List<myNode> nodes = new List<myNode>();

            // first find the current node
            String rtf = "";
            myNode? node = selectNode(ctx, id, ref rtf, false);
            if (node == null)
                return nodes; // error node not found

            // if user demands to find deleted nodes, then validate and add accordingly
            if (useDeletedParam)
            {
                if (node.chapter.IsDeleted == deleted)
                    nodes.Add(node); // matching node either deleted marked or not deleted marked as demanded by user
            }
            else
            {
                // add this node at index 0 of the list
                nodes.Add(node);
            }

            if (nodes.Count <= 0)
                return nodes;

            while (true)
            {
                // find and load all parent nodes recursively from bottom to root
                node = selectNode(ctx, node.chapter.parentNodeID, ref rtf, false);
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

        // find chapter in database by it's index
        public static Chapter? findDbChapter(SingleFileDBContext ctx, UInt32 id)
        {
            IEnumerable<Chapter> results = ctx.chapterCollection.Find(x => x.nodeID == id);
            if (results.Count() <= 0)
                return null;

            return results.FirstOrDefault();

        }
        // find chapter in database by it's litedb id
        public static Chapter? findDbChapterByLiteDBID(SingleFileDBContext ctx, long LiteDBID)
        {
            Chapter? chapter = ctx.chapterCollection.FindById(LiteDBID);
            if (chapter == null)
                return null;

            return chapter;

        }

        // find chapter in database by it's date and time
        public static Chapter? findDbChapterByDateTime(SingleFileDBContext ctx, DateTime dateTime)
        {
            IEnumerable<Chapter> results = ctx.chapterCollection.Find(x => x.Equals(dateTime));
            if (results.Count() <= 0)
                return null;

            return results.FirstOrDefault();

        }

        // find chapter in database by it's date and time
        public static Chapter? findDbChapterByDateTime(SingleFileDBContext ctx, int year, int month, int day)
        {
            IEnumerable<Chapter> results = ctx.chapterCollection.Find(x => x.chapterDateTime.Year == year && x.chapterDateTime.Month == month && x.chapterDateTime.Day == day);
            if (results.Count() <= 0)
                return null;

            return results.FirstOrDefault();
        }

        // find chapter in database by it's date and time
        public static myNode? findNodeByDateTime(SingleFileDBContext ctx, int year, int month, int day)
        {
            IEnumerable<Chapter> results = ctx.chapterCollection.Find(x => x.chapterDateTime.Year == year && x.chapterDateTime.Month == month && x.chapterDateTime.Day == day);
            if (results.Count() <= 0)
                return null;

            Chapter? dbChapter = results.FirstOrDefault();
            myNode node = new myNode(ref dbChapter);
            return node;
        }
    }
}
