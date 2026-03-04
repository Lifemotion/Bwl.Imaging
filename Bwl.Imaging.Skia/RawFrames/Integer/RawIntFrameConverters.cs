using System;
using System.Threading.Tasks;

namespace Bwl.Imaging.Skia
{

    public static class RawIntFrameConverters
    {

        public static RGBMatrix ConvertTo8Bit(this RawIntFrame frame, int gainByBitOffset)
        {
            var mtr1 = new RGBMatrix(frame.Width, frame.Height);
            for (int y = 0, loopTo = frame.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = frame.Width - 1; x <= loopTo1; x++)
                {
                    int b = frame.Data[(x + y * frame.Width) * 3 + 0];
                    int g = frame.Data[(x + y * frame.Width) * 3 + 1];
                    int r = frame.Data[(x + y * frame.Width) * 3 + 2];
                    if (gainByBitOffset <= 4)
                    {
                        r = r >> 4 - gainByBitOffset;
                        g = g >> 4 - gainByBitOffset;
                        b = b >> 4 - gainByBitOffset;
                    }
                    else
                    {
                        r = r << gainByBitOffset - 4;
                        g = g << gainByBitOffset - 4;
                        b = b << gainByBitOffset - 4;
                    }
                    mtr1.SetRedPixel(x, y, Math.Min(r, 255));
                    mtr1.SetGreenPixel(x, y, Math.Min(g, 255));
                    mtr1.SetBluePixel(x, y, Math.Min(b, 255));
                }
            }
            return mtr1;
        }

        public static RGBMatrix ConvertTo8BitFast(this RawIntFrame frame, int gainByBitOffset)
        {
            var mtr1 = new RGBMatrix(frame.Width, frame.Height);
            int[] frameData = frame.Data;
            if (gainByBitOffset <= 4)
            {
                Parallel.For(0, 3, (channel) =>
                    {
                        int[] matrix = mtr1.GetMatrix(2 - channel);
                        for (int i = 0, loopTo = matrix.Length - 1; i <= loopTo; i++)
                        {
                            int px = frameData[i * 3 + channel] >> 4 - gainByBitOffset;
                            matrix[i] = Math.Min(px, 255);
                        }
                    });
            }
            else
            {
                Parallel.For(0, 3, (channel) =>
                    {
                        int[] matrix = mtr1.GetMatrix(2 - channel);
                        for (int i = 0, loopTo = matrix.Length - 1; i <= loopTo; i++)
                        {
                            int px = frameData[i * 3 + channel] << gainByBitOffset - 4;
                            matrix[i] = Math.Min(px, 255);
                        }
                    });
            }
            return mtr1;
        }

