using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DiaryJournal.Net
{
    public class Chapter
    {
        public long Id { get; set; }
        public UInt32 nodeID { get; set; } = 0;
        public UInt32 parentNodeID { get; set; } = 0;
        public String? Title { get; set; } = "";
        public bool IsDeleted { get; set; } = false;
        public DateTime chapterDateTime { get; set; }
        public String HLFont { get; set; } = "";
        public String HLFontColor { get; set; } = "";
        public String HLBackColor { get; set; } = "";
        public NodeType nodeType { get; set; } = NodeType.EntryNode;
        public DomainType domainType { get; set; } = DomainType.Journal;

    }

    // note that we cannot store data blob with chapter in db, because when we load the chapter, entire blob is loaded as well, which is a bug.
    // so we keep the data blob in another table to prevent it from automatically loading.
    public class ChapterData
    {
        public long Id { get; set; }
        public UInt32 nodeID { get; set; } = 0;
        public String data { get; set; } = "";
    }

    public enum NodeType : byte
    {
        SetNode = 0,
        YearNode = 1,
        MonthNode = 2,
        EntryNode = 3,
        TemplateNode = 4,
        AnyOrAll = 100
    }
    public enum DomainType : byte
    {
        Journal = 0,
        Library = 1
    }

    public enum EntryType : byte
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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct nodeItem
    {
        public UInt32 nodeID = 0;
        public UInt32 parentNodeID = 0;
    }

}
