using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DiaryJournal.Net
{
    public static class rtfEntry
    {
        public static String toRtf(String rtf)
        {
            if (rtf.Length > 0)
                rtf = commonMethods.Base64Decode(rtf);

            return rtf;
        }
        public static bool fromRtf(ref Chapter chapter, String file, ref String rtf, String extComplete, bool freeStandingEntry = false)
        {
            if (!File.Exists(file))
                return false;

            rtf = File.ReadAllText(file);
            rtf = commonMethods.Base64Encode(rtf);
            entryMethods.convertEntryFilenameToChapter(ref chapter, file);
            return true;
        }

    }
}
