using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Sgoliver.NRtfTree.Core;
using Net.Sgoliver.NRtfTree.Util;
using System.Text.RegularExpressions;

namespace DiaryJournal.Net
{
    public static class theJournalMethods
    {
        public static Dictionary<String, String> tableFixes = new Dictionary<String, String>
        {
            // 1st table
            /*
            {"cellx210", "cellx1425"},
            {"cellx1005", "cellx10470"},
            */

            // 2nd table
            /*
            {"cellx1815", "cellx1657"},
            {"cellx3720", "cellx3337"},
            {"cellx5640", "cellx5032"},
            {"cellx7560", "cellx6712"},
            {"cellx9495", "cellx8407"},
            {"cellx11415", "cellx10087"},
            {"cellx13350", "cellx11782"},
            {"cellx15271", "cellx13470"},
            */
            // font sizes
            //                {@"\fs40 ", @"\fs28 "},
            //              {@"\fs36 ", @"\fs28 "},

        };

        public static Chapter? convertFilenameToChapter(String file)
        {
            Chapter chapter = new Chapter();
            FileInfo fileInfo = new FileInfo(file);
            String filename = fileInfo.Name;
            String title = "";
            String dateTime = "";
            String dateTimeFormat = "";
            DateTime chapterDate = DateTime.Now;

            // formal entry
            String pattern0 = @"(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d)(\..*)";
            // child loose leaf entry
            String pattern1 = @"(\d\d\d\d-\d\d-\d\d-\d\d-\d\d)\+(.*)(\..*)";
            // formal entry with title
            String pattern2 = @"(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d)(\s+-\s+)(.*)(\..*)";
            // formal entry with duplication index
            String pattern3 = @"(\d\d\d\d-\d\d-\d\d-\d\d-\d\d-\d\d)(.*)(\([0-9]+\))(.*)(\..*)";
            // child loose leaf entry multiple parent-child levels
            String pattern4 = @"(\d\d\d\d-\d\d-\d\d-\d\d-\d\d)(\+)([0-9]*)(--)(.*)(\.[a-zA-Z].*)";

            Regex regex0 = new Regex(pattern0, RegexOptions.IgnoreCase);
            MatchCollection matches0 = regex0.Matches(filename);

            Regex regex1 = new Regex(pattern1, RegexOptions.IgnoreCase);
            MatchCollection matches1 = regex1.Matches(filename);

            Regex regex2 = new Regex(pattern2, RegexOptions.IgnoreCase);
            MatchCollection matches2 = regex2.Matches(filename);

            Regex regex3 = new Regex(pattern3, RegexOptions.IgnoreCase);
            MatchCollection matches3 = regex3.Matches(filename);

            Regex regex4 = new Regex(pattern4, RegexOptions.IgnoreCase);
            MatchCollection matches4 = regex4.Matches(filename);

            /*
            if (matches4.Count >= 1)
            {
                if (matches4[0].Groups.Count < 7)
                {

                }
                String[] values = matches4[0].Groups[5].Value.Split("-");
                values = values;
            }
            if (matches0.Count == 0 && matches1.Count == 0 && matches2.Count == 0)
            {
                title = "";

            }
            */

            if (matches0.Count > 0)
            {
                if (matches0[0].Groups.Count < 2)
                    return null;
                
                dateTime = matches0[0].Groups[1].Value;
                dateTimeFormat = @"yyyy-MM-dd-HH-mm-ss";
            }
            else if (matches1.Count > 0)
            {
                if (matches1[0].Groups.Count < 2)
                    return null;

                dateTime = matches1[0].Groups[1].Value;
                dateTimeFormat = @"yyyy-MM-dd-HH-mm";
                title = matches1[0].Groups[2].Value;
            }
            else if (matches2.Count > 0)
            {
                if (matches2[0].Groups.Count < 5)
                    return null;

                dateTime = matches2[0].Groups[1].Value;
                dateTimeFormat = @"yyyy-MM-dd-HH-mm-ss";
                title = matches2[0].Groups[3].Value;
            }
            else if (matches3.Count > 0)
            {
                if (matches3[0].Groups.Count < 5)
                    return null;

                dateTime = matches3[0].Groups[1].Value;
                dateTimeFormat = @"yyyy-MM-dd-HH-mm-ss";
                title = matches3[0].Groups[3].Value;
            }
            else
            {
                return null;
            }

            try
            {
                chapterDate = DateTime.ParseExact(dateTime, dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
                chapter.chapterDateTime = chapterDate;
            }
            catch (Exception)
            {

            }
            chapter.Title = title;
            return chapter;
        }

        public class rowColumns
        {
            public long row = 0;
            public long columnIndex = 0;
            public long totalColumns = 0;
            public RtfTreeNode? column = null;
        }

        public static List<rowColumns> autoloadTemplates()
        {
            String templatefilename = String.Format("{0}", "0--2022-08-02-12-39-57-246----.rtf");
            String file = Path.Combine(Application.StartupPath, Path.GetFileName(templatefilename));

            List<RtfTreeNode> list = new List<RtfTreeNode>();
            List<rowColumns> template = new List<rowColumns>();

            RtfTree tree = new RtfTree();
            try
            {
                tree.LoadRtfFile(file);
            }
            catch (Exception)
            {
                return template;
            }

            RtfTreeNode node = tree.RootNode.ChildNodes[0];
            // first get all rows and all columns which they contain.
            int rows = 0;
            int columns = 0;
            bool rowstart = false;
            foreach (RtfTreeNode child in node.ChildNodes)
            {
                if (child.NodeKey == "trowd")
                    rowstart = true;

                if (child.NodeKey == "cellx")
                {
                    if (rowstart)
                    {
                        rowColumns col = new rowColumns();
                        col.column = child;
                        col.row = rows;
                        col.columnIndex = columns;
                        template.Add(col);
                        columns++;
                    }
                }

                if (child.NodeKey == "row")
                {
                    List<rowColumns> rowNodes = template.FindAll(x => x.row == rows);
                    foreach (rowColumns entry in rowNodes)
                    {
                        entry.totalColumns = columns;
                    }

                    rowstart = false;
                    rows++;
                    columns = 0;
                }
            }

            // finally output
            return template;

        }

        public static String fixTheJournalRtfEntry(String rtf)
        {
            if (rtf == null)
                return "";

            List<rowColumns> templates = autoloadTemplates();
            if (templates.Count == 0)
                return rtf;

            RtfTree tree = new RtfTree();
            try
            {
                tree.LoadRtfText(rtf);
            }
            catch (Exception)
            {
                return "";
            }

            /*
            RtfTreeNode node = tree.RootNode.ChildNodes[0];

            List<RtfTreeNode> list = new List<RtfTreeNode>();
            List<(int, int, RtfTreeNode)> rowscolumns = new List<(int, int, RtfTreeNode)>();

            // first get all rows and all columns which they contain.
            int rows = 0;
            int columns = 0;
            bool rowstart = false;
            foreach (RtfTreeNode child in node.ChildNodes)
            {
                if (child.Rtf == "\\trowd")
                    rowstart = true;

                if (child.NodeKey == "cellx")
                {
                    if (rowstart)
                    {
                        rowscolumns.Add((rows, columns, child));
                        columns++;
                    }
                }

                if (child.NodeKey == "row")
                {
                    rowstart = false;
                    rows++;
                    columns = 0;
                }
            }

            // now row by row process all columns based on their row condition whatever table/row/column is of interest.
            for (int row = 0; row < rows; row++)
            {
                int size = 0;
                List<(int, int, RtfTreeNode)> childcolumns = rowscolumns.FindAll(x => x.Item1 == row);//Find(x => x.Item1 == row);
                if (childcolumns.Count == 8)
                {
                    List<rowColumns> templateMatch = template.FindAll(x => x.totalColumns == 8);
                    int templateIndex = 0;
                    foreach ((int, int, RtfTreeNode) col in childcolumns)
                        col.Item3.Parameter = templateMatch[templateIndex++].column.Parameter;
                }
            }
            */

            List<rowColumns> matchedEntries = new List<rowColumns>();
            long rows = 0;
            long columns = 0;
            long totalColumns = 0;
            bool rowStarted = false;
            CallNonRecursive(tree.RootNode, matchedEntries, ref rows, ref columns, ref totalColumns, ref rowStarted);

            // setup matched entries
            setMatchedEntries(matchedEntries, ref rows);

            // finally modify the matched entries with templates
            modifyMatchedEntries(templates, matchedEntries, ref rows);

            // finally get the updated rtf
            rtf = tree.Rtf;

            //            foreach (KeyValuePair<String, String> pair in tableFixes)
            //              rtf = rtf.Replace(pair.Key, pair.Value);

            return rtf;
        }

        public static void parseIndividualNode(RtfTreeNode treeNode, List<rowColumns> list,
            ref long rows, ref long columns, ref long totalColumns, ref bool rowstarted)
        {
            // first get all rows and all columns which they contain.

            if (treeNode.NodeKey == "trowd")
            {
                rows++;
                columns = 0;
            }
            else if (treeNode.NodeKey == "cellx")
            {
                rowColumns col = new rowColumns();
                col.column = treeNode;
                col.row = rows;
                col.columnIndex = columns;
                list.Add(col);
                columns++;
            }
        }

        // 2nd setup configuration in all selected entries.
        public static void setMatchedEntries(List<rowColumns> list, ref long rows)
        {
            // now row by row process all columns based on their row condition whatever table/row/column is of interest.
            for (long row = 1; row <= rows; row++)
            {
                List<rowColumns> matchedColumns = list.FindAll(x => x.row == row);
                long count = matchedColumns.LongCount();

                if (count <= 0)
                    continue;

                foreach (rowColumns col in matchedColumns)
                    col.totalColumns = count;

            }

        }

        // 3rd and finally modify all selected entries.
        public static void modifyMatchedEntries(List<rowColumns> templates, List<rowColumns> list, ref long rows)            
        {
            // now row by row process all columns based on their row condition whatever table/row/column is of interest.
            for (long row = 1; row <= rows; row++)
            {
                List<rowColumns> matchedColumns = list.FindAll(x => x.row == row);
                long count = matchedColumns.LongCount();
                /*if (matchedColumns.LongCount() == 5)
                {
                    List<rowColumns> matchedtemplate = templates.FindAll(x => x.totalColumns == 5);
                    int templateIndex = 0;
                    foreach (rowColumns col in matchedColumns)
                        col.column.Parameter = matchedtemplate[templateIndex++].column.Parameter;
                }*/
                if (count == 8)
                {
                    List<rowColumns> matchedtemplate = templates.FindAll(x => x.totalColumns == 8);
                    int templateIndex = 0;
                    foreach (rowColumns col in matchedColumns)
                        col.column.Parameter = matchedtemplate[templateIndex++].column.Parameter;
                }
                else if (count == 2)
                {
                    List<rowColumns> matchedtemplate = templates.FindAll(x => x.totalColumns == 2);
                    int templateIndex = 0;
                    foreach (rowColumns col in matchedColumns)
                        col.column.Parameter = matchedtemplate[templateIndex++].column.Parameter;
                }

            }
        }

        public static void parseNonRecursive(RtfTreeNode treeNode, List<rowColumns> list, ref long rows, ref long columns, ref long totalColumns,
             ref bool rowStarted)
        {

            if (treeNode != null)
            {
                //Using a queue to store and process each node in the TreeView
                Queue<RtfTreeNode> staging = new Queue<RtfTreeNode>();
                staging.Enqueue(treeNode);

                while (staging.Count > 0)
                {
                    RtfTreeNode childNode = staging.Dequeue();

                    if (childNode.ChildNodes != null)
                    {
                        foreach (RtfTreeNode node in childNode.ChildNodes)
                        {
                            staging.Enqueue(node);
                        }
                    }

                    // todo put here the individual child node parsing code
                    parseIndividualNode(childNode, list, ref rows, ref columns, ref totalColumns, ref rowStarted);

                }
            }
        }

        // Call the procedure using the TreeView.
        public static void CallNonRecursive(RtfTreeNode root, List<rowColumns> list, ref long rows, ref long columns, ref long totalColumns,
             ref bool rowStarted)
        {
            foreach (RtfTreeNode n in root.ChildNodes)   
            {
                parseNonRecursive(n, list, ref rows, ref columns, ref totalColumns, ref rowStarted);
                //PrintNonRecursive(n);
            }
        }
    }
}
