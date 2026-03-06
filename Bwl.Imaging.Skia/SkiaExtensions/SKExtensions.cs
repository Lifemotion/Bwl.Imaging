using SkiaSharp;

namespace Bwl.Imaging.Skia;

public static class SKExtensions
{
    /// <summary>
    /// Creates a rectangle that is defined by a specified top-left point and size.
    /// </summary>
    /// <param name="point">The point that specifies the coordinates of the rectangle's top-left corner.</param>
    /// <param name="size">The size that specifies the width and height of the rectangle.</param>
    /// <returns>An SKRect structure that represents the rectangle defined by the specified point and size.</returns>
    public static SKRect SKRectFromPointSize(SKPoint point, SKSize size)
    {
        return SKRectFromXYWH(point.X, point.Y, size.Width, size.Height);
    }

    /// <summary>
    /// Creates a rectangle defined by the specified top-left corner and dimensions.
    /// </summary>
    /// <remarks>The width and height parameters must be non-negative. Supplying negative values may
    /// result in an invalid rectangle.</remarks>
    /// <param name="x">The x-coordinate of the rectangle's top-left corner.</param>
    /// <param name="y">The y-coordinate of the rectangle's top-left corner.</param>
    /// <param name="width">The width of the rectangle, in pixels. Must be a non-negative value.</param>
    /// <param name="height">The height of the rectangle, in pixels. Must be a non-negative value.</param>
    /// <returns>An SKRect that represents the rectangle defined by the specified coordinates and dimensions.</returns>
    public static SKRect SKRectFromXYWH(float x, float y, float width, float height)
    {
        return new SKRect(
            x,
            y,
            x + width,
            y + height
        );
    }

    /// <summary>
    /// Creates a rectangle with integer coordinates and size, using the specified top-left point and dimensions.
    /// </summary>
    /// <param name="point">The point that defines the top-left corner of the rectangle.</param>
    /// <param name="siize">The size that specifies the width and height of the rectangle.</param>
    /// <returns>An SKRectI structure representing the rectangle defined by the given point and size.</returns>
    public static SKRectI SKRectIFromPointSize(SKPointI point, SKSizeI siize)
    {
        return SKRectIFromXYWH(point.X, point.Y, siize.Width, siize.Height);
    }

    /// <summary>
    /// Creates a new SKRectI structure that represents a rectangle defined by the specified position and
    /// dimensions.
    /// </summary>
    /// <remarks>The width and height parameters must be non-negative; otherwise, the resulting
    /// rectangle may not be valid.</remarks>
    /// <param name="x">The x-coordinate of the rectangle's top-left corner.</param>
    /// <param name="y">The y-coordinate of the rectangle's top-left corner.</param>
    /// <param name="width">The width of the rectangle, in pixels. Must be a non-negative value.</param>
    /// <param name="height">The height of the rectangle, in pixels. Must be a non-negative value.</param>
    /// <returns>An SKRectI structure representing the rectangle defined by the specified coordinates and dimensions.</returns>
    public static SKRectI SKRectIFromXYWH(int x, int y, int width, int height)
    {
        return new SKRectI(
            x,
            y,
            x + width,
            y + height
        );
    }

    /// <summary>
    /// Creates a new bitmap that is a resized version of the specified bitmap, using the given target size.
    /// </summary>
    /// <param name="bmp">The original bitmap to resize. This parameter cannot be null.</param>
    /// <param name="size">The target size for the new bitmap, specified as an <see cref="SKSize"/> structure. The width and height
    /// must be positive values.</param>
    /// <returns>A new <see cref="SKBitmap"/> instance that represents the resized bitmap.</returns>
    public static SKBitmap CloneResized(this SKBitmap bmp, SKSize size)
    {
        return CloneResized(bmp, (int)size.Width, (int)size.Height);
    }

    /// <summary>
    /// Creates a new bitmap that is a resized version of the specified bitmap using cubic resampling for improved
    /// image quality.
    /// </summary>
    /// <remarks>The resizing operation uses the Mitchell cubic resampling algorithm to enhance the
    /// quality of the output image. The original bitmap's color type and alpha type are retained in the resized
    /// bitmap.</remarks>
    /// <param name="bmp">The source bitmap to resize. This parameter must not be null.</param>
    /// <param name="width">The width, in pixels, of the resulting resized bitmap. Must be greater than zero.</param>
    /// <param name="height">The height, in pixels, of the resulting resized bitmap. Must be greater than zero.</param>
    /// <returns>A new SKBitmap instance that represents the resized version of the original bitmap, preserving the color
    /// type and alpha type.</returns>
    public static SKBitmap CloneResized(this SKBitmap bmp, int width, int height)
    {
        var img = SKImage.FromBitmap(bmp);
        var resizedBmp = new SKBitmap(width, height, bmp.ColorType, bmp.AlphaType);
        var options = new SKSamplingOptions(SKCubicResampler.Mitchell);
        using (var canvas = new SKCanvas(resizedBmp))
        {
            canvas.DrawImage(img, new SKRect(0, 0, width, height), options);
        }
        return resizedBmp;
    }

    public static SKSizeI Size(this SKBitmap bmp)
    {
        return new SKSizeI(bmp.Width, bmp.Height);
    }

    /// <summary>
    /// Rotates and flips the specified bitmap according to the given transformation type.
    /// </summary>
    /// <remarks>This method modifies the bitmap in place. Ensure that the bitmap is properly
    /// initialized before calling this method.</remarks>
    /// <param name="bmp">The bitmap to transform. This parameter cannot be null.</param>
    /// <param name="rotateFlipType">The type of rotation and flip operation to apply to the bitmap.</param>
    public static void RotateFlip(this SKBitmap bmp, SKRotateFlipType rotateFlipType)
    {
        var value = ((int)rotateFlipType) & 7;
        if (value == 0)
            return;

        var rotationDegrees = value & 3;
        var flipX = (value & 4) != 0;

        var source = bmp.Copy();
        if (source == null)
            throw new InvalidOperationException("Unable to copy source bitmap.");

        using (source)
        {
            var outputWidth = (rotationDegrees == 1 || rotationDegrees == 3) ? source.Height : source.Width;
            var outputHeight = (rotationDegrees == 1 || rotationDegrees == 3) ? source.Width : source.Height;

            var info = new SKImageInfo(outputWidth, outputHeight, source.ColorType, source.AlphaType, source.ColorSpace);
            if (!bmp.TryAllocPixels(info))
                throw new InvalidOperationException("Unable to allocate destination bitmap pixels.");

            using (var canvas = new SKCanvas(bmp))
            {
                canvas.Clear(SKColors.Transparent);
                canvas.Translate(outputWidth / 2f, outputHeight / 2f);

                if (rotationDegrees != 0)
                    canvas.RotateDegrees(rotationDegrees * 90f);

                if (flipX)
                    canvas.Scale(-1f, 1f);

                canvas.Translate(-source.Width / 2f, -source.Height / 2f);
                canvas.DrawBitmap(source, 0, 0);
            }
        }
    }


}
