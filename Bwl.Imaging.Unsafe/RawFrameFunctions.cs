using System.Drawing;
using System.Drawing.Imaging;

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
                    //  int[] srcInts = data;
                    byte* trgtBytes = (byte*)trgtBmd.Scan0;
                    int r, g, b;
                    int koeff = 12;

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
    }
}
