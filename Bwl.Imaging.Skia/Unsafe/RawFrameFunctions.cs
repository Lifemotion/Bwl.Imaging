using SkiaSharp;
using System;
using System.Threading.Tasks;

namespace Bwl.Imaging.Skia
{
    /// <summary>
    /// Provides functions for converting raw HDR (High Dynamic Range) frame data to displayable SKBitmap images.
    /// </summary>
    public static class RawFrameFunctions
    {
        /// <summary>
        /// Converts raw HDR frame data to an SKBitmap with gain adjustment and tone mapping.
        /// </summary>
        /// <param name="data">Raw pixel data as int array in RGB format (B, G, R triplets).</param>
        /// <param name="width">Width of the image in pixels.</param>
        /// <param name="height">Height of the image in pixels.</param>
        /// <param name="baseGain">Base gain value for exposure adjustment. Values &lt;= 4 reduce exposure, values &gt; 4 increase exposure.</param>
        /// <returns>An SKBitmap containing the tone-mapped HDR image.</returns>
        public static unsafe SKBitmap ConvertRawToHDRBitmap1(int[] data, int width, int height, int baseGain)
        {
            SKBitmap trgtBmp = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Opaque);

            fixed (int* srcInts = data)
            {
                byte* trgtBytes = (byte*)trgtBmp.GetPixels();
                int r, g, b;
                int koeff = 14;

                for (int pixelIdx = 0; pixelIdx < width * height; pixelIdx++)
                {
                    int srcIdx = pixelIdx * 3;
                    int dstIdx = pixelIdx * 4;

                    b = srcInts[srcIdx + 0];
                    g = srcInts[srcIdx + 1];
                    r = srcInts[srcIdx + 2];

                    if (baseGain <= 4)
                    {
                        r = r >> (4 - baseGain);
                        g = g >> (4 - baseGain);
                        b = b >> (4 - baseGain);
                    }
                    else
                    {
                        r = r << (baseGain - 4);
                        g = g << (baseGain - 4);
                        b = b << (baseGain - 4);
                    }

                    while ((r > 255 | g > 255 | b > 255))
                    {
                        r = (r * koeff) >> 4;
                        g = (g * koeff) >> 4;
                        b = (b * koeff) >> 4;
                    }

                    trgtBytes[dstIdx + 0] = (byte)b;
                    trgtBytes[dstIdx + 1] = (byte)g;
                    trgtBytes[dstIdx + 2] = (byte)r;
                    trgtBytes[dstIdx + 3] = 255; // Alpha
                }
            }

            return trgtBmp;
        }

        /// <summary>
        /// Optimized version of ConvertRawToHDRBitmap1 with conditional logic moved outside the main loop.
        /// </summary>
        /// <param name="data">Raw pixel data as int array in RGB format (B, G, R triplets).</param>
        /// <param name="width">Width of the image in pixels.</param>
        /// <param name="height">Height of the image in pixels.</param>
        /// <param name="baseGain">Base gain value for exposure adjustment. Values &lt;= 4 reduce exposure, values &gt; 4 increase exposure.</param>
        /// <returns>An SKBitmap containing the tone-mapped HDR image.</returns>
        public static unsafe SKBitmap ConvertRawToHDRBitmap1Fast(int[] data, int width, int height, int baseGain)
        {
            SKBitmap trgtBmp = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Opaque);

            fixed (int* srcInts = data)
            {
                byte* trgtBytes = (byte*)trgtBmp.GetPixels();
                int r, g, b;
                int k = 14;

                if (baseGain <= 4)
                {
                    int bitShift = 4 - baseGain;
                    for (int pixelIdx = 0; pixelIdx < width * height; pixelIdx++)
                    {
                        int srcIdx = pixelIdx * 3;
                        int dstIdx = pixelIdx * 4;

                        b = srcInts[srcIdx + 0] >> bitShift;
                        g = srcInts[srcIdx + 1] >> bitShift;
                        r = srcInts[srcIdx + 2] >> bitShift;

                        while (r > 255 | g > 255 | b > 255)
                        {
                            r = (r * k) >> 4;
                            g = (g * k) >> 4;
                            b = (b * k) >> 4;
                        }

                        trgtBytes[dstIdx + 0] = (byte)b;
                        trgtBytes[dstIdx + 1] = (byte)g;
                        trgtBytes[dstIdx + 2] = (byte)r;
                        trgtBytes[dstIdx + 3] = 255; // Alpha
                    }
                }
                else
                {
                    int bitShift = baseGain - 4;
                    for (int pixelIdx = 0; pixelIdx < width * height; pixelIdx++)
                    {
                        int srcIdx = pixelIdx * 3;
                        int dstIdx = pixelIdx * 4;

                        b = srcInts[srcIdx + 0] << bitShift;
                        g = srcInts[srcIdx + 1] << bitShift;
                        r = srcInts[srcIdx + 2] << bitShift;

                        while (r > 255 | g > 255 | b > 255)
                        {
                            r = (r * k) >> 4;
                            g = (g * k) >> 4;
                            b = (b * k) >> 4;
                        }

                        trgtBytes[dstIdx + 0] = (byte)b;
                        trgtBytes[dstIdx + 1] = (byte)g;
                        trgtBytes[dstIdx + 2] = (byte)r;
                        trgtBytes[dstIdx + 3] = 255; // Alpha
                    }
                }
            }

            return trgtBmp;
        }

        private static byte[] _powTableHDR3;

        /// <summary>
        /// Initializes the lookup table for HDR tone mapping using a power function.
        /// </summary>
        private static void InitPowTableHDR3()
        {
            _powTableHDR3 = new byte[(1 << 12)];
            var v = 0.4;
            var k = 12;
            for (var powArg = 0; powArg < _powTableHDR3.Length; powArg++)
            {
                _powTableHDR3[powArg] = Convert.ToByte(Math.Min(Math.Pow(powArg, v) * k, 255));
            }
        }

        /// <summary>
        /// Fast HDR conversion using a pre-computed power function lookup table with parallel processing.
        /// </summary>
        /// <param name="data">Raw pixel data as int array in RGB format (B, G, R triplets).</param>
        /// <param name="width">Width of the image in pixels.</param>
        /// <param name="height">Height of the image in pixels.</param>
        /// <returns>An SKBitmap containing the tone-mapped HDR image.</returns>
        public static unsafe SKBitmap ConvertRawToHDRBitmap3Fast(int[] data, int width, int height)
        {
            if (_powTableHDR3 == null) InitPowTableHDR3();

            SKBitmap trgtBmp = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Opaque);

            fixed (int* srcIntsFixed = data)
            {
                int* srcInts = srcIntsFixed;
                byte* trgtBytes = (byte*)trgtBmp.GetPixels();

                // Process RGB channels in parallel
                Parallel.For(0, 3, (int channel) =>
                {
                    for (int pixelIdx = 0; pixelIdx < width * height; pixelIdx++)
                    {
                        int srcIdx = pixelIdx * 3 + channel;
                        int dstIdx = pixelIdx * 4 + channel;
                        trgtBytes[dstIdx] = _powTableHDR3[srcInts[srcIdx]];
                    }
                });

                // Set alpha channel
                for (int pixelIdx = 0; pixelIdx < width * height; pixelIdx++)
                {
                    trgtBytes[pixelIdx * 4 + 3] = 255;
                }
            }

            return trgtBmp;
        }
    }
}
