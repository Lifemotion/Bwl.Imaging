using System;

namespace Bwl.Imaging
{
    public static class SimpleLSB
    {
        private readonly static byte[] _bitMask = new[] { (byte)1, (byte)2, (byte)4, (byte)8, (byte)16, (byte)32, (byte)64, (byte)128 };

        public static int GetMaxPayloadSize(int hostSize)
        {
            return hostSize / 8 - 4;
        }

        public static void Append(byte[] host, int hostOffset, byte[] payload)
        {
            CheckMaxPayloadSize(host.Length - hostOffset, payload.Length);
            byte[] payloadSizeBytes = BitConverter.GetBytes(payload.Length);
            SetBits(host, hostOffset, payloadSizeBytes);
            SetBits(host, hostOffset + payloadSizeBytes.Length * 8, payload);
        }

        public static byte[] Extract(byte[] host, int hostOffset)
        {
            byte[] payloadSizeBytes = GetBits(host, hostOffset, 4);
            int payloadSize = BitConverter.ToInt32(payloadSizeBytes, 0);
            CheckMaxPayloadSize(host.Length - hostOffset, payloadSize);
            return GetBits(host, hostOffset + payloadSizeBytes.Length * 8, payloadSize);
        }

        private static byte[] GetBits(byte[] host, int hostOffset, int payloadCount)
        {
            int lsbMask = 0x1;
            byte[] payload = new byte[payloadCount];
            int payloadIdx = 0;
            int offset = hostOffset;
            for (int i = 0, loopTo = payloadCount - 1; i <= loopTo; i++)
            {
                int bt = 0;
                bt = bt | (host[offset + 0] & lsbMask) << 0;
                bt = bt | (host[offset + 1] & lsbMask) << 1;
                bt = bt | (host[offset + 2] & lsbMask) << 2;
                bt = bt | (host[offset + 3] & lsbMask) << 3;
                bt = bt | (host[offset + 4] & lsbMask) << 4;
                bt = bt | (host[offset + 5] & lsbMask) << 5;
                bt = bt | (host[offset + 6] & lsbMask) << 6;
                bt = bt | (host[offset + 7] & lsbMask) << 7;
                offset += 8;
                payload[payloadIdx] = (byte)bt;
                payloadIdx += 1;
            }
            return payload;
        }

        private static void SetBits(byte[] host, int hostOffset, byte[] payload)
        {
            byte topBitsMask = 0xFE;
            int offset = hostOffset;
            for (int i = 0, loopTo = payload.Length - 1; i <= loopTo; i++)
            {
                byte bt = payload[i];
                host[offset + 0] = (byte)((host[offset + 0] & topBitsMask) | ((bt & _bitMask[0]) >> 0));
                host[offset + 1] = (byte)((host[offset + 1] & topBitsMask) | ((bt & _bitMask[1]) >> 1));
                host[offset + 2] = (byte)((host[offset + 2] & topBitsMask) | ((bt & _bitMask[2]) >> 2));
                host[offset + 3] = (byte)((host[offset + 3] & topBitsMask) | ((bt & _bitMask[3]) >> 3));
                host[offset + 4] = (byte)((host[offset + 4] & topBitsMask) | ((bt & _bitMask[4]) >> 4));
                host[offset + 5] = (byte)((host[offset + 5] & topBitsMask) | ((bt & _bitMask[5]) >> 5));
                host[offset + 6] = (byte)((host[offset + 6] & topBitsMask) | ((bt & _bitMask[6]) >> 6));
                host[offset + 7] = (byte)((host[offset + 7] & topBitsMask) | ((bt & _bitMask[7]) >> 7));
                offset += 8;
            }
        }

        private static void CheckMaxPayloadSize(int hostSize, int payloadSize)
        {
            int maxPayloadSize = GetMaxPayloadSize(hostSize);
            if (payloadSize > maxPayloadSize)
            {
                throw new Exception($"SimpleLSB.CheckMaxPayloadSize(): payloadSize({payloadSize}) > maxPayloadSize({maxPayloadSize})");
            }
        }
    }
}