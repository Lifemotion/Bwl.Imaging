using System.Drawing;

namespace Bwl.Imaging
{

    public class BitmapObject
    {
        public RectangleF RectangleF { get; set; }
        public Bitmap Bitmap { get; set; }

        public BitmapObject()
        {
        }

        public BitmapObject(Bitmap bitmap, RectangleF rectangle)
        {
            RectangleF = rectangle;
            Bitmap = bitmap;
        }
        public BitmapObject(Bitmap bitmap, float x1, float y1, float x2, float y2)
        {
            RectangleF = RectangleF.FromLTRB(x1, y1, x2, y2).ToPositiveSized();
            Bitmap = bitmap;
        }
    }
}