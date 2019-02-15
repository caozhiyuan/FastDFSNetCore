using System;
using System.Buffers;

namespace FastDFS.Client
{
    internal class FDFSHeader:IDisposable
	{
        private byte[] _headerBuffer;

		public byte Command { get; set; }

		public long Length { get; set; }

        public byte Status { get; set; }

        public FDFSHeader(long length, byte command, byte status)
		{
			this.Length = length;
			this.Command = command;
			this.Status = status;
        }

        public ArraySegment<byte> GetBuffer()
        {
            _headerBuffer = _headerBuffer ?? ArrayPool<byte>.Shared.Rent(10);
            Util.LongToBuffer(this.Length, _headerBuffer, 0);        
            _headerBuffer[8] = this.Command;
            _headerBuffer[9] = this.Status;
            return new ArraySegment<byte>(_headerBuffer, 0, 10);
        }

        public void Dispose()
        {
            if (_headerBuffer != null)
            {
                ArrayPool<byte>.Shared.Return(_headerBuffer);
            }
        }
    }
}