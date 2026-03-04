namespace Bwl.Imaging.Skia
{
    /// <summary>
    /// Represents the formatting options for rendering text, including alignment, line alignment, and trimming behavior.
    /// </summary>
    public class SKStringFormat
    {
        /// <summary>
        /// Gets or sets the horizontal alignment of the text. The default value is SKStringAlignment.Near, which aligns the text to the left.
        /// </summary>
        public SKStringAlignment Alignment { get; set; } = SKStringAlignment.Near;

        /// <summary>
        /// Gets or sets the vertical alignment of the text. The default value is SKStringAlignment.Near, which aligns the text to the top.
        /// </summary>
        public SKStringAlignment LineAlignment { get; set; } = SKStringAlignment.Near;

        /// <summary>
        /// Gets or sets the trimming behavior of the text. The default value is SKStringTrimming.None, which means no trimming is applied.
        /// </summary>
        public SKStringTrimming Trimming { get; set; } = SKStringTrimming.None;

        /// <summary>
        /// Initializes a new instance of the SKStringFormat class with default values.
        /// </summary>
        public SKStringFormat() { }
    }
}
