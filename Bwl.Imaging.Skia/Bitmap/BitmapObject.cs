using SkiaSharp;

namespace Bwl.Imaging.Skia
{

    public class BitmapObject
    {
        public SKRect SKRect { get; set; }
        public SKBitmap SKBitmap { get; set; }

        public BitmapObject()
        {
        }

        public BitmapObject(SKBitmap bitmap, SKRect rectangle)
        {
            SKRect = rectangle;
            SKBitmap = bitmap;
        }
        public BitmapObject(SKBitmap bitmap, float x1, float y1, float x2, float y2)
        {
            SKRect = new SKRect(x1, y1, x2, y2).ToPositiveSized();
            SKBitmap = bitmap;
        }
    }
}