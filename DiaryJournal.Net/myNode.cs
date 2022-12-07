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
        public Int64 Id { get; set; }
        public Int64 parentId { get; set; } = 0;
        public String? Title { get; set; } = "";
        public bool IsDeleted { get; set; } = false;
        public DateTime chapterDateTime { get; set; }
        public DateTime creationDateTime { get; set; }
        public DateTime modificationDateTime { get; set; }
        public DateTime deletionDateTime { get; set; }
        public String HLFont { get; set; } = "";
        public String HLFontColor { get; set; } = "";
        public String HLBackColor { get; set; } = "";
        public Int32 caretIndex { get; set; }
        public Int32 caretSelectionLength { get; set; }
        public NodeType nodeType { get; set; } = NodeType.EntryNode;
        public SpecialNodeType specialNodeType { get; set; } = SpecialNodeType.None;
        public DomainType domainType { get; set; } = DomainType.Journal;

    }

    // note that we cannot store data blob with chapter in db, because when we load the chapter, entire blob is loaded as well, which is a bug.
    // so we keep the data blob in another table to prevent it from automatically loading.
    public class ChapterData
    {
        public Int64 Id { get; set; }
        public String data { get; set; } = "";
    }

    public enum NodeType : byte
    {
        JournalNode = 0,
        LibraryNode = 1,
        SetNode = 2,
        YearNode = 3,
        MonthNode = 4,
        EntryNode = 5,
        TemplateNode = 6,
        LabelNode = 7,
        NonCalendarEntryNode = 8,
        AnyOrAll = 100
    }
    public enum SpecialNodeType : byte
    {
        SystemNode = 0,
        NonSystemNode = 1,
        None = 2,
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
        Txt = 3,
        Cfg = 4,
        Pdf = 5
    }

    public class myNode
    {
        public Chapter? chapter = null;
        public Int64 previousID = 0;
        public long DirectorySectionID = 0;

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

    // system nodes collection
    public class mySystemNodes
    {
        public myNode? JournalSystemNode = null;
        public myNode? LibrarySystemNode = null;
        public List<myNode> YearNodes = new List<myNode>();
        public List<myNode> MonthNodes = new List<myNode>();

        public const String JournalSystemNodeName = "Journal";
        public const String LibrarySystemNodeName = "Library";

        public mySystemNodes()
        {

        }
        public mySystemNodes(ref myNode? JournalSystemNode, ref myNode? LibrarySystemNode, 
            ref List<myNode>? YearNodes, ref List<myNode>? MonthNodes)
        {
            this.JournalSystemNode = JournalSystemNode;
            this.LibrarySystemNode = LibrarySystemNode;
            this.YearNodes = YearNodes;
            this.MonthNodes = MonthNodes;
        }

    }
}
