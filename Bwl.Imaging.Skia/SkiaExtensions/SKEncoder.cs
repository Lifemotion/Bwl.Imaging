using SkiaSharp;

namespace Bwl.Imaging.Skia
{
    public static class SKEncoder
    {

        /// <summary>
        /// Decodes a JPEG image from a byte array into an SKBitmap, ensuring the appropriate color format based on the
        /// image's channel count.
        /// </summary>
        /// <remarks>If the JPEG image has a single channel, it is decoded as Gray8; otherwise, it is
        /// decoded as Bgra8888. If the channel count cannot be determined, the default decoder is used, and the
        /// resulting bitmap is ensured to have a color format.</remarks>
        /// <param name="jpegData">The byte array containing the JPEG image data to decode. Cannot be null.</param>
        /// <returns>An SKBitmap representing the decoded JPEG image, using either the Bgra8888 or Gray8 color format depending
        /// on the image's channel count.</returns>
        public static SKBitmap DecodeJpeg(byte[] jpegData)
        {
            // Checking the jpeg data bytes to get image data
            int channelCount = -1;
            var size = GetJpegSize(jpegData, ref channelCount);
            if (channelCount > 0 && size.Width > 0 && size.Height > 0)
            {
                // If channel count is 1 then it's gray8; otherwise we decode it to bgra8888
                var colorType = channelCount == 1 ? SKColorType.Gray8 : SKColorType.Bgra8888;
                var info = new SKImageInfo(size.Width, size.Height, colorType);
                var bitmap = SKBitmap.Decode(jpegData, info);
                if (bitmap != null) return bitmap;
            }
            // Use default decoder, just check for color
            var fallback = SKBitmap.Decode(jpegData);
            return fallback?.EnsureColor();
        }


        /// <summary>
        /// Извлечение размера изображения из JPEG-потока без его декодирования.
        /// </summary>
        private static SKSizeI GetJpegSize(byte[] jpg, ref int channelCount, int maxBytesSearch = 1024)
        {
            // Инициализация фиктивными значениями, чтобы фиксировать ситуации "не считывания"
            channelCount = -1;
            var res = new SKSizeI(-1, -1);
            try
            {
                int pos = 0; // Начальная позиция в файле
                if (jpg[pos] != 0xFF || jpg[pos + 1] != 0xD8)
                    return res; // Ошибка - не найден стартовый маркер
                pos += 2; // Перешагиваем через маркер
                int blockLength = 0; // Длина блока пока неизвестна...
                while (pos < Math.Min(maxBytesSearch, jpg.Length)) // Работа в пределах допустимой области файла
                {
                    pos += blockLength; // Переходим к следующему блоку
                    if (pos > jpg.Length - 2)
                        return res; // Выход за пределы массива: "jpg(pos + 1)"; Ошибка - вышли за пределы области данных для поиска
                    if (jpg[pos] != 0xFF)
                        return res; // Проверка на то, действительно ли мы перешли на заголовок блока (если нет - ошибка); Ошибка - не обнаружен признак маркера блока
                    // SOF markers: 0xC0 (Baseline), 0xC1 (Extended), 0xC2 (Progressive), 0xC3, 0xC5-0xC7, 0xC9-0xCB, 0xCD-0xCF
                    // Exclude: 0xC4 (DHT), 0xC8 (JPG), 0xCC (DAC)
                    byte marker = jpg[pos + 1];
                    if ((marker >= 0xC0 && marker <= 0xC3) || (marker >= 0xC5 && marker <= 0xC7) ||
                        (marker >= 0xC9 && marker <= 0xCB) || (marker >= 0xCD && marker <= 0xCF))
                    {
                        // Структура блока 0xFFC0: [0xFFC0][ushort length][uchar precision][ushort x][ushort y]
                        int height = jpg[pos + 5] * 256 + jpg[pos + 6];
                        int width = jpg[pos + 7] * 256 + jpg[pos + 8];
                        channelCount = jpg[pos + 9];
                        res = new SKSizeI(width, height);
                        return res; // Данные о размере изображения считаны
                    }
                    else
                    {
                        pos += 2; // Перешагиваем через маркер блока...
                        blockLength = jpg[pos] * 256 + jpg[pos + 1];
                    } // ...и переходим к следующему
                }
                return res; // Ошибка - исчерпали данные для поиска
            }
            catch (Exception ex)
            {
                channelCount = -1;
                res = new SKSizeI(-1, -1);
            }
            return res;
        }

        public static SKBitmap DecodeBitmap(byte[] bitmapData)
        {
            int channelCount = -1;
            var size = GetBitmapSize(bitmapData, ref channelCount);
            if (channelCount > 0 && size.Width > 0 && size.Height > 0)
            {
                var colorType = channelCount == 1 ? SKColorType.Gray8 : SKColorType.Bgra8888;
                var info = new SKImageInfo(size.Width, size.Height, colorType);
                var bitmap = SKBitmap.Decode(bitmapData, info);
                if (bitmap != null) return bitmap;
            }
            var fallback = SKBitmap.Decode(bitmapData);
            return fallback?.EnsureColor();
        }

