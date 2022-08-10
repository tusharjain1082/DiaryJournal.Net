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


    }

    // note that we cannot store data blob with chapter in db, because when we load the chapter, entire blob is loaded as well, which is a bug.
    // so we keep the data blob in another table to prevent it from automatically loading.
    public class ChapterData
    {
        public long Id { get; set; }
        public Guid guid { get; set; }
        public String? data { get; set; } = "";
    }
    public enum EntryType : int
    {
        Xml = 0,
        Rtf = 1,
        Html = 2
    }

    public static class myDB
    {
        public const String xmlExt = "xml";
        public const String rtfExt = "rtf";
        public const String htmlExt = "html";
        public const String xmlExtComplete = ".xml";
        public const String rtfExtComplete = ".rtf";
        public const String htmlExtComplete = ".html";
        public const String xmlExtSearchPattern = "*.xml";
        public const String rtfExtSearchPattern = "*.rtf";
        public const String htmlExtSearchPattern = "*.html";

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

        public static Chapter? newChapter(myContext ctx, ref long chaptersInDB, DateTime chapterDateTime = default(DateTime), bool DBImport = true, 
            Chapter? parent = null)
        {
            // first get the total number of chapters which exist in db
            long ChaptersCount = myDB.ChaptersCount(ctx);
            chaptersInDB = ChaptersCount;

            // create new chapter now
            Chapter chapter = new Chapter();
            if (chapterDateTime.Ticks == 0)
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
            myDB.importNewDBChapterData(ctx, chapterData);

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
            return chapter;
        }

        public static ChapterData? newChapterData(Guid guid, String? data = "")
        {
            ChapterData? chapterData = new ChapterData();
            chapterData.guid = guid;    
            chapterData.data = data;
            return chapterData;
        }

        public static bool importNewDBChapterData(myContext ctx, ChapterData chapterData)
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

        public static bool updateChapterByIDChapter(myContext ctx, Chapter identifier, String decodedRtf = "") 
        {
            Chapter? dbChapter = findDbChapterByGuid(ctx, identifier.guid);
            if (dbChapter == null)
                return false;

            // chapter found, update it and it's data
            return UpdateChapterAndData(ctx, dbChapter, decodedRtf);
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

        public static bool UpdateChapterAndData(myContext ctx, Chapter dbChapter, String decodedRtf)
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

        public static bool pureDBChapterRecursive(myContext ctx, Guid guid)
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
