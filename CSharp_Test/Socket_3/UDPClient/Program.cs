using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using UDPHelper;

namespace UDPClient
{
    class Program
    {
        static void Main(string[] args)
        {

            UDPSocket client = new UDPSocket();
            client.SetClient("127.0.0.1", 2345);
            client.Send("Test udp");

            Console.ReadKey();

#if Test1

            byte[] data = new byte[1024];
            string input = string.Empty;
            string strData = string.Empty;

            Console.WriteLine($"Client host name : {Dns.GetHostName()}");

            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2346);
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            string msg = "Hello !";
            data = Encoding.UTF8.GetBytes(msg);
            client.SendTo(data, data.Length, SocketFlags.None, ipep);
            IPEndPoint serverIpep = new IPEndPoint(IPAddress.Any, 0);
            EndPoint remote = serverIpep;

            //对于不存在的ip  可以在指定时间内解除阻塞模式限制
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 100);

            int recv = client.ReceiveFrom(data, ref remote);
            Console.WriteLine($"Message received from {remote}: ");
            Console.WriteLine(Encoding.UTF8.GetString(data, 0, recv));

            while (true)
            {
                input = Console.ReadLine();
                if (input == "$q")
                {
                    break;
                }
                client.SendTo(Encoding.UTF8.GetBytes(input), remote);
                data = new byte[1024];
                recv = client.ReceiveFrom(data, ref remote);
                strData = Encoding.UTF8.GetString(data, 0, recv);
                Console.WriteLine(strData);
            }
            Console.WriteLine("disconnected...");
            client.Close();

#endif
        }
    }
}
