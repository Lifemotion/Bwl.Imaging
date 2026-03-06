using SkiaSharp;

namespace Bwl.Imaging.Skia;


public class RegionWithVector : Region, ICloneable
{

    public static string VectorMarker { get; private set; } = "Vector";

    public string VectorCaption
    {
        get
        {
            return Caption + VectorMarker;
        }
    }

    public string VectorID
    {
        get
        {
            return ID + VectorMarker;
        }
    }

    public SKPoint Vector { get; set; } = new SKPoint();

    public Vector ImagingVector
    {
        get
        {
            var point1 = new SKPoint(Points.Average(item => item.X), Points.Average(item => item.Y));
            float point2X = point1.X + Vector.X;
            point2X = point2X < 0f ? 0f : point2X;
            point2X = point2X > 1f ? 1f : point2X;
            float point2Y = point1.Y + Vector.Y;
            point2Y = point2Y < 0f ? 0f : point2Y;
            point2Y = point2Y > 1f ? 1f : point2Y;
            var point2 = new SKPoint(point2X, point2Y);
            return new Vector(point1, point2);
        }
        set
        {
            Vector = new SKPoint(value.Points[1].X - value.Points[0].X, value.Points[1].Y - value.Points[0].Y);
        }
    }

    public RegionWithVector() : base(Guid.NewGuid().ToString(), new[] { new SKPoint(), new SKPoint(), new SKPoint(), new SKPoint() }) // Как Tetragon
    {
    }

    public RegionWithVector(SKPoint[] points) : base(Guid.NewGuid().ToString(), points)
    {
    }

    public RegionWithVector(string id) : base(id, new[] { new SKPoint(), new SKPoint(), new SKPoint(), new SKPoint() }) // Как Tetragon
    {
    }

    public RegionWithVector(string id, SKPoint[] points) : base(id, points) // Как Tetragon
    {
    }

    public override DisplayObject[] ToDisplayObjects(bool fullDisplay = true, float textSizeF = 0.013f, string channelIdxKey = "{ChannelIdxKey}")
    {
        var displayObjects = new List<DisplayObject>(base.ToDisplayObjects(fullDisplay, textSizeF, channelIdxKey));
        // Vector
        if (Vector.X != 0f | Vector.Y != 0f)
        {
            var displayObjectVector = new DisplayObject(VectorID, Color, ImagingVector)
            {
                Caption = Caption + VectorMarker,
                IsMoveable = true,
                IsSelectable = true
            };
            displayObjects.Insert(1, displayObjectVector);
        }
        return displayObjects.ToArray();
    }

    public override object Clone()
    {
        return CloneWithNewPoints(Points);
    }

    public new RegionWithVector CloneWithNewPoints(SKPoint[] points)
    {
        var obj = new RegionWithVector(points)
        {
            Caption = Caption,
            ID = ID,
            Color = Color,
            Vector = Vector,
            Description = Description
        };
        foreach (var kvp in Parameters.ToArray())
            obj.Parameters.Add(kvp.Key, kvp.Value);
        return obj;
    }

    public static Line[] GetVectorLines(Vector vector)
    {
        var lines = new List<Line>();
        double mulX = 1.0d;
        double mulY = 1.0d;
        float x1 = vector.Point1.X;
        float y1 = vector.Point1.Y;
        float x2 = vector.Point2.X;
        float y2 = vector.Point2.Y;
        float dx = x2 - x1;
        float dy = y2 - y1;
        double length = Math.Sqrt(Math.Pow((double)dx, 2d) + Math.Pow((double)dy, 2d));
        if (length > 0d)
        {
            float angle = (float)Math.Atan2((double)dy, (double)dx);
            double sz = 0.005d;
            var line1 = new Line((float)((double)x1 * mulX + (double)(float)(Math.Cos((double)angle - Math.PI / 2.0d) * sz)), (float)((double)y1 * mulY + (double)(float)(Math.Sin((double)angle - Math.PI / 2.0d) * sz)), (float)((double)x2 * mulX), (float)((double)y2 * mulY));
            var line2 = new Line((float)((double)x1 * mulX + (double)(float)(Math.Cos((double)angle + Math.PI / 2.0d) * sz)), (float)((double)y1 * mulY + (double)(float)(Math.Sin((double)angle + Math.PI / 2.0d) * sz)), (float)((double)x2 * mulX), (float)((double)y2 * mulY));
            var line3 = new Line((float)((double)x1 * mulX + (double)(float)(Math.Cos((double)angle - Math.PI / 2.0d) * sz)), (float)((double)y1 * mulY + (double)(float)(Math.Sin((double)angle - Math.PI / 2.0d) * sz)), (float)((double)x1 * mulX + (double)(float)(Math.Cos((double)angle + Math.PI / 2.0d) * sz)), (float)((double)y1 * mulY + (double)(float)(Math.Sin((double)angle + Math.PI / 2.0d) * sz)));
            lines.AddRange(new[] { line1, line2, line3 });
        }
        return lines.ToArray();
    }
}