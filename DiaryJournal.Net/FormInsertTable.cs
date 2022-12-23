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
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

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
        public VerticalAlignment cellVAlign = VerticalAlignment.Top;
        public TextAlignment cellAlignment = TextAlignment.Left;
        public int tableInnerBorderSize = 1;
        public int tableOuterBorderSize = 1;

        public bool isEditingCells = false;
        public String fontName = "Times New Roman";
        public int fontSize = 14;
        public System.Drawing.Font font = new Font("Times New Roman", 14);
        public System.Drawing.Color fontColor = System.Drawing.Color.Black;
        public System.Drawing.Color fontBackColor = System.Drawing.Color.White;
        public bool Bold = false;
        public bool Italic = false;

        public FormInsertTable()
        {
            InitializeComponent();
        }

        private void FormInsertTable_Load(object sender, EventArgs e)
        {
            cmbTableWidth.Text = tableWidth.ToString();
            cmbRows.Text = rows.ToString();
            cmbColumns.Text = columns.ToString();
            cmbTableAlignment.SelectedIndex = 0;
            cmbRowHeight.Text = rowHeight.ToString();
            cmbColumnWidth.Text = columnWidth.ToString();
            cmbCellVAlignment.SelectedIndex = 0;
            cmbCellTextAlignment.SelectedIndex = cmbCellTextAlignment.FindString(cellAlignment.ToString());
            //cmbTableInnerBorder.SelectedIndex = cmbTableInnerBorder.FindString(tableInnerBorderSize.ToString());
            //cmbTableOuterBorder.SelectedIndex = cmbTableOuterBorder.FindString(tableOuterBorderSize.ToString());
            cmbTableInnerBorderSize.Text = tableInnerBorderSize.ToString();
            cmbTableOuterBorderSize.Text = tableOuterBorderSize.ToString();

            if (isEditingCells)
                cmbColumns.Enabled = cmbRows.Enabled = cmbColumnWidth.Enabled = cmbTableOuterBorder.Enabled = false;

            txtboxFont.Text = fontName;
            txtboxFontSize.Text = fontSize.ToString();  

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
                    cellAlignment = TextAlignment.Left;
                    break;
                case "Right":
                    cellAlignment = TextAlignment.Right;
                    break;
                case "Center":
                    cellAlignment = TextAlignment.Center;
                    break;
                case "Justify":
                    cellAlignment = TextAlignment.Justify;
                    break;
                default:
                    return;
            }

            switch (cellVAlignString)
            {
                case "Top":
                    cellVAlign = VerticalAlignment.Top;
                    break;
                case "Bottom":
                    cellVAlign = VerticalAlignment.Bottom;
                    break;
                case "Center":
                    cellVAlign = VerticalAlignment.Center;
                    break;
                default:
                    return;
            }

            myResult = DialogResult.OK;
            this.Close();

        }

        private void buttonFont_Click(object sender, EventArgs e)
        {
            CustomFontDialog fontDialog = new CustomFontDialog();

            fontDialog.font = this.font;
            fontDialog.size = (int)this.fontSize;
            fontDialog.bold = this.Bold;
            fontDialog.italic = this.Italic;
            fontDialog.underline = false;
            fontDialog.fontColor = this.fontColor;
            fontDialog.fontBackColor = this.fontBackColor;
            if (fontDialog.ShowDialog() != DialogResult.OK) return;

            this.font = fontDialog.font;
            this.fontName = fontDialog.font.Name;
            this.fontColor = fontDialog.fontColor;
            this.fontBackColor = fontDialog.fontBackColor;
            this.fontSize = fontDialog.size;
            this.Bold = fontDialog.bold;
            this.Italic = fontDialog.italic;

            txtboxFont.Text = this.font.Name;
            txtboxFontSize.Text = this.fontSize.ToString(); 
        }

    }
}
