using System;
using System.IO;

namespace MarkupConverter
{
    public class HtmlXamlImage
    {
        public enum FloatDirection
        {
            None,
            Left,
            Right
        }

        public int Index { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public bool IsInline { get; set; }

        public FloatDirection Float { get; set; }

        public string InlineFormat { get; set; }

        public string ContentsBase64
        {
            get
            {
                if (string.IsNullOrEmpty(contentsBase64) && contents != null)
                {
                    // conversion should be avoided as it's costly
                    contentsBase64 = Convert.ToBase64String(contents);
                }
                return contentsBase64;
            }
            set
            {
                contentsBase64 = value;
                contents = null;
                contentsLength = 0;
                MimeType = null;
            }
        }
        private string contentsBase64;

        public int Size
        {
            get
            {
                if (contents != null)
                {
                    return (int)contentsLength;
                }
                else if (!string.IsNullOrEmpty(contentsBase64))
                {
                    return contentsBase64.Length * 3 / 4;
                }
                else
                {
                    return 0;
                }
            }
        }

        public byte[] Contents
        {
            get
            {
                if (contents == null && !string.IsNullOrEmpty(contentsBase64))
                {
                    // conversion should be avoided as it's costly
                    contents = Convert.FromBase64String(contentsBase64);
                    contentsLength = contents.Length;
                }
                return contents;
            }
        }
        private byte[] contents;
        private long contentsLength;

        public void SetContents(byte[] array, long length)
        {
            contents = array;
            contentsLength = length;
            contentsBase64 = null;
            MimeType = null;
        }

        public string Alt { get; set; }

        public string FileName { get; set; }

        public string MimeType { get; set; }

        public bool WriteToStream(Stream stream)
        {
            var c = Contents;
            if (c != null && contentsLength > 0)
            {
                stream.Write(Contents, 0, (int)contentsLength);
                return true;
            }
            return false;
        }

        public byte[] GetPartialContents(int maxSize)
        {
            if (contents != null)
            {
                return contents;
            }
            else if (!string.IsNullOrEmpty(contentsBase64))
            {
                var partBase64 = contentsBase64.ToCharArray(0, Math.Min(contentsBase64.Length, maxSize));
                return Convert.FromBase64CharArray(partBase64, 0, partBase64.Length);
            }
            return null;
        }
    }
}