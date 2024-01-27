using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Xml.Xsl;
using System.Xml;
using System.Windows.Markup;

namespace DiaryJournal.Net
{
    // currently i only have flowdocument to html transform style sheet.
    // so i also need html to xaml flowdocument style sheet

    public class FlowDocumentToHtmlConverter : IValueConverter
    {
        private static XslCompiledTransform ToHtmlTransform;
        private static XslCompiledTransform ToXamlTransform;

        public FlowDocumentToHtmlConverter()
        {
            if (ToHtmlTransform == null)
            {
                ToHtmlTransform = LoadTransformResource("XamlToHtmlEx.xslt");
            }
            //if (ToXamlTransform == null)
            //{
            //    ToXamlTransform = LoadTransformResource("Converters/XhtmlToFlowDocument.xslt");
           // }
        }
        private static XslCompiledTransform LoadTransformResource(string path)
        {
            Uri uri = new Uri(path, UriKind.Relative);
            FileStream stream = File.OpenRead(path);
            stream.Position = 0;
            XmlReader xr = XmlReader.Create(stream);//Application.GetResourceStream(uri).Stream);
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xr);
            return xslt;
        }

        #region IValueConverter Members

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is FlowDocument))
            {
                return null;
            }
            if (targetType == typeof(FlowDocument))
            {
                return value;
            }

            if (targetType != typeof(string))
            {
                throw new InvalidOperationException(
                    "FlowDocumentToHtmlConverter can only convert back from a FlowDocument to a string.");
            }

            FlowDocument d = (FlowDocument)value;

            using (MemoryStream ms = new MemoryStream())
            {
                // write XAML out to a MemoryStream
                TextRange tr = new TextRange(
                    d.ContentStart,
                    d.ContentEnd);
                tr.Save(ms, System.Windows.DataFormats.Xaml);
                ms.Seek(0, SeekOrigin.Begin);

                // transform the contents of the MemoryStream to HTML
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    XmlWriterSettings xws = new XmlWriterSettings();
                    xws.OmitXmlDeclaration = true;
                    XmlReader xr = XmlReader.Create(ms);
                    XmlWriter xw = XmlWriter.Create(sw, xws);
                    ToHtmlTransform.Transform(xr, xw);
                }
                return sb.ToString();
            }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return new FlowDocument();
            }
            if (value is FlowDocument)
            {
                return value;
            }
            if (targetType != typeof(FlowDocument))
            {
                throw new InvalidOperationException(
                    "FlowDocumentToHtmlConverter can only convert to a FlowDocument.");
            }
            if (!(value is string))
            {
                throw new InvalidOperationException(
                    "FlowDocumentToHtmlConverter can only convert from a string or FlowDocument.");
            }

            string s = (string)value;

            FlowDocument d;

            using (MemoryStream ms = new MemoryStream())
            using (StringReader sr = new StringReader(s))
            {
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.OmitXmlDeclaration = true;
                using (XmlReader xr = XmlReader.Create(sr))
                using (XmlWriter xw = XmlWriter.Create(ms, xws))
                {
                    ToXamlTransform.Transform(xr, xw);
                }
                ms.Seek(0, SeekOrigin.Begin);

                d = XamlReader.Load(ms) as FlowDocument;
            }
            XamlWriter.Save(d, Console.Out);
            return d;
        }

        #endregion
    }
}
