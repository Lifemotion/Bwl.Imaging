Imports System.Drawing

Public Module VectorTools
    Public Class Vector
        Public Property X As Double
        Public Property Y As Double

        Public Sub New(X As Double, Y As Double)
            Me.X = X
            Me.Y = Y
        End Sub

        ''' <summary>
        ''' При вычислении вектора между (1,1) и (1,-1) этот метод дает - Pi/2! Это угловой переход от первого вектора ко второму
        ''' </summary>
        Public Shared Function AngleAB(A As Vector, B As Vector) As Double
            Return Math.Atan2(Cross(A, B), Dot(A, B))
        End Function

        Public ReadOnly Property Magnitude() As Double
            Get
                Return Math.Sqrt(Dot(Me, Me))
            End Get
        End Property

        Public Shared Function Dot(A As Vector, B As Vector) As Double
            Return A.X * B.X + A.Y * B.Y
        End Function

        Public Shared Function Cross(A As Vector, B As Vector) As Double
            Return A.X * B.Y - A.Y * B.X
        End Function
    End Class

    ''' <summary>
    ''' Тест: полигон пустой?
    ''' </summary>
    Public Function PolygonIsEmpty(polygon As PointF()) As Boolean
        If polygon.Length > 3 Then
            Dim firstPoint = polygon(0)
            For i = 1 To polygon.Length - 1
                Dim currentPoint = polygon(i)
                If currentPoint <> firstPoint Then
                    Return False
                End If
            Next
        End If
        Return True
    End Function

    ''' <summary>
    ''' Тест: точка находится внутри полигона?
    ''' </summary>
    Public Function PointInPolygon(point As PointF, polygon As PointF()) As Boolean
        Dim result = False
        Dim N = polygon.Length
        Dim j = N - 1
        For i = 0 To N - 1
            If ((polygon(i).Y <= point.Y) AndAlso (point.Y < polygon(j).Y)) OrElse ((polygon(j).Y <= point.Y) AndAlso (point.Y < polygon(i).Y)) Then
                If point.X < (polygon(j).X - polygon(i).X) * (point.Y - polygon(i).Y) / (polygon(j).Y - polygon(i).Y) + polygon(i).X Then
                    result = Not result
                End If
            End If
            j = i
        Next
        Return result
    End Function

    ''' <summary>
    ''' Нормализация длины вектора
    ''' </summary>    
    Public Function NormalizeVector(vector As PointF, Optional targetLen As Single = 1.0) As PointF
        Dim z = Math.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y))
        Dim zNorm = targetLen / z 'Нормализатор к заданной длине
        Dim xn = vector.X * zNorm
        Dim yn = vector.Y * zNorm
        Return New PointF(CSng(xn), CSng(yn))
    End Function

    ''' <summary>
    ''' Проверка на нахождение вектора между векторами-границами
    ''' </summary>
    Public Function VectorInBounds(vector As PointF, vectorBound1 As PointF, vectorBound2 As PointF, Optional tol As Double = 0.017) As Boolean
        Dim boundsAngle = AngleBetweenVectors(vectorBound1, vectorBound2)
        Dim angle1 = AngleBetweenVectors(vector, vectorBound1)
        Dim angle2 = AngleBetweenVectors(vector, vectorBound2)
        Dim angle = angle1 + angle2
        Dim angleDiff = Math.Abs(boundsAngle - angle)
        Return angleDiff <= tol
    End Function

    ''' <summary>
    ''' Вычисление модуля угла между векторами №1
    ''' </summary>
    Public Function AngleBetweenVectors(vector1 As PointF, vector2 As PointF) As Double
        Return Math.Abs(AnglePathBetweenVectors(vector1, vector2))
    End Function

    ''' <summary>
    ''' Вычисление углового перехода между двумя векторами
    ''' </summary>    
    Public Function AnglePathBetweenVectors(vector1 As PointF, vector2 As PointF) As Double
        Dim v1 = New Vector(vector1.X, vector1.Y)
        Dim v2 = New Vector(vector2.X, vector2.Y)
        Return Vector.AngleAB(v1, v2)
    End Function
End Module
