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

                Console.WriteLine();
                Console.WriteLine("Reading: " + fileName);

                // jakość docelowych plików
                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                myEncoderParameters.Param[0] = myEncoderParameter;

                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var orientation = 1;
                    using (FileStream ofs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        Image currentPicture = Image.FromStream(ofs);

                        try
                        {
                            orientation = (int)currentPicture.GetPropertyItem(274).Value[0];
                        }
                        catch (Exception e)
                        {

                        }
                    }

                    BitmapSource img = BitmapFrame.Create(fs);
                    BitmapMetadata md = (BitmapMetadata)img.Metadata;

                    string date = md.DateTaken;
                    string title = md.Title;
                    string subject = md.Subject;
                    string comment = md.Comment;

                    //Console.WriteLine("Date: " + date);
                    //Console.WriteLine("Title: " + title);
                    //Console.WriteLine("Subject: " + subject);
                    int chars = 0;
                    if (title != null)
                        chars += title.Length;
                    if (subject != null)
                        chars += subject.Length;
                    Console.WriteLine("Chars: " + chars);

                    //Console.WriteLine("Comment: " + comment);

                    Bitmap bitmap = GetBitmap(img);
                    var graphics = Graphics.FromImage(bitmap);

                    int width = bitmap.Width;
                    int height = bitmap.Height;
                    Console.WriteLine("Width: " + width);
                    Console.WriteLine("Height: " + height);

                    switch (orientation)
                    {
                        case 1:
                            Console.WriteLine("// No rotation required.");
                            break;
                        case 2:
                            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case 3:
                            bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 4:
                            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                            break;
                        case 5:
                            bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
                            break;
                        case 6:
                            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case 7:
                            bitmap.RotateFlip(RotateFlipType.Rotate270FlipX);
                            break;
                        case 8:
                            bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                    }

                    if (orientation == 1)
                    {
                        int tempWidth = width;
                        width = height;
                        height = tempWidth;
                    }

                    int actualWidth = 1920 * width / 1080;

                    Bitmap wideBitmap = new Bitmap(actualWidth, width);
                    using (Graphics graph = Graphics.FromImage(wideBitmap))
                    {
                        Rectangle ImageSize = new Rectangle(0, 0, actualWidth, width);

                        graph.FillRectangle(Brushes.Black, ImageSize);

                        graph.DrawImage(bitmap, 0, 0);

                        int start = height;
                        if ((height * 2) > actualWidth)
                            start = (int)Math.Round(0.8f * height);

                        RectangleF textBox = new RectangleF(
                            start,
                            0,
                            actualWidth - start,
                            width);

                        graph.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 0)), textBox);

                        int fontSize = actualWidth / 22;

                        if (title != null)
                        {
                            Font titleFont = new Font("Arial", fontSize, System.Drawing.FontStyle.Bold);
                            RectangleF titleTextBox = new RectangleF(
                           start,
                            0,
                            actualWidth - start,
                            width);
                            using (StringFormat sf = new StringFormat())
                            {
                                sf.LineAlignment = StringAlignment.Near;
                                // Top/Left.
                                sf.Alignment = StringAlignment.Far;
                                graph.DrawString(title, titleFont, Brushes.White, titleTextBox, sf);
                            }
                        }

                        if (subject != null)
                        {
                            if (subject != title)
                            {
                                Font subjectFont = new Font("Arial", fontSize, System.Drawing.FontStyle.Regular);
                                RectangleF subjectTextBox = new RectangleF(
                                    start,
                                    width / 2,
                                    actualWidth - start,
                                    width / 2);
                                //http://csharphelper.com/blog/2014/07/align-text-drawn-by-drawstring-in-c/
                                using (StringFormat sf = new StringFormat())
                                {
                                    sf.LineAlignment = StringAlignment.Near;
                                    // Top/Left.
                                    sf.Alignment = StringAlignment.Far;
                                    graph.DrawString(subject, subjectFont, Brushes.White, subjectTextBox, sf);
                                }

                            }
                        }
                    }

                    switch (orientation)
                    {
                        case 1:
                            Console.WriteLine("// No rotation required.");
                            break;
                        case 2:
                            wideBitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case 3:
                            wideBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 4:
                            wideBitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                            break;
                        case 5:
                            wideBitmap.RotateFlip(RotateFlipType.Rotate270FlipX);
                            break;
                        case 6:
                            wideBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        case 7:
                            wideBitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
                            break;
                        case 8:
                            wideBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                    }

                    bitmap = wideBitmap;

                    // Important part!

                    string destinationFile = Path.Combine(destinationDir, fileName + ".jpg");
                    bitmap.Save(destinationFile, myImageCodecInfo, myEncoderParameters);
                }
            }

            foreach (string file in files)
            {
                int lastDirectoryPos = file.LastIndexOf("\\") + 1;
                int extentionPos = file.IndexOf(".jpg", StringComparison.CurrentCultureIgnoreCase);
                string fileName = file.Substring(lastDirectoryPos, extentionPos - lastDirectoryPos);

                string destinationFile = Path.Combine(destinationDir, fileName + ".jpg");
                CopyMetadata(file, destinationFile);
            }

            //Console.ReadLine();
        }

        private Bitmap GetBitmap(BitmapSource source)
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

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private void CopyMetadata(string sourceFile, string destinationFile)
        {
            BitmapCreateOptions createOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
            // Create backup of the file so you can read and write to different files 
            string tempFile = destinationFile + ".tmp";
            File.Copy(destinationFile, tempFile, true);
            // Open the source file 
            using (Stream sourceStream = File.Open(sourceFile, FileMode.Open, FileAccess.Read))
            {
                BitmapDecoder sourceDecoder = BitmapDecoder.Create(sourceStream, createOptions, BitmapCacheOption.None);
                // Check source is has valid frames 
                if (sourceDecoder.Frames[0] != null && sourceDecoder.Frames[0].Metadata != null)
                {
                    // Get a clone copy of the metadata 
                    BitmapMetadata sourceMetadata = sourceDecoder.Frames[0].Metadata.Clone() as BitmapMetadata;
                    // Open the temp file 
                    using (Stream tempStream = File.Open(tempFile, FileMode.Open, FileAccess.Read))
                    {
                        BitmapDecoder tempDecoder = BitmapDecoder.Create(tempStream, createOptions, BitmapCacheOption.None);
                        // Check temp file has valid frames 
                        if (tempDecoder.Frames[0] != null && tempDecoder.Frames[0].Metadata != null)
                        {
                            // Open the destination file 
                            using (Stream destinationStream = File.Open(destinationFile, FileMode.Open, FileAccess.ReadWrite))
                            {
                                // Create a new jpeg frame, replacing the destination metadata with the source 
                                BitmapFrame destinationFrame = BitmapFrame.Create(tempDecoder.Frames[0],
                                      tempDecoder.Frames[0].Thumbnail,
                                      sourceMetadata,
                                      tempDecoder.Frames[0].ColorContexts);
                                // Save the file 
                                JpegBitmapEncoder destinationEncoder = new JpegBitmapEncoder();
                                destinationEncoder.Frames.Add(destinationFrame);
                                destinationEncoder.Save(destinationStream);
                            }
                        }
                    }
                }
            }
            // Delete the temp file 
            File.Delete(tempFile);
        }

        private int GetLinesCount(string sText, Font objFont, int iMaxWidth)
        {
            StringFormat sfFmt = new StringFormat(StringFormatFlags.LineLimit);
            Graphics g = Graphics.FromImage(new Bitmap(1, 1));
            int iHeight = (int)g.MeasureString(sText, objFont, iMaxWidth, sfFmt).Height;
            int iOneLineHeight = (int)g.MeasureString("Z", objFont, iMaxWidth, sfFmt).Height;
            int iNumLines = (int)(iHeight / iOneLineHeight);

            return iNumLines;
        }
    }
}