        public static RGBMatrix[] ConvertTo8BitPair(RawIntFrame frame, int totalBits = 12)
        {
            var mtr1 = new RGBMatrix(frame.Width, frame.Height);
            var mtr2 = new RGBMatrix(frame.Width, frame.Height);

            for (int y = 0, loopTo = frame.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = frame.Width - 1; x <= loopTo1; x++)
                {
                    int b = frame.Data[(x + y * frame.Width) * 3 + 0];
                    int g = frame.Data[(x + y * frame.Width) * 3 + 1];
                    int r = frame.Data[(x + y * frame.Width) * 3 + 2];

                    if (totalBits == 12)
                    {
                        int rh = r >> 4;
                        int rl = r;

                        int bh = b >> 4;
                        int bl = b;

                        int gh = g >> 4;
                        int gl = g;

                        if (rl > 63)
                            rl = 63;
                        if (gl > 63)
                            gl = 63;
                        if (bl > 63)
                            bl = 63;

                        // If rl > 31 Then rl = rl And 15 Or 16
                        // If gl > 31 Then gl = gl And 15 Or 16
                        // If bl > 31 Then bl = bl And 15 Or 16

                        mtr1.SetRedPixel(x, y, rh);
                        mtr1.SetGreenPixel(x, y, gh);
                        mtr1.SetBluePixel(x, y, bh);

                        mtr2.SetRedPixel(x, y, rl);
                        mtr2.SetGreenPixel(x, y, gl);
                        mtr2.SetBluePixel(x, y, bl);
                    }
                }
            }
            return new[] { mtr1, mtr2 };
        }

        public static RawIntFrame ConvertFrom8BitPair(RGBMatrix[] pair, int totalBits = 12)
        {
            var mtr1 = pair[0];
            var mtr2 = pair[1];
            var arr = new int[(mtr1.Width * mtr1.Height * 3)];
            var frame = new RawIntFrame(mtr1.Width, mtr2.Height, arr);

            for (int y = 0, loopTo = mtr1.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = mtr1.Width - 1; x <= loopTo1; x++)
                {

                    int rh = mtr1.GetRedPixel(x, y);
                    int gh = mtr1.GetGreenPixel(x, y);
                    int bh = mtr1.GetBluePixel(x, y);

                    int rl = mtr2.GetRedPixel(x, y);
                    int gl = mtr2.GetGreenPixel(x, y);
                    int bl = mtr2.GetBluePixel(x, y);

                    if (totalBits == 12)
                    {
                        int r = (rh & 0xFF) << 4 | rl & 0xF;
                        int g = (gh & 0xFF) << 4 | gl & 0xF;
                        int b = (bh & 0xFF) << 4 | bl & 0xF;

                        frame.Data[(x + y * frame.Width) * 3 + 0] = b;
                        frame.Data[(x + y * frame.Width) * 3 + 1] = g;
                        frame.Data[(x + y * frame.Width) * 3 + 2] = r;
                    }
                }
            }
            return frame;
        }

        public static RGBMatrix ConvertHDR1(this RawIntFrame frame, int baseGain)
        {
            var mtr1 = new RGBMatrix(frame.Width, frame.Height);
            for (int i = 0, loopTo = frame.Width * frame.Height - 1; i <= loopTo; i++)
            {
                int b = frame.Data[i * 3 + 0];
                int g = frame.Data[i * 3 + 1];
                int r = frame.Data[i * 3 + 2];

                if (baseGain <= 4)
                {
                    r = r >> 4 - baseGain;
                    g = g >> 4 - baseGain;
                    b = b >> 4 - baseGain;
                }
                else
                {
                    r = r << baseGain - 4;
                    g = g << baseGain - 4;
                    b = b << baseGain - 4;
                }
                int p = 255;
                int t = 60;
                double k = 0.8d;
                while (r > p | g > p | b > p) // And (Math.Abs(r - b) > t Or Math.Abs(g - b) > t Or Math.Abs(g - r) > t)
                {
                    // Do While (r > p Or g > p Or b > p) And (Math.Abs(r - b) > t Or Math.Abs(g - b) > t Or Math.Abs(g - r) > t)
                    // Do While (r > p Or g > p Or b > p) And Not (r > t And g > t And b > t)
                    r = (int)Math.Round(r * k);
                    g = (int)Math.Round(g * k);
                    b = (int)Math.Round(b * k);
                }
                mtr1.Red[i] = Math.Min(r, 255);
                mtr1.Green[i] = Math.Min(g, 255);
                mtr1.Blue[i] = Math.Min(b, 255);
            }
            return mtr1;
        }

        public static RGBMatrix ConvertHDR1Fast(this RawIntFrame frame, int baseGain)
        {
            var mtr1 = new RGBMatrix(frame.Width, frame.Height);
            int[] frameData = frame.Data;
            int p = 255;
            int k = 14;
            if (baseGain <= 4)
            {
                int bitShift = 4 - baseGain;
                Parallel.For(0, frame.Width * frame.Height, (i) =>
                    {
                        int b = frameData[i * 3 + 0] >> bitShift;
                        int g = frameData[i * 3 + 1] >> bitShift;
                        int r = frameData[i * 3 + 2] >> bitShift;
                        while (r > p | g > p | b > p)
                        {
                            r = r * k >> 4;
                            g = g * k >> 4;
                            b = b * k >> 4;
                        }
                        mtr1.Red[i] = r;
                        mtr1.Green[i] = g;
                        mtr1.Blue[i] = b;
                    });
            }
            else
            {
                int bitShift = baseGain - 4;
                Parallel.For(0, frame.Width * frame.Height, (i) =>
                    {
                        int b = frameData[i * 3 + 0] << bitShift;
                        int g = frameData[i * 3 + 1] << bitShift;
                        int r = frameData[i * 3 + 2] << bitShift;
                        while (r > p | g > p | b > p)
                        {
                            r = r * k >> 4;
                            g = g * k >> 4;
                            b = b * k >> 4;
                        }
                        mtr1.Red[i] = r;
                        mtr1.Green[i] = g;
                        mtr1.Blue[i] = b;
                    });
            }
            return mtr1;
        }

        public static RGBMatrix ConvertHDR2(this RawIntFrame frame, int baseGain)
        {
            var mtr1 = new RGBMatrix(frame.Width, frame.Height);
            var max1 = new GrayMatrix(frame.Width / 4 + 8, frame.Height / 4 + 8);
            var min1 = new GrayMatrix(frame.Width / 4 + 8, frame.Height / 4 + 8);

            for (int y = 0, loopTo = frame.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = frame.Width - 1; x <= loopTo1; x++)
                {
                    int b = frame.Data[(x + y * frame.Width) * 3 + 0];
                    int g = frame.Data[(x + y * frame.Width) * 3 + 1];
                    int r = frame.Data[(x + y * frame.Width) * 3 + 2];

                    if (baseGain > 4)
                        baseGain = 4;
                    // r = r >> (4 - baseGain)
                    // g = g >> (4 - baseGain)
                    // b = b >> (4 - baseGain)

                    int max = Math.Max(Math.Max(r, g), b);
                    int min = Math.Min(Math.Min(r, g), b);
                    max1.SetGrayPixel(x / 4, y / 4, max / 16);
                    min1.SetGrayPixel(x / 4, y / 4, min / 16);

                    mtr1.SetRedPixel(x, y, r);
                    mtr1.SetGreenPixel(x, y, g);
                    mtr1.SetBluePixel(x, y, b);
                }
            }
            var bmp = max1.ToBitmap();

            int n = 4;
            int k = 256;
            max1 = Filters.MedianFilter2D(max1, n);
            for (int y = 0, loopTo2 = frame.Height - 1; y <= loopTo2; y++)
            {
                for (int x = 0, loopTo3 = frame.Width - 1; x <= loopTo3; x++)
                {
                    int b = frame.Data[(x + y * frame.Width) * 3 + 0];
                    int g = frame.Data[(x + y * frame.Width) * 3 + 1];
                    int r = frame.Data[(x + y * frame.Width) * 3 + 2];

                    int min = min1.GetGrayPixel(x / 4, y / 4) * 16;
                    int max = max1.GetGrayPixel(x / 4, y / 4) * 16;

                    if (max > 255)
                    {
                        r = r * k / max;
                        g = g * k / max;
                        b = b * k / max;
                    }

                    mtr1.SetRedPixel(x, y, Math.Min(r, 255));
                    mtr1.SetGreenPixel(x, y, Math.Min(g, 255));
                    mtr1.SetBluePixel(x, y, Math.Min(b, 255));
                }
            }
            return mtr1;
        }

        public static RGBMatrix ConvertHDR3(this RawIntFrame frame)
        {
            var mtr1 = new RGBMatrix(frame.Width, frame.Height);
            for (int y = 0, loopTo = frame.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = frame.Width - 1; x <= loopTo1; x++)
                {
                    int b = frame.Data[(x + y * frame.Width) * 3 + 0];
                    int g = frame.Data[(x + y * frame.Width) * 3 + 1];
                    int r = frame.Data[(x + y * frame.Width) * 3 + 2];

                    double v = 0.4d;
                    int k = 12;
                    int m = 0;
                    if (r < m)
                        r = m;
                    if (g < m)
                        g = m;
                    if (b < m)
                        b = m;
                    b = (int)Math.Round(Math.Pow(b - m, v) * k);
                    g = (int)Math.Round(Math.Pow(g - m, v) * k);
                    r = (int)Math.Round(Math.Pow(r - m, v) * k);

                    mtr1.SetRedPixel(x, y, Math.Min(r, 255));
                    mtr1.SetGreenPixel(x, y, Math.Min(g, 255));
                    mtr1.SetBluePixel(x, y, Math.Min(b, 255));
                }
            }
            return mtr1;
        }
        // Таблицы
        private static double _ConvertHDR3Fast_vs = 0.5d;
        private static double _ConvertHDR3Fast_ks = 5d;
        private static int[] _ConvertHDR3Fast_powTable = default;

        public static RGBMatrix ConvertHDR3Fast(this RawIntFrame frame, double v = 0.5d, double k = 5d)
        {
            if (_ConvertHDR3Fast_powTable is null || _ConvertHDR3Fast_vs != v || _ConvertHDR3Fast_ks != k)
            {
                _ConvertHDR3Fast_vs = v;
                _ConvertHDR3Fast_ks = k;
                _ConvertHDR3Fast_powTable = new int[4096]; // 4095 - 12 bit
                for (int powArg = 0, loopTo = _ConvertHDR3Fast_powTable.Length - 1; powArg <= loopTo; powArg++)
                    _ConvertHDR3Fast_powTable[powArg] = (int)Math.Round(Math.Min(Math.Pow(powArg, _ConvertHDR3Fast_vs) * _ConvertHDR3Fast_ks, 255d));
            }
            // Расчет
            var mtr1 = new RGBMatrix(frame.Width, frame.Height);
            int[] frameData = frame.Data;
            Parallel.For(0, 3, (channel) =>
                {
                    int[] matrix = mtr1.GetMatrix(2 - channel);
                    for (int i = 0, loopTo = matrix.Length - 1; i <= loopTo; i++)
                    {
                        int px = frameData[i * 3 + channel];
                        matrix[i] = _ConvertHDR3Fast_powTable[px];
                    }
                });
            return mtr1;
        }
    }
}