using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace myJournal.Net
{
    public class myContext
    {
        public LiteDatabase? db = null;
        public ILiteCollection<Chapter>? chapterCollection = null;
        public ILiteCollection<ChapterData>? chapterDataCollection = null;
        public long totalEntries = 0;
        public String dbpath = "";
        public String dbfile = "";
        public List<Chapter>? identifiers = null;

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
            if (identifiers != null)
            {
                identifiers.Clear();
                identifiers = null;
            }
            chapterCollection = null;
            chapterDataCollection = null;
            db = null;
            totalEntries = 0;
            dbfile = dbpath = "";
        }
    }
}
