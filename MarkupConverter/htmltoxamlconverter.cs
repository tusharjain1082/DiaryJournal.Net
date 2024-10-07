//---------------------------------------------------------------------------
// 
// File: HtmlXamlConverter.cs
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
// Description: Prototype for Html - Xaml conversion 
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Drawing;
using static System.Windows.Forms.DataFormats;
using System.Windows.Forms;
using commonMethods;
using System.Drawing;
using System.Windows.Media;

namespace MarkupConverter
{
    // DependencyProperty

    // TextElement

    /// <summary>
    /// HtmlToXamlConverter is a static class that takes an HTML string
    /// and converts it into XAML
    /// </summary>
    public static class HtmlToXamlConverter
    {
        public static string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        // ---------------------------------------------------------------------
        //
        // Internal Methods
        //
        // ---------------------------------------------------------------------

        #region Internal Methods

        /// <summary>
        /// Converts an html string into xaml string.
        /// </summary>
        /// <param name="htmlString">
        /// Input html which may be badly formated xml.
        /// </param>
        /// <param name="context">
        /// Conversion context
        /// </param>
        /// <returns>
        /// xml document representing XAML equivalent for the input html string.
        /// </returns>
        public static string ConvertHtmlToXaml(string htmlString, HtmlToXamlContext context)
        {
            var xamlTree = ConvertHtmlToXamlDocument(htmlString, context);
            return WriteXamlString(xamlTree);
        }

        public static string WriteXamlString(XDocument xamlDocument)
        {
            var builder = new StringBuilder();
            using (TextWriter writer = new StringWriter(builder))
            {
                xamlDocument.Save(writer, SaveOptions.DisableFormatting);
                var xaml = builder.ToString();
                var fdIndex = xaml.IndexOf("<FlowDocument");
                if(fdIndex > 0)
                {
                    xaml = xaml.Substring(fdIndex);
                }
                return xaml;
            }
        }

        /// <summary>
        /// Converts an html string into xaml string.
        /// </summary>
        /// <param name="htmlString">
        /// Input html which may be badly formated xml.
        /// </param>
        /// <param name="context">
        /// Conversion context
        /// </param>
        /// <returns>
        /// Well-formed xml representing XAML equivalent for the input html string.
        /// </returns>
        public static XDocument ConvertHtmlToXamlDocument(string htmlString, HtmlToXamlContext context)
        {
            // Create well-formed Xml from Html string
            XElement htmlElement = HtmlParser.ParseHtml(htmlString);

            // Decide what name to use as a root
            string rootElementName = context.Options.IsRootSection ? Xaml_Section : Xaml_FlowDocument;

            // Create an XDocument for generated xaml
            var xamlTree = new XDocument();
            var spacePreserve = new XAttribute(XNamespace.Xml + "space", "preserve");
            var xamlFlowDocumentElement = new XElement(XName.Get(rootElementName, XamlNamespace), spacePreserve);
            
            // Destination context is a stack of generated Xaml elements
            context.DestinationContext = new List<XElement>(10);
            PushDestination(xamlFlowDocumentElement, context);

            xamlTree.Add(xamlFlowDocumentElement);

            // Extract style definitions from all STYLE elements in the document
            context.Stylesheet = new CssStylesheet(htmlElement);

            // Source context is a stack of all elements - ancestors of a parentElement
            context.SourceContext = new List<XElement>(10);

            // Clear fragment parent
            InlineFragmentParentElement = null;

            // convert root html element
            AddBlock(xamlFlowDocumentElement, htmlElement, new Dictionary<string, string>(), context);

            CheckPop(context.DestinationContext, xamlFlowDocumentElement);

            // In case if the selected fragment is inline, extract it into a separate Span wrapper
            if (context.Options.IsRootSection)
            {
                xamlFlowDocumentElement = ExtractInlineFragment(xamlFlowDocumentElement);
            }

            return xamlTree;
        }

        private static void PushDestination(XElement element, HtmlToXamlContext context)
        {
            context.DestinationContext.Add(element);
            context.OnElementAdded?.Invoke(element, context);
        }

        /// <summary>
        /// Returns a value for an attribute by its name (ignoring casing)
        /// </summary>
        /// <param name="element">
        /// XElement in which we are trying to find the specified attribute
        /// </param>
        /// <param name="attributeName">
        /// String representing the attribute name to be searched for
        /// </param>
        /// <returns></returns>
        public static string GetAttribute(XElement element, string attributeName)
        {
            foreach (var attribute in element.Attributes())
            {
                if (attribute.Name.TagEquals(attributeName))
                {
                    return attribute.Value;
                }
            }

            return null;
        }

        public static string SetProperty(XElement element, string attributeName, IDictionary<string, string> properties, string propertyName = null)
        {
            var value = GetAttribute(element, attributeName);
            if (value != null)
            {
                properties[propertyName ?? attributeName] = value;
            }
            return value;
        }

        /// <summary>
        /// Returns string extracted from quotation marks
        /// </summary>
        /// <param name="value">
        /// String representing value enclosed in quotation marks
        /// </param>
        internal static string UnQuote(string value)
        {
            if (value.StartsWith("\"") && value.EndsWith("\"") || value.StartsWith("'") && value.EndsWith("'"))
            {
                value = value.Substring(1, value.Length - 2).Trim();
            }
            return value;
        }

        #endregion Internal Methods

        // ---------------------------------------------------------------------
        //
        // Private Methods
        //
        // ---------------------------------------------------------------------

        #region Private Methods

        /// <summary>
        /// Analyzes the given htmlElement expecting it to be converted
        /// into some of xaml Block elements and adds the converted block
        /// to the children collection of xamlParentElement.
        /// 
        /// Analyzes the given XElement htmlElement, recognizes it as some HTML element
        /// and adds it as a child to a xamlParentElement.
        /// In some cases several following siblings of the given htmlElement
        /// will be consumed too (e.g. LIs encountered without wrapping UL/OL, 
        /// which must be collected together and wrapped into one implicit List element).
        /// </summary>
        /// <param name="xamlParentElement">
        /// Parent xaml element, to which new converted element will be added
        /// </param>
        /// <param name="htmlNode">
        /// Source html element subject to convert to xaml.
        /// </param>
        /// <param name="inheritedProperties">
        /// Properties inherited from an outer context.
        /// </param>
        /// <param name="context">Conversion context</param>
        /// <returns>
        /// Last processed html node. Normally it should be the same htmlElement
        /// as was passed as a paramater, but in some irregular cases
        /// it could one of its following siblings.
        /// The caller must use this node to get to next sibling from it.
        /// </returns>
        private static XNode AddBlock(XElement xamlParentElement, XNode htmlNode, IDictionary<string, string> inheritedProperties, HtmlToXamlContext context)
        {
            if (htmlNode is XComment)
            {
                DefineInlineFragmentParent((XComment)htmlNode, /*xamlParentElement:*/null);
            }
            else if (htmlNode is XText)
            {
                htmlNode = AddImplicitParagraph(xamlParentElement, htmlNode, inheritedProperties, context);
            }
            else if (htmlNode is XElement)
            {
                // Identify element name
                XElement htmlElement = (XElement)htmlNode;

                string htmlElementName = htmlElement.Name.LocalName; // Keep the name case-sensitive to check xml names
                string htmlElementNamespace = htmlElement.Name.Namespace.NamespaceName;

                if (htmlElementNamespace != HtmlParser.XhtmlNamespace)
                {
                    // Non-html element. skip it
                    // Isn't it too agressive? What if this is just an error in html tag name?
                    // TODO: Consider skipping just a wparrer in recursing into the element tree,
                    // which may produce some garbage though coming from xml fragments.
                    return htmlElement;
                }

                // Put source element to the stack
                context.SourceContext.Add(htmlElement);

                // Convert the name to lowercase, because html elements are case-insensitive
                htmlElementName = htmlElementName.ToLower();

                // Switch to an appropriate kind of processing depending on html element name
                switch (htmlElementName)
                {
                    // Sections:
                    case "html":
                    case "body":
                    case "div":
                    case "form": // not a block according to xhtml spec
                    case "pre": // Renders text in a fixed-width font
                    case "blockquote":
                    case "caption":
                    case "center":

                    case "cite":
                        AddSection(xamlParentElement, htmlElement, inheritedProperties, context);
                        break;

                    // Paragraphs:
                    case "p":
                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                    case "h6":
                    case "nsrtitle":
                    case "textarea":
                    case "dd": // ???
                    case "dl": // ???
                    case "dt": // ???
                    case "tt": // ???
                        AddParagraph(xamlParentElement, htmlElement, inheritedProperties, context);
                        break;

                    case "ol":
                    case "ul":
                    case "dir": //  treat as UL element
                    case "menu": //  treat as UL element
                        // List element conversion
                        AddList(xamlParentElement, htmlElement, inheritedProperties, context);
                        break;
                    case "li":
                        // LI outside of OL/UL
                        // Collect all sibling LIs, wrap them into a List and then proceed with the element following the last of LIs
                        htmlNode = AddOrphanListItems(xamlParentElement, htmlElement, inheritedProperties, context);
                        break;

                    case "img":
                        // Image
                        AddImage(xamlParentElement, htmlElement, inheritedProperties, context);
                        break;

                    case "table":
                        // hand off to table parsing function which will perform special table syntax checks
                        AddTable(xamlParentElement, htmlElement, inheritedProperties, context);
                        break;

                    case "tbody":
                    case "tfoot":
                    case "thead":
                    case "tr":
                    case "td":
                    case "th":
                        // Table stuff without table wrapper
                        // TODO: add special-case processing here for elements that should be within tables when the
                        // parent element is NOT a table. If the parent element is a table they can be processed normally.
                        // we need to compare against the parent element here, we can't just break on a switch
                        goto default; // Thus we will skip this element as unknown, but still recurse into it.

                    case "style": // We already pre-processed all style elements. Ignore it now
                    case "meta":
                    case "head":
                    case "title":
                    case "script":
                        // Ignore these elements
                        break;

                    default:
                        // Wrap a sequence of inlines into an implicit paragraph
                        htmlNode = AddImplicitParagraph(xamlParentElement, htmlElement, inheritedProperties, context);
                        break;
                }

                // Remove the element from the stack
                CheckPop(context.SourceContext, htmlElement);
            }

            // Return last processed node
            return htmlNode;
        }

        private static void CheckPop(IList<XElement> stack, XElement element)
        {
            Debug.Assert(stack.Count > 0 && stack[stack.Count - 1] == element);
            stack.RemoveAt(stack.Count - 1);
        }

        // .............................................................
        //
        // Line Breaks
        //
        // .............................................................

        private static void AddBreak(XElement xamlParentElement, string htmlElementName)
        {
            // Create new xaml element corresponding to this html element
            XElement xamlLineBreak = new XElement(XName.Get(Xaml_LineBreak, XamlNamespace));
            xamlParentElement.Add(xamlLineBreak);
            if (htmlElementName == "hr")
            {
                xamlParentElement.Add(new XText("----------------------"));
                xamlLineBreak = new XElement(XName.Get(Xaml_LineBreak, XamlNamespace));
                xamlParentElement.Add(xamlLineBreak);
            }
        }

        // .............................................................
        //
        // Text Flow Elements
        //
        // .............................................................

        /// <summary>
        /// Generates Section or Paragraph element from DIV depending whether it contains any block elements or not
        /// </summary>
        /// <param name="xamlParentElement">
        /// XElement representing Xaml parent to which the converted element should be added
        /// </param>
        /// <param name="htmlElement">
        /// XElement representing Html element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// properties inherited from parent context
        /// </param>
        /// <param name="context">Conversion context</param>
        private static void AddSection(XElement xamlParentElement, XElement htmlElement, IDictionary<string, string> inheritedProperties, HtmlToXamlContext context)
        {
            // Analyze the content of htmlElement to decide what xaml element to choose - Section or Paragraph.
            // If this Div has at least one block child then we need to use Section, otherwise use Paragraph
            bool htmlElementContainsBlocks = false;
            for (XNode htmlChildNode = htmlElement.FirstNode; htmlChildNode != null; htmlChildNode = htmlChildNode.NextNode)
            {
                if (htmlChildNode is XElement)
                {
                    if (HtmlSchema.IsBlockElement(((XElement)htmlChildNode).Name.LocalName))
                    {
                        htmlElementContainsBlocks = true;
                        break;
                    }
                }
            }

            if (!htmlElementContainsBlocks)
            {
                // The Div does not contain any block elements, so we can treat it as a Paragraph
                AddParagraph(xamlParentElement, htmlElement, inheritedProperties, context);
            }
            else
            {
                // The Div has some nested blocks, so we treat it as a Section

                // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
                IDictionary<string, string> localProperties;
                IDictionary<string, string> currentProperties = GetElementProperties(htmlElement, inheritedProperties, out localProperties, context);

                // Create a XAML element corresponding to this html element
                XElement xamlElement = new XElement(XName.Get(Xaml_Section, XamlNamespace));
                PushDestination(xamlElement, context);
                ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/true, context);

                // this element is a single child of the parent element which is also a section or flow document
                // we can flatten the hierarchy and apply style to the parent element
                if (htmlElement.Parent?.Elements().Count() == 1 && (xamlParentElement.Name.LocalName == Xaml_Section || xamlParentElement.Name.LocalName == Xaml_FlowDocument))
                {
                    ApplyLocalProperties(xamlParentElement, localProperties, /*isBlock:*/true, context);
                    CheckPop(context.DestinationContext, xamlElement);
                    xamlElement = xamlParentElement;
                }
                // Decide whether we can unwrap this element as not having any formatting significance.
                else if (!xamlElement.HasAttributes)
                {
                    // This elements is a group of block elements whitout any additional formatting.
                    // We can add blocks directly to xamlParentElement and avoid
                    // creating unnecessary Sections nesting.
                    CheckPop(context.DestinationContext, xamlElement);
                    xamlElement = xamlParentElement;
                }

                // Recurse into element subtree
                for (XNode htmlChildNode = htmlElement.FirstNode; htmlChildNode != null; htmlChildNode = htmlChildNode != null ? htmlChildNode.NextNode : null)
                {
                    htmlChildNode = AddBlock(xamlElement, htmlChildNode, currentProperties, context);
                }

                // Add the new element to the parent.
                if (xamlElement != xamlParentElement)
                {
                    xamlParentElement.Add(xamlElement);
                    CheckPop(context.DestinationContext, xamlElement);
                }
            }
        }

