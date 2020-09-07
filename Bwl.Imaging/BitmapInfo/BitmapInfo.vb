Imports System.Threading
Imports Bwl.Imaging.Unsafe

Public Class BitmapInfo
    Implements IDisposable

    Private _jpg As Byte()
    Private _bmp As Bitmap
    Private _bmpIsNothing As Boolean
    Private _bmpSize As Nullable(Of Size)
    Private _bmpPixelFormat As Nullable(Of PixelFormat)

    Private ReadOnly _bmpSemaphore As New Semaphore(1, 1)

    ''' <summary>
    ''' Время хранения декомпрессированного битмапа (если изначально работаем от JPEG-потока).
    ''' </summary>
    Public Property BitmapKeepTimeS As Single = Single.MaxValue

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

    Public Sub New(jpg As Byte(), Optional bitmapKeepTimeS As Single = Single.MaxValue)
        SetJpg(jpg, bitmapKeepTimeS)
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

    Public Sub SetJpg(jpg As Byte(), Optional bitmapKeepTimeS As Single = Single.MaxValue)
        Try
            BmpLock()
            EliminateBmp() 'При установке Jpeg чистим Bmp
            _jpg = jpg
            Me.BitmapKeepTimeS = bitmapKeepTimeS
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock()
        End Try
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
    ''' Извлечение данных Bitmap-а.
    ''' </summary>
    Private Function GetBitmapDecoded() As Bitmap
        If _bmp Is Nothing AndAlso _jpg IsNot Nothing Then
            Dim bmp = DecodeJpeg(_jpg)
            SetBmpInternal(bmp)
            BitmapDisposeWithDelay(BitmapKeepTimeS)
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
