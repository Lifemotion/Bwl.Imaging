using SkiaSharp;
using System.Runtime.InteropServices;
using System.Security;

namespace Bwl.Imaging.Skia;


/// <summary>
/// Provides a set of unsafe functions for performing low-level image processing operations on SKBitmap objects. These functions utilize
/// direct memory access to manipulate pixel data efficiently.
/// </summary>
public static class UnsafeFunctions
{

    /// <summary>
    /// Crops a specified rectangular region from the provided bitmap and returns a new bitmap containing the cropped
    /// area.
    /// </summary>
    /// <remarks>The cropped bitmap will have the same color type and alpha type as the source bitmap. Ensure
    /// that the crop rectangle does not exceed the dimensions of the source bitmap to avoid undefined
    /// behavior.</remarks>
    /// <param name="bmp">The source bitmap from which the region will be cropped. This parameter must not be null.</param>
    /// <param name="cropRect">The rectangle, in pixel coordinates, that defines the area to crop from the source bitmap. The rectangle must be
    /// fully contained within the bounds of the source bitmap.</param>
    /// <returns>A new SKBitmap containing the pixels from the specified rectangular region of the source bitmap.</returns>
    public static unsafe SKBitmap Crop(SKBitmap bmp, SKRectI cropRect)
    {
        if (bmp == null) throw new ArgumentNullException(nameof(bmp));
        if (cropRect.Left < 0 || cropRect.Top < 0 || cropRect.Right > bmp.Width || cropRect.Bottom > bmp.Height)
            throw new ArgumentOutOfRangeException(nameof(cropRect), "Crop rectangle exceeds bitmap bounds.");

        lock (bmp)
        {
            var cropped = new SKBitmap(cropRect.Width, cropRect.Height, bmp.ColorType, bmp.AlphaType);

            IntPtr srcPtr = bmp.GetPixels();
            IntPtr dstPtr = cropped.GetPixels();
            int bytesPerPixel = bmp.BytesPerPixel;
            int srcStride = bmp.RowBytes;
            int dstStride = cropped.RowBytes;
            byte* src = (byte*)srcPtr + (cropRect.Top * srcStride) + (cropRect.Left * bytesPerPixel);
            byte* dst = (byte*)dstPtr;
            int rowSize = cropRect.Width * bytesPerPixel;
            for (int y = 0; y < cropRect.Height; y++)
            {
                Buffer.MemoryCopy(src, dst, rowSize, rowSize);
                src += srcStride;
                dst += dstStride;
            }
            cropped.NotifyPixelsChanged();
            return cropped;
        }
    }

    /// <summary>
    /// Copies pixel data from the source bitmap to the specified rectangular region of the target bitmap.
    /// </summary>
    /// <param name="srcBmp">The source bitmap containing the pixel data to copy. Cannot be null. Must have the same color type and alpha
    /// type as the target bitmap.</param>
    /// <param name="trgtBmp">The target bitmap to which the pixel data will be copied. Cannot be null. Must have the same color type and
    /// alpha type as the source bitmap.</param>
    /// <param name="region">The rectangular region within the target bitmap where the pixel data from the source bitmap will be applied. The
    /// width and height of this region must match the dimensions of the source bitmap.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="srcBmp"/> or <paramref name="trgtBmp"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the source and target bitmaps do not have the same color type and alpha type, or if the dimensions of
    /// the source bitmap do not match the width and height of the specified region.</exception>
    public static unsafe void Patch(SKBitmap srcBmp, SKBitmap trgtBmp, SKRectI region)
    {
        if (srcBmp == null) throw new ArgumentNullException(nameof(srcBmp));
        if (trgtBmp == null) throw new ArgumentNullException(nameof(trgtBmp));

        if (srcBmp.ColorType != trgtBmp.ColorType || srcBmp.AlphaType != trgtBmp.AlphaType)
        {
            throw new ArgumentException("Source and target bitmaps must have the same color type and alpha type.");
        }

        if (srcBmp.Width != region.Width || srcBmp.Height != region.Height)
        {
            throw new ArgumentException("The dimensions of the source bitmap must match the width and height of the specified region.");
        }

        if (region.Left < 0 || region.Top < 0 || region.Right > trgtBmp.Width || region.Bottom > trgtBmp.Height)
            throw new ArgumentOutOfRangeException(nameof(region), "Region exceeds target bitmap bounds.");

        // lock ordering to avoid deadlocks: always lock src then target
        lock (srcBmp)
        {
            lock (trgtBmp)
            {
                IntPtr srcPtr = srcBmp.GetPixels();
                IntPtr dstPtr = trgtBmp.GetPixels();
                int bytesPerPixel = srcBmp.BytesPerPixel;

                int srcStride = srcBmp.RowBytes;
                int dstStride = trgtBmp.RowBytes;

                byte* src = (byte*)srcPtr;
                byte* dst = (byte*)dstPtr;

                for (int y = 0; y < region.Height; y++)
                {
                    byte* srcRow = src + (y * srcStride);
                    byte* dstRow = dst + ((region.Top + y) * dstStride) + (region.Left * bytesPerPixel);
                    Buffer.MemoryCopy(srcRow, dstRow, region.Width * bytesPerPixel, region.Width * bytesPerPixel);
                }
            }
        }
    }

    /// <summary>
    /// Applies a sharpening filter to the specified bitmap image, enhancing its details.
    /// </summary>
    /// <remarks>The sharpening effect is applied using a predefined algorithm that enhances the edges in the
    /// image.</remarks>
    /// <param name="srcBmp">The source bitmap image to be sharpened. Must not be null.</param>
    /// <returns>A new SKBitmap containing the sharpened version of the source image.</returns>
    public static SKBitmap Sharpen5(SKBitmap srcBmp)
    {
        SKBitmap trgtBmp = new SKBitmap(srcBmp.Info);
        Sharpen5(srcBmp, trgtBmp);
        return trgtBmp;
    }