        /// <summary>
        /// Get size and channel count from a bitmap image for correct decode
        /// </summary>
        /// <param name="bmpData"></param>
        /// <param name="channelCount"></param>
        /// <returns></returns>
        private static SKSizeI GetBitmapSize(byte[] bmpData, ref int channelCount)
        {
            channelCount = -1;
            var res = new SKSizeI(-1, -1);
            try
            {
                if (bmpData == null || bmpData.Length < 30)
                    return res;

                if (bmpData[0] != 0x42 || bmpData[1] != 0x4D) // "BM"
                    return res;

                int dibHeaderSize = ReadInt32LE(bmpData, 14);
                if (dibHeaderSize < 12 || bmpData.Length < 14 + dibHeaderSize)
                    return res;

                int width;
                int height;
                int bitsPerPixel;

                if (dibHeaderSize == 12)
                {
                    width = ReadUInt16LE(bmpData, 18);
                    height = ReadUInt16LE(bmpData, 20);
                    bitsPerPixel = ReadUInt16LE(bmpData, 24);
                }
                else
                {
                    width = ReadInt32LE(bmpData, 18);
                    height = Math.Abs(ReadInt32LE(bmpData, 22));
                    bitsPerPixel = ReadUInt16LE(bmpData, 28);
                }

                if (width <= 0 || height <= 0)
                    return res;

                channelCount = GetBitmapChannelCount(bmpData, dibHeaderSize, bitsPerPixel);
                res = new SKSizeI(width, height);
            }
            catch
            {
                channelCount = -1;
                res = new SKSizeI(-1, -1);
            }
            return res;
        }

        public static SKBitmap DecodePng(byte[] pngData)
        {
            int channelCount = -1;
            var size = GetPngSize(pngData, ref channelCount);
            if (channelCount > 0 && size.Width > 0 && size.Height > 0)
            {
                var colorType = channelCount == 1 ? SKColorType.Gray8 : SKColorType.Bgra8888;
                var info = new SKImageInfo(size.Width, size.Height, colorType);
                var bitmap = SKBitmap.Decode(pngData, info);
                if (bitmap != null) return bitmap;
            }
            var fallback = SKBitmap.Decode(pngData);
            return fallback?.EnsureColor();
        }

        /// <summary>
        /// Get size and channel count from a png image for correct decode
        /// </summary>
        /// <param name="pngData"></param>
        /// <param name="channelCount"></param>
        /// <returns></returns>
        private static SKSizeI GetPngSize(byte[] pngData, ref int channelCount)
        {
            channelCount = -1;
            var res = new SKSizeI(-1, -1);
            try
            {
                if (pngData == null || pngData.Length < 33)
                    return res;

                // PNG signature
                if (pngData[0] != 0x89 || pngData[1] != 0x50 || pngData[2] != 0x4E || pngData[3] != 0x47 ||
                    pngData[4] != 0x0D || pngData[5] != 0x0A || pngData[6] != 0x1A || pngData[7] != 0x0A)
                {
                    return res;
                }

                int ihdrLength = ReadInt32BE(pngData, 8);
                if (ihdrLength != 13)
                    return res;

                if (pngData[12] != 0x49 || pngData[13] != 0x48 || pngData[14] != 0x44 || pngData[15] != 0x52) // IHDR
                    return res;

                int width = ReadInt32BE(pngData, 16);
                int height = ReadInt32BE(pngData, 20);
                int colorType = pngData[25];

                if (width <= 0 || height <= 0)
                    return res;

                channelCount = GetPngChannelCount(colorType);
                res = new SKSizeI(width, height);
            }
            catch
            {
                channelCount = -1;
                res = new SKSizeI(-1, -1);
            }

            return res;
        }

        private static int GetPngChannelCount(int colorType)
        {
            switch (colorType)
            {
                case 0: return 1; // Grayscale
                case 2: return 3; // Truecolor
                case 3: return 3; // Indexed color
                case 4: return 2; // Grayscale + alpha
                case 6: return 4; // Truecolor + alpha
                default: return -1;
            }
        }

        private static int GetBitmapChannelCount(byte[] bmpData, int dibHeaderSize, int bitsPerPixel)
        {
            switch (bitsPerPixel)
            {
                case 32:
                    return 4;
                case 24:
                    return 3;
                case 16:
                    return 3;
                case 8:
                    return IsBitmapPaletteGrayscale(bmpData, dibHeaderSize, bitsPerPixel) ? 1 : 3;
                default:
                    return -1;
            }
        }

