# Bwl.Imaging

[![CI](https://github.com/Lifemotion/Bwl.Imaging/actions/workflows/ci.yml/badge.svg)](https://github.com/Lifemotion/Bwl.Imaging/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Bwl.Imaging.svg)](https://www.nuget.org/packages/Bwl.Imaging)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Bwl.Imaging.svg)](https://www.nuget.org/packages/Bwl.Imaging)
[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![.NET Standard](https://img.shields.io/badge/.NET_Standard-2.0-purple.svg)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)

A computer vision library for .NET providing matrix-based image processing, high-performance unsafe bitmap operations, geometry primitives, color space conversions, HDR frame support, and WinForms display controls.

**Platform:** Windows (built on `System.Drawing.Common` / GDI+).

---

## Packages

| Package | NuGet | Target | Description |
|---------|-------|--------|-------------|
| [Bwl.Imaging](https://www.nuget.org/packages/Bwl.Imaging) | [![NuGet](https://img.shields.io/nuget/v/Bwl.Imaging.svg)](https://www.nuget.org/packages/Bwl.Imaging) | `netstandard2.0` | Core: matrices, filters, geometry, color, HDR frames |
| [Bwl.Imaging.Unsafe](https://www.nuget.org/packages/Bwl.Imaging.Unsafe) | [![NuGet](https://img.shields.io/nuget/v/Bwl.Imaging.Unsafe.svg)](https://www.nuget.org/packages/Bwl.Imaging.Unsafe) | `netstandard2.0` | Fast crop, patch, sharpen, normalize via unsafe code |
| [Bwl.Imaging.WinForms](https://www.nuget.org/packages/Bwl.Imaging.WinForms) | [![NuGet](https://img.shields.io/nuget/v/Bwl.Imaging.WinForms.svg)](https://www.nuget.org/packages/Bwl.Imaging.WinForms) | `net48` | Bitmap viewer, object overlay controls |

---

## Installation

```bash
dotnet add package Bwl.Imaging           # Core library (includes Bwl.Imaging.Unsafe)
dotnet add package Bwl.Imaging.WinForms  # Optional: display controls
```

---

## Usage examples

### Basic image processing pipeline

```vb
Imports Bwl.Imaging

' Load image and convert to matrix representation
Dim bmp = New Bitmap("photo.jpg")
Dim gray = BitmapConverter.BitmapToGrayMatrix(bmp)

' Apply filters
Dim sharpened = Filters.Sharpen5Gray(gray)
Dim filtered = Filters.MedianFilter2D(sharpened, 3)
Dim contrasted = Filters.LinearContrast(filtered, 0, 255)

' Convert back to bitmap and save
Dim result = BitmapConverter.GrayMatrixToBitmap(contrasted)
result.Save("output.bmp")
```

Or using extension methods:

```vb
Dim result = New Bitmap("photo.jpg").BitmapToGrayMatrix().ToBitmap()
```

### RGB per-pixel access

```vb
Dim rgb = BitmapConverter.BitmapToRGBMatrix(bmp)

For y = 0 To rgb.Height - 1
    For x = 0 To rgb.Width - 1
        ' Access as RGB structure
        Dim pixel = rgb.RGBPixel(x, y)

        ' Access as HSV (automatic conversion)
        Dim hsv = rgb.HSVPixel(x, y)

        ' Access as System.Drawing.Color
        Dim color = rgb.ColorPixel(x, y)

        ' Individual channel access
        Dim r = rgb.RedPixel(x, y)
        rgb.GreenPixel(x, y) = 0
        rgb.BluePixel(x, y) = pixel.B \ 2
    Next
Next

' Convert back
Dim output = rgb.ToBitmap()
```

### Matrix operations

```vb
' Resize
Dim half = gray.ResizeHalf()         ' Downsample 2x (averages 2x2 blocks)
Dim doubled = gray.ResizeTwo()       ' Upsample 2x (pixel replication)

' Sub-region extraction
Dim region = MatrixTools.GrayMatrixSubRect(gray, New Rectangle(10, 10, 100, 100))

' Alignment (required for some bitmap formats)
Dim aligned = MatrixTools.GrayMatrixAlign4(gray)   ' Align width to multiple of 4

' Inversion
Dim inverted = MatrixTools.InverseGray(gray)       ' 255 - value

' Brightness statistics
Dim stats = ImagingMath.GetBrightnessStats(gray)
' stats.BrMin, stats.BrMax, stats.BrAvg, stats.Histogram(0..255)

' Gray <-> RGB conversion
Dim rgbFromGray = gray.ToRGBMatrix()
Dim grayFromRgb = rgb.ToGrayMatrix()  ' Uses CCIR-601: Y = 0.299R + 0.587G + 0.114B
```

### Unsafe high-performance operations

All operations use `Bitmap.LockBits()` + pointer arithmetic + `Parallel.For` for maximum throughput.

```vb
Imports Bwl.Imaging.Unsafe

' --- Crop & Patch ---
Dim cropped = UnsafeFunctions.CropRgb(sourceBmp, New Rectangle(10, 10, 200, 200))
UnsafeFunctions.PatchRgb(patchBmp, targetBmp, New Rectangle(50, 50, 200, 200))

' Grayscale variants
Dim grayCrop = UnsafeFunctions.CropGray(sourceBmp, New Rectangle(0, 0, 64, 64))
UnsafeFunctions.PatchGray(grayPatch, targetBmp, region)

' --- Sharpen (5x5 sparse kernel, parallelized) ---
Dim sharp = UnsafeFunctions.Sharpen5Rgb(sourceBmp)
Dim sharpGray = UnsafeFunctions.Sharpen5Gray(grayBmp)

' --- Histogram normalization ---
Dim normalized = UnsafeFunctions.NormalizeRgb(sourceBmp, borderPercent:=1)
Dim normalizedGray = UnsafeFunctions.NormalizeGray(grayBmp)

' --- Color conversion ---
Dim grayBmp = UnsafeFunctions.RgbToGray(rgbBmp)
UnsafeFunctions.RgbReverse(bmp)    ' Invert all channels

' --- Fast bitmap clone via memcpy ---
Dim clone = UnsafeFunctions.BitmapClone(sourceBmp)

' --- Bitmap probing (downsampled pixel check) ---
Dim probeGray = UnsafeFunctions.BitmapProbeGray(bmp, stepSize)  ' Returns GrayMatrix
Dim probeRgb = UnsafeFunctions.BitmapProbeRgb(bmp, stepSize)    ' Returns RGBMatrix

' --- Integrity check (detect corrupted JPEG decodes) ---
Dim isOk = UnsafeFunctions.JpegDecodedOK(bmp)

' --- Hash (fast bitmap fingerprinting) ---
Dim hash = UnsafeFunctions.BitmapHashRgb(bmp, stepSize)

' --- Unmanaged memory interop ---
Dim bmpFromPtr = UnsafeFunctions.BitmapFromIntPtr(scan0, size, pixelFormat)
Dim byteArray = UnsafeFunctions.BitmapToArray(bmp)
Dim bmpFromArray = UnsafeFunctions.ArrayToBitmap(byteArray)
```

### Motion detection

```vb
Dim detector As New MoveDetector With {
    .PointDiffThreshSetting = 5,     ' Per-pixel brightness difference threshold
    .MoveThresholdSetting = 1.0,     ' Fraction of changed pixels to trigger motion
    .AfterMoveSetting = 3            ' Keep reporting motion for N frames after event
}

AddHandler detector.Logger, Sub(logType, msg)
    Console.WriteLine($"[{logType}] {msg}")
End Sub

' Feed frames in a loop (e.g. from a camera)
While capturing
    Dim hasMotion = detector.Process(currentFrame)  ' Accepts Bitmap or GrayMatrix
    If hasMotion Then
        ' Handle motion event
    End If
End While
```

### Connected component segmentation

```vb
' Binarize and label connected components
Dim segMap = Segmentation.CreateSegmentsMapWithBinarize(grayMatrix,
    binarizeThreshold:=128, invert:=False, passes:=2)

' Or from a pre-processed matrix
Dim segMap2 = Segmentation.CreateSegmentsMap(binaryMatrix, threshold:=1)

' Extract segment list with bounding boxes
Dim segments = Segmentation.CreateSegmentsList(segMap)

' Filter by size
Dim filtered = Segmentation.FilterSegmentsList(segments,
    minWidth:=10, minHeight:=10, maxWidth:=500, maxHeight:=500)

' Filter by aspect ratio
Dim squares = Segmentation.FilterSegmentsList(segments,
    minWHratio:=0.8, maxWHRatio:=1.2)

' Visualize: each segment gets a random color
Dim colorized = Segmentation.ColorizeSegments(segMap)  ' Returns RGBMatrix

' Access individual segments
For Each seg In filtered
    Console.WriteLine($"Segment {seg.ID}: ({seg.Left},{seg.Top}) {seg.Width}x{seg.Height}, " &
                      $"center=({seg.CenterX},{seg.CenterY}), ratio={seg.WHRatio:F2}")
    Dim contains = seg.IsPointInside(x, y)
    Dim rect = seg.ToRectangle()
Next
```

### HDR frames (12-bit+)

```vb
' Create raw integer frame (e.g. from 12-bit camera)
Dim frame As New RawIntFrame(width, height, rawData)

' Tone mapping to 8-bit Bitmap
Dim hdr1 = RawFrameFunctions.ConvertRawToHDRBitmap1Fast(frame.Data, width, height, baseGain)
Dim hdr3 = RawFrameFunctions.ConvertRawToHDRBitmap3Fast(frame.Data, width, height)  ' Power law

' Serialize: saves as JPEG pair (high bytes + low bytes) for lossless 12-bit storage
frame.SaveToJpegPair("frame_001", quality:=95)
Dim loaded = RawIntFrame.FromJpegPair("frame_001")

' Binary serialization
frame.Save("frame.dat")
Dim fromFile = RawIntFrame.FromFile("frame.dat")

' 8-bit pair conversion
Dim pair = RawIntFrameConverters.ConvertIntArrayTo8BitPair(frame.Data)
Dim restored = RawIntFrameConverters.Convert8BitPairToIntArray(pair.Item1, pair.Item2)
```

### Geometry primitives

```vb
' --- Polygon (base class) ---
Dim poly As New Polygon(closed:=True, p1, p2, p3, p4)
Dim bounds = poly.GetBoundRectangleF()
' poly.Left, poly.Top, poly.Right, poly.Bottom, poly.Width, poly.Height

' --- Line ---
Dim line As New Line(x1, y1, x2, y2)
' line.Point1, line.Point2

' --- Vector (directed line) ---
Dim vec As New Vector(fromPoint, toPoint)
' vec.PointFrom, vec.PointTo, vec.Vector (as PointF offset)

' --- Tetragon (4-point polygon) ---
Dim quad As New Tetragon(p1, p2, p3, p4)
quad.SetRectangle(left, top, right, bottom)
quad.Expand(offset:=5)
' quad.Point1, quad.Point2, quad.Point3, quad.Point4

' --- Region (polygon with metadata) ---
Dim region As New Region("zone-1", points)
region.Color = Color.Red
region.Caption = "Detection Zone"
region.Description = "Primary monitoring area"
region.Visible = True
region.Parameters.Add("priority", "high")

' --- RegionWithVector (region + direction) ---
Dim rv As New RegionWithVector("tracker-1", points)
rv.ImagingVector = New Vector(p1, p2)
Dim arrowLines = RegionWithVector.GetVectorLines(rv.ImagingVector)

' --- Utilities ---
Dim dist = point1.Dist(point2)                  ' Distance between PointF
Dim normalized = rectF.ToPositiveSized()         ' Normalize negative dimensions
```

### Color spaces

```vb
' --- RGB structure ---
Dim rgb As New RGB(255, 128, 0)           ' R, G, B (alpha defaults to 255)
Dim rgba As New RGB(255, 128, 0, 200)     ' R, G, B, A
Dim fromColor As New RGB(Color.Coral)     ' From System.Drawing.Color

' --- HSV structure ---
Dim hsv As New HSV(180, 100, 50)          ' Hue (0-360), Saturation (0-255), Value (0-255)

' --- Bidirectional conversion ---
Dim hsvFromRgb = rgb.ToHSV()
Dim rgbFromHsv = RGB.FromHsv(hsv)
Dim color = rgb.ToColor()                 ' -> System.Drawing.Color
Dim hsvFromColor = HSV.FromRgb(Color.Red)

' --- Hue distance (handles 360-degree wraparound) ---
Dim dist = HSV.HueDistance(350, 10)       ' Returns 20, not 340
```

### Thread-safe BitmapInfo wrapper

```vb
' Create from bitmap or JPEG bytes
Dim info As New BitmapInfo(myBitmap)
Dim infoFromJpg As New BitmapInfo(jpegBytes)

' --- Lock-based access (required for .Bmp property) ---
info.BmpLock(timeoutMs:=1000)
Try
    Dim bmp = info.Bmp           ' Direct Bitmap access
    ' ... work with bitmap ...
Finally
    info.BmpUnlock()
End Try

' --- JPEG compression (frees Bitmap memory, keeps compressed data) ---
info.Compress(quality:=85, timeoutMs:=1000)
Dim jpg = info.GetJpg(createFromBitmapIfEmpty:=True, quality:=85, timeoutMs:=1000)
info.SetJpg(newJpegData, timeoutMs:=1000)

' --- Safe cloning ---
Dim clonedBmp = info.GetClonedBmp(timeoutMs:=1000)
Dim clonedGray = info.GetClonedBmpGray(timeoutMs:=1000)
Dim clonedInfo = info.GetClonedCopy(timeoutMs:=1000)

' --- BitmapInfo extensions ---
Dim grayMatrix = info.GetGrayMatrix()     ' MatrixTools extension
Dim rgbMatrix = info.GetRGBMatrix()

' --- State queries ---
Dim size = info.BmpSize                   ' Cached: no lock needed
Dim fmt = info.BmpPixelFormat
Dim hasData = Not info.BmpAndJpgAreNothing

' --- Global statistics (thread-safe via Interlocked) ---
Dim totalBytes = BitmapInfo.GlobalAllocatedDataCount
Dim compressions = BitmapInfo.GlobalCompressedCount
Dim decompressions = BitmapInfo.GlobalDecompressedCount

' --- Global clone mode ---
BitmapInfo.SafeCloneMode = True           ' Use managed cloning (slower but safer)

' --- Cleanup ---
info.Clear(timeoutMs:=1000)
info.Dispose()
```

### WinForms display controls

```vb
' --- PictureBox extensions ---
PictureBox1.ShowMatrix(grayMatrix)        ' Display GrayMatrix
PictureBox1.ShowMatrix(rgbMatrix)         ' Display RGBMatrix

' --- DisplayBitmapControl (UserControl) ---
' A double-buffered bitmap viewer with auto-resize
DisplayBitmapControl1.Bitmap = myBitmap

' --- DisplayObjectsControl (extends DisplayBitmapControl) ---
' Interactive overlay: draws polygons/lines/regions on top of a bitmap
' Supports mouse click events, object selection, move mode

AddHandler DisplayObjectsControl1.MouseClickF, Sub(sender, e, pointF)
    Console.WriteLine($"Clicked at object coordinates: {pointF}")
End Sub

AddHandler DisplayObjectsControl1.MouseClickOnBackgroundF, Sub(sender, e, pointF)
    Console.WriteLine($"Clicked on background at: {pointF}")
End Sub

DisplayObjectsControl1.BackgroundBitmap = bgBitmap
DisplayObjectsControl1.MoveMode = True
DisplayObjectsControl1.ShowStatusBar = True
DisplayObjectsControl1.ShowClickPoint = True
DisplayObjectsControl1.SelectedObject = someDisplayObject
```

---

## Architecture

### Processing pipeline

The core design separates pixel storage from GDI+ Bitmap, enabling direct array manipulation without overhead:

```
Bitmap (GDI+)
  |
  v
BitmapOperations.LoadBitmap()     -- handles Format32bppArgb, Format24bppRgb, Format8bppIndexed
  |                                  reads raw bytes respecting stride alignment
  v
byte[] (raw pixel data)
  |
  +-> GetGrayMatrix()              -- CCIR-601 luma: Y = 0.299R + 0.587G + 0.114B
  |     -> GrayMatrix               -- Integer[x + y * Width], single channel
  |
  +-> GetRGBMatrix()
        -> RGBMatrix                -- Integer[] per channel (Red, Green, Blue)
          |
          v
        Processing                  -- filters, resize, segmentation, motion detection
          |                            all operations work on Integer[] arrays
          v
        BitmapConverter.MatrixToBitmap()  -- back to Bitmap
```

### Matrix class hierarchy

```
CommonMatrix                          -- base: List(Of Integer[]), Width, Height
  |                                      MatrixPixel(channel, x, y), FitX/FitY bounds clamping
  |                                      Clone, ResizeHalf, ResizeTwo
  |
  +-- GrayMatrix                      -- single channel: GrayPixel(x, y)
  |     ToRGBMatrix(), ToBitmap()
  |
  +-- RGBMatrix                       -- 3 channels: Red, Green, Blue arrays
        RGBPixel(x,y), HSVPixel(x,y), ColorPixel(x,y)
        RedPixel/GreenPixel/BluePixel individual access
        ToGrayMatrix(), ToBitmap()

CommonFloatMatrix                     -- base: List(Of Double[])
  +-- GrayFloatMatrix                 -- double-precision grayscale
  +-- RGBFloatMatrix                  -- double-precision RGB
```

### Unsafe layer (C#)

`Bwl.Imaging.Unsafe` provides performance-critical operations in two files:

**UnsafeFunctions.cs:**
- `Bitmap.LockBits()` + `Scan0` pointer arithmetic for zero-copy pixel access
- `Parallel.For` for multi-threaded row-based processing
- P/Invoke to `msvcrt.dll` (`memcpy`) for bulk memory operations
- Supports 8bpp grayscale, 24bpp RGB, 32bpp ARGB pixel formats
- Operations: Crop, Patch, Sharpen (5x5), Normalize, RgbToGray, Clone, Probe, Hash

**RawFrameFunctions.cs:**
- HDR (12-bit+) to 8-bit Bitmap conversion
- Gain-based linear mapping (`ConvertRawToHDRBitmap1Fast`)
- Power-law tone mapping with lookup tables (`ConvertRawToHDRBitmap3Fast`)
- Parallelized per-channel processing

### Thread safety model

| Component | Mechanism | Notes |
|-----------|-----------|-------|
| `BitmapInfo` | `Semaphore(1,1)` per instance | Must call `BmpLock()`/`BmpUnlock()` for `.Bmp` access |
| `BitmapInfo` counters | `Interlocked` operations | `GlobalAllocatedDataCount`, etc. |
| `MoveDetector` | `SyncLock Me` | Instance-level lock |
| Matrix classes | **Not thread-safe** | Use `BitmapInfo` wrapper for concurrent access |

---

## Build

```bash
dotnet build Bwl.Imaging.sln                         # Debug build
dotnet build Bwl.Imaging.sln -c Release              # Release build
dotnet test Bwl.Imaging.UnitTests/Bwl.Imaging.UnitTests.vbproj
```

Versioning is handled automatically by `Directory.Build.props` using git commit date and hash.

## CI/CD

| Workflow | Trigger | Action |
|----------|---------|--------|
| `ci.yml` | Push to `master`/`netstandard`, PRs | Build + test |
| `publish.yml` | Push tag `v*` | Build + test + publish to NuGet.org |

To publish: create a tag like `v2.0.0` and push it. Requires `NUGET_API_KEY` secret in repository settings.

## License

[Apache License 2.0](LICENSE)
