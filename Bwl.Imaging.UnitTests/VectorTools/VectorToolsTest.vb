Imports System.Drawing

<TestClass()> Public Class VectorToolsTests

    <TestMethod()> Public Sub PointInPolygonIntegerTest()
        Dim polygonPoints = {
                                New PointF(370, 279),
                                New PointF(160, 106),
                                New PointF(262, 390),
                                New PointF(61, 433),
                                New PointF(358, 530),
                                New PointF(160, 869),
                                New PointF(412, 624),
                                New PointF(439, 942),
                                New PointF(528, 601),
                                New PointF(672, 797),
                                New PointF(598, 425),
                                New PointF(673, 276),
                                New PointF(427, 420),
                                New PointF(660, 124),
                                New PointF(358, 356),
                                New PointF(507, 95)
                            }
        PointInPolygonTest(polygonPoints)
    End Sub

    <TestMethod()> Public Sub PointInPolygonSingleTest()
        Dim polygonPoints = {
                                New PointF(370, 279),
                                New PointF(160, 106),
                                New PointF(262, 390),
                                New PointF(61, 433),
                                New PointF(358, 530),
                                New PointF(160, 869),
                                New PointF(412, 624),
                                New PointF(439, 942),
                                New PointF(528, 601),
                                New PointF(672, 797),
                                New PointF(598, 425),
                                New PointF(673, 276),
                                New PointF(427, 420),
                                New PointF(660, 124),
                                New PointF(358, 356),
                                New PointF(507, 95)
                            }.Select(Function(item) New PointF(item.X / 1024, item.Y / 1024)).ToArray()
        PointInPolygonTest(polygonPoints)
    End Sub

    Public Sub PointInPolygonTest(polygonPoints As IEnumerable(Of PointF))
        Dim nErr = 0
        Dim polygonTest = New PolygonTest(polygonPoints)
        For x = 0 To 1024
            For y = 0 To 1024
                Dim p = New PointF(x, y)
                Dim pointInPolygonRes = polygonTest.PointInPolygonTest(p)
                Dim touchDetected0 = pointInPolygonRes = PolygonTest.PointInPolygonResult.INSIDE OrElse pointInPolygonRes = PolygonTest.PointInPolygonResult.BOUNDARY
                Dim touchDetectedVectorTools = VectorTools.PointInPolygon(p, polygonPoints)
                If touchDetected0 <> touchDetectedVectorTools Then
                    nErr += 1
                    If nErr > 1 Then 'В отдельных угловых точках значения могут расходиться
                        Throw New Exception("touchDetected0 <> touchDetectedVectorTools")
                    End If
                Else
                    nErr = 0
                End If
            Next
        Next
    End Sub
End Class
