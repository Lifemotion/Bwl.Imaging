
using System.Drawing;

namespace Bwl.Imaging
{

    public class TextObject
    {

        public PointF Point1 { get; set; }
        public string Text { get; set; } = "";
        public float Size { get; set; } = 0.03f;

        public TextObject()
        {
        }

        public TextObject(PointF point1, string text, float size)
        {
            Text = text;
            Point1 = point1;
            Size = size;
        }

        public TextObject(float x1, float y1, string text, float size)
        {
            Point1 = new PointF(x1, y1);
            Text = text;
            Size = size;
        }
    }
}