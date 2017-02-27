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

        public static Bitmap CropGray(Bitmap srcBmp, Rectangle region)
        {
            if (srcBmp.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                Bitmap trgtBmp = new Bitmap(srcBmp.Width, srcBmp.Height, srcBmp.PixelFormat);
                CropGray(srcBmp, trgtBmp, region);
                return trgtBmp;
            }
            else
            {
                throw new Exception("Unsupported pixel format");
            }
        }

        public static void CropGray(Bitmap srcBmp, Bitmap trgtBmp, Rectangle region)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            lock (srcBmp)
            {
                if ((srcBmp.PixelFormat == PixelFormat.Format8bppIndexed) && (trgtBmp.PixelFormat == PixelFormat.Format8bppIndexed))
                {
                    if ((trgtBmp.Width == region.Width) && (trgtBmp.Height == region.Height))
                    {
                        BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                        trgtBmp.Palette = GetGrayScalePalette();
                        BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
                        unsafe
                        {
                            int regionX = region.X;
                            int regionY = region.Y;
                            int regionW = region.Width;
                            int regionH = region.Height;
                            byte* srcBytes = (byte*)srcBmd.Scan0 + (regionY * srcBmd.Stride) + regionX;
                            byte* trgtBytes = (byte*)trgtBmd.Scan0;
                            for (int rectRow = 0; rectRow < regionH; rectRow++)
                            {
                                for (int rectCol = 0; rectCol < regionW; rectCol++)
                                {
                                    trgtBytes[rectCol] = srcBytes[rectCol];
                                }
                                srcBytes += srcBmd.Stride;
                                trgtBytes += trgtBmd.Stride;
                            }
                        }
                        srcBmp.UnlockBits(srcBmd);
                        trgtBmp.UnlockBits(trgtBmd);
                    }
                    else
                    {
                        throw new Exception("Target bitmap's size != region's size");
                    }
                }
                else
                {
                    throw new Exception("Unsupported pixel format");
                }
            }
        }

        public static Bitmap CropRgb(Bitmap srcBmp, Rectangle region)
        {
            if ((srcBmp.PixelFormat == PixelFormat.Format24bppRgb) || (srcBmp.PixelFormat == PixelFormat.Format32bppArgb))
            {
                Bitmap trgtBmp = new Bitmap(srcBmp.Width, srcBmp.Height, srcBmp.PixelFormat);
                CropRgb(srcBmp, trgtBmp, region);
                return trgtBmp;
            }
            else
            {
                throw new Exception("Unsupported pixel format");
            }
        }

        public static void CropRgb(Bitmap srcBmp, Bitmap trgtBmp, Rectangle region)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            lock (srcBmp)
            {
                if ((srcBmp.PixelFormat == trgtBmp.PixelFormat) && ((srcBmp.PixelFormat == PixelFormat.Format24bppRgb) || (srcBmp.PixelFormat == PixelFormat.Format32bppArgb)))
                {
                    if ((trgtBmp.Width == region.Width) && (trgtBmp.Height == region.Height))
                    {
                        BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                        BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
                        int pixelSize = GetPixelSize(srcBmp.PixelFormat);
                        unsafe
                        {
                            int regionX = region.X;
                            int regionY = region.Y;
                            int regionW = region.Width;
                            int regionH = region.Height;
                            byte* srcBytes = (byte*)srcBmd.Scan0 + (regionY * srcBmd.Stride) + regionX;
                            byte* trgtBytes = (byte*)trgtBmd.Scan0;
                            for (int rectRow = 0; rectRow < regionH; rectRow++)
                            {
                                for (int rectCol = 0; rectCol < regionW * pixelSize; rectCol += pixelSize)
                                {
                                    trgtBytes[rectCol] = srcBytes[rectCol];
                                    trgtBytes[rectCol + 1] = srcBytes[rectCol + 1];
                                    trgtBytes[rectCol + 2] = srcBytes[rectCol + 2];
                                }
                                srcBytes += srcBmd.Stride;
                                trgtBytes += trgtBmd.Stride;
                            }
                        }
                        srcBmp.UnlockBits(srcBmd);
                        trgtBmp.UnlockBits(trgtBmd);
                    }
                }
                else
                {
                    throw new Exception("Unsupported pixel format");
                }
            }
        }

        public static Bitmap Sharpen5Gray(Bitmap srcBmp)
        {
            if (srcBmp.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                Bitmap trgtBmp = new Bitmap(srcBmp.Width, srcBmp.Height, srcBmp.PixelFormat);
                Sharpen5Gray(srcBmp, trgtBmp);
                return trgtBmp;
            }
            else
            {
                throw new Exception("Unsupported pixel format");
            }
        }

        public static void Sharpen5Gray(Bitmap srcBmp, Bitmap trgtBmp)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            lock (srcBmp)
            {
                if ((srcBmp.PixelFormat == PixelFormat.Format8bppIndexed) && (trgtBmp.PixelFormat == PixelFormat.Format8bppIndexed))
                {
                    BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                    trgtBmp.Palette = GetGrayScalePalette();
                    BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
                    unsafe
                    {
                        int msize = 5;
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        byte* trgtBytes = (byte*)trgtBmd.Scan0;
                        Parallel.For(0, (srcBmd.Height - msize), (int row) =>
                        {
                            byte* srcScan0 = srcBytes + (row * srcBmd.Stride);
                            byte* srcScan2 = srcScan0 + (2 * srcBmd.Stride);
                            byte* srcScan4 = srcScan2 + (2 * srcBmd.Stride);

                            byte* trgtScan0 = trgtBytes + (row * trgtBmd.Stride);
                            byte* trgtScan2 = trgtScan0 + (2 * trgtBmd.Stride);

                            for (int col = 0; col < (srcBmd.Stride - msize); col++)
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
                                t2[2] = (byte)value;
                            }
                        });
                    }
                    srcBmp.UnlockBits(srcBmd);
                    trgtBmp.UnlockBits(trgtBmd);
                }
                else
                {
                    throw new Exception("Unsupported pixel format");
                }
            }
        }

        public static Bitmap NormalizeGray(Bitmap srcBmp)
        {
            if (srcBmp.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                Bitmap trgtBmp = new Bitmap(srcBmp.Width, srcBmp.Height, srcBmp.PixelFormat);
                NormalizeGray(srcBmp, trgtBmp);
                return trgtBmp;
            }
            else
            {
                throw new Exception("Unsupported pixel format");
            }
        }

        public static void NormalizeGray(Bitmap srcBmp, Bitmap trgtBmp)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            lock (srcBmp)
            {
                if ((srcBmp.PixelFormat == PixelFormat.Format8bppIndexed) && (trgtBmp.PixelFormat == PixelFormat.Format8bppIndexed))
                {
                    BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                    trgtBmp.Palette = GetGrayScalePalette();
                    BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
                    unsafe
                    {
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        byte* trgtBytes = (byte*)trgtBmd.Scan0;
                        int min = srcBytes[0];
                        int max = srcBytes[0];
                        for (int i = 1; i < srcBmd.Stride * srcBmd.Height; i++)
                        {
                            int px = srcBytes[i];
                            min = px < min ? px : min;
                            max = px > max ? px : max;
                        }
                        double coeff = 255 / (double)(max - min);
                        for (int i = 0; i < srcBmd.Stride * srcBmd.Height; i++)
                        {
                            trgtBytes[i] = (byte)((srcBytes[i] - min) * coeff);
                        }
                    }
                    srcBmp.UnlockBits(srcBmd);
                    trgtBmp.UnlockBits(trgtBmd);
                }
                else
                {
                    throw new Exception("Unsupported pixel format");
                }
            }
        }

        public static Bitmap RgbToGray(Bitmap srcBmp)
        {
            if ((srcBmp.PixelFormat == PixelFormat.Format24bppRgb) || (srcBmp.PixelFormat == PixelFormat.Format32bppArgb))
            {
                Bitmap trgtBmp = new Bitmap(srcBmp.Width, srcBmp.Height, PixelFormat.Format8bppIndexed);
                RgbToGray(srcBmp, trgtBmp);
                return trgtBmp;
            }
            else
            {
                throw new Exception("Unsupported pixel format");
            }
        }

        public static Bitmap RgbToGray(Bitmap srcBmp, Bitmap trgtBmp)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            lock (srcBmp)
            {
                if ((srcBmp.PixelFormat == PixelFormat.Format24bppRgb) || (srcBmp.PixelFormat == PixelFormat.Format32bppArgb) && (trgtBmp.PixelFormat == PixelFormat.Format8bppIndexed))
                {
                    BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                    trgtBmp.Palette = GetGrayScalePalette();
                    BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
                    int pixelSize = GetPixelSize(srcBmp.PixelFormat);
                    bool aligned4 = (srcBmd.Stride == srcBmd.Width * pixelSize);
                    unsafe
                    {
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        byte* trgtBytes = (byte*)trgtBmd.Scan0;
                        if (aligned4)
                        {
                            for (int i = 0, j = 0; i < srcBmd.Width * srcBmd.Height; i++, j += pixelSize)
                            {
                                trgtBytes[i] = (byte)(0.071 * srcBytes[j] + 0.707 * srcBytes[j + 1] + 0.222 * srcBytes[j + 2]);
                            }
                        }
                        else
                        {
                            for (int row = 0; row < srcBmd.Height; row++)
                            {
                                for (int i = 0, j = 0; i < srcBmd.Width; i++, j += pixelSize)
                                {
                                    trgtBytes[i] = (byte)(0.071 * srcBytes[j] + 0.707 * srcBytes[j + 1] + 0.222 * srcBytes[j + 2]);
                                }
                                srcBytes += srcBmd.Stride;
                                trgtBytes += trgtBmd.Stride;
                            }
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
                return null;
            }
            lock (srcBmp)
            {
                BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                Bitmap trgtBmp = new Bitmap(srcBmp.Width, srcBmp.Height, srcBmp.PixelFormat);
                int pixelSize = GetPixelSize(srcBmp.PixelFormat);
                if (pixelSize == 1)
                {
                    trgtBmp.Palette = GetGrayScalePalette();
                }
                BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
                unsafe
                {
                    memcpy((byte*)trgtBmd.Scan0, (byte*)srcBmd.Scan0, (ulong)(srcBmd.Stride * srcBmd.Height));
                }
                srcBmp.UnlockBits(srcBmd);
                trgtBmp.UnlockBits(trgtBmd);
                return trgtBmp;
            }
        }

        public static byte[] BitmapProbe(Bitmap srcBmp, int step)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            lock (srcBmp)
            {
                if ((srcBmp.PixelFormat == PixelFormat.Format24bppRgb) || (srcBmp.PixelFormat == PixelFormat.Format32bppArgb))
                {
                    BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                    int targetSize = (srcBmd.Width * srcBmd.Height) / step;
                    byte[] trgtData = new byte[targetSize];
                    int pixelSize = GetPixelSize(srcBmp.PixelFormat);
                    int t = 0;
                    unsafe
                    {
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        for (int row = 0; row < srcBmd.Height; row += step)
                        {
                            byte* rowBytes = srcBytes + row * srcBmd.Stride;
                            for (int col = 0; col < srcBmd.Width; col += step * pixelSize)
                            {
                                int k = col * 3;
                                trgtData[t++] = (byte)(0.071 * rowBytes[k] + 0.707 * rowBytes[k + 1] + 0.222 * rowBytes[k + 2]);
                            }
                        }
                    }
                    srcBmp.UnlockBits(srcBmd);
                    return trgtData;
                }
                else
                {
                    throw new Exception("Unsupported pixel format");
                }
            }
        }

        public static ulong BitmapHash(Bitmap srcBmp, int step)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            lock (srcBmp)
            {
                BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                int pixelSize = GetPixelSize(srcBmp.PixelFormat);
                ulong hash = 0;
                unsafe
                {
                    byte* srcBytes = (byte*)srcBmd.Scan0;
                    for (int row = 0; row < srcBmd.Height; row += step)
                    {
                        byte* rowBytes = srcBytes + row * srcBmd.Stride;
                        for (int col = 0; col < srcBmd.Width; col += step * pixelSize)
                        {
                            int k = col * 3;
                            hash += (byte)(0.071 * rowBytes[k] + 0.707 * rowBytes[k + 1] + 0.222 * rowBytes[k + 2]);
                        }
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
