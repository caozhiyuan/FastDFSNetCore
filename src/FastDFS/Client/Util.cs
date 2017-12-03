namespace FastDFS.Client
{
    internal static class Util
    {
        public static long BufferToLong(byte[] buffer, int offset)
        {
            return (long) buffer[offset] << 56
                   | ((long) buffer[offset + 1] << 48)
                   | ((long) buffer[offset + 2] << 40)
                   | ((long) buffer[offset + 3] << 32)
                   | ((long) buffer[offset + 4] << 24)
                   | ((long) buffer[offset + 5] << 16)
                   | ((long) buffer[offset + 6] << 8)
                   | (long) buffer[offset + 7];
        }

        public static string ByteToString(byte[] input)
        {
            return new string(FDFSConfig.Charset.GetChars(input), 0, input.Length);
        }

        public static byte[] LongToBuffer(long l)
        {
            return new byte[]
            {
                (byte) ((l >> 56) & 0xffL),
                (byte) ((l >> 48) & 0xffL),
                (byte) ((l >> 40) & 0xffL),
                (byte) ((l >> 32) & 0xffL),
                (byte) ((l >> 24) & 0xffL),
                (byte) ((l >> 16) & 0xffL),
                (byte) ((l >> 8) & 0xffL),
                (byte) (l & 0xffL)
            };
        }

        public static byte[] StringToByte(string input)
        {
            return FDFSConfig.Charset.GetBytes(input);
        }
    }
}