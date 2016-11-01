using System;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security;

namespace Bwl.Imaging.Unsafe
{
    public static class UnsafeFunctions
    {
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false), SuppressUnmanagedCodeSecurity]
        public static unsafe extern void* memcpy(void* dest, void* src, ulong count);

        public static Bitmap RgbToGray(Bitmap srcBmp)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            lock (srcBmp)
            {
                if (srcBmp.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                    Bitmap trgtBmp = new Bitmap(srcBmp.Width, srcBmp.Height, PixelFormat.Format8bppIndexed);
                    trgtBmp.Palette = GetGrayScalePalette();
                    BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
                    unsafe
                    {
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        byte* trgtBytes = (byte*)trgtBmd.Scan0;
                        for (int i = 0, j = 0; i < srcBmd.Width * srcBmd.Height; i++, j += 3)
                        {
                            trgtBytes[i] = (byte)(0.071 * srcBytes[j] + 0.707 * srcBytes[j + 1] + 0.222 * srcBytes[j + 2]);
                        }
                    }
                    srcBmp.UnlockBits(srcBmd);
                    trgtBmp.UnlockBits(trgtBmd);

                    return trgtBmp;
                }
                else
                {
                    throw new Exception("Unsupported pixel format");
                }
            }
        }

        public static Bitmap Sharpen5Gray(Bitmap srcBmp)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            lock (srcBmp)
            {
                if (srcBmp.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                    Bitmap trgtBmp = new Bitmap(srcBmp.Width, srcBmp.Height, srcBmp.PixelFormat);
                    trgtBmp.Palette = GetGrayScalePalette();
                    BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
                    unsafe
                    {
                        int msize = 5;
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        byte* trgtBytes = (byte*)trgtBmd.Scan0;
                        Parallel.For(0, (srcBmd.Height - msize), (int row) =>
                        {
                            byte* srcScan = srcBytes + (row * srcBmd.Width);
                            byte* srcScan2 = srcScan + (2 * srcBmd.Width);
                            byte* srcScan4 = srcScan + (4 * srcBmd.Width);

                            byte* trgtScan = trgtBytes + (row * trgtBmd.Width);

                            for (int col = 0; col < (srcBmd.Width - msize); col++)
                            {
                                byte* m0 = srcScan + col;
                                byte* m2 = srcScan + col;
                                byte* m4 = srcScan + col;
                                byte* t2 = trgtScan + col;
                                double value = -0.1 * m0[0] + -0.1 * m0[2] + -0.1 * m0[4] +
                                               -0.1 * m2[0] + 1.8 * m2[2] + -0.1 * m2[4] +
                                               -0.1 * m4[0] + -0.1 * m4[2] + -0.1 * m4[4];
                                value = value < 0 ? 0 : value;
                                value = value > 255 ? 255 : value;
                                t2[2] = (byte)value;
                            }
                        });
                    }
                    srcBmp.UnlockBits(srcBmd);
                    trgtBmp.UnlockBits(trgtBmd);

                    return trgtBmp;
                }
                else
                {
                    throw new Exception("Unsupported pixel format");
                }
            }
        }

        public static Bitmap NormalizeGray(Bitmap srcBmp)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            lock (srcBmp)
            {
                if (srcBmp.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                    Bitmap trgtBmp = new Bitmap(srcBmp.Width, srcBmp.Height, srcBmp.PixelFormat);
                    trgtBmp.Palette = GetGrayScalePalette();
                    BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
                    unsafe
                    {
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        byte* trgtBytes = (byte*)trgtBmd.Scan0;
                        int min = srcBytes[0];
                        int max = srcBytes[0];
                        for (int i = 1; i < srcBmd.Width * srcBmd.Height; i++)
                        {
                            int px = srcBytes[i];
                            min = px < min ? px : min;
                            max = px > max ? px : max;
                        }
                        double coeff = 255 / (double)(max - min);
                        for (int i = 0; i < srcBmd.Width * srcBmd.Height; i++)
                        {
                            trgtBytes[i] = (byte)((srcBytes[i] - min) * coeff);
                        }
                    }
                    srcBmp.UnlockBits(srcBmd);
                    trgtBmp.UnlockBits(trgtBmd);

                    return trgtBmp;
                }
                else
                {
                    throw new Exception("Unsupported pixel format");
                }
            }
        }

        public static Bitmap BitmapClone(Bitmap srcBmp)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            lock (srcBmp)
            {
                int pixelSize = GetPixelSize(srcBmp.PixelFormat);
                BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                Bitmap trgtBmp = new Bitmap(srcBmp.Width, srcBmp.Height, srcBmp.PixelFormat);
                if (pixelSize == 1)
                {
                    trgtBmp.Palette = GetGrayScalePalette();
                }
                BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
                unsafe
                {
                    memcpy((byte*)trgtBmd.Scan0, (byte*)srcBmd.Scan0, (ulong)(srcBmd.Width * srcBmd.Height * pixelSize));
                }
                srcBmp.UnlockBits(srcBmd);
                trgtBmp.UnlockBits(trgtBmd);
                return trgtBmp;
            }
        }

        public static ulong BitmapHash(Bitmap srcBmp)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            lock (srcBmp)
            {
                int pixelSize = GetPixelSize(srcBmp.PixelFormat);
                BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                long hash = 0;
                unsafe
                {
                    byte* srcBytes = (byte*)srcBmd.Scan0;
                    for (int i = 0; i < srcBmd.Width * srcBmd.Height * pixelSize; i++)
                    {
                        Interlocked.Add(ref hash, (long)srcBytes[i]);
                    }
                }
                srcBmp.UnlockBits(srcBmd);
                return (ulong)hash;
            }
        }

        private static int GetPixelSize(PixelFormat fmt)
        {
            int pixelSize = 0;
            switch (fmt)
            {
                case PixelFormat.Format8bppIndexed:
                    {
                        pixelSize = 1;
                        break;
                    }
                case PixelFormat.Format24bppRgb:
                    {
                        pixelSize = 3;
                        break;
                    }
                case PixelFormat.Format32bppArgb:
                    {
                        pixelSize = 4;
                        break;
                    }
                case PixelFormat.Format32bppPArgb:
                    {
                        pixelSize = 4;
                        break;
                    }
                case PixelFormat.Format32bppRgb:
                    {
                        pixelSize = 4;
                        break;
                    }
                default:
                    {
                        throw new Exception("Unsupported pixel format");
                    }
            }
            return pixelSize;
        }

        private static ColorPalette GetGrayScalePalette()
        {
            Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
            ColorPalette monoPalette = bmp.Palette;
            Color[] entries = monoPalette.Entries;
            for (int i = 0; i < 256; i++)
            {
                entries[i] = Color.FromArgb(i, i, i);
            }
            return monoPalette;
        }
    }
}
