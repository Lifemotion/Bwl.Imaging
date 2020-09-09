﻿Imports System.Threading
Imports Bwl.Imaging.Unsafe

''' <summary>
''' Потокобезопасная обвязка Bitmap-а, с поддержкой кешированных свойств изображения и JPEG-кеша.
''' </summary>
Public Class BitmapInfo
    Implements IDisposable

    Private _jpg As Byte()
    Private _bmp As Bitmap
    Private _bmpIsNothing As Boolean
    Private _bmpSize As Nullable(Of Size)
    Private _bmpPixelFormat As Nullable(Of PixelFormat)

    Private ReadOnly _bmpSemaphore As New Semaphore(1, 1)

    ''' <summary>
    ''' Время хранения декомпрессированного битмапа (если доступен JPEG-поток для экономии ОЗУ).
    ''' </summary>
    Public Property BitmapKeepTimeS As Single = 5

    '''<remarks>
    ''' При обращении к Bmp обязательно использовать методы BmpLock() / BmpUnlock()
    '''</remarks>
    Public ReadOnly Property Bmp As Bitmap
        Get
            'Перед предоставлением объекта проверяем, выполнен ли Lock на объекте синхронизации...
            If _bmpSemaphore.WaitOne(0) Then 'Если удалось заблокировать ресурс...
                _bmpSemaphore.Release()
                Throw New Exception("BitmapInfo.Bmp: 'Bmp' property access without BmpLock() before") '...свойство Bmp используется некорректно (то есть без блокировки)!
            End If
            Return GetBitmapDecoded()
        End Get
    End Property

    Public ReadOnly Property JpgIsNothing As Boolean
        Get
            Return _jpg Is Nothing
        End Get
    End Property

    Public ReadOnly Property BmpIsNothing As Boolean
        Get
            Return _bmpIsNothing
        End Get
    End Property

    Public ReadOnly Property BmpAndJpgAreNothing As Boolean
        Get
            Return _bmpIsNothing AndAlso _jpg Is Nothing
        End Get
    End Property

    Public ReadOnly Property BmpSize As Size
        Get
            If _bmpSize IsNot Nothing Then
                Return _bmpSize.Value
            Else
                Throw New Exception("BitmapInfo.BmpSize: Bitmap is Nothing")
            End If
        End Get
    End Property

    Public ReadOnly Property BmpPixelFormat As PixelFormat
        Get
            If _bmpPixelFormat IsNot Nothing Then
                Return _bmpPixelFormat.Value
            Else
                Throw New Exception("BitmapInfo.PixelFormat: Bitmap is Nothing")
            End If
        End Get
    End Property

    Public Sub New()
    End Sub

    Public Sub New(bmp As Bitmap, jpg As Byte())
        SetBmpAndJpg(bmp, jpg)
    End Sub

    Public Sub New(jpg As Byte())
        SetJpg(jpg)
    End Sub

    Public Sub New(bmp As Bitmap)
        SetBmp(bmp)
    End Sub

    Public Sub BmpLock(Optional timeoutMs As Integer = 10000)
        If Not _bmpSemaphore.WaitOne(timeoutMs) Then
            Throw New Exception(String.Format("BitmapInfo.BmpLock(): Timeout, {0} ms", timeoutMs))
        End If
    End Sub

    Public Sub BmpUnlock()
        _bmpSemaphore.Release()
    End Sub

    Public Function GetJpg() As Byte()
        Return _jpg
    End Function

    Public Sub SetBmpAndJpg(bmp As Bitmap, jpg As Byte())
        Dim jpgChannelCount = 0 'Количество каналов JPEG
        Dim jpgSize = GetJpegSize(jpg, jpgChannelCount) 'Извлекаем данные о размере изображения из JPEG-потока
        If (jpgChannelCount = 3 OrElse jpgChannelCount = 1) AndAlso (jpgSize.Width > 0 AndAlso jpgSize.Height > 0) Then
            If bmp.Size = jpgSize Then
                Dim bmpChannelCount = If(bmp.PixelFormat = PixelFormat.Format8bppIndexed, 1, 3)
                If bmpChannelCount = jpgChannelCount Then
                    Try
                        BmpLock()
                        EliminateBmp() 'При установке JPEG чистим Bmp
                        SetBmpInternal(bmp) 'Устанавливаем Bmp
                        _jpg = jpg 'Все данные по размеру и форматам изображения установлены с Bmp, JPEG проверен и просто присваиваем
                        BitmapDisposeWithDelay(Me.BitmapKeepTimeS) 'Отложенная очистка битмапа также должна быть активирована
                    Catch ex As Exception
                        Throw ex
                    Finally
                        BmpUnlock()
                    End Try
                Else
                    Throw New Exception("BitmapInfo.SetBmpAndJpg(): Bitmap vs JPEG: incompatible pixel format")
                End If
            Else
                Throw New Exception("BitmapInfo.SetBmpAndJpg(): Bitmap vs JPEG: different size")
            End If
        Else
            Throw New Exception("BitmapInfo.SetBmpAndJpg(): Can't parse JPEG data")
        End If
    End Sub

    Public Sub SetJpg(jpg As Byte())
        Dim jpgChannelCount = 0 'Количество каналов JPEG
        Dim jpgSize = GetJpegSize(jpg, jpgChannelCount) 'Извлекаем данные о размере изображения из JPEG-потока
        If (jpgChannelCount = 3 OrElse jpgChannelCount = 1) AndAlso (jpgSize.Width > 0 AndAlso jpgSize.Height > 0) Then
            Try
                BmpLock()
                EliminateBmp() 'При установке JPEG чистим Bmp
                _bmpSize = jpgSize
                _bmpPixelFormat = If(jpgChannelCount = 3, PixelFormat.Format24bppRgb, PixelFormat.Format8bppIndexed)
                _jpg = jpg
            Catch ex As Exception
                Throw ex
            Finally
                BmpUnlock()
            End Try
        Else
            Throw New Exception("BitmapInfo.SetJpg(): Can't parse JPEG data")
        End If
    End Sub

    Public Sub SetBmp(bmp As Bitmap)
        Try
            BmpLock()
            EliminateJpg() 'При установке Bmp чистим Jpeg
            SetBmpInternal(bmp)
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock()
        End Try
    End Sub

    Private Sub SetBmpInternal(bmp As Bitmap)
        _bmp = bmp
        _bmpIsNothing = _bmp Is Nothing
        _bmpSize = If(_bmp IsNot Nothing, _bmp.Size, Nothing)
        _bmpPixelFormat = If(_bmp IsNot Nothing, _bmp.PixelFormat, Nothing)
    End Sub

    Public Function GetClonedBmp() As Bitmap
        Dim result As Bitmap = Nothing
        Try
            BmpLock()
            Dim bmp = GetBitmapDecoded()
            result = BmpClone2(bmp)
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock()
        End Try
        Return result
    End Function

    Public Function GetClonedBmpGray() As Bitmap
        Dim result As Bitmap = Nothing
        Try
            BmpLock()
            Dim bmp = GetBitmapDecoded()
            If bmp.PixelFormat = Drawing.Imaging.PixelFormat.Format8bppIndexed Then
                result = BmpClone2(bmp)
            Else
                result = UnsafeFunctions.RgbToGray(bmp)
            End If
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock()
        End Try
        Return result
    End Function

    Public Sub Clear()
        Try
            BmpLock()
            EliminateJpg()
            EliminateBmp()
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock()
        End Try
    End Sub

    ''' <summary>
    ''' Очистка Bitmap-а.
    ''' </summary>
    ''' <remarks>Если внутри есть JPEG - он будет переустановлен с подтяжкой свойств (размер/формат пикселя).</remarks>
    Public Sub ClearBmp()
        Dim jpg = _jpg
        If jpg IsNot Nothing Then 'Если есть данные JPEG...
            SetJpg(jpg) '...то очистку битмапа можно выполнить более рационально - EliminateBmp() внутри вызывается автоматически
        Else
            EliminateBmp() '...или же очистку Bitmap-а вызываем явно
        End If
    End Sub

    ''' <summary>
    ''' Очистка JPEG.
    ''' </summary>
    ''' <remarks>Если внутри есть Bitmap - он будет переустановлен с подтяжкой свойств (размер/формат пикселя).</remarks>
    Public Sub ClearJpg()
        SetBmp(_bmp) 'Очистка JPEG выполняется автоматически, гарантируется наличие свойств от Bitmap-а
    End Sub

    ''' <summary>
    ''' Извлечение данных Bitmap-а.
    ''' </summary>
    Private Function GetBitmapDecoded() As Bitmap
        If _bmp Is Nothing AndAlso _jpg IsNot Nothing Then
            Dim bmp = DecodeJpeg(_jpg)
            SetBmpInternal(bmp)
            BitmapDisposeWithDelay(Me.BitmapKeepTimeS) 'Отложенная очистка битмапа
        End If
        Return _bmp
    End Function

    ''' <summary>
    ''' Запуск потока отложенного Dispose для битмапа.
    ''' </summary>
    ''' <param name="bitmapKeepTimeS">Время доступности декомпрессированного битмапа.</param>
    Private Sub BitmapDisposeWithDelay(bitmapKeepTimeS As Single)
        If bitmapKeepTimeS >= 0 AndAlso bitmapKeepTimeS <> Single.MaxValue Then
            Dim thr = New Thread(Sub()
                                     Thread.Sleep(TimeSpan.FromSeconds(bitmapKeepTimeS))
                                     EliminateBmp()
                                 End Sub) With {.IsBackground = True}
            thr.Start()
        End If
    End Sub

    ''' <summary>
    ''' Декодирование JPEG-потока.
    ''' </summary>
    ''' <returns>Декомпрессированное изображение.</returns>
    Private Function DecodeJpeg(jpg As Byte()) As Bitmap
        Dim result As Bitmap = Nothing
        If jpg Is Nothing OrElse jpg.Length = 0 Then
            Throw New Exception("BitmapInfo.DecodeJpeg(): Can't get frame - stored JPEG stream is empty or null")
        Else
            Try
                result = New Bitmap(New IO.MemoryStream(jpg))
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
    Private Function BmpClone2(bmp As Bitmap) As Bitmap
        Dim result As Bitmap = Nothing
        Try
            result = UnsafeFunctions.BitmapClone(bmp)
        Catch ex As Exception
            Try
                result = New Bitmap(bmp)
            Catch
                Throw New Exception("BitmapInfo:BmpClone2() failed")
            End Try
        End Try
        Return result
    End Function

    Private Sub EliminateJpg()
        _jpg = Nothing
    End Sub

    Private Sub EliminateBmp()
        If _bmp IsNot Nothing Then
            _bmp.Dispose()
            _bmp = Nothing
        End If
        _bmpIsNothing = True
    End Sub

    ''' <summary>
    ''' Извлечение размера изображения из JPEG-потока без его декодирования.
    ''' </summary>
    Private Function GetJpegSize(jpg As Byte(), ByRef channelCount As Integer) As Size
        Dim res = New Size(-1, -1)
        channelCount = -1
        Try
            Dim pos = 0 'Keeps track of the position within the file
            If jpg(pos) = &HFF AndAlso jpg(pos + 1) = &HD8 AndAlso jpg(pos + 2) = &HFF AndAlso jpg(pos + 3) = &HE0 Then
                pos += 4
                'Check for valid JPEG header (null terminated JFIF)
                If jpg(pos + 2) = AscW("J"c) AndAlso jpg(pos + 3) = AscW("F"c) AndAlso jpg(pos + 4) = AscW("I"c) AndAlso jpg(pos + 5) = AscW("F"c) AndAlso jpg(pos + 6) = &H0 Then
                    'Retrieve the block length of the first block since the first block will not contain the size of file
                    Dim blockLength = jpg(pos) * 256 + jpg(pos + 1)
                    Do While pos < jpg.Length
                        pos += blockLength 'Increase the file index to get to the next block
                        If pos >= jpg.Length Then
                            Return res 'Check to protect against segmentation faults
                        End If
                        If jpg(pos) <> &HFF Then
                            Return res 'Check that we are truly at the start of another block
                        End If
                        If jpg(pos + 1) = &HC0 Then '0xFFC0 is the "Start of frame" marker which contains the file size
                            'The structure of the 0xFFC0 block is quite simple [0xFFC0][ushort length][uchar precision][ushort x][ushort y]
                            Dim height = jpg(pos + 5) * 256 + jpg(pos + 6)
                            Dim width = jpg(pos + 7) * 256 + jpg(pos + 8)
                            channelCount = jpg(pos + 9)
                            res = New Size(width, height)
                            Return res
                        Else
                            pos += 2 'Skip the block marker
                            blockLength = jpg(pos) * 256 + jpg(pos + 1) 'Go to the next block
                        End If
                    Loop
                    Return res 'If this point is reached then no size was found
                Else
                    Return res
                End If 'Not a valid JFIF string
            Else
                Return res
            End If 'Not a valid SOI header
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
                Try
                    BmpLock()
                    EliminateJpg()
                    EliminateBmp()
                Catch
                Finally
                    BmpUnlock()
                End Try
            End If
        End If
        _disposed = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub
#End Region
End Class
