using SkiaSharp;

namespace Bwl.Imaging.Skia;

public class RawIntFrame : BlobContainer
{

    public int[] Data;
    public int Width { get; private set; }
    public int Height { get; private set; }

    public RawIntFrame(int width, int height, int[] data)
    {
        Width = width;
        Height = height;
        Data = data;
        Attributes.Add("Width", width.ToString());
        Attributes.Add("Height", height.ToString());
        Blobs.Add(new IntegerBlob() { ID = "Scan0", Data = data });
    }

    public RawIntFrame(int width, int height) : this(width, height, Array.Empty<int>())
    {
    }

    public RawIntFrame(BlobContainer bc) : base(bc)
    {
        Width = Convert.ToInt32(Attributes["Width"]);
        Height = Convert.ToInt32(Attributes["Height"]);
        Data = Blobs[0].Data;
    }

    public void SetDataToScan0Blob(int[] data)
    {
        var scan0Blob = Blobs.First(item => item.ID == "Scan0");
        scan0Blob.Data = data;
    }

    public void SetDataToScan0Blob()
    {
        var scan0Blob = Blobs.First(item => item.ID == "Scan0");
        scan0Blob.Data = Data;
    }

    public void GetDataFromScan0Blob()
    {
        var scan0Blob = Blobs.First(item => item.ID == "Scan0");
        Data = scan0Blob.Data;
    }

    public static RawIntFrame FromLegacyFile(string filename)
    {
        using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            using (var sw = new StreamReader(fs))
            {
                int width = Convert.ToInt32(sw.ReadLine());
                int height = Convert.ToInt32(sw.ReadLine());
                var arr = new int[width * height * 3 + 1];
                for (int i = 0, loopTo = arr.Length - 1; i <= loopTo; i++)
                    arr[i] = Convert.ToInt32(sw.ReadLine());
                fs.Close();
                var frame = new RawIntFrame(width, height, arr);
                frame.Data = arr;
                return frame;
            }
        }
    }

    public new static RawIntFrame FromFile(string filename)
    {
        var @file = new RawIntFrame(BlobContainer.FromFile(filename));
        return @file;
    }

    public void SaveToJpegPair(string filenameWithoutExtension, int quality = 95)
    {
        RGBMatrix[] mtrs = RawIntFrameConverters.ConvertTo8BitPair(this);
        var bmph = mtrs[0].ToSKBitmap();
        var bmpl = mtrs[1].ToSKBitmap();
        string fnameh = filenameWithoutExtension + "_h.jpeg";
        string fnamel = filenameWithoutExtension + "_l.jpeg";
        SaveBitmapAsJpeg(bmph, fnameh, quality);
        SaveBitmapAsJpeg(bmpl, fnamel, quality);
    }

    private static void SaveBitmapAsJpeg(SKBitmap bitmap, string filename, int quality)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, quality);
        using var stream = File.OpenWrite(filename);
        data.SaveTo(stream);
    }

    public static RawIntFrame FromJpegPair(string filenameWithoutExtension)
    {
        string fnameh = filenameWithoutExtension + "_h.jpeg";
        string fnamel = filenameWithoutExtension + "_l.jpeg";
        var bmph = SKBitmap.Decode(fnameh);
        var bmpl = SKBitmap.Decode(fnamel);
        RGBMatrix[] mtrs = new[] { bmph.ToRgbMatrix(), bmpl.ToRgbMatrix() };
        var frame = RawIntFrameConverters.ConvertFrom8BitPair(mtrs);
        return frame;
    }
}