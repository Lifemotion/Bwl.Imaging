using System.Drawing;
using System.Drawing.Imaging;
// Copyright 2023 Artem Drobanov (artem.drobanov@gmail.com)

// Licensed under the Apache License, Version 2.0 (the "License");
// you may Not use this file except In compliance With the License.
// You may obtain a copy Of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law Or agreed To In writing, software
// distributed under the License Is distributed On an "AS IS" BASIS,
// WITHOUT WARRANTIES Or CONDITIONS Of ANY KIND, either express Or implied.
// See the License For the specific language governing permissions And
// limitations under the License.

using System.IO;
using System.Linq;

namespace Bwl.Imaging
{

    public static class JpegCodec
    {
        public static MemoryStream Encode(Bitmap bmp, int frameQuality = 60)
        {
            var jpegStream = new MemoryStream();
            if (bmp is not null)
            {
                lock (bmp)
                {
                    bmp.Save(jpegStream, GetCodecInfo(ImageFormat.Jpeg), GetEncoderParameters(frameQuality));
                    jpegStream.Seek(0L, SeekOrigin.Begin);
                    return jpegStream;
                }
            }
            return jpegStream;
        }

        public static Bitmap Decode(byte[] jpegBytes)
        {
            return Decode(new MemoryStream(jpegBytes));
        }

        public static Bitmap Decode(Stream jpegStream)
        {
            try
            {
                return new Bitmap(jpegStream);
            }
            catch
            {
                return null;
            }
        }

        private static ImageCodecInfo GetCodecInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        private static EncoderParameters GetEncoderParameters(int frameQuality)
        {
            var jpegEncoderParameters = new EncoderParameters(1);
            jpegEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, frameQuality);
            return jpegEncoderParameters;
        }
    }
}