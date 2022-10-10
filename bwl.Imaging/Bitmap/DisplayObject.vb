Imports System.Drawing
Imports System.Runtime.Serialization
<KnownType(GetType(Polygon))>
<KnownType(GetType(Tetragon))>
<KnownType(GetType(Line))>
<KnownType(GetType(PointC))>
<KnownType(GetType(PointF))>
<KnownType(GetType(Point))>
<KnownType(GetType(RectangleF))>
<KnownType(GetType(Rectangle))>
<KnownType(GetType(TextObject))>
<KnownType(GetType(BitmapObject))>
<KnownType(GetType(Bitmap))>
Public Class DisplayObject
    Implements ICloneable
    Public Property Color As Color = Drawing.Color.Red
    Public Property IsMoveable As Boolean = False
    Public Property IsVisible As Boolean = True
    Public Property IsSelectable As Boolean = True
    Public Property ID As String = ""
    Public Property DrawObject As Object
    Public Property LineWidth As Single = 0.0
    Public Property PointSize As Single = 0.0
    Public Property Caption As String = ""
    Public Property Group As String = ""
    Public Property Parameters As New ParametersDictionaryList

    Public Sub New()
    End Sub

    Public Sub New(id As String, color As Color, obj As Object)
        Me.ID = id
        Me.Color = color
        Me.DrawObject = obj
    End Sub

    Public Sub New(id As String, color As Color, obj As Object, isSelectable As Boolean, isMoveable As Boolean)
        Me.ID = id
        Me.Color = color
        Me.DrawObject = obj
        Me.IsMoveable = isMoveable
        Me.IsSelectable = isSelectable
    End Sub

    Public Sub New(obj As DisplayObject)
        Me.Color = obj.Color
        Me.IsMoveable = obj.IsMoveable
        Me.IsVisible = obj.IsVisible
        Me.IsSelectable = obj.IsSelectable
        Me.ID = obj.ID
        Me.DrawObject = obj.DrawObject
        Me.LineWidth = obj.LineWidth
        Me.PointSize = obj.PointSize
        Me.Caption = obj.Caption
        Me.Group = obj.Group
    End Sub

    Public Function Clone() As Object Implements ICloneable.Clone
        Return New DisplayObject(Me)
    End Function
End Class
