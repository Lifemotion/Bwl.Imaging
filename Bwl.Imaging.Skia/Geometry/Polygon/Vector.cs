using SkiaSharp;
using System.Runtime.Serialization;

namespace Bwl.Imaging.Skia;

public class Vector : Line
{

    [IgnoreDataMember]
    public SKPoint PointFrom
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
    public SKPoint PointTo
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
    public SKPoint VectorValue
    {
        set
        {
            _points[1].X = _points[0].X + value.X;
            _points[1].Y = _points[0].Y + value.Y;
        }
        get
        {
            return new SKPoint(_points[1].X - _points[0].X, _points[1].Y - _points[0].Y);
        }
    }

    public Vector() : base()
    {
    }

    public Vector(float x1, float y1, float x2, float y2) : base(x1, y1, x2, y2)
    {
    }

    public Vector(SKPoint p1, SKPoint p2) : base(p1, p2)
    {
    }

    public override string ToString()
    {
        return "Vector: " + Point1.ToString() + " -> " + Point2.ToString();
    }
}