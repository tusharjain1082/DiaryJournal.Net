using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryJournal.Net
{
    public class Chapter
    {
        public long Id { get; set; }
        public Guid guid { get; set; }
        public String? Title { get; set; } = "";
        public bool IsDeleted { get; set; } = false;
        public DateTime chapterDateTime { get; set; }
        public Guid parentGuid { get; set; } = Guid.Empty;
        public String HLFont { get; set; } = "";
        public String HLFontColor { get; set; } = "";
        public String HLBackColor { get; set; } = "";
        public NodeType nodeType { get; set; } = NodeType.EntryNode;
    }

    // note that we cannot store data blob with chapter in db, because when we load the chapter, entire blob is loaded as well, which is a bug.
    // so we keep the data blob in another table to prevent it from automatically loading.
    public class ChapterData
    {
        public long Id { get; set; }
        public Guid guid { get; set; }
        public String data { get; set; } = "";
    }

    public enum NodeType : int
    {
        RootNode = 0,
        YearNode = 1,
        MonthNode = 2,
        EntryNode = 3,
        LibraryNode = 4,
        TemplateNode = 5,
        AnyOrAll = 6
    }

    public enum EntryType : int
    {
        Xml = 0,
        Rtf = 1,
        Html = 2,
        Txt = 3
    }

    public class myNode
    {
        public Chapter? chapter = null;

        public myNode(ref Chapter chapter)
        {
            this.chapter = chapter;
        }
        public myNode(bool newChapter = true)
        {
            if (newChapter)
                this.chapter = new Chapter();
        }
    }


}
