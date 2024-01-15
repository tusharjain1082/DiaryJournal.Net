using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Markup.Primitives;
using System.Windows;

namespace DiaryJournal.Net
{
    public static class DirectoryInfoEx
    {
        public static void CopyToRecursive(this DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
                return;

            if (!target.Exists)
                target.Create();

            foreach (FileInfo f in source.GetFiles())
            {
                FileInfo newFile = new FileInfo(Path.Combine(target.FullName, f.Name));
                f.CopyTo(newFile.FullName, true);
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                diSourceSubDir.CopyToRecursive(nextTargetSubDir);
            }
        }
        public static bool CopyTo(this DirectoryInfo source, DirectoryInfo target, bool overwrite)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
                return false;

            try
            {
                if (!target.Exists)
                    Directory.CreateDirectory(target.FullName);
            } catch { return false; }

            foreach (FileInfo f in source.GetFiles())
            {
                try
                {
                    FileInfo newFile = new FileInfo(Path.Combine(target.FullName, f.Name));
                    f.CopyTo(newFile.FullName, overwrite);
                }
                catch { }
            }
            return true;
        }
    }

    public static class ControlHelper
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(this Control Target)
        {
            SendMessage(Target.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
        }

        public static void ResumeDrawing(this Control Target)
        {
            SendMessage(Target.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
            Target.Invalidate(true);
            Target.Update();
        }

        public static IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public static List<Control> GetAllChildControls(Control Root, Type? FilterType = null)
        {
            List<Control> AllChilds = new List<Control>();
            foreach (Control ctl in Root.Controls)
            {
                Type ctlType = ctl.GetType();

                if (FilterType != null)
                {
                    if (ctlType == FilterType)
                    {
                        AllChilds.Add(ctl);
                    }
                }
                else
                {
                    AllChilds.Add(ctl);
                }
                if (ctl.HasChildren)
                {
                    GetAllChildControls(ctl, FilterType);
                }
            }
            return AllChilds;
        }
    }

    public static class commonMethods
    {
        public static bool CopyFilesRecursively(string sourcePath, string targetPath, bool copy = true, bool overwrite = true)
        {
            //Now Create all of the directories
            try
            {
                foreach (string dirPath in Directory.EnumerateDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
                }

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.EnumerateFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, targetPath), overwrite);
                }
            }
            catch (Exception) 
            {
                return false;
            }

            if (!copy)
            {
                try
                {
                    Directory.Delete(sourcePath, true);
                }
                catch
                {
                }
            }
            return true;
        }

        public static Type GetListType<T>(this List<T> _)
        {
            return typeof(T);
        }

        public static Type GetEnumeratedType<T>(this IEnumerable<T> _)
        {
            return typeof(T);
        }

        public static List<ArraySegment<byte>> Split(byte[] arr, byte[] delimiter)
        {
            var result = new List<ArraySegment<byte>>();
            var segStart = 0;
            for (int i = 0, j = 0; i < arr.Length; i++)
            {
                if (arr[i] != delimiter[j])
                {
                    if (j == 0) continue;
                    j = 0;
                }

                if (arr[i] == delimiter[j])
                {
                    j++;
                }

                if (j == delimiter.Length)
                {
                    var segLen = (i + 1) - segStart - delimiter.Length;
                    if (segLen > 0) result.Add(new ArraySegment<byte>(arr, segStart, segLen));
                    segStart = i + 1;
                    j = 0;
                }
            }

            if (segStart < arr.Length)
            {
                result.Add(new ArraySegment<byte>(arr, segStart, arr.Length - segStart));
            }

            return result;
        }
        public static IEnumerable<FileInfo> EnumerateFiles(String path, EntryType entryType)
        {
            // first get the entry type and formats
            String ext = "";
            String extComplete = "";
            String extSearchPattern = "";
            entryMethods.getEntryTypeFormats(entryType, ref ext, ref extComplete, ref extSearchPattern);

            DirectoryInfo dir = new DirectoryInfo(path);
            //Date created latest first
            //var files = dir.EnumerateFiles().OrderByDescending(x => x.CreationTime);

            //Date created latest last
            IEnumerable<FileInfo> files = dir.EnumerateFiles(extSearchPattern);//.OrderBy(x => x.CreationTime);

            ////dir.EnumerateFiles() is the same as the ones below
            //var files3 = dir.EnumerateFiles("*");
            //var files4 = dir.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
            return files;
        }

        public static bool SecureEraseFile(String strPath, int iterations, bool deleteFile = false)
        {
            FileStream fs = null;
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            int sectorSize = 1048576; // 1 mb
            byte[] sector = new byte[sectorSize];

            // this method erases the file beyond any recovery. data is completely destroyed. this is for security reasons.

            try
            {
                for (int i = 0; i < iterations; i++)
                {
                    fs = new FileStream(strPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, sectorSize, FileOptions.RandomAccess);
                    fs.Seek(0, SeekOrigin.Begin);
                    long length = fs.Length;

                    long sectors = length / sectorSize;

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
                        // erase 3 times every sector. this overwrites beyond recovery.
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

        public static Font? StringToFont(string font)
        {
            if (font == "") return null;
            if (font.Length <= 0) return null;

            string[] parts = font.Split(':');
            if (parts.Length != 3)
                throw new ArgumentException("Not a valid font string", "font");

            Font loadedFont = new Font(parts[0], float.Parse(parts[1]), (System.Drawing.FontStyle)int.Parse(parts[2]));
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
