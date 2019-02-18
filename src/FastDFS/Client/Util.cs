using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FastDFS.Client
{
    internal static class Util
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        
        public static unsafe string ByteToString(byte[] input, int index, int count)
        {
            Span<char> span = FDFSConfig.Charset.GetChars(input, index, count);
            fixed (char* chars = &TrimEnd(span).GetPinnableReference())
            {
                return new string(chars);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Span<char> TrimEnd(Span<char> span)
        {
            int index = span.Length - 1;
            while (index >= 0 && span[index] == '\0')
                --index;
            return span.Slice(0, index + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int StringByteCount(string input)
        {
            return FDFSConfig.Charset.GetByteCount(input);
        }
        
        public static unsafe void StringToByte(string input, byte[] buffer, int offset, int? inputByteCount = null)
        {
            if (inputByteCount == null)
            {
                inputByteCount = StringByteCount(input);
                if (inputByteCount == 0)
                {
                    return;
                }
            }

            var bytes = new Span<byte>(buffer, offset, inputByteCount.Value);
            var chars = input.AsSpan();
            fixed (char* chars1 = &MemoryMarshal.GetReference(chars))
            {
                fixed (byte* bytes1 = &MemoryMarshal.GetReference(bytes))
                {
                    FDFSConfig.Charset.GetBytes(chars1, chars.Length, bytes1, bytes.Length);
                }
            }
        }
    }
}