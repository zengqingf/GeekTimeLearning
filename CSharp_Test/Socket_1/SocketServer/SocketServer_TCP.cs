using SocketUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 link:
     https://www.cnblogs.com/chenxizhang/archive/2011/09/10/2172994.html

     https://www.cnblogs.com/dolphinx/p/3460545.html
     */

namespace SocketServer
{
    class SocketServer_TCP
    {
        //private string m_ip = string.Empty;
        private int m_port = 0;
        private Socket m_socket = null;

        //public SocketServer_TCP(string ip, int port)
        //{
        //    this.m_ip = ip;
        //    this.m_port = port;
        //}

        public SocketServer_TCP(int port)
        {
            //this.m_ip = "127.0.0.1";
            this.m_port = port;
            _init();
        }

        private void _init()
        {
            //实例化套接字 （寻址协议：ipv4，传输方式：流，协议：TCP）
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //创建IP地址对象
            //IPAddress address = IPAddress.Parse(m_ip);
            IPAddress address = IPAddress.Any;

            //创建网络端口（ip + port）
            IPEndPoint endPoint = new IPEndPoint(address, m_port);
            //绑定套接字到端口
            m_socket.Bind(endPoint);

            Console.WriteLine("建立监听成功，{0}", m_socket.LocalEndPoint.ToString());     

            //设置最大连接数
            m_socket.Listen(int.MaxValue);

            //开启新线程，监听客户端的连接
            //Thread thread = new Thread(_listenClientConnect);
            //thread.IsBackground = true;
            //thread.Start();

            /*另一种监听客户端请求的方法*/
            _beginListen();

            Console.WriteLine("正在监听客户端连接...");
        }

        private void _beginListen()
        {
            //方法参考：http://msdn.microsoft.com/zh-cn/library/system.net.sockets.socket.beginaccept.aspx
            m_socket.BeginAccept(new AsyncCallback(_acceptClient), m_socket);
        }


        private void _acceptClient(IAsyncResult result)
        {
            try
            {
                var socket = result.AsyncState as Socket;
                var connectSocket = socket.EndAccept(result);
                var remotePoint = connectSocket.RemoteEndPoint.ToString();
                Console.WriteLine($"Client：{remotePoint}, connected");
                connectSocket.Send(Encoding.UTF8.GetBytes($"### Hi there, I accept you request at {DateTime.Now}"));

                //定时发送消息给客户端
                _sendMsgTiming(connectSocket);

                //接收客户端消息
                _receiveMsg(connectSocket);

                //继续接收下一个客户端请求
                _beginListen();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void _sendMsgTiming(Socket connectSocket)
        {
            if (null == connectSocket)
                return;
            var timer = new System.Timers.Timer();
            timer.Interval = 2000d;
            timer.Enabled = true;
            timer.Elapsed += (o, a) =>
            {
                if (connectSocket.Connected)
                {
                    try
                    {
                       // connectSocket.Send(Encoding.UTF8.GetBytes($"### Message from server at {DateTime.Now}"));
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    timer.Stop();
                    timer.Enabled = false;
                    Console.WriteLine("Client is disconnected, the timer is stop");
                }
            };
            timer.Start();
        }

        private void _receiveMsg(Socket connectSocket)
        {
            if (null == connectSocket)
                return;
            SocketUtil.BeginReceive(connectSocket);
        }


        /*-----------------------测试--------------------------*/

        private void _listenClientConnect()
        {
            while (true)
            {
                //等待连接并创建一个负责通信的socket
                var sendSocket = m_socket.Accept();
                var remotePoint = sendSocket.RemoteEndPoint.ToString();
                Console.WriteLine($"已连接客户端：{remotePoint}");
                sendSocket.Send(Encoding.UTF8.GetBytes($"### {remotePoint} 连接服务器成功 ###\n"));

                Thread thread = new Thread(_receive);
                thread.Start(sendSocket);
            }
        }

        private void _receive(object o)
        {
            var sendSocket = o as Socket;
            while (true)
            {
                try
                {
                    //接收客户端发送的消息容器
                    byte[] buffer = new byte[1024 * 1024 * 2];
                    int length = sendSocket.Receive(buffer);

                    //消息有效长度为0则跳过
                    if (length <= 0)
                    {
                        break;
                    }

                    Console.WriteLine("消息来源：{0}, 内容：{1}", sendSocket.RemoteEndPoint.ToString(), Encoding.UTF8.GetString(buffer, 0, length));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    sendSocket.Shutdown(SocketShutdown.Both);
                    sendSocket.Close();
                    break;
                }
            }
        }
    }
}
