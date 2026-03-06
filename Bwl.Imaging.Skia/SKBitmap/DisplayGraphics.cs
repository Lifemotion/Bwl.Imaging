using SkiaSharp;

namespace Bwl.Imaging.Skia;

public class DisplayGraphics
{
    public enum QualityMode
    {
        Normal,
        Fast
    }

    private int _width;
    private int _height;
    private float _offsetX;
    private float _offsetY;
    private float _mulX;
    private float _mulY;
    private float _baseMulX;
    private float _baseMulY;
    private bool _multiplyOnBitmapSize = true;
    protected object _syncRoot = new object();

    protected float _bkgX1F = 0f;
    protected float _bkgY1F = 0f;
    protected float _bkgX2F = 1.0f;
    protected float _bkgY2F = 1.0f;

    private const float PointsToPixels = 96f / 72f;

    protected SKCanvas _canvas;
    protected SKPaint _linePaint = new SKPaint { Color = SKColors.Black, IsStroke = true, StrokeWidth = 1f };
    protected SKPaint _fillPaint = new SKPaint { Color = SKColors.Black, IsStroke = false };
    protected SKFont _font = new SKFont(SKFontLoader.GetSKTypeface(SKFontFamily.GenericSansSerif));

    public SKColor BackgroundColor { get; set; } = SKColors.White;
    public float DefaultWidth { get; set; } = 1.0f;
    public float DefaultPointSize { get; set; } = 5.0f;
    public QualityMode Quality { get; set; } = QualityMode.Fast;

    public DisplayGraphics(SKCanvas graphics, int width, int height)
    {
        _font.SetFontSizePoints(21f);
        if (graphics is null)
            throw new ArgumentException("graphics is nothing");
        SetGraphics(graphics, width, height);
        ComputeMultipliers();
    }

    public SKCanvas Graphics
    {
        get
        {
            return _canvas;
        }
    }

    public int Width
    {
        get
        {
            return _width;
        }
    }

    public int Height
    {
        get
        {
            return _height;
        }
    }

    public void KeepAspectRatio()
    {
        KeepAspectRatio((int)Math.Round(_baseMulX), (int)Math.Round(_baseMulY));
    }

    public void KeepAspectRatio(int bkgWidth, int bkgHeight)
    {
        lock (_syncRoot)
        {
            _mulX = _baseMulX;
            _mulY = _baseMulY;
            _offsetX = 0f;
            _offsetY = 0f;
            float aspectRatioControl = _baseMulX / _baseMulY;
            double aspectRatioBitmap = bkgWidth / (double)bkgHeight;
            if ((double)aspectRatioControl > aspectRatioBitmap)
            {
                double ardivF = aspectRatioBitmap / (double)aspectRatioControl;
                _bkgX1F = (float)(0.5d - ardivF * 0.5d);
                _bkgY1F = 0f;
                _bkgX2F = (float)(0.5d + ardivF * 0.5d);
                _bkgY2F = 1f;
                float bkgWF = _bkgX2F - _bkgX1F;
                float bkgW = MultiplyOnBitmapSize ? bkgWF * _width : bkgWF;
                _mulX = bkgW;
                _offsetX = MultiplyOnBitmapSize ? _bkgX1F * _width : _bkgX1F;
            }
            if ((double)aspectRatioControl < aspectRatioBitmap)
            {
                double ardivF = (double)aspectRatioControl / aspectRatioBitmap;
                _bkgX1F = 0f;
                _bkgY1F = (float)(0.5d - ardivF * 0.5d);
                _bkgX2F = 1f;
                _bkgY2F = (float)(0.5d + ardivF * 0.5d);
                float bkgHF = _bkgY2F - _bkgY1F;
                float bkgH = MultiplyOnBitmapSize ? bkgHF * _height : bkgHF;
                _mulY = bkgH;
                _offsetY = MultiplyOnBitmapSize ? _bkgY1F * _height : _bkgY1F;
            }
        }
    }

    public void Clear(SKColor color)
    {
        lock (_syncRoot)
            _canvas.Clear(color);
    }

    public void Clear()
    {
        lock (_syncRoot)
            _canvas.Clear(BackgroundColor);
    }

    public void DrawSKBitmap(SKBitmapObject bo)
    {
        DrawSKBitmap(bo.SKBitmap, bo.SKRect);
    }