    /// <summary>
    /// Applies a sharpening filter to the source bitmap and stores the result in the target bitmap.
    /// </summary>
    /// <remarks>This method supports grayscale and color bitmaps. Ensure that both bitmaps are locked during
    /// processing to avoid concurrency issues.</remarks>
    /// <param name="srcBmp">The source bitmap to be sharpened. Must not be null and must have the same color type and dimensions as the
    /// target bitmap.</param>
    /// <param name="trgtBmp">The target bitmap where the sharpened result will be stored. Must not be null and must have the same color type
    /// and dimensions as the source bitmap.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="srcBmp"/> or <paramref name="trgtBmp"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the source and target bitmaps do not have the same color type, alpha type, or dimensions.</exception>
    public static unsafe void Sharpen5(SKBitmap srcBmp, SKBitmap trgtBmp)
    {
        if (srcBmp == null) throw new ArgumentNullException(nameof(srcBmp));
        if (trgtBmp == null) throw new ArgumentNullException(nameof(trgtBmp));

        if (srcBmp.ColorType != trgtBmp.ColorType || srcBmp.AlphaType != trgtBmp.AlphaType)
        {
            throw new ArgumentException("Source and target bitmaps must have the same color type and alpha type.");
        }

        if (srcBmp.Width != trgtBmp.Width || srcBmp.Height != trgtBmp.Height)
        {
            throw new ArgumentException("Source and target bitmaps must have the same dimensions.");
        }

        lock (srcBmp)
        {
            if (srcBmp.ColorType == SKColorType.Gray8)
            {
                Sharpen5Gray(srcBmp, trgtBmp);
            }
            else if (srcBmp.ColorType == SKColorType.Bgra8888 || srcBmp.ColorType == SKColorType.Rgba8888)
            {
                Sharpen5Color(srcBmp, trgtBmp);
            }
            else
            {
                throw new ArgumentException($"Unsupported color type: {srcBmp.ColorType}");
            }
        }
    }

    /// <summary>
    /// Applies a sharpening filter to a grayscale image using a 5x5 kernel.
    /// </summary>
    /// <remarks>This method processes the image in parallel for improved performance. It modifies the target
    /// bitmap directly based on the pixel values of the source bitmap.</remarks>
    /// <param name="srcBmp">The source bitmap from which the grayscale image is read. This bitmap must have a valid pixel format compatible
    /// with the sharpening operation.</param>
    /// <param name="trgtBmp">The target bitmap where the sharpened grayscale image will be written. This bitmap must be of the same
    /// dimensions as the source bitmap.</param>
    private static unsafe void Sharpen5Gray(SKBitmap srcBmp, SKBitmap trgtBmp)
    {
        int msize = 5;
        int msize2 = 2; // 5 / 2

        IntPtr srcPtr = srcBmp.GetPixels();
        IntPtr trgtPtr = trgtBmp.GetPixels();
        byte* srcBytes = (byte*)srcPtr;
        byte* trgtBytes = (byte*)trgtPtr;

        int srcStride = srcBmp.RowBytes;
        int trgtStride = trgtBmp.RowBytes;
        int width = srcBmp.Width;
        int height = srcBmp.Height;

        Parallel.For(0, height - msize, (int row) =>
        {
            byte* srcScan0 = srcBytes + (row * srcStride);
            byte* srcScan2 = srcScan0 + (2 * srcStride);
            byte* srcScan4 = srcScan2 + (2 * srcStride);

            byte* trgtScan0 = trgtBytes + (row * trgtStride);
            byte* trgtScan2 = trgtScan0 + (2 * trgtStride);

            for (int col = 0; col < width - msize; col++)
            {
                byte* m0 = srcScan0 + col;
                byte* m2 = srcScan2 + col;
                byte* m4 = srcScan4 + col;
                byte* t2 = trgtScan2 + col;

                double value = -0.1 * m0[0] + -0.1 * m0[2] + -0.1 * m0[4] +
                               -0.1 * m2[0] + 1.8 * m2[2] + -0.1 * m2[4] +
                               -0.1 * m4[0] + -0.1 * m4[2] + -0.1 * m4[4];

                value = value < 0 ? 0 : value;
                value = value > 255 ? 255 : value;
                t2[msize2] = (byte)value;
            }
        });
    }

    /// <summary>
    /// Applies a sharpening filter to the source bitmap and stores the result in the target bitmap using a 5x5 kernel.
    /// </summary>
    /// <remarks>This method uses a parallel processing approach to enhance performance. It converts the RGB
    /// values to YUV for processing and then back to RGB. Ensure that the target bitmap is properly initialized before
    /// calling this method.</remarks>
    /// <param name="srcBmp">The source bitmap to be sharpened. This bitmap must be in a format compatible with the sharpening operation.</param>
    /// <param name="trgtBmp">The target bitmap where the sharpened result will be stored. It must have the same dimensions and format as the
    /// source bitmap.</param>
    private static unsafe void Sharpen5Color(SKBitmap srcBmp, SKBitmap trgtBmp)
    {
        int msize = 5;
        int msize2 = 2; // 5 / 2

        IntPtr srcPtr = srcBmp.GetPixels();
        IntPtr trgtPtr = trgtBmp.GetPixels();
        byte* srcBytes = (byte*)srcPtr;
        byte* trgtBytes = (byte*)trgtPtr;

        int srcStride = srcBmp.RowBytes;
        int trgtStride = trgtBmp.RowBytes;
        int width = srcBmp.Width;
        int height = srcBmp.Height;
        int pixelSize = srcBmp.BytesPerPixel;

        // Convert RGB to Y (luminance) component
        IntPtr hglobal = Marshal.AllocHGlobal(width * height);
        byte* Y = (byte*)hglobal;
        var Yindex = Y;

        for (int row = 0; row < height; row++)
        {
            byte* srcRow = srcBytes + (row * srcStride);
            for (int col = 0; col < width; col++)
            {
                int offset = col * pixelSize;
                // Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601
                // For BGRA: B=0, G=1, R=2
                Yindex[col] = (byte)(0.114 * srcRow[offset] + 0.587 * srcRow[offset + 1] + 0.299 * srcRow[offset + 2]);
            }
            Yindex += width;
        }

        Parallel.For(0, height - msize, (int row) =>
        {
            byte* srcScan0 = srcBytes + (row * srcStride);
            byte* srcScan2 = srcScan0 + (2 * srcStride);
            byte* srcScan4 = srcScan2 + (2 * srcStride);

            byte* yScan0 = Y + (row * width);
            byte* yScan2 = yScan0 + (2 * width);
            byte* yScan4 = yScan2 + (2 * width);

            byte* trgtScan0 = trgtBytes + (row * trgtStride);
            byte* trgtScan2 = trgtScan0 + (2 * trgtStride);

            for (int col = 0; col < width - msize; col++)
            {
                int offset = col * pixelSize;

                // Calculate U and V from the center pixel
                var U = 0.436 * srcScan2[offset] - 0.28886 * srcScan2[offset + 1] - 0.14713 * srcScan2[offset + 2] + 128;
                var V = -0.10001 * srcScan2[offset] - 0.51499 * srcScan2[offset + 1] + 0.615 * srcScan2[offset + 2] + 128;

                byte* m0 = yScan0 + col;
                byte* m2 = yScan2 + col;
                byte* m4 = yScan4 + col;

                double valueY = -0.1 * m0[0] + -0.1 * m0[2] + -0.1 * m0[4] +
                                -0.1 * m2[0] + 1.8 * m2[2] + -0.1 * m2[4] +
                                -0.1 * m4[0] + -0.1 * m4[2] + -0.1 * m4[4];

                valueY = valueY < 0 ? 0 : valueY;
                valueY = valueY > 255 ? 255 : valueY;

                // Convert back to RGB
                var r = valueY + 1.13983 * (V - 128);
                var g = valueY - 0.39465 * (U - 128) - 0.58060 * (V - 128);
                var b = valueY + 2.03211 * (U - 128);

                r = r < 0 ? 0 : r;
                r = r > 255 ? 255 : r;

                g = g < 0 ? 0 : g;
                g = g > 255 ? 255 : g;

                b = b < 0 ? 0 : b;
                b = b > 255 ? 255 : b;

                int targetOffset = (col + msize2) * pixelSize;
                trgtScan2[targetOffset] = (byte)b;
                trgtScan2[targetOffset + 1] = (byte)g;
                trgtScan2[targetOffset + 2] = (byte)r;
                if (pixelSize == 4)
                {
                    trgtScan2[targetOffset + 3] = srcScan2[offset + 3]; // Preserve alpha
                }
            }
        });

        Marshal.FreeHGlobal(hglobal);
    }

