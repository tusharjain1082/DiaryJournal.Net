using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace DiaryJournal.Net
{
    public static class commonMethods
    {
        public static bool SecureEraseFile(String strPath, int iterations, bool deleteFile = false)
        {
            FileStream fs = null;
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            int sectorSize = 1048576; // 1 mb

            // this method erases the file beyond any recovery. data is completely destroyed. this is for security reasons.

            try
            {
                for (int i = 0; i < iterations; i++)
                {
                    fs = new FileStream(strPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, sectorSize, FileOptions.RandomAccess);
                    fs.Seek(0, SeekOrigin.Begin);
                    long length = fs.Length;

                    long sectors = length / sectorSize;
                    byte[] sector = new byte[sectorSize];

                    // first erase sector by sector
                    for (long ctr = 0; ctr < sectors; ctr++)
                    {
                        // erase 3 times every sector. this overwrites beyond recovery.
                        long pos = fs.Position;
                        rng.GetBytes(sector);
                        fs.Write(sector, 0, sectorSize);
                        fs.Flush();
                        fs.Seek(pos, SeekOrigin.Begin);
                        rng.GetBytes(sector);
                        fs.Write(sector, 0, sectorSize);
                        fs.Flush();
                        fs.Seek(pos, SeekOrigin.Begin);
                        rng.GetBytes(sector);
                        fs.Write(sector, 0, sectorSize);
                        fs.Flush();
                    }

                    // then erase overwrite the remaining bytes 3 times so that the data is completely destroyed.
                    if (fs.Position != fs.Length)
                    {
                        long pos = fs.Position;
                        rng.GetBytes(sector);
                        fs.Write(sector, 0, (int)(fs.Length % sectorSize));
                        fs.Flush();
                        fs.Seek(pos, SeekOrigin.Begin);
                        rng.GetBytes(sector);
                        fs.Write(sector, 0, (int)(fs.Length % sectorSize));
                        fs.Flush();
                        fs.Seek(pos, SeekOrigin.Begin);
                        rng.GetBytes(sector);
                        fs.Write(sector, 0, (int)(fs.Length % sectorSize));
                        fs.Flush();
                    }

                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }

                // iterations erasure completed, now finally delete the file if required.
                if (deleteFile)
                    DeleteFile(strPath);
            }
            catch
            {
                rng.Dispose();
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }
                return false;
            }
            rng.Dispose();
            return true;
        }

        public static bool DeleteFile(String strPathFile)
        {
            if (strPathFile == "")
                return false;

            if (!File.Exists(strPathFile))
                return true;

            try
            {
                File.Delete(strPathFile); ;
            }
            catch
            {
                // Access denied or error while deleting the file.
                return false;

            }
            return true;
        }

        public static bool SecureEraseFilesRecursive(String root)
        {
            Queue<String> queue = new Queue<String>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                // the first node is dequeued and processed first, then it's children are processed level by level.
                String currentNode = queue.Dequeue();

                // get children directories of this node.
                IEnumerable<String> childDirectories = Directory.EnumerateDirectories(currentNode);

                // add all children is queue, they will be processed in this same way in this same place: 1st parent node, 2nd children nodes.
                foreach (String childNode in childDirectories)
                    queue.Enqueue(childNode);

                // get children files of this node.
                IEnumerable<String> childFiles = Directory.EnumerateFiles(currentNode);

                // now secure erase then delete all children files
                foreach (String childNode in childFiles)
                    SecureEraseFile(childNode, 1, true);
            }
            return true;
        }

        public static bool SecureDeleteDirectory(String root)
        {
            if (!SecureEraseFilesRecursive(root))
                return false;

            try
            {
                Directory.Delete(root, true);
            }
            catch 
            {
            }
            return true;

        }
        public static bool IsValidRegex(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern)) return false;

            try
            {
                Regex.Match("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

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
