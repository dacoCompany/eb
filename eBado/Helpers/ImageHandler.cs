using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;


namespace Web.eBado.Helpers
{
    public class ImageHandler
    {
            /// <summary>
            /// Method to resize, convert and save the image.
            /// </summary>
            /// <param name="image">Bitmap image.</param>
            /// <param name="maxWidth">resize width.</param>
            /// <param name="maxHeight">resize height.</param>
            /// <param name="quality">quality setting value.</param>
            /// <param name="filePath">file path.</param>      
            public void Save(Bitmap image, int maxWidth, int maxHeight, int quality, string filePath)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;

                float ratioX = (float)maxWidth / (float)originalWidth;
                float ratioY = (float)maxHeight / (float)originalHeight;
                float ratio = Math.Min(ratioX, ratioY);

                int newWidth = (int)(originalWidth * ratio);
                int newHeight = (int)(originalHeight * ratio);

                Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                }

                ImageCodecInfo imageCodecInfo = this.GetEncoderInfo(ImageFormat.Jpeg);
                System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;

                EncoderParameters encoderParameters = new EncoderParameters(1);
                EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
                encoderParameters.Param[0] = encoderParameter;
                newImage.Save(filePath, imageCodecInfo, encoderParameters);
            }

            /// <summary>
            /// Method to get encoder infor for given image format.
            /// </summary>
            /// <param name="format">Image format</param>
            /// <returns>image codec info.</returns>
            private ImageCodecInfo GetEncoderInfo(ImageFormat format)
            {
                return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
            }
            public static Bitmap LoadImage(string path)
            {
                var ms = new MemoryStream(File.ReadAllBytes(path));
                GC.KeepAlive(ms);
                return (Bitmap)Image.FromStream(ms);
            }
            public static Size GetThumbnailSize(Image original)
            {
                const int maxPixels = 40;

                int originalWidth = original.Width;
                int originalHeight = original.Height;

                double factor;
                if (originalWidth > originalHeight)
                {
                    factor = (double)maxPixels / originalWidth;
                }
                else
                {
                    factor = (double)maxPixels / originalHeight;
                }

                return new Size((int)(originalWidth * factor), (int)(originalHeight * factor));
            }
        }
    }
