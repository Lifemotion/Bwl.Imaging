using SkiaSharp;
using System;



namespace Bwl.Imaging.Skia
{

    public class BitmapOperations
    {

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

        /// <summary>
        /// Loads pixel data from an SKBitmap into the internal raw bytes array.
        /// </summary>
        /// <param name="bitmap">The source SKBitmap to load data from. Supports Gray8, Bgra8888, and Rgba8888 formats.</param>
        /// <remarks>
        /// For color images (BGRA/RGBA), the alpha channel is stripped and data is stored as 3-channel BGR.
        /// For grayscale images (Gray8), data is stored as-is in 1-channel format.
        /// </remarks>
        public unsafe void LoadBitmap(SKBitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));

            lock (bitmap)
            {

                _width = bitmap.Width;
                _height = bitmap.Height;
                int size = _width * _height;

                // Determine source format
                int srcChannels = bitmap.BytesPerPixel;
                bool hasAlpha = (bitmap.ColorType == SKColorType.Bgra8888 || bitmap.ColorType == SKColorType.Rgba8888);

                // Set target channels (strip alpha if present)
                _channels = hasAlpha ? 3 : srcChannels;

                // Allocate or reuse buffer
                if (_rawBytes is null || _rawBytes.Length != size * _channels)
                {
                    _rawBytes = new byte[size * _channels];
                }

                byte* srcPtr = (byte*)bitmap.GetPixels();
                int srcStride = bitmap.RowBytes;

                // Copy pixel data
                if (srcChannels == 1)
                {
                    // Grayscale (Gray8)
                    if (srcStride == _width)
                    {
                        // No padding, copy all at once
                        using (var ap = new AutoPinner(_rawBytes))
                        {
                            Buffer.MemoryCopy(srcPtr, ap.GetIntPtr().ToPointer(), _rawBytes.Length, _rawBytes.Length);
                        }
                    }
                    else
                    {
                        // Has padding, copy row by row
                        using (var ap = new AutoPinner(_rawBytes))
                        {
                            IntPtr dstPtr = ap.GetIntPtr();
                            for (int row = 0; row < _height; row++)
                            {
                                Buffer.MemoryCopy(srcPtr, dstPtr.ToPointer(), _width, _width);
                                srcPtr += srcStride;
                                dstPtr += _width;
                            }
                        }
                    }
                }
                else if (hasAlpha)
                {
                    // Color with alpha (BGRA/RGBA) -> strip alpha, store as BGR
                    // BGRA: memory layout [B,G,R,A]; RGBA: memory layout [R,G,B,A]
                    int bSrcIdx = bitmap.ColorType == SKColorType.Rgba8888 ? 2 : 0;
                    int rSrcIdx = bitmap.ColorType == SKColorType.Rgba8888 ? 0 : 2;
                    for (int row = 0; row < _height; row++)
                    {
                        byte* rowPtr = srcPtr + row * srcStride;
                        int dstRowOffset = row * _width * 3;
                        for (int col = 0; col < _width; col++)
                        {
                            _rawBytes[dstRowOffset + col * 3 + 0] = rowPtr[col * 4 + bSrcIdx]; // B
                            _rawBytes[dstRowOffset + col * 3 + 1] = rowPtr[col * 4 + 1];       // G
                            _rawBytes[dstRowOffset + col * 3 + 2] = rowPtr[col * 4 + rSrcIdx]; // R
                        }
                    }
                }
                else
                {
                    // Unsupported format
                    throw new NotSupportedException($"Unsupported color type: {bitmap.ColorType}");
                }
            }
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

        /// <summary>
        /// Creates an SKBitmap from the internal raw bytes array.
        /// </summary>
        /// <returns>A new SKBitmap containing the pixel data. Grayscale data creates Gray8 format, color data creates Bgra8888 format.</returns>
        /// <remarks>
        /// For grayscale (1-channel), creates Gray8 bitmap directly.
        /// For color (3-channel BGR), creates Bgra8888 bitmap with alpha set to 255 (opaque).
        /// </remarks>
        public unsafe SKBitmap GetBitmap()
        {
            if (_channels == 1)
            {
                // Grayscale
                var result = new SKBitmap(_width, _height, SKColorType.Gray8, SKAlphaType.Opaque);
                byte* dstPtr = (byte*)result.GetPixels();
                int dstStride = result.RowBytes;

                if (dstStride == _width)
                {
                    // No padding, copy all at once
                    using (var ap = new AutoPinner(_rawBytes))
                    {
                        Buffer.MemoryCopy(ap.GetIntPtr().ToPointer(), dstPtr, _rawBytes.Length, _rawBytes.Length);
                    }
                }
                else
                {
                    // Has padding, copy row by row
                    using (var ap = new AutoPinner(_rawBytes))
                    {
                        IntPtr srcPtr = ap.GetIntPtr();
                        for (int row = 0; row < _height; row++)
                        {
                            Buffer.MemoryCopy(srcPtr.ToPointer(), dstPtr, _width, _width);
                            srcPtr += _width;
                            dstPtr += dstStride;
                        }
                    }
                }

                return result;
            }
            else if (_channels == 3)
            {
                // Color (BGR) -> BGRA with alpha = 255
                var result = new SKBitmap(_width, _height, SKColorType.Bgra8888, SKAlphaType.Opaque);
                byte* dstPtr = (byte*)result.GetPixels();
                int dstStride = result.RowBytes;

                for (int row = 0; row < _height; row++)
                {
                    byte* rowDstPtr = dstPtr + row * dstStride;
                    int srcRowOffset = row * _width * 3;
                    for (int col = 0; col < _width; col++)
                    {
                        rowDstPtr[col * 4 + 0] = _rawBytes[srcRowOffset + col * 3 + 0]; // B
                        rowDstPtr[col * 4 + 1] = _rawBytes[srcRowOffset + col * 3 + 1]; // G
                        rowDstPtr[col * 4 + 2] = _rawBytes[srcRowOffset + col * 3 + 2]; // R
                        rowDstPtr[col * 4 + 3] = 255;                                    // A (opaque)
                    }
                }

                return result;
            }
            else
            {
                throw new NotSupportedException($"Unsupported channel count: {_channels}");
            }
        }
    }
}