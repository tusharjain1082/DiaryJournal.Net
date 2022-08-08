using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace myJournal.Net
{
    public static class rtfEntry
    {
        public static String toRtf(String rtf)
        {
            rtf = commonMethods.Base64Decode(rtf);
            return rtf;
        }
        public static bool fromRtf(ref Chapter chapter, String file, ref String rtf, String extComplete)
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
