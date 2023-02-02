Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Threading
Imports Bwl.Imaging.Unsafe

''' <summary>
''' Потокобезопасная обвязка Bitmap-а, с поддержкой кешированных свойств изображения и JPEG-кеша.
''' </summary>
Public Class BitmapInfo
    Implements IDisposable
    Implements ICloneable

    Private ReadOnly _bmpSemaphore As New Semaphore(1, 1)

    Private _jpg As Byte()
    Private _bmp As Bitmap
    Private _bmpSize As Nullable(Of Size)
    Private _bmpPixelFormat As Nullable(Of PixelFormat)

    Private Shared _globalCompressedCount As Long
    Private Shared _globalDecompressedCount As Long
    Private Shared _globalBitmapEliminatedCount As Long
    Private _compressedCount As Long
    Private _decompressedCount As Long
    Private _bitmapEliminatedCount As Long

    ''' <summary>
    ''' Глобальный счетчик осуществленных компрессий JPEG (а не переприсваиваний).
    ''' </summary>
    Public Shared ReadOnly Property GlobalCompressedCount As Long
        Get
            Return Interlocked.Read(_globalCompressedCount)
        End Get
    End Property

    ''' <summary>
    ''' Глобальный счетчик декомпрессий JPEG.
    ''' </summary>
    Public Shared ReadOnly Property GlobalDecompressedCount As Long
        Get
            Return Interlocked.Read(_globalDecompressedCount)
        End Get
    End Property

    ''' <summary>
    ''' Глобальный счетчик количества элиминирований Bitmap-а.
    ''' </summary>
    Public Shared ReadOnly Property GlobalBitmapEliminatedCount As Long
        Get
            Return Interlocked.Read(_globalBitmapEliminatedCount)
        End Get
    End Property

    ''' <summary>
    ''' Счетчик осуществленных компрессий JPEG (а не переприсваиваний).
    ''' </summary>
    Public ReadOnly Property CompressedCount As Long
        Get
            Return Interlocked.Read(_compressedCount)
        End Get
    End Property

    ''' <summary>
    ''' Счетчик декомпрессий JPEG.
    ''' </summary>
    Public ReadOnly Property DecompressedCount As Long
        Get
            Return Interlocked.Read(_decompressedCount)
        End Get
    End Property

    ''' <summary>
    ''' Счетчик количества элиминирований Bitmap-а.
    ''' </summary>
    Public ReadOnly Property BitmapEliminatedCount As Long
        Get
            Return Interlocked.Read(_bitmapEliminatedCount)
        End Get
    End Property

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
                Throw New Exception("BitmapInfo.Bmp: 'Bmp' property access without BmpLock() before") '...свойство Bmp используется некорректно (то есть без блокировки)!
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

    Public Sub New()
    End Sub

    Public Sub New(jpg As Byte())
        SetJpg(jpg)
    End Sub

    Public Sub New(bmp As Bitmap)
        SetBmp(bmp)
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
            Throw New Exception(String.Format("BitmapInfo.BmpLock(): Timeout, {0} ms", timeoutMs))
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
                    jpg = JpegCodec.Encode(_bmp, quality).ToArray()
                    Interlocked.Increment(_globalCompressedCount)
                    Interlocked.Increment(_compressedCount)
                Else
                    jpg = _jpg
                End If
                'Этот вызов очень важен несмотря на возможность формальной отработки по ветке 'jpg = _jpg',
                'т.к. внутри элиминируется Bitmap(), освобождая ОЗУ.
                SetJpg(jpg, -1) '-1 - для отказа от блокировки/разблокировки разделяемого ресурса
            End If
        Finally
            BmpUnlock()
        End Try
    End Sub

    ''' <summary>
    ''' Возвращает JPEG-данные.
    ''' </summary>
    ''' <param name="ifEmptyCreateFromBitmap">Сформировать JPEG на основе Bitmap-а (если JPEG-потока нет).</param>
    ''' <param name="quality">Уровень качества JPEG.</param>
    ''' <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
    ''' <remarks>
    ''' Формирование JPEG на основе Bitmap-а при вызове данного метода при активном флаге ifEmptyCreateFromBitmap
    ''' не приводит к установке внутреннего JPEG-потока или элиминированию исходного Bitmap-а.
    ''' Если требуется сжать Bitmap, установить JPEG-поток и затем освободить память от Bitmap-а - используйте метод Compress().
    ''' </remarks>
    Public Function GetJpg(Optional ifEmptyCreateFromBitmap As Boolean = True,
                           Optional quality As Integer = 80,
                           Optional timeoutMs As Integer = 10000) As Byte()
        BmpLock(timeoutMs)
        Try
            Dim jpg = _jpg?.ToArray()
            If jpg Is Nothing AndAlso ifEmptyCreateFromBitmap AndAlso _bmp IsNot Nothing Then
                jpg = JpegCodec.Encode(_bmp, quality).ToArray()
            End If
            Return jpg
        Finally
            BmpUnlock()
        End Try
    End Function

    ''' <summary>
    ''' Устанавливает JPEG-данные и элиминирует данные Bitmap-а.
    ''' </summary>
    ''' <param name="jpg">JPEG-данные.</param>
    ''' <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
    Public Sub SetJpg(jpg As Byte(), Optional timeoutMs As Integer = 10000)
        Dim jpgChannelCount = 0 'Количество каналов JPEG
        Dim jpgSize = GetJpegSize(jpg, jpgChannelCount) 'Извлекаем данные о размере изображения из JPEG-потока
        If (jpgChannelCount = 3 OrElse jpgChannelCount = 1) AndAlso (jpgSize.Width > 0 AndAlso jpgSize.Height > 0) Then
            If timeoutMs >= 0 Then
                BmpLock(timeoutMs)
            End If
            Try
                EliminateBmpInternal(_bmp) 'При установке JPEG чистим Bmp
                _bmpSize = jpgSize
                _bmpPixelFormat = If(jpgChannelCount = 3, PixelFormat.Format24bppRgb, PixelFormat.Format8bppIndexed)
                _jpg = jpg
            Finally
                If timeoutMs >= 0 Then
                    BmpUnlock()
                End If
            End Try
        Else
            Throw New Exception("BitmapInfo.SetJpg(): Can't parse JPEG data")
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
        BmpLock(timeoutMs)
        Try
            EliminateJpgInternal() 'При установке Bmp чистим Jpeg
            SetBmpInternal(bmp)
        Finally
            BmpUnlock()
        End Try
    End Sub

    ''' <summary>
    ''' Получение клонированного изображения.
    ''' </summary>
    ''' <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
    ''' <returns>Клонированный Bitmap.</returns>
    Public Function GetClonedBmp(Optional timeoutMs As Integer = 10000) As Bitmap
        Dim result As Bitmap = Nothing
        BmpLock(timeoutMs)
        Try
            Dim bmp = GetBitmapDecodedInternal()
            result = BmpCloneInternal(bmp)
        Finally
            BmpUnlock()
        End Try
        Return result
    End Function

    ''' <summary>
    ''' Получение клонированного изображения.
    ''' </summary>
    ''' <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
    ''' <returns>Клонированный Bitmap.</returns>
    Public Function GetClonedBmpGray(Optional timeoutMs As Integer = 10000) As Bitmap
        Dim result As Bitmap = Nothing
        BmpLock(timeoutMs)
        Try
            Dim bmp = GetBitmapDecodedInternal()
            If bmp.PixelFormat = Drawing.Imaging.PixelFormat.Format8bppIndexed Then
                result = BmpCloneInternal(bmp)
            Else
                result = UnsafeFunctions.RgbToGray(bmp)
            End If
        Finally
            BmpUnlock()
        End Try
        Return result
    End Function

    ''' <summary>
    ''' Получение клонированной копии.
    ''' </summary>
    ''' <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
    ''' <returns>Клонированная копия.</returns>
    Public Function GetClonedCopy(Optional timeoutMs As Integer = 10000) As BitmapInfo
        Dim result As BitmapInfo = Nothing
        BmpLock(timeoutMs)
        Try
            If _jpg IsNot Nothing Then
                result = New BitmapInfo(_jpg.ToArray())
            ElseIf _bmp IsNot Nothing Then
                result = New BitmapInfo(BmpCloneInternal(_bmp))
            End If
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
            EliminateBmpInternal(_bmp)
        Finally
            BmpUnlock()
        End Try
    End Sub

    ''' <summary>
    ''' Запуск потока отложенного Dispose для битмапа.
    ''' </summary>
    ''' <param name="bitmapKeepTimeS">Время доступности декомпрессированного битмапа.</param>
    Private Sub BitmapDisposeWithDelay(target As Bitmap, bitmapKeepTimeS As Single)
        If bitmapKeepTimeS >= 0 Then
            Dim thr = New Thread(Sub()
                                     Thread.Sleep(TimeSpan.FromSeconds(bitmapKeepTimeS))
                                     BmpLock(-1) 'Бесконечное ожидание, т.к. нужно высвободить ресурс
                                     Try
                                         EliminateBmpInternal(target)
                                     Finally
                                         BmpUnlock()
                                     End Try
                                 End Sub) With {.IsBackground = True}
            thr.Start()
        End If
    End Sub

    ''' <summary>
    ''' Извлечение данных Bitmap-а (если он пуст, и есть JPEG-данные).
    ''' </summary>
    Private Function GetBitmapDecodedInternal() As Bitmap
        If _bmp Is Nothing AndAlso _jpg IsNot Nothing Then
            Dim bmp = DecodeJpegInternal(_jpg)
            SetBmpInternal(bmp)
            BitmapDisposeWithDelay(bmp, Me.BitmapKeepTimeS) 'Отложенная очистка битмапа (именного этого, контроль по ссылке)
        End If
        Return _bmp
    End Function

    ''' <summary>
    ''' Декодирование JPEG-потока.
    ''' </summary>
    ''' <param name="jpg">JPEG-поток.</param>
    ''' <returns>Декомпрессированное изображение.</returns>
    Private Function DecodeJpegInternal(jpg As Byte()) As Bitmap
        Dim result As Bitmap = Nothing
        If jpg Is Nothing Then
            Throw New Exception("BitmapInfo.DecodeJpeg(): JPEG is Nothing")
        Else
            Try
                result = New Bitmap(New IO.MemoryStream(jpg))
                Interlocked.Increment(_globalDecompressedCount)
                Interlocked.Increment(_decompressedCount)
            Catch ex As Exception
                Throw New Exception("BitmapInfo.DecodeJpeg(): decode error")
            End Try
        End If
        Return result
    End Function

    ''' <summary>
    ''' Клонирование битмапа - сначала быстрым методом, а при неудаче - более медленным, но более безопасным.
    ''' </summary>
    ''' <param name="bmp">Исходное изображение.</param>
    ''' <returns>Клонированное изображение.</returns>
    Private Function BmpCloneInternal(bmp As Bitmap) As Bitmap
        Dim result As Bitmap = Nothing
        Try
            result = UnsafeFunctions.BitmapClone(bmp)
        Catch ex As Exception
            Try
                result = New Bitmap(bmp)
            Catch
                Throw New Exception("BitmapInfo.BmpCloneInternal() failed")
            End Try
        End Try
        Return result
    End Function

    Private Sub EliminateJpgInternal()
        _jpg = Nothing
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
    Private Sub EliminateBmpInternal(target As Bitmap)
        Try
            target?.Dispose()
            Interlocked.Increment(_globalBitmapEliminatedCount)
            Interlocked.Increment(_bitmapEliminatedCount)
        Catch
        Finally
            'При нескольких декомпрессиях элиминирование _bmp должно произойти на крайнем по времени экземпляре
            If Object.ReferenceEquals(_bmp, target) Then
                _bmp = Nothing
            End If
        End Try
    End Sub

    Private Sub SetBmpInternal(bmp As Bitmap)
        _bmp = bmp
        _bmpSize = _bmp?.Size
        _bmpPixelFormat = _bmp?.PixelFormat
    End Sub

    ''' <summary>
    ''' Извлечение размера изображения из JPEG-потока без его декодирования.
    ''' </summary>
    Private Function GetJpegSize(jpg As Byte(), ByRef channelCount As Integer,
                                 Optional maxBytesInSearch As Integer = 1024) As Size
        'Инициализация фиктивными значениями, чтобы фиксировать ситуации "не считывания"
        channelCount = -1
        Dim res = New Size(-1, -1)

        Try
            'Начальная позиция в файле
            Dim pos = 0

            'Ошибка - не найден стартовый маркер
            If jpg(pos) <> &HFF OrElse jpg(pos + 1) <> &HD8 Then Return res

            pos += 2 'Перешагиваем через маркер
            Dim blockLength = 0 'Длина блока пока неизвестна...

            'Работа в пределах допустимой области файла
            Do While pos < Math.Min(maxBytesInSearch, jpg.Length)
                'Переходим к следующему блоку
                pos += blockLength

                'Выход за пределы массива: "jpg(pos + 1)"; Ошибка - вышли за пределы области данных для поиска
                If pos > jpg.Length - 2 Then Return res

                'Проверка на то, действительно ли мы перешли на заголовок блока (если нет - ошибка); Ошибка - не обнаружен признак маркера блока
                If jpg(pos) <> &HFF Then Return res

                '0xFFC0 - маркер начала кадра, далее можно узнать размеры изображения
                If jpg(pos + 1) = &HC0 Then
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

#Region "IDisposable Support"
    Private _disposed As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _disposed Then
            If disposing Then
                Clear()
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
