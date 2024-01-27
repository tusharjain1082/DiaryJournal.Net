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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Markup.Primitives;
using DiaryJournal.Net;

namespace DiaryJournal.Net
{
    public static class WpfRtbMethods
    {
        public static void Replace(System.Windows.Controls.RichTextBox rtBox, string strOld, string strNew)
        {
            String rtf = "";
            TextRange tr = new TextRange(rtBox.Document.ContentStart, rtBox.Document.ContentEnd);
            using (var memoryStream = new MemoryStream())
            {
                tr.Save(memoryStream, System.Windows.DataFormats.Rtf);
                rtf = ASCIIEncoding.Default.GetString(memoryStream.ToArray());
            }
            rtf = rtf.Replace(ConvertString2RTF(strOld), ConvertString2RTF(strNew));
            MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(rtf));
            rtBox.SelectAll();
            rtBox.Selection.Load(stream, System.Windows.DataFormats.Rtf);
        }

        public static String ConvertString2RTF(string input)
        {
            //first take care of special RTF chars  
            StringBuilder backslashed = new StringBuilder(input);
            backslashed.Replace(@"\", @"\\");
            backslashed.Replace(@"{", @"\{");
            backslashed.Replace(@"}", @"\}");

            //then convert the string char by char  
            StringBuilder sb = new StringBuilder();
            foreach (char character in backslashed.ToString())
            {
                if (character <= 0x7f)
                    sb.Append(character);
                else
                    sb.Append("\\u" + Convert.ToUInt32(character) + "?");
            }
            return sb.ToString();
        }

        public static void FromRtf(System.Windows.Controls.RichTextBox rtb, String rtf)
        {
            System.Windows.Documents.FlowDocument doc = rtb.Document;
            doc.Blocks.Clear();

            if (rtf == "") return;

            TextRange content = new System.Windows.Documents.TextRange(doc.ContentStart, doc.ContentEnd);
            SetRtf(rtb, rtf);
        }
        public static void SetRtf(System.Windows.Controls.RichTextBox rtb, string document)
        {
            var documentBytes = Encoding.UTF8.GetBytes(document);
            using (var reader = new MemoryStream(documentBytes))
            {
                reader.Position = 0;
                rtb.SelectAll();
                rtb.Selection.Load(reader, System.Windows.DataFormats.Rtf);
            }
        }

        public static String ToRtf(System.Windows.Controls.RichTextBox rtb)
        {
            System.Windows.Documents.FlowDocument doc = rtb.Document;
            TextRange content = new System.Windows.Documents.TextRange(doc.ContentStart, doc.ContentEnd);
            MemoryStream stream = new MemoryStream();
            stream.Position = 0;
            content.Save(stream, System.Windows.DataFormats.Rtf, true);
            stream.Flush();
            stream.Position = 0;
            //Encoding enc = GetEncoding(stream);
            //stream.Position = 0;
            //TextReader reader = new StreamReader(stream, Encoding.ASCII);            
            String rtf = System.Text.Encoding.ASCII.GetString(stream.ToArray());//enc.GetString(stream.ToArray());//System.Text.Encoding.UTF8.GetString(stream.ToArray()); //reader.ReadToEnd();
            //reader.Close();
            stream.Close();
            stream.Dispose();
            return rtf;
        }


    }
}
