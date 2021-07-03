using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using UDPHelper;

namespace UDPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            UDPSocket server = new UDPSocket();
            server.SetServer("127.0.0.1", 2345);

            Console.ReadKey();

#if Test1

            int recv;
            byte[] data = new byte[1024];

            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 2345);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);  //udp 用户数据报
            //绑定网络地址
            server.Bind(ipep);
            Console.WriteLine($"Server host name : {Dns.GetHostName()}");
            Console.WriteLine("Waiting for client ...");

            //接收客户端连接
            IPEndPoint clientIpep = new IPEndPoint(IPAddress.Any, 0);
            EndPoint remote = clientIpep;
            recv = server.ReceiveFrom(data, ref remote);
            Console.WriteLine($"Message received from {remote}: ");
            Console.WriteLine(Encoding.UTF8.GetString(data, 0, recv));

            //发送欢迎消息
            string welcomeMsg = "Welcome !";
            data = Encoding.UTF8.GetBytes(welcomeMsg);
            server.SendTo(data, data.Length, SocketFlags.None, remote);

            while (true)
            {
                //发送接收消息
                data = new byte[1024];
                recv = server.ReceiveFrom(data, ref remote);
                Console.WriteLine(Encoding.UTF8.GetString(data, 0, recv));
                //server.SendTo(data, recv, SocketFlags.None, remote);
            }

#endif
        }
    }
}