    public void DrawSKBitmap(SKBitmap bitmap, SKRect rect)
    {
        DrawSKBitmap(bitmap, rect.Left, rect.Top, rect.Right, rect.Bottom);
    }

    public void DrawSKBitmap(SKBitmap bitmap, float x1, float y1, float x2, float y2)
    {
        lock (_syncRoot)
        {
            float tx1 = x1 * _mulX + _offsetX;
            float ty1 = y1 * _mulY + _offsetY;
            float tx2 = tx1 + (x2 - x1) * _mulX;
            float ty2 = ty1 + (y2 - y1) * _mulY;
            _canvas.DrawBitmap(bitmap, new SKRect(tx1, ty1, tx2, ty2));
        }
    }

    public void DrawSKBitmap(SKBitmap bitmap, float x1, float y1)
    {
        lock (_syncRoot)
            _canvas.DrawBitmap(bitmap, x1 * _mulX + _offsetX, y1 * _mulY + _offsetY);
    }

    public void DrawSKBitmap(SKBitmap bitmap)
    {
        lock (_syncRoot)
            _canvas.DrawBitmap(bitmap, 0f, 0f);
    }

    public void FillRectanglesBase(SKColor color, SKRect[] rects)
    {
        lock (_syncRoot)
        {
            _fillPaint.Color = color;
            foreach (var item in rects)
                _canvas.DrawRect(SKExtensions.SKRectFromXYWH(item.Left * _baseMulX, item.Top * _baseMulY, item.Width * _baseMulX, item.Height * _baseMulY), _fillPaint);
        }
    }

    public void FillRectangles(SKColor color, SKRect[] rects)
    {
        lock (_syncRoot)
        {
            _fillPaint.Color = color;
            foreach (var item in rects)
                _canvas.DrawRect(SKExtensions.SKRectFromXYWH(item.Left * _mulX + _offsetX, item.Top * _mulY + _offsetY, item.Width * _mulX, item.Height * _mulY), _fillPaint);
        }
    }

    public bool MultiplyOnBitmapSize
    {
        set
        {
            _multiplyOnBitmapSize = value;
            ComputeMultipliers();
        }
        get
        {
            return _multiplyOnBitmapSize;
        }
    }

    private void ComputeMultipliers()
    {
        lock (_syncRoot)
        {
            _baseMulX = 1.0f;
            _baseMulY = 1.0f;
            if (MultiplyOnBitmapSize)
            {
                _baseMulX *= _width;
                _baseMulY *= _height;
            }
            _mulX = _baseMulX;
            _mulY = _baseMulY;
        }
    }

    public void DrawLine(SKColor color, float x1, float y1, float x2, float y2, float width = 0f)
    {
        lock (_syncRoot)
        {
            if (width <= 0f)
                width = DefaultWidth;
            if (_linePaint.Color != color || _linePaint.StrokeWidth != width)
            {
                _linePaint.Color = color;
                _linePaint.StrokeWidth = width;
            }
            _canvas.DrawLine(x1 * _mulX + _offsetX, y1 * _mulY + _offsetY, x2 * _mulX + _offsetX, y2 * _mulY + _offsetY, _linePaint);
        }
    }

    public void DrawVector(SKColor color, float x1, float y1, float x2, float y2, float width = 0f)
    {
        lock (_syncRoot)
        {
            if (width <= 0f)
                width = DefaultWidth;
            if (_linePaint.Color != color || _linePaint.StrokeWidth != width)
            {
                _linePaint.Color = color;
                _linePaint.StrokeWidth = width;
            }
            float dx = x2 - x1;
            float dy = y2 - y1;
            double length = Math.Sqrt(Math.Pow((double)dx, 2d) + Math.Pow((double)dy, 2d));
            if (length > 0d)
            {
                float angle = (float)Math.Atan2((double)dy, (double)dx);
                int sz = 5;
                _canvas.DrawLine(x1 * _mulX + _offsetX + (float)(Math.Cos((double)angle - Math.PI / 2d) * sz), y1 * _mulY + _offsetY + (float)(Math.Sin((double)angle - Math.PI / 2d) * sz), x2 * _mulX + _offsetX, y2 * _mulY + _offsetY, _linePaint);
                _canvas.DrawLine(x1 * _mulX + _offsetX + (float)(Math.Cos((double)angle + Math.PI / 2d) * sz), y1 * _mulY + _offsetY + (float)(Math.Sin((double)angle + Math.PI / 2d) * sz), x2 * _mulX + _offsetX, y2 * _mulY + _offsetY, _linePaint);
                _canvas.DrawLine(x1 * _mulX + _offsetX + (float)(Math.Cos((double)angle - Math.PI / 2d) * sz), y1 * _mulY + _offsetY + (float)(Math.Sin((double)angle - Math.PI / 2d) * sz), x1 * _mulX + _offsetX + (float)(Math.Cos((double)angle + Math.PI / 2d) * sz), y1 * _mulY + _offsetY + (float)(Math.Sin((double)angle + Math.PI / 2d) * sz), _linePaint);
            }
        }
    }

