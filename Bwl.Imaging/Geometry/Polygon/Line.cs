using System.Drawing;
using System.Runtime.Serialization;

namespace Bwl.Imaging
{

    [DataContract()]
    public class Line : Polygon
    {

        [IgnoreDataMember]
        public PointF Point1
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
        public PointF Point2
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

        public Line() : base(false, new PointF(), new PointF())
        {
        }

        public Line(float x1, float y1, float x2, float y2) : base(false, new PointF(x1, y1), new PointF(x2, y2))
        {
        }

        public Line(PointF p1, PointF p2) : base(false, p1, p2)
        {
        }

        public override string ToString()
        {
            return "Line: " + Point1.ToString() + " - " + Point2.ToString();
        }
    }
}