using RtfPipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DiaryJournal.Net
{
    public class TemplateFormat
    {
        public enum TemplateCode : int
        {
            TF_FullDate = 0,
            TF_FullTime = 1,
        }

        public class TemplateCodeItem
        {
            public TemplateCode code;
            public String value = "";
            public String info = "";

            public TemplateCodeItem() { }
            public TemplateCodeItem(TemplateCode code, String value, String info)
            {
                this.code = code;
                this.value = value;
                this.info = info;
            }
        }

        public Dictionary<TemplateCode, TemplateCodeItem> TemplateCodes = new Dictionary<TemplateCode, TemplateCodeItem>()
        {
            { TemplateCode.TF_FullDate, new TemplateCodeItem(TemplateCode.TF_FullDate, "[%TF_FULLDATE%]", "full proper date") },
            { TemplateCode.TF_FullTime, new TemplateCodeItem(TemplateCode.TF_FullTime, "[%TF_FULLTIME%]", "full proper time") },
        };

        public TemplateCodeItem? getTemplateCodeItem(TemplateCode key)
        {
            TemplateCodeItem? value = null;
            if (TemplateCodes.TryGetValue(key, out value))
                return value;
            else
                return null;
        }

        public static String? getTemplateCodeName(TemplateCode key)
        {
            return key.convertToString();
        }

        public static TemplateCode? getTemplateCodeEnumByName(String name)
        {
            object? key = null;
            if (Enum.TryParse(typeof(TemplateCode), name, out key))
            {
                return (TemplateCode?)key;
            }
            return null;
        }
        public List<Object> findAllTemplateCodeItems()
        {
            List<Object> list = new List<Object>();
            foreach (KeyValuePair<TemplateCode, TemplateCodeItem> item in TemplateCodes)
                list.Add(item.Value);

            return list;
        }

        // this is the primary template transform function which transforms template into final finished product
        public void transform(System.Windows.Controls.RichTextBox rtb, TemplateCodeItem code)
        {
            DateTime dateTime = DateTime.Now;

            rtb.BeginChange();

            // transform all codes
            switch (code.code)
            {
                case TemplateCode.TF_FullDate:
                    WpfRtbMethods.Replace(rtb, code.value, dateTime.ToString("dddd, dd MMMM yyyy"));

                    break;
                case TemplateCode.TF_FullTime:
                    WpfRtbMethods.Replace(rtb, code.value, dateTime.ToString("hh:mm:ss tt"));

                    break;
            }
            rtb.EndChange();
            rtb.UpdateLayout();
        }

    }

    public class myTemplates
    {

    }

}