    /// <summary>
    /// Creates a normalized bitmap by adjusting the borders of the source bitmap according to the specified percentage.
    /// </summary>
    /// <remarks>Normalization modifies the visual appearance of the bitmap by adjusting its borders based on
    /// the specified percentage. The original bitmap remains unchanged.</remarks>
    /// <param name="srcBmp">The source bitmap to be normalized. Cannot be null.</param>
    /// <param name="borderPercent">The percentage of the border to apply during normalization, expressed as a decimal value. The default is 0.1,
    /// representing 10%. Must be greater than or equal to 0.</param>
    /// <returns>A new SKBitmap instance containing the normalized version of the source bitmap.</returns>
    public static SKBitmap Normalize(SKBitmap srcBmp, double borderPercent = 0.1)
    {
        SKBitmap trgtBmp = new SKBitmap(srcBmp.Info);
        Normalize(srcBmp, trgtBmp, borderPercent);
        return trgtBmp;
    }

    /// <summary>
    /// Normalizes the pixel values of the source bitmap and writes the result to the target bitmap, ensuring both
    /// bitmaps have the same color type and dimensions.
    /// </summary>
    /// <remarks>This method supports normalization for bitmaps with Gray8, Bgra8888, and Rgba8888 color
    /// types. Both bitmaps must be compatible in terms of color type, alpha type, and dimensions. The borderPercent
    /// parameter allows customization of the border area used in normalization, which may affect the result depending
    /// on the image content.</remarks>
    /// <param name="srcBmp">The source bitmap to normalize. Must not be null and must have the same color type and dimensions as the target
    /// bitmap.</param>
    /// <param name="trgtBmp">The target bitmap where the normalized pixel values will be written. Must not be null and must have the same
    /// color type and dimensions as the source bitmap.</param>
    /// <param name="borderPercent">The percentage of the border area to consider during normalization, expressed as a value between 0.0 and 1.0.
    /// Defaults to 0.1.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="srcBmp"/> or <paramref name="trgtBmp"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the source and target bitmaps do not have matching color types, alpha types, or dimensions, or if the
    /// color type is unsupported.</exception>
    public static unsafe void Normalize(SKBitmap srcBmp, SKBitmap trgtBmp, double borderPercent = 0.1)
    {
        if (srcBmp == null) throw new ArgumentNullException(nameof(srcBmp));
        if (trgtBmp == null) throw new ArgumentNullException(nameof(trgtBmp));

        if (srcBmp.ColorType != trgtBmp.ColorType || srcBmp.AlphaType != trgtBmp.AlphaType)
        {
            throw new ArgumentException("Source and target bitmaps must have the same color type and alpha type.");
        }

        if (srcBmp.Width != trgtBmp.Width || srcBmp.Height != trgtBmp.Height)
        {
            throw new ArgumentException("Source and target bitmaps must have the same dimensions.");
        }

        lock (srcBmp)
        {
            if (srcBmp.ColorType == SKColorType.Gray8)
            {
                NormalizeGray(srcBmp, trgtBmp);
            }
            else if (srcBmp.ColorType == SKColorType.Bgra8888 || srcBmp.ColorType == SKColorType.Rgba8888)
            {
                NormalizeColor(srcBmp, trgtBmp, borderPercent);
            }
            else
            {
                throw new ArgumentException($"Unsupported color type: {srcBmp.ColorType}");
            }
        }
    }

    /// <summary>
    /// Normalizes the grayscale values of the source bitmap and stores the result in the target bitmap.
    /// </summary>
    /// <remarks>This method adjusts the grayscale values to fit within the full range of 0 to 255, based on
    /// the minimum and maximum values found in the source bitmap. It is important that both source and target bitmaps
    /// are of the same size to avoid data corruption.</remarks>
    /// <param name="srcBmp">The source bitmap containing the grayscale values to be normalized. The bitmap must not be null and should
    /// contain valid pixel data.</param>
    /// <param name="trgtBmp">The target bitmap where the normalized grayscale values will be stored. The bitmap must not be null and should
    /// have the same dimensions as the source bitmap.</param>
    private static unsafe void NormalizeGray(SKBitmap srcBmp, SKBitmap trgtBmp)
    {
        IntPtr srcPtr = srcBmp.GetPixels();
        IntPtr trgtPtr = trgtBmp.GetPixels();
        byte* srcBytes = (byte*)srcPtr;
        byte* trgtBytes = (byte*)trgtPtr;

        int totalBytes = srcBmp.RowBytes * srcBmp.Height;

        // Find min and max values
        int min = srcBytes[0];
        int max = srcBytes[0];
        for (int i = 1; i < totalBytes; i++)
        {
            int px = srcBytes[i];
            min = px < min ? px : min;
            max = px > max ? px : max;
        }

        // Normalize
        if (max == min)
        {
            Buffer.MemoryCopy(srcBytes, trgtBytes, totalBytes, totalBytes);
            return;
        }
        double coeff = 255.0 / (max - min);
        for (int i = 0; i < totalBytes; i++)
        {
            trgtBytes[i] = (byte)((srcBytes[i] - min) * coeff);
        }
    }

