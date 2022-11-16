using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RtfPipe;
using PdfSharp;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace DiaryJournal.Net
{
    public static class pdfEntry
    {
        public static byte[]? toPDF(String rtf)
        {
            // get rtf and update
            if (rtf.Length <= 0)
                return null;

            // first create html
            String html = Rtf.ToHtml(rtf);
            html = "<html><body>" + html + "</body></html>";    
            // finally output pdf
            var document = new PdfSharp.Pdf.PdfDocument();
            PdfGenerator.AddPdfPages(document, html, PageSize.A4, 10);
            Byte[] pdfBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms);
                pdfBytes = ms.ToArray();
            }
            document.Close();
            document.Dispose();
            return pdfBytes;
        }
    }
}
