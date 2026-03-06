using System.Drawing; // The only place where System.Drawing is used in the whole project :)
using SkiaSharp;

namespace Bwl.Imaging.Skia;

/// <summary>
/// Class that contains extension methods to convert data between SkiaSharp and System.Drawing to ensure compatibility
/// Covers conversion of: colors, points, rectangles, sizes
/// </summary>
public static class SKDrawingCompatibility
{

    #region ColorConversions
    /// <summary>
    /// Converts a <see cref="Color"/> structure to an <see cref="SKColor"/> structure.
    /// </summary>
    /// <param name="color">The <see cref="Color"/> structure to convert, which represents the color in the RGB color model.</param>
    /// <returns>An <see cref="SKColor"/> structure that represents the same color as the input <see cref="Color"/>, with the same red, green, blue, and alpha values.</returns>
    public static SKColor ToSKColor(this Color color)
    {
        return new SKColor(color.R, color.G, color.B, color.A);
    }

    /// <summary>
    /// Converts an <see cref="SKColor"/> structure to a <see cref="Color"/> structure.
    /// </summary>
    /// <param name="color">The <see cref="SKColor"/> structure to convert, which contains the color components (alpha, red, green, blue).</param>
    /// <returns>A <see cref="Color"/> structure that represents the same color as the provided <see cref="SKColor"/>, with the corresponding alpha, red,
    /// green, and blue values.</returns>
    public static Color ToColor(this SKColor color)
    {
        return Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
    }
    #endregion

    #region PointConversions
    /// <summary>
    /// Converts a <see cref="PointF"/> structure to an <see cref="SKPoint"/> structure with
    /// equivalent X and Y coordinates.
    /// </summary>
    /// <param name="point">The <see cref="PointF"/> structure to convert. Represents the X and Y coordinates to be used for
    /// the resulting <see cref="SKPoint"/>.</param>
    /// <returns>An <see cref="SKPoint"/> structure that contains the same X and Y coordinate values as the specified
    /// <paramref name="point"/>.</returns>
    public static SKPoint ToSKPoint(this PointF point)
    {
        return new SKPoint(point.X, point.Y);
    }

    /// <summary>
    /// Converts a <see cref="Point"/> structure to an <see cref="SKPoint"/> structure with the same X and Y coordinates.
    /// </summary>
    /// <param name="point">The <see cref="Point"/> structure to convert. Represents the X and Y coordinates to be used for the resulting <see cref="SKPoint"/>.</param>
    /// <returns>An <see cref="SKPoint"/> structure whose X and Y values are set to those of the specified <see cref="Point"/>.</returns>
    public static SKPoint ToSkPoint(this Point point)
    {
        return new SKPoint(point.X, point.Y);
    }

    /// <summary>
    /// Converts a <see cref="PointF"/> structure to an <see cref="SKPointI"/> structure by rounding the X and Y coordinates to the nearest
    /// integers.
    /// </summary>
    /// <remarks>The conversion truncates any fractional component of the X and Y coordinates. This may result
    /// in a loss of precision if the original values are not whole numbers.</remarks>
    /// <param name="point">The <see cref="PointF"/> structure representing the floating-point coordinates to be converted.</param>
    /// <returns>An <see cref="SKPointI"/> structure containing the integer coordinates derived from the provided <see cref="PointF"/> structure.</returns>
    public static SKPointI ToSKPointI(this PointF point)
    {
        return new SKPointI((int)Math.Round(point.X), (int)Math.Round(point.Y));
    }

    /// <summary>
    /// Converts a <see cref="Point"/> structure to an <see cref="SKPointI"/> structure with the same X and Y coordinates.
    /// </summary>
    /// <param name="point">The <see cref="Point"/> structure to convert. Represents the X and Y coordinates to be used for the <see cref="SKPointI"/>.</param>
    /// <returns>An <see cref="SKPointI"/> structure whose X and Y values correspond to those of the specified <see cref="Point"/>.</returns>
    public static SKPointI ToSkPointI(this Point point)
    {
        return new SKPointI(point.X, point.Y);
    }

    /// <summary>
    /// Converts the specified <see cref="SKPoint"/> to an equivalent <see cref="PointF"/> structure.
    /// </summary>
    /// <param name="point">The <see cref="SKPoint"/> instance to convert, representing a point in two-dimensional space.</param>
    /// <returns>A <see cref="PointF"/> structure that represents the same coordinates as the specified <see cref="SKPoint"/>.</returns>
    public static PointF ToPointF(this SKPoint point)
    {
        return new PointF(point.X, point.Y);
    }

    /// <summary>
    /// Converts an <see cref="SKPointI"/> structure to a <see cref="PointF"/> structure with equivalent coordinates.
    /// </summary>
    /// <param name="point">The <see cref="SKPointI"/> instance to convert, representing a point with integer coordinates.</param>
    /// <returns>A <see cref="PointF"/> structure that represents the same point as the input, using floating-point coordinates.</returns>
    public static PointF ToPointF(this SKPointI point)
    {
        return new PointF(point.X, point.Y);
    }

