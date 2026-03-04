using SkiaSharp;

namespace Bwl.Imaging.Skia;

/// <summary>
/// Provides extension methods for SKCanvas to facilitate text rendering with various formatting and layout options
/// </summary>
public static class SKTextExtensions
{

    /// <summary>
    /// Creates an SKRect from a given point and size, where the point represents the top-left corner of the rectangle.
    /// </summary>
    /// <param name="point">The top-left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <returns>A new SKRect instance.</returns>
    public static SKRect CreateSKRect(SKPoint point, SKSize size)
    {
        return new SKRect(point.X, point.Y, point.X + size.Width, point.Y + size.Height);
    }

    /// <summary>
    /// Wraps text into multiple lines that fit within the specified width.
    /// </summary>
    /// <param name="text">The text to wrap.</param>
    /// <param name="maxWidth">The maximum width for each line.</param>
    /// <param name="font">The font object used to measure text.</param>
    /// <param name="breakLongWords">If <see langword="true"/>, words longer than <paramref name="maxWidth"/> are split character-by-character; 
    /// otherwise they are kept intact (potentially exceeding the width).</param>
    /// <returns>A list of text lines.</returns>
    public static string[] WrapText(string text, float maxWidth, SKFont font, bool breakLongWords = true)
    {
        var lines = new List<string>();
        var paragraphs = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

        foreach (var paragraph in paragraphs)
        {
            if (string.IsNullOrEmpty(paragraph))
            {
                lines.Add(string.Empty);
                continue;
            }

            var words = paragraph.Split(new[] { ' ', '\t' }, StringSplitOptions.None);
            string currentLine = string.Empty;

            foreach (var word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                float width = font.MeasureText(testLine);

                if (width <= maxWidth || string.IsNullOrEmpty(currentLine))
                    currentLine = testLine;
                else
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }

                if (breakLongWords)
                {
                    while (font.MeasureText(currentLine) > maxWidth)
                    {
                        string fitting = string.Empty;
                        for (int i = 0; i < currentLine.Length; i++)
                        {
                            string test = currentLine.Substring(0, i + 1);
                            if (font.MeasureText(test) <= maxWidth)
                                fitting = test;
                            else
                                break;
                        }

                        if (string.IsNullOrEmpty(fitting))
                            fitting = currentLine.Substring(0, Math.Min(1, currentLine.Length));

                        lines.Add(fitting);
                        currentLine = currentLine.Substring(fitting.Length);
                    }
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
                lines.Add(currentLine);
        }

        return [.. lines];
    }

    /// <summary>
    /// Finds the largest font size such that <paramref name="text"/> fits within <paramref name="room"/>
    /// using binary search. The behavior depends on which dimensions are specified:
    /// <list type="bullet">
    /// <item><description>Only width specified: fits text as a single line within the width.</description></item>
    /// <item><description>Only height specified: fits text as a single line within the height.</description></item>
    /// <item><description>Both dimensions specified: fits multi-line wrapped text within both width and height.</description></item>
    /// </list>
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="typeface">The typeface to use for rendering the text.</param>
    /// <param name="room">The area the rendered text must fit within. At least one dimension must be greater than zero.</param>
    /// <returns>The largest font size in pixels that fits the text within the specified constraints, or 0 if fitting is impossible.</returns>
    public static float CalculateFontSizeToFit(string text, SKTypeface typeface, SKSize room)
    {
        if (string.IsNullOrEmpty(text)) return 0f;
        if (room.Width <= 0f && room.Height <= 0f) return 0f;

        bool hasWidth = room.Width > 0f;
        bool hasHeight = room.Height > 0f;

        // Start with the larger available dimension
        float size = hasHeight ? room.Height : room.Width;
        float step = size / 2f;
        float lastFittingSize = 0f;

        using var font = new SKFont(typeface, size);

        // For multi-line mode, cache the line count at each size to detect when we can stop early
        int previousLineCount = -1;

        while (step >= 0.1f || lastFittingSize == 0f)
        {
            if (size < 0.5f)
                break;

            font.Size = size;

            var metrics = font.Metrics;
            float lineHeight = metrics.Descent - metrics.Ascent + metrics.Leading;
            if (lineHeight <= 0f) lineHeight = metrics.Descent - metrics.Ascent;

            bool fits;

            if (hasWidth && hasHeight)
            {
                // Both dimensions specified: multi-line text (don't break long words - reduce size instead)
                var lines = WrapText(text, room.Width, font, breakLongWords: false);

                // Early exit: if increasing size doesn't change line count and we already found a fit, we're done
                if (lines.Length == previousLineCount && lastFittingSize > 0f && step < 1f)
                    break;

                previousLineCount = lines.Length;

                // Measure max line width directly during iteration (avoid LINQ overhead)
                float maxLineWidth = 0f;
                foreach (var line in lines)
                {
                    float w = font.MeasureText(line);
                    if (w > maxLineWidth) maxLineWidth = w;
                }

                float totalHeight = lines.Length * lineHeight;
                fits = maxLineWidth <= room.Width && totalHeight <= room.Height;
            }
            else if (hasWidth)
            {
                // Only width specified: single line constrained by width
                float textWidth = font.MeasureText(text);
                fits = textWidth <= room.Width;
            }
            else
            {
                // Only height specified: single line constrained by height
                fits = lineHeight <= room.Height;
            }

            if (fits)
            {
                lastFittingSize = size;
                size += step;
            }
            else
            {
                size -= step;
            }

            step /= 2f;
        }

        return lastFittingSize;
    }

    /// <summary>
    /// Sets the font size of the specified SKFont instance in pixels, converting from the provided points size.
    /// </summary>
    /// <remarks>The method converts the points size (i.e. System.Drawing.Common way) to pixels using a factor of 96 divided by 72,
    /// which aligns with standard font size conversions between points and pixels.</remarks>
    /// <param name="font">The SKFont instance whose font size will be updated.</param>
    /// <param name="pointsSize">The font size in points to be converted to pixels and applied to the SKFont.</param>
    public static void SetFontSizePoints(this SKFont font, float pointsSize)
    {
        font.Size = pointsSize * 96f / 72f;
    }

    /// <summary>
    /// Draws the specified text on the canvas at the given coordinates using the provided font and color.
    /// </summary>
    /// <remarks>The text is drawn starting at the specified (x, y) coordinates. The position corresponds to
    /// the origin point for the text layout.</remarks>
    /// <param name="canvas">The SKCanvas on which to render the text.</param>
    /// <param name="s">The text string to draw. If null, no text is rendered.</param>
    /// <param name="font">The SKFont that defines the typeface and size for the text.</param>
    /// <param name="color">The SKColor to use when rendering the text.</param>
    /// <param name="x">The x-coordinate, in pixels, of the text's starting position.</param>
    /// <param name="y">The y-coordinate, in pixels, of the text's starting position.</param>
    public static void DrawText(this SKCanvas canvas, string? s, SKFont font, SKColor color, float x, float y)
    {
        DrawText(canvas, s, font, color, CreateSKRect(new SKPoint(x, y), new SKSize(0f, 0f)), null);
    }

    /// <summary>
    /// Draws the specified text on the canvas at the given coordinates using the provided font and color.
    /// </summary>
    /// <remarks>This method provides a simplified way to draw text at a specific point on the canvas. The
    /// text is positioned using the top-left corner of its bounding box at the specified coordinates.</remarks>
    /// <param name="canvas">The SKCanvas on which to draw the text. Must be initialized before calling this method.</param>
    /// <param name="s">A read-only span of characters representing the text to be drawn.</param>
    /// <param name="font">The SKFont that defines the style and size of the text.</param>
    /// <param name="color">The SKColor to use when rendering the text.</param>
    /// <param name="x">The x-coordinate, in pixels, of the position where the text will be drawn.</param>
    /// <param name="y">The y-coordinate, in pixels, of the position where the text will be drawn.</param>
    public static void DrawText(this SKCanvas canvas, ReadOnlySpan<char> s, SKFont font, SKColor color, float x, float y)
    {
        DrawText(canvas, s, font, color, CreateSKRect(new SKPoint(x, y), new SKSize(0f, 0f)), null);
    }

    /// <summary>
    /// Draws the specified text on the canvas at the given position using the provided font and color.
    /// </summary>
    /// <remarks>This method provides a simplified way to render text at a specific point by automatically
    /// creating a layout rectangle based on the provided position.</remarks>
    /// <param name="canvas">The canvas on which to draw the text.</param>
    /// <param name="s">The text string to draw. If null, no text is rendered.</param>
    /// <param name="font">The font that defines the style and size of the text.</param>
    /// <param name="color">The color to use when rendering the text.</param>
    /// <param name="point">The position, in canvas coordinates, where the text will be drawn.</param>
    public static void DrawText(this SKCanvas canvas, string? s, SKFont font, SKColor color, SKPoint point)
    {
        DrawText(canvas, s, font, color, CreateSKRect(point, new SKSize(0f, 0f)), null);
    }

    /// <summary>
    /// Draws the specified text at the given position on the canvas using the provided font and color.
    /// </summary>
    /// <remarks>This method provides a simplified way to render text at a specific point without requiring a
    /// bounding rectangle. The text is positioned based on the provided point.</remarks>
    /// <param name="canvas">The canvas on which to draw the text.</param>
    /// <param name="s">A read-only span of characters representing the text to be drawn.</param>
    /// <param name="font">The font that defines the style and size of the text.</param>
    /// <param name="color">The color to use when rendering the text.</param>
    /// <param name="point">The position, in canvas coordinates, where the text will be drawn.</param>
    public static void DrawText(this SKCanvas canvas, ReadOnlySpan<char> s, SKFont font, SKColor color, SKPoint point)
    {
        DrawText(canvas, s, font, color, CreateSKRect(point, new SKSize(0f, 0f)), null);
    }

    /// <summary>
    /// Draws the specified text at the given coordinates on the canvas using the provided font and color.
    /// </summary>
    /// <remarks>This overload simplifies text drawing by allowing you to specify a position directly, rather
    /// than a bounding rectangle. The text is positioned based on the provided coordinates.</remarks>
    /// <param name="canvas">The canvas on which to draw the text.</param>
    /// <param name="s">The text string to draw. If null, no text is rendered.</param>
    /// <param name="font">The font that defines the style and size of the text.</param>
    /// <param name="color">The color to use when drawing the text.</param>
    /// <param name="x">The x-coordinate of the text's position on the canvas.</param>
    /// <param name="y">The y-coordinate of the text's position on the canvas.</param>
    /// <param name="format">An optional formatting object that specifies layout and alignment options for the text.</param>
    public static void DrawText(this SKCanvas canvas, string? s, SKFont font, SKColor color, float x, float y, SKStringFormat? format)
    {
        DrawText(canvas, s, font, color, CreateSKRect(new SKPoint(x, y), new SKSize(0f, 0f)), format);
    }

    /// <summary>
    /// Draws the specified text at the given coordinates on the canvas using the provided font and color.
    /// </summary>
    /// <remarks>This method draws text at a specific point on the canvas without specifying a bounding
    /// rectangle. Ensure that the canvas is properly initialized before calling this method.</remarks>
    /// <param name="canvas">The canvas on which to draw the text.</param>
    /// <param name="s">A read-only span of characters representing the text to be drawn.</param>
    /// <param name="font">The font that defines the style and size of the text.</param>
    /// <param name="color">The color to use when rendering the text.</param>
    /// <param name="x">The x-coordinate of the position where the text will be drawn.</param>
    /// <param name="y">The y-coordinate of the position where the text will be drawn.</param>
    /// <param name="format">An optional formatting object that specifies additional text layout options. May be null.</param>
    public static void DrawText(this SKCanvas canvas, ReadOnlySpan<char> s, SKFont font, SKColor color, float x, float y, SKStringFormat? format)
    {
        DrawText(canvas, s, font, color, CreateSKRect(new SKPoint(x, y), new SKSize(0f, 0f)), format);
    }

    /// <summary>
    /// Draws the specified text at the given position on the canvas using the provided font and color.
    /// </summary>
    /// <remarks>If the text string is null, this method does not render any text. The text is drawn with the
    /// specified font and color at the provided point on the canvas.</remarks>
    /// <param name="canvas">The SKCanvas on which to draw the text.</param>
    /// <param name="s">The text string to draw. If null, no text is rendered.</param>
    /// <param name="font">The SKFont that defines the typeface and size of the text.</param>
    /// <param name="color">The SKColor used to render the text.</param>
    /// <param name="point">The SKPoint specifying the position where the text will be drawn.</param>
    /// <param name="format">An optional SKStringFormat that specifies text formatting options. Can be null.</param>
    public static void DrawText(this SKCanvas canvas, string? s, SKFont font, SKColor color, SKPoint point, SKStringFormat? format)
    {
        DrawText(canvas, s, font, color, CreateSKRect(point, new SKSize(0f, 0f)), format);
    }

    /// <summary>
    /// Draws the specified text at a given point on the canvas using the provided font and color.
    /// </summary>
    /// <remarks>This method draws text at the specified point without requiring a bounding rectangle. The
    /// text will be rendered using the provided font and color. If additional formatting is needed, supply a non-null
    /// format parameter.</remarks>
    /// <param name="canvas">The canvas on which to draw the text. Cannot be null.</param>
    /// <param name="s">A read-only span of characters representing the text to draw.</param>
    /// <param name="font">The font that defines the style and size of the text. Cannot be null.</param>
    /// <param name="color">The color to use when drawing the text.</param>
    /// <param name="point">The location on the canvas where the text will be drawn.</param>
    /// <param name="format">An optional formatting object that specifies text layout options. May be null.</param>
    public static void DrawText(this SKCanvas canvas, ReadOnlySpan<char> s, SKFont font, SKColor color, SKPoint point, SKStringFormat? format)
    {
        DrawText(canvas, s, font, color, CreateSKRect(point, new SKSize(0f, 0f)), format);
    }

    /// <summary>
    /// Draws the specified text onto the given canvas using the provided font and color within the specified layout
    /// rectangle.
    /// </summary>
    /// <remarks>This method provides a simplified interface for drawing text on a canvas. It internally calls
    /// an overload that allows for additional customization.</remarks>
    /// <param name="canvas">The canvas on which to draw the text.</param>
    /// <param name="s">The text string to draw. If null, no text is rendered.</param>
    /// <param name="font">The font that defines the style and size of the text.</param>
    /// <param name="color">The color to use when drawing the text.</param>
    /// <param name="layoutRectangle">The rectangle that defines the area in which the text will be drawn. The text is clipped to this rectangle if it
    /// exceeds its bounds.</param>
    public static void DrawText(this SKCanvas canvas, string? s, SKFont font, SKColor color, SKRect layoutRectangle)
    {
        DrawText(canvas, s, font, color, layoutRectangle, null);
    }

    /// <summary>
    /// Draws the specified text onto the canvas using the given font and color, positioning it within the specified
    /// layout rectangle.
    /// </summary>
    /// <remarks>Ensure that the layout rectangle is appropriately sized to contain the rendered text. The
    /// method does not perform text wrapping or clipping beyond the bounds of the specified rectangle.</remarks>
    /// <param name="canvas">The SKCanvas on which to draw the text.</param>
    /// <param name="s">A read-only span of characters representing the text to render.</param>
    /// <param name="font">The SKFont that defines the typeface, size, and style for the text.</param>
    /// <param name="color">The SKColor to use when rendering the text.</param>
    /// <param name="layoutRectangle">The SKRect that specifies the area in which the text will be laid out and drawn.</param>
    public static void DrawText(this SKCanvas canvas, ReadOnlySpan<char> s, SKFont font, SKColor color, SKRect layoutRectangle)
    {
        DrawText(canvas, s, font, color, layoutRectangle, null);
    }

    /// <summary>
    /// Draws the specified text onto the given canvas using the provided font and color, constrained within the
    /// specified layout rectangle.
    /// </summary>
    /// <param name="canvas">The SKCanvas on which the text will be drawn.</param>
    /// <param name="s">The string to be drawn. If null, no text will be rendered.</param>
    /// <param name="font">The SKFont that defines the style and size of the text.</param>
    /// <param name="color">The SKColor that specifies the color of the text.</param>
    /// <param name="layoutRectangle">The SKRect that defines the area within which the text will be drawn. The text may be clipped if it exceeds this
    /// area.</param>
    /// <param name="format">An optional SKStringFormat that specifies how the text should be formatted, such as alignment and line spacing.</param>
    public static void DrawText(this SKCanvas canvas, string? s, SKFont font, SKColor color, SKRect layoutRectangle, SKStringFormat? format)
    {
        DrawTextInternal(canvas, s.AsSpan(), font, color, layoutRectangle, format);
    }

    /// <summary>
    /// Draws the specified text onto the provided canvas using the given font and color, positioning it within the
    /// defined layout rectangle and applying optional formatting.
    /// </summary>
    /// <remarks>This method enables custom text rendering on a canvas, allowing for precise control over
    /// layout and formatting. Use the format parameter to adjust alignment or text direction as needed.</remarks>
    /// <param name="canvas">The SKCanvas on which to draw the text.</param>
    /// <param name="s">A read-only span of characters representing the text to render.</param>
    /// <param name="font">The SKFont that specifies the typeface, size, and style for the text.</param>
    /// <param name="color">The SKColor used to paint the text.</param>
    /// <param name="layoutRectangle">The SKRect that defines the area in which the text will be laid out and drawn.</param>
    /// <param name="format">An optional SKStringFormat that specifies text formatting options such as alignment and direction. If null,
    /// default formatting is applied.</param>
    public static void DrawText(this SKCanvas canvas, ReadOnlySpan<char> s, SKFont font, SKColor color, SKRect layoutRectangle, SKStringFormat? format)
    {
        DrawTextInternal(canvas, s, font, color, layoutRectangle, format);
    }

    /// <summary>
    /// Draws the specified text onto the given canvas using the provided font and color, positioning and formatting it
    /// within the specified layout rectangle according to the given string format options.
    /// </summary>
    /// <remarks>This method supports multi-line text, word wrapping, alignment, and trimming within the
    /// specified rectangle. If the text exceeds the available space, it is trimmed or wrapped according to the provided
    /// format. Subpixel rendering and full hinting are enabled for improved text clarity.</remarks>
    /// <param name="canvas">The SKCanvas on which to render the text.</param>
    /// <param name="s">A read-only span of characters representing the text to be drawn.</param>
    /// <param name="font">The SKFont that defines the typeface, size, and rendering options for the text.</param>
    /// <param name="color">The SKColor used to render the text.</param>
    /// <param name="layoutRectangle">The SKRect that specifies the area within which the text will be drawn and aligned.</param>
    /// <param name="format">An optional SKStringFormat that determines text alignment, line alignment, and trimming behavior. If null,
    /// default alignment and no trimming are used.</param>
    private static void DrawTextInternal(SKCanvas canvas, ReadOnlySpan<char> s, SKFont font, SKColor color, SKRect layoutRectangle, SKStringFormat? format)
    {
        var prevSubpixel = font.Subpixel;
        var prevEdging = font.Edging;
        var prevHinting = font.Hinting;

        font.Subpixel = true; // Enable subpixel rendering for better text quality
        font.Edging = SKFontEdging.SubpixelAntialias; // Use subpixel anti-aliasing for clearer text
        font.Hinting = SKFontHinting.Full; // Use full hinting for better readability at small sizes

        try
        {

            using var paint = new SKPaint
            {
                Color = color,
                IsAntialias = true
            };

            var alignment = format?.Alignment ?? SKStringAlignment.Near;
            var lineAlignment = format?.LineAlignment ?? SKStringAlignment.Near;
            var trimming = format?.Trimming ?? SKStringTrimming.None;

            var metrics = font.Metrics;
            float lineHeight = metrics.Descent - metrics.Ascent + metrics.Leading;
            if (lineHeight <= 0f) lineHeight = metrics.Descent - metrics.Ascent;
            float padding = metrics.Descent;

            bool hasWidth = layoutRectangle.Width > 0f;
            bool hasHeight = layoutRectangle.Height > 0f;

            if (!hasHeight)
            {
                string text = s.ToString();
                if (hasWidth && trimming != SKStringTrimming.None)
                    text = ApplyTrimming(text, layoutRectangle.Width - padding * 2f, font, trimming);

                float x = ComputeLineX(text, font, layoutRectangle, padding, alignment, hasWidth);
                float y = layoutRectangle.Top - metrics.Ascent;
                canvas.DrawText(text, x, y, font, paint);
            }
            else
            {
                string[] allLines = hasWidth
                    ? WrapText(s.ToString(), layoutRectangle.Width - padding * 2f, font, breakLongWords: true)
                    : [s.ToString()];

                // startY must be derived from the full block so Center/Far alignment is correct
                // even when the block overflows; ClipRect handles the visual boundary.
                float totalTextHeight = allLines.Length * lineHeight;

                float startY = lineAlignment switch
                {
                    SKStringAlignment.Center => layoutRectangle.Top + (layoutRectangle.Height - totalTextHeight) / 2f,
                    SKStringAlignment.Far => layoutRectangle.Bottom - totalTextHeight,
                    _ => layoutRectangle.Top
                };

                // Find the lines that actually intersect the layout rectangle.
                int firstVisible = 0;
                while (firstVisible < allLines.Length - 1 && startY + lineHeight * (firstVisible + 1) <= layoutRectangle.Top)
                    firstVisible++;

                int lastVisible = allLines.Length - 1;
                while (lastVisible > firstVisible && startY + lineHeight * lastVisible >= layoutRectangle.Bottom)
                    lastVisible--;

                // Apply trimming only when content is hidden below the last visible line.
                if (lastVisible < allLines.Length - 1 && hasWidth && trimming != SKStringTrimming.None)
                    allLines[lastVisible] = ApplyTrimming(allLines[lastVisible], layoutRectangle.Width - padding * 2f, font, trimming);

                canvas.Save();
                canvas.ClipRect(layoutRectangle);

                for (int i = firstVisible; i <= lastVisible; i++)
                {
                    float baseline = startY - metrics.Ascent + lineHeight * i;
                    float x = ComputeLineX(allLines[i], font, layoutRectangle, padding, alignment, hasWidth);
                    canvas.DrawText(allLines[i], x, baseline, font, paint);
                }

                canvas.Restore();
            }
        }
        finally
        {
            font.Subpixel = prevSubpixel;
            font.Edging = prevEdging;
            font.Hinting = prevHinting;
        }
    }

    /// <summary>
    /// Calculates the X-coordinate at which to render the specified text within a layout rectangle, taking into account
    /// alignment, padding, and whether the text width should be considered.
    /// </summary>
    /// <param name="text">The text to be measured and positioned within the layout rectangle.</param>
    /// <param name="font">The font used to measure the width of the text.</param>
    /// <param name="layoutRectangle">The rectangle that defines the area in which the text will be rendered.</param>
    /// <param name="padding">The amount of horizontal padding to apply when positioning the text within the layout rectangle.</param>
    /// <param name="alignment">The alignment of the text within the layout rectangle. Determines how the text is positioned horizontally (for
    /// example, centered or aligned to the far edge).</param>
    /// <param name="hasWidth">A value indicating whether the width of the text should be considered when calculating its position. If false,
    /// the calculation does not account for the text width.</param>
    /// <returns>The calculated X-coordinate for rendering the text, based on the specified alignment, padding, and layout
    /// rectangle.</returns>
    private static float ComputeLineX(string text, SKFont font, SKRect layoutRectangle, float padding, SKStringAlignment alignment, bool hasWidth)
    {
        float textWidth = font.MeasureText(text);

        if (!hasWidth)
        {
            return alignment switch
            {
                SKStringAlignment.Center => layoutRectangle.Left - textWidth / 2f,
                SKStringAlignment.Far => layoutRectangle.Left - textWidth,
                _ => layoutRectangle.Left + padding
            };
        }

        return alignment switch
        {
            SKStringAlignment.Center => layoutRectangle.Left + (layoutRectangle.Width - textWidth) / 2f,
            SKStringAlignment.Far => layoutRectangle.Right - textWidth - padding,
            _ => layoutRectangle.Left + padding
        };
    }

    /// <summary>
    /// Trims the specified text to ensure it fits within the given maximum width, using the provided font and trimming
    /// style.
    /// </summary>
    /// <remarks>If the input text does not exceed the specified width, it is returned as-is. The method
    /// supports multiple trimming styles, including options that append an ellipsis or trim by word or
    /// character.</remarks>
    /// <param name="text">The text to be trimmed to fit within the specified width.</param>
    /// <param name="maxWidth">The maximum width, in pixels, that the resulting text should occupy.</param>
    /// <param name="font">The font used to measure the width of the text and determine how trimming is applied.</param>
    /// <param name="trimming">The trimming style to apply, which specifies how the text should be shortened (for example, by character, by
    /// word, or with an ellipsis).</param>
    /// <returns>A string containing the trimmed version of the input text that fits within the specified maximum width. If the
    /// text already fits, it is returned unchanged. The result may include an ellipsis if the trimming style specifies
    /// it.</returns>
    private static string ApplyTrimming(string text, float maxWidth, SKFont font, SKStringTrimming trimming)
    {
        if (font.MeasureText(text) <= maxWidth)
            return text;

        const string ellipsis = "\u2026";
        float ellipsisWidth = font.MeasureText(ellipsis);

        return trimming switch
        {
            SKStringTrimming.Character => TrimToCharacter(text, maxWidth, font, string.Empty),
            SKStringTrimming.Word => TrimToWord(text, maxWidth, font, string.Empty),
            SKStringTrimming.EllipsisCharacter => TrimToCharacter(text, maxWidth - ellipsisWidth, font, ellipsis),
            SKStringTrimming.EllipsisWord => TrimToWord(text, maxWidth - ellipsisWidth, font, ellipsis),
            SKStringTrimming.EllipsisPath => TrimPath(text, maxWidth, font, ellipsis),
            _ => text
        };
    }

    /// <summary>
    /// Trims the specified text so that its rendered width does not exceed the given maximum width, appending the
    /// specified suffix to the result.
    /// </summary>
    /// <remarks>If the entire text fits within the maximum width, the full text is returned with the suffix
    /// appended. The method does not account for the width of the suffix when determining how much of the text can
    /// fit.</remarks>
    /// <param name="text">The text to be trimmed based on the maximum allowed rendered width.</param>
    /// <param name="maxWidth">The maximum width, in pixels, that the rendered text and suffix can occupy.</param>
    /// <param name="font">The font used to measure the rendered width of the text.</param>
    /// <param name="suffix">The string to append to the trimmed text.</param>
    /// <returns>A string containing the longest leading substring of the input text that fits within the specified width when
    /// rendered with the given font, followed by the specified suffix.</returns>
    private static string TrimToCharacter(string text, float maxWidth, SKFont font, string suffix)
    {
        string fitting = string.Empty;
        for (int i = 0; i < text.Length; i++)
        {
            string candidate = text.Substring(0, i + 1);
            if (font.MeasureText(candidate) <= maxWidth)
                fitting = candidate;
            else
                break;
        }
        return fitting + suffix;
    }

    /// <summary>
    /// Trims the specified text to fit within the given maximum width when rendered with the provided font, preserving
    /// whole words where possible and appending a suffix if truncation occurs.
    /// </summary>
    /// <remarks>If no complete words fit within the maximum width, the method falls back to trimming the text
    /// at the character level. The result will always fit within the specified width when rendered with the given
    /// font.</remarks>
    /// <param name="text">The text to be trimmed to fit within the specified width.</param>
    /// <param name="maxWidth">The maximum width, in pixels, that the resulting text should occupy when rendered with the specified font. Must
    /// be greater than zero.</param>
    /// <param name="font">The font used to measure the width of the text. Cannot be null.</param>
    /// <param name="suffix">The string to append to the trimmed text if truncation is necessary, such as an ellipsis.</param>
    /// <returns>A string containing the trimmed text that fits within the specified width, followed by the suffix if truncation
    /// occurs.</returns>
    private static string TrimToWord(string text, float maxWidth, SKFont font, string suffix)
    {
        var words = text.Split(' ');
        string fitting = string.Empty;
        foreach (var word in words)
        {
            string candidate = string.IsNullOrEmpty(fitting) ? word : fitting + " " + word;
            if (font.MeasureText(candidate) <= maxWidth)
                fitting = candidate;
            else
                break;
        }
        if (string.IsNullOrEmpty(fitting))
            return TrimToCharacter(text, maxWidth, font, suffix);
        return fitting + suffix;
    }

    /// <summary>
    /// Trims a file or directory path to fit within a specified maximum width, appending an ellipsis if necessary to
    /// indicate omitted sections.
    /// </summary>
    /// <remarks>If the path fits within the specified width, it is returned unchanged. Otherwise, the method
    /// attempts to preserve the last segment of the path and uses a symmetric middle trim as a fallback to ensure the
    /// result fits within the given width.</remarks>
    /// <param name="text">The full path string to be trimmed.</param>
    /// <param name="maxWidth">The maximum width, in pixels, that the resulting string should occupy.</param>
    /// <param name="font">The font used to measure the width of the text and ellipsis.</param>
    /// <param name="ellipsis">The string to append to the trimmed path when it exceeds the maximum width.</param>
    /// <returns>A string representing the trimmed path that fits within the specified maximum width, including the ellipsis if
    /// trimming occurred.</returns>
    private static string TrimPath(string text, float maxWidth, SKFont font, string ellipsis)
    {
        float ellipsisWidth = font.MeasureText(ellipsis);
        int lastSep = text.LastIndexOfAny(['\\', '/']);
        string lastSegment = lastSep >= 0 ? text.Substring(lastSep + 1) : text;
        string prefix = lastSep >= 0 ? text.Substring(0, lastSep + 1) : string.Empty;

        float prefixBudget = maxWidth - ellipsisWidth - font.MeasureText(lastSegment);
        if (prefixBudget >= 0f)
        {
            string trimmedPrefix = TrimToCharacter(prefix, prefixBudget, font, string.Empty);
            string candidate = trimmedPrefix + ellipsis + lastSegment;
            if (font.MeasureText(candidate) <= maxWidth)
                return candidate;
        }

        // Fall back to symmetric middle trim
        float halfMax = (maxWidth - ellipsisWidth) / 2f;
        string start = TrimToCharacter(text, halfMax, font, string.Empty);
        string end = string.Empty;
        for (int i = 1; i <= text.Length; i++)
        {
            string seg = text.Substring(text.Length - i);
            if (font.MeasureText(seg) <= halfMax)
                end = seg;
            else
                break;
        }
        return start + ellipsis + end;
    }
}