using SkiaSharp;

namespace Bwl.Imaging.Skia
{

    /// <summary>
    /// Потокобезопасная обвязка Bitmap-а, с поддержкой
    /// кешированных свойств изображения и JPEG-кеша.
    /// </summary>
    public class BitmapInfo : IDisposable, ICloneable
    {

        #region Shared
        public static bool SafeCloneMode { get; set; } = false;

        /// <summary>
        /// Клонирование Bitnap-а в соотв. с выбранным режимом.
        /// </summary>
        public static SKBitmap BitmapCloneManaged(SKBitmap bmp)
        {
            SKBitmap result = null;
            bool safeCloneMode = SafeCloneMode;
            if (bmp is not null)
            {
                try
                {
                    var clonedBmp = safeCloneMode ? bmp.Copy() : UnsafeFunctions.SKBitmapClone(bmp);
                    if (CheckBitmap(clonedBmp))
                    {
                        result = clonedBmp; // Bitmap прошел проверку
                    }
                    else
                    {
                        throw new Exception("Not CheckBitmap(clonedBmp)");
                    }
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref _globalCloneErrorCount);
                    throw new Exception($"BitmapInfo.BitmapCloneManaged(safeCloneMode:{safeCloneMode}) failed: {ex.Message}");
                }
            }
            return result;
        }

