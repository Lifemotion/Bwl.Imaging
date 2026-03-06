using SkiaSharp;

namespace Bwl.Imaging.Skia;

public static class Extensions
{
    public static SKPointI ToPoint(this SKPoint pointF)
    {
        return new SKPointI((int)Math.Round(pointF.X), (int)Math.Round(pointF.Y));
    }

    public static SKPoint ToPointF(this SKPointI point)
    {
        return new SKPoint(point.X, point.Y);
    }

    public static float Dist(this SKPoint @this, SKPoint arg)
    {
        return (float)Math.Sqrt(Math.Pow((double)(@this.X - arg.X), 2d) + Math.Pow((double)(@this.Y - arg.Y), 2d));
    }

    public static string ToString(this SKPoint @this)
    {
        return "X: " + @this.X.ToString() + " Y: " + @this.Y.ToString();
    }

    public static SKRect ToPositiveSized(this SKRect @this)
    {
        float left = Math.Min(@this.Left, @this.Right);
        float right = Math.Max(@this.Left, @this.Right);
        float top = Math.Min(@this.Top, @this.Bottom);
        float bottom = Math.Max(@this.Top, @this.Bottom);
        return new SKRect(left, top, right, bottom);
    }

    public static SKRectI ToPositiveSized(this SKRectI @this)
    {
        int left = Math.Min(@this.Left, @this.Right);
        int right = Math.Max(@this.Left, @this.Right);
        int top = Math.Min(@this.Top, @this.Bottom);
        int bottom = Math.Max(@this.Top, @this.Bottom);
        return new SKRectI(left, top, right, bottom);
    }
}