Imports System.Drawing

Public Class Region
    Inherits Polygon
    Implements ICloneable

    Public Const ParameterKey = "{RegionParameterKey}"

    Protected Const _lineWidth = 2
    Protected _parameters As New ParametersDictionary
    Protected _syncRoot As New Object

    Public Overloads Property Parameters As ParametersDictionary
        Get
            SyncLock _syncRoot
                Return _parameters
            End SyncLock
        End Get
        Set(value As ParametersDictionary)
            SyncLock _syncRoot
                _parameters = value
            End SyncLock
        End Set
    End Property

    Public Property Caption As String = String.Empty
    Public Property Description As String = String.Empty
    Public Property Color As Color = Color.LawnGreen
    Public Property ID As String = String.Empty
    Public Property Visible As Boolean = True

    Public Sub New()
        MyBase.New(True, {New PointF(), New PointF(), New PointF(), New PointF()}) 'Как Tetragon
        Me.ID = Guid.NewGuid.ToString()
    End Sub

    Public Sub New(points As PointF())
        MyBase.New(True, points)
        Me.ID = Guid.NewGuid.ToString()
    End Sub

    Public Sub New(id As String)
        MyBase.New(True, {New PointF(), New PointF(), New PointF(), New PointF()}) 'Как Tetragon
        Me.ID = id
    End Sub

    Public Sub New(id As String, points As PointF())
        MyBase.New(True, points)
        Me.ID = id
    End Sub

    Public Overridable Function ToDisplayObjects(Optional fullDisplay As Boolean = True, Optional textSizeF As Single = 0.013,
                                                 Optional channelIdxKey As String = "{ChannelIdxKey}") As DisplayObject()
        SyncLock _syncRoot
            'Check-Up
            If Not Visible OrElse Not IsNotEmpty() Then
                Return New DisplayObject() {}
            End If
            Dim displayObjects = New List(Of DisplayObject)

            'ChannelIdx / channelIdxDisplayObjectMarker
            Dim channelIdx As Integer
            If Not Parameters.ContainsKey(channelIdxKey) Then
                Throw New Exception("Not Parameters.ContainsKey(SharedParametersKeys.ChannelIdxKey)")
            Else
                channelIdx = Convert.ToInt32(Parameters(channelIdxKey).Value)
            End If
            Dim channelIdxDisplayObjectMarker = New Parameter(channelIdxKey, channelIdx.ToString())

            'Region
            Dim regionDisplayObj = New DisplayObject(ID, Color, New Polygon(True, Me.Points), True, True) With {.LineWidth = _lineWidth, .Group = ID}
            regionDisplayObj.Parameters.Add(channelIdxDisplayObjectMarker)
            displayObjects.Add(regionDisplayObj)

            'Если выбран режим "показать всё"...
            If fullDisplay Then
                Dim xF = Me.Points.Max(Function(item)
                                           Return item.X
                                       End Function) + textSizeF
                Dim yF = Me.Points.Min(Function(item)
                                           Return item.Y
                                       End Function) - textSizeF / 1.5F
                Dim textRowIdx = 0
                If Parameters IsNot Nothing Then
                    Dim parameterID As String
                    If Caption <> String.Empty Then
                        'Caption
                        parameterID = ID + "Caption"
                        Dim minX = Me.Points.Min(Function(item)
                                                     Return item.X
                                                 End Function) - textSizeF / 2.0F
                        Dim captionDisplayObject = New DisplayObject(parameterID, Color,
                                                                     New TextObject(minX, yF + (textRowIdx - 3) * textSizeF, Caption, textSizeF)) With {.Group = ID + "Text", .IsVisible = True}
                        captionDisplayObject.Parameters.Add(channelIdxDisplayObjectMarker)
                        displayObjects.Add(captionDisplayObject)
                    End If

                    'Перебираем все параметры, формируя пары "читаемое описание ключа" - "значение"
                    Dim parametersArr = Parameters.ToArray()
                    For Each parameterKVP In parametersArr
                        'Если параметр не начинается с маркера "ключ параметра"
                        If parameterKVP.Visible AndAlso Not parameterKVP.Key.StartsWith(Region.ParameterKey) _
                           AndAlso Not parameterKVP.Key.StartsWith(channelIdxKey) Then

                            parameterID = ID + parameterKVP.Key

                            Dim parameterKeyKVPValue As String
                            Dim parameterKeyKVP = parametersArr.FirstOrDefault(Function(item) item.Key = Region.ParameterKey + parameterKVP.Key)
                            If parameterKeyKVP IsNot Nothing AndAlso parameterKeyKVP.Visible Then
                                parameterKeyKVPValue = parameterKeyKVP.Value
                                '"Читаемое" наименование параметра (parameterKeyKVP.Value) / Значение параметра (parameterKVP.Value)
                                Dim parameterKeyDisplayObject = New DisplayObject(parameterID + Region.ParameterKey, Color,
                                                                                  New TextObject(xF, yF + textRowIdx * textSizeF, "< " + parameterKeyKVPValue + " >", textSizeF)) With {.Group = ID + "Text", .IsVisible = True}
                                Dim parameterValueDisplayObject = New DisplayObject(parameterID, Color,
                                                                                    New TextObject(xF, yF + (textRowIdx + 3) * textSizeF, parameterKVP.Value, textSizeF)) With {.Group = ID + "Text", .IsVisible = True}

                                parameterKeyDisplayObject.Parameters.Add(channelIdxDisplayObjectMarker)
                                parameterValueDisplayObject.Parameters.Add(channelIdxDisplayObjectMarker)
                                displayObjects.Add(parameterKeyDisplayObject)
                                displayObjects.Add(parameterValueDisplayObject)
                                textRowIdx += 6
                            End If
                        End If
                    Next

                    ''Position
                    'parameterID = ID + "Position"
                    ''"Читаемое" наименование параметра (parameterKeyKVP.Value) / Значение параметра (parameterKVP.Value)
                    'Dim positionKeyDisplayObject = New DisplayObject(parameterID + Region.ParameterKey, Color,
                    '                                                 New TextObject(xF, yF + textRowIdx * textSizeF, "< Положение >", textSizeF)) With {.Group = ID + "Text", .IsVisible = True}
                    'Dim positionValueDisplayObject = New DisplayObject(parameterID, Color,
                    '                                                   New TextObject(xF, yF + (textRowIdx + 3) * textSizeF, Position, textSizeF)) With {.Group = ID + "Text", .IsVisible = True}
                    'positionKeyDisplayObject.Parameters.Add(channelIdxDisplayObjectMarker)
                    'positionValueDisplayObject.Parameters.Add(channelIdxDisplayObjectMarker)
                    'displayObjects.Add(positionKeyDisplayObject)
                    'displayObjects.Add(positionValueDisplayObject)
                End If
            End If

            Return displayObjects.ToArray()
        End SyncLock
    End Function

    Public Overridable Function Clone() As Object Implements ICloneable.Clone
        Return CloneWithNewPoints(Me.Points)
    End Function

    Public Overridable Function CloneWithNewPoints(points As PointF()) As Region
        Dim obj = New Region(points) With
            {
                .Caption = Me.Caption,
                .Description = Me.Description,
                .Color = Me.Color,
                .ID = Me.ID,
                .Visible = Me.Visible
            }
        For Each kvp In Me.Parameters.ToArray()
            obj.Parameters.Add(kvp.Key, kvp.Value, kvp.Visible)
        Next
        Return obj
    End Function

    Private Function Position() As String
        Dim X = Points.Average(Function(item) item.X)
        Dim Y = Points.Average(Function(item) item.Y)
        Return String.Format("X:{0};Y:{1}", X.ToString("F3"), Y.ToString("F3"))
    End Function

    Private Function IsNotEmpty() As Boolean
        Return Points.Min(Function(item) item.X) <> Points.Max(Function(item) item.X)
    End Function
End Class
