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
                return true;
            }
            catch(Exception)
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
            if (result)
            {
                ctx.dbpath = path;
                FileInfo fileInfo = new FileInfo(path);
                ctx.dbfile = fileInfo.Name;
            }
            return result;
        }

        public static bool autoLoadCreateDefaultDB(SingleFileDBContext ctx, myConfig cfg, bool overwrite)
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

        public static List<myNode> findAllNodes(SingleFileDBContext ctx, bool sort = true, bool ascending = false)
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
                entryMethods.sortNodesByDateTime(ref nodes, ascending);

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
                initChapterDataTable(ctx);
                return true;
            }
            catch(Exception)
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

        // node is generic universal entry object used for all purposes. node can be any defined entry type including chapter.
        public static myNode? newNode(SingleFileDBContext ctx, NodeType nodeType,
            ref ChapterData? chapterDataOut, DateTime chapterDateTime = default(DateTime), String rtf = "",
            Chapter? parent = null, bool DBImport = true)
        {
            // create new entry but not import it in db.
            ChapterData? dbChapterData = null;
            Chapter? dbChapter = newChapter(ctx, ref dbChapterData, ref parent, chapterDateTime, false, rtf);
            if (dbChapter == null)
                return null;

            if (dbChapterData == null)
                return null;

            // setup new entry
            dbChapter.nodeType = nodeType;
            // now when all setup is done, import the entry and it's data object into the db if required
            if (DBImport)
            {
                // user asked to import into db
                if (!importNewDBChapterData(ctx, ref dbChapterData))
                    return null;

                if (!importNewDBChapter(ctx, ref dbChapter))
                    return null;
            }

            // finally init node object and output everything
            myNode node = new myNode(ref dbChapter);
            chapterDataOut = dbChapterData;
            return node;
        }

        public static Chapter? chapterAtIndex(IEnumerable<Chapter> chapters, long index)
        {
            long ctr = 0;
            foreach (Chapter chapter in chapters)
            {
                if (ctr == index)
                    return chapter;

                ctr++;
            }

            return null;
        }
        public static List<myNode> findNodesByNodeType(SingleFileDBContext ctx, NodeType nodeType, int Year = -1, int Month = -1)
        {
            List<myNode> nodes = new List<myNode>();
            IEnumerable<Chapter> chapters = null;
            if (nodeType == NodeType.YearNode && Year != -1)
                chapters = ctx.chapterCollection.Find(x => x.nodeType == nodeType && x.chapterDateTime.Year == Year);
            else if (nodeType == NodeType.MonthNode && Year != -1 && Month != -1)
                chapters = ctx.chapterCollection.Find(x => x.nodeType == nodeType && x.chapterDateTime.Year == Year && x.chapterDateTime.Month == Month);
            else
                chapters = ctx.chapterCollection.Find(x => x.nodeType == nodeType);

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

        // find all children chapters in db by parent
        public static List<myNode> findNodesByParentGuid(SingleFileDBContext ctx, Guid parentGuid)
        {
            List<myNode> nodes = new List<myNode>();
            IEnumerable<Chapter> chapters = ctx.chapterCollection.Find(x => x.parentGuid == parentGuid);
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

        public static Chapter? newChapter(SingleFileDBContext ctx, ref ChapterData? chapterDataOut, ref Chapter? parent, 
            DateTime chapterDateTime = default(DateTime), bool DBImport = true, String rtf = "")
        {

            // create new chapter now
            Chapter chapter = new Chapter();
            if (chapterDateTime == default(DateTime))
                chapter.chapterDateTime = DateTime.Now;
            else
                chapter.chapterDateTime = chapterDateTime;

            chapter.Title = "";
            chapter.guid = Guid.NewGuid();

            if (parent != null)
            {
                // meaning this is child entry chapter, that's why parent attributes
                chapter.parentGuid = parent.guid;
            }

            // 1st create the chapter's data blob
            ChapterData? chapterData = SingleFileDB.newChapterData(chapter.guid, rtf);
            if (DBImport)
            {
                if (!SingleFileDB.importNewDBChapterData(ctx, ref chapterData))
                    return null;
            }

            // 2nd create chapter in db
            if (DBImport)
            {
                // now import the entry into db
                try
                {
                    ctx.chapterCollection.Insert(chapter);
                    ctx.db.Checkpoint();
                }
                catch (Exception)
                {
                    return null;
                }
            }
            // set output
            chapterDataOut = chapterData;
            return chapter;
        }

        public static ChapterData newChapterData(Guid guid, String? data = "")
        {
            ChapterData chapterData = new ChapterData();
            chapterData.guid = guid;    
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
            catch (Exception)
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
        // find chapter data in database by it's guid
        public static ChapterData? findDbChapterDataByGuid(SingleFileDBContext ctx, Guid guid)
        {
            // first load the table from db
            //ILiteCollection<ChapterData> collection = SingleFileDB.initChapterDataTable(ctx);
            //if (collection == null)
             //   return null;

            IEnumerable<ChapterData> results = ctx.chapterDataCollection.Find(x => x.guid.Equals(guid));
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
        public static IEnumerable<Chapter> findChildrenByParentGuid(SingleFileDBContext ctx, Guid parentGuid)
        {
            return ctx.chapterCollection.Find(x => x.parentGuid == parentGuid);
        }

        public static bool updateNodeTitle(SingleFileDBContext ctx, Guid guid, String title = "")
        {
            String rtf = "";
            myNode? node = findLoadNode(ctx, guid, ref rtf, false);
            if (node == null)
                return false;

            // set title to the chapter
            node.chapter.Title = title;

            // chapter found, update it and it's data
            return updateNode(ctx, ref node);
        }

        public static bool updateWholeChapterByGuid(SingleFileDBContext ctx, Chapter? chapter)
        {
            if (chapter == null)
                return false;

            Chapter? dbChapter = findDbChapterByGuid(ctx, chapter.guid);
            if (dbChapter == null)
                return false;

            // update whole chapter
            dbChapter.IsDeleted = chapter.IsDeleted;
            dbChapter.HLFontColor = chapter.HLFontColor;
            dbChapter.HLFont = chapter.HLFont;
            dbChapter.Title = chapter.Title;
            dbChapter.chapterDateTime = chapter.chapterDateTime;    
            dbChapter.guid = chapter.guid;  
            dbChapter.parentGuid = chapter.parentGuid;

            // update
            return updateDBChapter(ctx, ref dbChapter);
        }

        public static bool updateChapterHLBackColorByGuid(SingleFileDBContext ctx, Chapter? chapter)
        {
            if (chapter == null)
                return false;

            Chapter? dbChapter = findDbChapterByGuid(ctx, chapter.guid);
            if (dbChapter == null)
                return false;

            dbChapter.HLBackColor = chapter.HLBackColor;

            // update
            return updateDBChapter(ctx, ref dbChapter);
        }

        public static bool updateChapterHLFontByGuid(SingleFileDBContext ctx, Chapter? chapter)
        {
            if (chapter == null)
                return false;

            Chapter? dbChapter = findDbChapterByGuid(ctx, chapter.guid);
            if (dbChapter == null)
                return false;

            dbChapter.HLFontColor = chapter.HLFontColor;
            dbChapter.HLFont = chapter.HLFont;
            dbChapter.HLBackColor = chapter.HLBackColor;

            // update
            return updateDBChapter(ctx, ref dbChapter);
        }

        public static bool updateChapterByGuid(SingleFileDBContext ctx, Guid guid, String decodedRtf = "") 
        {
            Chapter? dbChapter = findDbChapterByGuid(ctx, guid);
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

        public static bool setNodeParent(SingleFileDBContext ctx, Guid guid, ref myNode updatedOut, Guid parentGuid = default(Guid))
        {
            String rtf = "";
            myNode? node = findLoadNode(ctx, guid, ref rtf, false);
            if (node == null)
                return false;

            // configure
            node.chapter.parentGuid = parentGuid;

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

        public static bool createNode(SingleFileDBContext ctx, ref myNode node, String rtf)
        {
            return importNewChapterAndData(ctx, ref node.chapter, rtf);
        }

        public static bool importNewChapterAndData(SingleFileDBContext ctx, ref Chapter chapter, String rtf)
        {
            // 1st import the chapter's data blob
            ChapterData chapterData = SingleFileDB.newChapterData(chapter.guid, rtf);
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
                ChapterData? dbChapterData = SingleFileDB.loadDBChapterData(ctx, dbChapter.guid);
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

        // find & load a node by it's guid
        public static myNode? findLoadNode(SingleFileDBContext ctx, Guid guid, ref String rtf, bool loadData = false)
        {
            // validate guid
            if (guid == Guid.Empty)
                return null;

            // load the node
            Chapter? dbChapter = findDbChapterByGuid(ctx, guid);
            if (dbChapter == null)
                return null;

            if (loadData)
            {
                // load node's data
                ChapterData? dbChapterData = findDbChapterDataByGuid(ctx, guid);
                if (dbChapterData != null)
                    rtf = dbChapterData.data;
            }
            myNode node = new myNode(ref dbChapter);
            return node;
        }

        // load the entry data through entry's node
        public static String? loadNodeData(SingleFileDBContext ctx, Guid guid)
        {
            String rtf = "";
            myNode? node = findLoadNode(ctx, guid, ref rtf, true);
            if (node == null)
                return null;

            return rtf;
        }

        public static ChapterData? loadDBChapterData(SingleFileDBContext ctx, Guid guid)
        {
            // get original database chapter data using identifier
            ChapterData? dbChapterData = findDbChapterDataByGuid(ctx, guid);
            if (dbChapterData == null)
                return null;

            // success
            return dbChapterData;
        }

        // load the entry config through entry's node 
        public static bool loadNodeConfig(SingleFileDBContext ctx, Guid guid, ref myNode nodeOut)
        {
            String rtf = "";
            myNode? node = findLoadNode(ctx, guid, ref rtf, false);
            if (node == null)
                return false;

            nodeOut = node;
            return true;
        }


        public static bool purgeDBDataNode(SingleFileDBContext ctx, Guid guid)
        {
            // 1st load chapter's data blob
            ChapterData? dbChapterData = SingleFileDB.loadDBChapterData(ctx, guid);
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
        public static bool purgeNode(SingleFileDBContext ctx, Guid guid)
        {
            String rtf = "";
            myNode? node = findLoadNode(ctx, guid, ref rtf, false);
            if (node == null)
                return false; // no more parents found, this is end of loop

            // first purge this chapter's data
            if (!purgeDBDataNode(ctx, guid))
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
        public static bool DeleteOrPurgeRecursive(SingleFileDBContext ctx, Guid guid, bool mark = true, bool purge = false)
        {

            // first get the root node
            String rtf = "";
            myNode? node = findLoadNode(ctx, guid, ref rtf, false);
            if (node == null)
                return false; // no more parents found, this is end of loop

            Queue<myNode> queue = new Queue<myNode>();
            queue.Enqueue(node);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                if (currentNode == null)
                    continue;

                List<myNode> children = findFirstLevelChildren(ctx, currentNode.chapter.guid);

                foreach (myNode childNode in children)
                {
                    queue.Enqueue(childNode);
                }

                // mark
                currentNode.chapter.IsDeleted = mark;

                // finally update the node
                updateNode(ctx, ref currentNode);

                // purge the marked nodes if demanded
                if (purge)
                    purgeNode(ctx, currentNode.chapter.guid);
            }
            return true;
        }

        public static List<myNode> findFirstLevelChildren(SingleFileDBContext ctx, Guid parentGuid)
        {
            List<myNode> nodes = new List<myNode>();

            if (parentGuid == Guid.Empty)
                return nodes;

            IEnumerable<Chapter> results = ctx.chapterCollection.Find(x => x.parentGuid.Equals(parentGuid));
            foreach (Chapter chapter in results)
            {
                myNode node = new myNode();
                node.chapter = chapter;
                nodes.Add(node);
            }
            return nodes;
        }

        public static Chapter? findParent(SingleFileDBContext ctx, Guid parentGuid)
        {
            if (parentGuid == null)
                return null;

            IEnumerable<Chapter> results = ctx.chapterCollection.Find(x => x.guid.Equals(parentGuid));
            if (results.Count() <= 0)
                return null;

            return results.FirstOrDefault();
        }

        // this method first finds & loads the current node by it's guid, then recursively finds & loads all it's parents and ancestors
        // right to the root ancestor which has no parent of it's own.
        public static List<myNode> findBottomToRootNodesRecursive(SingleFileDBContext ctx, Guid guid)
        {
            List<myNode> nodes = new List<myNode>();
            String rtf = "";

            // first find the current node
            Chapter? dbChapter = findDbChapterByGuid(ctx, guid);
            if (dbChapter == null)
                return nodes; // error chapter not found

            // add this node at index 0 of the list
            myNode node0 = new myNode(ref dbChapter);
            nodes.Add(node0);

            Chapter? currentChapter = dbChapter;
            while (true)
            {
                // find and load all parent nodes recursively from bottom to root
                currentChapter = findParent(ctx, currentChapter.parentGuid);
                if (currentChapter == null)
                    break; // no more parents found, this is end of loop

                // a parent found
                // add this parent node in the list
                myNode parentNode = new myNode(ref currentChapter);
                nodes.Add(parentNode);
            }
            return nodes;
        }

        // find chapter in database by it's index
        public static Chapter? findDbChapterByGuid(SingleFileDBContext ctx, Guid guid)
        {
            if (guid == null)
                return null;

            IEnumerable<Chapter> results = ctx.chapterCollection.Find(x => x.guid.Equals(guid));
            if (results.Count() <= 0)
                return null;

            return results.FirstOrDefault();

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

        // find all nodes in database by date and time
        public static List<myNode> findNodesByDate(SingleFileDBContext ctx, NodeType nodeType, int year, int month, int day)
        {
            List<myNode> nodes = new List<myNode>();
            IEnumerable<Chapter> results = ctx.chapterCollection.Find(x => x.chapterDateTime.Year == year && x.chapterDateTime.Month == month && x.chapterDateTime.Day == day);
            if (results.Count() <= 0)
                return nodes;

            foreach (Chapter chapter in results)
            {
                myNode node = new myNode();
                node.chapter = chapter;

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
    }
}
