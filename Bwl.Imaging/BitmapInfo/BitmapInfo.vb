Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Bwl.Imaging.Unsafe

''' <summary>
''' Потокобезопасная обвязка Bitmap-а, с поддержкой
''' кешированных свойств изображения и JPEG-кеша.
''' </summary>
Public Class BitmapInfo
    Implements IDisposable
    Implements ICloneable

#Region "Shared"
    ''' <summary>
    ''' Глобальный режим клонирования Bitmap-ов.
    ''' </summary>
    Private Shared _safeCloneMode As Boolean = False
    Public Shared Property SafeCloneMode As Boolean
        Get
            Return _safeCloneMode
        End Get
        Set(value As Boolean)
            _safeCloneMode = value
        End Set
    End Property

    ''' <summary>
    ''' Клонирование Bitnap-а в соотв. с выбранным режимом.
    ''' </summary>
    Public Shared Function BitmapCloneManaged(bmp As Bitmap) As Bitmap
        Dim result As Bitmap = Nothing
        Dim safeCloneMode = _safeCloneMode
        If bmp IsNot Nothing Then
            Try
                Dim clonedBmp = If(safeCloneMode, New Bitmap(bmp), UnsafeFunctions.BitmapClone(bmp))
                If CheckBitmap(clonedBmp) Then
                    result = clonedBmp 'Bitmap прошел проверку
                Else
                    Throw New Exception("Not CheckBitmap(clonedBmp)")
                End If
            Catch ex As Exception
                Interlocked.Increment(_globalCloneErrorCount)
                Throw New Exception($"BitmapInfo.BitmapCloneManaged(safeCloneMode:{safeCloneMode}) failed: {ex.Message}")
            End Try
        End If
        Return result
    End Function

    ''' <summary>
    ''' Проверка Bitmap-а на рабочее состояние.
    ''' </summary>
    Public Shared Function CheckBitmap(bmp As Bitmap) As Boolean
        Dim result As Boolean
        Try
            result = bmp IsNot Nothing AndAlso bmp.PixelFormat <> PixelFormat.DontCare AndAlso bmp.Width * bmp.Height > 0
        Catch
            result = False
        End Try
        Return result
    End Function
#End Region

#Region "Data"
    Private _jpg As Byte()
    Private _bmp As Bitmap
    Private _bmpSize As Nullable(Of Size)
    Private _bmpPixelFormat As Nullable(Of PixelFormat)

    Private ReadOnly _bmpSemaphore As New Semaphore(1, 1)
#End Region

#Region "Statistic Properties"
    ''' <summary>
    ''' Глобальный счетчик количества выделенных байт Bitmap-ов.
    ''' </summary>
    Private Shared _globalAllocatedDataCount As Long
    Public Shared ReadOnly Property GlobalAllocatedDataCount As Long
        Get
            Return Interlocked.Read(_globalAllocatedDataCount)
        End Get
    End Property

    ''' <summary>
    ''' Глобальный счетчик осуществленных компрессий JPEG (а не переприсваиваний).
    ''' </summary>
    Private Shared _globalCompressedCount As Long
    Public Shared ReadOnly Property GlobalCompressedCount As Long
        Get
            Return Interlocked.Read(_globalCompressedCount)
        End Get
    End Property

    ''' <summary>
    ''' Глобальный счетчик декомпрессий JPEG (с формированием Bitmap-ов).
    ''' </summary>
    Private Shared _globalDecompressedCount As Long
    Public Shared ReadOnly Property GlobalDecompressedCount As Long
        Get
            Return Interlocked.Read(_globalDecompressedCount)
        End Get
    End Property

    ''' <summary>
    ''' Глобальный счетчик количества ошибок декомпресии.
    ''' </summary>
    Private Shared _globalDecompressedErrorCount As Long
    Public Shared ReadOnly Property GlobalDecompressedErrorCount As Long
        Get
            Return Interlocked.Read(_globalDecompressedErrorCount)
        End Get
    End Property

    ''' <summary>
    ''' Глобальный счетчик количества элиминирований Bitmap-ов (Dispose).
    ''' </summary>
    Private Shared _globalDisposeCount As Long
    Public Shared ReadOnly Property GlobalDisposeCount As Long
        Get
            Return Interlocked.Read(_globalDisposeCount)
        End Get
    End Property

    ''' <summary>
    ''' Глобальный счетчик количества ошибок высвобождения Bitmap-ов.
    ''' </summary>
    Private Shared _globalDisposeErrorCount As Long
    Public Shared ReadOnly Property GlobalDisposeErrorCount As Long
        Get
            Return Interlocked.Read(_globalDisposeErrorCount)
        End Get
    End Property

    ''' <summary>
    ''' Глобальный счетчик количества ошибок клонирований Bitmap-ов.
    ''' </summary>
    Private Shared _globalCloneErrorCount As Long
    Public Shared ReadOnly Property GlobalCloneErrorCount As Long
        Get
            Return Interlocked.Read(_globalCloneErrorCount)
        End Get
    End Property

    ''' <summary>
    ''' Счетчик осуществленных компрессий JPEG (а не переприсваиваний).
    ''' </summary>
    Private _compressedCount As Long
    Public ReadOnly Property CompressedCount As Long
        Get
            Return Interlocked.Read(_compressedCount)
        End Get
    End Property

    ''' <summary>
    ''' Счетчик декомпрессий JPEG (с формированием Bitmap-ов).
    ''' </summary>
    Private _decompressedCount As Long
    Public ReadOnly Property DecompressedCount As Long
        Get
            Return Interlocked.Read(_decompressedCount)
        End Get
    End Property

    ''' <summary>
    ''' Счетчик количества ошибок декомпресии.
    ''' </summary>
    Private _decompressedErrorCount As Long
    Public ReadOnly Property DecompressedErrorCount As Long
        Get
            Return Interlocked.Read(_decompressedErrorCount)
        End Get
    End Property

    ''' <summary>
    ''' Счетчик количества элиминирований Bitmap-ов (Dispose).
    ''' </summary>
    Private _disposeCount As Long
    Public ReadOnly Property DisposeCount As Long
        Get
            Return Interlocked.Read(_disposeCount)
        End Get
    End Property

    ''' <summary>
    ''' Счетчик количества ошибок высвобождения Bitmap-ов.
    ''' </summary>
    Private _disposeErrorCount As Long
    Public ReadOnly Property DisposeErrorCount As Long
        Get
            Return Interlocked.Read(_disposeErrorCount)
        End Get
    End Property

    ''' <summary>
    ''' Счетчик количества ошибок клонирований Bitmap-ов.
    ''' </summary>
    Private _cloneErrorCount As Long
    Public ReadOnly Property CloneErrorCount As Long
        Get
            Return Interlocked.Read(_cloneErrorCount)
        End Get
    End Property
