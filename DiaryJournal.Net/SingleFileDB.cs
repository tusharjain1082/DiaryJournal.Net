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
        public String dbBasePath = "";
        public String dbpath = "";
        public String dbfile = "";
        public DatabaseIndexing dbIndexing = new DatabaseIndexing();
        public String dbIndexingFile = "";

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

                // delete previous db indexing file if it exists
                String dbBasePath = new FileInfo(file).DirectoryName;
                String dbIndexingFile = Path.Combine(dbBasePath, DatabaseIndexing.dbIndexingFileName);
                if (File.Exists(dbIndexingFile))
                    File.Delete(dbIndexingFile);

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
                db.CheckpointSize = 131072;
                db.Checkpoint();
                return db;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static bool initDB(SingleFileDBContext ctx, String file)
        {
            bool result = initChaptersTable(ctx);
            result = initChapterDataTable(ctx);

            if (result)
            {
                ctx.dbpath = file;
                FileInfo fileInfo = new FileInfo(file);
                ctx.dbfile = fileInfo.Name;

                // create/load config file
                String dbConfigFile = file + configFileName;
                ctx.dbConfig = new DatabaseConfig();
                ctx.dbConfigFile = dbConfigFile;
                if (File.Exists(dbConfigFile))
                    DatabaseConfig.fromXml(ref ctx.dbConfig, dbConfigFile);
                else
                    DatabaseConfig.toXmlFile(ref ctx.dbConfig, dbConfigFile);

                // initialize and write brand new db indexing file. if it exists, load it.
                ctx.dbBasePath = new FileInfo(file).DirectoryName;
                ctx.dbIndexingFile = Path.Combine(ctx.dbBasePath, DatabaseIndexing.dbIndexingFileName);
                ctx.dbIndexing = new DatabaseIndexing();
                if (File.Exists(ctx.dbIndexingFile))
                    DatabaseIndexing.fromFile(ref ctx.dbIndexing, ctx.dbIndexingFile);
                else
                    DatabaseIndexing.toFile(ref ctx.dbIndexing, ctx.dbIndexingFile);

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

        public static myNode? newNode(ref SingleFileDBContext ctx,
            SpecialNodeType specialNodeType, NodeType nodeType, DomainType domainType,
            ref myNode? initialNode, DateTime nodeDateTime = default(DateTime), Int64 parentId = 0, bool DBImport = true,
            String title = "", String rtf = "", bool newId = true, bool checkpoint = true)
        {
            // prepare and configure
            myNode? node = initialNode;
            if (node == null)
                node = new myNode(true);

            node.chapter.parentId = parentId;
            node.chapter.chapterDateTime = nodeDateTime;
            node.chapter.nodeType = nodeType;
            node.chapter.specialNodeType = specialNodeType;
            node.chapter.domainType = domainType;
            node.chapter.Title = title;


            // now when all setup is done, import the entry and it's data object into the db if required
            if (DBImport)
            {
                if (!createNode(ctx, ref node, rtf, newId, checkpoint))
                    return null;
            }
            return node;
        }
        public static ChapterData newChapterData(Int64 id, String? data = "")
        {
            ChapterData chapterData = new ChapterData();
            chapterData.Id = id;
            chapterData.data = data;
            return chapterData;
        }

        public static bool importNewDBChapterData(SingleFileDBContext ctx, ref ChapterData chapterData, 
            bool checkpoint = true)
        {
            // now import the entry into db
            try
            {
                ctx.chapterDataCollection.Insert(chapterData);

                if (checkpoint)
                    ctx.db.Checkpoint();

                return true;
            }
            catch (Exception ex)
            {
                // error importing entry
                return false;
            }
        }
        public static bool importNewDBChapter(SingleFileDBContext ctx, ref Chapter dbChapter,
            bool checkpoint = true)
        {
            // now import the entry into db
            try
            {
                ctx.chapterCollection.Insert(dbChapter);
                
                if (checkpoint)
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
        public static ChapterData? findDbChapterData(SingleFileDBContext ctx, Int64 id)
        {
            ChapterData? chapterData = ctx.chapterDataCollection.FindById(id);
            if (chapterData == null)
                return null;

            return chapterData;
        }

        public static bool updateNodeTitle(SingleFileDBContext ctx, Int64 id, String title = "")
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

        public static bool updateChapter(SingleFileDBContext ctx, Int64 id, String decodedRtf = "")
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
        public static bool updateNode(SingleFileDBContext ctx, ref myNode node)
        {
            if (node == null)
                return false;

            return updateDBChapter(ctx, ref node.chapter);
        }

        public static void copyDBChapters(ref Chapter src, ref Chapter dest)
        {
            dest.Title = src.Title;
            dest.parentId = src.parentId;
            dest.chapterDateTime = src.chapterDateTime;
            dest.specialNodeType = src.specialNodeType;
            dest.nodeType = src.nodeType;
            dest.IsDeleted = src.IsDeleted;
            dest.HLFontColor = src.HLFontColor;
            dest.HLFont = src.HLFont;
            dest.HLBackColor = src.HLBackColor;
            dest.domainType = src.domainType;

        }
        public static bool updateDBChapter(SingleFileDBContext ctx, ref Chapter dbChapter)
        {
            try
            {
                ctx.chapterCollection.Update(dbChapter.Id, dbChapter);
                ctx.db.Checkpoint();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool Checkpoint(SingleFileDBContext ctx)
        { 
            try
            {
                ctx.db.Checkpoint();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static bool createNode(SingleFileDBContext ctx, ref myNode node, String rtf, bool newId,
            bool checkpoint = true)
        {
            if (node == null)
                return false;

            if (newId)
            {
                // generate new id if demanded
                node.chapter.Id = entryMethods.CreateNodeID(ref ctx.dbIndexing.currentDBIndex);
            }

            // this is a new node, so setup dates
            node.chapter.creationDateTime = DateTime.Now;
            node.chapter.modificationDateTime = node.chapter.creationDateTime;

            return importNewChapterAndData(ctx, ref node.chapter, rtf, checkpoint);
        }

        public static bool importNewChapterAndData(SingleFileDBContext ctx, ref Chapter chapter, String rtf,
            bool checkpoint = true)
        {
            // 1st import the chapter's data blob
            ChapterData chapterData = SingleFileDB.newChapterData(chapter.Id, rtf);
            if (!SingleFileDB.importNewDBChapterData(ctx, ref chapterData, checkpoint))
                return false;

            // 2nd now import the entry as a chapter into db
            if (!SingleFileDB.importNewDBChapter(ctx, ref chapter, checkpoint))
                return false;

            return true;
        }

        public static bool updateNode(SingleFileDBContext ctx, ref myNode node, String rtf = "", bool storeData = false, bool updateModificationDate = true)
        {
            // prepare
            // update dates
            if (updateModificationDate)
                node.chapter.modificationDateTime = DateTime.Now;

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
                ChapterData? dbChapterData = SingleFileDB.findDbChapterData(ctx, dbChapter.Id);
                if (dbChapterData == null)
                    return false;

                // 2nd set blob in chapterData and update
                dbChapterData.data = rtf;
                if (!updateDBChapterData(ctx, ref dbChapterData))
                    return false;
            }

            // finally update chapter
            return updateDBChapter(ctx, ref dbChapter);
        }

        // find & load a node by it's guid
        public static myNode? findLoadNode(SingleFileDBContext ctx, Int64 id, ref String rtf, bool loadData = false)
        {
            // load the node
            Chapter? dbChapter = findDbChapter(ctx, id);
            if (dbChapter == null)
                return null;

            if (loadData)
            {
                // load node's data
                ChapterData? dbChapterData = findDbChapterData(ctx, dbChapter.Id);
                if (dbChapterData != null)
                    rtf = dbChapterData.data;
            }
            myNode node = new myNode(ref dbChapter);
            return node;
        }

        // load the entry data through entry's node
        public static String loadNodeData(SingleFileDBContext ctx, Int64 id)
        {
            String rtf = "";

            // load node's data
            ChapterData? dbChapterData = findDbChapterData(ctx, id);
            if (dbChapterData == null)
                return "";

            rtf = dbChapterData.data;

            return rtf;
        }

        // load the entry config through entry's node 
        public static bool loadNodeConfig(SingleFileDBContext ctx, Int64 id, ref myNode nodeOut)
        {
            String rtf = "";
            myNode? node = findLoadNode(ctx, id, ref rtf, false);
            if (node == null)
                return false;

            nodeOut = node;
            return true;
        }

        public static bool purgeDBDataNode(SingleFileDBContext ctx, ref myNode node)
        {
            if (node == null)
                return false;

            try
            {
                // directly using node's litedb id to identify the data
                bool result = ctx.chapterDataCollection.Delete(node.chapter.Id);
                ctx.db.Checkpoint();
                return result;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static bool purgeDBDataNode(SingleFileDBContext ctx, Int64 id)
        {
            // 1st load chapter's data blob
            ChapterData? dbChapterData = SingleFileDB.findDbChapterData(ctx, id);
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
        public static bool purgeNode(SingleFileDBContext ctx, ref myNode node)
        {
            if (node == null)
                return false;

            // first purge this chapter's data
            if (!purgeDBDataNode(ctx, ref node))
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
        public static bool purgeNode(SingleFileDBContext ctx, Int64 id)
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

        // finds or selects the node from db
        public static myNode? selectNode(SingleFileDBContext ctx, Int64 id, ref String rtf, bool loadData = false)
        {
            return findLoadNode(ctx, id, ref rtf, loadData);
        }

        // find chapter in database by it's index
        public static Chapter? findDbChapter(SingleFileDBContext ctx, Int64 id)
        {
            Chapter? chapter = ctx.chapterCollection.FindById(id);
            if (chapter == null)
                return null;

            return chapter;

        }

        //public static UInt32 findLatestExistentNodeID(SingleFileDBContext ctx)
        //{
         //   return ctx.chapterCollection.Max(x => x.Id);
       // }
    }
}
