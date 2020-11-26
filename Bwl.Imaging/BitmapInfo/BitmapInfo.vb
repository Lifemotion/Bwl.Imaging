Imports System.Threading
Imports Bwl.Imaging.Unsafe

''' <summary>
''' Потокобезопасная обвязка Bitmap-а, с поддержкой кешированных свойств изображения и JPEG-кеша.
''' </summary>
Public Class BitmapInfo
    Implements IDisposable

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
    Public Property BitmapKeepTimeS As Single = 2

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
    '''Если Bitmap пуст, это не означает, что изображение отсутствует.
    '''Возможно, используется хранение в JPEG.
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
    ''' Возвращает JPEG-данные (возвращаемое значение может быть пустым при работе с обычным Bitmap-ом).
    ''' </summary>
    ''' <param name="timeoutMs">Таймаут блокировки доступа к разделяемому ресурсу.</param>
    Public Function GetJpg(Optional timeoutMs As Integer = 10000) As Byte()
        BmpLock(timeoutMs)
        Try
            Return If(_jpg IsNot Nothing, _jpg.ToArray(), Nothing) 'Так безопаснее
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
        If bitmapKeepTimeS >= 0 AndAlso bitmapKeepTimeS <> Single.MaxValue Then
            Dim thr = New Thread(Sub()
                                     Thread.Sleep(TimeSpan.FromSeconds(bitmapKeepTimeS))
                                     BmpLock()
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
                Throw New Exception("BitmapInfo.BmpClone() failed")
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
        If _bmp IsNot Nothing AndAlso target IsNot Nothing AndAlso Object.ReferenceEquals(_bmp, target) Then
            Try
                _bmp.Dispose()
                Interlocked.Increment(_globalBitmapEliminatedCount)
                Interlocked.Increment(_bitmapEliminatedCount)
            Catch
            Finally
                _bmp = Nothing
            End Try
        End If
    End Sub

    Private Sub SetBmpInternal(bmp As Bitmap)
        _bmp = bmp
        _bmpSize = If(_bmp IsNot Nothing, _bmp.Size, Nothing)
        _bmpPixelFormat = If(_bmp IsNot Nothing, _bmp.PixelFormat, Nothing)
    End Sub

    ''' <summary>
    ''' Извлечение размера изображения из JPEG-потока без его декодирования.
    ''' </summary>
    Private Function GetJpegSize(jpg As Byte(), ByRef channelCount As Integer,
                                 Optional maxBytesInSearch As Integer = 1024) As Size
        'Инициализация фиктивными значениями, чтобы фиксировать ситуации "не считывания"
        Dim res = New Size(-1, -1)
        channelCount = -1
        'Количество обнаруженных таблиц квантования
        Dim quantTbl = 0
        Try
            Dim pos = 0 'Позиция в файле
            If jpg(pos) = &HFF AndAlso jpg(pos + 1) = &HD8 Then
                pos += 2 'Перешагиваем через маркер
                Dim blockLength = 0 'Длина блока пока неизвестна...
                Do While pos < Math.Min(maxBytesInSearch, jpg.Length) 'Работа в пределах допустимой области файла
                    pos += blockLength 'Переходим к следующему блоку
                    If pos > jpg.Length - 2 Then 'Выход за пределы массива: "jpg(pos + 1)"
                        Return res 'Ошибка - вышли за пределы области данных для поиска
                    End If
                    If jpg(pos) <> &HFF Then 'Проверка на то, действительно ли мы перешли на заголовок блока (если нет - ошибка)
                        Return res 'Ошибка - не обнаружен признак маркера блока
                    End If
                    If jpg(pos + 1) = &HC0 Then '0xFFC0 - маркер начала кадра, далее можно узнать размеры изображения
                        If quantTbl > 0 Then 'Если JPEG валиден, должны были встретить минимум один блок 0xFFDB - таблицу квантования
                            'Структура блока 0xFFC0: [0xFFC0][ushort length][uchar precision][ushort x][ushort y]
                            Dim height = jpg(pos + 5) * 256 + jpg(pos + 6)
                            Dim width = jpg(pos + 7) * 256 + jpg(pos + 8)
                            channelCount = jpg(pos + 9)
                            res = New Size(width, height)
                        End If
                        Return res 'Данные о размере изображения считаны
                    Else
                        If jpg(pos + 1) = &HDB Then '0xFFDB - маркер таблицы квантования (учитываем ее нахождение, т.к. это также является признаком валидного JPEG)
                            quantTbl += 1 'При поиске 0xFFC0 все равно проходим по этим маркерам, лучше их подсчитать
                        End If
                        pos += 2 'Перешагиваем через маркер блока...
                        blockLength = jpg(pos) * 256 + jpg(pos + 1) '...и переходим к следующему
                    End If
                Loop
                Return res 'Ошибка - исчерпали данные для поиска
            Else
                Return res 'Ошибка - не найден стартовый маркер
            End If
        Catch ex As Exception
            res = New Size(-1, -1)
            channelCount = -1
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
End Class