    /// <summary>
    /// Normalizes the color values of the source bitmap and writes the result to the target bitmap using luminance
    /// thresholds determined by the specified border percentage.
    /// </summary>
    /// <remarks>This method processes the source bitmap by converting RGB values to luminance, building a
    /// histogram, and normalizing the color values based on calculated thresholds. It preserves the alpha channel in
    /// the target bitmap if present. Use this method to enhance image contrast while maintaining
    /// transparency.</remarks>
    /// <param name="srcBmp">The source bitmap containing the original color values to be normalized.</param>
    /// <param name="trgtBmp">The target bitmap where the normalized color values will be written.</param>
    /// <param name="borderPercent">The percentage of the total pixel count used to determine the minimum and maximum luminance thresholds for
    /// normalization. Must be between 0 and 1.</param>
    private static unsafe void NormalizeColor(SKBitmap srcBmp, SKBitmap trgtBmp, double borderPercent)
    {
        IntPtr srcPtr = srcBmp.GetPixels();
        IntPtr trgtPtr = trgtBmp.GetPixels();
        byte* srcBytes = (byte*)srcPtr;
        byte* trgtBytes = (byte*)trgtPtr;

        int srcStride = srcBmp.RowBytes;
        int trgtStride = trgtBmp.RowBytes;
        int width = srcBmp.Width;
        int height = srcBmp.Height;
        int pixelSize = srcBmp.BytesPerPixel;
        int pixelsCount = width * height;

        // Convert RGB to Y (luminance) and build histogram
        IntPtr hglobal = Marshal.AllocHGlobal(pixelsCount);
        byte* Y = (byte*)hglobal;
        var Yindex = Y;
        var hist = new int[256];

        for (int row = 0; row < height; row++)
        {
            byte* srcRow = srcBytes + (row * srcStride);
            for (int col = 0; col < width; col++)
            {
                int offset = col * pixelSize;
                // Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601
                // For BGRA: B=0, G=1, R=2
                Yindex[col] = (byte)(0.114 * srcRow[offset] + 0.587 * srcRow[offset + 1] + 0.299 * srcRow[offset + 2]);
                hist[Yindex[col]] += 1;
            }
            Yindex += width;
        }

        // Find min threshold
        var min = 255;
        var count = 0;
        var th = pixelsCount * borderPercent;
        for (int i = 0; i < hist.Length - 1; i++)
        {
            count += hist[i];
            if (count > th)
            {
                min = i;
                break;
            }
        }

        // Find max threshold
        var max = 0;
        count = 0;
        for (int i = hist.Length - 1; i > 0; i--)
        {
            count += hist[i];
            if (count > th)
            {
                max = i;
                break;
            }
        }

        // Normalize and convert back to RGB
        if (max <= min)
        {
            Marshal.FreeHGlobal(hglobal);
            return;
        }
        Parallel.For(0, height, (int row) =>
        {
            byte* srcScan0 = srcBytes + (row * srcStride);
            byte* yScan0 = Y + (row * width);
            byte* trgtScan0 = trgtBytes + (row * trgtStride);

            for (int col = 0; col < width; col++)
            {
                int offset = col * pixelSize;

                // Calculate U and V from source pixel
                var U = 0.436 * srcScan0[offset] - 0.28886 * srcScan0[offset + 1] - 0.14713 * srcScan0[offset + 2] + 128;
                var V = -0.10001 * srcScan0[offset] - 0.51499 * srcScan0[offset + 1] + 0.615 * srcScan0[offset + 2] + 128;

                // Normalize Y value
                double valueY = yScan0[col];
                valueY = (valueY - min) * 255.0 / (max - min);

                valueY = valueY < 0 ? 0 : valueY;
                valueY = valueY > 255 ? 255 : valueY;

                // Convert back to RGB
                var r = valueY + 1.13983 * (V - 128);
                var g = valueY - 0.39465 * (U - 128) - 0.58060 * (V - 128);
                var b = valueY + 2.03211 * (U - 128);

                r = r < 0 ? 0 : r;
                r = r > 255 ? 255 : r;

                g = g < 0 ? 0 : g;
                g = g > 255 ? 255 : g;

                b = b < 0 ? 0 : b;
                b = b > 255 ? 255 : b;

                trgtScan0[offset] = (byte)b;
                trgtScan0[offset + 1] = (byte)g;
                trgtScan0[offset + 2] = (byte)r;
                if (pixelSize == 4)
                {
                    trgtScan0[offset + 3] = srcScan0[offset + 3]; // Preserve alpha
                }
            }
        });

        Marshal.FreeHGlobal(hglobal);
    }

    /// <summary>
    /// Converts the specified RGB bitmap to a grayscale bitmap.
    /// </summary>
    /// <remarks>The method creates a new bitmap with the same dimensions as the source bitmap, using a
    /// grayscale color type.</remarks>
    /// <param name="srcBmp">The source bitmap to be converted to grayscale. Must not be null.</param>
    /// <returns>A new SKBitmap object representing the grayscale version of the source bitmap.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="srcBmp"/> is null.</exception>
    public static SKBitmap RgbToGray(SKBitmap srcBmp)
    {
        if (srcBmp == null) throw new ArgumentNullException(nameof(srcBmp));

        // Create grayscale target bitmap
        SKBitmap trgtBmp = new SKBitmap(srcBmp.Width, srcBmp.Height, SKColorType.Gray8, SKAlphaType.Opaque);
        RgbToGray(srcBmp, trgtBmp);
        return trgtBmp;
    }

