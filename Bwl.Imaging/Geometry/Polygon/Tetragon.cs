using System.Drawing;
using System.Runtime.Serialization;
using System.Text;

namespace Bwl.Imaging
{

    public class Tetragon : Polygon
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

        [IgnoreDataMember]
        public PointF Point3
        {
            set
            {
                _points[2] = value;
            }
            get
            {
                return _points[2];
            }
        }

        [IgnoreDataMember]
        public PointF Point4
        {
            set
            {
                _points[3] = value;
            }
            get
            {
                return _points[3];
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(string.Format("Point1:{0}; ", Point1));
            sb.Append(string.Format("Point2:{0}; ", Point2));
            sb.Append(string.Format("Point3:{0}; ", Point3));
            sb.Append(string.Format("Point4:{0}; ", Point4));
            return sb.ToString();
        }

        public void SetRectangle(float left, float top, float right, float bottom)
        {
            Point1 = new PointF(left, top);
            Point2 = new PointF(right, top);
            Point3 = new PointF(right, bottom);
            Point4 = new PointF(left, bottom);
        }

        public void Expand(float offset)
        {
            Point1 = new PointF(Point1.X - offset, Point1.Y - offset);
            Point2 = new PointF(Point2.X + offset, Point2.Y - offset);
            Point3 = new PointF(Point3.X + offset, Point3.Y + offset);
            Point4 = new PointF(Point4.X - offset, Point4.Y + offset);
        }

        public Tetragon() : base(true, new[] { new PointF(), new PointF(), new PointF(), new PointF() })
        {
        }

        public Tetragon(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4) : base(true, new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3), new PointF(x4, y4))
        {
        }

        public Tetragon(PointF p1, PointF p2, PointF p3, PointF p4) : base(true, p1, p2, p3, p4)
        {
        }

        public Tetragon(float x1, float y1, float x2, float y2) : this()
        {
            SetRectangle(x1, y1, x2, y2);
        }

    }
}