        /// <summary>
        /// Проверка Bitmap-а на рабочее состояние.
        /// </summary>
        public static bool CheckBitmap(SKBitmap bmp)
        {
            if (bmp is null)
                return false;

            // Avoid querying disposed/native-invalid bitmaps: once disposed, SKBitmap.Handle becomes IntPtr.Zero.
            // Calling Width/Height on a disposed bitmap can crash the process (native access violation).
            if (bmp.Handle == IntPtr.Zero)
                return false;

            try
            {
                return bmp.Width > 0 && bmp.Height > 0;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Data
        private byte[] _jpg;
        private SKBitmap _bmp;
        private SKSizeI? _bmpSize;
        private int? _rowBytes;
        private int? _bytesPerPixel;
        private SKColorType? _colorType;
        private SKAlphaType? _alphaType;

        private readonly Semaphore _bmpSemaphore = new Semaphore(1, 1);
        #endregion

        #region Statistic Properties
        /// <summary>
        /// Глобальный счетчик количества выделенных байт Bitmap-ов.
        /// </summary>
        private static long _globalAllocatedDataCount;
        public static long GlobalAllocatedDataCount
        {
            get
            {
                return Interlocked.Read(ref _globalAllocatedDataCount);
            }
        }

        /// <summary>
        /// Глобальный счетчик осуществленных компрессий JPEG (а не переприсваиваний).
        /// </summary>
        private static long _globalCompressedCount;
        public static long GlobalCompressedCount
        {
            get
            {
                return Interlocked.Read(ref _globalCompressedCount);
            }
        }

        /// <summary>
        /// Глобальный счетчик декомпрессий JPEG (с формированием Bitmap-ов).
        /// </summary>
        private static long _globalDecompressedCount;
        public static long GlobalDecompressedCount
        {
            get
            {
                return Interlocked.Read(ref _globalDecompressedCount);
            }
        }

        /// <summary>
        /// Глобальный счетчик количества ошибок декомпресии.
        /// </summary>
        private static long _globalDecompressedErrorCount;
        public static long GlobalDecompressedErrorCount
        {
            get
            {
                return Interlocked.Read(ref _globalDecompressedErrorCount);
            }
        }

        /// <summary>
        /// Глобальный счетчик количества элиминирований Bitmap-ов (Dispose).
        /// </summary>
        private static long _globalDisposeCount;
        public static long GlobalDisposeCount
        {
            get
            {
                return Interlocked.Read(ref _globalDisposeCount);
            }
        }

        /// <summary>
        /// Глобальный счетчик количества ошибок высвобождения Bitmap-ов.
        /// </summary>
        private static long _globalDisposeErrorCount;
        public static long GlobalDisposeErrorCount
        {
            get
            {
                return Interlocked.Read(ref _globalDisposeErrorCount);
            }
        }

        /// <summary>
        /// Глобальный счетчик количества ошибок клонирований Bitmap-ов.
        /// </summary>
        private static long _globalCloneErrorCount;
        public static long GlobalCloneErrorCount
        {
            get
            {
                return Interlocked.Read(ref _globalCloneErrorCount);
            }
        }

        /// <summary>
        /// Счетчик осуществленных компрессий JPEG (а не переприсваиваний).
        /// </summary>
        private long _compressedCount;
        public long CompressedCount
        {
            get
            {
                return Interlocked.Read(ref _compressedCount);
            }
        }

        /// <summary>
        /// Счетчик декомпрессий JPEG (с формированием Bitmap-ов).
        /// </summary>
        private long _decompressedCount;
        public long DecompressedCount
        {
            get
            {
                return Interlocked.Read(ref _decompressedCount);
            }
        }

        /// <summary>
        /// Счетчик количества ошибок декомпресии.
        /// </summary>
        private long _decompressedErrorCount;
        public long DecompressedErrorCount
        {
            get
            {
                return Interlocked.Read(ref _decompressedErrorCount);
            }
        }

        /// <summary>
        /// Счетчик количества элиминирований Bitmap-ов (Dispose).
        /// </summary>
        private long _disposeCount;
        public long DisposeCount
        {
            get
            {
                return Interlocked.Read(ref _disposeCount);
            }
        }

        /// <summary>
        /// Счетчик количества ошибок высвобождения Bitmap-ов.
        /// </summary>
        private long _disposeErrorCount;
        public long DisposeErrorCount
        {
            get
            {
                return Interlocked.Read(ref _disposeErrorCount);
            }
        }

        /// <summary>
        /// Счетчик количества ошибок клонирований Bitmap-ов.
        /// </summary>
        private long _cloneErrorCount;
        public long CloneErrorCount
        {
            get
            {
                return Interlocked.Read(ref _cloneErrorCount);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Время хранения декомпрессированного битмапа (если доступны JPEG-данные для экономии ОЗУ).
        /// </summary>
        public float BitmapKeepTimeS { get; set; } = 5f;

        /// <summary>
        /// Прямой доступ по ссылке к хранимому Bitmap-у.
        /// </summary>
        /// <remarks>
        /// При обращении к Bmp обязательно использовать методы BmpLock() / BmpUnlock().
        /// </remarks>
        public SKBitmap Bmp
        {
            get
            {
                // Перед предоставлением объекта проверяем, выполнен ли Lock() на объекте синхронизации...
                if (_bmpSemaphore.WaitOne(0)) // Если удалось заблокировать ресурс...
                {
                    _bmpSemaphore.Release();
                    throw new Exception("BitmapInfo.Bmp: 'Bmp' property access without using BmpLock() before"); // ...свойство Bmp используется некорректно (то есть без блокировки)!
                }
                return GetBitmapDecodedInternal(); // Под защитой BmpLock()
            }
        }

        /// <summary>
        /// Данные JPEG пусты?
        /// </summary>
        /// <remarks>
        /// Если данных JPEG нет, это не означает, что изображение отсутствует.
        /// Возможно, используется Bitmap.
        /// </remarks>
        public bool JpgIsNothing
        {
            get
            {
                return _jpg is null;
            }
        }

        /// <summary>
        /// Bitmap пуст?
        /// </summary>
        /// <remarks>
        /// Если Bitmap пуст, это не означает, что изображение отсутствует,
        /// возможно, используется хранение в JPEG.
        /// </remarks>
        public bool BmpIsNothing
        {
            get
            {
                return _bmp is null;
            }
        }

        /// <summary>
        /// Данные изображения пусты?
        /// </summary>
        /// <remarks>
        /// Наличие этого флага означает, что данных изображения нет.
        /// </remarks>
        public bool BmpAndJpgAreNothing
        {
            get
            {
                return _bmp is null && _jpg is null;
            }
        }

        /// <summary>
        /// Кешированный размер сохраненного Bitmap/JPEG.
        /// </summary>
        public SKSizeI BmpSize
        {
            get
            {
                var item = _bmpSize;
                if (item is not null)
                {
                    return item.Value;
                }
                else
                {
                    throw new Exception("BitmapInfo.BmpSize is Nothing");
                }
            }
        }

        /// <summary>
        /// Кешированный формат пикселя сохраненного Bitmap/JPEG.
        /// </summary>
        public int RowBytes
        {
            get
            {
                var item = _rowBytes;
                if (item is not null)
                {
                    return item.Value;
                }
                else
                {
                    throw new Exception("BitmapInfo.RowBytes is Nothing");
                }
            }
        }

        public int BytesPerPixel
        {
            get
            {
                var item = _bytesPerPixel;
                if (item is not null)
                {
                    return item.Value;
                }
                else
                {
                    throw new Exception("BitmapInfo.BytesPerPixel is Nothing");
                }
            }
        }

        public SKColorType? ColorType
        {
            get
            {
                var item = _colorType;
                if (item is not null)
                {
                    return item.Value;
                }
                else
                {
                    throw new Exception("BitmapInfo.ColorType is Nothing");
                }
            }
        }

        public SKAlphaType? AlphaType
        {
            get
            {
                var item = _alphaType;
                if (item is not null)
                {
                    return item.Value;
                }
                else
                {
                    throw new Exception("BitmapInfo.AlphaType is Nothing");
                }
            }
        }

        #endregion

        #region Public Methods
        public BitmapInfo()
        {
        }

        public BitmapInfo(byte[] jpg)
        {
            SetJpg(jpg, -1); // Создается новая сущность, без ожидания
        }

        public BitmapInfo(SKBitmap bmp)
        {
            SetBmp(bmp, -1); // Создается новая сущность, без ожидания
        }

        /// <summary>
        /// Блокировка доступа к разделяемому ресурсу (порождает исключение по таймауту).
        /// </summary>
        /// <remarks>  
        /// Типичная схема использования:
        /// 1: .BmpLock() 'Не оборачиваем Try-Catch: может вызвать исключение таймаута и в Finally вызовет в итоге лишний .BmpUnlock()
        /// 2: Try
        /// 3:     ... 'Этот код может вызвать исключения, но если дошли до него - ресурс был получен на .BmpLock() - исключения не было
        /// 4: Catch ex As Exception
        /// 5:     ... 'Обработали исключения кода как требуется...
        /// 6: Finally
        /// 7:     .BmpUnlock() '...и в итоге освободили доступ к разделяемому ресурсу
        /// 8: End Try
        /// </remarks>
        /// <param name="timeoutMs"></param>
        public void BmpLock(int timeoutMs = 10000)
        {
            if (!_bmpSemaphore.WaitOne(timeoutMs))
            {
                throw new Exception($"BitmapInfo.BmpLock(): Timeout, {timeoutMs} ms");
            }
        }

        /// <summary>
        /// Освобождение доступа к разделяемому ресурсу (при попытке выполнить на свободном ресурсе дает исключение).
        /// </summary>
        public void BmpUnlock()
        {
            try
            {
                _bmpSemaphore.Release();
            }
            catch
            {
                throw new Exception("BitmapInfo.BmpUnlock(): Already unlocked");
            }
        }

        /// <summary>
        /// Обеспечивает экономию ОЗУ:
        /// 1 - При наличии Bitmap-а и отсутствии JPEG-а: формируем JPEG на основе Bitmap-а.
        /// 2 - При наличии Bitmap-а и наличии JPEG-а: элиминирование Bitmap-а и переустановка JPEG-а.
        /// </summary>
        /// <remarks>
        /// Q=90 - высокое качество с сохранением мелких деталей и цветных градиентов.
        /// Q=80 - высокое качество с сохранением мелких деталей.
        /// Q=60 - приемлемое качество для технических целей (дальше размер падает медленнее и растут артефакты).
        /// Q=50 - качество отладочного канала.
        /// </remarks>
        /// <param name="quality">Уровень качества JPEG.</param>
        /// <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
        public void Compress(int quality = 80, int timeoutMs = 10000)
        {
            BmpLock(timeoutMs);
            try
            {
                if (_bmp is not null)
                {
                    // Если JPEG-а нет, сформируем. Если есть - битмап был получен декомпрессией JPEG,
                    // просто переустановим JPEG-данные и элиминируем Bitmap, освободив ОЗУ.
                    byte[] jpg = null;
                    if (_jpg is null)
                    {
                        try
                        {
                            jpg = _bmp.Encode(SKEncodedImageFormat.Jpeg, quality).ToArray();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"JpegCodec.Encode() failed: {ex.Message}");
                        }
                        Interlocked.Increment(ref _globalCompressedCount);
                        Interlocked.Increment(ref _compressedCount);
                    }
                    else
                    {
                        jpg = _jpg;
                    }
                    // Этот вызов очень важен несмотря на возможность формальной отработки по ветке 'jpg = _jpg',
                    // т.к. внутри элиминируется Bitmap(), освобождая ОЗУ.
                    SetJpg(jpg, -1); // -1 - для отказа от блокировки/разблокировки разделяемого ресурса
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"BitmapInfo.Compress() failed: {ex.Message}");
            }
            finally
            {
                BmpUnlock();
            }
        }

        /// <summary>
        /// Возвращает JPEG-данные.
        /// </summary>
        /// <param name="createFromBitmapIfEmpty">Сформировать JPEG на основе Bitmap-а (если JPEG-потока нет).</param>
        /// <param name="quality">Уровень качества JPEG.</param>
        /// <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
        /// <remarks>
        /// Формирование JPEG на основе Bitmap-а при вызове данного метода при активном флаге ifEmptyCreateFromBitmap
        /// не приводит к установке внутреннего JPEG-потока или элиминированию исходного Bitmap-а.
        /// Если требуется сжать Bitmap, установить JPEG-поток и затем освободить память от Bitmap-а - используйте метод Compress().
        /// </remarks>
        public byte[] GetJpg(bool createFromBitmapIfEmpty = true, int quality = 80, int timeoutMs = 10000)
        {
            BmpLock(timeoutMs);
            try
            {
                byte[] jpg = ArrayCopy(_jpg);
                if (jpg is null && _bmp is not null && createFromBitmapIfEmpty)
                {
                    try
                    {
                        jpg = _bmp.Encode(SKEncodedImageFormat.Jpeg, quality).ToArray();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"JpegCodec.Encode() failed: {ex.Message}");
                    }
                }
                return jpg;
            }
            catch (Exception ex)
            {
                throw new Exception($"BitmapInfo.GetJpg() failed: {ex.Message}");
            }
            finally
            {
                BmpUnlock();
            }
        }

        /// <summary>
        /// Быстрый метод получения JPEG-данных.
        /// </summary>
        /// <param name="clone">Клонировать массив байт?</param>
        public byte[] GetJpgFast(bool clone = true)
        {
            return clone ? ArrayCopy(_jpg) : _jpg;
        }

        /// <summary>
        /// Быстрый метод сравнения двух JPEG-потоков.
        /// </summary>
        /// <param name="obj">Объект для сравнения.</param>
        public bool CompareJpgFast(BitmapInfo obj)
        {
            if (obj is null)
                return false;

            byte[] jpgMe = GetJpgFast(false); // Без клонирования
            byte[] jpgObj = obj.GetJpgFast(false); // Без клонирования

            // Быстрые проверки
            if (ReferenceEquals(jpgMe, jpgObj))
                return true;

            if (jpgMe is null || jpgObj is null)
                return false;

            if (jpgMe.Length != jpgObj.Length)
                return false;

#if NET5_0_OR_GREATER
            // Используем оптимизированное сравнение (может использовать SIMD на поддерживаемых платформах)
            return jpgMe.SequenceEqual(jpgObj);
#else
            // Начинаем сравнивать байты в обратном порядке для .NET Framework и .NET Standard 2.0
            for (int i = jpgMe.Length - 1; i >= 0; i -= 1)
            {
                if (jpgMe[i] != jpgObj[i])
                {
                    return false;
                }
            }
            return true; // Потоки 100% идентичны
#endif
        }
        /// <summary>
        /// Устанавливает JPEG-данные и элиминирует данные Bitmap-а.
        /// </summary>
        /// <param name="jpg">JPEG-данные.</param>
        /// <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
        public void SetJpg(byte[] jpg, int timeoutMs = 10000)
        {
            int jpgChannelCount = 0; // Количество каналов JPEG
            var jpgSize = GetJpegSize(jpg, ref jpgChannelCount); // Извлекаем данные о размере изображения из JPEG-потока
            if (jpgSize.Width * jpgSize.Height > 0 && new[] { 1, 3, 4 }.Contains(jpgChannelCount))
            {
                if (timeoutMs >= 0)
                {
                    BmpLock(timeoutMs);
                }
                try
                {
                    DisposeBmpInternal(_bmp); // При установке JPEG чистим Bmp
                    jpgChannelCount = jpgChannelCount > 1 ? 4 : 1; // Если каналов больше одного, то при декодировании JPEG-данных в Bitmap используется формат BGRA8888, который имеет 4 канала (B, G, R и A). Если каналов один, то используется формат Gray8.
                    _bmpSize = new SKSizeI(jpgSize.Width, jpgSize.Height);
                    _rowBytes = jpgChannelCount * jpgSize.Width; // Для формата BGRA8888, который используется при декодировании JPEG-данных в Bitmap, количество байт на строку равно 4 (байта на пиксель) умножить на ширину изображения в пикселях.
                    _bytesPerPixel = jpgChannelCount;
                    _colorType = jpgChannelCount > 1 ? SKColorType.Bgra8888 : SKColorType.Gray8;
                    _alphaType = jpgChannelCount > 1 ? SKAlphaType.Premul : SKAlphaType.Opaque;
                    _jpg = jpg;
                }
                catch (Exception ex)
                {
                    _bmpSize = default;
                    _rowBytes = default;
                    _bytesPerPixel = default;
                    _colorType = default;
                    _alphaType = default;
                    _jpg = null;
                    throw ex;
                }
                finally
                {
                    if (timeoutMs >= 0)
                    {
                        BmpUnlock();
                    }
                }
            }
            else
            {
                throw new Exception("BitmapInfo.SetJpg(): Can't parse JPEG header data");
            }
        }

        /// <summary>
        /// Устанавливает Bitmap и элиминирует данные JPEG.
        /// </summary>
        /// <param name="bmp">Bitmap для установки.</param>
        /// <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
        /// <remarks>Несмотря на то, что при вызове этого метода, возможно, есть
        /// "висящий" отложенный Dispose для Bitmap-а, у каждого такого вызова
        /// своя цель, и ложного элиминирования не будет.</remarks>
        public void SetBmp(SKBitmap bmp, int timeoutMs = 10000)
        {
            if (timeoutMs >= 0)
            {
                BmpLock(timeoutMs);
            }
            try
            {
                EliminateJpgInternal(); // При установке Bmp чистим Jpeg
                SetBmpInternal(bmp);
            }
            catch (Exception ex)
            {
                throw new Exception($"BitmapInfo.SetBmp() failed: {ex.Message}");
            }
            finally
            {
                if (timeoutMs >= 0)
                {
                    BmpUnlock();
                }
            }
        }

        /// <summary>
        /// Получение клонированного изображения.
        /// </summary>
        /// <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
        public SKBitmap GetClonedBmp(int timeoutMs = 10000)
        {
            SKBitmap result = null;
            BmpLock(timeoutMs);
            try
            {
                var bmp = GetBitmapDecodedInternal();
                result = BmpCloneInternal(bmp);
            }
            catch (Exception ex)
            {
                throw new Exception($"BitmapInfo.GetClonedBmp() failed: {ex.Message}");
            }
            finally
            {
                BmpUnlock();
            }
            return result;
        }

        /// <summary>
        /// Получение клонированного изображения.
        /// </summary>
        /// <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
        public SKBitmap GetClonedBmpGray(int timeoutMs = 10000)
        {
            SKBitmap result = null;
            BmpLock(timeoutMs);
            try
            {
                var bmp = GetBitmapDecodedInternal();
                if (bmp.ColorType == SKColorType.Gray8)
                {
                    result = BmpCloneInternal(bmp);
                }
                else
                {
                    try
                    {
                        result = UnsafeFunctions.RgbToGray(bmp);
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref _globalCloneErrorCount);
                        Interlocked.Increment(ref _cloneErrorCount);
                        throw new Exception($"UnsafeFunctions.RgbToGray() failed: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"BitmapInfo.GetClonedBmpGray() failed: {ex.Message}");
            }
            finally
            {
                BmpUnlock();
            }
            return result;
        }

        /// <summary>
        /// Получение клонированной копии.
        /// </summary>
        /// <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
        public BitmapInfo GetClonedCopy(int timeoutMs = 10000)
        {
            BitmapInfo result = null;
            BmpLock(timeoutMs);
            try
            {
                if (_jpg is not null)
                {
                    result = new BitmapInfo(ArrayCopy(_jpg));
                }
                else if (_bmp is not null)
                {
                    result = new BitmapInfo(BmpCloneInternal(_bmp));
                }
                else
                {
                    result = new BitmapInfo();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"BitmapInfo.GetClonedCopy() failed: {ex.Message}");
            }
            finally
            {
                BmpUnlock();
            }
            return result;
        }

        /// <summary>
        /// Очистка изображений Bitmap/JPEG.
        /// </summary>
        /// <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
        public void Clear(int timeoutMs = 10000)
        {
            BmpLock(timeoutMs);
            try
            {
                EliminateJpgInternal();
                DisposeBmpInternal(_bmp);
            }
            catch (Exception ex)
            {
                throw new Exception($"BitmapInfo.Clear() failed: {ex.Message}");
            }
            finally
            {
                BmpUnlock();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Установка Bitmap-а.
        /// </summary>
        private void SetBmpInternal(SKBitmap bmp)
        {
            if (CheckBitmap(bmp))
            {
                _bmp = bmp;
                _bmpSize = new SKSizeI(_bmp.Width, _bmp.Height);
                _rowBytes = _bmp.RowBytes;
                _bytesPerPixel = _bmp.RowBytes;
                _colorType = _bmp.ColorType;
                _alphaType = _bmp.AlphaType;
            }
            else
            {
                throw new Exception($"BitmapInfo.SetBmpInternal() failed: Not CheckBitmap(bmp)");
            }
        }

        /// <summary>
        /// Извлечение данных Bitmap-а (если он пуст, и есть JPEG-данные).
        /// </summary>
        private SKBitmap GetBitmapDecodedInternal()
        {
            if (_bmp is null && _jpg is not null)
            {
                var bmp = DecodeJpegInternal(_jpg);
                if (bmp is not null)
                {
                    SetBmpInternal(bmp);
                    BitmapDisposeWithDelay(bmp, BitmapKeepTimeS); // Отложенная очистка битмапа (именного этого, контроль по ссылке)
                }
            }
            return _bmp;
        }

        /// <summary>
        /// Клонирование битмапа с фиксацией ошибок.
        /// </summary>
        private SKBitmap BmpCloneInternal(SKBitmap bmp)
        {
            SKBitmap result = null;
            try
            {
                result = BitmapCloneManaged(bmp);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _cloneErrorCount);
                throw new Exception($"BitmapInfo.BmpCloneInternal() failed: {ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// Декодирование JPEG-потока.
        /// </summary>
        /// <param name="jpg">JPEG-поток.</param>
        /// <returns>Декомпрессированное изображение.</returns>
        private SKBitmap DecodeJpegInternal(byte[] jpg)
        {
            SKBitmap result = null;
            try
            {
                if (jpg is null)
                    throw new Exception("JPEG data is Nothing");

                var bmp = SKEncoder.DecodeJpeg(jpg);

                if (CheckBitmap(bmp))
                {
                    result = bmp;
                    Interlocked.Add(ref _globalAllocatedDataCount,
                        (long)bmp.ByteCount);
                    Interlocked.Increment(ref _globalDecompressedCount);
                    Interlocked.Increment(ref _decompressedCount);
                }
                else
                {
                    Interlocked.Increment(ref _globalDecompressedErrorCount);
                    Interlocked.Increment(ref _decompressedErrorCount);
                    throw new Exception("Not CheckBitmap(bmp)");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"BitmapInfo.DecodeJpegInternal() failed: {ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// Извлечение размера изображения из JPEG-потока без его декодирования.
        /// </summary>
        private SKSizeI GetJpegSize(byte[] jpg, ref int channelCount, int maxBytesSearch = 1024)
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
                    if (jpg[pos + 1] == 0xC0) // 0xFFC0 - маркер начала кадра, далее можно узнать размеры изображения
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

        /// <summary>
        /// Элиминирование JPEG.
        /// </summary>
        private void EliminateJpgInternal()
        {
            _jpg = null;
        }

        /// <summary>
        /// Получение копии массива байт.
        /// </summary>
        private byte[] ArrayCopy(byte[] src)
        {
            if (src is null)
                return null;
            byte[] res = new byte[src.Length];
            Array.Copy(src, res, src.Length);
            return res;
        }
        #endregion

        #region DisposeWithDelay
        /// <summary>
        /// Запуск потока отложенного Dispose для битмапа.
        /// </summary>
        /// <param name="bitmapKeepTimeS">Время доступности декомпрессированного битмапа.</param>
        private void BitmapDisposeWithDelay(SKBitmap bmp, float bitmapKeepTimeS)
        {
            if (bmp is not null && bitmapKeepTimeS >= 0f)
            {
                ResourceCollector<BitmapInfo>.Add(new CollectedBitmap(bmp, bitmapKeepTimeS, _bmpSemaphore, DisposeBmpInternal));
            }
        }

        /// <summary>
        /// Метод для элиминирования определенного битмапа.
        /// </summary>
        /// <param name="target">Целевой Bitmap (элиминирование другой цели невозможно).</param>
        /// <remarks>Если не отслеживать цель для элиминирования, может случиться ситуация, когда
        /// объект будет инициализирован JPEG-ом, с него будет запрошен Bitmap, запустится
        /// отложенные элиминирование после декомпресии, и далее пользователь обработает
        /// декомпрессированное изображение, которое будет установлено, а далее стерто
        /// отложенным Dispose()-ом. При отслеживании ссылки-цели элиминируется в точности
        /// тот Bitmap, что и планировалось.</remarks>
        private void DisposeBmpInternal(SKBitmap target)
        {
            if (CheckBitmap(target))
            {
                try
                {
                    if (ReferenceEquals(_bmp, target))
                        _bmp = null;
                    Interlocked.Add(ref _globalAllocatedDataCount, -1L * target.ByteCount);
                    target.Dispose();
                    Interlocked.Increment(ref _globalDisposeCount);
                    Interlocked.Increment(ref _disposeCount);
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref _globalDisposeErrorCount);
                    Interlocked.Increment(ref _disposeErrorCount);
                    throw new Exception($"BitmapInfo.DisposeBmpInternal() failed: {ex.Message}");
                }
            }
        }
        #endregion

        #region IDisposable
        private int _disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
                Clear(-1); // Бесконечное ожидание, т.к. нужно высвободить ресурс
        }
        #endregion

        #region ICloneable
        public object Clone()
        {
            return GetClonedCopy();
        }
        #endregion
    }

    internal class CollectedBitmap : ICollectedResource
    {

        private SKBitmap _bmp;
        private Semaphore _sema = new Semaphore(1, 1);
        private Action<SKBitmap> _disposeAction;

        public DateTime CollectTimeUtc { get; private set; }

        public CollectedBitmap(SKBitmap bmp, float bitmapKeepTimeS, Semaphore sema, Action<SKBitmap> disposeAction)
        {
            _bmp = bmp;
            _sema = sema;
            _disposeAction = disposeAction;
            CollectTimeUtc = DateTime.UtcNow.AddSeconds((double)bitmapKeepTimeS);
        }

        public bool TryCollect()
        {
            if (!BitmapInfo.CheckBitmap(_bmp))
            {
                _bmp = null; // Already disposed elsewhere
                return true;
            }
            if (_sema.WaitOne(0))
            {
                try
                {
                    _disposeAction(_bmp);
                }
                catch
                {
                }
                finally
                {
                    _sema.Release();
                    _bmp = null;
                }
            }
            return _bmp is null;
        }

        public int CompareTo(ICollectedResource item)
        {
            return CollectTimeUtc.CompareTo(item.CollectTimeUtc);
        }
    }
}