
namespace Bwl.Imaging
{
    public class RGBFloatMatrix : CommonFloatMatrix
    {

        public RGBFloatMatrix(int width, int height) : base(3, width, height)
        {
        }

        public RGBFloatMatrix(double[] red, double[] green, double[] blue, int width, int height) : base(new[] { red, green, blue }, width, height)
        {
        }

        public RGBFloatMatrix(int[] red, int[] green, int[] blue, int width, int height) : base(new[] { red, green, blue }, width, height)
        {
        }

        public RGBFloatMatrix(double[] red, double[] green, double[] blue, int width, int height, double multiplier) : base(new[] { red, green, blue }, width, height, multiplier)
        {
        }

        public RGBFloatMatrix(int[] red, int[] green, int[] blue, int width, int height, double multiplier) : base(new[] { red, green, blue }, width, height, multiplier)
        {
        }

        public double GetRedPixel(int x, int y)
        {
            return _matrices[0][x + y * Width];
        }
        public void SetRedPixel(int x, int y, double value)
        {
            _matrices[0][x + y * Width] = value;
        }

        public double GetGreenPixel(int x, int y)
        {
            return _matrices[1][x + y * Width];
        }
        public void SetGreenPixel(int x, int y, double value)
        {
            _matrices[1][x + y * Width] = value;
        }

        public double GetBluePixel(int x, int y)
        {
            return _matrices[2][x + y * Width];
        }
        public void SetBluePixel(int x, int y, double value)
        {
            _matrices[2][x + y * Width] = value;
        }

        public double[] Red
        {
            get
            {
                return _matrices[0];
            }
        }

        public double[] Green
        {
            get
            {
                return _matrices[1];
            }
        }

        public double[] Blue
        {
            get
            {
                return _matrices[2];
            }
        }

        public new RGBFloatMatrix Clone()
        {
            return new RGBFloatMatrix(CloneMatrix(Red), CloneMatrix(Green), CloneMatrix(Blue), Width, Height);
        }

        public GrayFloatMatrix ToGrayMatrix()
        {
            var gray = new double[(Width * Height)];
            for (int i = 0, loopTo = Width * Height - 1; i <= loopTo; i++)
                // Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601 (http://inst.eecs.berkeley.edu/~cs150/Documents/ITU601.PDF)
                gray[i] = Red[i] * 0.299d + Green[i] * 0.587d + Blue[i] * 0.114d;
            return new GrayFloatMatrix(gray, Width, Height);
        }

        public new RGBFloatMatrix ResizeTwo()
        {
            var resized = base.ResizeTwo();
            return new RGBFloatMatrix(resized.GetMatrix(0), resized.GetMatrix(1), resized.GetMatrix(2), resized.Width, resized.Height);
        }

        public new RGBFloatMatrix ResizeHalf()
        {
            var resized = base.ResizeHalf();
            return new RGBFloatMatrix(resized.GetMatrix(0), resized.GetMatrix(1), resized.GetMatrix(2), resized.Width, resized.Height);
        }
    }
}