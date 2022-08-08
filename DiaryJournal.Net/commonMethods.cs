using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myJournal.Net
{
    public static class commonMethods
    {
        public static string Base64Encode(string plainText)
        {
            if (plainText == null)
                return "";

            if (plainText.Length == 0)
                return "";

            //var encoding = new UnicodeEncoding();
            //var plainTextBytes = encoding.GetBytes(plainText);
            //return System.Convert.ToBase64String(plainTextBytes);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            if (base64EncodedData == null)
                return "";

            if (base64EncodedData.Length == 0)
                return "";

            //var encoding = new UnicodeEncoding();
            //byte[] bytes = Convert.FromBase64String(base64EncodedData);
            //return encoding.GetString(bytes);
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string FontToString(Font font)
        {
            return font.FontFamily.Name + ":" + font.Size + ":" + (int)font.Style;
        }

        public static Font StringToFont(string font)
        {
            string[] parts = font.Split(':');
            if (parts.Length != 3)
                throw new ArgumentException("Not a valid font string", "font");

            Font loadedFont = new Font(parts[0], float.Parse(parts[1]), (FontStyle)int.Parse(parts[2]));
            return loadedFont;
        }
        public static string ColorToString(Color color)
        {
            return color.ToArgb().ToString();
        }
        public static Color StringToColor(String color)
        {
            return Color.FromArgb(int.Parse(color));
        }

        public static IEnumerable<Color> getWebColors()
        {
            return Enum.GetValues(typeof(KnownColor))
                .Cast<KnownColor>()
                .Where(k => k >= KnownColor.Transparent && k < KnownColor.ButtonFace) //Exclude system colors
                .Select(k => Color.FromKnownColor(k));
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