    /// <summary>
    /// Converts a bitmap in BGRA8888 or RGBA8888 color format to its grayscale representation using the CCIR-601
    /// formula.
    /// </summary>
    /// <remarks>The grayscale conversion uses the CCIR-601 formula, which weights the red, green, and blue
    /// channels to produce a perceptually accurate grayscale image. The method does not allocate a new bitmap; it
    /// writes the result into the provided target bitmap.</remarks>
    /// <param name="srcBmp">The source bitmap to convert, which must be in BGRA8888 or RGBA8888 format and have the same dimensions as the
    /// target bitmap.</param>
    /// <param name="trgtBmp">The target bitmap that receives the grayscale output. Must be in Gray8 format and match the dimensions of the
    /// source bitmap.</param>
    /// <returns>The target bitmap containing the grayscale version of the source bitmap.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="srcBmp"/> or <paramref name="trgtBmp"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the source bitmap is not in BGRA8888 or RGBA8888 format, the target bitmap is not in Gray8 format, or
    /// if the dimensions of the source and target bitmaps do not match.</exception>
    public static unsafe SKBitmap RgbToGray(SKBitmap srcBmp, SKBitmap trgtBmp)
    {
        if (srcBmp == null) throw new ArgumentNullException(nameof(srcBmp));
        if (trgtBmp == null) throw new ArgumentNullException(nameof(trgtBmp));

        if (trgtBmp.ColorType != SKColorType.Gray8)
        {
            throw new ArgumentException("Target bitmap must be Gray8 format.");
        }

        if (srcBmp.ColorType != SKColorType.Bgra8888 && srcBmp.ColorType != SKColorType.Rgba8888)
        {
            throw new ArgumentException("Source bitmap must be Bgra8888 or Rgba8888 format.");
        }

        if (srcBmp.Width != trgtBmp.Width || srcBmp.Height != trgtBmp.Height)
        {
            throw new ArgumentException("Source and target bitmaps must have the same dimensions.");
        }

        lock (srcBmp)
        {
            IntPtr srcPtr = srcBmp.GetPixels();
            IntPtr trgtPtr = trgtBmp.GetPixels();
            byte* srcBytes = (byte*)srcPtr;
            byte* trgtBytes = (byte*)trgtPtr;

            int srcStride = srcBmp.RowBytes;
            int trgtStride = trgtBmp.RowBytes;
            int srcPixelSize = srcBmp.BytesPerPixel;
            int width = srcBmp.Width;
            int height = srcBmp.Height;

            for (int row = 0; row < height; row++)
            {
                byte* srcRow = srcBytes + (row * srcStride);
                byte* trgtRow = trgtBytes + (row * trgtStride);

                for (int col = 0; col < width; col++)
                {
                    int srcOffset = col * srcPixelSize;
                    // Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601
                    // For BGRA: B=0, G=1, R=2
                    trgtRow[col] = (byte)(0.114 * srcRow[srcOffset] + 0.587 * srcRow[srcOffset + 1] + 0.299 * srcRow[srcOffset + 2]);
                }
            }
        }

        return trgtBmp;
    }

    /// <summary>
    /// Reverses the red and blue color channels of the specified bitmap image in place.
    /// </summary>
    /// <remarks>This method modifies the pixel data of the provided bitmap directly. The operation is
    /// thread-safe as the bitmap is locked during processing.</remarks>
    /// <param name="bmp">The bitmap image to process. Must not be null and must have a color type of SKColorType.Bgra8888.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="bmp"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="bmp"/>'s color type is not SKColorType.Bgra8888.</exception>
    public static unsafe void RgbReverse(SKBitmap bmp)
    {
        if (bmp == null) throw new ArgumentNullException(nameof(bmp));
        lock (bmp)
        {
            if (bmp.ColorType != SKColorType.Bgra8888) throw new ArgumentException("Unsupported color type");

            byte* src = (byte*)bmp.GetPixels();
            int stride = bmp.RowBytes;
            int width = bmp.Width;
            int height = bmp.Height;

            Parallel.For(0, height, row =>
            {
                byte* pixel = src + row * stride;
                byte* end = pixel + width * 4;
                while (pixel < end)
                {
                    (pixel[2], pixel[0]) = (pixel[0], pixel[2]);
                    pixel += 4;
                }
            });
        }
    }

    /// <summary>
    /// Creates a new SKBitmap that is a copy of the specified source bitmap.
    /// </summary>
    /// <remarks>The source bitmap is locked during the cloning process to ensure thread safety. The cloned
    /// bitmap has the same dimensions and pixel format as the source.</remarks>
    /// <param name="srcBmp">The source SKBitmap to clone. Cannot be null.</param>
    /// <returns>A new SKBitmap instance that is an exact copy of the source bitmap. Returns null if the source bitmap is null.</returns>
    public static unsafe SKBitmap SKBitmapClone(SKBitmap srcBmp)
    {
        if (srcBmp == null) throw new ArgumentNullException(nameof(srcBmp));
        lock (srcBmp)
        {
            var skBitmapClone = new SKBitmap(srcBmp.Info);
            int srcStride = srcBmp.RowBytes;
            int dstStride = skBitmapClone.RowBytes;

            IntPtr srcPtr = srcBmp.GetPixels();
            byte* src = (byte*)srcPtr;
            IntPtr dstPtr = skBitmapClone.GetPixels();
            byte* dst = (byte*)dstPtr;
            Buffer.MemoryCopy(src, dst, skBitmapClone.Info.Height * dstStride, srcBmp.Info.Height * srcStride);
            return skBitmapClone;
        }
    }

    /// <summary>
    /// Creates a new SKBitmap instance from a pointer to pixel data, using the specified size, pixel format, and alpha
    /// type.
    /// </summary>
    /// <remarks>The method copies pixel data directly from unmanaged memory into a new SKBitmap. The caller
    /// is responsible for ensuring that the source pointer is valid and that the pixel data matches the specified
    /// format and size. Improper use may result in memory access violations.</remarks>
    /// <param name="src">A pointer to the source pixel data in memory. The data must be compatible with the specified pixel format and
    /// size.</param>
    /// <param name="size">The dimensions of the bitmap to create, represented as an SKSize. Both width and height must be positive values.</param>
    /// <param name="pixelFormat">The pixel format of the source data, specified as an SKColorType. Determines how the pixel data is interpreted.</param>
    /// <param name="alphaType">The alpha type for the bitmap, specified as an SKAlphaType. Indicates how transparency is handled in the bitmap.</param>
    /// <returns>An SKBitmap containing the pixel data copied from the specified memory location.</returns>
    public static unsafe SKBitmap SKBitmapFromIntPtr(IntPtr src, SKSize size, SKColorType pixelFormat, SKAlphaType alphaType)
    {
        SKBitmap trgtBmp = new SKBitmap((int)size.Width, (int)size.Height, pixelFormat, alphaType);
        using SKPixmap trgtPixmap = trgtBmp.PeekPixels();
        Buffer.MemoryCopy((byte*)src.ToPointer(), (byte*)trgtPixmap.GetPixels(), (ulong)(trgtPixmap.RowBytes * trgtPixmap.Height), (ulong)(trgtPixmap.RowBytes * trgtPixmap.Height));
        return trgtBmp;
    }

