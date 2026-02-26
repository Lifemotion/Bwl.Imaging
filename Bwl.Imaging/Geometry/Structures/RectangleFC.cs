using System;
using System.Drawing;

namespace Bwl.Imaging
{

    public class RectangleFC : ICloneable
    {

        public RectangleF RectangleF { get; set; }

        public object Clone()
        {
            return new RectangleFC(RectangleF);
        }

        public RectangleFC(RectangleF rect)
        {
            RectangleF = rect;
        }

        public RectangleFC(Rectangle rect)
        {
            RectangleF = rect;
        }

        public RectangleFC(float x1, float y1, float x2, float y2)
        {
            RectangleF = RectangleF.FromLTRB(x1, y1, x2, y2);
        }

        public RectangleFC(PointF location, SizeF size)
        {
            RectangleF = new RectangleF(location, size);
        }

        public RectangleFC()
        {

        }

        public override string ToString()
        {
            return "RectangleC: " + RectangleF.ToString();
        }

    }
}