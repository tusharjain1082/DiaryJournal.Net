using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Xml;
using System.IO;

namespace DiaryJournal.Net
{
    public static class xmlEntry
    {
        public const string entryTagName = "entry";
        public const string entryDateTimeTagName = "entryDateTime";
        public const string entryTitleTagName = "entryTitle";
        public const string entryNodeIDTagName = "entryNodeID";
        public const string entryParentNodeIDTagName = "entryParentNodeID";
        public const string entryTextTagName = "entryText";
        public const string entryParentDateTimeTagName = "entryParentDateTime";
        public const string entryHLFontTagName = "entryHighlightFont";
        public const string entryHLFontColorTagName = "entryHighlightFontColor";
        public const string entryHLBackColorTagName = "entryHighlightBackColor";
        public const string entryIsDeletedTagName = "entryIsDeleted";
        public const string entryYearTagName = "entryYear";
        public const string entryMonthTagName = "entryMonth";
        public const string entryNodeTypeTagName = "entryNodeType";
        public const string entrySpecialNodeTypeTagName = "entrySpecialNodeType";
        public const string entryDomainTypeTagName = "entryDomainType";

        public static bool fromXml(ref Chapter chapter, String file, ref String rtf, bool decode = false)
        {
            if (!File.Exists(file))
                return false;

            // load stream
            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            XmlElement root = doc.DocumentElement;

            // initialize chapter
            XmlNodeList list = doc.GetElementsByTagName(entryTagName);
            if (list.Count <= 0)
                return false;

            // we got entry element
            XmlElement entryElement = (XmlElement)list[0];

            // get all child elements

            list = entryElement.GetElementsByTagName(entryDateTimeTagName);
            if (list.Count <= 0)
                return false;

            // we got entry date and time child element
            XmlElement child0 = (XmlElement)list[0];
            chapter.chapterDateTime = DateTime.ParseExact(child0.InnerText, "yyyy-MM-dd-HH-mm-ss-fff",
                  System.Globalization.CultureInfo.InvariantCulture);

            list = entryElement.GetElementsByTagName(entryNodeIDTagName);
            if (list.Count <= 0)
                return false;

            // we got entry guid child element
            XmlElement child1 = (XmlElement)list[0];
            chapter.Id = Int64.Parse(child1.InnerText);

            list = entryElement.GetElementsByTagName(entryTitleTagName);
            if (list.Count <= 0)
                return false;


            // we got entry title child element
            XmlElement child2 = (XmlElement)list[0];
            chapter.Title = child2.InnerText;

            list = entryElement.GetElementsByTagName(entryParentNodeIDTagName);
            if (list.Count <= 0)
                return false;

            // we got entry parent guid element
            XmlElement child3 = (XmlElement)list[0];
            chapter.parentId = Int64.Parse(child3.InnerText);

            //list = entryElement.GetElementsByTagName(entryParentDateTimeTagName);
            //if (list.Count <= 0)
            //    return false;

            // we got entry parent guid element
            //XmlElement child4 = (XmlElement)list[0];
            //chapter.parentDateTime = DateTime.ParseExact(child4.InnerText, "yyyy-MM-dd-HH-mm-ss-fff", System.Globalization.CultureInfo.InvariantCulture);

            list = entryElement.GetElementsByTagName(entryHLFontTagName);
            if (list.Count <= 0)
                return false;

            // we got entry parent guid element
            XmlElement child5 = (XmlElement)list[0];
            chapter.HLFont = child5.InnerText;

            list = entryElement.GetElementsByTagName(entryHLFontColorTagName);
            if (list.Count <= 0)
                return false;

            // we got entry parent guid element
            XmlElement child6 = (XmlElement)list[0];
            chapter.HLFontColor = child6.InnerText;

            list = entryElement.GetElementsByTagName(entryHLBackColorTagName);
            if (list.Count <= 0)
                return false;

            // we got entry parent guid element
            XmlElement child7 = (XmlElement)list[0];
            chapter.HLBackColor = child7.InnerText;

            list = entryElement.GetElementsByTagName(entryIsDeletedTagName);
            if (list.Count <= 0)
                return false;

            // we got entry is deleted element
            XmlElement child8 = (XmlElement)list[0];
            chapter.IsDeleted = bool.Parse(child8.InnerText);

            list = entryElement.GetElementsByTagName(entryNodeTypeTagName);
            if (list.Count <= 0)
                return false;

            // we got node type element
            XmlElement child11 = (XmlElement)list[0];
            chapter.nodeType = (NodeType)Enum.Parse(typeof(NodeType), child11.InnerText);

            list = entryElement.GetElementsByTagName(entryDomainTypeTagName);
            if (list.Count <= 0)
                return false;

            // we got domain type element
            XmlElement child12 = (XmlElement)list[0];
            chapter.domainType = (DomainType)Enum.Parse(typeof(DomainType), child12.InnerText);

            list = entryElement.GetElementsByTagName(entrySpecialNodeTypeTagName);
            if (list.Count <= 0)
                return false;

            // we got special node type element
            XmlElement child13 = (XmlElement)list[0];
            chapter.specialNodeType = (SpecialNodeType)Enum.Parse(typeof(SpecialNodeType), child13.InnerText);

            // finally rtf data
            list = entryElement.GetElementsByTagName(entryTextTagName);
            if (list.Count <= 0)
                return false;

            // we got entry text child element
            XmlElement childData = (XmlElement)list[0];
            rtf = childData.InnerText;

            // decode
            if (rtf.Length > 0 && decode)
                rtf = commonMethods.Base64Decode(rtf);

            return true;
        }

