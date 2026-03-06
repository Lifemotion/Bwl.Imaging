using System.Drawing;
using System.Drawing.Imaging;

namespace Bwl.Imaging
{

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
            using (var fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                using (var sw = new System.IO.StreamReader(fs))
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

        private ImageCodecInfo GetCodecInfo(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public void SaveToJpegPair(string filenameWithoutExthension, int quality = 95)
        {
            var col = HSV.FromRgb(1, 1, 1);
            var _encoderParameters = new EncoderParameters(1);
            var _codecInfo = GetCodecInfo(ImageFormat.Jpeg);
            _encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            RGBMatrix[] mtrs = RawIntFrameConverters.ConvertTo8BitPair(this);
            var bmph = mtrs[0].ToBitmap();
            var bmpl = mtrs[1].ToBitmap();
            string fnameh = filenameWithoutExthension + "_h.jpeg";
            string fnamel = filenameWithoutExthension + "_l.jpeg";
            bmph.Save(fnameh, _codecInfo, _encoderParameters);
            bmpl.Save(fnamel, _codecInfo, _encoderParameters);
        }

        public static RawIntFrame FromJpegPair(string filenameWithoutExthension)
        {
            string fnameh = filenameWithoutExthension + "_h.jpeg";
            string fnamel = filenameWithoutExthension + "_l.jpeg";
            var bmph = new Bitmap(fnameh);
            var bmpl = new Bitmap(fnamel);
            RGBMatrix[] mtrs = new[] { bmph.BitmapToRgbMatrix(), bmpl.BitmapToRgbMatrix() };
            var frame = RawIntFrameConverters.ConvertFrom8BitPair(mtrs);
            return frame;
        }
    }
}