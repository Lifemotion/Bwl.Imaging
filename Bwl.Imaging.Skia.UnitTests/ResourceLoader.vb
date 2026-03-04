Imports SkiaSharp

Public Class ResourceLoader

    ' Preload resources to avoid loading them multiple times during tests
    Private Shared _res26FWD_IZHS As SKBitmap = SKEncoder.DecodeJpeg(LoadResourceData("26FWD_IZHS.jpg"))
    Private Shared _res636080817147998076 As SKBitmap = SKEncoder.DecodeJpeg(LoadResourceData("636080817147998076.jpg"))
    Private Shared _res638067000726558653 As SKBitmap = SKEncoder.DecodeJpeg(LoadResourceData("638067000726558653.jpg"))
    Private Shared _resCMYK As SKBitmap = SKEncoder.DecodeJpeg(LoadResourceData("CMYK.jpg"))
    Private Shared _resGray As SKBitmap = SKEncoder.DecodeJpeg(LoadResourceData("Gray.jpg"))
    Private Shared _resgray24 As SKBitmap = SKEncoder.DecodeBitmap(LoadResourceData("gray24.bmp"))
    Private Shared _resgray25 As SKBitmap = SKEncoder.DecodeBitmap(LoadResourceData("gray25.bmp"))
    Private Shared _resgrayProbeS4 As SKBitmap = SKEncoder.DecodeBitmap(LoadResourceData("grayProbeS4.bmp"))
    Private Shared _resjpeg24bpp As SKBitmap = SKEncoder.DecodeJpeg(LoadResourceData("jpeg24bpp.jpg"))
    Private Shared _resjpeg8bpp As SKBitmap = SKEncoder.DecodeJpeg(LoadResourceData("jpeg8bpp.jpg"))
    Private Shared _resNo_JFIF As SKBitmap = SKEncoder.DecodeJpeg(LoadResourceData("No_JFIF.jpg"))
    Private Shared _resRGB As SKBitmap = SKEncoder.DecodeJpeg(LoadResourceData("RGB.jpg"))
    Private Shared _resrgbProbeS4 As SKBitmap = SKEncoder.DecodeBitmap(LoadResourceData("rgbProbeS4.bmp"))
    Private Shared _resrgbw24 As SKBitmap = SKEncoder.DecodePng(LoadResourceData("rgbw24.png"))
    Private Shared _resrgbw25 As SKBitmap = SKEncoder.DecodePng(LoadResourceData("rgbw25.png"))
    Private Shared _resYCbCr As SKBitmap = SKEncoder.DecodeJpeg(LoadResourceData("YCbCr.jpg"))
    Private Shared _resYCbCrK As SKBitmap = SKEncoder.DecodeJpeg(LoadResourceData("YCbCrK.jpg"))
    Private Shared _res_4x3_gray As SKBitmap = SKEncoder.DecodeBitmap(LoadResourceData("_4x3-gray.bmp"))
    Private Shared _res_4x3_rgb As SKBitmap = SKEncoder.DecodeBitmap(LoadResourceData("_4x3-rgb.bmp"))

    Public Shared Function Res26FWD_IZHS() As SKBitmap
        Return _res26FWD_IZHS.Copy()
    End Function

    Public Shared Function Res636080817147998076() As SKBitmap
        Return _res636080817147998076.Copy()
    End Function

    Public Shared Function Res638067000726558653() As SKBitmap
        Return _res638067000726558653.Copy()
    End Function

    Public Shared Function ResCMYK() As SKBitmap
        Return _resCMYK.Copy()
    End Function

    Public Shared Function ResGray() As SKBitmap
        Return _resGray.Copy()
    End Function

    Public Shared Function Resgray24() As SKBitmap
        Return _resgray24.Copy()
    End Function

    Public Shared Function Resgray25() As SKBitmap
        Return _resgray25.Copy()
    End Function

    Public Shared Function ResgrayProbeS4() As SKBitmap
        Return _resgrayProbeS4.Copy()
    End Function

    Public Shared Function Resjpeg24bpp() As SKBitmap
        Return _resjpeg24bpp.Copy()
    End Function

    Public Shared Function Resjpeg8bpp() As SKBitmap
        Return _resjpeg8bpp.Copy()
    End Function

    Public Shared Function ResNo_JFIF() As SKBitmap
        Return _resNo_JFIF.Copy()
    End Function

    Public Shared Function ResRGB() As SKBitmap
        Return _resRGB.Copy()
    End Function

    Public Shared Function ResrgbProbeS4() As SKBitmap
        Return _resrgbProbeS4.Copy()
    End Function

    Public Shared Function Resrgbw24() As SKBitmap
        Return _resrgbw24.Copy()
    End Function

    Public Shared Function Resrgbw25() As SKBitmap
        Return _resrgbw25.Copy()
    End Function

    Public Shared Function ResYCbCr() As SKBitmap
        Return _resYCbCr.Copy()
    End Function

    Public Shared Function ResYCbCrK() As SKBitmap
        Return _resYCbCrK.Copy()
    End Function

    Public Shared Function Res_4x3_gray() As SKBitmap
        Return _res_4x3_gray.Copy()
    End Function

    Public Shared Function Res_4x3_rgb() As SKBitmap
        Return _res_4x3_rgb.Copy()
    End Function


    ''' <summary>
    ''' Load data from embedded resource with specified name.
    ''' </summary>
    ''' <param name="fileName">Embedded resource name</param>
    ''' <returns>Byte array</returns>
    Public Shared Function LoadResourceData(fileName As String) As Byte()
        Dim assembly = Reflection.Assembly.GetExecutingAssembly()
        Dim resourceName = assembly.GetManifestResourceNames().FirstOrDefault(Function(name) name.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
        If resourceName Is Nothing Then
            Throw New Exception($"Resource '{fileName}' not found in assembly.")
        End If
        Using stream = assembly.GetManifestResourceStream(resourceName)
            If stream Is Nothing Then
                Throw New Exception($"Failed to load resource stream for '{fileName}'.")
            End If
            Dim data As Byte() = New Byte(stream.Length - 1) {}
            stream.Read(data, 0, data.Length)
            Return data
        End Using
    End Function

End Class
