Imports System.Threading
Imports System.Drawing.Imaging
Imports Bwl.Imaging.Unsafe

Public Class BitmapInfo
    Implements IDisposable

    Private _jpg As Byte()
    Private _jpgIsNothing As Boolean
    Private _bmp As Bitmap
    Private _bmpIsNothing As Boolean
    Private _bmpSize As Nullable(Of Size)
    Private _bmpPixelFormat As Nullable(Of PixelFormat)
    Private _grayMatrix As GrayMatrix
    Private _rgbMatrix As RGBMatrix

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
            Return _jpgIsNothing
        End Get
    End Property

    Public ReadOnly Property BmpIsNothing As Boolean
        Get
            Return _bmpIsNothing
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

    Public Sub New(bmp As Bitmap)
        SetBmp(bmp)
    End Sub

    Public Sub BmpLock(Optional work As Boolean = True,
                       Optional timeoutMs As Integer = 10000)
        If work AndAlso Not _bmpSemaphore.WaitOne(timeoutMs) Then
            Throw New Exception(String.Format("BitmapInfo.BmpLock(): Timeout, {0} ms", timeoutMs))
        End If
    End Sub

    Public Sub BmpUnlock(Optional work As Boolean = True)
        If work Then
            _bmpSemaphore.Release()
        End If
    End Sub

    Public Sub SetBmp(bmp As Bitmap,
                      Optional clone As Boolean = False,
                      Optional useBmpLock As Boolean = True)
        Try
            BmpLock(useBmpLock)
            _bmp = If(clone, BmpClone2(bmp), bmp)
            _bmpIsNothing = _bmp Is Nothing
            _bmpSize = If(_bmp IsNot Nothing, _bmp.Size, Nothing)
            _bmpPixelFormat = If(_bmp IsNot Nothing, _bmp.PixelFormat, Nothing)
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock(useBmpLock)
        End Try
    End Sub

    Public Sub SetJpg(jpg As Byte(),
                      Optional bitmapKeepTimeS As Single = Single.MaxValue,
                      Optional clone As Boolean = False,
                      Optional useBmpLock As Boolean = True)
        Try
            BmpLock(useBmpLock)
            _jpg = If(clone, jpg.ToArray(), jpg)
            _jpgIsNothing = _jpg Is Nothing
            Me.BitmapKeepTimeS = bitmapKeepTimeS
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock(useBmpLock)
        End Try
    End Sub

    Public Function GetClonedBmp(Optional useBmpLock As Boolean = True) As Bitmap
        Dim result As Bitmap = Nothing
        Try
            BmpLock(useBmpLock)
            Dim bmp = GetBitmapDecoded()
            result = BmpClone2(bmp)
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock(useBmpLock)
        End Try
        Return result
    End Function

    Public Function GetClonedBmpGray(Optional useBmpLock As Boolean = True) As Bitmap
        Dim result As Bitmap = Nothing
        Try
            BmpLock(useBmpLock)
            Dim bmp = GetBitmapDecoded()
            If bmp.PixelFormat = Drawing.Imaging.PixelFormat.Format8bppIndexed Then
                result = BmpClone2(bmp)
            Else
                result = UnsafeFunctions.RgbToGray(bmp)
            End If
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock(useBmpLock)
        End Try
        Return result
    End Function

    Public Function GetGrayMatrix(Optional useBmpLock As Boolean = True) As GrayMatrix
        Dim result As GrayMatrix = Nothing
        Try
            BmpLock(useBmpLock)
            If _grayMatrix Is Nothing Then
                Dim bmp = GetBitmapDecoded()
                If bmp IsNot Nothing Then
                    _grayMatrix = BitmapConverter.BitmapToGrayMatrix(bmp)
                End If
            End If
            result = _grayMatrix
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock(useBmpLock)
        End Try
        Return result
    End Function

    Public Function GetRGBMatrix(Optional useBmpLock As Boolean = True) As RGBMatrix
        Dim result As RGBMatrix = Nothing
        Try
            BmpLock(useBmpLock)
            If _rgbMatrix Is Nothing Then
                Dim bmp = GetBitmapDecoded()
                If bmp IsNot Nothing Then
                    _rgbMatrix = BitmapConverter.BitmapToRGBMatrix(bmp)
                End If
            End If
            result = _rgbMatrix
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock(useBmpLock)
        End Try
        Return result
    End Function

    Public Sub EliminateJpg(Optional useBmpLock As Boolean = True)
        Try
            BmpLock(useBmpLock)
            _jpg = Nothing
            _jpgIsNothing = True
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock(useBmpLock)
        End Try
    End Sub

    Public Sub EliminateBmp(Optional useBmpLock As Boolean = True,
                            Optional eliminateGrayMatrix As Boolean = False,
                            Optional eliminateRGBMatrix As Boolean = False)
        Try
            BmpLock(useBmpLock)
            If _bmp IsNot Nothing Then
                _bmp.Dispose()
                _bmp = Nothing
            End If
            _bmpIsNothing = True
        Catch ex As Exception
            Throw ex
        Finally
            If eliminateGrayMatrix Then
                _grayMatrix = Nothing
            End If
            If eliminateRGBMatrix Then
                _rgbMatrix = Nothing
            End If
            BmpUnlock(useBmpLock)
        End Try
    End Sub

    Public Sub EliminateGrayMatrix(Optional useBmpLock As Boolean = True)
        Try
            BmpLock(useBmpLock)
            If _grayMatrix IsNot Nothing Then
                _grayMatrix = Nothing
            End If
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock(useBmpLock)
        End Try
    End Sub

    Public Sub EliminateRGBMatrix(Optional useBmpLock As Boolean = True)
        Try
            BmpLock(useBmpLock)
            If _rgbMatrix IsNot Nothing Then
                _rgbMatrix = Nothing
            End If
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock(useBmpLock)
        End Try
    End Sub

    ''' <summary>
    ''' Извлечение данных Bitmap-а.
    ''' </summary>
    Private Function GetBitmapDecoded() As Bitmap
        If _bmp Is Nothing AndAlso _jpg IsNot Nothing Then
            Dim bmp = DecodeJpeg(_jpg)
            SetBmp(bmp,, False) 'Установка декодированного значения Bmp позволяет также прописать свойства, False - блокировка не требуется (уже работаем под блокировкой)
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
                                     EliminateBmp(, True, True) 'True, True - при автоматическом элиминировании убираем также ссылки на матрицы
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

#Region "IDisposable Support"
    Private _disposed As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _disposed Then
            If disposing Then
                Try
                    BmpLock()
                    If _bmp IsNot Nothing Then
                        _bmp.Dispose()
                    End If
                Catch
                Finally
                    _jpg = Nothing
                    _jpgIsNothing = True
                    _bmp = Nothing
                    _bmpIsNothing = True
                    _bmpSize = Nothing
                    _bmpPixelFormat = Nothing
                    _grayMatrix = Nothing
                    _rgbMatrix = Nothing
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
