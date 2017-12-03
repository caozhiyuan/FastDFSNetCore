using System;

namespace FastDFS.Client
{
    internal class FDFSHeader
	{
		private long _length;

		private byte _command;

		private byte _status;

		public byte Command
		{
			get
			{
				return this._command;
			}
			set
			{
				this._command = value;
			}
		}

		public long Length
		{
			get
			{
				return this._length;
			}
			set
			{
				this._length = value;
			}
		}

		public byte Status
		{
			get
			{
				return this._status;
			}
			set
			{
				this._status = value;
			}
		}

		public FDFSHeader(long length, byte command, byte status)
		{
			this._length = length;
			this._command = command;
			this._status = status;
		}

		public byte[] ToByte()
		{
			byte[] numArray = new byte[10];
			byte[] buffer = Util.LongToBuffer(this._length);
			Array.Copy(buffer, 0, numArray, 0, (int)buffer.Length);
			numArray[8] = this._command;
			numArray[9] = this._status;
			return numArray;
		}
	}
}