    public void DrawRectangle(SKColor color, SKRect rect, float width = 0f)
    {
        DrawRectangle(color, rect.Left, rect.Top, rect.Right, rect.Bottom, width);
    }

    public void DrawRectangle(SKColor color, SKRectI rect, float width = 0f)
    {
        DrawRectangle(color, rect.Left, rect.Top, rect.Right, rect.Bottom, width);
    }

    public void DrawRectangle(SKColor color, float x1, float y1, float x2, float y2, float width = 0f)
    {
        lock (_syncRoot)
        {
            if (width <= 0f)
                width = DefaultWidth;
            if (_linePaint.Color != color || _linePaint.StrokeWidth != width)
            {
                _linePaint.Color = color;
                _linePaint.StrokeWidth = width;
            }
            float tmp;
            if (x1 > x2)
            {
                tmp = x2;
                x2 = x1;
                x1 = tmp;
            }
            if (y1 > y2)
            {
                tmp = y2;
                y2 = y1;
                y1 = tmp;
            }
            _canvas.DrawRect(SKExtensions.SKRectFromXYWH(x1 * _mulX + _offsetX, y1 * _mulY + _offsetY, (x2 - x1) * _mulX, (y2 - y1) * _mulY), _linePaint);
        }
    }

    public void DrawPoint(SKColor color, float x1, float y1, float size = 0f)
    {
        lock (_syncRoot)
        {
            if (size <= 0f)
                size = DefaultPointSize;
            if (_fillPaint.Color != color)
                _fillPaint.Color = color;
            _canvas.DrawCircle(x1 * _mulX + _offsetX, y1 * _mulY + _offsetY, size / 2f, _fillPaint);
        }
    }

    public void DrawCircle(SKColor color, float x1, float y1, float radius, float width = 0f)
    {
        lock (_syncRoot)
        {
            if (width <= 0f)
                width = DefaultWidth;
            if (_linePaint.Color != color || _linePaint.StrokeWidth != width)
            {
                _linePaint.Color = color;
                _linePaint.StrokeWidth = width;
            }
            _canvas.DrawOval(SKExtensions.SKRectFromXYWH(x1 * _mulX + _offsetX, y1 * _mulY + _offsetY, radius * _mulX, radius * _mulX), _linePaint);
        }
    }

    public void DrawText(SKColor color, float x1, float y1, float size, string text)
    {
        lock (_syncRoot)
        {
            if (_fillPaint.Color != color)
                _fillPaint.Color = color;
            if (_font.Size != size * _mulX * PointsToPixels)
            {
                _font = new SKFont(SKFontLoader.GetSKTypeface(SKFontFamily.GenericSansSerif));
                _font.SetFontSizePoints(size * _mulX);
            }
            _canvas.DrawText(text, x1 * _mulX + _offsetX, y1 * _mulY + _offsetY, _font, _fillPaint);
        }
    }

    public void DrawText(SKColor color, TextObject textObj)
    {
        lock (_syncRoot)
        {
            if (_fillPaint.Color != color)
                _fillPaint.Color = color;
            if (_font.Size != textObj.Size * _mulX * PointsToPixels)
            {
                _font = new SKFont(SKFontLoader.GetSKTypeface(SKFontFamily.GenericSansSerif));
                _font.SetFontSizePoints(textObj.Size * _mulX * PointsToPixels);
            }
            _canvas.DrawText(textObj.Text, textObj.Point1.X * _mulX + _offsetX, textObj.Point1.Y * _mulY + _offsetY, _font, _fillPaint);
        }
    }

