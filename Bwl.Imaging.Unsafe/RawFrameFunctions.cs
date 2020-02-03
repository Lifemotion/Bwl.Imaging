using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Bwl.Imaging.Unsafe
{
    public static class RawFrameFunctions
    {
        public static Bitmap ConvertRawToHDRBitmap1(int[] data, int width, int height, int baseGain)
        {
            Bitmap trgtBmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);

            unsafe
            {
                fixed (int* srcInts = data)
                {
                    byte* trgtBytes = (byte*)trgtBmd.Scan0;
                    int r, g, b;
                    int koeff = 14;

                    {
                        for (int pixelAddr = 0; pixelAddr < width * height * 3; pixelAddr += 3)
                        {
                            b = srcInts[pixelAddr + 0];
                            g = srcInts[pixelAddr + 1];
                            r = srcInts[pixelAddr + 2];

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

                            trgtBytes[pixelAddr + 0] = (byte)b;
                            trgtBytes[pixelAddr + 1] = (byte)g;
                            trgtBytes[pixelAddr + 2] = (byte)r;
                        }
                    }
                }
            }

            trgtBmp.UnlockBits(trgtBmd);
            return trgtBmp;
        }

        public static Bitmap ConvertRawToHDRBitmap1Fast(int[] data, int width, int height, int baseGain)
        {
            Bitmap trgtBmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);

            unsafe
            {
                fixed (int* srcInts = data)
                {
                    byte* trgtBytes = (byte*)trgtBmd.Scan0;
                    int r, g, b;
                    int k = 14;

                    if (baseGain <= 4)
                    {
                        int bitShift = 4 - baseGain;
                        for (int pixelAddr = 0; pixelAddr < width * height * 3; pixelAddr += 3)
                        {
                            b = srcInts[pixelAddr + 0] >> bitShift;
                            g = srcInts[pixelAddr + 1] >> bitShift;
                            r = srcInts[pixelAddr + 2] >> bitShift;

                            while (r > 255 | g > 255 | b > 255)
                            {
                                r = (r * k) >> 4;
                                g = (g * k) >> 4;
                                b = (b * k) >> 4;
                            }

                            trgtBytes[pixelAddr + 0] = (byte)b;
                            trgtBytes[pixelAddr + 1] = (byte)g;
                            trgtBytes[pixelAddr + 2] = (byte)r;
                        }
                    }
                    else
                    {
                        int bitShift = baseGain - 4;
                        for (int pixelAddr = 0; pixelAddr < width * height * 3; pixelAddr += 3)
                        {
                            b = srcInts[pixelAddr + 0] << bitShift;
                            g = srcInts[pixelAddr + 1] << bitShift;
                            r = srcInts[pixelAddr + 2] << bitShift;

                            while (r > 255 | g > 255 | b > 255)
                            {
                                r = (r * k) >> 4;
                                g = (g * k) >> 4;
                                b = (b * k) >> 4;
                            }

                            trgtBytes[pixelAddr + 0] = (byte)b;
                            trgtBytes[pixelAddr + 1] = (byte)g;
                            trgtBytes[pixelAddr + 2] = (byte)r;
                        }
                    }
                }
            }

            trgtBmp.UnlockBits(trgtBmd);
            return trgtBmp;
        }

        private static byte[] _powTableHDR3;
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
        public static Bitmap ConvertRawToHDRBitmap3Fast(int[] data, int width, int height)
        {
            if (_powTableHDR3 == null) InitPowTableHDR3();
 
            Bitmap trgtBmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);

            unsafe
            {
                fixed (int* srcIntsFixed = data)
                {
                    int* srcInts = srcIntsFixed;
                    byte* trgtBytes = (byte*)trgtBmd.Scan0;
                    {
                        Parallel.For(0, 3, (int channel) =>
                        {
                            for (int pixelAddr = channel; pixelAddr < width * height * 3; pixelAddr += 3)
                            {
                                trgtBytes[pixelAddr] = _powTableHDR3[srcInts[pixelAddr]];
                            }
                        });
                    }
                }

                trgtBmp.UnlockBits(trgtBmd);
                return trgtBmp;
            }
        }
    }
}
