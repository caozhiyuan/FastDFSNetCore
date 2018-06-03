using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FastDFS.Client
{
    public static class SocketEx
    {
        public static Task ConnectExAsync(this Socket socket, EndPoint remoteEp)
        {
            var tcs = new TaskCompletionSource<bool>(socket);
            socket.BeginConnect(remoteEp, iar =>
            {
                var innerTcs = (TaskCompletionSource<bool>)iar.AsyncState;
                try
                {
                    ((Socket)innerTcs.Task.AsyncState).EndConnect(iar);
                    innerTcs.TrySetResult(true);
                }
                catch (Exception e)
                {
                    innerTcs.TrySetException(e);
                }
            }, tcs);
            return tcs.Task;
        }

        public static async Task<int> ReceiveExAsync(this Socket socket, byte[] buffer)
        {
            int size = buffer.Length;
            int index = 0;
            while (index < size)
            {
                int read = await socket.ReceiveExAsync0(buffer);
                if (read == 0)
                {
                    throw new SocketException((int) SocketError.ConnectionReset);
                }
                index += read;
            }
            return size;
        }

        private static Task<int> ReceiveExAsync0(this Socket socket, byte[] buffer)
        {
            var tcs = new TaskCompletionSource<int>(socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, iar =>
            {
                var innerTcs = (TaskCompletionSource<int>)iar.AsyncState;
                try
                {
                    var rec = ((Socket)innerTcs.Task.AsyncState).EndReceive(iar);
                    innerTcs.TrySetResult(rec);
                }
                catch (Exception e)
                {
                    innerTcs.TrySetException(e);
                }
            }, tcs);
            return tcs.Task;
        }

        public static Task<int> SendExAsync(this Socket socket, byte[] buffer)
        {
            var tcs = new TaskCompletionSource<int>(socket);
            socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, iar =>
            {
                var innerTcs = (TaskCompletionSource<int>)iar.AsyncState;
                try
                {
                    var sent = ((Socket)innerTcs.Task.AsyncState).EndSend(iar);
                    innerTcs.TrySetResult(sent);
                }
                catch (Exception e)
                {
                    innerTcs.TrySetException(e);
                }
            }, tcs);
            return tcs.Task;
        }
    }
}
