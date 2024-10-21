namespace MarkupConverter
{
    public class HtmlToXamlDocumentOptions
    {
        /// <summary>
        /// true indicates that we need a FlowDocument as a root element;
        /// false means that Section or Span elements will be used
        /// dependeing on StartFragment/EndFragment comments locations.
        /// </summary>
        public bool IsRootSection { get; set; }
    }
}
