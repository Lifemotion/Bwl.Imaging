using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace Bwl.Imaging
{

    public static class ImageTools
    {
        public static Bitmap Resize(Bitmap img, int W, int H, bool align4 = true, InterpolationMode interp = InterpolationMode.Bilinear)
        {
            if (align4)
            {
                W = W % 4 != 0 ? W + (4 - W % 4) : W;
            }
            var resized = new Bitmap(W, H, PixelFormat.Format24bppRgb);
            using (var gr = Graphics.FromImage(resized))
            {
                gr.SmoothingMode = SmoothingMode.None;
                gr.InterpolationMode = interp;
                gr.PixelOffsetMode = PixelOffsetMode.Half;
                gr.DrawImage(img, 0, 0, W, H);
            }
            return resized;
        }

        public static RectangleF RectangleToRectangleF(Rectangle rect, Size size)
        {
            float X = rect.X / (float)size.Width;
            float Y = rect.Y / (float)size.Height;
            float W = rect.Width / (float)size.Width;
            float H = rect.Height / (float)size.Height;
            return new RectangleF(X, Y, W, H);
        }

        public static Rectangle RectangleFToRectangle(RectangleF rectF, Size size)
        {
            int X = (int)Math.Floor((double)(rectF.X * size.Width));
            int Y = (int)Math.Floor((double)(rectF.Y * size.Height));
            int W = (int)Math.Floor((double)(rectF.Width * size.Width));
            int H = (int)Math.Floor((double)(rectF.Height * size.Height));
            return new Rectangle(X, Y, W, H);
        }

        public static PointF PointToPointF(Point point, Size size)
        {
            float X = point.X / (float)size.Width;
            float Y = point.Y / (float)size.Height;
            return new PointF(X, Y);
        }

        public static Point PointFToPoint(PointF pointF, Size size)
        {
            int X = (int)Math.Floor((double)(pointF.X * size.Width));
            int Y = (int)Math.Floor((double)(pointF.Y * size.Height));
            return new Point(X, Y);
        }

        public static RectangleF PointsToRectangleF(PointF[] points)
        {
            float left = points.Min(item => item.X);
            float right = points.Max(item => item.X);
            float top = points.Min(item => item.Y);
            float bottom = points.Max(item => item.Y);
            return RectangleF.FromLTRB(left, top, right, bottom);
        }

        public static Rectangle PointsToRectangle(Point[] points)
        {
            int left = points.Min(item => item.X);
            int right = points.Max(item => item.X);
            int top = points.Min(item => item.Y);
            int bottom = points.Max(item => item.Y);
            return Rectangle.FromLTRB(left, top, right, bottom);
        }

        public static Bitmap ConvertTo(this Image img, PixelFormat targetPixelFormat)
        {
            var bmp = new Bitmap(img.Width, img.Height, targetPixelFormat);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            }
            return bmp;
        }
    }
}