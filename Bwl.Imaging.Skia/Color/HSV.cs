using SkiaSharp;
using System;


namespace Bwl.Imaging.Skia
{

    // Новых конструкторов добавлять не надо!
    public struct HSV : IRGBConvertable
    {
        public int A;
        public int H;
        public int S;
        public int V;

        public HSV(int H, int S, int V)
        {
            this.H = H;
            this.S = S;
            this.V = V;
            A = 255;
        }

        public HSV(int H, int S, int V, int A)
        {
            this.H = H;
            this.S = S;
            this.V = V;
            this.A = A;
        }

        public static HSV FromRgb(RGB rgb)
        {
            return FromRgb(rgb.A, rgb.R, rgb.G, rgb.B);
        }

        public static HSV FromRgb(SKColor rgb)
        {
            return FromRgb(rgb.Alpha, rgb.Red, rgb.Green, rgb.Blue);
        }

        public static HSV FromRgb(int R, int G, int B)
        {
            return FromRgb(255, R, G, B);
        }

        public static HSV FromRgb(int A, int R, int G, int B)
        {
            var result = default(HSV);
            int cmax = Math.Max(Math.Max(R, G), B);
            int cmin = Math.Min(Math.Min(R, G), B);
            // H
            if (cmax == cmin)
            {
                result.H = 0;
            }
            else if (cmax == R & G >= B)
            {
                result.H = ImagingMath.Limit(60 * (G - B) / (double)(cmax - cmin));
            }
            else if (cmax == R & G < B)
            {
                result.H = ImagingMath.Limit(60 * (G - B) / (double)(cmax - cmin) + 360d);
            }
            else if (cmax == G)
            {
                result.H = ImagingMath.Limit(60 * (B - R) / (double)(cmax - cmin) + 120d);
            }
            else if (cmax == B)
            {
                result.H = ImagingMath.Limit(60 * (R - G) / (double)(cmax - cmin) + 240d);
            }
            // S
            if (cmax == 0)
            {
                result.S = 0;
            }
            else
            {
                result.S = ImagingMath.Limit((1d - cmin / (double)cmax) * 255d);
            }
            // V
            result.V = cmax;
            result.A = A;
            return result;
        }

        public RGB ToRGB()
        {
            return RGB.FromHsv(this);
        }

        public SKColor ToColor()
        {
            return RGB.FromHsv(this).ToColor();
        }

        /// <summary>
    /// Вычисление кратчайшего расстояния между двумя тонами
    /// </summary>
    /// <remarks>Наибольшее возможное расстояние в пространстве Hue - это 0.5 [0..1],
    /// которое сооотв. противоположному цвету. Если получили больше 0.5, нужно
    /// нормализовывать результат.</remarks>
        public static double HueDistance(double hue1, double hue2)
        {
            double dist = Math.Abs(hue1 - hue2);
            return dist > 0.5d ? 1d - dist : dist;
        }
    }
}