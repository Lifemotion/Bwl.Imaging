

using SkiaSharp;

namespace Bwl.Imaging.Skia
{

    public class RGBMatrix : CommonMatrix
    {

        public RGBMatrix(int width, int height) : base(3, width, height)
        {
        }

        public RGBMatrix(int[] red, int[] green, int[] blue, int width, int height) : base(new[] { red, green, blue }, width, height)
        {
        }

        public RGBMatrix(double[] red, double[] green, double[] blue, int width, int height) : base(new[] { red, green, blue }, width, height)
        {
        }

        public RGBMatrix(int[] red, int[] green, int[] blue, int width, int height, double multiplier) : base(new[] { red, green, blue }, width, height, multiplier)
        {
        }

        public RGBMatrix(double[] red, double[] green, double[] blue, int width, int height, double multiplier) : base(new[] { red, green, blue }, width, height, multiplier)
        {
        }

        public RGB GetRGBPixel(int x, int y)
        {
            int r = _matrices[0][x + y * Width];
            int g = _matrices[1][x + y * Width];
            int b = _matrices[2][x + y * Width];
            return new RGB(r, g, b);
        }
        public void SetRGBPixel(int x, int y, RGB value)
        {
            _matrices[0][x + y * Width] = value.R;
            _matrices[1][x + y * Width] = value.G;
            _matrices[2][x + y * Width] = value.B;
        }

        // high cpu overhead!
        public HSV GetHSVPixel(int x, int y)
        {
            int r = _matrices[0][x + y * Width];
            int g = _matrices[1][x + y * Width];
            int b = _matrices[2][x + y * Width];
            return new RGB(r, g, b).ToHSV();
        }
        public void SetHSVPixel(int x, int y, HSV value)
        {
            var rgb = value.ToRGB();
            _matrices[0][x + y * Width] = rgb.R;
            _matrices[1][x + y * Width] = rgb.G;
            _matrices[2][x + y * Width] = rgb.B;
        }

        public SKColor GetColorPixel(int x, int y)
        {
            int r = _matrices[0][x + y * Width];
            int g = _matrices[1][x + y * Width];
            int b = _matrices[2][x + y * Width];
            return new SKColor((byte)r, (byte)g, (byte)b);
        }
        public void SetColorPixel(int x, int y, SKColor value)
        {
            _matrices[0][x + y * Width] = value.Red;
            _matrices[1][x + y * Width] = value.Green;
            _matrices[2][x + y * Width] = value.Blue;
        }

        public int GetRedPixel(int x, int y)
        {
            return _matrices[0][x + y * Width];
        }
        public void SetRedPixel(int x, int y, int value)
        {
            _matrices[0][x + y * Width] = value;
        }

        public int GetGreenPixel(int x, int y)
        {
            return _matrices[1][x + y * Width];
        }
        public void SetGreenPixel(int x, int y, int value)
        {
            _matrices[1][x + y * Width] = value;
        }

        public int GetBluePixel(int x, int y)
        {
            return _matrices[2][x + y * Width];
        }
        public void SetBluePixel(int x, int y, int value)
        {
            _matrices[2][x + y * Width] = value;
        }

        public int[] Red
        {
            get
            {
                return _matrices[0];
            }
        }

        public int[] Green
        {
            get
            {
                return _matrices[1];
            }
        }

        public int[] Blue
        {
            get
            {
                return _matrices[2];
            }
        }

        public new RGBMatrix Clone()
        {
            return new RGBMatrix(CloneMatrix(Red), CloneMatrix(Green), CloneMatrix(Blue), Width, Height);
        }

        public GrayMatrix ToGrayMatrix()
        {
            var gray = new int[(Width * Height)];
            for (int i = 0, loopTo = Width * Height - 1; i <= loopTo; i++)
                // Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601 (http://inst.eecs.berkeley.edu/~cs150/Documents/ITU601.PDF)
                gray[i] = ImagingMath.Limit(Red[i] * 0.299d + Green[i] * 0.587d + Blue[i] * 0.114d);
            return new GrayMatrix(gray, Width, Height);
        }

        public new RGBMatrix ResizeTwo()
        {
            var resized = base.ResizeTwo();
            return new RGBMatrix(resized.GetMatrix(0), resized.GetMatrix(1), resized.GetMatrix(2), resized.Width, resized.Height);
        }

        public new RGBMatrix ResizeHalf()
        {
            var resized = base.ResizeHalf();
            return new RGBMatrix(resized.GetMatrix(0), resized.GetMatrix(1), resized.GetMatrix(2), resized.Width, resized.Height);
        }
    }
}