    public void DrawPoligon(SKColor color, Polygon poligon, float width = 0f)
    {
        lock (_syncRoot)
        {
            if (width <= 0f)
                width = DefaultWidth;
            if (_linePaint.Color != color || _linePaint.StrokeWidth != width)
            {
                _linePaint.Color = color;
                _linePaint.StrokeWidth = width;
            }
            for (int i = 0, loopTo = poligon.Points.Count() - 2; i <= loopTo; i++)
                _canvas.DrawLine(poligon.Points[i].X * _mulX + _offsetX, poligon.Points[i].Y * _mulY + _offsetY, poligon.Points[i + 1].X * _mulX + _offsetX, poligon.Points[i + 1].Y * _mulY + _offsetY, _linePaint);
            if (poligon.Points.Count() > 2 && poligon.IsClosed)
            {
                int last = poligon.Points.Count() - 1;
                _canvas.DrawLine(poligon.Points[last].X * _mulX + _offsetX, poligon.Points[last].Y * _mulY + _offsetY, poligon.Points[0].X * _mulX + _offsetX, poligon.Points[0].Y * _mulY + _offsetY, _linePaint);
            }
        }
    }

    public void DrawObject(SKColor color, object obj, float lineWidth = 0f, float pointSize = 0f)
    {
        if (typeof(Vector).IsAssignableFrom(obj.GetType()))
        {
            Vector vector = (Vector)obj;
            DrawVector(color, vector.Point1.X, vector.Point1.Y, vector.Point2.X, vector.Point2.Y);
        }
        else if (typeof(Polygon).IsAssignableFrom(obj.GetType()))
        {
            DrawPoligon(color, (Polygon)obj, lineWidth);
        }
        else if (typeof(SKPointC).IsAssignableFrom(obj.GetType()))
        {
            SKPointC pointC = (SKPointC)obj;
            DrawPoint(color, pointC.X, pointC.Y, pointSize);
        }
        else if (typeof(SKPoint).IsAssignableFrom(obj.GetType()))
        {
            SKPoint point = (SKPoint)obj;
            DrawPoint(color, point.X, point.Y, pointSize);
        }
        else if (typeof(SKPointI).IsAssignableFrom(obj.GetType()))
        {
            SKPointI point = (SKPointI)obj;
            DrawPoint(color, point.X, point.Y, pointSize);
        }
        else if (typeof(SKRect).IsAssignableFrom(obj.GetType()))
        {
            DrawRectangle(color, (SKRect)obj, lineWidth);
        }
        else if (typeof(SKRectC).IsAssignableFrom(obj.GetType()))
        {
            DrawRectangle(color, ((SKRectC)obj).SKRect, lineWidth);
        }
        else if (typeof(SKRectI).IsAssignableFrom(obj.GetType()))
        {
            DrawRectangle(color, (SKRectI)obj, lineWidth);
        }
        else if (typeof(SKBitmapObject).IsAssignableFrom(obj.GetType()))
        {
            DrawSKBitmap((SKBitmapObject)obj);
        }
        else if (typeof(SKBitmap).IsAssignableFrom(obj.GetType()))
        {
            DrawSKBitmap((SKBitmap)obj, 0f, 0f, 1f, 1f);
        }
        else if (typeof(TextObject).IsAssignableFrom(obj.GetType()))
        {
            DrawText(color, (TextObject)obj);
        }
    }

    public SKRect GetBitmapRectangle(SKRect objectRectangle)
    {
        return SKExtensions.SKRectFromXYWH(objectRectangle.Left * _mulX + _offsetX, objectRectangle.Top * _mulY + _offsetY, objectRectangle.Width * _mulX, objectRectangle.Height * _mulY);
    }

    public SKRect GetObjectRectangle(SKRect bitmapRectangle)
    {
        return SKExtensions.SKRectFromXYWH((bitmapRectangle.Left - _offsetX) / _mulX, (bitmapRectangle.Top - _offsetY) / _mulY, bitmapRectangle.Width / _mulX, bitmapRectangle.Height / _mulY);
    }

    public SKPoint GetBitmapPoint(SKPoint objectPoint)
    {
        return new SKPoint(objectPoint.X * _mulX + _offsetX, objectPoint.Y * _mulY + _offsetY);
    }

