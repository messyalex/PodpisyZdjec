using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PodpisyZdjec
{
    class Program
    {
        static void Main(string[] args)
        {
            Stamper stamper = new Stamper();
        }
    }

    class Stamper
    {
        private string desktopPath;
        private string sourceDir;
        private string destinationDir;

        public Stamper()
        {
            desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            Console.WriteLine("Source directory name in desktop: ");
            sourceDir = Console.ReadLine();
            Console.WriteLine("Destination directory name in desktop: ");
            destinationDir = Path.Combine(desktopPath, Console.ReadLine());
            System.IO.Directory.CreateDirectory(destinationDir);

            string[] files = Directory.GetFiles(Path.Combine(desktopPath, sourceDir), "*.jpg", SearchOption.TopDirectoryOnly);

            foreach (string file in files)
            {
                int lastDirectoryPos = file.LastIndexOf("\\") + 1;
                int extentionPos = file.IndexOf(".jpg", StringComparison.CurrentCultureIgnoreCase);
                string fileName = file.Substring(lastDirectoryPos, extentionPos - lastDirectoryPos);

                //string contents = getSourceString(i);

                //if (contents == null)
                //{
                //    Console.WriteLine(i.ToString() + " already exists");
                //    continue;
                //}

                //string base64 = extractImage(i, contents);

                //SaveImageFile(i, base64);

                Console.WriteLine("Reading " + fileName);

                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BitmapSource img = BitmapFrame.Create(fs);
                    BitmapMetadata md = (BitmapMetadata)img.Metadata;

                    string date = md.DateTaken;
                    string title = md.Title;
                    string subject = md.Subject;
                    string comment = md.Comment;

                    Console.WriteLine("Date: " + date);
                    Console.WriteLine("Title: " + title);
                    Console.WriteLine("Subject: " + subject);
                    //Console.WriteLine("Comment: " + comment);

                    Bitmap bitmap = GetBitmap(img);
                    using (var graphics = Graphics.FromImage(bitmap))
                    using (var font = new Font("Arial", 20, System.Drawing.FontStyle.Regular))
                    {
                        // Do what you want using the Graphics object here.
                        graphics.DrawString("Hello World!", font, Brushes.Red, 0, 0);

                        // Important part!
                        bitmap.Save(Path.Combine(destinationDir, fileName + ".jpg"));
                    }
                }
            }

            Console.ReadLine();
        }

        public static Bitmap GetBitmap(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(
              source.PixelWidth,
              source.PixelHeight,
              PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(
              new Rectangle(System.Drawing.Point.Empty, bmp.Size),
              ImageLockMode.WriteOnly,
              PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }
    }
}
