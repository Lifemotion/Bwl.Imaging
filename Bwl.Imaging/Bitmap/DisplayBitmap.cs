using System;
using System.Drawing;

namespace Bwl.Imaging
{

    public class DisplayBitmap : DisplayGraphics
    {
        protected Bitmap _bitmap;

        public Bitmap Bitmap
        {
            get
            {
                return _bitmap;
            }
        }

        public DisplayBitmap(Bitmap bitmap) : base(Graphics.FromImage(bitmap), bitmap.Width, bitmap.Height)
        {
            _bitmap = bitmap;
        }

        public DisplayBitmap(int width, int height) : this(new Bitmap(width, height))
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
                _bitmap = new Bitmap(width, height);
                SetGraphics(Graphics.FromImage(_bitmap), width, height);
            }
        }
    }
}