    public SKPoint GetObjectPoint(SKPoint bitmapPoint)
    {
        return new SKPoint((bitmapPoint.X - _offsetX) / _mulX, (bitmapPoint.Y - _offsetY) / _mulY);
    }

    public bool IsBitmapPointInsideBound(object obj, int bitmapX, int bitmapY)
    {
        var bound = GetBoundRectangeF(obj);
        var scrp = GetObjectPoint(new SKPoint(bitmapX, bitmapY));
        return bound.Contains(scrp);
    }

    public SKRect ExtendRectangleAtLineWidth(SKRect rect)
    {
        float width = DefaultWidth;
        if (width < 1f)
            width = 1f;
        float offset = width / _mulX;
        rect = rect.ToPositiveSized();
        return SKExtensions.SKRectFromXYWH(rect.Left - offset, rect.Top - offset, rect.Width + offset, rect.Height + offset);
    }

    public SKRect GetBoundRectangeF(object obj)
    {
        if (typeof(SKPointC).IsAssignableFrom(obj.GetType()))
        {
            SKPointC pointC = (SKPointC)obj;
            float px = pointC.X;
            float py = pointC.Y;
            float sx = DefaultPointSize * 2f / _mulX;
            var bound = SKExtensions.SKRectFromXYWH(px - sx / 2f, py - sx / 2f, sx, sx);
            return ExtendRectangleAtLineWidth(bound);
        }
        else if (typeof(SKPoint).IsAssignableFrom(obj.GetType()))
        {
            SKPoint point = (SKPoint)obj;
            float px = point.X;
            float py = point.Y;
            float sx = DefaultPointSize * 2f / _mulX;
            var bound = SKExtensions.SKRectFromXYWH(px - sx / 2f, py - sx / 2f, sx, sx);
            return ExtendRectangleAtLineWidth(bound);
        }
        else if (typeof(SKPointI).IsAssignableFrom(obj.GetType()))
        {
            SKPointI point = (SKPointI)obj;
            int px = point.X;
            int py = point.Y;
            float sx = DefaultPointSize * 2f / _mulX;
            var bound = SKExtensions.SKRectFromXYWH(px - sx / 2f, py - sx / 2f, sx, sx);
            return ExtendRectangleAtLineWidth(bound);
        }
        else if (typeof(SKRect).IsAssignableFrom(obj.GetType()))
        {
            SKRect bound = (SKRect)obj;
            return ExtendRectangleAtLineWidth(bound);
        }
        else if (typeof(SKRectI).IsAssignableFrom(obj.GetType()))
        {
            SKRectI bound = (SKRectI)obj;
            return ExtendRectangleAtLineWidth(new SKRect(bound.Left, bound.Top, bound.Right, bound.Bottom));
        }
        else if (typeof(Polygon).IsAssignableFrom(obj.GetType()))
        {
            var bound = ((Polygon)obj).GetBoundRectangleF();
            return ExtendRectangleAtLineWidth(bound);
        }
        else if (typeof(SKBitmapObject).IsAssignableFrom(obj.GetType()))
        {
            var bound = ((SKBitmapObject)obj).SKRect;
            return ExtendRectangleAtLineWidth(bound);
        }
        else if (typeof(SKBitmap).IsAssignableFrom(obj.GetType()))
        {
            var bound = new SKRect(0f, 0f, 1f, 1f);
            return ExtendRectangleAtLineWidth(bound);
        }
        else if (typeof(TextObject).IsAssignableFrom(obj.GetType()))
        {
            var bound = SKExtensions.SKRectFromPointSize(((TextObject)obj).Point1, new SKSize(0.1f, 0.1f));
            return ExtendRectangleAtLineWidth(bound);
        }
        return new SKRect(0f, 0f, 0f, 0f);
    }

    public void DrawDisplayObject(DisplayObject dispObj)
    {
        DrawObject(dispObj.Color, dispObj.DrawObject, dispObj.LineWidth, dispObj.PointSize);
    }

    public void SetGraphics(int width, int height)
    {
        SetGraphics(_canvas, width, height);
    }

    public void SetGraphics(SKCanvas graphics, int width, int height)
    {
        lock (_syncRoot)
        {
            if (width < 1)
                throw new ArgumentException("width must be >0");
            if (height < 1)
                throw new ArgumentException("height must be >0");
            _canvas = graphics;
            _width = width;
            _height = height;
            ComputeMultipliers();
        }
    }
}