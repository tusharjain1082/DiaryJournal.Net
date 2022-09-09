using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Xml;

namespace DiaryJournal.Net
{
    public static class cfgEntry
    {

        public static bool fromCfg(ref Chapter chapter, String file)
        {
            if (!File.Exists(file))
                return false;

            // read entire file
            String body = File.ReadAllText(file);

            // load values
            String[] values = body.Split(":::" + Environment.NewLine);
            if (values.Length == 0)
                return false;

            // load values into the chapter/node
            chapter.Id = Int64.Parse(values[0]);
            chapter.parentId = Int64.Parse(values[1]);
            chapter.Title = values[2];
            chapter.chapterDateTime = DateTime.ParseExact(values[3], "yyyy-MM-dd-HH-mm-ss-fff",
                  System.Globalization.CultureInfo.InvariantCulture);
            chapter.IsDeleted = bool.Parse(values[4]);
            chapter.nodeType = (NodeType)Enum.Parse(typeof(NodeType), values[5]);
            chapter.specialNodeType = (SpecialNodeType)Enum.Parse(typeof(SpecialNodeType), values[6]);
            chapter.domainType = (DomainType)Enum.Parse(typeof(DomainType), values[7]);
            chapter.HLFont = values[8];
            chapter.HLFontColor = values[9];
            chapter.HLBackColor = values[10];
            return true;
        }

        public static String toCfg(ref Chapter chapter)
        {
            String body = "";
            body += String.Format(@"{0}:::" + Environment.NewLine, chapter.Id.ToString());
            body += String.Format(@"{0}:::" + Environment.NewLine, chapter.parentId.ToString());
            body += String.Format(@"{0}:::" + Environment.NewLine, chapter.Title);
            body += String.Format(@"{0}:::" + Environment.NewLine, chapter.chapterDateTime.ToString("yyyy-MM-dd-HH-mm-ss-fff"));
            body += String.Format(@"{0}:::" + Environment.NewLine, chapter.IsDeleted.ToString());
            body += String.Format(@"{0}:::" + Environment.NewLine, chapter.nodeType.ToString());
            body += String.Format(@"{0}:::" + Environment.NewLine, chapter.specialNodeType.ToString());
            body += String.Format(@"{0}:::" + Environment.NewLine, chapter.domainType.ToString());
            body += String.Format(@"{0}:::" + Environment.NewLine, chapter.HLFont);
            body += String.Format(@"{0}:::" + Environment.NewLine, chapter.HLFontColor);
            body += String.Format(@"{0}:::" + Environment.NewLine, chapter.HLBackColor);
            return body;
        }
    }
}
