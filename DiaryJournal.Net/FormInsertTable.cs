using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing.Text;
using Elistia.DotNetRtfWriter;
using MigraDoc.DocumentObjectModel;

namespace DiaryJournal.Net
{
    public partial class FormInsertTable : Form
    {
        public textFormatting formatting = null;
        public DialogResult myResult = DialogResult.Cancel;

        public int tableWidth = 0;
        public int rows = 2;
        public int columns = 2;
        public MigraDoc.DocumentObjectModel.Tables.RowAlignment tableAlignment = MigraDoc.DocumentObjectModel.Tables.RowAlignment.Left;
        public int rowHeight = 50;
        public int columnWidth = 50;
        public MigraDoc.DocumentObjectModel.Tables.VerticalAlignment cellVAlign = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Top;
        public String fontName = "Times New Roman";
        public int fontSize = 14;
        public MigraDoc.DocumentObjectModel.ParagraphAlignment cellAlignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Left;
        public MigraDoc.DocumentObjectModel.BorderStyle tableInnerBorder = MigraDoc.DocumentObjectModel.BorderStyle.Single;
        public MigraDoc.DocumentObjectModel.BorderStyle tableOuterBorder = MigraDoc.DocumentObjectModel.BorderStyle.Single;
        public int tableInnerBorderSize = 1;
        public int tableOuterBorderSize = 1;
        

        public FormInsertTable()
        {
            InitializeComponent();
        }

        private void FormInsertTable_Load(object sender, EventArgs e)
        {
            // initialize all formatting config
            formatting = new textFormatting();
            cmbFonts.Items.AddRange(formatting.fontNames.ToArray());

            cmbTableWidth.Text = tableWidth.ToString();
            cmbRows.Text = rows.ToString();
            cmbColumns.Text = columns.ToString();
            cmbTableAlignment.SelectedIndex = 0;
            cmbRowHeight.Text = rowHeight.ToString();
            cmbColumnWidth.Text = columnWidth.ToString();
            cmbCellVAlignment.SelectedIndex = 0;
            cmbFonts.Text = fontName;
            cmbCellTextAlignment.SelectedIndex = 0;
            cmbFontSize.Text = fontSize.ToString();
            cmbTableInnerBorder.SelectedIndex = 0;
            cmbTableOuterBorder.SelectedIndex = 0;
            cmbTableInnerBorderSize.Text = tableInnerBorderSize.ToString();
            cmbTableOuterBorderSize.Text = tableOuterBorderSize.ToString();

            /*
            Elistia.DotNetRtfWriter.BorderStyle.Dashed
            Elistia.DotNetRtfWriter.BorderStyle.Double
            Elistia.DotNetRtfWriter.BorderStyle.Dotted
            Elistia.DotNetRtfWriter.BorderStyle.Single
            Elistia.DotNetRtfWriter.BorderStyle.None
            */

        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            myResult = DialogResult.Cancel;
            this.Close();
            
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            if (!int.TryParse((String)cmbTableWidth.Text, out tableWidth))
                return;

            if (!int.TryParse((String)cmbRows.Text, out rows))
                return;

            if (!int.TryParse((String)cmbColumns.Text, out columns))
                return;

            if (!int.TryParse((String)cmbRowHeight.Text, out rowHeight))
                return;

            if (!int.TryParse((String)cmbColumnWidth.Text, out columnWidth))
                return;

            fontName = (String)cmbFonts.SelectedItem;
            if (fontName == "")
                return;

            if (!int.TryParse((String)cmbFontSize.Text, out fontSize))
                return;

            if (!int.TryParse((String)cmbTableInnerBorderSize.Text, out tableInnerBorderSize))
                return;

            if (!int.TryParse((String)cmbTableOuterBorderSize.Text, out tableOuterBorderSize))
                return;

            String tableAlignmentString = (String)cmbTableAlignment.SelectedItem;
            if (tableAlignmentString == "")
                return;

            String cellTextAlignString = (String)cmbCellTextAlignment.SelectedItem;
            if (cellTextAlignString == "")
                return;

            String tableInnerBorderString = (String)cmbTableInnerBorder.SelectedItem;
            if (tableInnerBorderString == "")
                return;

            String tableOuterBorderString = (String)cmbTableOuterBorder.SelectedItem;
            if (tableOuterBorderString == "")
                return;

            String cellVAlignString = (String)cmbCellVAlignment.SelectedItem;
            if (cellVAlignString == "")
                return;

            switch (cellTextAlignString)
            {
                case "Left":
                    cellAlignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Left;
                    break;
                case "Right":
                    cellAlignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Right;
                    break;
                case "Center":
                    cellAlignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center;
                    break;
                case "Justify":
                    cellAlignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Justify;
                    break;
                default:
                    return;
            }

            switch (tableAlignmentString)
            {
                case "Left":
                    tableAlignment = MigraDoc.DocumentObjectModel.Tables.RowAlignment.Left;
                    break;
                case "Right":
                    tableAlignment = MigraDoc.DocumentObjectModel.Tables.RowAlignment.Right;
                    break;
                case "Center":
                    tableAlignment = MigraDoc.DocumentObjectModel.Tables.RowAlignment.Center;
                    break;
//                case "Justify":
  //                  tableAlignment = MigraDoc.DocumentObjectModel.Tables.RowAlignment.Justify;
            //        break;
                default:
                    return;
            }

            switch (cellVAlignString)
            {
                case "Top":
                    cellVAlign = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Top;
                    break;
                case "Bottom":
                    cellVAlign = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Bottom;
                    break;
                case "Center":
                    cellVAlign = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                    break;
                default:
                    return;
            }

            switch (tableInnerBorderString)
            {
                case "DashSmallGap":
                    tableInnerBorder = MigraDoc.DocumentObjectModel.BorderStyle.DashSmallGap;
                    break;
                case "Dot":
                    tableInnerBorder = MigraDoc.DocumentObjectModel.BorderStyle.Dot;
                    break;
                case "Single":
                    tableInnerBorder = MigraDoc.DocumentObjectModel.BorderStyle.Single;
                    break;
                case "None":
                    tableInnerBorder = MigraDoc.DocumentObjectModel.BorderStyle.None;
                    break;
                default:
                    return;
            }

            switch (tableOuterBorderString)
            {
                case "DashSmallGap":
                    tableOuterBorder = MigraDoc.DocumentObjectModel.BorderStyle.DashSmallGap;
                    break;
                case "Dot":
                    tableOuterBorder = MigraDoc.DocumentObjectModel.BorderStyle.Dot;
                    break;
                case "Single":
                    tableOuterBorder = MigraDoc.DocumentObjectModel.BorderStyle.Single;
                    break;
                case "None":
                    tableOuterBorder = MigraDoc.DocumentObjectModel.BorderStyle.None;
                    break;
                default:
                    return;
            }

            myResult = DialogResult.OK;
            this.Close();

        }
    }
}