#End Region

#Region "Properties"
    ''' <summary>
    ''' Время хранения декомпрессированного битмапа (если доступны JPEG-данные для экономии ОЗУ).
    ''' </summary>
    Public Property BitmapKeepTimeS As Single = 5

    ''' <summary>
    ''' Прямой доступ по ссылке к хранимому Bitmap-у.
    ''' </summary>
    '''<remarks>
    ''' При обращении к Bmp обязательно использовать методы BmpLock() / BmpUnlock().
    '''</remarks>
    Public ReadOnly Property Bmp As Bitmap
        Get
            'Перед предоставлением объекта проверяем, выполнен ли Lock() на объекте синхронизации...
            If _bmpSemaphore.WaitOne(0) Then 'Если удалось заблокировать ресурс...
                _bmpSemaphore.Release()
                Throw New Exception("BitmapInfo.Bmp: 'Bmp' property access without using BmpLock() before") '...свойство Bmp используется некорректно (то есть без блокировки)!
            End If
            Return GetBitmapDecodedInternal() 'Под защитой BmpLock()
        End Get
    End Property

    ''' <summary>
    ''' Данные JPEG пусты?
    ''' </summary>
    ''' <remarks>
    ''' Если данных JPEG нет, это не означает, что изображение отсутствует.
    ''' Возможно, используется Bitmap.
    ''' </remarks>
    Public ReadOnly Property JpgIsNothing As Boolean
        Get
            Return _jpg Is Nothing
        End Get
    End Property

    ''' <summary>
    ''' Bitmap пуст?
    ''' </summary>
    '''<remarks>
    '''Если Bitmap пуст, это не означает, что изображение отсутствует,
    '''возможно, используется хранение в JPEG.
    '''</remarks>
    Public ReadOnly Property BmpIsNothing As Boolean
        Get
            Return _bmp Is Nothing
        End Get
    End Property

    ''' <summary>
    ''' Данные изображения пусты?
    ''' </summary>
    '''<remarks>
    '''Наличие этого флага означает, что данных изображения нет.
    '''</remarks>
    Public ReadOnly Property BmpAndJpgAreNothing As Boolean
        Get
            Return _bmp Is Nothing AndAlso _jpg Is Nothing
        End Get
    End Property

    ''' <summary>
    ''' Кешированный размер сохраненного Bitmap/JPEG.
    ''' </summary>
    Public ReadOnly Property BmpSize As Size
        Get
            Dim item = _bmpSize
            If item IsNot Nothing Then
                Return item.Value
            Else
                Throw New Exception("BitmapInfo.BmpSize is Nothing")
            End If
        End Get
    End Property

    ''' <summary>
    ''' Кешированный формат пикселя сохраненного Bitmap/JPEG.
    ''' </summary>
    Public ReadOnly Property BmpPixelFormat As PixelFormat
        Get
            Dim item = _bmpPixelFormat
            If item IsNot Nothing Then
                Return item.Value
            Else
                Throw New Exception("BitmapInfo.BmpPixelFormat is Nothing")
            End If
        End Get
    End Property
#End Region

#Region "Public Methods"
    Public Sub New()
    End Sub

    Public Sub New(jpg As Byte())
        SetJpg(jpg, -1) 'Создается новая сущность, без ожидания
    End Sub

    Public Sub New(bmp As Bitmap)
        SetBmp(bmp, -1) 'Создается новая сущность, без ожидания
    End Sub

    ''' <summary>
    ''' Блокировка доступа к разделяемому ресурсу (порождает исключение по таймауту).
    ''' </summary>
    ''' <remarks>  
    ''' Типичная схема использования:
    ''' 1: .BmpLock() 'Не оборачиваем Try-Catch: может вызвать исключение таймаута и в Finally вызовет в итоге лишний .BmpUnlock()
    ''' 2: Try
    ''' 3:     ... 'Этот код может вызвать исключения, но если дошли до него - ресурс был получен на .BmpLock() - исключения не было
    ''' 4: Catch ex As Exception
    ''' 5:     ... 'Обработали исключения кода как требуется...
    ''' 6: Finally
    ''' 7:     .BmpUnlock() '...и в итоге освободили доступ к разделяемому ресурсу
    ''' 8: End Try
    ''' </remarks>
    ''' <param name="timeoutMs"></param>
    Public Sub BmpLock(Optional timeoutMs As Integer = 10000)
        If Not _bmpSemaphore.WaitOne(timeoutMs) Then
            Throw New Exception($"BitmapInfo.BmpLock(): Timeout, {timeoutMs} ms")
        End If
    End Sub

    ''' <summary>
    ''' Освобождение доступа к разделяемому ресурсу (при попытке выполнить на свободном ресурсе дает исключение).
    ''' </summary>
    Public Sub BmpUnlock()
        Try
            _bmpSemaphore.Release()
        Catch
            Throw New Exception("BitmapInfo.BmpUnlock(): Already unlocked")
        End Try
    End Sub

    ''' <summary>
    ''' Обеспечивает экономию ОЗУ:
    ''' 1 - При наличии Bitmap-а и отсутствии JPEG-а: формируем JPEG на основе Bitmap-а.
    ''' 2 - При наличии Bitmap-а и наличии JPEG-а: элиминирование Bitmap-а и переустановка JPEG-а.
    ''' </summary>
    ''' <remarks>
    ''' Q=90 - высокое качество с сохранением мелких деталей и цветных градиентов.
    ''' Q=80 - высокое качество с сохранением мелких деталей.
    ''' Q=60 - приемлемое качество для технических целей (дальше размер падает медленнее и растут артефакты).
    ''' Q=50 - качество отладочного канала.
    ''' </remarks>
    ''' <param name="quality">Уровень качества JPEG.</param>
    ''' <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
    Public Sub Compress(Optional quality As Integer = 80,
                        Optional timeoutMs As Integer = 10000)
        BmpLock(timeoutMs)
        Try
            If _bmp IsNot Nothing Then
                'Если JPEG-а нет, сформируем. Если есть - битмап был получен декомпрессией JPEG,
                'просто переустановим JPEG-данные и элиминируем Bitmap, освободив ОЗУ.
                Dim jpg As Byte() = Nothing
                If _jpg Is Nothing Then
                    Try
                        jpg = JpegCodec.Encode(_bmp, quality).ToArray()
                    Catch ex As Exception
                        Throw New Exception($"JpegCodec.Encode() failed: {ex.Message}")
                    End Try
                    Interlocked.Increment(_globalCompressedCount)
                    Interlocked.Increment(_compressedCount)
                Else
                    jpg = _jpg
                End If
                'Этот вызов очень важен несмотря на возможность формальной отработки по ветке 'jpg = _jpg',
                'т.к. внутри элиминируется Bitmap(), освобождая ОЗУ.
                SetJpg(jpg, -1) '-1 - для отказа от блокировки/разблокировки разделяемого ресурса
            End If
        Catch ex As Exception
            Throw New Exception($"BitmapInfo.Compress() failed: {ex.Message}")
        Finally
            BmpUnlock()
        End Try
    End Sub

    ''' <summary>
    ''' Возвращает JPEG-данные.
    ''' </summary>
    ''' <param name="createFromBitmapIfEmpty">Сформировать JPEG на основе Bitmap-а (если JPEG-потока нет).</param>
    ''' <param name="quality">Уровень качества JPEG.</param>
    ''' <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
    ''' <remarks>
    ''' Формирование JPEG на основе Bitmap-а при вызове данного метода при активном флаге ifEmptyCreateFromBitmap
    ''' не приводит к установке внутреннего JPEG-потока или элиминированию исходного Bitmap-а.
    ''' Если требуется сжать Bitmap, установить JPEG-поток и затем освободить память от Bitmap-а - используйте метод Compress().
    ''' </remarks>
    Public Function GetJpg(Optional createFromBitmapIfEmpty As Boolean = True,
                           Optional quality As Integer = 80,
                           Optional timeoutMs As Integer = 10000) As Byte()
        BmpLock(timeoutMs)
        Try
            Dim jpg = ArrayCopy(_jpg)
            If jpg Is Nothing AndAlso _bmp IsNot Nothing AndAlso createFromBitmapIfEmpty Then
                Try
                    jpg = JpegCodec.Encode(_bmp, quality).ToArray()
                Catch ex As Exception
                    Throw New Exception($"JpegCodec.Encode() failed: {ex.Message}")
                End Try
            End If
            Return jpg
        Catch ex As Exception
            Throw New Exception($"BitmapInfo.GetJpg() failed: {ex.Message}")
        Finally
            BmpUnlock()
        End Try
    End Function

    ''' <summary>
    ''' Быстрый метод получения JPEG-данных.
    ''' </summary>
    ''' <param name="clone">Клонировать массив байт?</param>
    Public Function GetJpgFast(Optional clone As Boolean = True) As Byte()
        Return If(clone, ArrayCopy(_jpg), _jpg)
    End Function

    ''' <summary>
    ''' Быстрый метод сравнения двух JPEG-потоков.
    ''' </summary>
    ''' <param name="obj">Объект для сравнения.</param>
    Public Function CompareJpgFast(obj As BitmapInfo) As Boolean
        Dim jpgMe = Me.GetJpgFast(False) 'Без клонирования
        Dim jpgObj = obj.GetJpgFast(False) 'Без клонирования
        'Две последовательности байт отличаются, если...
        If jpgMe.Length <> jpgObj.Length Then '...их длины не равны
            Return False
        Else
            'Начинаем сравнивать байты в обратном порядке
            For i = jpgMe.Length - 1 To 0 Step -1
                If jpgMe(i) <> jpgObj(i) Then
                    Return False
                End If
            Next
        End If
        Return True 'Потоки 100% идентичны
    End Function

    ''' <summary>
    ''' Устанавливает JPEG-данные и элиминирует данные Bitmap-а.
    ''' </summary>
    ''' <param name="jpg">JPEG-данные.</param>
    ''' <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
    Public Sub SetJpg(jpg As Byte(), Optional timeoutMs As Integer = 10000)
        Dim jpgChannelCount = 0 'Количество каналов JPEG
        Dim jpgSize = GetJpegSize(jpg, jpgChannelCount) 'Извлекаем данные о размере изображения из JPEG-потока
        If (jpgSize.Width * jpgSize.Height > 0) AndAlso {1, 3}.Contains(jpgChannelCount) Then
            If timeoutMs >= 0 Then
                BmpLock(timeoutMs)
            End If
            Try
                DisposeBmpInternal(_bmp) 'При установке JPEG чистим Bmp
                _bmpSize = jpgSize
                _bmpPixelFormat = If(jpgChannelCount = 3, PixelFormat.Format24bppRgb, PixelFormat.Format8bppIndexed)
                _jpg = jpg
            Catch ex As Exception
                _bmpSize = Nothing
                _bmpPixelFormat = Nothing
                _jpg = Nothing
                Throw ex
            Finally
                If timeoutMs >= 0 Then
                    BmpUnlock()
                End If
            End Try
        Else
            Throw New Exception("BitmapInfo.SetJpg(): Can't parse JPEG header data")
        End If
    End Sub

    ''' <summary>
    ''' Устанавливает Bitmap и элиминирует данные JPEG.
    ''' </summary>
    ''' <param name="bmp">Bitmap для установки.</param>
    ''' <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
    ''' <remarks>Несмотря на то, что при вызове этого метода, возможно, есть
    ''' "висящий" отложенный Dispose для Bitmap-а, у каждого такого вызова
    ''' своя цель, и ложного элиминирования не будет.</remarks>
    Public Sub SetBmp(bmp As Bitmap, Optional timeoutMs As Integer = 10000)
        If timeoutMs >= 0 Then
            BmpLock(timeoutMs)
        End If
        Try
            EliminateJpgInternal() 'При установке Bmp чистим Jpeg
            SetBmpInternal(bmp)
        Catch ex As Exception
            Throw New Exception($"BitmapInfo.SetBmp() failed: {ex.Message}")
        Finally
            If timeoutMs >= 0 Then
                BmpUnlock()
            End If
        End Try
    End Sub

    ''' <summary>
    ''' Получение клонированного изображения.
    ''' </summary>
    ''' <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
    Public Function GetClonedBmp(Optional timeoutMs As Integer = 10000) As Bitmap
        Dim result As Bitmap = Nothing
        BmpLock(timeoutMs)
        Try
            Dim bmp = GetBitmapDecodedInternal()
            result = BmpCloneInternal(bmp)
        Catch ex As Exception
            Throw New Exception($"BitmapInfo.GetClonedBmp() failed: {ex.Message}")
        Finally
            BmpUnlock()
        End Try
        Return result
    End Function

    ''' <summary>
    ''' Получение клонированного изображения.
    ''' </summary>
    ''' <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
    Public Function GetClonedBmpGray(Optional timeoutMs As Integer = 10000) As Bitmap
        Dim result As Bitmap = Nothing
        BmpLock(timeoutMs)
        Try
            Dim bmp = GetBitmapDecodedInternal()
            If bmp.PixelFormat = Drawing.Imaging.PixelFormat.Format8bppIndexed Then
                result = BmpCloneInternal(bmp)
            Else
                Try
                    result = UnsafeFunctions.RgbToGray(bmp)
                Catch ex As Exception
                    Interlocked.Increment(_globalCloneErrorCount)
                    Interlocked.Increment(_cloneErrorCount)
                    Throw New Exception($"UnsafeFunctions.RgbToGray() failed: {ex.Message}")
                End Try
            End If
        Catch ex As Exception
            Throw New Exception($"BitmapInfo.GetClonedBmpGray() failed: {ex.Message}")
        Finally
            BmpUnlock()
        End Try
        Return result
    End Function

    ''' <summary>
    ''' Получение клонированной копии.
    ''' </summary>
    ''' <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
    Public Function GetClonedCopy(Optional timeoutMs As Integer = 10000) As BitmapInfo
        Dim result As BitmapInfo = Nothing
        BmpLock(timeoutMs)
        Try
            If _jpg IsNot Nothing Then
                result = New BitmapInfo(ArrayCopy(_jpg))
            ElseIf _bmp IsNot Nothing Then
                result = New BitmapInfo(BmpCloneInternal(_bmp))
            End If
        Catch ex As Exception
            Throw New Exception($"BitmapInfo.GetClonedCopy() failed: {ex.Message}")
        Finally
            BmpUnlock()
        End Try
        Return result
    End Function

    ''' <summary>
    ''' Очистка изображений Bitmap/JPEG.
    ''' </summary>
    ''' <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
    Public Sub Clear(Optional timeoutMs As Integer = 10000)
        BmpLock(timeoutMs)
        Try
            EliminateJpgInternal()
            DisposeBmpInternal(_bmp)
        Catch ex As Exception
            Throw New Exception($"BitmapInfo.Clear() failed: {ex.Message}")
        Finally
            BmpUnlock()
        End Try
    End Sub
