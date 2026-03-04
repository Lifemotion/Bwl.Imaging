using SkiaSharp;
using System;

using System.Runtime.Serialization;

namespace Bwl.Imaging.Skia
{

    [DataContract()]
    public class Polygon
    {
        [DataMember()]
        protected SKPoint[] _points;
        [DataMember()]
        protected bool _isClosed;

        public ParametersDictionaryList Parameters { get; set; } = new ParametersDictionaryList();

        public SKPoint[] Points
        {
            get
            {
                return _points;
            }
        }

        public bool IsClosed
        {
            get
            {
                return _isClosed;
            }
        }

        public Polygon(bool closed, params SKPoint[] points)
        {
            if (points is null || points.Length < 2)
                throw new ArgumentException("points() must have at least 2 elements");
            _isClosed = closed;
            _points = points;
        }

        public float Left
        {
            get
            {
                float min = float.MaxValue;
                foreach (var pnt in _points)
                {
                    if (pnt.X < min)
                        min = pnt.X;
                }
                return min;
            }
        }

        public float Top
        {
            get
            {
                float min = float.MaxValue;
                foreach (var pnt in _points)
                {
                    if (pnt.Y < min)
                        min = pnt.Y;
                }
                return min;
            }
        }

        public float Right
        {
            get
            {
                float max = float.MinValue;
                foreach (var pnt in _points)
                {
                    if (pnt.X > max)
                        max = pnt.X;
                }
                return max;
            }
        }

        public float Bottom
        {
            get
            {
                float max = float.MinValue;
                foreach (var pnt in _points)
                {
                    if (pnt.Y > max)
                        max = pnt.Y;
                }
                return max;
            }
        }

        public float Width
        {
            get
            {
                return Right - Left;
            }
        }

        public float Height
        {
            get
            {
                return Bottom - Top;
            }
        }

        public SKRect GetBoundRectangleF()
        {
            return new SKRect(Left, Top, Right, Bottom);
        }
    }
}