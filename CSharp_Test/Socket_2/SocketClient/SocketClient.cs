using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Socket_2
{
    class SocketClient
    {
        private string m_ip;
        private int m_port;
        private byte[] m_buffer;

        public SocketClient(string ip, int port)
        {
            m_ip = ip;
            m_port = port;
            m_buffer = new byte[1024];
        }


        public void ConnetServer(string clientName)
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(new IPEndPoint(IPAddress.Parse(m_ip), m_port));
            Console.WriteLine("Connected to server, enter $q to quit");

            clientName = "{<" + clientName.Trim() + ">}";
            byte[] nameBuf = Encoding.UTF8.GetBytes(clientName);
            client.BeginSend(nameBuf, 0, nameBuf.Length, SocketFlags.None, null, null);

            _beginReceive(client);

            _inputSendMsg(client);
        }    

        private void _beginReceive(Socket client)
        {
            client.BeginReceive(m_buffer, 0, m_buffer.Length, SocketFlags.None, new AsyncCallback(_receive), client);
        }

        private void _receive(IAsyncResult result)
        {
            try
            {
                Socket client = result.AsyncState as Socket;
                var length = client.EndReceive(result);
                string msg = Encoding.UTF8.GetString(m_buffer, 0, length);
                Console.WriteLine(msg);

                //继续接收下一条消息
                _beginReceive(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine("接收服务器消息异常");
            }
        }

        private void _inputSendMsg(Socket client)
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (input == "$q")
                {
                    client.Close();
                    break;
                }

                byte[] msg = Encoding.UTF8.GetBytes(input);
                client.BeginSend(msg, 0, msg.Length, SocketFlags.None, null, null);
            }

            Console.Write("Disconnected. Press any key to exit...");
            Console.ReadKey();
        }
    }
}