        /// <summary>
        /// Generates Paragraph element from P, H1-H7, Center etc.
        /// </summary>
        /// <param name="xamlParentElement">
        /// XElement representing Xaml parent to which the converted element should be added
        /// </param>
        /// <param name="htmlElement">
        /// XElement representing Html element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// properties inherited from parent context
        /// </param>
        /// <param name="context">Conversion context</param>
        private static void AddParagraph(XElement xamlParentElement, XElement htmlElement, IDictionary<string, string> inheritedProperties, HtmlToXamlContext context)
        {
            // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
            IDictionary<string, string> localProperties;
            IDictionary<string, string> currentProperties = GetElementProperties(htmlElement, inheritedProperties, out localProperties, context);

            // Create a XAML element corresponding to this html element
            XElement xamlElement = new XElement(XName.Get(Xaml_Paragraph, XamlNamespace));
            PushDestination(xamlElement, context);
            ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/true, context);

            // Recurse into element subtree
            for (XNode htmlChildNode = htmlElement.FirstNode; htmlChildNode != null; htmlChildNode = htmlChildNode.NextNode)
            {
                AddInline(xamlElement, htmlChildNode, currentProperties, context);
            }

            // Add the new element to the parent.
            xamlParentElement.Add(xamlElement);
            CheckPop(context.DestinationContext, xamlElement);
        }

        /// <summary>
        /// Creates a Paragraph element and adds all nodes starting from htmlNode
        /// converted to appropriate Inlines.
        /// </summary>
        /// <param name="xamlParentElement">
        /// XElement representing Xaml parent to which the converted element should be added
        /// </param>
        /// <param name="htmlNode">
        /// XNode starting a collection of implicitly wrapped inlines.
        /// </param>
        /// <param name="inheritedProperties">
        /// properties inherited from parent context
        /// </param>
        /// <param name="context">Conversion context</param>
        /// <returns>
        /// The last htmlNode added to the implicit paragraph
        /// </returns>
        private static XNode AddImplicitParagraph(XElement xamlParentElement, XNode htmlNode, IDictionary<string, string> inheritedProperties, HtmlToXamlContext context)
        {
            // Collect all non-block elements and wrap them into implicit Paragraph
            XElement xamlParagraph = new XElement(XName.Get(Xaml_Paragraph, XamlNamespace));
            PushDestination(xamlParagraph, context);
            XNode lastNodeProcessed = null;
            while (htmlNode != null)
            {
                if (htmlNode is XComment)
                {
                    DefineInlineFragmentParent((XComment)htmlNode, /*xamlParentElement:*/null);
                }
                else if (htmlNode is XText)
                {
                    if (((XText)htmlNode).Value.Trim().Length > 0)
                    {
                        AddTextRun(xamlParagraph, ((XText)htmlNode).Value, context);
                    }
                }
                else if (htmlNode is XElement)
                {
                    if (HtmlSchema.IsBlockElement(((XElement)htmlNode).Name.LocalName))
                    {
                        // The sequence of non-blocked inlines ended. Stop implicit loop here.
                        break;
                    }
                    else
                    {
                        AddInline(xamlParagraph, (XElement)htmlNode, inheritedProperties, context);
                    }
                }

                // Store last processed node to return it at the end
                lastNodeProcessed = htmlNode;
                htmlNode = htmlNode.NextNode;
            }

            // Add the Paragraph to the parent
            // If only whitespaces and commens have been encountered,
            // then we have nothing to add in implicit paragraph; forget it.
            if (xamlParagraph.DescendantNodes().Any())
            {
                xamlParentElement.Add(xamlParagraph);
            }

            CheckPop(context.DestinationContext, xamlParagraph);

            // Need to return last processed node
            return lastNodeProcessed;
        }

        // .............................................................
        //
        // Inline Elements
        //
        // .............................................................

        private static void AddInline(XElement xamlParentElement, XNode htmlNode, IDictionary<string, string> inheritedProperties, HtmlToXamlContext context)
        {
            if (htmlNode is XComment)
            {
                DefineInlineFragmentParent((XComment)htmlNode, xamlParentElement);
            }
            else if (htmlNode is XText)
            {
                AddTextRun(xamlParentElement, ((XText)htmlNode).Value, context);
            }
            else if (htmlNode is XElement)
            {
                XElement htmlElement = (XElement)htmlNode;

                // Check whether this is an html element
                if (htmlElement.Name.Namespace.NamespaceName != HtmlParser.XhtmlNamespace)
                {
                    return; // Skip non-html elements
                }

                // Identify element name
                string htmlElementName = htmlElement.Name.LocalName.ToLower();

                // Put source element to the stack
                context.SourceContext.Add(htmlElement);

                switch (htmlElementName)
                {
                    case "a":
                        AddHyperlink(xamlParentElement, htmlElement, inheritedProperties, context);
                        break;
                    case "img":
                        AddImage(xamlParentElement, htmlElement, inheritedProperties, context);
                        break;
                    case "br":
                    case "hr":
                        AddBreak(xamlParentElement, htmlElementName);
                        break;
                    default:
                        if (HtmlSchema.IsInlineElement(htmlElementName) || HtmlSchema.IsBlockElement(htmlElementName))
                        {
                            // Note: actually we do not expect block elements here,
                            // but if it happens to be here, we will treat it as a Span.

                            AddSpanOrRun(xamlParentElement, htmlElement, inheritedProperties, context);
                        }
                        break;
                }
                // Ignore all other elements non-(block/inline/image)

                // Remove the element from the stack
                CheckPop(context.SourceContext, htmlElement);
            }
        }

        private static void AddSpanOrRun(XElement xamlParentElement, XElement htmlElement, IDictionary<string, string> inheritedProperties, HtmlToXamlContext context)
        {
            // Decide what XAML element to use for this inline element.
            // Check whether it contains any nested inlines
            bool elementHasChildren = false;
            for (XNode htmlNode = htmlElement.FirstNode; htmlNode != null; htmlNode = htmlNode.NextNode)
            {
                if (htmlNode is XElement)
                {
                    string htmlChildName = ((XElement)htmlNode).Name.LocalName.ToLower();
                    if (HtmlSchema.IsInlineElement(htmlChildName) || HtmlSchema.IsBlockElement(htmlChildName) ||
                        ((XElement)htmlNode).Name.TagEquals("img", "br", "hr"))
                    {
                        elementHasChildren = true;
                        break;
                    }
                }
            }

            string xamlElementName = elementHasChildren ? Xaml_Span : Xaml_Run;

            // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
            IDictionary<string, string> localProperties;
            IDictionary<string, string> currentProperties = GetElementProperties(htmlElement, inheritedProperties, out localProperties, context);

            // Create a XAML element corresponding to this html element
            XElement xamlElement = new XElement(XName.Get(xamlElementName, XamlNamespace));
            PushDestination(xamlElement, context);
            ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/false, context);

            // Recurse into element subtree
            for (XNode htmlChildNode = htmlElement.FirstNode; htmlChildNode != null; htmlChildNode = htmlChildNode.NextNode)
            {
                AddInline(xamlElement, htmlChildNode, currentProperties, context);
            }

            // Add the new element to the parent.
            xamlParentElement.Add(xamlElement);
            CheckPop(context.DestinationContext, xamlElement);
        }

        // Adds a text run to a xaml tree
        private static void AddTextRun(XElement xamlElement, string textData, HtmlToXamlContext context)
        {
            // Remove control characters
            for (int i = 0; i < textData.Length; i++)
            {
                if (Char.IsControl(textData[i]))
                {
                    textData = textData.Remove(i--, 1);  // decrement i to compensate for character removal
                }
            }

            // Replace No-Breaks by spaces (160 is a code of &nbsp; entity in html)
            //  This is a work around since WPF/XAML does not support &nbsp.
            textData = textData.Replace((char)160, ' ');

            context.OnWriteText?.Invoke(xamlElement, context, ref textData);

            if (textData.Length > 0)
            {
                xamlElement.Add(new XText(textData));
            }
        }

        private static void AddHyperlink(XElement xamlParentElement, XElement htmlElement, IDictionary<string, string> inheritedProperties, HtmlToXamlContext context)
        {
            // 1st create uicontainer
            //XElement xamlContainerElement1 = null;
            //XElement xamlContainerElement2 = null;
            //            if (xamlParentElement.Name.LocalName == Xaml_Section)
            //              xamlContainerElement = new XElement(XName.Get(Xaml_BlockUIContainer, XamlNamespace));
            //            else if (xamlParentElement.Name.LocalName == Xaml_Paragraph)
            //              xamlContainerElement = new XElement(XName.Get(Xaml_InlineUIContainer, XamlNamespace));
            //if (xamlParentElement.Name.LocalName == Xaml_Span)
            //xamlContainerElement = new XElement(XName.Get(Xaml_InlineUIContainer, XamlNamespace));
            //xamlContainerElement1 = new XElement(XName.Get(Xaml_TextBlock, XamlNamespace));
            //PushDestination(xamlContainerElement1, context);
            //xamlContainerElement2 = new XElement(XName.Get(Xaml_Span, XamlNamespace));
            //PushDestination(xamlContainerElement2, context);

            // Convert href attribute into NavigateUri and TargetName
            string href = GetAttribute(htmlElement, "href");
            if (href == null)
            {
                // When href attribute is missing - ignore the hyperlink
                AddSpanOrRun(xamlParentElement, htmlElement, inheritedProperties, context);
            }
            else
            {
                // When href attribute is missing - ignore the hyperlink
                AddSpanOrRun(xamlParentElement, htmlElement, inheritedProperties, context);

                // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
                //IDictionary<string, string> localProperties;
                //IDictionary<string, string> currentProperties = GetElementProperties(htmlElement, inheritedProperties, out localProperties, context);

                // Create a XAML element corresponding to this html element
                //XElement xamlElement = new XElement(XName.Get(Xaml_Hyperlink, XamlNamespace)); //Xaml_Hyperlink, XamlNamespace));
                //PushDestination(xamlElement, context);
                //ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/false, context);

                //string[] hrefParts = href.Split(new char[] { '#' });
                //if (hrefParts.Length > 0 && hrefParts[0].Trim().Length > 0)
                //{
                //    xamlElement.SetAttributeValue(Xaml_Hyperlink_NavigateUri, hrefParts[0].Trim());
                //}
                //if (hrefParts.Length == 2 && hrefParts[1].Trim().Length > 0)
                //{
                 //   xamlElement.SetAttributeValue(Xaml_Hyperlink_TargetName, hrefParts[1].Trim());
                //}

                // Recurse into element subtree
                //for (XNode htmlChildNode = htmlElement.FirstNode; htmlChildNode != null; htmlChildNode = htmlChildNode.NextNode)
               // {
                //    AddInline(xamlElement, htmlChildNode, currentProperties, context);
                //}

                // tushar: Add the Image element to parent element - if it is not empty
                //xamlBlockUIContainerElement.Add(xamlImageElement);
                //xamlContainerElement2.Add(xamlElement);
                //xamlContainerElement1.Add(xamlElement);
                //CheckPop(context.DestinationContext, xamlElement);
                //xamlInlineUIContainerElement.Add(xamlImageElement);
                //CheckPop(context.DestinationContext, xamlImageElement);

                // tushar: Add the parent element to xaml tree - if it is not empty
                //xamlContainerElement1.Add(xamlContainerElement2);
                //CheckPop(context.DestinationContext, xamlContainerElement2);
                //xamlParentElement.Add(xamlContainerElement1);
                //CheckPop(context.DestinationContext, xamlContainerElement1);
                //xamlParentElement.Add(xamlBlockUIContainerElement);
                //CheckPop(context.DestinationContext, xamlBlockUIContainerElement);
                //xamlParentElement.Add(xamlInlineUIContainerElement);
                //CheckPop(context.DestinationContext, xamlInlineUIContainerElement);

                // Add the new element to the parent.
                // original
                //xamlParentElement.Add(xamlElement);
                //CheckPop(context.DestinationContext, xamlElement);
            }
        }

        // Stores a parent xaml element for the case when selected fragment is inline.
        private static XElement InlineFragmentParentElement;

        // Called when html comment is encountered to store a parent element
        // for the case when the fragment is inline - to extract it to a separate
        // Span wrapper after the conversion.
        private static void DefineInlineFragmentParent(XComment htmlComment, XElement xamlParentElement)
        {
            if (htmlComment.Value == "StartFragment")
            {
                InlineFragmentParentElement = xamlParentElement;
            }
            else if (htmlComment.Value == "EndFragment")
            {
                if (InlineFragmentParentElement == null && xamlParentElement != null)
                {
                    // Normally this cannot happen if comments produced by correct copying code
                    // in Word or IE, but when it is produced manually then fragment boundary
                    // markers can be inconsistent. In this case StartFragment takes precedence,
                    // but if it is not set, then we get the value from EndFragment marker.
                    InlineFragmentParentElement = xamlParentElement;
                }
            }
        }

        // Extracts a content of an element stored as InlineFragmentParentElement
        // into a separate Span wrapper.
        // Note: when selected content does not cross paragraph boundaries,
        // the fragment is marked within
        private static XElement ExtractInlineFragment(XElement xamlFlowDocumentElement)
        {
            if (InlineFragmentParentElement != null)
            {
                if (InlineFragmentParentElement.Name.LocalName == Xaml_Span)
                {
                    xamlFlowDocumentElement = InlineFragmentParentElement;
                }
                else
                {
                    xamlFlowDocumentElement = new XElement(XName.Get(Xaml_Span, XamlNamespace));
                    foreach (var copyNode in InlineFragmentParentElement.Elements())
                    {
                        copyNode.Remove();
                        xamlFlowDocumentElement.Add(copyNode);
                    }
                }
            }

            return xamlFlowDocumentElement;
        }

        // .............................................................
        //
        // Images
        //
        // .............................................................

        private static void AddImage(XElement xamlParentElement, XElement htmlElement, IDictionary<string, string> inheritedProperties, HtmlToXamlContext context)
        {
            IDictionary<string, string> localProperties;
            IDictionary<string, string> currentProperties = GetElementProperties(htmlElement, inheritedProperties, out localProperties, context);
            var image = new HtmlXamlImage();


            // 1st create uicontainer
            XElement xamlContainerElement = null;
            if (xamlParentElement.Name.LocalName == Xaml_Section)
                xamlContainerElement = new XElement(XName.Get(Xaml_BlockUIContainer, XamlNamespace));
            else if (xamlParentElement.Name.LocalName == Xaml_Paragraph)
                xamlContainerElement = new XElement(XName.Get(Xaml_InlineUIContainer, XamlNamespace));
            else if (xamlParentElement.Name.LocalName == Xaml_Span)
                xamlContainerElement = new XElement(XName.Get(Xaml_InlineUIContainer, XamlNamespace));
            else if (xamlParentElement.Name.LocalName == Xaml_FlowDocument)
                xamlContainerElement = new XElement(XName.Get(Xaml_Paragraph, XamlNamespace));
            else 
                xamlContainerElement = new XElement(XName.Get(Xaml_BlockUIContainer, XamlNamespace));

            PushDestination(xamlContainerElement, context);

            //XElement xamlBlockUIContainerElement = new XElement(XName.Get(Xaml_BlockUIContainer, XamlNamespace));
            //PushDestination(xamlBlockUIContainerElement, context);
            //XElement xamlInlineUIContainerElement = new XElement(XName.Get(Xaml_InlineUIContainer, XamlNamespace));
            //PushDestination(xamlInlineUIContainerElement, context);

            // 2nd tushar: Create Xaml Image element
            XElement xamlImageElement = new XElement(XName.Get(Xaml_Image, XamlNamespace));
            PushDestination(xamlImageElement, context);

            string widthText;
            double width;
            if (currentProperties.TryGetValue("width", out widthText) && TryGetLengthValue(widthText, out width))
            {
                image.Width = (int)width;
                // tushar: set xaml image node attributes
                xamlImageElement.SetAttributeValue(Xaml_Width, (int)width);
            }
            string heightText;
            double height;
            if (currentProperties.TryGetValue("height", out heightText) && TryGetLengthValue(heightText, out height))
            {
                image.Height = (int)height;
                // tushar: set xaml image node attributes
                xamlImageElement.SetAttributeValue(Xaml_Height, (int)height);
            }
            string alt;
            if (currentProperties.TryGetValue("alt", out alt))
            {
                image.Alt = alt;
            }

            string src;
            if (currentProperties.TryGetValue("src", out src) && !string.IsNullOrEmpty(src))
            {
                //static string _base64ImageHeader = "data:";
                //static string _base64ImageTag = ";base64,";

                if (src.StartsWith(_base64ImageHeader))
                {
                    var baseIndex = src.IndexOf(_base64ImageTag);
                    if (baseIndex > 0)
                    {
                        image.ContentsBase64 = src.Substring(baseIndex + _base64ImageTag.Length);
                        image.MimeType = src.Substring(_base64ImageHeader.Length, baseIndex - _base64ImageHeader.Length);
                        image.IsInline = true;
                    }
                }
                else
                {
                    // url
                    /* original
                    Bitmap? bitmap = commonMethods.DownloadImage(src, "", null);
                    if (bitmap != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bitmap.Save(ms, bitmap.RawFormat);
                            src = _base64ImageHeader + _base64ImageTag + Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    */

                    
                    String tmpFileName = commonMethods.commonMethods.autoCreateInitTmpFile();
                    //Bitmap? bitmap = 
                    var task = commonMethods.commonMethods.DownloadImageAsync(src, tmpFileName, false,0,0);
                    task.Wait();
                    Bitmap? bitmap = task.Result;

                    /*
                    if (bitmap != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bitmap.Save(ms, bitmap.RawFormat);
                            //commonMethods.commonMethods.SaveToPngWpf(bitmap, ms);
                            File.WriteAllBytes(tmpFileName, ms.ToArray());
                        }
                    }
                    */

                    // tushar: set xaml image node attributes
                    var uri = new System.Uri(tmpFileName);
                    var converted = uri.AbsoluteUri;
                    xamlImageElement.SetAttributeValue(Xaml_TagProperty, tmpFileName);
                    //xamlImageElement.SetAttributeValue(Xaml_Source, src);
                    xamlImageElement.SetAttributeValue(Xaml_Source, converted);
                    xamlImageElement.SetAttributeValue(Xaml_BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
                    xamlImageElement.SetAttributeValue(Xaml_RenderOptionsEdgeModeProperty, "Aliased");
                    xamlImageElement.SetAttributeValue("HorizontalAlignment", "Left");
                    xamlImageElement.SetAttributeValue("VerticalAlignment", "Top");
                    xamlImageElement.SetAttributeValue("SnapsToDevicePixels", "True");


                }
            }
            string floatValue;
            if (currentProperties.TryGetValue("float", out floatValue) && !string.IsNullOrEmpty(src))
            {
                switch (floatValue?.ToLower())
                {
                    case "left":
                        image.Float = HtmlXamlImage.FloatDirection.Left;
                        break;
                    case "right":
                        image.Float = HtmlXamlImage.FloatDirection.Right;
                        break;
                    default:
                        image.Float = HtmlXamlImage.FloatDirection.None;
                        break;
                }
            }

            // tushar: Add the Image element to parent element - if it is not empty
            //xamlBlockUIContainerElement.Add(xamlImageElement);
            xamlContainerElement.Add(xamlImageElement);
            CheckPop(context.DestinationContext, xamlImageElement);
            //xamlInlineUIContainerElement.Add(xamlImageElement);
            //CheckPop(context.DestinationContext, xamlImageElement);

            // tushar: Add the parent element to xaml tree - if it is not empty
            xamlParentElement.Add(xamlContainerElement);
            CheckPop(context.DestinationContext, xamlContainerElement);
            //xamlParentElement.Add(xamlBlockUIContainerElement);
            //CheckPop(context.DestinationContext, xamlBlockUIContainerElement);
            //xamlParentElement.Add(xamlInlineUIContainerElement);
            //CheckPop(context.DestinationContext, xamlInlineUIContainerElement);

            //xamlParentElement.Add(xamlImageElement);
            //CheckPop(context.DestinationContext, xamlImageElement);

            context.OnProcessImage?.Invoke(image, xamlParentElement, context);
        }

        // .............................................................
        //
        // Lists
        //
        // .............................................................

        /// <summary>
        /// Converts Html ul or ol element into Xaml list element. During conversion if the ul/ol element has any children 
        /// that are not li elements, they are ignored and not added to the list element
        /// </summary>
        /// <param name="xamlParentElement">
        /// XElement representing Xaml parent to which the converted element should be added
        /// </param>
        /// <param name="htmlListElement">
        /// XElement representing Html ul/ol element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// properties inherited from parent context
        /// </param>
        /// <param name="context">Conversion context</param>
        private static void AddList(XElement xamlParentElement, XElement htmlListElement, IDictionary<string, string> inheritedProperties, HtmlToXamlContext context)
        {
            IDictionary<string, string> localProperties;
            IDictionary<string, string> currentProperties = GetElementProperties(htmlListElement, inheritedProperties, out localProperties, context);

            // Create Xaml List element
            XElement xamlListElement = new XElement(XName.Get(Xaml_List, XamlNamespace));
            PushDestination(xamlListElement, context);

            // Set default list markers
            if (htmlListElement.Name.TagEquals("ol"))
            {
                // Ordered list
                xamlListElement.SetAttributeValue(Xaml_List_MarkerStyle, Xaml_List_MarkerStyle_Decimal);
            }
            else
            {
                // Unordered list - all elements other than OL treated as unordered lists
                xamlListElement.SetAttributeValue(Xaml_List_MarkerStyle, Xaml_List_MarkerStyle_Disc);
            }

            // Apply local properties to list to set marker attribute if specified
            // TODO: Should we have separate list attribute processing function?
            ApplyLocalProperties(xamlListElement, localProperties, /*isBlock:*/true, context);

            // Recurse into list subtree
            foreach (var htmlChildNode in htmlListElement.Elements())
            {
                if (htmlChildNode is XElement && htmlChildNode.Name.TagEquals("li"))
                {
                    context.SourceContext.Add(htmlChildNode);
                    AddListItem(xamlListElement, htmlChildNode, currentProperties, context);

                    CheckPop(context.SourceContext, htmlChildNode);
                }
                else
                {
                    // Not an li element. Add it to previous ListBoxItem
                    //  We need to append the content to the end
                    // of a previous list item.
                }
            }

            // Add the List element to xaml tree - if it is not empty
            if (xamlListElement.HasElements)
            {
                xamlParentElement.Add(xamlListElement);
            }
            CheckPop(context.DestinationContext, xamlListElement);
        }

        /// <summary>
        /// If li items are found without a parent ul/ol element in Html string, creates xamlListElement as their parent and adds
        /// them to it. If the previously added node to the same xamlParentElement was a List, adds the elements to that list.
        /// Otherwise, we create a new xamlListElement and add them to it. Elements are added as long as li elements appear sequentially.
        /// The first non-li or text node stops the addition.
        /// </summary>
        /// <param name="xamlParentElement">
        /// Parent element for the list
        /// </param>
        /// <param name="htmlLIElement">
        /// Start Html li element without parent list
        /// </param>
        /// <param name="inheritedProperties">
        /// Properties inherited from parent context
        /// </param>
        /// <param name="context">Conversion context</param>
        /// <returns>
        /// XNode representing the first non-li node in the input after one or more li's have been processed.
        /// </returns>
        private static XElement AddOrphanListItems(XElement xamlParentElement, XElement htmlLIElement, IDictionary<string, string> inheritedProperties, HtmlToXamlContext context)
        {
            Debug.Assert(htmlLIElement.Name.TagEquals("li"));

            XElement lastProcessedListItemElement = null;

            // Find out the last element attached to the xamlParentElement, which is the previous sibling of this node
            var xamlListItemElementPreviousSibling = xamlParentElement.Elements().LastOrDefault();
            XElement xamlListElement;
            if (xamlListItemElementPreviousSibling != null && xamlListItemElementPreviousSibling.Name.LocalName == Xaml_List)
            {
                // Previously added Xaml element was a list. We will add the new li to it
                xamlListElement = xamlListItemElementPreviousSibling;
            }
            else
            {
                // No list element near. Create our own.
                xamlListElement = new XElement(XName.Get(Xaml_List, XamlNamespace));
                xamlParentElement.Add(xamlListElement);
            }

            var htmlChildNode = htmlLIElement;
            string htmlChildNodeName = htmlChildNode == null ? null : htmlChildNode.Name.LocalName.ToLower();

            //  Current element properties missed here.
            //currentProperties = GetElementProperties(htmlLIElement, inheritedProperties, out localProperties, stylesheet);

            // Add li elements to the parent xamlListElement we created as long as they appear sequentially
            // Use properties inherited from xamlParentElement for context 
            while (htmlChildNode != null && htmlChildNodeName == "li")
            {
                AddListItem(xamlListElement, htmlChildNode, inheritedProperties, context);
                lastProcessedListItemElement = htmlChildNode;
                htmlChildNode = htmlChildNode.ElementsAfterSelf().FirstOrDefault();
                htmlChildNodeName = htmlChildNode == null ? null : htmlChildNode.Name.LocalName.ToLower();
            }

            return lastProcessedListItemElement;
        }

        /// <summary>
        /// Converts htmlLIElement into Xaml ListItem element, and appends it to the parent xamlListElement
        /// </summary>
        /// <param name="xamlListElement">
        /// XElement representing Xaml List element to which the converted td/th should be added
        /// </param>
        /// <param name="htmlLIElement">
        /// XElement representing Html li element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// Properties inherited from parent context
        /// </param>
        /// <param name="context">Conversion context</param>
        private static void AddListItem(XElement xamlListElement, XElement htmlLIElement, IDictionary<string, string> inheritedProperties, HtmlToXamlContext context)
        {
            // Parameter validation
            Debug.Assert(xamlListElement != null);
            Debug.Assert(xamlListElement.Name.LocalName == Xaml_List);
            Debug.Assert(htmlLIElement != null);
            Debug.Assert(htmlLIElement.Name.TagEquals("li"));
            Debug.Assert(inheritedProperties != null);

            IDictionary<string, string> localProperties;
            IDictionary<string, string> currentProperties = GetElementProperties(htmlLIElement, inheritedProperties, out localProperties, context);

            XElement xamlListItemElement = new XElement(XName.Get(Xaml_ListItem, XamlNamespace));
            PushDestination(xamlListItemElement, context);

            // TODO: process local properties for li element

            // Process children of the ListItem
            for (XNode htmlChildNode = htmlLIElement.FirstNode; htmlChildNode != null; htmlChildNode = htmlChildNode != null ? htmlChildNode.NextNode : null)
            {
                htmlChildNode = AddBlock(xamlListItemElement, htmlChildNode, currentProperties, context);
            }

            // Add resulting ListBoxItem to a xaml parent
            xamlListElement.Add(xamlListItemElement);
            CheckPop(context.DestinationContext, xamlListItemElement);
        }

        // .............................................................
        //
        // Tables
        //
        // .............................................................

        /// <summary>
        /// Converts htmlTableElement to a Xaml Table element. Adds tbody elements if they are missing so
        /// that a resulting Xaml Table element is properly formed.
        /// </summary>
        /// <param name="xamlParentElement">
        /// Parent xaml element to which a converted table must be added.
        /// </param>
        /// <param name="htmlTableElement">
        /// XElement reprsenting the Html table element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// Dictionary representing properties inherited from parent context. 
        /// </param>
        /// <param name="context">Conversion context</param>
        private static void AddTable(XElement xamlParentElement, XElement htmlTableElement, IDictionary<string, string> inheritedProperties, HtmlToXamlContext context)
        {
            // Parameter validation
            Debug.Assert(htmlTableElement.Name.TagEquals("table"));
            Debug.Assert(xamlParentElement != null);
            Debug.Assert(inheritedProperties != null);

            // Create current properties to be used by children as inherited properties, set local properties
            IDictionary<string, string> localProperties;
            IDictionary<string, string> currentProperties = GetElementProperties(htmlTableElement, inheritedProperties, out localProperties, context);

            // TODO: process localProperties for tables to override defaults, decide cell spacing defaults

            // Check if the table contains only one cell - we want to take only its content
            XElement singleCell = GetCellFromSingleCellTable(htmlTableElement);

            if (singleCell != null)
            {
                //  Need to push skipped table elements onto sourceContext
                context.SourceContext.Add(singleCell);

                // Add the cell's content directly to parent
                foreach (var htmlChildNode in singleCell.Nodes())
                {
                    AddBlock(xamlParentElement, htmlChildNode, currentProperties, context);
                }

                CheckPop(context.SourceContext, singleCell);
            }
            else
            {
                // Create xamlTableElement
                XElement xamlTableElement = new XElement(XName.Get(Xaml_Table, XamlNamespace));
                ApplyLocalProperties(xamlTableElement, localProperties, false, context);
                PushDestination(xamlTableElement, context);

                // Analyze table structure for column widths and rowspan attributes
                var columnStarts = AnalyzeTableStructure(htmlTableElement, context);

                // Process COLGROUP & COL elements
                AddColumnInformation(htmlTableElement, xamlTableElement, columnStarts, currentProperties, context);

                // Process table body - TBODY and TR elements
                var htmlChildNode = htmlTableElement.Elements().FirstOrDefault();

                while (htmlChildNode != null)
                {
                    // Process the element
                    if (htmlChildNode.Name.TagEquals("tbody", "thead", "tfoot"))
                    {
                        //  Add more special processing for TableHeader and TableFooter
                        XElement xamlTableBodyElement = new XElement(XName.Get(Xaml_TableRowGroup, XamlNamespace));
                        context.SourceContext.Add(htmlChildNode);
                        context.DestinationContext.Add(xamlTableBodyElement);

                        // Get properties of Html tbody element
                        IDictionary<string, string> tbodyElementLocalProperties;
                        IDictionary<string, string> tbodyElementCurrentProperties = GetElementProperties(htmlChildNode, currentProperties, out tbodyElementLocalProperties, context);
                        // TODO: apply local properties for tbody

                        // Process children of htmlChildNode, which is tbody, for tr elements
                        AddTableRowsToTableBody(xamlTableBodyElement, htmlChildNode.Elements().FirstOrDefault(), tbodyElementCurrentProperties, columnStarts, context);
                        if (xamlTableBodyElement.HasElements)
                        {
                            xamlTableElement.Add(xamlTableBodyElement);
                            // else: if there is no TRs in this TBody, we simply ignore it
                        }

                        CheckPop(context.DestinationContext, xamlTableBodyElement);
                        CheckPop(context.SourceContext, htmlChildNode);

                        htmlChildNode = htmlChildNode.ElementsAfterSelf().FirstOrDefault();
                    }
                    else if (htmlChildNode.Name.TagEquals("tr"))
                    {
                        // Tbody is not present, but tr element is present. Tr is wrapped in tbody
                        XElement xamlTableBodyElement = new XElement(XName.Get(Xaml_TableRowGroup, XamlNamespace));

                        // We use currentProperties of xamlTableElement when adding rows since the tbody element is artificially created and has 
                        // no properties of its own

                        htmlChildNode = AddTableRowsToTableBody(xamlTableBodyElement, htmlChildNode, currentProperties, columnStarts, context);
                        if (xamlTableBodyElement.HasElements)
                        {
                            xamlTableElement.Add(xamlTableBodyElement);
                        }
                    }
                    else
                    {
                        // Element is not tbody or tr. Ignore it.
                        // TODO: add processing for thead, tfoot elements and recovery for td elements
                        htmlChildNode = htmlChildNode.ElementsAfterSelf().FirstOrDefault();
                    }
                }

                if (xamlTableElement.HasElements)
                {
                    xamlParentElement.Add(xamlTableElement);
                }
                CheckPop(context.DestinationContext, xamlTableElement);
            }
        }

        private static XElement GetCellFromSingleCellTable(XElement htmlTableElement)
        {
            XElement singleCell = null;

            foreach (var tableChild in htmlTableElement.Elements())
            {
                if (tableChild.Name.TagEquals("tbody", "thead", "tfoot"))
                {
                    if (singleCell != null)
                    {
                        return null;
                    }
                    foreach (var tbodyChild in tableChild.Elements())
                    {
                        if (tbodyChild.Name.TagEquals("tr"))
                        {
                            if (singleCell != null)
                            {
                                return null;
                            }
                            foreach (var trChild in tbodyChild.Elements())
                            {
                                if (trChild.Name.TagEquals("td", "th"))
                                {
                                    if (singleCell != null)
                                    {
                                        return null;
                                    }
                                    singleCell = trChild;
                                }
                            }
                        }
                    }
                }
                else if (tableChild.Name.TagEquals("tr"))
                {
                    if (singleCell != null)
                    {
                        return null;
                    }
                    foreach (var trChild in tableChild.Elements())
                    {
                        if (trChild.Name.TagEquals("td", "th"))
                        {
                            if (singleCell != null)
                            {
                                return null;
                            }
                            singleCell = trChild;
                        }
                    }
                }
            }

            return singleCell;
        }

        /// <summary>
        /// Processes the information about table columns - COLGROUP and COL html elements.
        /// </summary>
        /// <param name="htmlTableElement">
        /// XElement representing a source html table.
        /// </param>
        /// <param name="xamlTableElement">
        /// XElement repesenting a resulting xaml table.
        /// </param>
        /// <param name="columnStartsAllRows">
        /// Array of doubles - column start coordinates.
        /// Can be null, which means that column size information is not available
        /// and we must use source colgroup/col information.
        /// In case wneh it's not null, we will ignore source colgroup/col information.
        /// </param>
        /// <param name="currentProperties"></param>
        /// <param name="context">Conversion context</param>
        private static void AddColumnInformation(XElement htmlTableElement, XElement xamlTableElement, IList<double> columnStartsAllRows, IDictionary<string, string> currentProperties, HtmlToXamlContext context)
        {
            ICollection<XElement> columns = new List<XElement>();

            // Add column information
            if (columnStartsAllRows != null)
            {
                // We have consistent information derived from table cells; use it
                // The last element in columnStarts represents the end of the table
                for (int columnIndex = 0; columnIndex < columnStartsAllRows.Count - 1; columnIndex++)
                {
                    var xamlColumnElement = new XElement(XName.Get(Xaml_TableColumn, XamlNamespace));
                    xamlColumnElement.SetAttributeValue(Xaml_Width, (columnStartsAllRows[columnIndex + 1] - columnStartsAllRows[columnIndex]).ToString(CultureInfo.InvariantCulture));
                    columns.Add(xamlColumnElement);
                }
            }
            else
            {
                // We do not have consistent information from table cells;
                // Translate blindly colgroups from html.                
                foreach (var htmlChildNode in htmlTableElement.Elements())
                {
                    if (htmlChildNode.Name.TagEquals("colgroup"))
                    {
                        context.SourceContext.Add(htmlChildNode);
                        foreach(var column in GetTableColumnGroupColumns(htmlChildNode, currentProperties, context))
                        {
                            columns.Add(column);
                        }
                        CheckPop(context.SourceContext, htmlChildNode);
                    }
                    else if (htmlChildNode.Name.TagEquals("col"))
                    {
                        columns.Add(CreateTableColumn(htmlChildNode, currentProperties, context));
                    }
                    else
                    {
                        // Some element which belongs to table body. Stop column loop.
                        break;
                    }
                }
            }

            if(columns.Any())
            {
                var xamlColumnsElement = new XElement(XName.Get(Xaml_TableColumns, XamlNamespace));
                xamlTableElement.Add(xamlColumnsElement);

                CheckCorrectColumnWidths(htmlTableElement, columns);

                foreach (var column in columns)
                {
                    xamlColumnsElement.Add(column);
                }
            }
        }

        private static void CheckCorrectColumnWidths(XElement htmlTableElement, ICollection<XElement> columns)
        {
            var tableWidth = GetColumnWidth(htmlTableElement);
            if (tableWidth > 0.0)
            {
                var columnsWidth = 0.0;
                foreach (var column in columns)
                {
                    var widthAttribute = column.Attribute(Xaml_Width);
                    if (widthAttribute == null)
                    {
                        // not all columns have width set
                        columnsWidth = 0.0;
                        break;
                    }
                    columnsWidth += double.Parse(widthAttribute.Value, CultureInfo.InvariantCulture);
                }
                if (columnsWidth > 0.0)
                {
                    var widthRatio = tableWidth / columnsWidth;
                    foreach (var column in columns)
                    {
                        var widthAttribute = column.Attribute(Xaml_Width);
                        var columnWidth = double.Parse(widthAttribute.Value) * widthRatio;
                        column.SetAttributeValue(Xaml_Width, columnWidth.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
        }

        /// <summary>
        /// Converts htmlColgroupElement into Xaml TableColumnGroup element, and appends it to the parent
        /// xamlTableElement
        /// </summary>
        /// <param name="htmlColgroupElement">
        /// XElement representing Html colgroup element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// Properties inherited from parent context
        /// </param>
        /// <param name="context">Conversion context</param>
        private static IEnumerable<XElement> GetTableColumnGroupColumns(XElement htmlColgroupElement, IDictionary<string, string> inheritedProperties, HtmlToXamlContext context)
        {
            IDictionary<string, string> localProperties;
            IDictionary<string, string> currentProperties = GetElementProperties(htmlColgroupElement, inheritedProperties, out localProperties, context);

            // Process children of colgroup. Colgroup may contain only col elements.
            foreach (var htmlNode in htmlColgroupElement.Elements())
            {
                if (htmlNode.Name.TagEquals("col"))
                {
                    yield return CreateTableColumn(htmlNode, currentProperties, context);
                }
            }
        }

        /// <summary>
        /// Converts htmlColElement into Xaml TableColumn element, and appends it to the parent
        /// xamlTableColumnGroupElement
        /// </summary>
        /// <param name="xamlColumnsElement"></param>
        /// <param name="htmlColElement">
        /// XElement representing Html col element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// properties inherited from parent context
        /// </param>
        /// <param name="context">Conversion context</param>
        private static XElement CreateTableColumn(XElement htmlColElement, IDictionary<string, string> inheritedProperties, HtmlToXamlContext context)
        {
            context.SourceContext.Add(htmlColElement);

            XElement xamlTableColumnElement = new XElement(XName.Get(Xaml_TableColumn, XamlNamespace));
            context.DestinationContext.Add(xamlTableColumnElement);

            var width = GetColumnWidth(htmlColElement);
            if (width >= 0.0)
            {
                xamlTableColumnElement.SetAttributeValue(Xaml_Width, width.ToString(CultureInfo.InvariantCulture));
            }

            CheckPop(context.DestinationContext, xamlTableColumnElement);
            CheckPop(context.SourceContext, htmlColElement);

            // Col is an empty element, with no subtree 
            return xamlTableColumnElement;
        }

        /// <summary>
        /// Adds TableRow elements to xamlTableBodyElement. The rows are converted from Html tr elements that
        /// may be the children of an Html tbody element or an Html table element with tbody missing
        /// </summary>
        /// <param name="xamlTableBodyElement">
        /// XElement representing Xaml TableRowGroup element to which the converted rows should be added
        /// </param>
        /// <param name="htmlTRStartNode">
        /// XElement representing the first tr child of the tbody element to be read
        /// </param>
        /// <param name="currentProperties">
        /// Dictionary representing current properties of the tbody element that are generated and applied in the
        /// AddTable function; to be used as inheritedProperties when adding tr elements
        /// </param>
        /// <param name="columnStarts"></param>
        /// <param name="context">Conversion context</param>
        /// <returns>
        /// XNode representing the current position of the iterator among tr elements
        /// </returns>
        private static XElement AddTableRowsToTableBody(XElement xamlTableBodyElement, XElement htmlTRStartNode, IDictionary<string, string> currentProperties, IList<double> columnStarts, HtmlToXamlContext context)
        {
            // Parameter validation
            Debug.Assert(xamlTableBodyElement.Name.LocalName == Xaml_TableRowGroup);
            Debug.Assert(currentProperties != null);

            // Initialize child node for iteratimg through children to the first tr element
            var htmlChildNode = htmlTRStartNode;
            IList<double> activeRowSpans = null;
            if (columnStarts != null)
            {
                activeRowSpans = new List<double>();
                InitializeActiveRowSpans(activeRowSpans, columnStarts.Count);
            }

            while (htmlChildNode != null && !htmlChildNode.Name.TagEquals("tbody"))
            {
                if (htmlChildNode.Name.TagEquals("tr"))
                {
                    XElement xamlTableRowElement = new XElement(XName.Get(Xaml_TableRow, XamlNamespace));
                    PushDestination(xamlTableRowElement, context);
                    context.SourceContext.Add(htmlChildNode);

                    // Get tr element properties
                    IDictionary<string, string> trElementLocalProperties;
                    IDictionary<string, string> trElementCurrentProperties = GetElementProperties(htmlChildNode, currentProperties, out trElementLocalProperties, context);
                    // TODO: apply local properties to tr element

                    AddTableCellsToTableRow(xamlTableRowElement, htmlChildNode.Elements().FirstOrDefault(), trElementCurrentProperties, columnStarts, activeRowSpans, context);
                    if (xamlTableRowElement.HasElements)
                    {
                        xamlTableBodyElement.Add(xamlTableRowElement);
                    }

                    CheckPop(context.SourceContext, htmlChildNode);
                    CheckPop(context.DestinationContext, xamlTableRowElement);

                    // Advance
                    htmlChildNode = htmlChildNode.ElementsAfterSelf().FirstOrDefault();

                }
                else if (htmlChildNode.Name.TagEquals("td"))
                {
                    // Tr element is not present. We create one and add td elements to it
                    XElement xamlTableRowElement = new XElement(XName.Get(Xaml_TableRow, XamlNamespace));
                    PushDestination(xamlTableRowElement, context);

                    // This is incorrect formatting and the column starts should not be set in this case
                    Debug.Assert(columnStarts == null);

                    htmlChildNode = AddTableCellsToTableRow(xamlTableRowElement, htmlChildNode, currentProperties, columnStarts, activeRowSpans, context);
                    if (xamlTableRowElement.HasElements)
                    {
                        xamlTableBodyElement.Add(xamlTableRowElement);
                    }
                    CheckPop(context.DestinationContext, xamlTableRowElement);
                }
                else
                {
                    // Not a tr or td  element. Ignore it.
                    // TODO: consider better recovery here
                    htmlChildNode = htmlChildNode.ElementsAfterSelf().FirstOrDefault();
                }
            }
            return htmlChildNode;
        }

        /// <summary>
        /// Adds TableCell elements to xamlTableRowElement.
        /// </summary>
        /// <param name="xamlTableRowElement">
        /// XElement representing Xaml TableRow element to which the converted cells should be added
        /// </param>
        /// <param name="htmlTDStartNode">
        /// XElement representing the child of tr or tbody element from which we should start adding td elements
        /// </param>
        /// <param name="currentProperties">
        /// properties of the current html tr element to which cells are to be added
        /// </param>
        /// <param name="activeRowSpans"></param>
        /// <param name="context">Conversion context</param>
        /// <param name="columnStarts"></param>
        /// <returns>
        /// XElement representing the current position of the iterator among the children of the parent Html tbody/tr element
        /// </returns>
        private static XElement AddTableCellsToTableRow(XElement xamlTableRowElement, XElement htmlTDStartNode, IDictionary<string, string> currentProperties, IList<double> columnStarts, IList<double> activeRowSpans, HtmlToXamlContext context)
        {
            // parameter validation
            Debug.Assert(xamlTableRowElement.Name.LocalName == Xaml_TableRow);
            Debug.Assert(currentProperties != null);
            if (columnStarts != null)
            {
                Debug.Assert(activeRowSpans.Count == columnStarts.Count);
            }

            var htmlChildNode = htmlTDStartNode;
            double columnStart = 0;
            double columnWidth = 0;
            int columnIndex = 0;
            int columnSpan = 0;

            while (htmlChildNode != null && !htmlChildNode.Name.TagEquals("tr", "tbody", "thead", "tfoot"))
            {
                if (htmlChildNode.Name.TagEquals("td", "th"))
                {
                    XElement xamlTableCellElement = new XElement(XName.Get(Xaml_TableCell, XamlNamespace));
                    PushDestination(xamlTableCellElement, context);
                    context.SourceContext.Add(htmlChildNode);

                    IDictionary<string, string> tdElementLocalProperties;
                    IDictionary<string, string> tdElementCurrentProperties = GetElementProperties(htmlChildNode, currentProperties, out tdElementLocalProperties, context);

                    // TODO: determine if localProperties can be used instead of htmlChildNode in this call, and if they can,
                    // make necessary changes and use them instead.
                    ApplyPropertiesToTableCellElement(htmlChildNode, xamlTableCellElement);

                    if (columnStarts != null)
                    {
                        Debug.Assert(columnIndex < columnStarts.Count - 1);
                        while (columnIndex < activeRowSpans.Count && (int)activeRowSpans[columnIndex] > 0)
                        {
                            activeRowSpans[columnIndex] = (int)activeRowSpans[columnIndex] - 1;
                            Debug.Assert((int)activeRowSpans[columnIndex] >= 0);
                            columnIndex++;
                        }
                        Debug.Assert(columnIndex < columnStarts.Count - 1);
                        columnStart = (double)columnStarts[columnIndex];
                        columnWidth = GetColumnWidth(htmlChildNode);
                        columnSpan = CalculateColumnSpan(columnIndex, columnWidth, columnStarts);
                        int rowSpan = GetRowSpan(htmlChildNode);

                        // Column cannot have no span
                        Debug.Assert(columnSpan > 0);
                        Debug.Assert(columnIndex + columnSpan < columnStarts.Count);

                        xamlTableCellElement.SetAttributeValue(Xaml_TableCell_ColumnSpan, columnSpan.ToString());

                        // Apply row span
                        for (int spannedColumnIndex = columnIndex; spannedColumnIndex < columnIndex + columnSpan; spannedColumnIndex++)
                        {
                            Debug.Assert(spannedColumnIndex < activeRowSpans.Count);
                            activeRowSpans[spannedColumnIndex] = (rowSpan - 1);
                            Debug.Assert((int)activeRowSpans[spannedColumnIndex] >= 0);
                        }

                        columnIndex = columnIndex + columnSpan;
                    }

                    if(!htmlChildNode.HasElements && !htmlChildNode.IsEmpty)
                    {
                        AddParagraph(xamlTableCellElement, htmlChildNode, null, context);
                    }
                    else
                    {
                        AddDataToTableCell(xamlTableCellElement, htmlChildNode.Elements().FirstOrDefault(), tdElementCurrentProperties, context);
                    }
                    if (xamlTableCellElement.HasElements)
                    {
                        xamlTableRowElement.Add(xamlTableCellElement);
                    }

                    CheckPop(context.SourceContext, htmlChildNode);
                    CheckPop(context.DestinationContext, xamlTableCellElement);

                    htmlChildNode = htmlChildNode.ElementsAfterSelf().FirstOrDefault();
                }
                else
                {
                    // Not td element. Ignore it.
                    // TODO: Consider better recovery
                    htmlChildNode = htmlChildNode.ElementsAfterSelf().FirstOrDefault();
                }
            }
            return htmlChildNode;
        }

        /// <summary>
        /// adds table cell data to xamlTableCellElement
        /// </summary>
        /// <param name="xamlTableCellElement">
        /// XElement representing Xaml TableCell element to which the converted data should be added
        /// </param>
        /// <param name="htmlDataStartNode">
        /// XElement representing the start element of data to be added to xamlTableCellElement
        /// </param>
        /// <param name="currentProperties">
        /// Current properties for the html td/th element corresponding to xamlTableCellElement
        /// </param>
        /// <param name="context">Conversion context</param>
        private static void AddDataToTableCell(XElement xamlTableCellElement, XNode htmlDataStartNode, IDictionary<string, string> currentProperties, HtmlToXamlContext context)
        {
            // Parameter validation
            Debug.Assert(xamlTableCellElement.Name.LocalName == Xaml_TableCell);
            Debug.Assert(currentProperties != null);

            for (XNode htmlChildNode = htmlDataStartNode; htmlChildNode != null; htmlChildNode = htmlChildNode != null ? htmlChildNode.NextNode : null)
            {
                // Process a new html element and add it to the td element
                htmlChildNode = AddBlock(xamlTableCellElement, htmlChildNode, currentProperties, context);
            }
        }

        /// <summary>
        /// Performs a parsing pass over a table to read information about column width and rowspan attributes. This information
        /// is used to determine the starting point of each column. 
        /// </summary>
        /// <param name="htmlTableElement">
        /// XElement representing Html table whose structure is to be analyzed
        /// </param>
        /// <returns>
        /// ArrayList of type double which contains the function output. If analysis is successful, this ArrayList contains
        /// all the points which are the starting position of any column in the table, ordered from left to right.
        /// In case if analisys was impossible we return null.
        /// </returns>
        /// <param name="context">Conversion context</param>
        private static IList<double> AnalyzeTableStructure(XElement htmlTableElement, HtmlToXamlContext context)
        {
            // Parameter validation
            Debug.Assert(htmlTableElement.Name.TagEquals("table"));
            if (!htmlTableElement.HasElements)
            {
                return null;
            }

            bool columnWidthsAvailable = true;

            var columnStarts = new List<double>();
            var activeRowSpans = new List<double>();
            Debug.Assert(columnStarts.Count == activeRowSpans.Count);

            var htmlChildNode = htmlTableElement.Elements().FirstOrDefault();
            double tableWidth = 0;  // Keep track of table width which is the width of its widest row

            // Analyze tbody and tr elements
            while (htmlChildNode != null && columnWidthsAvailable)
            {
                Debug.Assert(columnStarts.Count == activeRowSpans.Count);

                switch (htmlChildNode.Name.LocalName.ToLower())
                {
                    case "tbody":
                        // Tbody element, we should analyze its children for trows
                        double tbodyWidth = AnalyzeTbodyStructure(htmlChildNode, columnStarts, activeRowSpans, tableWidth, context);
                        if (tbodyWidth > tableWidth)
                        {
                            // Table width must be increased to supported newly added wide row
                            tableWidth = tbodyWidth;
                        }
                        else if (tbodyWidth == 0)
                        {
                            // Tbody analysis may return 0, probably due to unprocessable format. 
                            // We should also fail.
                            columnWidthsAvailable = false; // interrupt the analisys
                        }
                        break;
                    case "tr":
                        // Table row. Analyze column structure within row directly
                        double trWidth = AnalyzeTRStructure(htmlChildNode, columnStarts, activeRowSpans, tableWidth, context);
                        if (trWidth > tableWidth)
                        {
                            tableWidth = trWidth;
                        }
                        else if (trWidth == 0)
                        {
                            columnWidthsAvailable = false; // interrupt the analisys
                        }
                        break;
                    case "td":
                        // Incorrect formatting, too deep to analyze at this level. Return null.
                        // TODO: implement analysis at this level, possibly by creating a new tr
                        columnWidthsAvailable = false; // interrupt the analisys
                        break;
                    default:
                        // Element should not occur directly in table. Ignore it.
                        break;
                }

                htmlChildNode = htmlChildNode.ElementsAfterSelf().FirstOrDefault();
            }

            if (columnWidthsAvailable)
            {
                // Add an item for whole table width
                columnStarts.Add(tableWidth);
                VerifyColumnStartsAscendingOrder(columnStarts);
            }
            else
            {
                columnStarts = null;
            }

            return columnStarts;
        }

        /// <summary>
        /// Performs a parsing pass over a tbody to read information about column width and rowspan attributes. Information read about width
        /// attributes is stored in the reference ArrayList parameter columnStarts, which contains a list of all starting
        /// positions of all columns in the table, ordered from left to right. Row spans are taken into consideration when 
        /// computing column starts
        /// </summary>
        /// <param name="htmlTbodyElement">
        /// XElement representing Html tbody whose structure is to be analyzed
        /// </param>
        /// <param name="columnStarts">
        /// ArrayList of type double which contains the function output. If analysis fails, this parameter is set to null
        /// </param>
        /// <param name="activeRowSpans"></param>
        /// <param name="tableWidth">
        /// Current width of the table. This is used to determine if a new column when added to the end of table should
        /// come after the last column in the table or is actually splitting the last column in two. If it is only splitting
        /// the last column it should inherit row span for that column
        /// </param>
        /// <param name="context">Conversion context</param>
        /// <returns>
        /// Calculated width of a tbody.
        /// In case of non-analizable column width structure return 0;
        /// </returns>
        private static double AnalyzeTbodyStructure(XElement htmlTbodyElement, IList<double> columnStarts, IList<double> activeRowSpans, double tableWidth, HtmlToXamlContext context)
        {
            // Parameter validation
            Debug.Assert(htmlTbodyElement.Name.TagEquals("tbody"));
            Debug.Assert(columnStarts != null);

            double tbodyWidth = 0;
            bool columnWidthsAvailable = true;

            if (!htmlTbodyElement.HasElements)
            {
                return tbodyWidth;
            }

            // Set active row spans to 0 - thus ignoring row spans crossing tbody boundaries
            ClearActiveRowSpans(activeRowSpans);

            var htmlChildNode = htmlTbodyElement.Elements().FirstOrDefault();

            // Analyze tr elements
            while (htmlChildNode != null && columnWidthsAvailable)
            {
                switch (htmlChildNode.Name.LocalName.ToLower())
                {
                    case "tr":
                        double trWidth = AnalyzeTRStructure(htmlChildNode, columnStarts, activeRowSpans, tbodyWidth, context);
                        if (trWidth > tbodyWidth)
                        {
                            tbodyWidth = trWidth;
                        }
                        break;
                    case "td":
                        columnWidthsAvailable = false; // interrupt the analisys
                        break;
                    default:
                        break;
                }
                htmlChildNode = htmlChildNode.ElementsAfterSelf().FirstOrDefault();
            }

            // Set active row spans to 0 - thus ignoring row spans crossing tbody boundaries
            ClearActiveRowSpans(activeRowSpans);

            return columnWidthsAvailable ? tbodyWidth : 0;
        }

        /// <summary>
        /// Performs a parsing pass over a tr element to read information about column width and rowspan attributes.  
        /// </summary>
        /// <param name="htmlTRElement">
        /// XElement representing Html tr element whose structure is to be analyzed
        /// </param>
        /// <param name="columnStarts">
        /// ArrayList of type double which contains the function output. If analysis is successful, this ArrayList contains
        /// all the points which are the starting position of any column in the tr, ordered from left to right. If analysis fails,
        /// the ArrayList is set to null
        /// </param>
        /// <param name="activeRowSpans">
        /// ArrayList representing all columns currently spanned by an earlier row span attribute. These columns should
        /// not be used for data in this row. The ArrayList actually contains notation for all columns in the table, if the
        /// active row span is set to 0 that column is not presently spanned but if it is > 0 the column is presently spanned
        /// </param>
        /// <param name="tableWidth">
        /// Double value representing the current width of the table.
        /// Return 0 if analisys was insuccessful.
        /// </param>
        /// <param name="context">Conversion context</param>
        private static double AnalyzeTRStructure(XElement htmlTRElement, IList<double> columnStarts, IList<double> activeRowSpans, double tableWidth, HtmlToXamlContext context)
        {
            double columnWidth;

            // Parameter validation
            Debug.Assert(htmlTRElement.Name.TagEquals("tr"));
            Debug.Assert(columnStarts != null);
            Debug.Assert(activeRowSpans != null);
            Debug.Assert(columnStarts.Count == activeRowSpans.Count);

            if (!htmlTRElement.HasElements)
            {
                return 0;
            }

            bool columnWidthsAvailable = true;

            double columnStart = 0; // starting position of current column
            var htmlChildNode = htmlTRElement.Elements().First();
            int columnIndex = 0;
            double trWidth = 0;

            // Skip spanned columns to get to real column start
            if (columnIndex < activeRowSpans.Count)
            {
                Debug.Assert(columnStarts[columnIndex] >= columnStart);
                if (columnStarts[columnIndex] == columnStart)
                {
                    // The new column may be in a spanned area
                    while (columnIndex < activeRowSpans.Count && (int)activeRowSpans[columnIndex] > 0)
                    {
                        activeRowSpans[columnIndex] = (int)activeRowSpans[columnIndex] - 1;
                        Debug.Assert((int)activeRowSpans[columnIndex] >= 0);
                        columnIndex++;
                        columnStart = columnStarts[columnIndex];
                    }
                }
            }

            while (htmlChildNode != null && columnWidthsAvailable)
            {
                Debug.Assert(columnStarts.Count == activeRowSpans.Count);

                VerifyColumnStartsAscendingOrder(columnStarts);

                switch (htmlChildNode.Name.LocalName.ToLower())
                {
                    case "td":
                        Debug.Assert(columnIndex <= columnStarts.Count);
                        if (columnIndex < columnStarts.Count)
                        {
                            Debug.Assert(columnStart <= (double)columnStarts[columnIndex]);
                            if (columnStart < (double)columnStarts[columnIndex])
                            {
                                columnStarts.Insert(columnIndex, columnStart);
                                // There can be no row spans now - the column data will appear here
                                // Row spans may appear only during the column analysis
                                activeRowSpans.Insert(columnIndex, 0);
                            }
                        }
                        else
                        {
                            // Column start is greater than all previous starts. Row span must still be 0 because
                            // we are either adding after another column of the same row, in which case it should not inherit
                            // the previous column's span. Otherwise we are adding after the last column of some previous
                            // row, and assuming the table widths line up, we should not be spanned by it. If there is
                            // an incorrect tbale structure where a columns starts in the middle of a row span, we do not
                            // guarantee correct output
                            columnStarts.Add(columnStart);
                            activeRowSpans.Add(0);
                        }
                        columnWidth = GetColumnWidth(htmlChildNode);
                        if (columnWidth != -1)
                        {
                            int nextColumnIndex;
                            int rowSpan = GetRowSpan(htmlChildNode);

                            nextColumnIndex = GetNextColumnIndex(columnIndex, columnWidth, columnStarts, activeRowSpans);
                            if (nextColumnIndex != -1)
                            {
                                // Entire column width can be processed without hitting conflicting row span. This means that
                                // column widths line up and we can process them
                                Debug.Assert(nextColumnIndex <= columnStarts.Count);

                                // Apply row span to affected columns
                                for (int spannedColumnIndex = columnIndex; spannedColumnIndex < nextColumnIndex; spannedColumnIndex++)
                                {
                                    activeRowSpans[spannedColumnIndex] = rowSpan - 1;
                                    Debug.Assert((int)activeRowSpans[spannedColumnIndex] >= 0);
                                }

                                columnIndex = nextColumnIndex;

                                // Calculate columnsStart for the next cell
                                columnStart = columnStart + columnWidth;

                                if (columnIndex < activeRowSpans.Count)
                                {
                                    Debug.Assert((double)columnStarts[columnIndex] >= columnStart);
                                    if ((double)columnStarts[columnIndex] == columnStart)
                                    {
                                        // The new column may be in a spanned area
                                        while (columnIndex < activeRowSpans.Count && (int)activeRowSpans[columnIndex] > 0)
                                        {
                                            activeRowSpans[columnIndex] = (int)activeRowSpans[columnIndex] - 1;
                                            Debug.Assert((int)activeRowSpans[columnIndex] >= 0);
                                            columnIndex++;
                                            columnStart = (double)columnStarts[columnIndex];
                                        }
                                    }
                                    // else: the new column does not start at the same time as a pre existing column
                                    // so we don't have to check it for active row spans, it starts in the middle
                                    // of another column which has been checked already by the GetNextColumnIndex function
                                }
                            }
                            else
                            {
                                // Full column width cannot be processed without a pre existing row span.
                                // We cannot analyze widths
                                columnWidthsAvailable = false;
                            }
                        }
                        else
                        {
                            // Incorrect column width, stop processing
                            columnWidthsAvailable = false;
                        }
                        break;
                    default:
                        break;
                }

                htmlChildNode = htmlChildNode.ElementsAfterSelf().FirstOrDefault();
            }

            // The width of the tr element is the position at which it's last td element ends, which is calculated in
            // the columnStart value after each td element is processed
            if (columnWidthsAvailable)
            {
                trWidth = columnStart;
            }
            else
            {
                trWidth = 0;
            }

            return trWidth;
        }

        /// <summary>
        /// Gets row span attribute from htmlTDElement. Returns an integer representing the value of the rowspan attribute.
        /// Default value if attribute is not specified or if it is invalid is 1
        /// </summary>
        /// <param name="htmlTDElement">
        /// Html td element to be searched for rowspan attribute
        /// </param>
        private static int GetRowSpan(XElement htmlTDElement)
        {
            string rowSpanAsString;
            int rowSpan;

            rowSpanAsString = GetAttribute(htmlTDElement, "rowspan");
            if (rowSpanAsString != null)
            {
                if (!Int32.TryParse(rowSpanAsString, out rowSpan))
                {
                    // Ignore invalid value of rowspan; treat it as 1
                    rowSpan = 1;
                }
            }
            else
            {
                // No row span, default is 1
                rowSpan = 1;
            }
            return rowSpan;
        }

        /// <summary>
        /// Gets index at which a column should be inseerted into the columnStarts ArrayList. This is
        /// decided by the value columnStart. The columnStarts ArrayList is ordered in ascending order.
        /// Returns an integer representing the index at which the column should be inserted
        /// </summary>
        /// <param name="columnWidth"></param>
        /// <param name="columnStarts">
        /// Array list representing starting coordinates of all columns in the table
        /// </param>
        /// <param name="columnIndex">
        /// Int representing the current column index. This acts as a clue while finding the insertion index.
        /// If the value of columnStarts at columnIndex is the same as columnStart, then this position alrady exists
        /// in the array and we can jsut return columnIndex.
        /// </param>
        /// <param name="activeRowSpans"></param>
        /// <returns></returns>
        private static int GetNextColumnIndex(int columnIndex, double columnWidth, IList<double> columnStarts, IList<double> activeRowSpans)
        {
            double columnStart;
            int spannedColumnIndex;

            // Parameter validation
            Debug.Assert(columnStarts != null);
            Debug.Assert(0 <= columnIndex && columnIndex <= columnStarts.Count);
            Debug.Assert(columnWidth > 0);

            columnStart = columnStarts[columnIndex];
            spannedColumnIndex = columnIndex + 1;

            while (spannedColumnIndex < columnStarts.Count && columnStarts[spannedColumnIndex] < columnStart + columnWidth && spannedColumnIndex != -1)
            {
                if ((int)activeRowSpans[spannedColumnIndex] > 0)
                {
                    // The current column should span this area, but something else is already spanning it
                    // Not analyzable
                    spannedColumnIndex = -1;
                }
                else
                {
                    spannedColumnIndex++;
                }
            }

            return spannedColumnIndex;
        }


        /// <summary>
        /// Used for clearing activeRowSpans array in the beginning/end of each tbody
        /// </summary>
        /// <param name="activeRowSpans">
        /// ArrayList representing currently active row spans
        /// </param>
        private static void ClearActiveRowSpans(IList<double> activeRowSpans)
        {
            for (int columnIndex = 0; columnIndex < activeRowSpans.Count; columnIndex++)
            {
                activeRowSpans[columnIndex] = 0;
            }
        }

        /// <summary>
        /// Used for initializing activeRowSpans array in the before adding rows to tbody element
        /// </summary>
        /// <param name="activeRowSpans">
        /// ArrayList representing currently active row spans
        /// </param>
        /// <param name="count">
        /// Size to be give to array list
        /// </param>
        private static void InitializeActiveRowSpans(IList<double> activeRowSpans, int count)
        {
            for (int columnIndex = 0; columnIndex < count; columnIndex++)
            {
                activeRowSpans.Add(0);
            }
        }


        /// <summary>
        /// Calculates width of next TD element based on starting position of current element and it's width, which
        /// is calculated byt he function
        /// </summary>
        /// <param name="htmlTDElement">
        /// XElement representing Html td element whose width is to be read
        /// </param>
        /// <param name="columnStart">
        /// Starting position of current column
        /// </param>
        private static double GetNextColumnStart(XElement htmlTDElement, double columnStart)
        {
            double columnWidth;
            double nextColumnStart;

            // Parameter validation
            Debug.Assert(htmlTDElement.Name.TagEquals("td", "th"));
            Debug.Assert(columnStart >= 0);

            nextColumnStart = -1;  // -1 indicates inability to calculate columnStart width

            columnWidth = GetColumnWidth(htmlTDElement);

            if (columnWidth == -1)
            {
                nextColumnStart = -1;
            }
            else
            {
                nextColumnStart = columnStart + columnWidth;
            }

            return nextColumnStart;
        }


        private static double GetColumnWidth(XElement htmlTDElement)
        {
            string columnWidthAsString;
            double columnWidth;

            columnWidthAsString = null;
            columnWidth = -1;

            // Get string valkue for the width
            columnWidthAsString = GetAttribute(htmlTDElement, "width");
            if (columnWidthAsString == null)
            {
                columnWidthAsString = GetCssAttribute(GetAttribute(htmlTDElement, "style"), "width");
            }

            // We do not allow column width to be 0, if specified as 0 we will fail to record it
            if (!TryGetLengthValue(columnWidthAsString, out columnWidth) || columnWidth == 0)
            {
                columnWidth = -1;
            }
            return columnWidth;
        }

        /// <summary>
        /// Calculates column span based the column width and the widths of all other columns. Returns an integer representing 
        /// the column span
        /// </summary>
        /// <param name="columnIndex">
        /// Index of the current column
        /// </param>
        /// <param name="columnWidth">
        /// Width of the current column
        /// </param>
        /// <param name="columnStarts">
        /// ArrayList repsenting starting coordinates of all columns
        /// </param>
        private static int CalculateColumnSpan(int columnIndex, double columnWidth, IList<double> columnStarts)
        {
            // Current status of column width. Indicates the amount of width that has been scanned already
            double columnSpanningValue;
            int columnSpanningIndex;
            int columnSpan;
            double subColumnWidth; // Width of the smallest-grain columns in the table

            Debug.Assert(columnStarts != null);
            Debug.Assert(columnIndex < columnStarts.Count - 1);
            Debug.Assert((double)columnStarts[columnIndex] >= 0);
            Debug.Assert(columnWidth > 0);

            columnSpanningIndex = columnIndex;
            columnSpanningValue = 0;
            columnSpan = 0;
            subColumnWidth = 0;

            while (columnSpanningValue < columnWidth && columnSpanningIndex < columnStarts.Count - 1)
            {
                subColumnWidth = (double)columnStarts[columnSpanningIndex + 1] - (double)columnStarts[columnSpanningIndex];
                Debug.Assert(subColumnWidth > 0);
                columnSpanningValue += subColumnWidth;
                columnSpanningIndex++;
            }

            // Now, we have either covered the width we needed to cover or reached the end of the table, in which
            // case the column spans all the columns until the end
            columnSpan = columnSpanningIndex - columnIndex;
            Debug.Assert(columnSpan > 0);

            return columnSpan;
        }

        /// <summary>
        /// Verifies that values in columnStart, which represent starting coordinates of all columns, are arranged
        /// in ascending order
        /// </summary>
        /// <param name="columnStarts">
        /// ArrayList representing starting coordinates of all columns
        /// </param>
        private static void VerifyColumnStartsAscendingOrder(IList<double> columnStarts)
        {
            Debug.Assert(columnStarts != null);

            double columnStart;

            columnStart = -0.01;

            for (int columnIndex = 0; columnIndex < columnStarts.Count; columnIndex++)
            {
                Debug.Assert(columnStart < columnStarts[columnIndex]);
                columnStart = columnStarts[columnIndex];
            }
        }

        // .............................................................
        //
        // Attributes and Properties
        //
        // .............................................................

        /// <summary>
        /// Analyzes local properties of Html element, converts them into Xaml equivalents, and applies them to xamlElement
        /// </summary>
        /// <param name="xamlElement">
        /// XElement representing Xaml element to which properties are to be applied
        /// </param>
        /// <param name="localProperties">
        /// Dictionary representing local properties of Html element that is converted into xamlElement
        /// </param>
        /// <param name="isBlock"></param>
        /// <param name="context">Conversion context</param>
        private static void ApplyLocalProperties(XElement xamlElement, IDictionary<string, string> localProperties, bool isBlock, HtmlToXamlContext context)
        {
            bool marginSet = false;
            string marginTop = "0";
            string marginBottom = "0";
            string marginLeft = "0";
            string marginRight = "0";

            bool paddingSet = false;
            string paddingTop = "0";
            string paddingBottom = "0";
            string paddingLeft = "0";
            string paddingRight = "0";

            string borderColor = null;

            bool borderThicknessSet = false;
            string borderThicknessTop = "0";
            string borderThicknessBottom = "0";
            string borderThicknessLeft = "0";
            string borderThicknessRight = "0";

            string htmlElementName = xamlElement.Name.LocalName; // Keep the name case-sensitive to check xml names
            string htmlElementNamespace = xamlElement.Name.Namespace.NamespaceName;


            XElement textDecorationsElement = new XElement(XName.Get($"{xamlElement.Name.LocalName}.{Xaml_TextDecorations}", XamlNamespace));
            var adjustFontSize = false;
            foreach (var property in localProperties)
            {
                // skip inherited properties
                if ("inherit".Equals(property.Value))
                {
                    continue;
                }
                switch (property.Key)
                {
                    case "font-family":
                        //  Convert from font-family value list into xaml FontFamily value
                        xamlElement.SetAttributeValue(Xaml_FontFamily, property.Value);
                        break;
                    case "font-style":
                        xamlElement.SetAttributeValue(Xaml_FontStyle, property.Value);
                        break;
                    case "font-variant":
                        //  Convert from font-variant into xaml property
                        break;
                    case "font-weight":
                        xamlElement.SetAttributeValue(Xaml_FontWeight, property.Value);
                        break;
                    case "font-size":
                        //  Convert from css size into FontSize
                        if (property.Value == "0")
                            xamlElement.SetAttributeValue(Xaml_FontSize, Xaml_FontSize_XLarge);
                        else
                            xamlElement.SetAttributeValue(Xaml_FontSize, property.Value);

                        int v  = 0;
                        if (int.TryParse(property.Value, out v))
                        {
                            if (v < 10)
                                xamlElement.SetAttributeValue(Xaml_FontSize, Xaml_FontSize_XLarge);
                        }

                        break;
                    case "color":
                        SetColor(xamlElement, "Foreground", property.Value);
                        break;
                    case "background-color":
                        SetColor(xamlElement, "Background", property.Value);
                        break;
                    case "text-decoration-underline":
                        if (!isBlock)
                        {
                            SetTextDecoration(textDecorationsElement, property.Value, Xaml_TextDecorations_Underline);
                        }
                        break;
                    case "text-decoration-line-through":
                        if (!isBlock)
                        {
                            SetTextDecoration(textDecorationsElement, property.Value, Xaml_TextDecorations_Strikethrough);
                        }
                        break;
                    case "text-decoration-none":
                    case "text-decoration-overline":
                    case "text-decoration-blink":
                        // not supported decorations
                        break;
                    case "baseline-alignment":
                        if (!isBlock)
                        {
                            xamlElement.SetAttributeValue(Xaml_BaselineAlignment, property.Value);
                            adjustFontSize = true;
                        }
                        break;
                    case "text-transform":
                        //  Convert from text-transform into xaml property
                        break;

                    case "text-indent":
                        // tushar: error at list
                        if ((isBlock) && (htmlElementName != Xaml_Section) && (htmlElementName != Xaml_List))
                        {
                            xamlElement.SetAttributeValue(Xaml_TextIndent, property.Value);
                        }
                        break;

                    case "text-align":
                        if (isBlock)
                        {
                            xamlElement.SetAttributeValue(Xaml_TextAlignment, property.Value);
                        }
                        break;

                    case "width":
                    case "height":
                        //  Decide what to do with width and height propeties
                        break;

                    case "margin-top":
                        marginSet = true;
                        marginTop = property.Value;
                        break;
                    case "margin-right":
                        marginSet = true;
                        marginRight = property.Value;
                        break;
                    case "margin-bottom":
                        marginSet = true;
                        marginBottom = property.Value;
                        break;
                    case "margin-left":
                        marginSet = true;
                        marginLeft = property.Value;
                        break;

                    case "padding-top":
                        paddingSet = true;
                        paddingTop = property.Value;
                        break;
                    case "padding-right":
                        paddingSet = true;
                        paddingRight = property.Value;
                        break;
                    case "padding-bottom":
                        paddingSet = true;
                        paddingBottom = property.Value;
                        break;
                    case "padding-left":
                        paddingSet = true;
                        paddingLeft = property.Value;
                        break;

                    // NOTE: css names for elementary border styles have side indications in the middle (top/bottom/left/right)
                    // In our internal notation we intentionally put them at the end - to unify processing in ParseCssRectangleProperty method
                    case "border-color-top":
                        borderColor = property.Value;
                        break;
                    case "border-color-right":
                        borderColor = property.Value;
                        break;
                    case "border-color-bottom":
                        borderColor = property.Value;
                        break;
                    case "border-color-left":
                        borderColor = property.Value;
                        break;
                    case "border-style-top":
                    case "border-style-right":
                    case "border-style-bottom":
                    case "border-style-left":
                        //  Implement conversion from border style
                        break;
                    case "border-width-top":
                        borderThicknessSet = true;
                        borderThicknessTop = property.Value;
                        break;
                    case "border-width-right":
                        borderThicknessSet = true;
                        borderThicknessRight = property.Value;
                        break;
                    case "border-width-bottom":
                        borderThicknessSet = true;
                        borderThicknessBottom = property.Value;
                        break;
                    case "border-width-left":
                        borderThicknessSet = true;
                        borderThicknessLeft = property.Value;
                        break;

                    case "list-style-type":
                        if (xamlElement.Name.LocalName == Xaml_List)
                        {
                            string markerStyle;
                            switch ((property.Value).ToLower())
                            {
                                case "disc":
                                    markerStyle = Xaml_List_MarkerStyle_Disc;
                                    break;
                                case "circle":
                                    markerStyle = Xaml_List_MarkerStyle_Circle;
                                    break;
                                case "none":
                                    markerStyle = Xaml_List_MarkerStyle_None;
                                    break;
                                case "square":
                                    markerStyle = Xaml_List_MarkerStyle_Square;
                                    break;
                                case "box":
                                    markerStyle = Xaml_List_MarkerStyle_Box;
                                    break;
                                case "lower-latin":
                                    markerStyle = Xaml_List_MarkerStyle_LowerLatin;
                                    break;
                                case "upper-latin":
                                    markerStyle = Xaml_List_MarkerStyle_UpperLatin;
                                    break;
                                case "lower-roman":
                                    markerStyle = Xaml_List_MarkerStyle_LowerRoman;
                                    break;
                                case "upper-roman":
                                    markerStyle = Xaml_List_MarkerStyle_UpperRoman;
                                    break;
                                case "decimal":
                                    markerStyle = Xaml_List_MarkerStyle_Decimal;
                                    break;
                                default:
                                    markerStyle = Xaml_List_MarkerStyle_Disc;
                                    break;
                            }
                            xamlElement.SetAttributeValue(Xaml_List_MarkerStyle, markerStyle);
                        }
                        break;

                    case "float":
                    case "clear":
                        if (isBlock)
                        {
                            //  Convert float and clear properties
                        }
                        break;

                    case "display":
                        break;

                    case "border-collapse":
                        if("collapse".Equals(property.Value.ToLower()))
                        {
                            xamlElement.SetAttributeValue(Xaml_Table_CellSpacing, "0");
                        }
                        break;
                }
            }

            if (isBlock)
            {
                if (marginSet)
                {
                    ComposeThicknessProperty(xamlElement, Xaml_Margin, marginLeft, marginRight, marginTop, marginBottom);
                }

                if (paddingSet)
                {
                    ComposeThicknessProperty(xamlElement, Xaml_Padding, paddingLeft, paddingRight, paddingTop, paddingBottom);
                }

                if (borderColor != null)
                {
                    //  We currently ignore possible difference in brush colors on different border sides. Use the last colored side mentioned
                    xamlElement.SetAttributeValue(Xaml_BorderBrush, borderColor);
                }

                if (borderThicknessSet)
                {
                    ComposeThicknessProperty(xamlElement, Xaml_BorderThickness, borderThicknessLeft, borderThicknessRight, borderThicknessTop, borderThicknessBottom);
                }
            }

            if (textDecorationsElement.HasElements)
            {
                xamlElement.Add(textDecorationsElement);
            }
            if (adjustFontSize)
            {
                foreach (var element in new[] { xamlElement }.Concat(context.DestinationContext.Reverse()))
                {
                    var fontSizeAttribute = element.Attribute(Xaml_FontSize);
                    double fontSize;
                    if (fontSizeAttribute != null && double.TryParse(fontSizeAttribute.Value, out fontSize))
                    {
                        // established subscript and superscript font size ratio
                        fontSize *= 0.65;
                        xamlElement.SetAttributeValue(Xaml_FontSize, fontSize);
                        break;
                    }
                }
            }
        }

        private static void SetTextDecoration(XElement xamlElement, object propertyValue, string textDecoration)
        {
            if ("true".Equals(propertyValue))
            {
                var decorationElement = new XElement(XName.Get(Xaml_TextDecoration, XamlNamespace));
                decorationElement.SetAttributeValue(Xaml_TextDecoration_Location, textDecoration);
                xamlElement.Add(decorationElement);
            }
        }

        // Create syntactically optimized four-value Thickness
        private static void ComposeThicknessProperty(XElement xamlElement, string propertyName, string left, string right, string top, string bottom)
        {
            // Xaml syntax:
            // We have a reasonable interpreation for one value (all four edges), two values (horizontal, vertical),
            // and four values (left, top, right, bottom).
            //  switch (i) {
            //    case 1: return new Thickness(lengths[0]);
            //    case 2: return new Thickness(lengths[0], lengths[1], lengths[0], lengths[1]);
            //    case 4: return new Thickness(lengths[0], lengths[1], lengths[2], lengths[3]);
            //  }
            string thickness;

            // We do not accept negative margins
            if (left[0] == '0' || left[0] == '-') left = "0";
            if (right[0] == '0' || right[0] == '-') right = "0";
            if (top[0] == '0' || top[0] == '-') top = "0";
            if (bottom[0] == '0' || bottom[0] == '-') bottom = "0";

            if (left == right && top == bottom)
            {
                if (left == top)
                {
                    thickness = left;
                }
                else
                {
                    thickness = left + "," + top;
                }
            }
            else
            {
                thickness = left + "," + top + "," + right + "," + bottom;
            }

            //  Need safer processing for a thickness value
            xamlElement.SetAttributeValue(propertyName, thickness);
        }

        private static void SetColor(XElement xamlElement, string propertyName, string stringValue)
        {
            if(stringValue?.StartsWith("#") == true && stringValue.Length < 9)
            {
                stringValue = stringValue.Replace("#", "#ff");
            }
            xamlElement.SetAttributeValue(propertyName, stringValue);
        }

        /// <summary>
        /// Analyzes the tag of the htmlElement and infers its associated formatted properties.
        /// After that parses style attribute and adds all inline css styles.
        /// The resulting style attributes are collected in output parameter localProperties.
        /// </summary>
        /// <param name="htmlElement">
        /// </param>
        /// <param name="inheritedProperties">
        /// set of properties inherited from ancestor elements. Currently not used in the code. Reserved for the future development.
        /// </param>
        /// <param name="localProperties">
        /// returns all formatting properties defined by this element - implied by its tag, its attributes, or its css inline style
        /// </param>
        /// <param name="context">Conversion context</param>
        /// <returns>
        /// returns a combination of previous context with local set of properties.
        /// This value is not used in the current code - inntended for the future development.
        /// </returns>
        private static IDictionary<string, string> GetElementProperties(XElement htmlElement, IDictionary<string, string> inheritedProperties, out IDictionary<string, string> localProperties, HtmlToXamlContext context)
        {
            // Start with context formatting properties
            IDictionary<string, string> currentProperties = new Dictionary<string, string>();
            if (inheritedProperties == null)
            {
                localProperties = currentProperties;
                return localProperties;
            }
            foreach(var property in inheritedProperties)
            {
                currentProperties[property.Key] = property.Value;
            }

            // Identify element name
            string elementName = htmlElement.Name.LocalName.ToLower();
            string elementNamespace = htmlElement.Name.Namespace.NamespaceName;

            // update current formatting properties depending on element tag

            localProperties = new Dictionary<string, string>();
            switch (elementName)
            {
                // Character formatting
                case "i":
                case "italic":
                case "em":
                    localProperties["font-style"] = "italic";
                    break;
                case "b":
                case "bold":
                case "strong":
                case "dfn":
                    localProperties["font-weight"] = "bold";
                    break;
                case "u":
                case "underline":
                    localProperties["text-decoration-underline"] = "true";
                    break;
                case "font":
                    SetProperty(htmlElement, "face", localProperties, "font-family");
                    SetProperty(htmlElement, "color", localProperties);
                    SetLength(htmlElement, "size", localProperties, "font-size");
                    break;
                case "samp":
                    localProperties["font-family"] = "Courier New"; // code sample
                    localProperties["font-size"] = Xaml_FontSize_XXSmall;
                    localProperties["text-align"] = "Left";
                    break;
                case "sub":
                    localProperties["baseline-alignment"] = "Subscript";
                    break;
                case "sup":
                    localProperties["baseline-alignment"] = "Superscript";
                    break;

                // Hyperlinks
                case "a": // href, hreflang, urn, methods, rel, rev, title
                    //  Set default hyperlink properties
                    break;
                case "acronym":
                    break;

                // Paragraph formatting:
                case "p":
                    //  Set default paragraph properties
                    break;
                case "div":
                    //  Set default div properties
                    break;
                case "pre":
                    localProperties["font-family"] = "Courier New"; // renders text in a fixed-width font
                    localProperties["font-size"] = Xaml_FontSize_XXSmall;
                    localProperties["text-align"] = "Left";
                    break;
                case "blockquote":
                    localProperties["margin-left"] = "16";
                    break;

                case "h1":
                    localProperties["font-size"] = Xaml_FontSize_XXLarge;
                    break;
                case "h2":
                    localProperties["font-size"] = Xaml_FontSize_XLarge;
                    break;
                case "h3":
                    localProperties["font-size"] = Xaml_FontSize_Large;
                    break;
                case "h4":
                    localProperties["font-size"] = Xaml_FontSize_Medium;
                    break;
                case "h5":
                    localProperties["font-size"] = Xaml_FontSize_Small;
                    break;
                case "h6":
                    localProperties["font-size"] = Xaml_FontSize_XSmall;
                    break;
                // List properties
                case "ul":
                    localProperties["list-style-type"] = "disc";
                    break;
                case "ol":
                    localProperties["list-style-type"] = "decimal";
                    break;
                case "table":
                case "body":
                case "html":
                    break;
                case "img":
                    SetProperty(htmlElement, "src", localProperties);
                    SetProperty(htmlElement, "alt", localProperties);
                    SetLength(htmlElement, "width", localProperties);
                    SetLength(htmlElement, "height", localProperties);
                    break;
            }

            // Override html defaults by css attributes - from stylesheets and inline settings
            HtmlCssParser.GetElementPropertiesFromCssAttributes(htmlElement, elementName, localProperties, context);

            // Combine local properties with context to create new current properties
            foreach(var property in localProperties)
            {
                currentProperties[property.Key] = property.Value;
            }

            return currentProperties;
        }

        private static void SetLength(XElement htmlElement, string attributeName, IDictionary<string, string> localProperties, string propertyName = null)
        {
            var text = GetAttribute(htmlElement, attributeName);
            if (text != null)
            {
                double value;
                if (TryGetLengthValue(text, out value))
                {
                    localProperties[propertyName ?? attributeName] = value.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        /// <summary>
        /// Extracts a value of css attribute from css style definition.
        /// </summary>
        /// <param name="cssStyle">
        /// Source csll style definition
        /// </param>
        /// <param name="attributeName">
        /// A name of css attribute to extract
        /// </param>
        /// <returns>
        /// A string rrepresentation of an attribute value if found;
        /// null if there is no such attribute in a given string.
        /// </returns>
        private static string GetCssAttribute(string cssStyle, string attributeName)
        {
            //  This is poor man's attribute parsing. Replace it by real css parsing
            if (cssStyle != null)
            {
                string[] styleValues;

                attributeName = attributeName.ToLower();

                // Check for width specification in style string
                styleValues = cssStyle.Split(';');

                for (int styleValueIndex = 0; styleValueIndex < styleValues.Length; styleValueIndex++)
                {
                    string[] styleNameValue;

                    styleNameValue = styleValues[styleValueIndex].Split(':');
                    if (styleNameValue.Length == 2)
                    {
                        if (styleNameValue[0].Trim().ToLower() == attributeName)
                        {
                            return styleNameValue[1].Trim();
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Converts a length value from string representation to a double.
        /// </summary>
        /// <param name="lengthAsString">
        /// Source string value of a length.
        /// </param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool TryGetLengthValue(string lengthAsString, out double length)
        {
            length = Double.NaN;

            if (lengthAsString != null)
            {
                lengthAsString = lengthAsString.Trim().ToLower();

                // We try to convert currentColumnWidthAsString into a double. This will eliminate widths of type "50%", etc.
                if (lengthAsString.EndsWith("pt"))
                {
                    lengthAsString = lengthAsString.Substring(0, lengthAsString.Length - 2);
                    if (Double.TryParse(lengthAsString, out length))
                    {
                        length = Math.Round(length, 1, MidpointRounding.AwayFromZero);
                        length = (length * 96.0) / 72.0; // convert from points to pixels
                    }
                    else
                    {
                        length = Double.NaN;
                    }
                }
                else if (lengthAsString.EndsWith("px"))
                {
                    lengthAsString = lengthAsString.Substring(0, lengthAsString.Length - 2);
                    if (double.TryParse(lengthAsString, out length))
                    {
                        length = RoundPixelsToPoints(length);
                    }
                    else
                    {
                        length = Double.NaN;
                    }
                }
                else
                {
                    if (double.TryParse(lengthAsString, out length)) // Assuming pixels
                    {
                        length = RoundPixelsToPoints(length);
                    }
                    else
                    {
                        length = Double.NaN;
                    }
                }
            }

            return !Double.IsNaN(length);
        }

        private static double RoundPixelsToPoints(double length)
        {
            length = (length * 72.0) / 96.0; // convert from pixels to points
            length = Math.Round(length, 1, MidpointRounding.AwayFromZero);
            length = (length * 96.0) / 72.0; // convert from points to pixels
            return length;
        }

        // .................................................................
        //
        // Pasring Color Attribute
        //
        // .................................................................

        private static string GetColorValue(string colorValue)
        {
            // TODO: Implement color conversion
            return colorValue;
        }

        /// <summary>
        /// Checks if html tag is equal to a provided text
        /// </summary>
        /// <param name="tag">html tag</param>
        /// <param name="values">values to compare (any match)</param>
        /// <returns>true if tag equals any value</returns>
        private static bool TagEquals(this XName tag, params string[] values)
        {
            foreach (var value in values)
            {
                if (value.Equals(tag.LocalName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Applies properties to xamlTableCellElement based on the html td element it is converted from.
        /// </summary>
        /// <param name="htmlChildNode">
        /// Html td/th element to be converted to xaml
        /// </param>
        /// <param name="xamlTableCellElement">
        /// XElement representing Xaml element for which properties are to be processed
        /// </param>
        /// <remarks>
        /// TODO: Use the processed properties for htmlChildNode instead of using the node itself 
        /// </remarks>
        private static void ApplyPropertiesToTableCellElement(XElement htmlChildNode, XElement xamlTableCellElement)
        {
            // Parameter validation
            Debug.Assert(htmlChildNode.Name.TagEquals("td", "th"));
            Debug.Assert(xamlTableCellElement.Name.LocalName == Xaml_TableCell);

            // set default border thickness for xamlTableCellElement to enable gridlines
            xamlTableCellElement.SetAttributeValue(Xaml_TableCell_BorderThickness, "1,1,1,1");
            xamlTableCellElement.SetAttributeValue(Xaml_TableCell_BorderBrush, Xaml_Brushes_Black);
            string rowSpanString = GetAttribute(htmlChildNode, "rowspan");
            if (rowSpanString != null)
            {
                xamlTableCellElement.SetAttributeValue(Xaml_TableCell_RowSpan, rowSpanString);
            }
        }

        #endregion Private Methods

        // ----------------------------------------------------------------
        //
        // Internal Constants
        //
        // ----------------------------------------------------------------

        // The constants reprtesent all Xaml names used in a conversion
        public const string Xaml_FlowDocument = "FlowDocument";

        public const string Xaml_Run = "Run";
        public const string Xaml_Span = "Span";
        public const string Xaml_Hyperlink = "Hyperlink";
        public const string Xaml_Hyperlink_NavigateUri = "NavigateUri";
        public const string Xaml_Hyperlink_TargetName = "TargetName";

        public const string Xaml_Section = "Section";

        public const string Xaml_List = "List";

        public const string Xaml_List_MarkerStyle = "MarkerStyle";
        public const string Xaml_List_MarkerStyle_None = "None";
        public const string Xaml_List_MarkerStyle_Decimal = "Decimal";
        public const string Xaml_List_MarkerStyle_Disc = "Disc";
        public const string Xaml_List_MarkerStyle_Circle = "Circle";
        public const string Xaml_List_MarkerStyle_Square = "Square";
        public const string Xaml_List_MarkerStyle_Box = "Box";
        public const string Xaml_List_MarkerStyle_LowerLatin = "LowerLatin";
        public const string Xaml_List_MarkerStyle_UpperLatin = "UpperLatin";
        public const string Xaml_List_MarkerStyle_LowerRoman = "LowerRoman";
        public const string Xaml_List_MarkerStyle_UpperRoman = "UpperRoman";

        public const string Xaml_ListItem = "ListItem";

        public const string Xaml_LineBreak = "LineBreak";

        public const string Xaml_Paragraph = "Paragraph";

        public const string Xaml_Margin = "Margin";
        public const string Xaml_Padding = "Padding";
        public const string Xaml_BorderBrush = "BorderBrush";
        public const string Xaml_BorderThickness = "BorderThickness";

        public const string Xaml_Table = "Table";

        public const string Xaml_Table_CellSpacing = "CellSpacing";

        public const string Xaml_TableColumns = "Table.Columns";
        public const string Xaml_TableColumn = "TableColumn";
        public const string Xaml_TableRowGroup = "TableRowGroup";
        public const string Xaml_TableRow = "TableRow";

        public const string Xaml_TableCell = "TableCell";
        public const string Xaml_TableCell_BorderThickness = "BorderThickness";
        public const string Xaml_TableCell_BorderBrush = "BorderBrush";

        public const string Xaml_TableCell_ColumnSpan = "ColumnSpan";
        public const string Xaml_TableCell_RowSpan = "RowSpan";

        public const string Xaml_Width = "Width";
        public const string Xaml_Height = "Height";
        public const string Xaml_Source = "Source";
        public const string Xaml_BlockUIContainer = "BlockUIContainer";
        public const string Xaml_InlineUIContainer = "InlineUIContainer";
        public const string Xaml_TextBlock = "TextBlock";
        public const string Xaml_Label = "Label";
        public const string Xaml_TagProperty = "Tag";
        public const string Xaml_BitmapScalingModeProperty = "RenderOptions.BitmapScalingMode";
        public const string Xaml_RenderOptionsEdgeModeProperty = "RenderOptions.EdgeMode";

        public const string Xaml_Brushes_Black = "Black";
        public const string Xaml_FontFamily = "FontFamily";

        public const string Xaml_FontSize = "FontSize";
        public const string Xaml_FontSize_XXLarge = "22pt"; // "XXLarge";
        public const string Xaml_FontSize_XLarge = "20pt"; // "XLarge";
        public const string Xaml_FontSize_Large = "18pt"; // "Large";
        public const string Xaml_FontSize_Medium = "16pt"; // "Medium";
        public const string Xaml_FontSize_Small = "12pt"; // "Small";
        public const string Xaml_FontSize_XSmall = "10pt"; // "XSmall";
        public const string Xaml_FontSize_XXSmall = "8pt"; // "XXSmall";

        public const string Xaml_FontWeight = "FontWeight";
        public const string Xaml_FontWeight_Bold = "Bold";

        public const string Xaml_FontStyle = "FontStyle";

        public const string Xaml_Foreground = "Foreground";
        public const string Xaml_Background = "Background";
        public const string Xaml_TextDecorations = "TextDecorations";
        public const string Xaml_TextDecorations_Underline = "Underline";
        public const string Xaml_TextDecorations_Strikethrough = "Strikethrough";
        public const string Xaml_TextDecoration = "TextDecoration";
        public const string Xaml_TextDecoration_Location = "Location";

        public const string Xaml_TextIndent = "TextIndent";
        public const string Xaml_TextAlignment = "TextAlignment";
        public const string Xaml_BaselineAlignment = "BaselineAlignment";

        public const string Xaml_HorizontalAlignment = "HorizontalAlignment";


        // ---------------------------------------------------------------------
        //
        // Private Fields
        //
        // ---------------------------------------------------------------------

        #region Private Fields

        public const string Xaml_Image = "Image";

        static string _base64ImageHeader = "data:";

        static string _base64ImageTag = ";base64,";

        #endregion Private Fields
    }
}
