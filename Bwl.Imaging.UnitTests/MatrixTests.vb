Imports System.Drawing
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class MatrixTests

    <TestMethod()> Public Sub LoadAndAccessRgbMatrix()
        Dim bmp = My.Resources._4x3_krgb
        Dim rgb = BitmapConverter.BitmapToRGBMatrix(bmp)
        Assert.AreEqual(4, rgb.Width)
        Assert.AreEqual(3, rgb.Height)
        Assert.AreEqual(0, rgb.RedPixel(0, 0)) : Assert.AreEqual(0, rgb.GreenPixel(0, 0)) : Assert.AreEqual(0, rgb.BluePixel(0, 0))
        Assert.AreEqual(255, rgb.RedPixel(1, 0)) : Assert.AreEqual(0, rgb.GreenPixel(1, 0)) : Assert.AreEqual(0, rgb.BluePixel(1, 0))
        Assert.AreEqual(0, rgb.RedPixel(2, 0)) : Assert.AreEqual(255, rgb.GreenPixel(2, 0)) : Assert.AreEqual(0, rgb.BluePixel(2, 0))
        Assert.AreEqual(0, rgb.RedPixel(3, 0)) : Assert.AreEqual(0, rgb.GreenPixel(3, 0)) : Assert.AreEqual(255, rgb.BluePixel(3, 0))

        Assert.AreEqual(255, rgb.RedPixel(0, 1)) : Assert.AreEqual(255, rgb.GreenPixel(0, 1)) : Assert.AreEqual(255, rgb.BluePixel(0, 1))
        Assert.AreEqual(255, rgb.RedPixel(1, 1)) : Assert.AreEqual(255, rgb.GreenPixel(1, 1)) : Assert.AreEqual(255, rgb.BluePixel(1, 1))
        Assert.AreEqual(255, rgb.RedPixel(2, 1)) : Assert.AreEqual(255, rgb.GreenPixel(2, 1)) : Assert.AreEqual(255, rgb.BluePixel(2, 1))
        Assert.AreEqual(255, rgb.RedPixel(3, 1)) : Assert.AreEqual(255, rgb.GreenPixel(3, 1)) : Assert.AreEqual(255, rgb.BluePixel(3, 1))

        Assert.AreEqual(0, rgb.RedPixel(0, 2)) : Assert.AreEqual(0, rgb.GreenPixel(0, 2)) : Assert.AreEqual(255, rgb.BluePixel(0, 2))
        Assert.AreEqual(0, rgb.RedPixel(1, 2)) : Assert.AreEqual(255, rgb.GreenPixel(1, 2)) : Assert.AreEqual(0, rgb.BluePixel(1, 2))
        Assert.AreEqual(255, rgb.RedPixel(2, 2)) : Assert.AreEqual(0, rgb.GreenPixel(2, 2)) : Assert.AreEqual(0, rgb.BluePixel(2, 2))
        Assert.AreEqual(0, rgb.RedPixel(3, 2)) : Assert.AreEqual(0, rgb.GreenPixel(3, 2)) : Assert.AreEqual(0, rgb.BluePixel(3, 2))
    End Sub

    <TestMethod()> Public Sub SaveRgbMatrix()
        Dim bmp = My.Resources._4x3_krgb
        Dim rgb = BitmapConverter.BitmapToRGBMatrix(bmp)
        Dim newbmp = rgb.ToBitmap
        Dim clr00 = newbmp.GetPixel(0, 0)
        Dim clr10 = newbmp.GetPixel(1, 0)
        Dim clr20 = newbmp.GetPixel(2, 0)
        Dim clr30 = newbmp.GetPixel(3, 0)
        ' newbmp.Save("C:\Users\heart\Repositories\test1.bmp")
        Assert.AreEqual(Color.Black.ToArgb, clr00.ToArgb)
        Assert.AreEqual(Color.Red.ToArgb, clr10.ToArgb)
        Assert.AreEqual(Color.FromArgb(0, 255, 0).ToArgb, clr20.ToArgb)
        Assert.AreEqual(Color.Blue.ToArgb, clr30.ToArgb)
    End Sub

    <TestMethod()> Public Sub RgbMatrixAccess()
        Dim bmp = New RGBMatrix(4, 4)
        bmp.ColorPixel(1, 1) = Color.Red
        Assert.AreEqual(255, bmp.RedPixel(1, 1))
        Assert.AreEqual(0, bmp.GreenPixel(1, 1))
        Assert.AreEqual(0, bmp.BluePixel(1, 1))
        Assert.AreEqual(255, bmp.Red(1 + 4 * 1))
        Assert.AreEqual(0, bmp.Green(1 + 4 * 1))
        Assert.AreEqual(0, bmp.Blue(1 + 4 * 1))

        Dim clr = bmp.ColorPixel(1, 1)
        Assert.AreEqual(CByte(255), clr.R)
        Assert.AreEqual(CByte(0), clr.G)
        Assert.AreEqual(CByte(0), clr.B)
    End Sub
End Class