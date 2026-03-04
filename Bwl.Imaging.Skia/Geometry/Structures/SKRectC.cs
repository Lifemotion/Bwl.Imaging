using SkiaSharp;

namespace Bwl.Imaging.Skia
{

    public class SKRectC : ICloneable
    {

        public SKRect SKRect { get; set; }

        public object Clone()
        {
            return new SKRectC(SKRect);
        }

        public SKRectC(SKRect rect)
        {
            SKRect = rect;
        }

        public SKRectC(SKRectI rect)
        {
            SKRect = rect;
        }

        public SKRectC(float x1, float y1, float x2, float y2)
        {
            SKRect = new SKRect(x1, y1, x2, y2);
        }

        public SKRectC(SKPoint location, SKSize size)
        {
            SKRect = SKExtensions.SKRectFromPointSize(location, size);
        }

        public SKRectC()
        {

        }

        public override string ToString()
        {
            return "SKRectC: " + SKRect.ToString();
        }

    }
}