    /// <summary>
    /// Converts an <see cref="SKPoint"/> structure to a <see cref="Point"/> structure by casting the X and Y
    /// coordinates to integers.
    /// </summary>
    /// <remarks>The conversion truncates any fractional component of the X and Y coordinates. This may result
    /// in a loss of precision if the original values are not whole numbers.</remarks>
    /// <param name="point">The <see cref="SKPoint"/> instance to convert. Represents a point in two-dimensional space with floating-point
    /// coordinates.</param>
    /// <returns>A <see cref="Point"/> whose X and Y values are the integer representations of the corresponding coordinates in
    /// <paramref name="point"/>.</returns>
    public static Point ToPoint(this SKPoint point)
    {
        return new Point((int)Math.Round(point.X), (int)Math.Round(point.Y));
    }

    /// <summary>
    /// Converts an instance of <see cref="SKPointI"/> to a <see cref="Point"/> structure.
    /// </summary>
    /// <param name="point">The <see cref="SKPointI"/> instance to convert, representing a point in integer coordinates.</param>
    /// <returns>A <see cref="Point"/> structure that represents the same coordinates as the provided <see cref="SKPointI"/>.</returns>
    public static Point ToPoint(this SKPointI point)
    {
        return new Point(point.X, point.Y);
    }
    #endregion

    #region RectangleConversions
    /// <summary>
    /// Converts a <see cref="RectangleF"/> structure to an <see cref="SKRect"/> structure with
    /// equivalent coordinates.
    /// </summary>
    /// <param name="rect">The <see cref="RectangleF"/> structure to convert. Represents a rectangle defined by
    /// floating-point coordinates.</param>
    /// <returns>An <see cref="SKRect"/> structure that represents the same rectangle as the input <paramref
    /// name="rect"/>.</returns>
    public static SKRect ToSKRect(this RectangleF rect)
    {
        return new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
    }

    /// <summary>
    /// Converts a <see cref="Rectangle"/> structure to an <see cref="SKRect"/> structure with
    /// equivalent coordinates.
    /// </summary>
    /// <param name="rect">The <see cref="Rectangle"/> structure to convert. Represents a rectangle defined by
    /// integer coordinates.</param>
    /// <returns>An <see cref="SKRect"/> structure that represents the same rectangle as the input <paramref
    /// name="rect"/>.</returns>
    public static SKRect ToSKRect(this Rectangle rect)
    {
        return new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
    }

    /// <summary>
    /// Converts a <see cref="RectangleF"/> structure to an <see cref="SKRectI"/> structure by casting coordinates to integers.
    /// </summary>
    /// <param name="rect">The <see cref="RectangleF"/> structure representing the floating-point rectangle to be converted.</param>
    /// <returns>An <see cref="SKRectI"/> structure containing integer coordinates derived from the provided <see cref="RectangleF"/>.</returns>
    public static SKRectI ToSKRectI(this RectangleF rect)
    {
        return new SKRectI((int)Math.Round(rect.Left), (int)Math.Round(rect.Top), (int)Math.Round(rect.Right), (int)Math.Round(rect.Bottom));
    }

    /// <summary>
    /// Converts a <see cref="Rectangle"/> structure to an <see cref="SKRectI"/> structure with equivalent coordinates.
    /// </summary>
    /// <param name="rect">The <see cref="Rectangle"/> structure to convert.</param>
    /// <returns>An <see cref="SKRectI"/> structure that represents the same rectangle as the input <see cref="Rectangle"/>.</returns>
    public static SKRectI ToSKRectI(this Rectangle rect)
    {
        return new SKRectI(rect.Left, rect.Top, rect.Right, rect.Bottom);
    }

    /// <summary>
    /// Converts the specified <see cref="SKRect"/> structure to an equivalent <see cref="RectangleF"/> structure.
    /// </summary>
    /// <param name="rect">The <see cref="SKRect"/> structure to convert. Represents the rectangle's position and size.</param>
    /// <returns>A <see cref="RectangleF"/> structure that has the same position and dimensions as the specified <see cref="SKRect"/>.</returns>
    public static RectangleF ToRectangleF(this SKRect rect)
    {
        return new RectangleF(rect.Left, rect.Top, (rect.Right - rect.Left), (rect.Bottom - rect.Top));
    }

    /// <summary>
    /// Converts the specified <see cref="SKRectI"/> structure to an equivalent <see cref="RectangleF"/> structure.
    /// </summary>
    /// <param name="rect">The <see cref="SKRectI"/> structure to convert. Represents the rectangle's position and size.</param>
    /// <returns>A <see cref="RectangleF"/> structure that has the same position and dimensions as the specified <see cref="SKRectI"/>.</returns>
    public static RectangleF ToRectangleF(this SKRectI rect)
    {
        return new RectangleF(rect.Left, rect.Top, rect.Width, rect.Height);
    }

