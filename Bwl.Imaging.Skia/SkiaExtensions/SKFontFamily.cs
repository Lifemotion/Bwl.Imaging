namespace Bwl.Imaging.Skia
{
    /// <summary>
    /// Defines a set of generic font families that can be used for rendering text in SkiaSharp.
    /// </summary>
    public enum SKFontFamily
    {
        /// <summary>
        /// Specifies a generic serif font family.
        /// </summary>
        /// <remarks>
        /// A serif font is a typeface that has small lines or decorative strokes (called "serifs") 
        /// at the ends of the main strokes of the characters. Examples of serif fonts include 
        /// Times New Roman, Georgia, and Garamond.</remarks>
        GenericSerif,
        /// <summary>
        /// Gets the generic sans-serif font family used for rendering text.
        /// </summary>
        /// <remarks>
        /// This font family is typically used for displaying text in a clean and modern style,
        /// suitable for various applications.
        /// </remarks>
        GenericSansSerif,
        /// <summary>
        /// Represents a generic monospace font style used for displaying text in a fixed-width format.
        /// </summary>
        /// <remarks>
        /// This font style is typically used in code editors and terminal applications to ensure that each
        /// character occupies the same amount of horizontal space, aiding in readability and alignment of text.
        /// </remarks>
        GenericMonospace
    }
}
