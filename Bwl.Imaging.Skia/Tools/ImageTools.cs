using System;
using System.Linq;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using SkiaSharp;

namespace Bwl.Imaging.Skia
{

    public static class ImageTools
    {
        public static SKBitmap Resize(SKBitmap bmp, int W, int H, bool align4 = true, SKSamplingOptions? options = null)
        {
            if (align4)
            {
                W = W % 4 != 0 ? W + (4 - (W % 4)) : W;
            }
            var resized = new SKBitmap(W, H, SKColorType.Bgra8888, SKAlphaType.Opaque);
            using var img = SKImage.FromBitmap(bmp);
            using var gr = new SKCanvas(resized);
            gr.Clear(SKColors.Transparent);
            if (options != null) gr.DrawImage(img, SKExtensions.SKRectFromXYWH(0, 0, W, H), options.Value);
            else gr.DrawImage(img, SKExtensions.SKRectFromXYWH(0, 0, W, H));
            return resized;
        }

        public static SKRect SKRectIToSKRect(SKRectI rect, SKSize size)
        {
            float X = rect.Left / (float)size.Width;
            float Y = rect.Top / (float)size.Height;
            float W = rect.Width / (float)size.Width;
            float H = rect.Height / (float)size.Height;
            return SKExtensions.SKRectFromXYWH(X, Y, W, H);
        }

        public static SKRectI SKRectFToSKRectI(SKRect rectF, SKSizeI size)
        {
            int X = (int)Math.Round(Math.Floor((double)(rectF.Left * size.Width)));
            int Y = (int)Math.Round(Math.Floor((double)(rectF.Top * size.Height)));
            int W = (int)Math.Round(Math.Floor((double)(rectF.Width * size.Width)));
            int H = (int)Math.Round(Math.Floor((double)(rectF.Height * size.Height)));
            return SKExtensions.SKRectIFromXYWH(X, Y, W, H);
        }

        public static SKRect PointsToRectangleF(SKPoint[] points)
        {
            float left = points.Min(item => item.X);
            float right = points.Max(item => item.X);
            float top = points.Min(item => item.Y);
            float bottom = points.Max(item => item.Y);
            return new SKRect(left, top, right, bottom);
        }

        public static SKRectI PointsToRectangle(SKPointI[] points)
        {
            int left = points.Min(item => item.X);
            int right = points.Max(item => item.X);
            int top = points.Min(item => item.Y);
            int bottom = points.Max(item => item.Y);
            return new SKRectI(left, top, right, bottom);
        }

        public static SKBitmap ConvertTo(this SKBitmap srcBmp, SKColorType colorType)
        {
            var alphaType = colorType == SKColorType.Gray8 ? SKAlphaType.Opaque : SKAlphaType.Premul;
            var bmp = new SKBitmap(srcBmp.Width, srcBmp.Height, colorType, alphaType);
            using var gr = new SKCanvas(bmp);
            gr.DrawBitmap(bmp, SKExtensions.SKRectFromXYWH(0, 0, bmp.Width, bmp.Height));
            return bmp;
        }
    }
}