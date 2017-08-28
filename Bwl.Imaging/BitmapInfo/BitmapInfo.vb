Imports System.Threading
Imports Bwl.Imaging.Unsafe

Public Class BitmapInfo
    Implements IDisposable

    Private _bmp As Bitmap
    Private _bmpIsNothing As Boolean
    Private _bmpSize As Nullable(Of Size)
    Private _grayMatrix As GrayMatrix
    Private _rgbMatrix As RGBMatrix

    Private ReadOnly _bmpSemaphore As New Semaphore(1, 1)

    '''<remarks>
    ''' При обращении к Bmp обязательно использовать BmpLock()/BmpUnlock()
    '''</remarks>
    Public ReadOnly Property Bmp As Bitmap
        Get
            'Перед предоставлением объекта проверяем, выполнен ли Lock на объекте синхронизации...
            If _bmpSemaphore.WaitOne(0) Then 'Если удалось заблокировать ресурс...
                _bmpSemaphore.Release()
                Throw New Exception("BitmapInfo.Bmp: 'Bmp' property access without BmpLock()") '...свойство Bmp используется некорректно (то есть без блокировки)!
            End If
            Return _bmp
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

    Public Sub New(bmp As Bitmap)
        SetBmp(bmp)
    End Sub

    Public Sub BmpLock()
        If Not _bmpSemaphore.WaitOne(TimeSpan.FromSeconds(10)) Then
            Throw New Exception("BitmapInfo.BmpLock(): Timeout")
        End If
    End Sub

    Public Sub BmpUnlock()
        _bmpSemaphore.Release()
    End Sub

    Public Sub SetBmp(bmp As Bitmap)
        BmpLock()
        _bmp = bmp
        _bmpIsNothing = bmp Is Nothing
        _bmpSize = If(bmp IsNot Nothing, bmp.Size, Nothing)
        BmpUnlock()
    End Sub

    Public Function GetClonedBmp() As Bitmap
        Dim result As Bitmap = Nothing
        Try
            BmpLock()
            result = UnsafeFunctions.BitmapClone(_bmp)
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
            If _bmp.PixelFormat = Drawing.Imaging.PixelFormat.Format8bppIndexed Then
                result = UnsafeFunctions.BitmapClone(_bmp)
            Else
                result = UnsafeFunctions.RgbToGray(_bmp)
            End If
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock()
        End Try
        Return result
    End Function

    Public Function GetRGBMatrix() As RGBMatrix
        Dim result As RGBMatrix = Nothing
        Try
            BmpLock()
            If _rgbMatrix Is Nothing AndAlso _bmp IsNot Nothing Then
                _rgbMatrix = BitmapConverter.BitmapToRGBMatrix(_bmp)
            End If
            result = _rgbMatrix
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock()
        End Try
        Return result
    End Function

    Public Function GetGrayMatrix() As GrayMatrix
        Dim result As GrayMatrix = Nothing
        Try
            BmpLock()
            If _grayMatrix Is Nothing AndAlso _bmp IsNot Nothing Then
                _grayMatrix = BitmapConverter.BitmapToGrayMatrix(_bmp)
            End If
            result = _grayMatrix
        Catch ex As Exception
            Throw ex
        Finally
            BmpUnlock()
        End Try
        Return result
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Try
                    BmpLock()
                    If _bmp IsNot Nothing Then
                        _bmp.Dispose()
                        _bmp = Nothing
                    End If
                    _grayMatrix = Nothing
                    _rgbMatrix = Nothing
                Catch
                Finally
                    BmpUnlock()
                End Try
            End If
        End If
        disposedValue = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub
#End Region
End Class
