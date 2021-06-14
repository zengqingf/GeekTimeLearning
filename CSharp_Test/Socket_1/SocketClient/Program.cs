using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("客户端：{0}", Dns.GetHostEntry(Dns.GetHostName()).AddressList[3]);
            
            SocketClient_TCP sc = new SocketClient_TCP("192.168.4.92", 2345);
            sc.Init();

            //for (int i = 0; i < 100; i++)
            //{
            //    sc.SendMsg($"发送第{i}条消息\n");                
            //}

            Console.ReadKey();
        }
    }
}
