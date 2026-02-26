using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Bwl.Imaging
{

    public class BitmapOperations
    {
        public static void CopyMemory(IntPtr dest, IntPtr src, ulong count)
        {
            UnsafeFunctions.CopyMemory(dest, src, count);
        }

        private byte[] _rawBytes;
        private int _width;
        private int _height;
        private int _channels;
        private int[] _gray2D;

        public byte[] RawBytes
        {
            set
            {
                _rawBytes = value;
            }
            get
            {
                return _rawBytes;
            }
        }

        public int Channels
        {
            get
            {
                return _channels;
            }
        }

        public void LoadBitmap(Bitmap bitmap)
        {
            _channels = bitmap.PixelFormat == PixelFormat.Format8bppIndexed ? 1 : 3;
            _width = bitmap.Width;
            _height = bitmap.Height;

            BitmapData srcBD;
            int srcStride;
            var srcRect = Rectangle.FromLTRB(0, 0, bitmap.Width, bitmap.Height);
            int size = bitmap.Width * bitmap.Height;
            if (_rawBytes is null || _rawBytes.Length != size * _channels)
            {
                _rawBytes = new byte[(size * _channels)];
            }
            byte[] tmpBytes;
            int tmpChannels = bitmap.PixelFormat == PixelFormat.Format32bppArgb ? 4 : _channels;

            if (tmpChannels == 4)
            {
                srcBD = bitmap.LockBits(srcRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                srcStride = srcBD.Stride;
                tmpBytes = new byte[(size * tmpChannels)];
            }
            else
            {
                srcBD = bitmap.LockBits(srcRect, ImageLockMode.ReadOnly, _channels == 1 ? PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb);
                srcStride = srcBD.Stride;
                tmpBytes = _rawBytes;
            }

            // Исходный битмап, некратный по ширине 4, имеет выравнивание, и загружать из него данные, как было реализовано ранее,
            // то есть одним блоком - нельзя! Требуетя загрузка построчно, с игнорированием выравнивания.
            if (srcStride == bitmap.Width * tmpChannels)
            {
                using (var ap = new AutoPinner(tmpBytes))
                {
                    var srcBytes = srcBD.Scan0;
                    var rawBytes = ap.GetIntPtr();
                    CopyMemory(rawBytes, srcBytes, (uint)tmpBytes.Length); // fast!
                }
            }
            else
            {
                using (var ap = new AutoPinner(tmpBytes))
                {
                    var srcBytes = srcBD.Scan0;
                    var rawBytes = ap.GetIntPtr();
                    int rawStride = _width * _channels;
                    for (int row = 0, loopTo = _height - 1; row <= loopTo; row++)
                    {
                        CopyMemory(rawBytes, srcBytes, (uint)rawStride); // exact!
                        srcBytes = srcBytes + srcStride;
                        rawBytes = rawBytes + rawStride;
                    }
                }
            }

            // Выбрасываем альфа-канал
            if (tmpChannels == 4)
            {
                for (int i = 0, loopTo1 = tmpBytes.Length / 4 - 1; i <= loopTo1; i++)
                {
                    _rawBytes[i * 3 + 0] = tmpBytes[i * 4 + 0];
                    _rawBytes[i * 3 + 1] = tmpBytes[i * 4 + 1];
                    _rawBytes[i * 3 + 2] = tmpBytes[i * 4 + 2];
                }
            }
            else
            {
                _rawBytes = tmpBytes;
            }

            bitmap.UnlockBits(srcBD);
        }

        public GrayMatrix GetGrayMatrix()
        {
            GrayMatrix result = null;
            if (_gray2D is null || _gray2D.Length != _width * _height)
            {
                _gray2D = new int[(_width * _height)];
            }
            switch (_channels)
            {
                case 1:
                    {
                        for (int i = 0, loopTo = _width * _height - 1; i <= loopTo; i++)
                            _gray2D[i] = _rawBytes[i];
                        result = new GrayMatrix(_gray2D, _width, _height);
                        break;
                    }
                case 3:
                    {
                        for (int i = 0, loopTo1 = _width * _height - 1; i <= loopTo1; i++)
                            // Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601 (http://inst.eecs.berkeley.edu/~cs150/Documents/ITU601.PDF)
                            _gray2D[i] = (int)Math.Round(_rawBytes[i * 3] * 0.114d + _rawBytes[i * 3 + 1] * 0.587d + _rawBytes[i * 3 + 2] * 0.299d);
                        result = new GrayMatrix(_gray2D, _width, _height);
                        break;
                    }
            }
            return result;
        }

        public RGBMatrix GetRGBMatrix()
        {
            RGBMatrix result = null;
            var bytesRed2D = new int[(_width * _height)];
            var bytesGreen2D = new int[(_width * _height)];
            var bytesBlue2D = new int[(_width * _height)];
            switch (_channels)
            {
                case 1:
                    {
                        for (int i = 0, loopTo = _width * _height - 1; i <= loopTo; i++)
                        {
                            byte rawByte = _rawBytes[i];
                            bytesRed2D[i] = rawByte;
                            bytesGreen2D[i] = rawByte;
                            bytesBlue2D[i] = rawByte;
                        }
                        result = new RGBMatrix(bytesRed2D, bytesGreen2D, bytesBlue2D, _width, _height);
                        break;
                    }
                case 3:
                    {
                        for (int i = 0, loopTo1 = _width * _height - 1; i <= loopTo1; i++)
                        {
                            bytesRed2D[i] = _rawBytes[i * 3 + 2];
                            bytesGreen2D[i] = _rawBytes[i * 3 + 1];
                            bytesBlue2D[i] = _rawBytes[i * 3];
                        }
                        result = new RGBMatrix(bytesRed2D, bytesGreen2D, bytesBlue2D, _width, _height);
                        break;
                    }
            }
            return result;
        }

        public void LoadGrayMatrixWithLimiter(GrayMatrix matrix)
        {
            _channels = 1;
            _width = matrix.Width;
            _height = matrix.Height;
            _rawBytes = new byte[(_width * _height)];
            int[] matrixGray = matrix.Gray;
            for (int i = 0, loopTo = _width * _height - 1; i <= loopTo; i++)
            {
                int pixel = matrixGray[i];
                _rawBytes[i] = ImagingMath.Limit(pixel);
            }
        }

        public void LoadRGBMatrixWithLimiter(RGBMatrix matrix)
        {
            _channels = 3;
            _width = matrix.Width;
            _height = matrix.Height;
            _rawBytes = new byte[(_width * _height * 3)];
            int[] matrixRed = matrix.Red;
            int[] matrixGreen = matrix.Green;
            int[] matrixBlue = matrix.Blue;
            int i, x, y;
            var loopTo = _height - 1;
            for (y = 0; y <= loopTo; y++)
            {
                int offset = _width * y;
                var loopTo1 = _width - 1;
                for (x = 0; x <= loopTo1; x++)
                {
                    i = x + offset;
                    int pixelR = matrixRed[i];
                    int pixelG = matrixGreen[i];
                    int pixelB = matrixBlue[i];
                    _rawBytes[i * 3 + 2] = ImagingMath.Limit(pixelR);
                    _rawBytes[i * 3 + 1] = ImagingMath.Limit(pixelG);
                    _rawBytes[i * 3] = ImagingMath.Limit(pixelB);
                }
            }
        }

        public Bitmap GetBitmap()
        {
            var result = new Bitmap(_width, _height, _channels == 1 ? PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb);
            if (_channels == 1)
            {
                result.Palette = GetGrayScalePalette();
            }
            var resRect = Rectangle.FromLTRB(0, 0, _width, _height);
            var resultBD = result.LockBits(resRect, ImageLockMode.ReadWrite, _channels == 1 ? PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb);
            int resStride = resultBD.Stride;
            if (resStride == result.Width * _channels)
            {
                using (var ap = new AutoPinner(_rawBytes))
                {
                    var rawBytes = ap.GetIntPtr();
                    var resBytes = resultBD.Scan0;
                    CopyMemory(resBytes, rawBytes, (uint)_rawBytes.Length); // fast!
                }
            }
            else
            {
                using (var ap = new AutoPinner(_rawBytes))
                {
                    var rawBytes = ap.GetIntPtr();
                    var resBytes = resultBD.Scan0;
                    int rawStride = _width * _channels;
                    for (int row = 0, loopTo = _height - 1; row <= loopTo; row++)
                    {
                        CopyMemory(resBytes, rawBytes, (uint)rawStride); // exact!
                        rawBytes = rawBytes + rawStride;
                        resBytes = resBytes + resStride;
                    }
                }
            }

            result.UnlockBits(resultBD);
            return result;
        }

        public static ColorPalette GetGrayScalePalette()
        {
            var bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
            var monoPalette = bmp.Palette;
            Color[] entries = monoPalette.Entries;
            for (int i = 0; i <= 255; i++)
                entries[i] = Color.FromArgb(i, i, i);
            return monoPalette;
        }
    }
}