    /// <summary>
    /// Copies pixel data from the specified memory location into the provided SKBitmap instance.
    /// </summary>
    /// <remarks>This method performs a direct memory copy of pixel data into the target bitmap. It is
    /// important to ensure that the source data is compatible with the target bitmap's format and size to avoid data
    /// corruption or unexpected results.</remarks>
    /// <param name="src">A pointer to the source pixel data in unmanaged memory. The data must match the dimensions and pixel format of
    /// the target bitmap.</param>
    /// <param name="trgtBmp">The SKBitmap object that will be filled with the pixel data from the source pointer.</param>
    public static unsafe void FillBitmapFromIntPtr(IntPtr src, SKBitmap trgtBmp)
    {
        using SKPixmap trgtPixmap = trgtBmp.PeekPixels();
        Buffer.MemoryCopy((byte*)src.ToPointer(), (byte*)trgtPixmap.GetPixels(), (ulong)(trgtPixmap.RowBytes * trgtPixmap.Height), (ulong)(trgtPixmap.RowBytes * trgtPixmap.Height));
    }

    /// <summary>
    /// Converts the specified SKBitmap to a byte array, optionally including a header with bitmap metadata.
    /// </summary>
    /// <remarks>The header consists of 5 bytes: the pixel size (1 byte), the width (2 bytes,
    /// little-endian), and the height (2 bytes, little-endian). The position of the header within the array is
    /// determined by the value of <paramref name="headerFirst"/>.</remarks>
    /// <param name="srcBmp">The source SKBitmap to convert. This parameter must not be null.</param>
    /// <param name="headerFirst">A value indicating whether the header containing bitmap metadata is placed at the beginning of the byte
    /// array. If set to <see langword="true"/>, the header precedes the pixel data; otherwise, it is appended after
    /// the pixel data.</param>
    /// <returns>A byte array containing the pixel data of the bitmap, along with a 5-byte header that includes pixel size,
    /// width, and height information.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="srcBmp"/> is null.</exception>
    public static unsafe byte[] SKBitmapToArray(SKBitmap srcBmp, bool headerFirst = false)
    {
        if (srcBmp == null) throw new ArgumentNullException(nameof(srcBmp));

        lock (srcBmp)
        {
            using SKPixmap srcPixmap = srcBmp.PeekPixels();
            if (srcPixmap == null) throw new InvalidOperationException("Bitmap has no pixel data.");

            int pixelSize = srcBmp.BytesPerPixel;
            int rowStride = srcBmp.Width * pixelSize; // tight stride (no padding) for serialization
            int pixelDataLength = rowStride * srcBmp.Height;
            int headerOffset = headerFirst ? 0 : pixelDataLength;
            int dataOffset = headerFirst ? 5 : 0;
            byte[] trgtData = new byte[pixelDataLength + 5];
            fixed (byte* trgtDataFixed = trgtData)
            {
                byte* srcBase = (byte*)srcPixmap.GetPixels();
                for (int row = 0; row < srcBmp.Height; row++)
                {
                    byte* srcRow = srcBase + row * srcPixmap.RowBytes;
                    byte* dstRow = trgtDataFixed + dataOffset + row * rowStride;
                    Buffer.MemoryCopy(srcRow, dstRow, rowStride, rowStride);
                }
            }
            trgtData[0 + headerOffset] = (byte)pixelSize;
            trgtData[1 + headerOffset] = (byte)((srcBmp.Width >> 0) & 0xFF);
            trgtData[2 + headerOffset] = (byte)((srcBmp.Width >> 8) & 0xFF);
            trgtData[3 + headerOffset] = (byte)((srcBmp.Height >> 0) & 0xFF);
            trgtData[4 + headerOffset] = (byte)((srcBmp.Height >> 8) & 0xFF);

            return trgtData;
        }
    }

    /// <summary>
    /// Creates an SKBitmap from a byte array containing image pixel data and header information.
    /// </summary>
    /// <remarks>The method supports images with pixel formats of 1 byte per pixel (grayscale) and 4
    /// bytes per pixel (BGRA). The byte array must be structured so that the header and pixel data are correctly
    /// positioned according to the headerFirst parameter. Supplying an unsupported pixel format or an incorrectly
    /// structured array will result in an exception.</remarks>
    /// <param name="srcData">The byte array that contains both the image header and pixel data. The header must include the pixel size
    /// and the width and height of the image, encoded in the first or last five bytes depending on the value of
    /// headerFirst.</param>
    /// <param name="headerFirst">A value indicating whether the header information is located at the beginning of the byte array. If set to
    /// true, the header precedes the pixel data; if false, the header follows the pixel data.</param>
    /// <returns>An SKBitmap instance representing the image constructed from the provided byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown if srcData is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the pixel format specified in the header is not supported.</exception>
    public static unsafe SKBitmap ArrayToSKBitmap(byte[] srcData, bool headerFirst = false)
    {
        if (srcData == null) throw new ArgumentNullException(nameof(srcData));

        int pixelDataLength = srcData.Length - 5;
        int headerOffset = headerFirst ? 0 : pixelDataLength;
        int dataOffset = headerFirst ? 5 : 0;
        int pixelSize = srcData[0 + headerOffset];
        int w = (int)(srcData[1 + headerOffset] | (srcData[2 + headerOffset] << 8));
        int h = (int)(srcData[3 + headerOffset] | (srcData[4 + headerOffset] << 8));
        int rowStride = w * pixelSize;
        if (rowStride <= 0 || h <= 0) throw new ArgumentException("Invalid bitmap dimensions in header.");
        if (pixelDataLength < rowStride * h) throw new ArgumentException("Pixel data is smaller than expected for provided dimensions.");

        SKColorType pixelFormat;
        switch (pixelSize)
        {
            case 1:
                {
                    pixelFormat = SKColorType.Gray8;
                    break;
                }
            case 4:
                {
                    pixelFormat = SKColorType.Bgra8888;
                    break;
                }
            default:
                {
                    throw new ArgumentException("Unsupported pixel format");
                }
        }

        var alphaType = pixelFormat == SKColorType.Gray8 ? SKAlphaType.Opaque : SKAlphaType.Premul;
        SKBitmap trgtBmp = new SKBitmap(w, h, pixelFormat, alphaType);
        using var trgtBmd = trgtBmp.PeekPixels();
        if (trgtBmd == null) throw new InvalidOperationException("Failed to access target bitmap pixels.");
        fixed (byte* srcDataFixed = srcData)
        {
            byte* srcBase = (byte*)srcDataFixed + dataOffset;
            byte* dstBase = (byte*)trgtBmd.GetPixels();
            for (int row = 0; row < h; row++)
            {
                byte* srcRow = srcBase + row * rowStride;
                byte* dstRow = dstBase + row * trgtBmd.RowBytes;
                Buffer.MemoryCopy(srcRow, dstRow, rowStride, rowStride);
            }
        }
        return trgtBmp;
    }

