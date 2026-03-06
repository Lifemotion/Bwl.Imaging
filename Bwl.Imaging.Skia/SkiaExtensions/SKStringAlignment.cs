namespace Bwl.Imaging.Skia;

/// <summary>
/// Defines the alignment options for text rendering in SkiaSharp.
/// </summary>
public enum SKStringAlignment
{
    // left or top in English
    /// <summary>
    /// Specifies the text be aligned near the layout. In a left-to-right layout, the near position is left. In a
    /// right-to-left layout, the near position is right.
    /// </summary>
    Near,

    /// <summary>
    /// Specifies that text is aligned in the center of the layout rectangle.
    /// </summary>
    Center,

    // right or bottom in English
    /// <summary>
    /// Specifies that text is aligned far from the origin position of the layout rectangle. In a left-to-right
    /// layout, the far position is right. In a right-to-left layout, the far position is left.
    /// </summary>
    Far
}
