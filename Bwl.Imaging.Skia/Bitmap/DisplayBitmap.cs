using System;
using SkiaSharp;

namespace Bwl.Imaging.Skia
{

    public class DisplayBitmap : DisplayGraphics
    {
        protected SKBitmap _bitmap;

        public SKBitmap Bitmap
        {
            get
            {
                return _bitmap;
            }
        }

        public DisplayBitmap(SKBitmap bitmap) : base(new SKCanvas(bitmap), bitmap.Width, bitmap.Height)
        {
            _bitmap = bitmap;
        }

        public DisplayBitmap(int width, int height) : this(new SKBitmap(width, height))
        {
            if (width < 1)
                throw new ArgumentException("width must be >0");
            if (height < 1)
                throw new ArgumentException("height must be >0");
        }

        public void Resize(int width, int height)
        {
            lock (_syncRoot)
            {
                _bitmap = new SKBitmap(width, height);
                SetGraphics(new SKCanvas(_bitmap), width, height);
            }
        }
    }
}