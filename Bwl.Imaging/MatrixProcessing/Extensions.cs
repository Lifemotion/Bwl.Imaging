using System;
using System.Drawing;

namespace Bwl.Imaging
{

    public static class Extensions
    {
        public static Point ToPoint(this PointF pointF)
        {
            return new Point((int)Math.Round(pointF.X), (int)Math.Round(pointF.Y));
        }

        public static PointF ToPointF(this Point point)
        {
            return new PointF(point.X, point.Y);
        }

        public static float Dist(this PointF @this, PointF arg)
        {
            return (float)Math.Sqrt(Math.Pow((double)(@this.X - arg.X), 2d) + Math.Pow((double)(@this.Y - arg.Y), 2d));
        }

        public static string ToString(this PointF @this)
        {
            return "X: " + @this.X.ToString() + " Y: " + @this.Y.ToString();
        }

        public static RectangleF ToPositiveSized(this RectangleF @this)
        {
            float left = Math.Min(@this.Left, @this.Right);
            float right = Math.Max(@this.Left, @this.Right);
            float top = Math.Min(@this.Top, @this.Bottom);
            float bottom = Math.Max(@this.Top, @this.Bottom);
            return RectangleF.FromLTRB(left, top, right, bottom);
        }

        public static Rectangle ToPositiveSized(this Rectangle @this)
        {
            int left = Math.Min(@this.Left, @this.Right);
            int right = Math.Max(@this.Left, @this.Right);
            int top = Math.Min(@this.Top, @this.Bottom);
            int bottom = Math.Max(@this.Top, @this.Bottom);
            return Rectangle.FromLTRB(left, top, right, bottom);
        }
    }
}