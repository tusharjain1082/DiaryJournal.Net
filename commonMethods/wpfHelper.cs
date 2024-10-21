using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Automation.Peers;
using System.Windows.Media.Media3D;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace commonMethods
{
    public static class WpfHelper
    {
        public static List<DependencyObject> FindAllChildren(this DependencyObject dpo, Predicate<DependencyObject> predicate)
        {
            //Example for your case:
            //var children = dpObject.FindAllChildren(child => child is TextBox);

                    var results = new List<DependencyObject>();
            if (predicate == null)
                return results;


            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dpo); i++)
            {
                var child = VisualTreeHelper.GetChild(dpo, i);
                if (predicate(child))
                    results.Add(child);

                var subChildren = child.FindAllChildren(predicate);
                results.AddRange(subChildren);
            }
            return results;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChildren<T>(ithChild)) yield return childOfChild;
            }
        }

        public static IEnumerable<T> FindLogicalChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                foreach (object rawChild in LogicalTreeHelper.GetChildren(depObj))
                {
                    if (rawChild is DependencyObject)
                    {
                        DependencyObject child = (DependencyObject)rawChild;
                        if (child is T)
                        {
                            yield return (T)child;
                        }

                        foreach (T childOfChild in FindLogicalChildren<T>(child))
                        {
                            yield return childOfChild;
                        }
                    }
                }
            }
        }
    
        public static bool reloadImageOriginalQuality(ref System.Windows.Controls.Image src)
        {
            if (src == null) return false;
            if (!File.Exists((String)src.Tag)) return false;
            using (FileStream fs = new FileStream((String)src.Tag, FileMode.Open, FileAccess.Read))
            {
                fs.Position = 0;

                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(fs, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None));
                MemoryStream ms = new MemoryStream();
                encoder.Save(ms);
                ms.Position = 0;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.None;  
                bitmap.StreamSource = ms;
                //bitmap.DecodePixelHeight = (int)src.Source.Height * 3;
                //bitmap.DecodePixelWidth = (int)src.Source.Width * 3;
                //bitmap.UriSource = new Uri((String)src.Tag);
                bitmap.EndInit();
                bitmap.Freeze();

                System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                image.BeginInit();
                image.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                image.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                image.Stretch = Stretch.Uniform;
                image.Width = src.Width;
                image.Height = src.Height;
                image.SnapsToDevicePixels = true;
                image.UseLayoutRounding = true;
                image.Source = bitmap;
                image.Tag = src.Tag;

                image.EndInit();
                image.UpdateLayout();
                DependencyObject parent = src.Parent;

                if (parent is BlockUIContainer)
                {
                    ((BlockUIContainer)parent).BeginInit();
                    ((BlockUIContainer)parent).Child = image;
                    ((BlockUIContainer)parent).EndInit();

                }
                else if (parent is InlineUIContainer)
                {
                    ((InlineUIContainer)parent).BeginInit();// image;
                    ((InlineUIContainer)parent).Child = image;
                    ((InlineUIContainer)parent).EndInit();// image;

                }
            }
            return true;
        }


    }
}
