using System;

namespace FastDFS.Client
{
	public class FDFSException : Exception
	{
		public FDFSException(string msg) : base(msg)
		{
		}
	}
}