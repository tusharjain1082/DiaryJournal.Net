using RtfPipe.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using static DiaryJournal.Net.TemplateFormat;

namespace DiaryJournal.Net
{
    public partial class FormList : Form
    {
        public List<Object> allItems = new List<Object>();
        public ListType listType = ListType.String;
        public bool checkMultipleItems = true;

        public List<Object> outCheckedItems = new List<Object>();
        public Object? outSelectedItem = null;

        public enum ListType : int
        {
            Int32 = 10,
            Int64 = 11,
            String = 400,
            TemplateCode = 5000,
        }

        public FormList()
        {
            InitializeComponent();
        }

        private void FormList_Load(object sender, EventArgs e)
        {
            lvList.MultiSelect = checkMultipleItems;
            lvList.CheckBoxes = checkMultipleItems;
            buttonAll.Enabled = buttonNone.Enabled = checkMultipleItems;

            refillList();
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvList.CheckedItems)
            {
                Object? tag = (Object?)listViewItem.Tag;
                if (tag == null) continue;
                outCheckedItems.Add(tag);
            }

            if (lvList.SelectedItems.Count > 0) 
            {
                Object? tag = (Object?)lvList.SelectedItems[0].Tag;
                outSelectedItem = tag;
            }
        }

        public void refillList()
        {
            lvList.Items.Clear();

            foreach (Object? item in allItems)
            {
                if (item == null) continue;

                if (listType == ListType.TemplateCode)
                {
                    // template code list has to be build
                    TemplateFormat.TemplateCodeItem? templateCodeItem = (TemplateFormat.TemplateCodeItem?)item;

                    System.Windows.Forms.ListViewItem lvitem = new System.Windows.Forms.ListViewItem();
                    lvitem.Name = templateCodeItem.code.convertToString();
                    lvitem.Tag = templateCodeItem;
                    lvitem.Text = templateCodeItem.value;
                    lvitem.SubItems.Add(templateCodeItem.info);
                    lvList.Items.Add(lvitem);
                }
                else if (listType == ListType.Int32)
                {
                    insertIntegerLVItem(item);
                }
                else if (listType == ListType.Int64)
                {
                    insertIntegerLVItem(item);
                }
            }
        }
        public void insertIntegerLVItem(Object? value)
        {
            System.Windows.Forms.ListViewItem lvitem = new System.Windows.Forms.ListViewItem();
            lvitem.Name = value.ToString();
            lvitem.Tag = value;
            lvitem.Text = value.ToString();
            lvitem.SubItems.Add("");
            lvList.Items.Add(lvitem);
        }

        private void buttonAll_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvList.Items)
            {
                listViewItem.Checked = true;
                listViewItem.Selected = true;
            }
        }

        private void buttonNone_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.ListViewItem listViewItem in lvList.Items)
            {
                listViewItem.Checked = false;
                listViewItem.Selected = false;
            }
        }
    }
}