        public static String toXml(ref Chapter chapter, String rtf, bool encode = false)
        {
            //xml Decalration:
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            //XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            XmlElement root = doc.DocumentElement;
            //doc.InsertBefore(xmlDeclaration, root);

            // create entry
            XmlElement entryElement = doc.CreateElement(string.Empty, entryTagName, string.Empty);
            doc.AppendChild(entryElement);
            XmlElement child0 = doc.CreateElement(string.Empty, entryDateTimeTagName, string.Empty);
            child0.InnerText = chapter.chapterDateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            entryElement.AppendChild(child0);
            XmlElement child1 = doc.CreateElement(string.Empty, entryTitleTagName, string.Empty);
            child1.InnerText = chapter.Title;
            entryElement.AppendChild(child1);
            XmlElement child2 = doc.CreateElement(string.Empty, entryNodeIDTagName, string.Empty);
            child2.InnerText = chapter.Id.ToString();
            entryElement.AppendChild(child2);
            XmlElement child3 = doc.CreateElement(string.Empty, entryParentNodeIDTagName, string.Empty);
            child3.InnerText = chapter.parentId.ToString();
            entryElement.AppendChild(child3);
            //XmlElement child4 = doc.CreateElement(string.Empty, entryParentDateTimeTagName, string.Empty);
            //child4.InnerText = chapter.parentDateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            //entryElement.AppendChild(child4);
            XmlElement child5 = doc.CreateElement(string.Empty, entryHLFontTagName, string.Empty);
            child5.InnerText = chapter.HLFont;
            entryElement.AppendChild(child5);
            XmlElement child6 = doc.CreateElement(string.Empty, entryHLFontColorTagName, string.Empty);
            child6.InnerText = chapter.HLFontColor;
            entryElement.AppendChild(child6);
            XmlElement child7 = doc.CreateElement(string.Empty, entryHLBackColorTagName, string.Empty);
            child7.InnerText = chapter.HLBackColor;
            entryElement.AppendChild(child7);
            XmlElement child8 = doc.CreateElement(string.Empty, entryIsDeletedTagName, string.Empty);
            child8.InnerText = chapter.IsDeleted.ToString();
            entryElement.AppendChild(child8);
            XmlElement child11 = doc.CreateElement(string.Empty, entryNodeTypeTagName, string.Empty);
            child11.InnerText = chapter.nodeType.ToString();
            entryElement.AppendChild(child11);
            XmlElement child12 = doc.CreateElement(string.Empty, entryDomainTypeTagName, string.Empty);
            child12.InnerText = chapter.domainType.ToString();
            entryElement.AppendChild(child12);
            XmlElement child13 = doc.CreateElement(string.Empty, entrySpecialNodeTypeTagName, string.Empty);
            child13.InnerText = chapter.specialNodeType.ToString();
            entryElement.AppendChild(child13);


            // finally rtf data
            if (encode)
                rtf = commonMethods.Base64Encode(rtf);

            XmlElement childData = doc.CreateElement(string.Empty, entryTextTagName, string.Empty);
            childData.InnerText = rtf;
            entryElement.AppendChild(childData);

            TextWriter writer = new StringWriterWithEncoding(Encoding.UTF8);
            //StringBuilder sb = new StringBuilder();
            //TextWriter writer = new System.IO.StreamWriter(ms, Encoding.UTF8);
            doc.Save(writer);
            writer.Flush();
            String output = writer.ToString();
            writer.Close();
            writer.Dispose();
            return output;
        }

        public sealed class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding encoding;

            public StringWriterWithEncoding() : this(Encoding.UTF8) { }

            public StringWriterWithEncoding(Encoding encoding)
            {
                this.encoding = encoding;
            }

            public override Encoding Encoding
            {
                get { return encoding; }
            }
        }
    }
}
