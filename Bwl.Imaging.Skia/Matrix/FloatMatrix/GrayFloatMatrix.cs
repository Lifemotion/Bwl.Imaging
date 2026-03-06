namespace Bwl.Imaging.Skia;

public class GrayFloatMatrix : CommonFloatMatrix
{

    public GrayFloatMatrix(int width, int height) : base(1, width, height)
    {
    }

    public GrayFloatMatrix(double[] matrix, int width, int height) : base(new[] { matrix }, width, height)
    {
    }

    public GrayFloatMatrix(int[] matrix, int width, int height) : base(new[] { matrix }, width, height)
    {
    }

    public GrayFloatMatrix(double[] matrix, int width, int height, double multiplier) : base(new[] { matrix }, width, height, multiplier)
    {
    }

    public GrayFloatMatrix(int[] matrix, int width, int height, double multiplier) : base(new[] { matrix }, width, height, multiplier)
    {
    }

    public double GetGrayPixel(int x, int y)
    {
        return _matrices[0][x + y * Width];
    }
    public void SetGrayPixel(int x, int y, double value)
    {
        _matrices[0][x + y * Width] = value;
    }

    public double[] Gray
    {
        get
        {
            return _matrices[0];
        }
    }

    public new GrayFloatMatrix Clone()
    {
        return new GrayFloatMatrix(CloneMatrix(Gray), Width, Height);
    }

    public RGBFloatMatrix ToRGBFloatMatrix()
    {
        return new RGBFloatMatrix(Gray, Gray, Gray, Width, Height);
    }

    public new GrayFloatMatrix ResizeTwo()
    {
        var resized = base.ResizeTwo();
        return new GrayFloatMatrix(resized.GetMatrix(0), resized.Width, resized.Height);
    }

    public new GrayFloatMatrix ResizeHalf()
    {
        var resized = base.ResizeHalf();
        return new GrayFloatMatrix(resized.GetMatrix(0), resized.Width, resized.Height);
    }
}