    /// <summary>
    /// Converts an <see cref="SKRect"/> structure to a <see cref="Rectangle"/> structure by casting the rectangle coordinates to integers.
    /// </summary>
    /// <param name="rect">The <see cref="SKRect"/> structure to convert.</param>
    /// <returns>A <see cref="Rectangle"/> structure that represents the same rectangle as the provided <see cref="SKRect"/>.</returns>
    public static Rectangle ToRectangle(this SKRect rect)
    {
        return new Rectangle((int)Math.Round(rect.Left), (int)Math.Round(rect.Top), (int)Math.Round(rect.Right - rect.Left), (int)Math.Round(rect.Bottom - rect.Top));
    }

    /// <summary>
    /// Converts an <see cref="SKRectI"/> structure to a <see cref="Rectangle"/> structure.
    /// </summary>
    /// <param name="rect">The <see cref="SKRectI"/> structure to convert.</param>
    /// <returns>A <see cref="Rectangle"/> structure that represents the same rectangle as the provided <see cref="SKRectI"/>.</returns>
    public static Rectangle ToRectangle(this SKRectI rect)
    {
        return new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
    }
    #endregion

    #region SizeConversions

    /// <summary>
    /// Converts a <see cref="SizeF"/> structure to an <see cref="SKSize"/> structure.
    /// </summary>
    /// <param name="size">The <see cref="SizeF"/> structure to convert.</param>
    /// <returns>An <see cref="SKSize"/> structure that represents the same dimensions as the specified <see cref="SizeF"/>.</returns>
    public static SKSize ToSKSize(this SizeF size)
    {
        return new SKSize(size.Width, size.Height);
    }

    /// <summary>
    /// Converts a <see cref="Size"/> structure to an <see cref="SKSize"/> structure.
    /// </summary>
    /// <param name="size">The <see cref="Size"/> structure to convert.</param>
    /// <returns>An <see cref="SKSize"/> structure that represents the same dimensions as the specified <see cref="Size"/>.</returns>
    public static SKSize ToSKSize(this Size size)
    {
        return new SKSize(size.Width, size.Height);
    }

    /// <summary>
    /// Converts a <see cref="SizeF"/> structure to an <see cref="SKSizeI"/> structure by casting dimensions to integers.
    /// </summary>
    /// <param name="size">The <see cref="SizeF"/> structure to convert.</param>
    /// <returns>An <see cref="SKSizeI"/> structure that represents the integer dimensions derived from the provided <see cref="SizeF"/>.</returns>
    public static SKSizeI ToSKSizeI(this SizeF size)
    {
        return new SKSizeI((int)Math.Round(size.Width), (int)Math.Round(size.Height));
    }

    /// <summary>
    /// Converts a <see cref="Size"/> structure to an <see cref="SKSizeI"/> structure.
    /// </summary>
    /// <param name="size">The <see cref="Size"/> structure to convert.</param>
    /// <returns>An <see cref="SKSizeI"/> structure that represents the same dimensions as the specified <see cref="Size"/>.</returns>
    public static SKSizeI ToSKSizeI(this Size size)
    {
        return new SKSizeI(size.Width, size.Height);
    }

    /// <summary>
    /// Converts an <see cref="SKSize"/> structure to a <see cref="SizeF"/> structure.
    /// </summary>
    /// <param name="size">The <see cref="SKSize"/> structure to convert.</param>
    /// <returns>A <see cref="SizeF"/> structure that represents the same dimensions as the provided <see cref="SKSize"/>.</returns>
    public static SizeF ToSizeF(this SKSize size)
    {
        return new SizeF(size.Width, size.Height);
    }

    /// <summary>
    /// Converts an <see cref="SKSizeI"/> structure to a <see cref="SizeF"/> structure.
    /// </summary>
    /// <param name="size">The <see cref="SKSizeI"/> structure to convert.</param>
    /// <returns>A <see cref="SizeF"/> structure that represents the same dimensions as the provided <see cref="SKSizeI"/>.</returns>
    public static SizeF ToSizeF(this SKSizeI size)
    {
        return new SizeF(size.Width, size.Height);
    }

    /// <summary>
    /// Converts an <see cref="SKSize"/> structure to a <see cref="Size"/> structure by casting dimensions to integers.
    /// </summary>
    /// <param name="size">The <see cref="SKSize"/> structure to convert.</param>
    /// <returns>A <see cref="Size"/> structure that represents the integer dimensions derived from the provided <see cref="SKSize"/>.</returns>
    public static Size ToSize(this SKSize size)
    {
        return new Size((int)Math.Round(size.Width), (int)Math.Round(size.Height));
    }

    /// <summary>
    /// Converts an <see cref="SKSizeI"/> structure to a <see cref="Size"/> structure.
    /// </summary>
    /// <param name="size">The <see cref="SKSizeI"/> structure to convert.</param>
    /// <returns>A <see cref="Size"/> structure that represents the same dimensions as the provided <see cref="SKSizeI"/>.</returns>
    public static Size ToSize(this SKSizeI size)
    {
        return new Size(size.Width, size.Height);
    }

    #endregion
}
