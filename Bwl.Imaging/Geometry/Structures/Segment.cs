using System;
using System.Drawing;

namespace Bwl.Imaging
{

    public class Segment
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ID { get; set; }
        public int Tag { get; set; }
        public string Debug { get; set; }

        public int Right
        {
            get
            {
                return Left + Width;
            }
            set
            {
                Width = value - Left;
            }
        }
        public int Bottom
        {
            get
            {
                return Top + Height;
            }
            set
            {
                Height = value - Top;
            }
        }
        public override string ToString()
        {
            return "L:" + Left.ToString() + " :T" + Top.ToString() + " :W" + Width.ToString() + " :H" + Height.ToString();
        }

        public int CenterX
        {
            get
            {
                return (int)Math.Round(Left + Width / 2d);
            }
        }

        public float WHRatio
        {
            get
            {
                if (Height == 0)
                    return 0f;
                return (int)Math.Round(Width / (double)Height);
            }
        }

        public int CenterY
        {
            get
            {
                return (int)Math.Round(Top + Height / 2d);
            }
        }

        public bool IsPointInside(int x, int y)
        {
            return x >= Left & x <= Left + Width & y >= Top & y <= Top + Height;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle(Left, Top, Width, Height);
        }

        public RectangleF ToRectangleF()
        {
            return new RectangleF(Left, Top, Width, Height);
        }

        public Segment()
        {

        }

        public Segment(int x, int y, int width, int height)
        {
            Left = x;
            Top = y;
            Width = width;
            Height = height;
        }

    }
}