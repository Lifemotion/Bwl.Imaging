using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace Bwl.Imaging
{
    public class PointC : ICloneable
    {

        private PointF _pointF;
        public PointF PointF
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

        public PointC(PointF p)
        {
            _pointF = p;
        }

        public PointC(Point p)
        {
            _pointF = p;
        }

        public PointC(PointC p)
        {
            _pointF = new PointF(p.X, p.Y);
        }

        public PointC(float x, float y)
        {
            _pointF = new PointF(x, y);
        }

        public PointC()
        {

        }

        public void CopyFrom(PointF point)
        {
            _pointF = point;
        }

        public void CopyFrom(PointC point)
        {
            _pointF = new PointF(point.X, point.Y);
        }

        public object Clone()
        {
            return new PointF(X, Y);
        }

        public Point ToPoint()
        {
            return new Point((int)Math.Round(X), (int)Math.Round(Y));
        }

    }
}