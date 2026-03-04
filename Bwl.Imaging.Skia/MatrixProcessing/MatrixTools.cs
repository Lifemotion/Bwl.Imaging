
using SkiaSharp;
using System.Threading.Tasks;

namespace Bwl.Imaging.Skia
{

    public static class MatrixTools
    {

        /// <summary>
        /// Получение полутоновой матрицы на основе BitmapInfo
        /// </summary>
        public static GrayMatrix GetGrayMatrix(this BitmapInfo bi)
        {
            return BitmapConverter.BitmapToGrayMatrix(bi.GetClonedBmp());
        }

        /// <summary>
        /// Получение полноцветной матрицы на основе BitmapInfo
        /// </summary>
        public static RGBMatrix GetRGBMatrix(this BitmapInfo bi)
        {
            return BitmapConverter.BitmapToRGBMatrix(bi.GetClonedBmp());
        }

        /// <summary>
        /// Выравнивание полутоновой матрицы до ширины кратной 4
        /// </summary>
        public static GrayMatrix GrayMatrixAlign4(GrayMatrix img)
        {
            return GrayMatrixAlign(img, 4);
        }

        /// <summary>
        /// Выравнивание полутоновой матрицы до ширины кратной align
        /// </summary>
        public static GrayMatrix GrayMatrixAlign(GrayMatrix img, int align)
        {
            if (img.Width % align != 0)
            {
                int padding = align - img.Width % align;
                int paddingL = padding / 2;
                var result = new GrayMatrix(img.Width + padding, img.Height);
                int[] imgGray = img.Gray;
                int[] resultGray = result.Gray;
                int offsetImg = 0;
                int offsetRes = paddingL;
                for (int y = 0, loopTo = img.Height - 1; y <= loopTo; y++)
                {
                    for (int x = 0, loopTo1 = img.Width - 1; x <= loopTo1; x++)
                        resultGray[x + offsetRes] = imgGray[x + offsetImg];
                    offsetImg += img.Width;
                    offsetRes += result.Width;
                }
                return result;
            }
            else
            {
                return img;
            }
        }

        /// <summary>
        /// Выравнивание цветной матрицы до ширины кратной 4
        /// </summary>
        public static RGBMatrix RGBMatrixAlign4(RGBMatrix img)
        {
            return RGBMatrixAlign(img, 4);
        }

        /// <summary>
        /// Выравнивание цветной матрицы до ширины кратной align
        /// </summary>
        public static RGBMatrix RGBMatrixAlign(RGBMatrix img, int align)
        {
            if (img.Width % align != 0)
            {
                int padding = align - img.Width % align;
                int paddingL = padding / 2;
                var result = new RGBMatrix(img.Width + padding, img.Height);
                Parallel.For(0, 3, (channel) =>
                    {
                        int[] imgMatrix = img.GetMatrix(channel);
                        int[] resultMatrix = result.GetMatrix(channel);
                        int offsetImg = 0;
                        int offsetRes = paddingL;
                        for (int y = 0, loopTo = img.Height - 1; y <= loopTo; y++)
                        {
                            for (int x = 0, loopTo1 = img.Width - 1; x <= loopTo1; x++)
                                resultMatrix[x + offsetRes] = imgMatrix[x + offsetImg];
                            offsetImg += img.Width;
                            offsetRes += result.Width;
                        }
                    });
                return result;
            }
            else
            {
                return img;
            }
        }

        /// <summary>
        /// Установка прямогольника по "выровненным" позициям (каждая позиция становится кратной align)
        /// </summary>
        public static SKRectI RectangleAlign(SKRectI rect, int align = 4)
        {
            int rX1 = rect.Left;
            int rY1 = rect.Top;
            int rX2 = rX1 + rect.Width;
            int rY2 = rY1 + rect.Height;
            rX1 += align - rX1 % align;
            rY1 += align - rY1 % align;
            rX2 -= rX2 % align;
            rY2 -= rY2 % align;
            if (rX2 < align)
                rX2 = align;
            if (rY2 < align)
                rY2 = align;
            return SKExtensions.SKRectIFromXYWH(rX1, rY1, rX2 - rX1, rY2 - rY1);
        }

        /// <summary>
        /// Получение полутоной подматрицы
        /// </summary>
        public static GrayMatrix GrayMatrixSubRect(GrayMatrix img, SKRectI rect)
        {
            var result = new GrayMatrix(rect.Width, rect.Height);
            int[] imgGray = img.Gray;
            int[] resultGray = result.Gray;
            int offsetImg = rect.Left + rect.Top * img.Width;
            int offsetRes = 0;
            for (int y = 0, loopTo = rect.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = rect.Width - 1; x <= loopTo1; x++)
                    resultGray[x + offsetRes] = imgGray[x + offsetImg];
                offsetImg += img.Width;
                offsetRes += result.Width;
            }
            return result;
        }

        /// <summary>
        /// Получение цветной подматрицы
        /// </summary>
        public static RGBMatrix RGBMatrixSubRect(RGBMatrix img, SKRectI rect)
        {
            var result = new RGBMatrix(rect.Width, rect.Height);
            Parallel.For(0, 3, (channel) =>
                {
                    int[] imgMatrix = img.GetMatrix(channel);
                    int[] resultMatrix = result.GetMatrix(channel);
                    int offsetImg = rect.Left + rect.Top * img.Width;
                    int offsetRes = 0;
                    for (int y = 0, loopTo = rect.Height - 1; y <= loopTo; y++)
                    {
                        for (int x = 0, loopTo1 = rect.Width - 1; x <= loopTo1; x++)
                            resultMatrix[x + offsetRes] = imgMatrix[x + offsetImg];
                        offsetImg += img.Width;
                        offsetRes += result.Width;
                    }
                });
            return result;
        }

        /// <summary>
        /// Инверсия полутонового изображения
        /// </summary>
        public static void InverseGray(GrayMatrix img)
        {
            int[] imgGray = img.Gray;
            int offset = 0;
            for (int y = 0, loopTo = img.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = img.Width - 1; x <= loopTo1; x++)
                    imgGray[x + offset] = byte.MaxValue - imgGray[x + offset];
                offset += img.Width;
            }
        }

        /// <summary>
        /// Инверсия цветного изображения
        /// </summary>
        public static void InverseRGB(RGBMatrix img)
        {
            Parallel.For(0, 3, (channel) =>
                {
                    int[] imgMatrix = img.GetMatrix(channel);
                    int offset = 0;
                    for (int y = 0, loopTo = img.Height - 1; y <= loopTo; y++)
                    {
                        for (int x = 0, loopTo1 = img.Width - 1; x <= loopTo1; x++)
                            imgMatrix[x + offset] = byte.MaxValue - imgMatrix[x + offset];
                        offset += img.Width;
                    }
                });
        }
    }
}