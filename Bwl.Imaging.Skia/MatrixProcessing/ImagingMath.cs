using SkiaSharp;

namespace Bwl.Imaging.Skia;

public static class ImagingMath
{

    /// <summary>
    /// Ограничение значения диапазоном Byte.
    /// </summary>
    public static byte Limit(int value, int maxValue = 255)
    {
        value = (int)Math.Round(Math.Floor((decimal)value));
        value = value < 0 ? 0 : value;
        value = value > maxValue ? maxValue : value;
        return (byte)value;
    }

    /// <summary>
    /// Ограничение значения диапазоном Byte.
    /// </summary>
    public static byte Limit(double value, double maxValue = 255d)
    {
        value = Math.Floor(value);
        value = value < 0d ? 0d : value;
        value = value > maxValue ? maxValue : value;
        return (byte)Math.Round(value);
    }

    /// <summary>
    /// Статистика по яркости
    /// </summary>
    public static BrightnessStats GetBrightnessStats(GrayMatrix img)
    {
        var tmpSum = default(long);
        var stats = new BrightnessStats() { BrMax = 0, BrMin = 255 };
        var tmpHist = new long[256];
        var tmpMax = default(long);

        int[] imgGray = img.Gray;
        for (int k = 0, loopTo = img.Width * img.Height - 1; k <= loopTo; k++)
        {
            if (imgGray[k] < stats.BrMin)
                stats.BrMin = imgGray[k];
            if (imgGray[k] > stats.BrMax)
                stats.BrMax = imgGray[k];
            tmpSum += imgGray[k];
            tmpHist[imgGray[k]] += 1L;
        }
        tmpSum = (long)Math.Round(tmpSum / (double)(img.Width * img.Height));
        stats.BrAvg = (int)tmpSum;

        for (int i = 0; i <= 255; i++)
            tmpMax = (long)Math.Round(tmpMax + tmpHist[i] / 255d);
        for (int i = 0; i <= 255; i++)
            stats.Histogram[i] = (int)(tmpHist[i] / (tmpMax / 128L + 1L));
        return stats;
    }

    /// <summary>
    /// Билинейная интерполяция между двумя значениями
    /// </summary>
    /// <param name="value1">Значение 1.</param>
    /// <param name="value2">Значение 2.</param>
    /// <param name="weight1">Вес для значения 1.</param>
    /// <param name="weight2">Вес для значения 2.</param>
    public static double Bilinear(double value1, double value2, double weight1, double weight2)
    {
        double ws = weight1 + weight2;
        weight1 = weight1 / ws;
        weight2 = weight2 / ws;
        return value1 * weight1 + value2 * weight2;
    }

    /// <summary>
    /// Билинейная интерполяция между двумя точками
    /// </summary>
    /// <param name="point1">Точка 1.</param>
    /// <param name="point2">Точка 2.</param>
    /// <param name="weight1">Вес для точки 1.</param>
    /// <param name="weight2">Вес для точки 2.</param>
    public static SKPoint Bilinear(SKPoint point1, SKPoint point2, double weight1, double weight2)
    {
        SKPoint result = new();
        result.X = (float)Bilinear((double)point1.X, (double)point2.X, weight1, weight2);
        result.Y = (float)Bilinear((double)point1.Y, (double)point2.Y, weight1, weight2);
        return result;
    }

    /// <summary>
    /// Поиск минимального/максимального значения в полутоновой матрице
    /// </summary>
    /// <param name="img">Исходная матрица.</param>
    /// <param name="min">Найденный минимум.</param>
    /// <param name="max">Найденный максимум.</param>
    public static void MinMax2D(GrayMatrix img, ref int min, ref int max)
    {
        min = img.GetGrayPixel(0, 0);
        max = img.GetGrayPixel(0, 0);
        int[] imgGray = img.Gray;
        int offset = 0;
        for (int y = 0, loopTo = img.Height - 1; y <= loopTo; y++)
        {
            for (int x = 0, loopTo1 = img.Width - 1; x <= loopTo1; x++)
            {
                int val = imgGray[x + offset];
                min = min < val ? min : val;
                max = max > val ? max : val;
            }
            offset += img.Width;
        }
    }
}