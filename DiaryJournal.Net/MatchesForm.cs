using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;

namespace DiaryJournal.Net
{
    public partial class MatchesForm : Form
    {
        ElementHost host1 = new ElementHost();
        System.Windows.Controls.ListView lv_wpf = new System.Windows.Controls.ListView();

        public Form myParentForm;

        public MatchesForm()
        {
            InitializeComponent();
        }

        private void MatchesForm_Load(object sender, EventArgs e)
        {
            host1.Dock = DockStyle.Fill;
            host1.AutoSize = true;
            host1.Child = lv_wpf;
            sc.Panel2.Controls.Add(host1);

            GridViewColumn gvc1 = new GridViewColumn();
            gvc1.DisplayMemberBinding = new System.Windows.Data.Binding("Index");
            gvc1.Header = "Index";
            gvc1.Width = 100;
            GridViewColumn gvc2 = new GridViewColumn();
            gvc2.DisplayMemberBinding = new System.Windows.Data.Binding("Match");
            gvc2.Header = "Match";
            gvc2.Width = 3000;

            var gridView = new GridView();
            lv_wpf.View = gridView;
            gridView.Columns.Add(gvc1);
            gridView.Columns.Add(gvc2);

            /* todo
             * 
            System.Windows.Controls.ListViewItem item1 = new System.Windows.Controls.ListViewItem();
            item1.Content = new User() { Index = 0, Match = str };
            System.Windows.Controls.ListViewItem item2 = new System.Windows.Controls.ListViewItem();
            item2.Content = new User() { Index = 1, Match = str };
            lv_wpf.Items.Add(item1);
            lv_wpf.Items.Add(item2);
            lv_wpf.UpdateLayout();

            foreach (System.Windows.Controls.ListViewItem item in lv_wpf.Items)
            {
                item.BorderThickness = new Thickness(1,1,1,1);
                item.BorderBrush = new SolidColorBrush(Colors.Gray);
            }
            lv_wpf.UpdateLayout();
            FindListViewItem(lv_wpf);
            */

        }
        public void FindListViewItem(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                System.Windows.Controls.ListViewItem lv = obj as System.Windows.Controls.ListViewItem;
                if (lv != null)
                {
                    HighlightText(lv);
                }
                FindListViewItem(VisualTreeHelper.GetChild(obj as DependencyObject, i));
            }
        }

        private void HighlightText(Object itx)
        {
            if (itx != null)
            {
                if (itx is TextBlock)
                {
                    Regex regex = new Regex("(" + "Ite" + ")", RegexOptions.IgnoreCase);
                    TextBlock tb = itx as TextBlock;

                    /*
                    if (textboxsearch.Text.Length == 0)
                    {
                        string str = tb.Text;
                        tb.Inlines.Clear();
                        tb.Inlines.Add(str);
                        return;
                    }
                    */
                    string[] substrings = regex.Split(tb.Text);
                    tb.Inlines.Clear();
                    foreach (var item in substrings)
                    {
                        if (regex.Match(item).Success)
                        {
                            Run runx = new Run(item);
                            runx.Background = System.Windows.Media.Brushes.Red;
                            tb.Inlines.Add(runx);
                        }
                        else
                        {
                            tb.Inlines.Add(item);
                        }
                    }
                    return;
                }
                else
                {
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(itx as DependencyObject); i++)
                    {
                        HighlightText(VisualTreeHelper.GetChild(itx as DependencyObject, i));
                    }
                }
            }
        }

        public class User
        {
            public int Index { get; set; }

            public string Match { get; set; }
        }
    }
}
