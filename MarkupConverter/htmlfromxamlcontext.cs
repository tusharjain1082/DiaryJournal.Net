using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MarkupConverter
{
    public class HtmlFromXamlContext
    {
        public delegate void HtmlWriteDelegate(XmlReader xamlReader, XmlWriter htmlWriter, StringBuilder inlineStyle, HtmlFromXamlContext context, string value);

        public delegate void HtmlOnWriteDelegate(XmlReader xamlReader, XmlWriter htmlWriter, StringBuilder inlineStyle, HtmlFromXamlContext context, ref string value);

        public delegate string HtmlOnWriteAttributeDelegate(string elementName, string attributeName, string attributeValue, string htmlValue);

        public HtmlFromXamlDocumentOptions Options { get; }

        public HtmlWriteDelegate OnWriteCustomProperty;

        public HtmlOnWriteDelegate OnWriteText { get; set; }

        public HtmlWriteDelegate OnWriteElementStyle { get; set; }

        public HtmlOnWriteAttributeDelegate OnWriteElementAttribute { get; set; }

        public Func<string, string> OnGetHtmlElementName { get; set; }

        public HtmlFromXamlContext(HtmlFromXamlDocumentOptions options)
        {
            Options = options;
        }

        public IReadOnlyList<HtmlFromXamlTableInfo> Tables => tables.AsReadOnly();

        public IReadOnlyList<string> ReaderElements => readerElements.AsReadOnly();

        public HtmlFromXamlTableInfo CurrentTable => (tableIndex >= 0 && tableIndex < Tables.Count) ? Tables[tableIndex] : null;

        internal HtmlFromXamlTableInfo AddTable()
        {
            var to = new HtmlFromXamlTableInfo();
            tables.Add(to);
            tableIndex = Tables.Count - 1;
            return to;
        }

        internal HtmlFromXamlTableInfo TableMove()
        {
            tableIndex++;
            return CurrentTable;
        }

        internal void AddReaderElement(string elementName)
        {
            readerElements.Add(elementName);
        }

        public void RemoveReaderElement(string elementName)
        {
            var i = readerElements.Count - 1;
            while (i >= 0 && readerElements[i] != elementName)
            {
                readerElements.RemoveAt(i);
                i--;
            }

            if (i >= 0 && readerElements[i] == elementName)
            {
                readerElements.RemoveAt(i);
                OnElementClosed(elementName);
            }
        }

        protected virtual void OnElementClosed(string elementName)
        {
            
        }

        private readonly List<HtmlFromXamlTableInfo> tables = new List<HtmlFromXamlTableInfo>();
        private readonly List<string> readerElements = new List<string>();
        private int tableIndex;
    }
}
