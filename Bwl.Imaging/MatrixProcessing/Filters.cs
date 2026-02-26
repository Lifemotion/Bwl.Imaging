using System;
using System.Threading.Tasks;

namespace Bwl.Imaging
{

    public static class Filters
    {

        /// <summary>
    /// Фильтр повышения резкости полутонового изображения разреженной маской 5*5
    /// </summary>
    /// <param name="img">Исходное изображение.</param>
        public static GrayMatrix Sharpen5Gray(GrayMatrix img)
        {
            int msize = 5;
            var res = new GrayMatrix(img.Width, img.Height);
            int[] imgGray = img.Gray;
            int[] resGray = res.Gray;
            Parallel.For(0, img.Height - msize, (row) =>
                {
                    int m0RowOffset = row * img.Width;
                    int m2RowOffset = (row + 2) * img.Width;
                    int m4RowOffset = (row + 4) * img.Width;
                    for (int col = 0, loopTo = img.Width - msize - 1; col <= loopTo; col++)
                    {
                        double value = -0.1d * imgGray[m0RowOffset + col] + -0.1d * imgGray[m0RowOffset + col + 2] + -0.1d * imgGray[m0RowOffset + col + 4] + -0.1d * imgGray[m2RowOffset + col] + (+1.8d * imgGray[m2RowOffset + col + 2]) + -0.1d * imgGray[m2RowOffset + col + 4] + -0.1d * imgGray[m4RowOffset + col] + -0.1d * imgGray[m4RowOffset + col + 2] + -0.1d * imgGray[m4RowOffset + col + 4];
                        resGray[m2RowOffset + col + 2] = ImagingMath.Limit(value);
                    }
                });
            return res;
        }

        /// <summary>
    /// 2D-медианный фильтр
    /// </summary>    
    /// <param name="img">Исходные данные.</param>
    /// <param name="N">Размер фильтра (нечетное значение).</param>
        public static GrayMatrix MedianFilter2D(GrayMatrix img, int N)
        {
            N = N % 2 == 0 ? N + 1 : N; // Нечетный размер окна фильтра
            int NR = (N - 1) / 2; // Радиус фильтра
            int M = (N * N - 1) / 2; // Координаты медианы
            var res = new GrayMatrix(img.Width, img.Height);
            int[] imgGray = img.Gray;
            int[] resGray = res.Gray;
            Parallel.For(NR, res.Height - NR, (y) =>
                {
                    int[] median = new int[(N * N)];
                    int offset = y * img.Width;
                    for (int x = NR, loopTo = res.Width - NR - 1; x <= loopTo; x++)
                    {
                        int k = 0;
                        for (int y2 = y - NR, loopTo1 = y + NR; y2 <= loopTo1; y2++)
                        {
                            for (int x2 = x - NR, loopTo2 = x + NR; x2 <= loopTo2; x2++)
                            {
                                median[k] = img.GetGrayPixel(x2, y2);
                                k += 1;
                            }
                        }
                        Array.Sort(median);
                        resGray[x + offset] = median[M];
                    }
                });
            return res;
        }

        public static void LinearContrast(GrayMatrix img, int ymin = 0, int ymax = 255)
        {
            var stats = ImagingMath.GetBrightnessStats(img);
            int width = img.Width;
            int height = img.Height;
            int xmin = stats.BrMin;
            int xmax = stats.BrMax;
            if (xmax > xmin)
            {
                int[] imgGray = img.Gray;
                for (int k = 0, loopTo = img.Width * img.Height - 1; k <= loopTo; k++)
                    imgGray[k] = (int)Math.Round((imgGray[k] - xmin) / (double)(xmax - xmin) * (ymax - ymin) + ymin);
            }
        }
    }
}