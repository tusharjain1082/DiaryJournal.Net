using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Net;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Drawing.Drawing2D;

namespace commonMethods
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
        // auto create init temporary path inside app's path
        public static String myTempPath = "";
        public static String myBackupPath = "";
        public static String TempDirName = "tmp";
        public static String BackupDirName = "bck";
        public static DirectoryInfo tmpDirConfig = null;
        public static DirectoryInfo BackupDirConfig = null;
        public static bool autoCreateInitLocalPaths()
        {
            myTempPath = Path.Combine(Application.StartupPath, TempDirName);
            tmpDirConfig = Directory.CreateDirectory(myTempPath);
            if (tmpDirConfig == null) return false;

            myBackupPath = Path.Combine(Application.StartupPath, BackupDirName);
            BackupDirConfig = Directory.CreateDirectory(myBackupPath);
            if (BackupDirConfig == null) return false;

            // done
            return true;
        }
        // auto clear paths
        public static bool autoClearLocalPaths(bool tmppath, bool backuppath)
        {
            if (tmppath)
            {
                try
                {
                    Directory.Delete(myTempPath, true);
                    Directory.CreateDirectory(myTempPath);
                }
                catch { }
            }
            if (backuppath)
            {
                try
                {
                    Directory.Delete(myBackupPath, true);
                    Directory.CreateDirectory(myBackupPath);

                }
                catch { }
            }

            // done
            return true;
        }
        // allocate return new tmp file name
        public static String autoCreateInitTmpFile()
        {
            String tmpName = Guid.NewGuid().ToString();
            return Path.Combine(myTempPath, tmpName);

        }
        public static String getFormattedEntryFileName(String title, DateTime dateTime)
        {
            return String.Format("{0}--{1}", title, dateTime.ToString("dddd-dd-MMMM-yyyy-HH-mm-ss"));// "yyyy -MM-dd-HH-mm-ss-fff"),
        }

        // allocate write new backup entry file in backup dir
        public static bool autoCreateBackupEntryFile(String entryPath, String entryConfigPath, DateTime dateTime = default(DateTime))
         {
            if (entryPath == "") return false;
            if (entryConfigPath == "") return false;

            // copy entry
            FileInfo info = new FileInfo(entryPath);
            String filename = info.Name;
            filename = getFormattedEntryFileName(filename, dateTime);
            String finalPath = Path.Combine(myBackupPath, filename);
            try
            {
                File.Copy(entryPath, finalPath, true);
            }
            catch { }

            // copy entry config
            info = new FileInfo(entryConfigPath);
            filename = info.Name;
            filename = getFormattedEntryFileName(filename, dateTime);
            finalPath = Path.Combine(myBackupPath, filename);
            try
            {
                File.Copy(entryConfigPath, finalPath, true);
            }
            catch { }

            return true;
        }
        // allocate write new backup entry file in backup dir
        public static bool autoCreateBackupEntryFileSFDB(String entryPath, String entryConfigPath, String rtfData, String cfgData, DateTime dateTime = default(DateTime))
        {
            if (entryPath == "") return false;
            if (entryConfigPath == "") return false;

            // copy entry
            entryPath = getFormattedEntryFileName(entryPath, dateTime);
            String finalPath = Path.Combine(myBackupPath, entryPath);
            try
            {
                File.WriteAllText(finalPath, rtfData);
            }
            catch { }

            // copy entry config
            entryConfigPath = getFormattedEntryFileName(entryConfigPath, dateTime);
            finalPath = Path.Combine(myBackupPath, entryConfigPath);
            try
            {
                File.WriteAllText(finalPath, cfgData);
            }
            catch { }

            return true;
        }

        /*
        // allocate write new backup entry file in backup dir
        public static bool autoCreateBackupEntryFile(String rtf, String initialFileName, DateTime dateTime = default(DateTime))
        {
            if (dateTime == default(DateTime)) dateTime = DateTime.Now;
            String filename = getFormattedEntryFileName(initialFileName, dateTime);
            String finalPath = Path.Combine(myBackupPath, filename);
            File.WriteAllText(finalPath, rtf);
            return true;
        }
        */


        public static bool SaveToPngWpf(System.Windows.Controls.Image image, Stream s)
        {
            if (s == null) return false;
            var encoder = new PngBitmapEncoder();
            return SaveUsingEncoderWpf(image, s, encoder);
        }

        public static bool SaveUsingEncoderWpf(System.Windows.Controls.Image image, Stream s, BitmapEncoder encoder)
        {
            if (s == null) return false;
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
            //using (FileStream stream = new FileStream(fileName, FileMode.Create))
            encoder.Save(s);
            return true;
        }
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }

        public static Bitmap? rescaleBitmapWithDpi(Bitmap bitmap, int width, int height, int dpi)
        {
            if (bitmap == null) return null;

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Position = 0;
                bitmap.Save(ms, bitmap.RawFormat);
                ms.Position = 0;
                var brush = new SolidBrush(Color.White);

                Image rawImage = Image.FromStream(ms);

                Bitmap newBitmap = new Bitmap(rawImage);
                newBitmap.SetResolution(dpi, dpi);
                rawImage = newBitmap;

                float scale = Math.Min((float)width / rawImage.Width, (float)height / rawImage.Height);
                var scaleWidth = (int)(rawImage.Width * scale);
                var scaleHeight = (int)(rawImage.Height * scale);
                var scaledBitmap = new Bitmap(scaleWidth, scaleHeight);

                Graphics graph = Graphics.FromImage(scaledBitmap);
                graph.InterpolationMode = InterpolationMode.High;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
                graph.DrawImage(rawImage, new Rectangle(0, 0, scaleWidth, scaleHeight));

                ms.Position = 0;
                scaledBitmap.Save(ms, ImageFormat.Bmp);

                return (Bitmap)scaledBitmap;
            }
        }
        public static async Task<Bitmap?> DownloadImageAsync(string imageUrl, string filename, bool rescale = false, int width = 0, int height = 0)
        {
            Bitmap? bitmap = null;
            try
            {
                // webclient is obsolete as informed by Microsoft.
                //WebClient client = new WebClient();
                //Stream stream = client.OpenRead(imageUrl);

                //                var stream = await httpClient.GetStreamAsync(imageUrl);
                //var stream = await httpClient.GetStreamAsync(imageUrl)
                //           .ConfigureAwait(false);

                var httpClient = new HttpClient();
                MemoryStream ms = new MemoryStream();
                ms.Position = 0;
                var stream = await httpClient.GetStreamAsync(imageUrl).ConfigureAwait(false);
                await stream.CopyToAsync(ms);
                ms.Position = 0;
                bitmap = new Bitmap(ms);
                if (bitmap == null) return null;
                ms.Position = 0;
                //client.DownloadFile(imageUrl, filename,);

                // rescale resolution
                //bitmap = rescaleBitmapWithDpi(bitmap, bitmap.Width * 2, bitmap.Height * 2, 600);

                // resize if required
                if (rescale)
                {
                    bitmap = ResizeImage(bitmap, ((width == 0) ? bitmap.Width : width), ((height == 0) ? bitmap.Height : height)); 

                }
                if (filename != "")                {
                    //bitmap.Save(filename);

                    using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite))
                    {
                        //stream.Seek(0, SeekOrigin.Begin);
                        //fs.Seek(0, SeekOrigin.Begin);
                        ms.CopyTo(fs);
                        ms.Flush();
                        //stream.CopyTo(fs);
                        //stream.Flush();
                        //fs.Position = 0;

                        //ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        //ImageCodecInfo pngEncoder = GetEncoder(ImageFormat.Png);
                        //ImageCodecInfo bmpEncoder = GetEncoder(ImageFormat.Bmp);
                        //System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                        //EncoderParameters myEncoderParameters = new EncoderParameters(1);
                        //EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                        //myEncoderParameters.Param[0] = myEncoderParameter;
                        //bitmap.Save(fs, bmpEncoder, myEncoderParameters);
                        fs.Flush();
                        //fs.Position = 0;
                        //bitmap = new Bitmap(fs);
                        fs.Close();
                    }
                }
                //client.Dispose();
                //client = new WebClient();
                //stream = client.OpenRead(imageUrl);
                stream.Close();
                stream.Dispose();
                httpClient.Dispose();
                //client.Dispose();
            }
            catch(Exception ex)
            {
                return null;
            }
            return bitmap;
        }
        public static bool CheckForInternetConnection(int timeoutMs = 10000, string url = null)
        {
            try
            {
                url ??= CultureInfo.InstalledUICulture switch
                {
                    { Name: var n } when n.StartsWith("fa") => // Iran
                        "http://www.aparat.com",
                    { Name: var n } when n.StartsWith("zh") => // China
                        "http://www.baidu.com",
                    _ =>
                        "http://www.gstatic.com/generate_204",
                };

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using (var response = (HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<bool> SendPing(string hostNameOrAddress)
        {
            using (var ping = new Ping())
            {
                try
                {
                    PingReply result = await ping.SendPingAsync(hostNameOrAddress);
                    return result.Status == IPStatus.Success;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        public static String convertToString(this Enum eff)
        {
            return Enum.GetName(eff.GetType(), eff);
        }

        public static EnumType convertToEnum<EnumType>(this String enumValue)
        {
            return (EnumType)Enum.Parse(typeof(EnumType), enumValue);
        }

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
