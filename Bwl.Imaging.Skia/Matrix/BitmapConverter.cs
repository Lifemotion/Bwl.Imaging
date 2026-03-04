using SkiaSharp;

namespace Bwl.Imaging.Skia
{

    public class BitmapConverter
    {
        public static GrayMatrix BitmapToGrayMatrix(SKBitmap bitmap)
        {
            var processor = new BitmapOperations();
            processor.LoadBitmap(bitmap);
            return processor.GetGrayMatrix();
        }

        public static RGBMatrix BitmapToRGBMatrix(SKBitmap bitmap)
        {
            var processor = new BitmapOperations();
            processor.LoadBitmap(bitmap);
            return processor.GetRGBMatrix();
        }

        public static SKBitmap GrayMatrixToBitmap(GrayMatrix matrix)
        {
            var processor = new BitmapOperations();
            processor.LoadGrayMatrixWithLimiter(matrix);
            return processor.GetBitmap();
        }

        public static SKBitmap RGBMatrixToBitmap(RGBMatrix matrix, bool useLimiter = false)
        {
            var processor = new BitmapOperations();
            processor.LoadRGBMatrixWithLimiter(matrix);
            return processor.GetBitmap();
        }
    }

    public static class BitmapConverterExtensions
    {
        public static GrayMatrix BitmapToGrayMatrix(this SKBitmap bitmap)
        {
            return BitmapConverter.BitmapToGrayMatrix(bitmap);
        }

        public static RGBMatrix BitmapToRgbMatrix(this SKBitmap bitmap)
        {
            return BitmapConverter.BitmapToRGBMatrix(bitmap);
        }

        public static SKBitmap ToBitmap(this GrayMatrix matrix)
        {
            return BitmapConverter.GrayMatrixToBitmap(matrix);
        }

        public static SKBitmap ToSKBitmap(this RGBMatrix matrix)
        {
            return BitmapConverter.RGBMatrixToBitmap(matrix);
        }
    }
}