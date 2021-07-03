using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SocketServer
{
    class TcpServer
    {
        private Dictionary<IntPtr, ClientInfo> m_clientPool = null;        
        private List<SocketMessage> m_msgPool = null;

        public TcpServer()
        {
            m_clientPool = new Dictionary<IntPtr, ClientInfo>();
            m_msgPool = new List<SocketMessage>();
        }

        public void Run(int port)
        {
            Thread serverSocketThread = new Thread(()=> {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(IPAddress.Any, port));
                socket.Listen(10);

                _beginAccept(socket);
            });

            serverSocketThread.Start();
            Console.WriteLine("Server is ready...");

            _broadcast();
        }

        private void _beginAccept(Socket socket)
        {
            socket.BeginAccept(new AsyncCallback(_accept), socket);
        }

        private void _accept(IAsyncResult result)
        {
            try
            {
                Socket server = result.AsyncState as Socket;
                Socket connectedSocket = server.EndAccept(result);

                //创建一个新的socket 处理接收消息
                _beginReceive(connectedSocket);                
                Console.WriteLine($"Client {connectedSocket.RemoteEndPoint} connected");

                //server处理下一个client连接
                _beginAccept(server);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server accept failed, error: {ex}");
            }
        }

        private void _beginReceive(Socket socket)
        {
            byte[] buffer = new byte[1024];
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(_receive), socket);
            _createClientInfo(socket, buffer);
        }

        private void _createClientInfo(Socket socket, byte[] buffer)
        {
            ClientInfo clientInfo = null;
            if (!m_clientPool.ContainsKey(socket.Handle))
            {
                clientInfo = new ClientInfo();
                clientInfo.socket = socket;
                clientInfo.buffer = buffer;
                m_clientPool.Add(clientInfo.Handle, clientInfo);
            }
            else
            {
                clientInfo = m_clientPool[socket.Handle];
                clientInfo.buffer = buffer;
            }
        }

        private void _receive(IAsyncResult result)
        {
            Socket client = result.AsyncState as Socket;
            if (client == null || !m_clientPool.ContainsKey(client.Handle)) {
                return;
            }
            ClientInfo clientInfo = m_clientPool[client.Handle];
            try
            {
                int msglength = client.EndReceive(result);
                _createSocketMsg(clientInfo, msglength);

                //等待接收下一条消息                
                _beginReceive(client);
            }
            catch (Exception ex)
            {
                client.Disconnect(true);
                Console.WriteLine("Client {0} disconnect", clientInfo.Name);
                m_clientPool.Remove(clientInfo.Handle);
            }
        }

        private void _createSocketMsg(ClientInfo client, int msgLength)
        {
            byte[] buffer = client.buffer;
            string msg = Encoding.UTF8.GetString(buffer, 0, msgLength);
            SocketMessage sm = new SocketMessage();
            sm.client = client;
            sm.time = DateTime.Now;

            Regex regex = new Regex(@"{<(.*?)>}");
            Match match = regex.Match(msg);

            if (match.Value != "")
            {
                m_clientPool[client.socket.Handle].nickName = Regex.Replace(match.Value, @"{<(.*?)>}", "$1");
                sm.isLogin = true;
                sm.message = "login...";
                Console.WriteLine($"{client.ID} login @ {sm.time}");
            }
            else
            {
                sm.isLogin = false;
                sm.message = msg;
                Console.WriteLine($"{client.ID} @ {sm.time} \r\n    {msg}");
            }

            m_msgPool.Add(sm);
        }


        private void _broadcast()
        {
            Thread broadcast = new Thread(() =>
            {
                while (true)
                {
                    if (m_msgPool.Count > 0)
                    {
                        byte[] msg = _packageMsg(m_msgPool[0]);
                        foreach (var client in m_clientPool)
                        {
                            var socket = client.Value.socket;
                            if (socket.Connected)
                            {
                                socket.Send(msg, msg.Length, SocketFlags.None);
                            }
                        }
                        m_msgPool.RemoveAt(0);
                    }
                }
            });

            broadcast.Start();
        }

        private byte[] _packageMsg(SocketMessage sm)
        {
            if (sm == null)
            {
                return new byte[0];
            }
            StringBuilder pkgMsg = new StringBuilder();
            if (sm.isLogin)
            {
                pkgMsg.AppendFormat("{0} {1} @ {2}", sm.client.Name, sm.message, sm.time.ToShortTimeString());
            }
            else
            {
                pkgMsg.AppendFormat("{0} @ {1}:\r\n    ", sm.client.Name, sm.time.ToShortTimeString());
                pkgMsg.Append(sm.message);
            }

            return Encoding.UTF8.GetBytes(pkgMsg.ToString());
        }
    }

    class ClientInfo
    {
        public byte[] buffer;
        public string nickName;
        public Socket socket;
        public EndPoint ID
        {
            get {
                if (socket != null)
                {
                    return socket.RemoteEndPoint;
                }
                return null;
            }
        }

        public IntPtr Handle
        {
            get {
                if (socket != null)
                {
                    return socket.Handle;
                }
                return IntPtr.Zero;
            }
        }

        public string Name
        {
            get {
                if (!string.IsNullOrEmpty(nickName))
                {
                    return nickName;
                }
                else
                {
                    return string.Format("{0}#{1}", ID, Handle);
                }
            }
        }
    }

    class SocketMessage
    {
        public bool isLogin;
        public ClientInfo client;
        public string message;
        public DateTime time;
    }
}
