using System.Runtime.Serialization;
using SkiaSharp;

namespace Bwl.Imaging.Skia;

[KnownType(typeof(Polygon))]
[KnownType(typeof(Tetragon))]
[KnownType(typeof(Line))]
[KnownType(typeof(SKPointC))]
[KnownType(typeof(SKPoint))]
[KnownType(typeof(SKPointI))]
[KnownType(typeof(SKRect))]
[KnownType(typeof(SKRectI))]
[KnownType(typeof(TextObject))]
[KnownType(typeof(SKBitmapObject))]
[KnownType(typeof(SKBitmap))]
public class DisplayObject : ICloneable
{
    public SKColor Color { get; set; } = SKColors.Red;
    public bool IsMoveable { get; set; } = false;
    public bool IsVisible { get; set; } = true;
    public bool IsSelectable { get; set; } = true;
    public string ID { get; set; } = "";
    public object DrawObject { get; set; }
    public float LineWidth { get; set; } = 0.0f;
    public float PointSize { get; set; } = 0.0f;
    public string Caption { get; set; } = "";
    public string Group { get; set; } = "";
    public ParametersDictionaryList Parameters { get; set; } = new ParametersDictionaryList();

    public DisplayObject()
    {
    }

    public DisplayObject(string id, SKColor color, object obj)
    {
        ID = id;
        Color = color;
        DrawObject = obj;
    }

    public DisplayObject(string id, SKColor color, object obj, bool isSelectable, bool isMoveable)
    {
        ID = id;
        Color = color;
        DrawObject = obj;
        IsMoveable = isMoveable;
        IsSelectable = isSelectable;
    }

    public DisplayObject(DisplayObject obj)
    {
        Color = obj.Color;
        IsMoveable = obj.IsMoveable;
        IsVisible = obj.IsVisible;
        IsSelectable = obj.IsSelectable;
        ID = obj.ID;
        DrawObject = obj.DrawObject;
        LineWidth = obj.LineWidth;
        PointSize = obj.PointSize;
        Caption = obj.Caption;
        Group = obj.Group;
    }

    public object Clone()
    {
        return new DisplayObject(this);
    }
}