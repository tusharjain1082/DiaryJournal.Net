using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing.Text;
using Elistia.DotNetRtfWriter;
using MigraDocPlusXml;
using MigraDoc.Extensions;

namespace myJournal.Net
{
    public class textFormatting
    {
        public string familyName = "";
        public string familyList = "";
        public FontFamily[]? fontFamilies = null;
        public List<String> fontNames = new List<string>();
        public int selStartIndex = 0;
        public int selLength = 0;

        public textFormatting()
        {
            initFonts();
        }

        public void initFonts()
        {
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();

            // Get the array of FontFamily objects.
            fontFamilies = installedFontCollection.Families;

            fontNames.Clear();
            for (long j = 0; j < fontFamilies.LongCount(); ++j)
                fontNames.Add(fontFamilies[j].Name);

        }

        public FontFamily findFont(String fontName)
        {
            foreach (FontFamily fontFamily in fontFamilies)
            {
                if (fontFamily.Name == fontName)
                    return fontFamily;
            }
            return null;
        }

        public void formatLeftJustify(AdvRichTextBox rtb, ToolStripButton button)
        {
            rtb.SelectionAlignment = TextAlign.Left;
        }
        public void formatJustify(AdvRichTextBox rtb, ToolStripButton button)
        {
            rtb.SelectionAlignment = TextAlign.Justify;
        }
        public void formatRightJustify(AdvRichTextBox rtb, ToolStripButton button)
        {
            rtb.SelectionAlignment = TextAlign.Right;
        }
        public void formatCenterJustify(AdvRichTextBox rtb, ToolStripButton button)
        {
            rtb.SelectionAlignment = TextAlign.Center;
        }

        public void formatStrikeout(AdvRichTextBox rtb, ToolStripButton button)
        {
            if (rtb.SelectionFont.Strikeout == true)
                button.Checked = false;
            else
                button.Checked = true;

            rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style ^ System.Drawing.FontStyle.Strikeout);

        }

