namespace Bwl.Imaging.Skia;

/// <summary>
/// Specifies the interpolation modes that can be used for image scaling and rendering operations.
/// </summary>
/// <remarks>While it's not used in this library directly, it still might help with migrations in other 
/// libraries or applications that rely on similar interpolation concepts.</remarks>
public enum SKInterpolationMode
{
    /// <summary>Indicates that the value is invalid.</summary>
    Invalid,
    /// <summary>Specifies the default interpolation mode.</summary>
    Default,
    /// <summary>Specifies a low-quality interpolation mode.</summary>
    Low,
    /// <summary>Specifies a high-quality interpolation mode.</summary>
    High,
    /// <summary>Specifies a bilinear interpolation mode.</summary>
    Bilinear,
    /// <summary>Specifies a bicubic interpolation mode.</summary>
    Bicubic,
    /// <summary>Specifies a nearest neighbor interpolation mode.</summary>
    NearestNeighbor,
    /// <summary>Specifies a high-quality bilinear interpolation mode.</summary>
    HighQualityBilinear,
    /// <summary>Specifies a high-quality bicubic interpolation mode.</summary>
    HighQualityBicubic
}
