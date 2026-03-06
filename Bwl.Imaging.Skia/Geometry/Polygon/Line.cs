using SkiaSharp;
using System.Runtime.Serialization;

namespace Bwl.Imaging.Skia
{

    [DataContract()]
    public class Line : Polygon
    {

        [IgnoreDataMember]
        public SKPoint Point1
        {
            set
            {
                _points[0] = value;
            }
            get
            {
                return _points[0];
            }
        }

        [IgnoreDataMember]
        public SKPoint Point2
        {
            set
            {
                _points[1] = value;
            }
            get
            {
                return _points[1];
            }
        }

        public Line() : base(false, new SKPoint(), new SKPoint())
        {
        }

        public Line(float x1, float y1, float x2, float y2) : base(false, new SKPoint(x1, y1), new SKPoint(x2, y2))
        {
        }

        public Line(SKPoint p1, SKPoint p2) : base(false, p1, p2)
        {
        }

        public override string ToString()
        {
            return "Line: " + Point1.ToString() + " - " + Point2.ToString();
        }
    }
}