    /// <summary>
    /// Samples a grayscale SKBitmap at regular intervals and returns the luminance values as a byte array.
    /// </summary>
    /// <param name="srcBmp">The source grayscale bitmap to sample. Must be Gray8 format.</param>
    /// <param name="step">The sampling interval in pixels. A step of 2 will sample every other pixel.</param>
    /// <returns>A byte array containing sampled luminance values.</returns>
    public static unsafe byte[] SKBitmapProbeGray(SKBitmap srcBmp, int step)
    {
        if (srcBmp == null) throw new ArgumentNullException(nameof(srcBmp));
        if (srcBmp.ColorType != SKColorType.Gray8)
        {
            throw new ArgumentException("Source bitmap must be Gray8 format.");
        }

        lock (srcBmp)
        {
            IntPtr srcPtr = srcBmp.GetPixels();
            byte* srcBytes = (byte*)srcPtr;
            int srcStride = srcBmp.RowBytes;
            int width = srcBmp.Width;
            int height = srcBmp.Height;

            int targetW = 1 + ((width - 1) / step);
            int targetH = 1 + ((height - 1) / step);
            byte[] trgtData = new byte[targetW * targetH];
            int t = 0;

            for (int row = 0; row < height; row += step)
            {
                byte* srcRow = srcBytes + (row * srcStride);
                for (int col = 0; col < width; col += step)
                {
                    trgtData[t++] = srcRow[col];
                }
            }

            return trgtData;
        }
    }

    /// <summary>
    /// Samples a color SKBitmap at regular intervals, converts to grayscale, and returns luminance values as a byte array.
    /// </summary>
    /// <param name="srcBmp">The source color bitmap to sample. Must be Bgra8888 or Rgba8888 format.</param>
    /// <param name="step">The sampling interval in pixels. A step of 2 will sample every other pixel.</param>
    /// <returns>A byte array containing sampled luminance values converted using CCIR-601 formula.</returns>
    public static unsafe byte[] SKBitmapProbeColor(SKBitmap srcBmp, int step)
    {
        if (srcBmp == null) throw new ArgumentNullException(nameof(srcBmp));
        if (srcBmp.ColorType != SKColorType.Bgra8888 && srcBmp.ColorType != SKColorType.Rgba8888)
        {
            throw new ArgumentException("Source bitmap must be Bgra8888 or Rgba8888 format.");
        }

        lock (srcBmp)
        {
            IntPtr srcPtr = srcBmp.GetPixels();
            byte* srcBytes = (byte*)srcPtr;
            int srcStride = srcBmp.RowBytes;
            int pixelSize = srcBmp.BytesPerPixel;
            int width = srcBmp.Width;
            int height = srcBmp.Height;

            int targetW = 1 + ((width - 1) / step);
            int targetH = 1 + ((height - 1) / step);
            byte[] trgtData = new byte[targetW * targetH];
            int t = 0;

            for (int row = 0; row < height; row += step)
            {
                byte* srcRow = srcBytes + (row * srcStride);
                for (int col = 0; col < width; col += step)
                {
                    int offset = col * pixelSize;
                    // Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601
                    // For BGRA: B=0, G=1, R=2
                    trgtData[t++] = (byte)(0.114 * srcRow[offset] + 0.587 * srcRow[offset + 1] + 0.299 * srcRow[offset + 2]);
                }
            }

            return trgtData;
        }
    }

    /// <summary>
    /// Samples a color SKBitmap at regular intervals and returns full RGB color data as a byte array.
    /// </summary>
    /// <param name="srcBmp">The source color bitmap to sample. Must be Bgra8888 or Rgba8888 format.</param>
    /// <param name="step">The sampling interval in pixels. A step of 2 will sample every other pixel.</param>
    /// <returns>A byte array containing RGB triplets (3 bytes per sampled pixel) in R,G,B order.</returns>
    public static unsafe byte[] SKBitmapProbeColorRgb(SKBitmap srcBmp, int step)
    {
        if (srcBmp == null) throw new ArgumentNullException(nameof(srcBmp));
        if (srcBmp.ColorType != SKColorType.Bgra8888 && srcBmp.ColorType != SKColorType.Rgba8888)
        {
            throw new ArgumentException("Source bitmap must be Bgra8888 or Rgba8888 format.");
        }

        lock (srcBmp)
        {
            IntPtr srcPtr = srcBmp.GetPixels();
            byte* srcBytes = (byte*)srcPtr;
            int srcStride = srcBmp.RowBytes;
            int pixelSize = srcBmp.BytesPerPixel;
            int width = srcBmp.Width;
            int height = srcBmp.Height;

            int targetW = 1 + ((width - 1) / step);
            int targetH = 1 + ((height - 1) / step);
            byte[] trgtData = new byte[targetW * targetH * 3]; // Always RGB
            int t = 0;

            for (int row = 0; row < height; row += step)
            {
                byte* srcRow = srcBytes + (row * srcStride);
                for (int col = 0; col < width; col += step)
                {
                    int offset = col * pixelSize;
                    // Output order: R, G, B
                    // For BGRA: B=0, G=1, R=2
                    trgtData[t] = srcRow[offset + 2];     // R
                    trgtData[t + 1] = srcRow[offset + 1]; // G
                    trgtData[t + 2] = srcRow[offset];     // B
                    t += 3;
                }
            }

            return trgtData;
        }
    }

