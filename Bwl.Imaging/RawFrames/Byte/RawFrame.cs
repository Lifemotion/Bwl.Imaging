using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bwl.Imaging
{

    /// <summary>
/// Универсальный формат сырого кадра.
/// </summary>
    public class RawFrame : ICloneable
    {

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Channels { get; private set; }
        public byte[] PixelData { get; private set; }

        public RawFrame()
        {
        }

        public RawFrame(byte[] serialized, bool headerFirst = false, bool loadPixelData = true)
        {
            Deserialize(serialized, headerFirst, loadPixelData);
        }

        public RawFrame(Stream serialized, bool headerFirst = false, bool loadPixelData = true)
        {
            Deserialize(serialized, headerFirst, loadPixelData);
        }

        public RawFrame(int width, int height, int channels, byte[] pixelData, bool clone = true)
        {
            Import(width, height, channels, pixelData, clone);
        }

        public RawFrame(byte[,,] pixelData)
        {
            Import(pixelData);
        }

        public override bool Equals(object obj)
        {
            if (obj is null || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                RawFrame rawFrame = (RawFrame)obj;
                return rawFrame.Width == Width && rawFrame.Height == Height && rawFrame.Channels == Channels && rawFrame.PixelData.SequenceEqual(PixelData);
            }
        }

        public void Import(int width, int height, int channels, byte[] pixelDataSrc, bool clone = true)
        {
            if (width * height * channels > pixelDataSrc.Length)
            {
                throw new Exception($"{GetType().Name}.Import(): width * height * channels > pixelDataSrc.Length");
            }
            byte[] pixelData = null;
            if (clone)
            {
                pixelData = new byte[(width * height * channels)];
                Array.Copy(pixelDataSrc, pixelData, pixelData.Length);
            }
            else
            {
                pixelData = pixelDataSrc;
            }
            Width = width;
            Height = height;
            Channels = channels;
            PixelData = pixelData;
        }

        public void Import(byte[,,] pixelDataSrc)
        {
            int width = pixelDataSrc.GetLength(1);
            int height = pixelDataSrc.GetLength(0);
            int channels = pixelDataSrc.GetLength(2);
            byte[] pixelData = new byte[(width * height * channels)];
            if (channels == 1)
            {
                ImportChannel(pixelDataSrc, pixelData, 0);
            }
            else
            {
                Parallel.For(0, channels, (channel) => ImportChannel(pixelDataSrc, pixelData, channel));
            }
            Width = width;
            Height = height;
            Channels = channels;
            PixelData = pixelData;
        }

        public byte[,,] Export()
        {
            byte[,,] pixelDataTgt = new byte[Height, Width, Channels];
            if (Channels == 1)
            {
                ExportChannel(PixelData, pixelDataTgt, 0);
            }
            else
            {
                Parallel.For(0, Channels, (channel) => ExportChannel(PixelData, pixelDataTgt, channel));
            }
            return pixelDataTgt;
        }

        public byte[] Serialize(bool headerFirst = false)
        {
            int pixelDataLength = PixelData.Length;
            int headerOffset = headerFirst ? 0 : pixelDataLength;
            int dataOffset = headerFirst ? 5 : 0;
            byte[] serialized = new byte[(pixelDataLength + 5)];
            serialized[0 + headerOffset] = (byte)(Channels & 0xFF);
            serialized[1 + headerOffset] = (byte)(Width >> 0 & 0xFF);
            serialized[2 + headerOffset] = (byte)(Width >> 8 & 0xFF);
            serialized[3 + headerOffset] = (byte)(Height >> 0 & 0xFF);
            serialized[4 + headerOffset] = (byte)(Height >> 8 & 0xFF);
            Array.Copy(PixelData, 0, serialized, dataOffset, pixelDataLength);
            return serialized;
        }

        public void Deserialize(byte[] serialized, bool headerFirst = false, bool loadPixelData = true)
        {
            using (var serializedStream = new MemoryStream(serialized))
            {
                Deserialize(serializedStream, headerFirst, loadPixelData);
            }
        }

        public void Deserialize(Stream serialized, bool headerFirst = false, bool loadPixelData = true)
        {
            long pixelDataLength = serialized.Length - 5L;
            long headerOffset = headerFirst ? 0L : pixelDataLength;
            int dataOffset = headerFirst ? 5 : 0;
            serialized.Seek(headerOffset, SeekOrigin.Begin);
            int channels = serialized.ReadByte();
            int width = serialized.ReadByte() | serialized.ReadByte() << 8;
            int height = serialized.ReadByte() | serialized.ReadByte() << 8;
            if (width * height * channels != pixelDataLength)
            {
                throw new Exception($"{GetType().Name}.Deserialize(): width * height * channels <> pixelDataLength");
            }
            Width = width;
            Height = height;
            Channels = channels;
            if (loadPixelData)
            {
                byte[] pixelData = new byte[(int)(pixelDataLength - 1L + 1)];
                serialized.Seek(dataOffset, SeekOrigin.Begin);
                int read = 0;
                while (read < pixelDataLength)
                    read += serialized.Read(pixelData, read, (int)(pixelDataLength - read));
                PixelData = pixelData;
            }
            else
            {
                PixelData = null;
            }
        }

        public RawFrame Copy()
        {
            return new RawFrame(Width, Height, Channels, PixelData, true);
        }

        public object Clone()
        {
            return Copy();
        }

        private void ImportChannel(byte[,,] src, byte[] tgt, int channel)
        {
            int width = src.GetLength(1);
            int height = src.GetLength(0);
            int channels = src.GetLength(2);
            int offset = channel;
            for (int y = 0, loopTo = height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = width - 1; x <= loopTo1; x++)
                {
                    tgt[offset] = src[y, x, channel];
                    offset += channels;
                }
            }
        }

        private void ExportChannel(byte[] src, byte[,,] tgt, int channel)
        {
            int width = tgt.GetLength(1);
            int height = tgt.GetLength(0);
            int channels = tgt.GetLength(2);
            int offset = channel;
            for (int y = 0, loopTo = height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = width - 1; x <= loopTo1; x++)
                {
                    tgt[y, x, channel] = src[offset];
                    offset += channels;
                }
            }
        }
    }
}