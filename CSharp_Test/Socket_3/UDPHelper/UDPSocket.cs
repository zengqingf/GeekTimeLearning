using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDPHelper
{
    public class UDPSocket
    {
        private Socket m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int kBufSize = 8 * 1024;
        private State m_state = new State();
        private EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;

        public class State
        {
            public byte[] buffer = new byte[kBufSize];
        }

        public void SetServer(string address, int port)
        {
            m_socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            m_socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));

            _receive();
        }

        public void SetClient(string address, int port)
        {
            m_socket.Connect(IPAddress.Parse(address), port);

            _receive();
        }

        public void Send(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            m_socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State st = (State)ar.AsyncState;
                int msgLength = m_socket.EndSend(ar);
                Console.WriteLine($"### Send, msg length : {msgLength}, msg : {text}");
            }, m_state);
        }

        private void _receive()
        {
            m_socket.BeginReceiveFrom(m_state.buffer, 0, kBufSize, SocketFlags.None, ref endPoint, recv = (ar) =>
            {
                State st = (State)ar.AsyncState;
                int msgLength = m_socket.EndReceiveFrom(ar, ref endPoint);
                m_socket.BeginReceiveFrom(st.buffer, 0, kBufSize, SocketFlags.None, ref endPoint, recv, st);
                Console.WriteLine("### Recv, {0}: {1}, {2}", endPoint.ToString(), msgLength, Encoding.UTF8.GetString(st.buffer, 0, msgLength));
            }, m_state);
        }
    }
}
