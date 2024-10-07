namespace MarkupConverter
{
    public class HtmlFromXamlDocumentOptions
    {
        public string InnerElement { get; set; }

        public string OuterElement { get; set; }

        public HtmlFromXamlDocumentOptions()
        {
            OuterElement = "html";
            InnerElement = "body";
        }
    }
}