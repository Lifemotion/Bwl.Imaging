using System;
using System.Drawing;
using System.Linq;

namespace Bwl.Imaging
{

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

        protected Graphics _graphics;
        protected Pen _pen = new Pen(Brushes.Black);
        protected SolidBrush _brush = new SolidBrush(Color.Black);
        protected Font _font = new Font(FontFamily.GenericSansSerif, 21f);

        public Color BackgroundColor { get; set; } = Color.White;
        public float DefaultWidth { get; set; } = 1.0f;
        public float DefaultPointSize { get; set; } = 5.0f;
        public QualityMode Quality { get; set; } = QualityMode.Fast;

        public DisplayGraphics(Graphics graphics, int width, int height)
        {
            if (graphics is null)
                throw new ArgumentException("graphics is nothing");
            SetGraphics(graphics, width, height);
            ComputeMultipliers();
        }

        public Graphics Graphics
        {
            get
            {
                return _graphics;
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

        public void Clear(Color color)
        {
            lock (_syncRoot)
                _graphics.Clear(color);
        }

        public void Clear()
        {
            lock (_syncRoot)
                _graphics.Clear(BackgroundColor);
        }

        public void DrawBitmap(BitmapObject bo)
        {
            DrawBitmap(bo.Bitmap, bo.RectangleF);
        }

        public void DrawBitmap(Bitmap bitmap, RectangleF rect)
        {
            DrawBitmap(bitmap, rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public void DrawBitmap(Bitmap bitmap, float x1, float y1, float x2, float y2)
        {
            lock (_syncRoot)
                _graphics.DrawImage(bitmap, x1 * _mulX + _offsetX, y1 * _mulY + _offsetY, (x2 - x1) * _mulX, (y2 - y1) * _mulY);
        }

        public void DrawBitmap(Bitmap bitmap, float x1, float y1)
        {
            lock (_syncRoot)
                _graphics.DrawImage(bitmap, x1 * _mulX + _offsetX, y1 * _mulY + _offsetY);
        }

        public void DrawBitmap(Bitmap bitmap)
        {
            lock (_syncRoot)
                _graphics.DrawImage(bitmap, 0, 0);
        }

        public void FillRectanglesBase(Color color, RectangleF[] rects)
        {
            lock (_syncRoot)
                _graphics.FillRectangles(new SolidBrush(color), rects.Select(item => new RectangleF(item.X * _baseMulX, item.Y * _baseMulY, item.Width * _baseMulX, item.Height * _baseMulY)).ToArray());
        }

        public void FillRectangles(Color color, RectangleF[] rects)
        {
            lock (_syncRoot)
                _graphics.FillRectangles(new SolidBrush(color), rects.Select(item => new RectangleF(item.X * _mulX + _offsetX, item.Y * _mulY + _offsetY, item.Width * _mulX, item.Height * _mulY)).ToArray());
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

        public void DrawLine(Color color, float x1, float y1, float x2, float y2, float width = 0f)
        {
            lock (_syncRoot)
            {
                if (width <= 0f)
                    width = DefaultWidth;
                if (_pen.Color != color | _pen.Width != width)
                    _pen = new Pen(color, width);
                _graphics.DrawLine(_pen, x1 * _mulX + _offsetX, y1 * _mulY + _offsetY, x2 * _mulX + _offsetX, y2 * _mulY + _offsetY);
            }
        }

        public void DrawVector(Color color, float x1, float y1, float x2, float y2, float width = 0f)
        {
            lock (_syncRoot)
            {
                if (width <= 0f)
                    width = DefaultWidth;
                if (_pen.Color != color | _pen.Width != width)
                    _pen = new Pen(color, width);
                float dx = x2 - x1;
                float dy = y2 - y1;
                double length = Math.Sqrt(Math.Pow((double)dx, 2d) + Math.Pow((double)dy, 2d));
                if (length > 0d)
                {
                    float angle = (float)Math.Atan2((double)dy, (double)dx);
                    int sz = 5;
                    _graphics.DrawLine(_pen, x1 * _mulX + _offsetX + (float)(Math.Cos((double)angle - Math.PI / 2d) * sz), y1 * _mulY + _offsetY + (float)(Math.Sin((double)angle - Math.PI / 2d) * sz), x2 * _mulX + _offsetX, y2 * _mulY + _offsetY);
                    _graphics.DrawLine(_pen, x1 * _mulX + _offsetX + (float)(Math.Cos((double)angle + Math.PI / 2d) * sz), y1 * _mulY + _offsetY + (float)(Math.Sin((double)angle + Math.PI / 2d) * sz), x2 * _mulX + _offsetX, y2 * _mulY + _offsetY);
                    _graphics.DrawLine(_pen, x1 * _mulX + _offsetX + (float)(Math.Cos((double)angle - Math.PI / 2d) * sz), y1 * _mulY + _offsetY + (float)(Math.Sin((double)angle - Math.PI / 2d) * sz), x1 * _mulX + _offsetX + (float)(Math.Cos((double)angle + Math.PI / 2d) * sz), y1 * _mulY + _offsetY + (float)(Math.Sin((double)angle + Math.PI / 2d) * sz));
                }
                // x1 * _mulX - 5, y1 * _mulY - 5)
            }
        }

        public void DrawRectangle(Color color, RectangleF rect, float width = 0f)
        {
            DrawRectangle(color, rect.Left, rect.Top, rect.Right, rect.Bottom, width);
        }

        public void DrawRectangle(Color color, Rectangle rect, float width = 0f)
        {
            DrawRectangle(color, rect.Left, rect.Top, rect.Right, rect.Bottom, width);
        }

        public void DrawRectangle(Color color, float x1, float y1, float x2, float y2, float width = 0f)
        {
            lock (_syncRoot)
            {
                if (width <= 0f)
                    width = DefaultWidth;
                if (_pen.Color != color | _pen.Width != width)
                    _pen = new Pen(color, width);
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
                _graphics.DrawRectangle(_pen, x1 * _mulX + _offsetX, y1 * _mulY + _offsetY, (x2 - x1) * _mulX, (y2 - y1) * _mulY);
            }
        }

        public void DrawPoint(Color color, float x1, float y1, float size = 0f)
        {
            lock (_syncRoot)
            {
                if (size <= 0f)
                    size = DefaultPointSize;
                if (_brush.Color != color)
                    _brush = new SolidBrush(color);
                _graphics.FillEllipse(_brush, x1 * _mulX + _offsetX - size / 2f, y1 * _mulY + _offsetY - size / 2f, size, size);
            }
        }

        public void DrawCircle(Color color, float x1, float y1, float radius, float width = 0f)
        {
            lock (_syncRoot)
            {
                if (width <= 0f)
                    width = DefaultWidth;
                if (_pen.Color != color | _pen.Width != width)
                    _pen = new Pen(color, width);
                _graphics.DrawEllipse(_pen, x1 * _mulX + _offsetX, y1 * _mulY + _offsetY, radius * _mulX, radius * _mulX);
            }
        }

        public void DrawText(Color color, float x1, float y1, float size, string text)
        {
            lock (_syncRoot)
            {
                if (_brush.Color != color)
                    _brush = new SolidBrush(color);
                if (_font.Size != size * _mulX)
                    _font = new Font(FontFamily.GenericSansSerif, size * _mulX);
                _graphics.DrawString(text, _font, _brush, x1 * _mulX + _offsetX, y1 * _mulY + _offsetY);
            }
        }

        public void DrawText(Color color, TextObject textObj)
        {
            lock (_syncRoot)
            {
                if (_brush.Color != color)
                    _brush = new SolidBrush(color);
                if (_font.Size != textObj.Size * _mulX)
                    _font = new Font(FontFamily.GenericSansSerif, textObj.Size * _mulX);
                _graphics.DrawString(textObj.Text, _font, _brush, textObj.Point1.X * _mulX + _offsetX, textObj.Point1.Y * _mulY + _offsetY);
            }
        }

        public void DrawPoligon(Color color, Polygon poligon, float width = 0f)
        {
            lock (_syncRoot)
            {
                if (width <= 0f)
                    width = DefaultWidth;
                if (_pen.Color != color | _pen.Width != width)
                    _pen = new Pen(color, width);
                for (int i = 0, loopTo = poligon.Points.Count() - 2; i <= loopTo; i++)
                    _graphics.DrawLine(_pen, poligon.Points[i].X * _mulX + _offsetX, poligon.Points[i].Y * _mulY + _offsetY, poligon.Points[i + 1].X * _mulX + _offsetX, poligon.Points[i + 1].Y * _mulY + _offsetY);
                if (poligon.Points.Count() > 2 & poligon.IsClosed)
                {
                    int last = poligon.Points.Count() - 1;
                    _graphics.DrawLine(_pen, poligon.Points[last].X * _mulX + _offsetX, poligon.Points[last].Y * _mulY + _offsetY, poligon.Points[0].X * _mulX + _offsetX, poligon.Points[0].Y * _mulY + _offsetY);
                }
            }
        }

        public void DrawObject(Color color, object obj, float lineWidth = 0f, float pointSize = 0f)
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
            else if (typeof(PointC).IsAssignableFrom(obj.GetType()))
            {
                PointC pointC = (PointC)obj;
                DrawPoint(color, pointC.X, pointC.Y, pointSize);
            }
            else if (typeof(PointF).IsAssignableFrom(obj.GetType()))
            {
                PointF pointF = (PointF)obj;
                DrawPoint(color, pointF.X, pointF.Y, pointSize);
            }
            else if (typeof(Point).IsAssignableFrom(obj.GetType()))
            {
                Point point = (Point)obj;
                DrawPoint(color, point.X, point.Y, pointSize);
            }
            else if (typeof(RectangleF).IsAssignableFrom(obj.GetType()))
            {
                DrawRectangle(color, (RectangleF)obj, lineWidth);
            }
            else if (typeof(RectangleFC).IsAssignableFrom(obj.GetType()))
            {
                DrawRectangle(color, ((RectangleFC)obj).RectangleF, lineWidth);
            }
            else if (typeof(Rectangle).IsAssignableFrom(obj.GetType()))
            {
                DrawRectangle(color, (Rectangle)obj, lineWidth);
            }
            else if (typeof(BitmapObject).IsAssignableFrom(obj.GetType()))
            {
                DrawBitmap((BitmapObject)obj);
            }
            else if (typeof(Bitmap).IsAssignableFrom(obj.GetType()))
            {
                DrawBitmap((Bitmap)obj, 0f, 0f, 1f, 1f);
            }
            else if (typeof(TextObject).IsAssignableFrom(obj.GetType()))
            {
                DrawText(color, (TextObject)obj);
            }
        }

        public RectangleF GetBitmapRectangle(RectangleF objectRecrtangle)
        {
            return new RectangleF(objectRecrtangle.Left * _mulX + _offsetX, objectRecrtangle.Top * _mulY + _offsetY, objectRecrtangle.Width * _mulX, objectRecrtangle.Height * _mulY);
        }

        public RectangleF GetObjectRectangle(RectangleF bitmapRectangle)
        {
            return new RectangleF((bitmapRectangle.Left - _offsetX) / _mulX, (bitmapRectangle.Top - _offsetY) / _mulY, bitmapRectangle.Width / _mulX, bitmapRectangle.Height / _mulY);
        }

        public PointF GetBitmapPoint(PointF objectPoint)
        {
            return new PointF(objectPoint.X * _mulX + _offsetX, objectPoint.Y * _mulY + _offsetY);
        }

        public PointF GetObjectPoint(PointF bitmapPoint)
        {
            return new PointF((bitmapPoint.X - _offsetX) / _mulX, (bitmapPoint.Y - _offsetY) / _mulY);
        }

        public bool IsBitmapPointInsideBound(object obj, int bitmapX, int bitmapY)
        {
            var bound = GetBoundRectangeF(obj);
            var scrp = GetObjectPoint(new PointF(bitmapX, bitmapY));
            return bound.Contains(scrp);
        }

        public RectangleF ExtendRectangleAtLineWidth(RectangleF rect)
        {
            float width = DefaultWidth;
            if (width < 1f)
                width = 1f;
            float offset = width / _mulX;
            rect = rect.ToPositiveSized();
            var newrect = new RectangleF(rect.Left - offset, rect.Top - offset, rect.Width + offset, rect.Height + offset);
            return newrect;
        }

        public RectangleF GetBoundRectangeF(object obj)
        {
            if (typeof(PointC).IsAssignableFrom(obj.GetType()))
            {
                PointC pointC = (PointC)obj;
                float px = pointC.X;
                float py = pointC.Y;
                float sx = DefaultPointSize * 2f / _mulX;
                var bound = new RectangleF(px - sx / 2f, py - sx / 2f, sx, sx);
                return ExtendRectangleAtLineWidth(bound);
            }
            else if (typeof(PointF).IsAssignableFrom(obj.GetType()))
            {
                PointF pointF = (PointF)obj;
                float px = pointF.X;
                float py = pointF.Y;
                float sx = DefaultPointSize * 2f / _mulX;
                var bound = new RectangleF(px - sx / 2f, py - sx / 2f, sx, sx);
                return ExtendRectangleAtLineWidth(bound);
            }
            else if (typeof(Point).IsAssignableFrom(obj.GetType()))
            {
                Point point = (Point)obj;
                int px = point.X;
                int py = point.Y;
                float sx = DefaultPointSize * 2f / _mulX;
                var bound = new RectangleF(px - sx / 2f, py - sx / 2f, sx, sx);
                return ExtendRectangleAtLineWidth(bound);
            }
            else if (typeof(RectangleF).IsAssignableFrom(obj.GetType()))
            {
                RectangleF bound = (RectangleF)obj;
                return ExtendRectangleAtLineWidth(bound);
            }
            else if (typeof(Rectangle).IsAssignableFrom(obj.GetType()))
            {
                Rectangle bound = (Rectangle)obj;
                return ExtendRectangleAtLineWidth(bound);
            }
            else if (typeof(Polygon).IsAssignableFrom(obj.GetType()))
            {
                var bound = ((Polygon)obj).GetBoundRectangleF();
                return ExtendRectangleAtLineWidth(bound);
            }
            else if (typeof(BitmapObject).IsAssignableFrom(obj.GetType()))
            {
                var bound = ((BitmapObject)obj).RectangleF;
                return ExtendRectangleAtLineWidth(bound);
            }
            else if (typeof(Bitmap).IsAssignableFrom(obj.GetType()))
            {
                var bound = new RectangleF(0f, 0f, 1f, 1f);
                return ExtendRectangleAtLineWidth(bound);
            }
            else if (typeof(TextObject).IsAssignableFrom(obj.GetType()))
            {
                var bound = new RectangleF(((TextObject)obj).Point1, new SizeF(0.1f, 0.1f));
                return ExtendRectangleAtLineWidth(bound);
            }
            return new RectangleF(0f, 0f, 0f, 0f);
        }

        public void DrawDisplayObject(DisplayObject dispObj)
        {
            DrawObject(dispObj.Color, dispObj.DrawObject, dispObj.LineWidth, dispObj.PointSize);
        }

        public void SetGraphics(int width, int height)
        {
            SetGraphics(_graphics, width, height);
        }

        public void SetGraphics(Graphics graphics, int width, int height)
        {
            lock (_syncRoot)
            {
                if (width < 1)
                    throw new ArgumentException("width must be >0");
                if (height < 1)
                    throw new ArgumentException("height must be >0");
                _graphics = graphics;
                _width = width;
                _height = height;
                if (Quality == QualityMode.Fast)
                {
                    _graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    _graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    _graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                }
                ComputeMultipliers();
            }
        }
    }
}