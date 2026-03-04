
Imports NUnit.Framework
Imports SkiaSharp

<TestFixture>
<Parallelizable(ParallelScope.Self)>
Public Class VectorToolsTests

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub PointInPolygonIntegerTest()
        Dim polygonPoints = {
                                New SKPoint(370, 279),
                                New SKPoint(160, 106),
                                New SKPoint(262, 390),
                                New SKPoint(61, 433),
                                New SKPoint(358, 530),
                                New SKPoint(160, 869),
                                New SKPoint(412, 624),
                                New SKPoint(439, 942),
                                New SKPoint(528, 601),
                                New SKPoint(672, 797),
                                New SKPoint(598, 425),
                                New SKPoint(673, 276),
                                New SKPoint(427, 420),
                                New SKPoint(660, 124),
                                New SKPoint(358, 356),
                                New SKPoint(507, 95)
                            }
        PointInPolygonTest(polygonPoints)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub PointInPolygonSingleTest()
        Dim polygonPoints = {
                                New SKPoint(370, 279),
                                New SKPoint(160, 106),
                                New SKPoint(262, 390),
                                New SKPoint(61, 433),
                                New SKPoint(358, 530),
                                New SKPoint(160, 869),
                                New SKPoint(412, 624),
                                New SKPoint(439, 942),
                                New SKPoint(528, 601),
                                New SKPoint(672, 797),
                                New SKPoint(598, 425),
                                New SKPoint(673, 276),
                                New SKPoint(427, 420),
                                New SKPoint(660, 124),
                                New SKPoint(358, 356),
                                New SKPoint(507, 95)
                            }.Select(Function(item) New SKPoint(item.X / 1024, item.Y / 1024)).ToArray()
        PointInPolygonTest(polygonPoints)
    End Sub

    Public Sub PointInPolygonTest(polygonPoints As IEnumerable(Of SKPoint))
        Dim nErr = 0
        Dim polygonTest = New PolygonTest(polygonPoints)
        For x = 0 To 1024
            For y = 0 To 1024
                Dim p = New SKPoint(x, y)
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
