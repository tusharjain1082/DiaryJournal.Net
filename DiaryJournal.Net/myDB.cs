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
    public class Chapter
    {
        public long Id { get; set; }
        public Guid guid { get; set; }
        public String? Title { get; set; } = "";
        public bool IsDeleted { get; set; } = false;
        public DateTime chapterDateTime { get; set; }
        public DateTime parentDateTime { get; set; } = default(DateTime);
        public Guid parentGuid { get; set; } = Guid.Empty;
        public String HLFont { get; set; } = "";
        public String HLFontColor { get; set; } = "";
        public String HLBackColor { get; set; } = "";
        public int year { get; set; } = 0;
        public int month { get; set; } = 0;
        public NodeType nodeType { get; set; } = NodeType.EntryNode;
    }

    // note that we cannot store data blob with chapter in db, because when we load the chapter, entire blob is loaded as well, which is a bug.
    // so we keep the data blob in another table to prevent it from automatically loading.
    public class ChapterData
    {
        public long Id { get; set; }
        public Guid guid { get; set; }
        public String? data { get; set; } = "";
    }

    public class myNode
    {
        public Chapter? chapter = null;
        public myNode()
        {

        }
        public myNode(ref Chapter? chapter)
        {
            this.chapter = chapter;
        }
    }

    public enum NodeType : int
    {
        RootNode = 0,
        YearNode = 1,
        MonthNode = 2,
        EntryNode = 3,
        LibraryNode = 4,
        TemplateNode = 5
    }

    public enum EntryType : int
    {
        Xml = 0,
        Rtf = 1,
        Html = 2,
        Txt = 3
    }

    public static class myDB
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

        public static String defaultDBPath = Application.StartupPath;
        public static String defaultDBPath_factory = Application.StartupPath;
        public static String defaultDBFileName = "myJournal";
        public static String defaultDBFileName_factory = "myJournal";

        public const String DBExt = "db";
        public const String DBExtComplete = ".db";
        public const String DBExtSearchPattern = "*.db";


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

        public static bool CreateLoadDB(String file, ref myContext ctx, bool overwrite)
        {
            // if db exists load it, else create new.
            if (overwrite)
            {
                // overwrite create new direct
                if (!myDB.createDb(file))
                    return false;
            }
            else
            {
                // skip db creation/overwrite if it already exists
                if (!File.Exists(file))
                {
                    if (!myDB.createDb(file))
                        return false;
                }
            }

            // load the db
            ctx.db = myDB.loadDB(file);
            if (ctx.db == null)
                return false;

            // initialize all tables
            if (!myDB.initDB(ctx, file))
                return false;

            return true;
        }

        public static bool createDb(String file)
        {
            try
            {
                if(File.Exists(file))
                    File.Delete(file);
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

        public static bool initDB(myContext ctx, String path)
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

        public static bool autoLoadCreateDefaultDB(myContext ctx, bool overwrite)
        {
            if (ctx.db != null)
                ctx.close();

            if (ctx.config.radioCfgUseDocumentsPath)
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
                    if (!myDB.createDb(file))
                        return false;
                }
                else
                {
                    // skip db creation/overwrite if it already exists
                    if (!File.Exists(file))
                    {
                        if (!myDB.createDb(file))
                            return false;
                    }
                }


                // load the db
                ctx.db = myDB.loadDB(file);
                if (ctx.db == null)
                    return false;

                // initialize all tables
                if (!myDB.initDB(ctx, file))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }

        }

        public static bool rebuildDB(myContext ctx)
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
        public static IEnumerable<Chapter> FindAllChapters(myContext ctx)
        {
            return ctx.chapterCollection.FindAll();
        }
        public static IEnumerable<ChapterData> FindAllChapterData(myContext ctx)
        {
            return ctx.chapterDataCollection.FindAll();
        }

        public static bool initChaptersTable(myContext ctx) 
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
        //public static ILiteCollection<ChapterData> initChapterDataTable(myContext ctx)
        public static bool initChapterDataTable(myContext ctx)
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

        public static long ChaptersCount(myContext ctx)
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
        public static myNode? newNode(myContext ctx, int Year, int Month, NodeType nodeType,
            ref ChapterData? chapterDataOut, DateTime chapterDateTime = default(DateTime),
            Chapter? parent = null, bool DBImport = true)
        {
            // create new entry but not import it in db.
            ChapterData? dbChapterData = null;
            Chapter? dbChapter = newChapter(ctx, ref dbChapterData, ref parent, chapterDateTime, false);
            if (dbChapter == null)
                return null;

            if (dbChapterData == null)
                return null;

            // setup new entry
            dbChapter.year = Year;
            dbChapter.month = Month;
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

        public static myNode? loadNode(myContext ctx, Guid guid)
        {
            Chapter? dbChapter = findDbChapterByGuid(ctx, guid);
            if (dbChapter == null)
                return null;

            myNode node = new myNode(ref dbChapter);
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
        public static List<myNode> findNodesByNodeType(myContext ctx, NodeType nodeType, int Year = -1, int Month = -1)
        {
            List<myNode> nodes = new List<myNode>();
            IEnumerable<Chapter> chapters = null;
            if (nodeType == NodeType.YearNode && Year != -1)
                chapters = ctx.chapterCollection.Find(x => x.nodeType == nodeType && x.year == Year);
            else if (nodeType == NodeType.MonthNode && Year != -1 && Month != -1)
                chapters = ctx.chapterCollection.Find(x => x.nodeType == nodeType && x.year == Year && x.month == Month);
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
        public static List<myNode> findNodesByParentGuid(myContext ctx, Guid parentGuid)
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

        public static Chapter? newChapter(myContext ctx, ref ChapterData? chapterDataOut, ref Chapter? parent, 
            DateTime chapterDateTime = default(DateTime), bool DBImport = true)
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
                chapter.parentDateTime = parent.chapterDateTime;
            }

            // 1st create the chapter's data blob
            String rtf = ""; // reset the chapter's rtf blob or it will be imported in db
            ChapterData? chapterData = myDB.newChapterData(chapter.guid, rtf);
            if (DBImport)
            {
                if (!myDB.importNewDBChapterData(ctx, ref chapterData))
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

        public static bool importNewDBChapterData(myContext ctx, ref ChapterData chapterData)
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
        public static bool importNewDBChapter(myContext ctx, ref Chapter dbChapter)
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
        public static ChapterData? findDbChapterDataByGuid(myContext ctx, Guid guid)
        {
            // first load the table from db
            //ILiteCollection<ChapterData> collection = myDB.initChapterDataTable(ctx);
            //if (collection == null)
             //   return null;

            IEnumerable<ChapterData> results = ctx.chapterDataCollection.Find(x => x.guid.Equals(guid));
            if (results.Count() <= 0)
                return null;

            return results.FirstOrDefault();
        }

        // find all deleted marked chapters in db
        public static IEnumerable<Chapter> findDeletedMarkedChapters(myContext ctx)
        {
            return ctx.chapterCollection.Find(x => x.IsDeleted == true);
        }

        // find all children chapters in db by parent
        public static IEnumerable<Chapter> findChildrenByParentGuid(myContext ctx, Guid parentGuid)
        {
            return ctx.chapterCollection.Find(x => x.parentGuid == parentGuid);
        }

        public static bool updateChapterTitleByIDChapter(myContext ctx, Chapter identifier, String title = "")
        {
            Chapter? dbChapter = findDbChapterByGuid(ctx, identifier.guid);
            if (dbChapter == null)
                return false;

            // set title to the chapter
            dbChapter.Title = title;

            // chapter found, update it and it's data
            return updateDBChapter(ctx, ref dbChapter);
        }

        public static bool updateWholeChapterByGuid(myContext ctx, Chapter? chapter)
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
            dbChapter.parentDateTime = chapter.parentDateTime;
            dbChapter.guid = chapter.guid;  
            dbChapter.parentGuid = chapter.parentGuid;

            // update
            return updateDBChapter(ctx, ref dbChapter);
        }

        public static bool updateChapterHLBackColorByGuid(myContext ctx, Chapter? chapter)
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

        public static bool updateChapterHLFontByGuid(myContext ctx, Chapter? chapter)
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

        public static bool updateChapterByGuid(myContext ctx, Guid guid, String decodedRtf = "") 
        {
            Chapter? dbChapter = findDbChapterByGuid(ctx, guid);
            if (dbChapter == null)
                return false;

            // chapter found, update it and it's data
            return UpdateChapterAndData(ctx, ref dbChapter, decodedRtf);
        }

        public static bool updateDBChapterData(myContext ctx, ref ChapterData chapterData)
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

        public static bool setUnsetEntryParentByGuid(myContext ctx, Guid guid, ref Chapter updatedEntryOut, Guid parentGuid = default(Guid))
        {
            Chapter? dbChapter = findDbChapterByGuid(ctx, guid);
            if (dbChapter == null)
                return false;

            Chapter? dbParentChapter = null;
            dbChapter.parentGuid = parentGuid;
            if (parentGuid == default(Guid))
            {
                // no parent for this chapter.
                dbChapter.parentDateTime = default(DateTime);
            }
            else
            {
                // a valid parent chapter is provided.
                dbParentChapter = findDbChapterByGuid(ctx, parentGuid);
                if (dbParentChapter == null)
                    return false;

                dbChapter.parentDateTime = dbParentChapter.chapterDateTime;
                dbChapter.year = dbParentChapter.parentDateTime.Year;
                dbChapter.month = dbParentChapter.parentDateTime.Month;
            }

            // finally update the chapter
            bool result = updateDBChapter(ctx, ref dbChapter);
            if (result)
                updatedEntryOut = dbChapter;

            return result;
        }

        public static bool updateDBChapter(myContext ctx, ref Chapter dbChapter)
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

        public static bool importNewChapterAndData(myContext ctx, ref Chapter chapter, String rtf)
        {
            // 1st import the chapter's data blob
            ChapterData chapterData = myDB.newChapterData(chapter.guid, rtf);
            if (!myDB.importNewDBChapterData(ctx, ref chapterData))
                return false;

            // 2nd now import the entry as a chapter into db
            if (!myDB.importNewDBChapter(ctx, ref chapter))
                return false;

            return true;
        }


        public static bool UpdateChapterAndData(myContext ctx, ref Chapter dbChapter, String decodedRtf)
        {
            // 1st load chapter's data blob
            ChapterData? dbChapterData = myDB.loadDBChapterData(ctx, dbChapter.guid);
            if (dbChapterData == null)
                return false;

            // existing chapter object, so update
            String encodedRtf = commonMethods.Base64Encode(decodedRtf);

            // 2nd set blob in chapterData and update
            dbChapterData.data = encodedRtf;
            if (!myDB.updateDBChapterData(ctx, ref dbChapterData))
                return false;
            
            // finally update chapter
            return updateDBChapter(ctx, ref dbChapter);
        }

        public static ChapterData? loadDBChapterData(myContext ctx, Guid guid)
        {
            // get original database chapter data using identifier
            ChapterData? dbChapterData = findDbChapterDataByGuid(ctx, guid);
            if (dbChapterData == null)
                return null;

            // success
            return dbChapterData;
        }
        public static bool purgeDBChapterData(myContext ctx, Guid guid)
        {
            // 1st load chapter's data blob
            ChapterData? dbChapterData = myDB.loadDBChapterData(ctx, guid);
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
        public static bool purgeDBChapter(myContext ctx, Guid guid)
        {
            Chapter? dbChapter = findDbChapterByGuid(ctx, guid);
            if (dbChapter == null)
                return false;

            // first purge this chapter's data
            if (!purgeDBChapterData(ctx, guid))
                return false;

            try
            {
                // finally purge the chapter from db
                bool result = ctx.chapterCollection.Delete(dbChapter.Id);
                ctx.db.Checkpoint();
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool purgeDBChapterRecursive(myContext ctx, Guid guid)
        {
            Chapter? parentChapter = findDbChapterByGuid(ctx, guid);
            if (parentChapter == null)
                return false;

            Queue<Chapter> queue = new Queue<Chapter>();
            queue.Enqueue(parentChapter);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                if (currentNode == null)
                    continue;

                IEnumerable<Chapter> children = findFirstLevelChildren(ctx, currentNode.guid);

                foreach (Chapter childNode in children)
                {
                    queue.Enqueue(childNode);
                    //purgeDBChapter(ctx, childNode.guid);
                }

                // finally purge the current parent entry
                purgeDBChapter(ctx, currentNode.guid);

            }
            return true;
        }

        public static bool markDBChapterDeletedRecursive(myContext ctx, Guid guid, bool mark)
        {
            Chapter? parentChapter = findDbChapterByGuid(ctx, guid);
            if (parentChapter == null)
                return false;

            Queue<Chapter> queue = new Queue<Chapter>();
            queue.Enqueue(parentChapter);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                if (currentNode == null)
                    continue;

                IEnumerable<Chapter> children = findFirstLevelChildren(ctx, currentNode.guid);

                foreach (Chapter childNode in children)
                {
                    queue.Enqueue(childNode);
                }

                // mark
                currentNode.IsDeleted = mark;

                // finally update chapter
                updateDBChapter(ctx, ref currentNode);
            }
            return true;
        }

        public static IEnumerable<Chapter> findFirstLevelChildren(myContext ctx, Guid parentGuid)
        {
            if (parentGuid == null)
                return null;

            IEnumerable<Chapter> results = ctx.chapterCollection.Find(x => x.parentGuid.Equals(parentGuid));
            return results;
        }

        // find chapter in database by it's index
        public static Chapter? findDbChapterByGuid(myContext ctx, Guid guid)
        {
            if (guid == null)
                return null;

            IEnumerable<Chapter> results = ctx.chapterCollection.Find(x => x.guid.Equals(guid));
            if (results.Count() <= 0)
                return null;

            return results.FirstOrDefault();

        }

        // find chapter in database by it's date and time
        public static Chapter? findDbChapterByDateTime(myContext ctx, DateTime dateTime)
        {
            IEnumerable<Chapter> results = ctx.chapterCollection.Find(x => x.Equals(dateTime));
            if (results.Count() <= 0)
                return null;

            return results.FirstOrDefault();

        }

        // find chapter in database by it's date and time
        public static Chapter? findDbChapterByDateTime(myContext ctx, int year, int month, int day)
        {
            IEnumerable<Chapter> results = ctx.chapterCollection.Find(x => x.chapterDateTime.Year == year && x.chapterDateTime.Month == month && x.chapterDateTime.Day == day);
            if (results.Count() <= 0)
                return null;

            return results.FirstOrDefault();
        }

    }
}
