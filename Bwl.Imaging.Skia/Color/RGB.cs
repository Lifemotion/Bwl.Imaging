

using SkiaSharp;

namespace Bwl.Imaging.Skia
{

    // Новых конструкторов добавлять не надо!
    public struct RGB : IRGBConvertable
    {

        public int A;
        public int R;
        public int G;
        public int B;

        public RGB(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
            A = 255;
        }

        public RGB(int r, int g, int b, int a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public RGB(SKColor rgb)
        {
            R = rgb.Red;
            G = rgb.Green;
            B = rgb.Blue;
            A = rgb.Alpha;
        }

        public static RGB FromHsv(HSV hsv)
        {
            return FromHsv(hsv.H, hsv.S, hsv.V, 255);
        }

        public static RGB FromHsv(int h, int s, int v)
        {
            return FromHsv(h, s, v, 255);
        }

        public static RGB FromHsv(int h, int s, int v, int alpha)
        {
            int valuemax = 255;
            var rgb = default(RGB);
            int hi = ImagingMath.Limit(h / 60.0d) % 6;
            byte vmin = ImagingMath.Limit((valuemax - s) * v / (double)valuemax);
            double a = (v - vmin) * (h % 60 / 60.0d);
            byte vinc = ImagingMath.Limit(vmin + a);
            byte vdec = ImagingMath.Limit(v - a);
            switch (hi)
            {
                case 0:
                    {
                        rgb.R = v;
                        rgb.G = vinc;
                        rgb.B = vmin;
                        break;
                    }
                case 1:
                    {
                        rgb.R = vdec;
                        rgb.G = v;
                        rgb.B = vmin;
                        break;
                    }
                case 2:
                    {
                        rgb.R = vmin;
                        rgb.G = v;
                        rgb.B = vinc;
                        break;
                    }
                case 3:
                    {
                        rgb.R = vmin;
                        rgb.G = vdec;
                        rgb.B = v;
                        break;
                    }
                case 4:
                    {
                        rgb.R = vinc;
                        rgb.G = vmin;
                        rgb.B = v;
                        break;
                    }
                case 5:
                    {
                        rgb.R = v;
                        rgb.G = vmin;
                        rgb.B = vdec;
                        break;
                    }
            }
            rgb.A = alpha;
            return rgb;
        }

        public HSV ToHSV()
        {
            return HSV.FromRgb(this);
        }

        public SKColor ToColor()
        {
            return new SKColor(ImagingMath.Limit(R), ImagingMath.Limit(G), ImagingMath.Limit(B), ImagingMath.Limit(A));
        }

        public RGB ToRGB()
        {
            return this;
        }
    }
}