using SkiaSharp;
using System.Runtime.Serialization;

namespace Bwl.Imaging.Skia;

public class SKPointC : ICloneable
{

    private SKPoint _pointF;
    public SKPoint PointF
    {
        get => _pointF;
        set => _pointF = value;
    }

    [IgnoreDataMember]
    public float X
    {
        get => _pointF.X;
        set => _pointF.X = value;
    }
    [IgnoreDataMember]
    public float Y
    {
        get => _pointF.Y;
        set => _pointF.Y = value;
    }

    public SKPointC(SKPoint p)
    {
        _pointF = p;
    }

    public SKPointC(SKPointI p)
    {
        _pointF = p;
    }

    public SKPointC(SKPointC p)
    {
        _pointF = new SKPoint(p.X, p.Y);
    }

    public SKPointC(float x, float y)
    {
        _pointF = new SKPoint(x, y);
    }

    public SKPointC()
    {

    }

    public void CopyFrom(SKPoint point)
    {
        _pointF = point;
    }

    public void CopyFrom(SKPointC point)
    {
        _pointF = new SKPoint(point.X, point.Y);
    }

    public object Clone()
    {
        return new SKPointC(X, Y);
    }

    public SKPoint ToSKPoint()
    {
        return new SKPoint(X, Y);
    }

    public SKPointI ToSKPointI()
    {
        return new SKPointI((int)Math.Round(X), (int)Math.Round(Y));
    }


}