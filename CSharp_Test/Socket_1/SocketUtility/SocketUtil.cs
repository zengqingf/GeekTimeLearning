using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketUtility
{
    public class SocketUtil
    {
        //.static静态存储区 申请一块空间
        static byte[] ms_buffer = new byte[1024 * 1024 * 2];

        public static void BeginReceive(Socket socket)
        {
            //方法参考：http://msdn.microsoft.com/zh-cn/library/system.net.sockets.socket.beginreceive.aspx
            socket.BeginReceive(ms_buffer, 0, ms_buffer.Length, SocketFlags.None, new AsyncCallback(receiveMsg), socket);
        }

        private static void receiveMsg(IAsyncResult result)
        {
            try
            {
                var socket = result.AsyncState as Socket;
                //结束挂起异步读取
                var length = socket.EndReceive(result);
                var remotePoint = socket.RemoteEndPoint.ToString();
                var mesage = Encoding.UTF8.GetString(ms_buffer, 0, length);
                //读取消息内容
                Console.WriteLine($"消息来源：{remotePoint}, 内容：{mesage}");

                //接收下一条消息
                socket.BeginReceive(ms_buffer, 0, ms_buffer.Length, SocketFlags.None, new AsyncCallback(receiveMsg), socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
