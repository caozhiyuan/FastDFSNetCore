using System;

namespace FastDFS.Client
{
	public class FDFSException : Exception
	{
        public FDFSException(string msg) : base(msg)
        {
           
        }
    }

    public class FDFSStatusException : Exception
    {
        public int Status { get; set; }

        public FDFSStatusException(int status, string msg) : base(msg)
        {
            Status = status;
        }
    }
}