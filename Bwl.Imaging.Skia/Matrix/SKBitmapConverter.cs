using SkiaSharp;

namespace Bwl.Imaging.Skia;

public class SKBitmapConverter
{
    public static GrayMatrix SKBitmapToGrayMatrix(SKBitmap bitmap)
    {
        var processor = new SKBitmapOperations();
        processor.LoadSKBitmap(bitmap);
        return processor.GetGrayMatrix();
    }

    public static RGBMatrix SKBitmapToRGBMatrix(SKBitmap bitmap)
    {
        var processor = new SKBitmapOperations();
        processor.LoadSKBitmap(bitmap);
        return processor.GetRGBMatrix();
    }

    public static SKBitmap GrayMatrixToSKBitmap(GrayMatrix matrix)
    {
        var processor = new SKBitmapOperations();
        processor.LoadGrayMatrixWithLimiter(matrix);
        return processor.GetSKBitmap();
    }

    public static SKBitmap RGBMatrixToSKBitmap(RGBMatrix matrix, bool useLimiter = false)
    {
        var processor = new SKBitmapOperations();
        processor.LoadRGBMatrixWithLimiter(matrix);
        return processor.GetSKBitmap();
    }
}

public static class SKBitmapConverterExtensions
{
    public static GrayMatrix ToGrayMatrix(this SKBitmap bitmap)
    {
        return SKBitmapConverter.SKBitmapToGrayMatrix(bitmap);
    }

    public static RGBMatrix ToRgbMatrix(this SKBitmap bitmap)
    {
        return SKBitmapConverter.SKBitmapToRGBMatrix(bitmap);
    }

    public static SKBitmap ToSKBitmap(this GrayMatrix matrix)
    {
        return SKBitmapConverter.GrayMatrixToSKBitmap(matrix);
    }

    public static SKBitmap ToSKBitmap(this RGBMatrix matrix)
    {
        return SKBitmapConverter.RGBMatrixToSKBitmap(matrix);
    }
}