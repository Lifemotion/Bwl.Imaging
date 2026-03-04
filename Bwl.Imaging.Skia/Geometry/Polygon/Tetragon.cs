
using SkiaSharp;
using System.Runtime.Serialization;
using System.Text;

namespace Bwl.Imaging.Skia
{

    public class Tetragon : Polygon
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

        [IgnoreDataMember]
        public SKPoint Point3
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
        public SKPoint Point4
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
            Point1 = new SKPoint(left, top);
            Point2 = new SKPoint(right, top);
            Point3 = new SKPoint(right, bottom);
            Point4 = new SKPoint(left, bottom);
        }

        public void Expand(float offset)
        {
            Point1 = new SKPoint(Point1.X - offset, Point1.Y - offset);
            Point2 = new SKPoint(Point2.X + offset, Point2.Y - offset);
            Point3 = new SKPoint(Point3.X + offset, Point3.Y + offset);
            Point4 = new SKPoint(Point4.X - offset, Point4.Y + offset);
        }

        public Tetragon() : base(true, new[] { new SKPoint(), new SKPoint(), new SKPoint(), new SKPoint() })
        {
        }

        public Tetragon(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4) : base(true, new SKPoint(x1, y1), new SKPoint(x2, y2), new SKPoint(x3, y3), new SKPoint(x4, y4))
        {
        }

        public Tetragon(SKPoint p1, SKPoint p2, SKPoint p3, SKPoint p4) : base(true, p1, p2, p3, p4)
        {
        }

        public Tetragon(float x1, float y1, float x2, float y2) : this()
        {
            SetRectangle(x1, y1, x2, y2);
        }

    }
}