        public void formatUnderline(AdvRichTextBox rtb, ToolStripButton button)
        {
            if (rtb.SelectionFont.Underline == true)
                button.Checked = false;
            else
                button.Checked = true;

            rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style ^ System.Drawing.FontStyle.Underline);

        }
        public void formatForceSetUnsetBold(AdvRichTextBox rtb, ToolStripMenuItem button, bool set)
        {
            int end = selStartIndex + selLength;
            int start = selStartIndex;
            for (int index = start; index < end; index++)
            {
                if (set)
                {
                    rtb.Select(index, 1);
                    if (!rtb.SelectionFont.Bold)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style | System.Drawing.FontStyle.Bold);
                }
                else
                {
                    rtb.Select(index, 1);
                    if (rtb.SelectionFont.Bold)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style & ~System.Drawing.FontStyle.Bold);
                }
            }

        }

        public void formatForceSetUnsetItalics(AdvRichTextBox rtb, ToolStripMenuItem button, bool set)
        {
            int end = selStartIndex + selLength;
            int start = selStartIndex;
            for (int index = start; index < end; index++)
            {
                if (set)
                {
                    rtb.Select(index, 1);
                    if (!rtb.SelectionFont.Italic)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style | System.Drawing.FontStyle.Italic);
                }
                else
                {
                    rtb.Select(index, 1);
                    if (rtb.SelectionFont.Italic)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style & ~System.Drawing.FontStyle.Italic);
                }
            }
        }
        public void formatForceSetUnsetUnderline(AdvRichTextBox rtb, ToolStripMenuItem button, bool set)
        {
            int end = selStartIndex + selLength;
            int start = selStartIndex;
            for (int index = start; index < end; index++)
            {
                if (set)
                {
                    rtb.Select(index, 1);
                    if (!rtb.SelectionFont.Underline)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style | System.Drawing.FontStyle.Underline);
                }
                else
                {
                    rtb.Select(index, 1);
                    if (rtb.SelectionFont.Underline)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style & ~System.Drawing.FontStyle.Underline);
                }
            }
        }
        public void formatForceSetUnsetStrikeout(AdvRichTextBox rtb, ToolStripMenuItem button, bool set)
        {
            int end = selStartIndex + selLength;
            int start = selStartIndex;
            for (int index = start; index < end; index++)
            {
                if (set)
                {
                    rtb.Select(index, 1);
                    if (!rtb.SelectionFont.Strikeout)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style | System.Drawing.FontStyle.Strikeout);
                }
                else
                {
                    rtb.Select(index, 1);
                    if (rtb.SelectionFont.Strikeout)
                        rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style & ~System.Drawing.FontStyle.Strikeout);
                }
            }
        }

        public void formatBold(AdvRichTextBox rtb, ToolStripButton button)
        {
            if (rtb.SelectionFont.Bold == true)
                button.Checked = false;
            else
                button.Checked = true;

            rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style ^ System.Drawing.FontStyle.Bold);

        }
        public void formatItalics(AdvRichTextBox rtb, ToolStripButton button)
        {
            
            if (rtb.SelectionFont.Italic == true)
                button.Checked = false;
            else
                button.Checked = true;

            rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style ^ System.Drawing.FontStyle.Italic);
            
            /*
            int end = selStartIndex + selLength;
            int start = selStartIndex;
            for (int index = start; index < end; index++)
            {
                rtb.Select(index, 1);
                if (!rtb.SelectionFont.Italic)
                rtb.SelectionFont = new Font(rtb.SelectionFont, rtb.SelectionFont.Style | FontStyle.Italic);
            }
            */
        }
        public void formatIncreaseFontSize(AdvRichTextBox rtb)
        {
            if (rtb.SelectionFont == null)
                return;

            if (rtb.SelectionFont.Size >= 80)
                return;

            rtb.SelectionFont = new Font(rtb.SelectionFont.FontFamily, rtb.SelectionFont.Size + 2, rtb.SelectionFont.Style);
        }
        public void formatDecreaseFontSize(AdvRichTextBox rtb)
        {
            if (rtb.SelectionFont == null)
                return;

            if (rtb.SelectionFont.Size <= 6)
                return;

            rtb.SelectionFont = new Font(rtb.SelectionFont.FontFamily, rtb.SelectionFont.Size - 2, rtb.SelectionFont.Style);
        }

        public void formatLineSpacing(AdvRichTextBox rtb, RichTextBoxEx.LineSpaceTypes type)
        {
            rtb.LineSpace(type, false);
        }

        public void formatIndentRight(AdvRichTextBox rtb, ToolStripButton buttonLeftIndent, ToolStripButton buttonRightIndent)
        {
            rtb.SelectionIndent += 30;
            buttonRightIndent.Checked = true;
            buttonLeftIndent.Checked = false;
        }
        public void formatIndentLeft(AdvRichTextBox rtb, ToolStripButton buttonLeftIndent, ToolStripButton buttonRightIndent)
        {
            if (rtb.SelectionIndent > 30)
            {
                rtb.SelectionIndent -= 30;
            }
            else if (rtb.SelectionIndent == 30)
            {
                rtb.SelectionIndent -= 30;
                buttonRightIndent.Checked = false;
                buttonLeftIndent.Checked = true;
            }
        }

        public void formatBullets(AdvRichTextBox rtb, ToolStripButton buttonBullets, ToolStripButton buttonNumberedList)
        {
            if (rtb.SelectionBullet == true)
            {
                buttonBullets.Checked = false;
                rtb.SelectionBullet = false;
            }
            else
            {
                rtb.NumberedBullet(false);
                buttonBullets.Checked = true;
                rtb.SelectionBullet = true;
            }
        }

        public void formatNumberedList(AdvRichTextBox rtb, ToolStripButton buttonBullets, ToolStripButton buttonNumberedList)
        {
            // rtf insertion
        //    rtb.SelectedRtf = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Calibri;}}" +
          //      @"{\*\generator Msftedit 5.41.21.2510; }\viewkind4\uc1\pard{\pntext\f0 1.\tab}" +
            //                @"{\*\pn\pnlvlbody\pnf0\pnindent0\pnstart1\pndec{\pntxta.} }" +
              //  @"\fi \li720\sa200\sl276\slmult1\lang9\f0\fs22 \par" +
                //@"}";
            
            if (buttonNumberedList.Checked == true)
            {
                buttonNumberedList.Checked = false;
                rtb.NumberedBullet(false);
            }
            else
            {
                rtb.SelectionBullet = false;
                buttonNumberedList.Checked = true;
                rtb.BulletType = RichTextBoxEx.AdvRichTextBulletType.Number;
                rtb.NumberedBullet(true);
            }
            
        }

        public void formatFont(AdvRichTextBox rtb, String fontName)
        {
            int end = selStartIndex + selLength;
            int start = selStartIndex;
            for (int index = start; index < end; index++)
            {
                rtb.Select(index, 1);
                rtb.SelectionFont = new Font(fontName, rtb.SelectionFont.Size, rtb.SelectionFont.Style);
            }

        }

        public void formatFontSize(AdvRichTextBox rtb, int size)
        {
            int end = selStartIndex + selLength;
            int start = selStartIndex;
            for (int index = start; index < end; index++)
            {
                rtb.Select(index, 1);
                rtb.SelectionFont = new Font(rtb.SelectionFont.FontFamily, size, rtb.SelectionFont.Style);
            }

        }
        public void formatFontColor(AdvRichTextBox rtb, System.Drawing.Color color)
        {
            rtb.SelectionColor = color;
        }
        public void formatBackColor(AdvRichTextBox rtb, System.Drawing.Color color)
        {
            rtb.SelectionBackColor = color;
        }

        public void formatInsertTable(AdvRichTextBox rtb, int rows, int columns, int tablewidth,
            MigraDoc.DocumentObjectModel.Tables.RowAlignment tablealignment,
            MigraDoc.DocumentObjectModel.BorderStyle tableOuterBorder, int tableOuterBorderSize,
            MigraDoc.DocumentObjectModel.BorderStyle tableInnerBorder, int tableInnerBorderSize, 
            int rowheight, int columnWidth, String fontname, int fontsize, 
            MigraDoc.DocumentObjectModel.Tables.VerticalAlignment alignVertical,
            MigraDoc.DocumentObjectModel.ParagraphAlignment paragraphalign)
        {
            MigraDoc.DocumentObjectModel.Document doc = new MigraDoc.DocumentObjectModel.Document();
            //StyleDoc(doc);
            MigraDoc.DocumentObjectModel.Section section = doc.AddSection();

            MigraDoc.DocumentObjectModel.Tables.Table table = new MigraDoc.DocumentObjectModel.Tables.Table();
            table.Borders.Width = tableInnerBorderSize;
            table.Borders.Style = tableInnerBorder;
            table.Format.Font.Size = fontsize;
            table.Format.Font.Name = fontname;
            table.Rows.Alignment = tablealignment;

            for (int j = 0; j < columns; j++)
            {
                MigraDoc.DocumentObjectModel.Tables.Column column = table.AddColumn(MigraDoc.DocumentObjectModel.Unit.FromPoint(columnWidth));
            }

            for (int i = 0; i < rows; i++)
            {
                MigraDoc.DocumentObjectModel.Tables.Row row = table.AddRow();
                row.Height = rowheight;

                for (int j = 0; j < columns; j++)
                {
                    MigraDoc.DocumentObjectModel.Tables.Cell cell = row.Cells[j];
                    cell.VerticalAlignment = alignVertical;
                    cell.Format.Font.Size = fontsize;
                    cell.Format.Font.Name = fontname;
                    cell.Format.Alignment = paragraphalign;
                }
                // reset
            }

            table.SetEdge(0, 0, columns, rows, MigraDoc.DocumentObjectModel.Tables.Edge.Box,  tableOuterBorder, tableOuterBorderSize,
                MigraDoc.DocumentObjectModel.Colors.Black);
            doc.LastSection.Add(table);

            var renderer = new MigraDoc.RtfRendering.RtfDocumentRenderer();
            String rtf = renderer.RenderToString(doc, "");
            rtb.SelectedRtf = rtf;
        }

        public void formatInsertTable(AdvRichTextBox rtb, int rows, int columns, int tablewidth, Align tablealignment,
            Elistia.DotNetRtfWriter.BorderStyle tableOuterBorder, int tableOuterBorderSize, Elistia.DotNetRtfWriter.BorderStyle tableInnerBorder,
            int tableInnerBorderSize, int rowheight, int columnWidth, String fontname, int fontsize, AlignVertical alignVertical, Align paragraphalign)
        {
            RtfTable rtfTable = null;
            Elistia.DotNetRtfWriter.RtfDocument doc = new RtfDocument(PaperSize.A3, PaperOrientation.Portrait, Lcid.English);
            FontDescriptor font = doc.CreateFont(fontname);
            rtfTable = doc.AddTable(rows, columns, tablewidth, fontsize);
            rtfTable.SetOuterBorder(tableOuterBorder, tableOuterBorderSize);
            rtfTable.SetInnerBorder(tableInnerBorder, tableInnerBorderSize);
            rtfTable.Alignment = tablealignment;

            for (int i = 0; i < rows; i++)
            {
                rtfTable.SetRowHeight(i, rowheight);
                for (int j = 0; j < columns; j++)
                {
                    if (columnWidth > 0)
                        rtfTable.SetColWidth(j, columnWidth);

                    RtfTableCell cell = rtfTable.Cell(i, j);
                    cell.AlignmentVertical = alignVertical;
                    RtfParagraph rtfParagraph = cell.AddParagraph();
                    RtfCharFormat charFormat = rtfParagraph.AddCharFormat();
                    charFormat.Font = font;
                    charFormat.FontSize = fontsize;
                    rtfParagraph.Alignment = paragraphalign;
                }
            }

            rtb.SelectedRtf = doc.Render();
            rtb.Select(rtb.TextLength, 0);
            rtb.SelectedRtf = File.ReadAllText(@"Q:\testoutput.rtf");
        }

        public void formatInsertImage(AdvRichTextBox rtb, Image image)
        {
            if (image == null)
                return;

            //using (var bmp = new Bitmap(image)
            //{
                rtb.SelectedRtf = ImageToRtf.Convert(image, Color.White);
            //}
        }
    }

    public static class ImageToRtf
    {
        public static void Write16(StringBuilder sb, params int[] args)
        {
            foreach (int w in args)
            {
                sb.AppendFormat("{0:X2}{1:X2}", w & 0xff, (w >> 8) & 0xff);
            }
        }

        public static string Convert(Image img, Color back)
        {
            int w = img.Width, h = img.Height;
            int picw = w * 2540 / 96, pich = h * 2540 / 96;
            int picwgoal = w * 1440 / 96, pichgoal = h * 1440 / 96;

            var sb = new StringBuilder();
            sb.AppendFormat(
                @"{{\rtf1{{\pict\wmetafile8\picw{0}\pich{1}\picwgoal{2}\pichgoal{3} ",
                picw, pich, picwgoal, pichgoal);

            byte[] bmp;
            using (var bitmap = new Bitmap(w, h))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.Clear(back);
                    g.DrawImage(img, 0, 0, w, h);
                }
                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Bmp);
                    bmp = ms.ToArray();
                }
            }

            int size1 = 14 + (bmp.Length - 14) / 2;
            int size2 = 9 + 5 + 5 + size1 + 3;

            Write16(sb,
                // META_HEADER Record
                1, 9, 0x300, size2, size2 >> 16, 0, size1, size1 >> 16, 0,
                // META_SETWINDOWORG Record
                5, 0, 0x20b, 0, 0,
                // META_SETWINDOWEXT Record
                5, 0, 0x20c, pich, picw,
                // META_STRETCHDIB Record
                size1, size1 >> 16, 0xf43, 0x20, 0xcc, 0, h, w, 0, 0, pich, picw, 0, 0
            );
            sb.Append(BitConverter.ToString(bmp, 14).Replace("-", ""));
            Write16(sb, 3, 0, 0); // META_EOF Record
            sb.Append("}}");

            return sb.ToString();
        }
    }
}
