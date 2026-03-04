using SkiaSharp;
using System;


namespace Bwl.Imaging.Skia
{

    public static class VectorTools
    {
        public class Vector
        {
            public double X { get; set; }
            public double Y { get; set; }

            public Vector(double X, double Y)
            {
                this.X = X;
                this.Y = Y;
            }

            /// <summary>
            /// При вычислении вектора между (1,1) и (1,-1) этот метод дает - Pi/2! Это угловой переход от первого вектора ко второму
            /// </summary>
            public static double AngleAB(Vector A, Vector B)
            {
                return Math.Atan2(Cross(A, B), Dot(A, B));
            }

            public double Magnitude
            {
                get
                {
                    return Math.Sqrt(Dot(this, this));
                }
            }

            public static double Dot(Vector A, Vector B)
            {
                return A.X * B.X + A.Y * B.Y;
            }

            public static double Cross(Vector A, Vector B)
            {
                return A.X * B.Y - A.Y * B.X;
            }
        }

        /// <summary>
        /// Тест: полигон пустой?
        /// </summary>
        public static bool PolygonIsEmpty(SKPoint[] polygon)
        {
            if (polygon.Length > 3)
            {
                var firstPoint = polygon[0];
                for (int i = 1, loopTo = polygon.Length - 1; i <= loopTo; i++)
                {
                    var currentPoint = polygon[i];
                    if (currentPoint != firstPoint)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Тест: точка находится внутри полигона?
        /// </summary>
        public static bool PointInPolygon(SKPoint point, SKPoint[] polygon)
        {
            bool result = false;
            int N = polygon.Length;
            int j = N - 1;
            for (int i = 0, loopTo = N - 1; i <= loopTo; i++)
            {
                if (polygon[i].Y <= point.Y && point.Y < polygon[j].Y || polygon[j].Y <= point.Y && point.Y < polygon[i].Y)
                {
                    if (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        /// <summary>
        /// Нормализация длины вектора
        /// </summary>
        public static SKPoint NormalizeVector(SKPoint vector, float targetLen = 1.0f)
        {
            double z = Math.Sqrt((double)(vector.X * vector.X + vector.Y * vector.Y));
            double zNorm = (double)targetLen / z; // Нормализатор к заданной длине
            double xn = (double)vector.X * zNorm;
            double yn = (double)vector.Y * zNorm;
            return new SKPoint((float)xn, (float)yn);
        }

        /// <summary>
        /// Проверка на нахождение вектора между векторами-границами
        /// </summary>
        public static bool VectorInBounds(SKPoint vector, SKPoint vectorBound1, SKPoint vectorBound2, double tol = 0.017d)
        {
            double boundsAngle = AngleBetweenVectors(vectorBound1, vectorBound2);
            double angle1 = AngleBetweenVectors(vector, vectorBound1);
            double angle2 = AngleBetweenVectors(vector, vectorBound2);
            double angle = angle1 + angle2;
            double angleDiff = Math.Abs(boundsAngle - angle);
            return angleDiff <= tol;
        }

        /// <summary>
        /// Вычисление модуля угла между векторами №1
        /// </summary>
        public static double AngleBetweenVectors(SKPoint vector1, SKPoint vector2)
        {
            return Math.Abs(AnglePathBetweenVectors(vector1, vector2));
        }

        /// <summary>
        /// Вычисление углового перехода между двумя векторами
        /// </summary>
        public static double AnglePathBetweenVectors(SKPoint vector1, SKPoint vector2)
        {
            var v1 = new Vector((double)vector1.X, (double)vector1.Y);
            var v2 = new Vector((double)vector2.X, (double)vector2.Y);
            return Vector.AngleAB(v1, v2);
        }
    }
}