﻿using System;
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
                Bitmap trgtBmp = new Bitmap(region.Width, region.Height, srcBmp.PixelFormat);
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
            if (trgtBmp == null)
            {
                throw new Exception("trgtBmp == null");
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
                        throw new Exception("Inconsistent target image and source region");
                    }
                }
                else
                {
                    throw new Exception("Unsupported pixel format");
                }
            }
        }

        public static void PatchGray(Bitmap srcBmp, Bitmap trgtBmp, Rectangle region)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            if (trgtBmp == null)
            {
                throw new Exception("trgtBmp == null");
            }
            lock (trgtBmp)
            {
                if ((trgtBmp.PixelFormat == PixelFormat.Format8bppIndexed) && (srcBmp.PixelFormat == PixelFormat.Format8bppIndexed))
                {
                    if ((srcBmp.Width == region.Width) && (srcBmp.Height == region.Height))
                    {
                        BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                        BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
                        unsafe
                        {
                            int regionX = region.X;
                            int regionY = region.Y;
                            int regionW = region.Width;
                            int regionH = region.Height;
                            byte* srcBytes = (byte*)srcBmd.Scan0;
                            byte* trgtBytes = (byte*)trgtBmd.Scan0 + (regionY * trgtBmd.Stride) + regionX;
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
                        throw new Exception("Inconsistent source image and source region");
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
                Bitmap trgtBmp = new Bitmap(region.Width, region.Height, srcBmp.PixelFormat);
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
            if (trgtBmp == null)
            {
                throw new Exception("trgtBmp == null");
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
                            byte* srcBytes = (byte*)srcBmd.Scan0 + (regionY * srcBmd.Stride) + (regionX * pixelSize);
                            byte* trgtBytes = (byte*)trgtBmd.Scan0;
                            if (pixelSize == 3)
                            {
                                for (int rectRow = 0; rectRow < regionH; rectRow++)
                                {
                                    for (int rectCol = 0; rectCol < regionW * 3; rectCol += 3)
                                    {
                                        trgtBytes[rectCol] = srcBytes[rectCol];
                                        trgtBytes[rectCol + 1] = srcBytes[rectCol + 1];
                                        trgtBytes[rectCol + 2] = srcBytes[rectCol + 2];
                                    }
                                    srcBytes += srcBmd.Stride;
                                    trgtBytes += trgtBmd.Stride;
                                }
                            }
                            else
                            {
                                for (int rectRow = 0; rectRow < regionH; rectRow++)
                                {
                                    for (int rectCol = 0; rectCol < regionW * 4; rectCol += 4)
                                    {
                                        trgtBytes[rectCol] = srcBytes[rectCol];
                                        trgtBytes[rectCol + 1] = srcBytes[rectCol + 1];
                                        trgtBytes[rectCol + 2] = srcBytes[rectCol + 2];
                                        trgtBytes[rectCol + 3] = srcBytes[rectCol + 3];
                                    }
                                    srcBytes += srcBmd.Stride;
                                    trgtBytes += trgtBmd.Stride;
                                }
                            }
                        }
                        srcBmp.UnlockBits(srcBmd);
                        trgtBmp.UnlockBits(trgtBmd);
                    }
                    else
                    {
                        throw new Exception("Inconsistent target image and source region");
                    }
                }
                else
                {
                    throw new Exception("Unsupported pixel format");
                }
            }
        }

        public static void PatchRgb(Bitmap srcBmp, Bitmap trgtBmp, Rectangle region)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            if (trgtBmp == null)
            {
                throw new Exception("trgtBmp == null");
            }
            lock (trgtBmp)
            {
                if ((trgtBmp.PixelFormat == srcBmp.PixelFormat) && ((trgtBmp.PixelFormat == PixelFormat.Format24bppRgb) || (trgtBmp.PixelFormat == PixelFormat.Format32bppArgb)))
                {
                    if ((srcBmp.Width == region.Width) && (srcBmp.Height == region.Height))
                    {
                        BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                        BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
                        int pixelSize = GetPixelSize(trgtBmp.PixelFormat);
                        unsafe
                        {
                            int regionX = region.X;
                            int regionY = region.Y;
                            int regionW = region.Width;
                            int regionH = region.Height;
                            byte* srcBytes = (byte*)srcBmd.Scan0;
                            byte* trgtBytes = (byte*)trgtBmd.Scan0 + (regionY * trgtBmd.Stride) + (regionX * pixelSize);
                            if (pixelSize == 3)
                            {
                                for (int rectRow = 0; rectRow < regionH; rectRow++)
                                {
                                    for (int rectCol = 0; rectCol < regionW * 3; rectCol += 3)
                                    {
                                        trgtBytes[rectCol] = srcBytes[rectCol];
                                        trgtBytes[rectCol + 1] = srcBytes[rectCol + 1];
                                        trgtBytes[rectCol + 2] = srcBytes[rectCol + 2];
                                    }
                                    srcBytes += srcBmd.Stride;
                                    trgtBytes += trgtBmd.Stride;
                                }
                            }
                            else
                            {
                                for (int rectRow = 0; rectRow < regionH; rectRow++)
                                {
                                    for (int rectCol = 0; rectCol < regionW * 4; rectCol += 4)
                                    {
                                        trgtBytes[rectCol] = srcBytes[rectCol];
                                        trgtBytes[rectCol + 1] = srcBytes[rectCol + 1];
                                        trgtBytes[rectCol + 2] = srcBytes[rectCol + 2];
                                        trgtBytes[rectCol + 3] = srcBytes[rectCol + 3];
                                    }
                                    srcBytes += srcBmd.Stride;
                                    trgtBytes += trgtBmd.Stride;
                                }
                            }
                        }
                        srcBmp.UnlockBits(srcBmd);
                        trgtBmp.UnlockBits(trgtBmd);
                    }
                    else
                    {
                        throw new Exception("Inconsistent source image and source region");
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
            if (trgtBmp == null)
            {
                throw new Exception("trgtBmp == null");
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
                        int msize2 = 2; // 5 \ 2
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        byte* trgtBytes = (byte*)trgtBmd.Scan0;
                        Parallel.For(0, (srcBmd.Height - msize), (int row) =>
                        {
                            byte* srcScan0 = srcBytes + (row * srcBmd.Stride);
                            byte* srcScan2 = srcScan0 + (2 * srcBmd.Stride);
                            byte* srcScan4 = srcScan2 + (2 * srcBmd.Stride);

                            byte* trgtScan0 = trgtBytes + (row * trgtBmd.Stride);
                            byte* trgtScan2 = trgtScan0 + (2 * trgtBmd.Stride);

                            for (int col = 0; col < (srcBmd.Width - msize); col++)
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
                    srcBmp.UnlockBits(srcBmd);
                    trgtBmp.UnlockBits(trgtBmd);
                }
                else
                {
                    throw new Exception("Unsupported pixel format");
                }
            }
        }

        public static Bitmap Sharpen5Rgb(Bitmap srcBmp)
        {
            if ((srcBmp.PixelFormat == PixelFormat.Format24bppRgb) || (srcBmp.PixelFormat == PixelFormat.Format32bppArgb))
            {
                Bitmap trgtBmp = new Bitmap(srcBmp.Width, srcBmp.Height, srcBmp.PixelFormat);
                Sharpen5Rgb(srcBmp, trgtBmp);
                return trgtBmp;
            }
            else
            {
                throw new Exception("Unsupported pixel format");
            }
        }

        public static void Sharpen5Rgb(Bitmap srcBmp, Bitmap trgtBmp)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            if (trgtBmp == null)
            {
                throw new Exception("trgtBmp == null");
            }
            lock (srcBmp)
            {
                if ((srcBmp.PixelFormat == PixelFormat.Format24bppRgb) || (srcBmp.PixelFormat == PixelFormat.Format32bppArgb))
                {
                    BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                    BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
                    int srcPixelSize = GetPixelSize(srcBmp.PixelFormat);
                    int trgtPixelSize = GetPixelSize(trgtBmp.PixelFormat);
                    bool aligned4 = (srcBmd.Stride == srcBmd.Width * srcPixelSize);
                    bool aligned4Y = (srcBmd.Width % 4) == 0;
                    unsafe
                    {
                        byte* srcBytesYUV = (byte*)srcBmd.Scan0;
                        IntPtr hglobal = Marshal.AllocHGlobal(srcBmd.Width * srcBmd.Height);
                        byte* Y = (byte*)hglobal;
                        var Yindex = Y;

                        if (aligned4 && aligned4Y)
                        {
                            for (int i = 0, j = 0; i < srcBmd.Width * srcBmd.Height; i++, j += srcPixelSize)
                            {
                                // Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601 (http://inst.eecs.berkeley.edu/~cs150/Documents/ITU601.PDF)
                                Yindex[i] = (byte)(0.114 * srcBytesYUV[j] + 0.587 * srcBytesYUV[j + 1] + 0.299 * srcBytesYUV[j + 2]);
                            }
                        }
                        else
                        {
                            for (int row = 0; row < srcBmd.Height; row++)
                            {
                                for (int i = 0, j = 0; i < srcBmd.Width; i++, j += srcPixelSize)
                                {
                                    // Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601 (http://inst.eecs.berkeley.edu/~cs150/Documents/ITU601.PDF)
                                    Yindex[i] = (byte)(0.114 * srcBytesYUV[j] + 0.587 * srcBytesYUV[j + 1] + 0.299 * srcBytesYUV[j + 2]);
                                }
                                srcBytesYUV += srcBmd.Stride;
                                Yindex += srcBmd.Width;
                            }
                        }

                        int msize = 5;
                        int msize2 = 2; // 5 \ 2
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        byte* trgtBytes = (byte*)trgtBmd.Scan0;

                        Parallel.For(0, (srcBmd.Height - msize), (int row) =>
                        {
                            byte* srcScan0 = srcBytes + (row * srcBmd.Stride);
                            byte* srcScan2 = srcScan0 + (2 * srcBmd.Stride);
                            byte* srcScan4 = srcScan2 + (2 * srcBmd.Stride);

                            byte* yScan0 = Y + (row * srcBmd.Width);
                            byte* yScan2 = yScan0 + (2 * srcBmd.Width);
                            byte* yScan4 = yScan2 + (2 * srcBmd.Width);
                            yScan0 -= msize2;
                            yScan2 -= msize2;
                            yScan4 -= msize2;

                            byte* trgtScan0 = trgtBytes + (row * trgtBmd.Stride);
                            byte* trgtScan2 = trgtScan0 + (2 * trgtBmd.Stride);

                            for (int col = 0; col < (srcBmd.Width - msize); col++)
                            {
                                var j = col * trgtPixelSize;
                                // https://ru.wikipedia.org/wiki/YUV
                                var U = 0.436 * srcScan2[j] - 0.28886 * srcScan2[j + 1] - 0.14713 * srcScan2[j + 2] + 128; // BGR
                                var V = -0.10001 * srcScan2[j] - 0.51499 * srcScan2[j + 1] + 0.615 * srcScan2[j + 2] + 128; // BGR

                                byte* m0 = yScan0 + col;
                                byte* m2 = yScan2 + col;
                                byte* m4 = yScan4 + col;
                                double valueY = -0.1 * m0[0] + -0.1 * m0[2] + -0.1 * m0[4] +
                                                -0.1 * m2[0] + 1.8 * m2[2] + -0.1 * m2[4] +
                                                -0.1 * m4[0] + -0.1 * m4[2] + -0.1 * m4[4];
                                valueY = valueY < 0 ? 0 : valueY;
                                valueY = valueY > 255 ? 255 : valueY;

                                var r = valueY + 1.13983 * (V - 128);
                                var g = valueY - 0.39465 * (U - 128) - 0.58060 * (V - 128);
                                var b = valueY + 2.03211 * (U - 128);

                                r = r < 0 ? 0 : r;
                                r = r > 255 ? 255 : r;

                                g = g < 0 ? 0 : g;
                                g = g > 255 ? 255 : g;

                                b = b < 0 ? 0 : b;
                                b = b > 255 ? 255 : b;

                                trgtScan2[j] = (byte)b;
                                trgtScan2[j + 1] = (byte)g;
                                trgtScan2[j + 2] = (byte)r;
                                if (trgtPixelSize == 4)
                                {
                                    trgtScan2[j + 3] = (byte)255;
                                }
                            }
                        });
                        Marshal.FreeHGlobal(hglobal);
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

        public static Bitmap NormalizeRgb(Bitmap srcBmp, double borderPercent = 0.1)
        {
            if ((srcBmp.PixelFormat == PixelFormat.Format24bppRgb) || (srcBmp.PixelFormat == PixelFormat.Format32bppArgb))
            {
                Bitmap trgtBmp = new Bitmap(srcBmp.Width, srcBmp.Height, srcBmp.PixelFormat);
                NormalizeRgb(srcBmp, trgtBmp, borderPercent);
                return trgtBmp;
            }
            else
            {
                throw new Exception("Unsupported pixel format");
            }
        }

        public static void NormalizeRgb(Bitmap srcBmp, Bitmap trgtBmp, double borderPercent = 0.1)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            if (trgtBmp == null)
            {
                throw new Exception("trgtBmp == null");
            }
            lock (srcBmp)
            {
                if ((srcBmp.PixelFormat == PixelFormat.Format24bppRgb) || (srcBmp.PixelFormat == PixelFormat.Format32bppArgb))
                {
                    BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                    BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
                    int srcPixelSize = GetPixelSize(srcBmp.PixelFormat);
                    int trgtPixelSize = GetPixelSize(trgtBmp.PixelFormat);
                    bool aligned4 = (srcBmd.Stride == srcBmd.Width * srcPixelSize);
                    bool aligned4Y = (srcBmd.Width % 4) == 0;
                    unsafe
                    {
                        byte* srcBytesYUV = (byte*)srcBmd.Scan0;
                        var pixelsCount = srcBmd.Width * srcBmd.Height;
                        IntPtr hglobal = Marshal.AllocHGlobal(pixelsCount);
                        byte* Y = (byte*)hglobal;
                        var Yindex = Y;
                        var hist = new int[256];

                        if (aligned4 && aligned4Y)
                        {
                            for (int i = 0, j = 0; i < pixelsCount; i++, j += srcPixelSize)
                            {
                                // Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601 (http://inst.eecs.berkeley.edu/~cs150/Documents/ITU601.PDF)
                                Yindex[i] = (byte)(0.114 * srcBytesYUV[j] + 0.587 * srcBytesYUV[j + 1] + 0.299 * srcBytesYUV[j + 2]);
                                hist[Yindex[i]] += 1;
                            }
                        }
                        else
                        {
                            for (int row = 0; row < srcBmd.Height; row++)
                            {
                                for (int i = 0, j = 0; i < srcBmd.Width; i++, j += srcPixelSize)
                                {
                                    // Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601 (http://inst.eecs.berkeley.edu/~cs150/Documents/ITU601.PDF)
                                    Yindex[i] = (byte)(0.114 * srcBytesYUV[j] + 0.587 * srcBytesYUV[j + 1] + 0.299 * srcBytesYUV[j + 2]);
                                    hist[Yindex[i]] += 1;
                                }
                                srcBytesYUV += srcBmd.Stride;
                                Yindex += srcBmd.Width;
                            }
                        }

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

                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        byte* trgtBytes = (byte*)trgtBmd.Scan0;

                        Parallel.For(0, (srcBmd.Height), (int row) =>
                        {
                            byte* srcScan0 = srcBytes + (row * srcBmd.Stride);
                            byte* yScan0 = Y + (row * srcBmd.Width);

                            byte* trgtScan0 = trgtBytes + (row * trgtBmd.Stride);

                            for (int col = 0; col < srcBmd.Width; col++)
                            {
                                var j = col * trgtPixelSize;
                                // https://ru.wikipedia.org/wiki/YUV
                                var U = 0.436 * srcScan0[j] - 0.28886 * srcScan0[j + 1] - 0.14713 * srcScan0[j + 2] + 128; // BGR
                                var V = -0.10001 * srcScan0[j] - 0.51499 * srcScan0[j + 1] + 0.615 * srcScan0[j + 2] + 128; // BGR

                                double valueY = yScan0[col];
                                valueY = (valueY - min) * 255 / (max - min);

                                valueY = valueY < 0 ? 0 : valueY;
                                valueY = valueY > 255 ? 255 : valueY;

                                var r = valueY + 1.13983 * (V - 128);
                                var g = valueY - 0.39465 * (U - 128) - 0.58060 * (V - 128);
                                var b = valueY + 2.03211 * (U - 128);

                                r = r < 0 ? 0 : r;
                                r = r > 255 ? 255 : r;

                                g = g < 0 ? 0 : g;
                                g = g > 255 ? 255 : g;

                                b = b < 0 ? 0 : b;
                                b = b > 255 ? 255 : b;

                                trgtScan0[j] = (byte)b;
                                trgtScan0[j + 1] = (byte)g;
                                trgtScan0[j + 2] = (byte)r;
                                if (trgtPixelSize == 4)
                                {
                                    trgtScan0[j + 3] = (byte)255;
                                }
                            }
                        });
                        Marshal.FreeHGlobal(hglobal);
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
            if (trgtBmp == null)
            {
                throw new Exception("trgtBmp == null");
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
            if (trgtBmp == null)
            {
                throw new Exception("trgtBmp == null");
            }
            lock (srcBmp)
            {
                if ((srcBmp.PixelFormat == PixelFormat.Format24bppRgb) || (srcBmp.PixelFormat == PixelFormat.Format32bppArgb) && (trgtBmp.PixelFormat == PixelFormat.Format8bppIndexed))
                {
                    BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                    trgtBmp.Palette = GetGrayScalePalette();
                    BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
                    int srcPixelSize = GetPixelSize(srcBmp.PixelFormat);
                    int trgtPixelSize = GetPixelSize(trgtBmp.PixelFormat);
                    if (trgtPixelSize != 1)
                    {
                        throw new Exception("Unsupported target pixel format");
                    }
                    bool srcAligned4 = (srcBmd.Stride == srcBmd.Width * srcPixelSize);
                    bool trgtAligned4 = (trgtBmd.Stride == trgtBmd.Width * trgtPixelSize);
                    unsafe
                    {
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        byte* trgtBytes = (byte*)trgtBmd.Scan0;
                        if (srcAligned4 && trgtAligned4)
                        {
                            for (int i = 0, j = 0; i < srcBmd.Width * srcBmd.Height; i++, j += srcPixelSize)
                            {
                                // Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601 (http://inst.eecs.berkeley.edu/~cs150/Documents/ITU601.PDF)
                                trgtBytes[i] = (byte)(0.114 * srcBytes[j] + 0.587 * srcBytes[j + 1] + 0.299 * srcBytes[j + 2]);
                            }
                        }
                        else
                        {
                            for (int row = 0; row < srcBmd.Height; row++)
                            {
                                for (int i = 0, j = 0; i < srcBmd.Width; i++, j += srcPixelSize)
                                {
                                    // Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601 (http://inst.eecs.berkeley.edu/~cs150/Documents/ITU601.PDF)
                                    trgtBytes[i] = (byte)(0.114 * srcBytes[j] + 0.587 * srcBytes[j + 1] + 0.299 * srcBytes[j + 2]);
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

        public static void RgbReverse(Bitmap srcBmp)
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
                    int srcPixelSize = GetPixelSize(srcBmp.PixelFormat);
                    bool srcAligned4 = (srcBmd.Stride == srcBmd.Width * srcPixelSize);

                    unsafe
                    {
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        if (srcAligned4)
                        {
                            for (int i = 0, j = 0; i < srcBmd.Width * srcBmd.Height; i++, j += srcPixelSize)
                            {
                                byte tmp = srcBytes[j];
                                srcBytes[j] = srcBytes[j + 2];
                                srcBytes[j + 2] = tmp;
                            }
                        }
                        else
                        {
                            for (int row = 0; row < srcBmd.Height; row++)
                            {
                                for (int i = 0, j = 0; i < srcBmd.Width; i++, j += srcPixelSize)
                                {
                                    byte tmp = srcBytes[j];
                                    srcBytes[j] = srcBytes[j + 2];
                                    srcBytes[j + 2] = tmp;
                                }
                                srcBytes += srcBmd.Stride;
                            }
                        }
                    }
                    srcBmp.UnlockBits(srcBmd);
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

        public static Bitmap BitmapFromIntPtr(IntPtr src, Size size, PixelFormat pixelFormat)
        {
            Bitmap trgtBmp = new Bitmap(size.Width, size.Height, pixelFormat);
            int pixelSize = GetPixelSize(pixelFormat);
            BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
            unsafe
            {
                memcpy((byte*)trgtBmd.Scan0, (byte*)src.ToPointer(), (ulong)(trgtBmd.Stride * trgtBmd.Height));
            }
            trgtBmp.UnlockBits(trgtBmd);
            return trgtBmp;
        }

        public static void FillBitmapFromIntPtr(IntPtr src, Bitmap trgtBmp, PixelFormat pixelFormat)
        {
            BitmapData trgtBmd = trgtBmp.LockBits(new Rectangle(0, 0, trgtBmp.Width, trgtBmp.Height), ImageLockMode.WriteOnly, trgtBmp.PixelFormat);
            unsafe
            {
                memcpy((byte*)trgtBmd.Scan0, (byte*)src.ToPointer(), (ulong)(trgtBmd.Stride * trgtBmd.Height));
            }
            trgtBmp.UnlockBits(trgtBmd);
        }

        public static byte[] BitmapProbeGray(Bitmap srcBmp, int step)
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
                    int targetW = 1 + ((srcBmd.Width - 1) / step);
                    int targetH = 1 + ((srcBmd.Height - 1) / step);
                    byte[] trgtData = new byte[targetW * targetH];
                    int t = 0;
                    unsafe
                    {
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        for (int row = 0; row < srcBmd.Height; row += step)
                        {
                            for (int col = 0; col < srcBmd.Width; col += step)
                            {
                                trgtData[t++] = (byte)srcBytes[col];
                            }
                            srcBytes += srcBmd.Stride * step;
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

        public static byte[] BitmapProbeRgb(Bitmap srcBmp, int step)
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
                    int srcPixelSize = GetPixelSize(srcBmp.PixelFormat);
                    int targetW = 1 + ((srcBmd.Width - 1) / step);
                    int targetH = 1 + ((srcBmd.Height - 1) / step);
                    byte[] trgtData = new byte[targetW * targetH];
                    int t = 0;
                    unsafe
                    {
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        for (int row = 0; row < srcBmd.Height; row += step)
                        {
                            for (int col = 0; col < srcBmd.Width; col += step)
                            {
                                int k = col * srcPixelSize;
                                // Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601 (http://inst.eecs.berkeley.edu/~cs150/Documents/ITU601.PDF)
                                trgtData[t++] = (byte)(0.114 * srcBytes[k] + 0.587 * srcBytes[k + 1] + 0.299 * srcBytes[k + 2]);
                            }
                            srcBytes += srcBmd.Stride * step;
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

        public static byte[] BitmapProbeRgb2(Bitmap srcBmp, int step)
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
                    int srcPixelSize = GetPixelSize(srcBmp.PixelFormat);
                    int targetW = 1 + ((srcBmd.Width - 1) / step);
                    int targetH = 1 + ((srcBmd.Height - 1) / step);
                    byte[] trgtData = new byte[targetW * targetH * 3]; // Всегда RGB
                    int t = 0;
                    unsafe
                    {
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        for (int row = 0; row < srcBmd.Height; row += step)
                        {
                            for (int col = 0; col < srcBmd.Width; col += step)
                            {
                                int k = col * srcPixelSize;
                                // Порядок байт в trgtData -> R,G,B
                                trgtData[t + 2] = (byte)srcBytes[k];
                                trgtData[t + 1] = (byte)srcBytes[k + 1];
                                trgtData[t] = (byte)srcBytes[k + 2];
                                t += 3; // Всегда RGB
                            }
                            srcBytes += srcBmd.Stride * step;
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

        public static bool JpegDecodedOK(Bitmap srcBmp)
        {
            if (srcBmp == null)
            {
                throw new Exception("srcBmp == null");
            }
            lock (srcBmp)
            {
                if ((srcBmp.PixelFormat == PixelFormat.Format24bppRgb) || (srcBmp.PixelFormat == PixelFormat.Format32bppArgb))
                {
                    bool checkResult = true;
                    int jpegBlockSize = 8;
                    BitmapData srcBmd = srcBmp.LockBits(new Rectangle(0, 0, srcBmp.Width, srcBmp.Height), ImageLockMode.ReadOnly, srcBmp.PixelFormat);
                    int srcPixelSize = GetPixelSize(srcBmp.PixelFormat);
                    int fullJpegBlocksH = srcBmd.Height / jpegBlockSize; // Количество полных JPEG-блоков по ширине
                    int fullJpegBlocksW = srcBmd.Width / jpegBlockSize; // Количество полных JPEG-блоков по ширине
                    int maxCol = fullJpegBlocksW * jpegBlockSize * srcPixelSize; // Максимальное значение столбца
                    for (int jpegBlockH = 0; jpegBlockH < fullJpegBlocksH - 2; jpegBlockH++)
                    {
                        var jpegRowA = (jpegBlockH + 0) * jpegBlockSize;
                        var jpegRowB = (jpegBlockH + 1) * jpegBlockSize;
                        var jpegRowC = (jpegBlockH + 2) * jpegBlockSize;
                        unsafe
                        {
                            byte* srcBytesA = (byte*)srcBmd.Scan0 + srcBmd.Stride * jpegRowA;
                            byte* srcBytesB = (byte*)srcBmd.Scan0 + srcBmd.Stride * jpegRowB;
                            byte* srcBytesC = (byte*)srcBmd.Scan0 + srcBmd.Stride * jpegRowC;
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
                                srcBytesA += srcBmd.Stride;
                                srcBytesB += srcBmd.Stride;
                                srcBytesC += srcBmd.Stride;
                            }
                            if (!diffDetected) // Если не обнаружено различий в данных JPEG-блока - проверка провалена
                            {
                                checkResult = false;
                                break;
                            }
                        }
                    }
                    srcBmp.UnlockBits(srcBmd);
                    return checkResult;
                }
                else
                {
                    throw new Exception("Unsupported pixel format");
                }
            }
        }

        public static ulong BitmapHashGray(Bitmap srcBmp, int step)
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
                    ulong hash = 0;
                    unsafe
                    {
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        for (int row = 0; row < srcBmd.Height; row += step)
                        {
                            for (int col = 0; col < srcBmd.Width; col += step)
                            {
                                hash += srcBytes[col];
                            }
                            srcBytes += srcBmd.Stride * step;
                        }
                    }
                    srcBmp.UnlockBits(srcBmd);
                    return (ulong)hash;
                }
                else
                {
                    throw new Exception("Unsupported pixel format");
                }
            }
        }

        public static ulong BitmapHashRgb(Bitmap srcBmp, int step)
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
                    int srcPixelSize = GetPixelSize(srcBmp.PixelFormat);
                    ulong hash = 0;
                    unsafe
                    {
                        byte* srcBytes = (byte*)srcBmd.Scan0;
                        for (int row = 0; row < srcBmd.Height; row += step)
                        {
                            for (int col = 0; col < srcBmd.Width; col += step)
                            {
                                int k = col * srcPixelSize;
                                // Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601 (http://inst.eecs.berkeley.edu/~cs150/Documents/ITU601.PDF)
                                hash += (byte)(0.114 * srcBytes[k] + 0.587 * srcBytes[k + 1] + 0.299 * srcBytes[k + 2]);
                            }
                            srcBytes += srcBmd.Stride * step;
                        }
                    }
                    srcBmp.UnlockBits(srcBmd);
                    return (ulong)hash;
                }
                else
                {
                    throw new Exception("Unsupported pixel format");
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
