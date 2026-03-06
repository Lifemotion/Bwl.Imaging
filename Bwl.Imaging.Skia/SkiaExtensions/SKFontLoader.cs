using SkiaSharp;

namespace Bwl.Imaging.Skia;

/// <summary>
/// Provides methods for loading and retrieving SKTypeface objects associated with specific font families.
/// </summary>
/// <remarks>This class manages a cache of SKTypeface objects for performance, ensuring that font
/// resources are efficiently loaded and accessed throughout the application. It is intended for internal use and is
/// not accessible outside the containing assembly.</remarks>
public static class SKFontLoader
{
    private static Dictionary<SKFontFamily, SKTypeface> _fontCache = new Dictionary<SKFontFamily, SKTypeface>();

    /// <summary>
    /// Initializes static resources for the FontLoader class before any static members are accessed or any
    /// instances are created.
    /// </summary>
    /// <remarks>This static constructor ensures that the default font families are loaded into the
    /// font cache and are available for use throughout the application. It is invoked automatically by the runtime
    /// and does not need to be called directly.</remarks>
    static SKFontLoader()
    {
        _fontCache[SKFontFamily.GenericSerif] = GetFont(SKFontFamily.GenericSerif);
        _fontCache[SKFontFamily.GenericSansSerif] = GetFont(SKFontFamily.GenericSansSerif);
        _fontCache[SKFontFamily.GenericMonospace] = GetFont(SKFontFamily.GenericMonospace);
    }

    /// <summary>
    /// Retrieves the SKTypeface associated with the specified font family.
    /// </summary>
    /// <remarks>This method accesses a cached SKTypeface for performance.</remarks>
    /// <param name="fontFamily">The font family for which to retrieve the SKTypeface. This parameter must not be null.</param>
    /// <returns>The SKTypeface corresponding to the specified font family.</returns>
    public static SKTypeface GetSKTypeface(SKFontFamily fontFamily)
    {
        return _fontCache[fontFamily];
    }


    /// <summary>
    /// Getting SKTypeface from embedded resources
    /// </summary>
    /// <param name="fontFamily">Font family to load</param>
    /// <returns>SKTypeface loaded from embedded resource</returns>
    private static SKTypeface GetFont(SKFontFamily fontFamily)
    {
        string resourceName = "Bwl.Imaging.Skia.Fonts.";
        switch (fontFamily)
        {
            case SKFontFamily.GenericSerif:
                resourceName += "NotoSerif.ttf";
                break;
            case SKFontFamily.GenericMonospace:
                resourceName += "NotoSansMono.ttf";
                break;
            default:
                resourceName += "NotoSans.ttf";
                break;
        }
        using var stream = typeof(SKFontLoader).Assembly.GetManifestResourceStream(resourceName);
        return SKTypeface.FromStream(stream);
    }
    
}
