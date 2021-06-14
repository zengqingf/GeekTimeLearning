using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thrift;
using Thrift.Protocol;
using Thrift.Server;
using Thrift.Transport;

using Common;

using System.Net.Sockets;

namespace ThriftServer
{
    class Program
    {
        static void Main(string[] args)
        {

            //TcpListener tcpListener = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), 7988);
            //TServerTransport transport = new TServerSocket(tcpListener);

            int port = 15789;
            TServerTransport transport = new TServerSocket(port);

            TestServer serverIfac = new TestServer();
            TProcessor processor = new ChatService.Processor(serverIfac);
            TServer server = new TThreadPoolServer(processor, transport);

            Task.Run(()=> {
                try
                {
                    Console.WriteLine("服务启动，端口：{0}", port);
                    server.Serve();
                }
                catch (Exception e)
                {
                    Console.WriteLine("服务启动异常：{0}", e.Message);
                }
            });

            Console.ReadKey();
        }
    }
}
