using System;
using System.Collections.Generic;
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

        public static async Task<int> ReceiveExAsync(this Socket socket, byte[] buffer, int length)
        {
            int index = 0;
            while (index < length)
            {
                int read = await socket.ReceiveExAsync0(buffer, index, length - index);
                if (read == 0)
                {
                    throw new SocketException((int) SocketError.ConnectionReset);
                }
                index += read;
            }
            return length;
        }

        private static Task<int> ReceiveExAsync0(this Socket socket, byte[] buffer,int index, int size)
        {
            var tcs = new TaskCompletionSource<int>(socket);
            socket.BeginReceive(buffer, index, size, SocketFlags.None, iar =>
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

        public static Task<int> SendExAsync(this Socket socket, IList<ArraySegment<byte>> buffers)
        {
            var tcs = new TaskCompletionSource<int>(socket);
            socket.BeginSend(buffers, SocketFlags.None, iar =>
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
