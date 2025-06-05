using System;
using System.Text;

namespace EncoderTool
{
    public static class Decoder
    {
        private const int IntKey = unchecked((int)0x55AA55AA);
        private const int ShortKey = unchecked((int)0x55AA);
        private const long LongKey = 0x0123456789ABCDEF;

        public static string DecodeString(string base64)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        }

        public static int DecodeInt(int value)
        {
            return value ^ IntKey;
        }

        public static short DecodeShort(int value)
        {
            return (short)(value ^ ShortKey);
        }

        public static long DecodeLong(long value)
        {
            return value ^ LongKey;
        }
    }
}
