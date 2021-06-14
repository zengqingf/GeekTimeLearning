using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SocketUtility;

namespace SocketClient
{
    class SocketClient_TCP
    {
        private string m_ip = string.Empty;
        private int m_port = 0;
        private Socket m_socket = null;

        public SocketClient_TCP(string ip, int port)
        {
            this.m_ip = ip;
            this.m_port = port;
        }

        public SocketClient_TCP(int port)
        {
            this.m_ip = "127.0.0.1";
            this.m_port = port;
        }

        public void Init()
        {
            //实例化套接字 （寻址协议：ipv4，传输方式：流，协议：TCP）
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //服务器IP + Port
            //创建IP地址对象
            IPAddress address = IPAddress.Parse(m_ip);
            //创建网络端口（ip + port）
            IPEndPoint endPoint = new IPEndPoint(address, m_port);

            //建立连接
            m_socket.Connect(endPoint);
            //Console.WriteLine("连接服务器成功");

            //在客户端关闭前，一直接收服务器发送消息
            //Thread thread = new Thread(_receive);
            //thread.IsBackground = true;
            //thread.Start();

            //另一种监听服务器消息的方法
            SocketUtil.BeginReceive(m_socket);

            _waitUserInput();
        }

        private void _waitUserInput()
        {
            while (true)
            {
                var message = $"Message from client {m_socket.LocalEndPoint} : {Console.ReadLine()}";
                var buffer = Encoding.UTF8.GetBytes(message);
                m_socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, null);
            }
        }

        /*-----------------------测试--------------------------*/

        private void _receive()
        {
            //客户端socket不需要这么操作！
            //while (true) { //do sth }

            //接收服务器消息
            byte[] buffer = new byte[1024 * 1024 * 2];
            var length = m_socket.Receive(buffer);
            if (length <= 0)
                return;
            Console.WriteLine("消息来源：{0}, 内容：{1}", m_socket.RemoteEndPoint.ToString(), Encoding.UTF8.GetString(buffer, 0, length));
        }

        public void SendMsg(string msg)
        {
            string sendMsg = string.Format("### {0}，当前客户端时间：{1} ###\n", msg, DateTime.Now.ToString());
            m_socket.Send(Encoding.UTF8.GetBytes(sendMsg));
            Console.WriteLine("向服务器发送消息：{0}", sendMsg);
            Thread.Sleep(1000);
        }
    }
}