    /// <summary>
    /// Determines whether the specified bitmap, presumed to contain a JPEG image, has been decoded correctly by
    /// analyzing pixel data consistency across JPEG blocks.
    /// </summary>
    /// <remarks>This method checks for differences in pixel data across multiple rows within each JPEG block
    /// to detect potential decoding errors. It is intended for use with bitmaps that originate from JPEG images and may
    /// not be applicable to other image formats.</remarks>
    /// <param name="srcBmp">The source bitmap containing the JPEG image to validate. This parameter must not be null.</param>
    /// <returns>true if the JPEG image appears to be decoded correctly; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown if srcBmp is null.</exception>
    public static unsafe bool JpegDecodedOK(SKBitmap srcBmp)
    {
        if (srcBmp == null) throw new ArgumentNullException(nameof(srcBmp));

        lock (srcBmp)
        {
            bool checkResult = true;
            int jpegBlockSize = 8;
            using var srcBmd = srcBmp.PeekPixels();
            int srcPixelSize = srcBmp.BytesPerPixel;
            int fullJpegBlocksH = srcBmd.Height / jpegBlockSize; // Количество полных JPEG-блоков по ширине
            int fullJpegBlocksW = srcBmd.Width / jpegBlockSize; // Количество полных JPEG-блоков по ширине
            int maxCol = fullJpegBlocksW * jpegBlockSize * srcPixelSize; // Максимальное значение столбца
            for (int jpegBlockH = 0; jpegBlockH < fullJpegBlocksH - 2; jpegBlockH++)
            {
                var jpegRowA = (jpegBlockH + 0) * jpegBlockSize;
                var jpegRowB = (jpegBlockH + 1) * jpegBlockSize;
                var jpegRowC = (jpegBlockH + 2) * jpegBlockSize;

                byte* srcBytesA = (byte*)srcBmd.GetPixels() + srcBmd.RowBytes * jpegRowA;
                byte* srcBytesB = (byte*)srcBmd.GetPixels() + srcBmd.RowBytes * jpegRowB;
                byte* srcBytesC = (byte*)srcBmd.GetPixels() + srcBmd.RowBytes * jpegRowC;
                var diffDetected = false; // Сброс флага для работы с очередной JPEG-строкой
                for (int jpegBlockRow = 0; jpegBlockRow < jpegBlockSize; jpegBlockRow++) // Проходим по всем строкам блоков JPEG
                {
                    for (int col = 0; col < maxCol; col++) // Работаем только с полными блоками JPEG
                    {
                        // Элементы всех трех строк JPEG должны быть различны
                        if ((srcBytesA[col] != srcBytesB[col]) && (srcBytesA[col] != srcBytesC[col]) && (srcBytesB[col] != srcBytesC[col]))
                        {
                            diffDetected = true; // Обнаружено отличие между JPEG-строками...
                            jpegBlockRow = jpegBlockSize; //...отключаем внешний цикл...
                            break; //...и выходим
                        }
                    }
                    srcBytesA += srcBmd.RowBytes;
                    srcBytesB += srcBmd.RowBytes;
                    srcBytesC += srcBmd.RowBytes;
                }
                if (!diffDetected) // Если не обнаружено различий в данных JPEG-блока - проверка провалена
                {
                    checkResult = false;
                    break;
                }
            }
            return checkResult;
        }
    }

    /// <summary>
    /// Calculates a hash value for the specified bitmap by sampling pixel data at defined intervals.
    /// </summary>
    /// <remarks>The method locks the bitmap during processing to ensure thread safety. The resulting hash
    /// value depends on the sampling interval specified by the <paramref name="step"/> parameter; using a larger step
    /// may result in a less precise hash.</remarks>
    /// <param name="srcBmp">The source bitmap from which to compute the hash. This parameter must not be null.</param>
    /// <param name="step">The interval, in bytes, at which to sample pixel values. Must be a positive integer.</param>
    /// <returns>A hash value represented as an unsigned long integer, derived from the sampled pixel data of the bitmap.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="srcBmp"/> is null.</exception>
    public static unsafe ulong SKBitmapHash(SKBitmap srcBmp, int step)
    {
        if (srcBmp == null) throw new ArgumentNullException(nameof(srcBmp));
        lock (srcBmp)
        {
            using SKPixmap srcPixmap = srcBmp.PeekPixels();
            ulong hash = 0;
            byte* srcBytes = (byte*)srcPixmap.GetPixels();
            int width = srcBmp.Width;
            int height = srcBmp.Height;
            int rowBytes = srcPixmap.RowBytes;

            if (srcBmp.ColorType == SKColorType.Gray8)
            {
                for (int row = 0; row < height; row += step)
                {
                    byte* srcRow = srcBytes + (row * rowBytes);
                    for (int col = 0; col < width; col += step)
                    {
                        hash += srcRow[col];
                    }
                }

                return hash;
            }

            if (srcBmp.ColorType == SKColorType.Bgra8888 || srcBmp.ColorType == SKColorType.Rgba8888)
            {
                int pixelSize = srcBmp.BytesPerPixel;
                for (int row = 0; row < height; row += step)
                {
                    byte* srcRow = srcBytes + (row * rowBytes);
                    for (int col = 0; col < width; col += step)
                    {
                        int offset = col * pixelSize;
                        hash += (byte)(0.114 * srcRow[offset] + 0.587 * srcRow[offset + 1] + 0.299 * srcRow[offset + 2]);
                    }
                }

                return hash;
            }

            for (int i = 0; i < srcPixmap.RowBytes * srcPixmap.Height; i += step)
                hash += srcBytes[i];
            return hash;
        }
    }


    /// <summary>
    /// Calculates a hash value for a bitmap represented in unmanaged memory.
    /// </summary>
    /// <remarks>This method is intended for scenarios where bitmap data is accessed directly in unmanaged
    /// memory. The hash is computed by sampling bytes at the specified interval, which can be used for quick
    /// comparisons or change detection. The accuracy and uniqueness of the hash depend on the chosen step
    /// value.</remarks>
    /// <param name="src">A pointer to the bitmap data in unmanaged memory. This must reference a valid memory location containing the
    /// bitmap's bytes.</param>
    /// <param name="size">The dimensions of the bitmap, specified as an SKSize structure representing the width and height.</param>
    /// <param name="rowBytes">The number of bytes in a single row of the bitmap, including any padding. Must be greater than zero.</param>
    /// <param name="step">The interval, in bytes, at which to sample the bitmap data when computing the hash. Must be greater than zero.</param>
    /// <returns>A 64-bit unsigned integer representing the computed hash value of the bitmap data.</returns>
    public static unsafe ulong SKBitmapHashIntPtr(IntPtr src, SKSize size, int rowBytes, int step)
    {
        ulong hash = 0;
        byte* srcBytes = (byte*)src.ToPointer();
        for (int i = 0; i < rowBytes * size.Height; i += step) hash += srcBytes[i];
        return hash;
    }
}
