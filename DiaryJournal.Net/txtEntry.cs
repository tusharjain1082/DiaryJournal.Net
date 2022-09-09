using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DiaryJournal.Net
{
    public static class txtEntry
    {
        public static String toTxt(String rtf)
        {
            String text = "";

            if (rtf.Length > 0)
            {
                AdvRichTextBox rtb = new AdvRichTextBox();
                rtb.SuspendLayout();
                rtb.BeginUpdate();
                rtb.Rtf = rtf;
                rtb.ResumeLayout(true);
                rtb.EndUpdate();
                text = rtb.TextNonRaw;
                rtb.Clear();
                rtb.Dispose();
                rtb = null;
                GC.Collect();
            }
            return text;
        }
        public static bool fromTxt(ref Chapter chapter, String file, ref String rtf, String extComplete)
        {
            if (!File.Exists(file))
                return false;

            String text = "";
            try
            {
                text = File.ReadAllText(file);
            }
            catch (Exception)
            {
                using (StreamReader reader = new StreamReader(file, Encoding.Unicode))
                {
                    text = reader.ReadToEnd();
                    reader.Close(); 
                    reader.Dispose();   
                }
            }

            AdvRichTextBox rtb = new AdvRichTextBox();
            rtb.SuspendLayout();
            rtb.BeginUpdate();
            rtb.Text = text;
            rtb.ResumeLayout(true);
            rtb.EndUpdate();
            rtf = rtb.Rtf;
            rtb.Dispose();
            GC.Collect();
            entryMethods.convertEntryFilenameToChapter(ref chapter, file);
            return true;
        }

    }
}