#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Установка Bitmap-а.
    ''' </summary>
    Private Sub SetBmpInternal(bmp As Bitmap)
        If CheckBitmap(bmp) Then
            _bmp = bmp
            _bmpSize = _bmp?.Size
            _bmpPixelFormat = _bmp?.PixelFormat
        Else
            Throw New Exception($"BitmapInfo.SetBmpInternal() failed: Not CheckBitmap(bmp)")
        End If
    End Sub

    ''' <summary>
    ''' Извлечение данных Bitmap-а (если он пуст, и есть JPEG-данные).
    ''' </summary>
    Private Function GetBitmapDecodedInternal() As Bitmap
        If _bmp Is Nothing AndAlso _jpg IsNot Nothing Then
            Dim bmp = DecodeJpegInternal(_jpg)
            If bmp IsNot Nothing Then
                SetBmpInternal(bmp)
                BitmapDisposeWithDelay(bmp, Me.BitmapKeepTimeS) 'Отложенная очистка битмапа (именного этого, контроль по ссылке)
            End If
        End If
        Return _bmp
    End Function

    ''' <summary>
    ''' Клонирование битмапа с фиксацией ошибок.
    ''' </summary>
    Private Function BmpCloneInternal(bmp As Bitmap) As Bitmap
        Dim result As Bitmap = Nothing
        Try
            result = BitmapCloneManaged(bmp)
        Catch ex As Exception
            Interlocked.Increment(_cloneErrorCount)
            Throw New Exception($"BitmapInfo.BmpCloneInternal() failed: {ex.Message}")
        End Try
        Return result
    End Function

    ''' <summary>
    ''' Декодирование JPEG-потока.
    ''' </summary>
    ''' <param name="jpg">JPEG-поток.</param>
    ''' <returns>Декомпрессированное изображение.</returns>
    Private Function DecodeJpegInternal(jpg As Byte()) As Bitmap
        Dim result As Bitmap = Nothing
        Try
            If jpg Is Nothing Then
                Throw New Exception("JPEG data is Nothing")
            Else
                Try
                    Dim bmp = New Bitmap(New IO.MemoryStream(jpg))
                    If CheckBitmap(bmp) Then
                        result = bmp 'Bitmap прошел проверку
                        Interlocked.Add(_globalAllocatedDataCount, GetBitmapAllocatedSize(result.Size, result.PixelFormat))
                        Interlocked.Increment(_globalDecompressedCount)
                        Interlocked.Increment(_decompressedCount)
                    Else
                        Interlocked.Increment(_globalDecompressedErrorCount)
                        Interlocked.Increment(_decompressedErrorCount)
                        Throw New Exception($"Not CheckBitmap(bmp)")
                    End If
                Catch ex As Exception
                    Throw New Exception($"New Bitmap(New IO.MemoryStream(jpg[{jpg.Length} bytes]) failed: {ex.Message}")
                End Try
            End If
        Catch ex As Exception
            Throw New Exception($"BitmapInfo.DecodeJpegInternal() failed: {ex.Message}")
        End Try
        Return result
    End Function

    ''' <summary>
    ''' Извлечение размера изображения из JPEG-потока без его декодирования.
    ''' </summary>
    Private Function GetJpegSize(jpg As Byte(), ByRef channelCount As Integer,
                                 Optional maxBytesSearch As Integer = 1024) As Size
        'Инициализация фиктивными значениями, чтобы фиксировать ситуации "не считывания"
        channelCount = -1
        Dim res = New Size(-1, -1)
        Try
            Dim pos = 0 'Начальная позиция в файле
            If jpg(pos) <> &HFF OrElse jpg(pos + 1) <> &HD8 Then Return res 'Ошибка - не найден стартовый маркер
            pos += 2 'Перешагиваем через маркер
            Dim blockLength = 0 'Длина блока пока неизвестна...
            Do While pos < Math.Min(maxBytesSearch, jpg.Length) 'Работа в пределах допустимой области файла
                pos += blockLength 'Переходим к следующему блоку
                If pos > jpg.Length - 2 Then Return res 'Выход за пределы массива: "jpg(pos + 1)"; Ошибка - вышли за пределы области данных для поиска
                If jpg(pos) <> &HFF Then Return res 'Проверка на то, действительно ли мы перешли на заголовок блока (если нет - ошибка); Ошибка - не обнаружен признак маркера блока
                If jpg(pos + 1) = &HC0 Then '0xFFC0 - маркер начала кадра, далее можно узнать размеры изображения
                    'Структура блока 0xFFC0: [0xFFC0][ushort length][uchar precision][ushort x][ushort y]
                    Dim height = jpg(pos + 5) * 256 + jpg(pos + 6)
                    Dim width = jpg(pos + 7) * 256 + jpg(pos + 8)
                    channelCount = jpg(pos + 9)
                    res = New Size(width, height)
                    Return res 'Данные о размере изображения считаны
                Else
                    pos += 2 'Перешагиваем через маркер блока...
                    blockLength = jpg(pos) * 256 + jpg(pos + 1) '...и переходим к следующему
                End If
            Loop
            Return res 'Ошибка - исчерпали данные для поиска
        Catch ex As Exception
            channelCount = -1
            res = New Size(-1, -1)
        End Try
        Return res
    End Function

    ''' <summary>
    ''' Элиминирование JPEG.
    ''' </summary>
    Private Sub EliminateJpgInternal()
        _jpg = Nothing
    End Sub

    ''' <summary>
    ''' Получение копии массива байт.
    ''' </summary>
    Private Function ArrayCopy(src As Byte()) As Byte()
        If src Is Nothing Then Return Nothing
        Dim res = New Byte(src.Length - 1) {}
        Array.Copy(src, res, src.Length)
        Return res
    End Function
#End Region

#Region "DisposeWithDelay"
    ''' <summary>
    ''' Запуск потока отложенного Dispose для битмапа.
    ''' </summary>
    ''' <param name="bitmapKeepTimeS">Время доступности декомпрессированного битмапа.</param>
    Private Sub BitmapDisposeWithDelay(target As Bitmap, bitmapKeepTimeS As Single)
        If target IsNot Nothing AndAlso bitmapKeepTimeS >= 0 Then
            Dim thr = New Thread(Sub()
                                     Thread.Sleep(TimeSpan.FromSeconds(bitmapKeepTimeS))
                                     BmpLock(-1) 'Бесконечное ожидание, т.к. нужно высвободить ресурс
                                     Try
                                         DisposeBmpInternal(target)
                                     Finally
                                         BmpUnlock()
                                     End Try
                                 End Sub) With {.IsBackground = True}
            thr.Start()
        End If
    End Sub

    ''' <summary>
    ''' Метод для элиминирования определенного битмапа.
    ''' </summary>
    ''' <param name="target">Целевой Bitmap (элиминирование другой цели невозможно).</param>
    ''' <remarks>Если не отслеживать цель для элиминирования, может случиться ситуация, когда
    ''' объект будет инициализирован JPEG-ом, с него будет запрошен Bitmap, запустится
    ''' отложенные элиминирование после декомпресии, и далее пользователь обработает
    ''' декомпрессированное изображение, которое будет установлено, а далее стерто
    ''' отложенным Dispose()-ом. При отслеживании ссылки-цели элиминируется в точности
    ''' тот Bitmap, что и планировалось.</remarks>
    Private Sub DisposeBmpInternal(target As Bitmap)
        If CheckBitmap(target) Then
            Try
                If Object.ReferenceEquals(_bmp, target) Then _bmp = Nothing
                Dim bitmapAllocatedSize = GetBitmapAllocatedSize(target.Size, target.PixelFormat)
                target.Dispose()
                Interlocked.Add(_globalAllocatedDataCount, -1L * bitmapAllocatedSize)
                Interlocked.Increment(_globalDisposeCount)
                Interlocked.Increment(_disposeCount)
            Catch ex As Exception
                Interlocked.Increment(_globalDisposeErrorCount)
                Interlocked.Increment(_disposeErrorCount)
                Throw New Exception($"BitmapInfo.DisposeBmpInternal() failed: {ex.Message}")
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Получение количества выделенных байт под Bitmap.
    ''' </summary>
    Private Function GetBitmapAllocatedSize(bmpSize As Size, bmpPixelFormat As PixelFormat) As Long
        Dim bytesPerPixel = 1
        Select Case bmpPixelFormat
            Case PixelFormat.Format32bppArgb
                bytesPerPixel = 4
            Case PixelFormat.Format32bppRgb
                bytesPerPixel = 4
            Case PixelFormat.Format24bppRgb
                bytesPerPixel = 3
            Case PixelFormat.Format8bppIndexed
                bytesPerPixel = 1
            Case Else
                Throw New Exception($"BitmapInfo.GetBitmapAllocatedSize(): Unsupported pixel format {[Enum].GetName(GetType(PixelFormat), Bmp.PixelFormat)}")
        End Select
        Dim stride = 4 * ((bmpSize.Width * bytesPerPixel + 3) \ 4) '4 - DWORD
        Return (stride * bmpSize.Height * bytesPerPixel) 'Количество байт, занятых изображением
    End Function
#End Region

#Region "IDisposable Support"
    Private _disposed As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _disposed Then
            If disposing Then
                Clear(-1) 'Бесконечное ожидание, т.к. нужно высвободить ресурс
            End If
        End If
        _disposed = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub
#End Region

#Region "ICloneable"
    Public Function Clone() As Object Implements ICloneable.Clone
        Return GetClonedCopy()
    End Function
#End Region
End Class
