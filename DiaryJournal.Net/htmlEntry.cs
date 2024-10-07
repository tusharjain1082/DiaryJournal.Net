using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RtfPipe;

namespace DiaryJournal.Net
{
    public static class htmlEntry
    {
        public static String toHtml(String rtf)
        {
            // get rtf and update
            if (rtf.Length <= 0)
                return "";

            String html = Rtf.ToHtml(rtf);
            return html;
        }
    }
}
