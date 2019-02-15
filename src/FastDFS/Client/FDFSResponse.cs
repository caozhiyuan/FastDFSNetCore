namespace FastDFS.Client
{
    internal interface IFDFSResponse
    {
        void ParseBuffer(byte[] responseBytes);
    }

    internal class EmptyFDFSResponse: IFDFSResponse
    {
        public void ParseBuffer(byte[] responseBytes)
        {
          
        }
    }
}
