using System;
using System.Threading;
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

        public static Bitmap Sharpen5Gray(Bitmap scrBmp)
        {
            if (scrBmp == null)
            {
                return null;
            }
            lock (scrBmp)
            {
                int pixelSize = GetPixelSize(scrBmp.PixelFormat);
                if (pixelSize == 1)
                {
                    BitmapData srcBmd = scrBmp.LockBits(new Rectangle(0, 0, scrBmp.Width, scrBmp.Height), ImageLockMode.ReadOnly, scrBmp.PixelFormat);
                    Bitmap trgtBmp = new Bitmap(scrBmp.Width, scrBmp.Height, scrBmp.PixelFormat);
                    trgtBmp.Palette = GetGrayScalePalette();
                    BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, scrBmp.Width, scrBmp.Height), ImageLockMode.WriteOnly, scrBmp.PixelFormat);
                    unsafe
                    {
                        int msize = 5;
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        byte* trgtBytes = (byte*)trgtBmd.Scan0;
                        for (int row = 0; row < (srcBmd.Height - msize); row++)
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
                                               -0.1 * m2[0] +  1.8 * m2[2] + -0.1 * m2[4] +
                                               -0.1 * m4[0] + -0.1 * m4[2] + -0.1 * m4[4];
                                value = value < 0 ? 0 : value;
                                value = value > 255 ? 255 : value;
                                t2[2] = (byte)value;
                            }
                        }
                    }
                    scrBmp.UnlockBits(srcBmd);
                    trgtBmp.UnlockBits(trgtBmd);

                    return trgtBmp;
                } else
                {
                    return null;
                }
            }
        }

        public static Bitmap NormalizeGray(Bitmap scrBmp)
        {
            if (scrBmp == null)
            {
                return null;
            }
            lock (scrBmp)
            {
                int pixelSize = GetPixelSize(scrBmp.PixelFormat);
                if (pixelSize == 1)
                {
                    BitmapData srcBmd = scrBmp.LockBits(new Rectangle(0, 0, scrBmp.Width, scrBmp.Height), ImageLockMode.ReadOnly, scrBmp.PixelFormat);
                    Bitmap trgtBmp = new Bitmap(scrBmp.Width, scrBmp.Height, scrBmp.PixelFormat);
                    trgtBmp.Palette = GetGrayScalePalette();
                    BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, scrBmp.Width, scrBmp.Height), ImageLockMode.WriteOnly, scrBmp.PixelFormat);
                    unsafe
                    {
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        byte* trgtBytes = (byte*)trgtBmd.Scan0;
                        int min = srcBytes[0];
                        int max = srcBytes[0];
                        for (int i = 1; i < srcBmd.Width * srcBmd.Height * pixelSize; i++)
                        {
                            int px = srcBytes[i];
                            min = px < min ? px : min;
                            max = px > max ? px : max;
                        }
                        double coeff = 255 / (double)(max - min);
                        for (int i = 0; i < srcBmd.Width * srcBmd.Height * pixelSize; i++)
                        {
                            trgtBytes[i] = (byte)((srcBytes[i] - min) * coeff);
                        }
                    }
                    scrBmp.UnlockBits(srcBmd);
                    trgtBmp.UnlockBits(trgtBmd);

                    return trgtBmp;
                } else
                {
                    return null;
                }
            }
        }

        public static Bitmap BitmapClone(Bitmap scrBmp)
        {
            if (scrBmp == null)
            {
                return null;
            }
            lock (scrBmp)
            {
                int pixelSize = GetPixelSize(scrBmp.PixelFormat);
                if (pixelSize != 0)
                {
                    BitmapData srcBmd = scrBmp.LockBits(new Rectangle(0, 0, scrBmp.Width, scrBmp.Height), ImageLockMode.ReadOnly, scrBmp.PixelFormat);
                    Bitmap trgtBmp = new Bitmap(scrBmp.Width, scrBmp.Height, scrBmp.PixelFormat);
                    if (pixelSize == 1)
                    {
                        trgtBmp.Palette = GetGrayScalePalette();
                    }
                    BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, scrBmp.Width, scrBmp.Height), ImageLockMode.WriteOnly, scrBmp.PixelFormat);
                    unsafe
                    {
                        memcpy((byte*)trgtBmd.Scan0, (byte*)srcBmd.Scan0, (ulong)(srcBmd.Width * srcBmd.Height * pixelSize));
                    }
                    scrBmp.UnlockBits(srcBmd);
                    trgtBmp.UnlockBits(trgtBmd);
                    return trgtBmp;
                }
                else
                {
                    return new Bitmap(scrBmp);
                }
            }
        }

        public static ulong BitmapHash(Bitmap scrBmp)
        {
            if (scrBmp == null)
            {
                return 0;
            }
            lock (scrBmp)
            {
                int pixelSize = GetPixelSize(scrBmp.PixelFormat);
                if (pixelSize != 0)
                {
                    BitmapData srcBmd = scrBmp.LockBits(new Rectangle(0, 0, scrBmp.Width, scrBmp.Height), ImageLockMode.ReadOnly, scrBmp.PixelFormat);
                    long hash = 0;
                    unsafe
                    {
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        for (int i = 0; i < srcBmd.Width * srcBmd.Height * pixelSize; i++)
                        {
                            Interlocked.Add(ref hash, (long)srcBytes[i]);
                        }
                    }
                    scrBmp.UnlockBits(srcBmd);
                    return (ulong)hash;
                }
                else
                {
                    return 0;
                }
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
                        break;
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
