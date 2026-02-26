
namespace Bwl.Imaging
{
    public class GrayMatrix : CommonMatrix
    {

        public GrayMatrix(int width, int height) : base(1, width, height)
        {
        }

        public GrayMatrix(int[] matrix, int width, int height) : base(new[] { matrix }, width, height)
        {
        }

        public GrayMatrix(double[] matrix, int width, int height) : base(new[] { matrix }, width, height)
        {
        }

        public GrayMatrix(int[] matrix, int width, int height, double multiplier) : base(new[] { matrix }, width, height, multiplier)
        {
        }

        public GrayMatrix(double[] matrix, int width, int height, double multiplier) : base(new[] { matrix }, width, height, multiplier)
        {
        }

        public int GetGrayPixel(int x, int y)
        {
            return _matrices[0][x + y * Width];
        }
        public void SetGrayPixel(int x, int y, int value)
        {
            _matrices[0][x + y * Width] = value;
        }

        public int[] Gray
        {
            get
            {
                return _matrices[0];
            }
        }

        public new GrayMatrix Clone()
        {
            return new GrayMatrix(CloneMatrix(Gray), Width, Height);
        }

        public RGBMatrix ToRGBMatrix()
        {
            return new RGBMatrix(CloneMatrix(Gray), CloneMatrix(Gray), CloneMatrix(Gray), Width, Height);
        }

        public new GrayMatrix ResizeTwo()
        {
            var resized = base.ResizeTwo();
            return new GrayMatrix(resized.GetMatrix(0), resized.Width, resized.Height);
        }

        public new GrayMatrix ResizeHalf()
        {
            var resized = base.ResizeHalf();
            return new GrayMatrix(resized.GetMatrix(0), resized.Width, resized.Height);
        }
    }
}