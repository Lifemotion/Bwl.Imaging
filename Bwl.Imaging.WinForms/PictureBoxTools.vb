Imports System.Runtime.CompilerServices

Public Module PictureBoxTools
    <Extension()>
    Public Sub ShowMatrix(pb As PictureBox, matrix As GrayMatrix)
        pb.Image = matrix.ToBitmap
        pb.Refresh()
    End Sub

    <Extension()>
    Public Sub ShowMatrix(pb As PictureBox, matrix As RGBMatrix)
        pb.Image = matrix.ToBitmap
        pb.Refresh()
    End Sub
End Module
