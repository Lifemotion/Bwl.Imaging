using System.Drawing;

namespace Bwl.Imaging
{

    public class BitmapConverter
    {
        public static GrayMatrix BitmapToGrayMatrix(Bitmap bitmap)
        {
            var processor = new BitmapOperations();
            processor.LoadBitmap(bitmap);
            return processor.GetGrayMatrix();
        }

        public static RGBMatrix BitmapToRGBMatrix(Bitmap bitmap)
        {
            var processor = new BitmapOperations();
            processor.LoadBitmap(bitmap);
            return processor.GetRGBMatrix();
        }

        public static Bitmap GrayMatrixToBitmap(GrayMatrix matrix)
        {
            var processor = new BitmapOperations();
            processor.LoadGrayMatrixWithLimiter(matrix);
            return processor.GetBitmap();
        }

        public static Bitmap RGBMatrixToBitmap(RGBMatrix matrix, bool useLimiter = false)
        {
            var processor = new BitmapOperations();
            processor.LoadRGBMatrixWithLimiter(matrix);
            return processor.GetBitmap();
        }
    }

    public static class BitmapConverterExtensions
    {
        public static GrayMatrix BitmapToGrayMatrix(this Bitmap bitmap)
        {
            return BitmapConverter.BitmapToGrayMatrix(bitmap);
        }

        public static RGBMatrix BitmapToRgbMatrix(this Bitmap bitmap)
        {
            return BitmapConverter.BitmapToRGBMatrix(bitmap);
        }

        public static Bitmap ToBitmap(this GrayMatrix matrix)
        {
            return BitmapConverter.GrayMatrixToBitmap(matrix);
        }

        public static Bitmap ToBitmap(this RGBMatrix matrix)
        {
            return BitmapConverter.RGBMatrixToBitmap(matrix);
        }
    }
}