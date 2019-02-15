using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

        public static void LongToBuffer(long l, byte[] buffer, int offset)
        {
            buffer[offset] = (byte) ((l >> 56) & 0xffL);
            buffer[offset + 1] = (byte) ((l >> 48) & 0xffL);
            buffer[offset + 2] = (byte) ((l >> 40) & 0xffL);
            buffer[offset + 3] = (byte) ((l >> 32) & 0xffL);
            buffer[offset + 4] = (byte) ((l >> 24) & 0xffL);
            buffer[offset + 5] = (byte) ((l >> 16) & 0xffL);
            buffer[offset + 6] = (byte) ((l >> 8) & 0xffL);
            buffer[offset + 7] = (byte) (l & 0xffL);
        }

        public static ReadOnlySpan<byte> StringToByte(string input)
        {
            return FDFSConfig.Charset.GetBytes(input);
        }
    }
}