        private static bool IsBitmapPaletteGrayscale(byte[] bmpData, int dibHeaderSize, int bitsPerPixel)
        {
            if (bitsPerPixel > 8)
                return false;

            int colorsUsedOffset = 14 + 32;
            int colorsUsed = 0;
            if (dibHeaderSize >= 40 && bmpData.Length >= colorsUsedOffset + 4)
                colorsUsed = ReadInt32LE(bmpData, colorsUsedOffset);

            int paletteColors = colorsUsed > 0 ? colorsUsed : (1 << bitsPerPixel);
            int paletteOffset = 14 + dibHeaderSize;
            if (paletteColors <= 0 || bmpData.Length < paletteOffset + paletteColors * 4)
                return false;

            for (int i = 0; i < paletteColors; i++)
            {
                int p = paletteOffset + i * 4;
                byte b = bmpData[p];
                byte g = bmpData[p + 1];
                byte r = bmpData[p + 2];
                if (r != g || g != b)
                    return false;
            }

            return true;
        }

        private static int ReadUInt16LE(byte[] data, int offset)
        {
            return data[offset] | (data[offset + 1] << 8);
        }

        private static int ReadInt32LE(byte[] data, int offset)
        {
            return data[offset] |
                   (data[offset + 1] << 8) |
                   (data[offset + 2] << 16) |
                   (data[offset + 3] << 24);
        }

        private static int ReadInt32BE(byte[] data, int offset)
        {
            return (data[offset] << 24) |
                   (data[offset + 1] << 16) |
                   (data[offset + 2] << 8) |
                    data[offset + 3];
        }


        /// <summary>
        /// Ensures that the specified bitmap is in a color format, converting it if necessary.
        /// </summary>
        /// <remarks>If the source bitmap is detected as grayscale, it is converted to a color format
        /// using a new bitmap. If the source is already in the correct color format, it is returned as is. The
        /// conversion process uses a canvas to draw the original bitmap onto a new bitmap with the target color
        /// type.</remarks>
        /// <param name="source">The source bitmap to check and convert if required. Cannot be null.</param>
        /// <returns>A bitmap in the appropriate color format. Returns null if the source bitmap is null.</returns>
        public static SKBitmap EnsureColor(this SKBitmap source)
        {
            if (source == null) return null;

            // 1. Быстрая проверка на цветность через уменьшенную копию
            bool isGrayscale = CheckIfGrayscaleFast(source, 32);

            // 2. Определяем целевой формат
            SKColorType targetType = isGrayscale ? SKColorType.Gray8 : SKColorType.Bgra8888;

            // 3. Если формат уже совпадает, возвращаем как есть (или копию)
            if (source.ColorType == targetType)
            {
                return source;
            }

            // 4. Конвертация
            var info = new SKImageInfo(source.Width, source.Height, targetType);
            var targetBitmap = new SKBitmap(info);

            using (var canvas = new SKCanvas(targetBitmap))
            {
                // SkiaSharp сама применит GrayScale фильтр при отрисовке в Gray8 битмап
                canvas.DrawBitmap(source, 0, 0);
            }

            source.Dispose();
            return targetBitmap;
        }

        /// <summary>
        /// Determines whether the specified bitmap image is grayscale by analyzing a downscaled version of the image.
        /// </summary>
        /// <remarks>This method performs a fast analysis by scaling the original image to a smaller size
        /// and checking the color channel differences for each pixel. A threshold of 2 is used to account for minor
        /// variations introduced by compression or scaling. This approach provides a balance between performance and
        /// accuracy for grayscale detection.</remarks>
        /// <param name="bitmap">The bitmap image to analyze. This parameter must not be null.</param>
        /// <param name="sampleSize">The width and height, in pixels, of the downscaled image used for analysis. Must be a positive integer.</param>
        /// <returns>true if the bitmap is determined to be grayscale; otherwise, false.</returns>
        private static unsafe bool CheckIfGrayscaleFast(SKBitmap bitmap, int sampleSize)
        {
            // Создаем маленькую превьюшку для анализа
            var info = new SKImageInfo(sampleSize, sampleSize, SKColorType.Bgra8888);
            using var temp = new SKBitmap(info);
            bitmap.ScalePixels(temp, SKSamplingOptions.Default);
            byte* ptr = (byte*)temp.GetPixels().ToPointer();
            int pixels = temp.Width * temp.Height;
            for (int i = 0; i < pixels; i++)
            {
                byte b = *ptr++;
                byte g = *ptr++;
                byte r = *ptr++;
                ptr++; // Alpha

                // Порог (threshold) в 2 единицы, так как JPEG и масштабирование шумят
                if (Math.Abs(r - g) > 2 || Math.Abs(g - b) > 2 || Math.Abs(r - b) > 2)
                    return false;
            }
            return true;
        }
    }
}
