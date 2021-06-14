using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer
{
    class TcpServer
    {
        private Dictionary<IntPtr, ClientInfo> m_clientInfoPool = null;
        private List<SocketMessage> m_msgPool = null;

        public TcpServer()
        {
            m_clientInfoPool = new Dictionary<IntPtr, ClientInfo>();
            m_msgPool = new List<SocketMessage>();
        }

        public void Run(int port)
        {
            Thread serverSocketThread = new Thread(()=> {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(IPAddress.Any, port));
                socket.Listen(10);

                _beginListenNew(socket);
            });

            serverSocketThread.Start();
            Console.WriteLine("Server is ready...");

        }

        private void _beginListenNew(Socket socket)
        {
            socket.BeginAccept(new AsyncCallback(_accept), socket);
        }

        private void _accept(IAsyncResult result)
        {
            try
            {
                Socket server = result.AsyncState as Socket;
                Socket connectSocket = server.EndAccept(result);

                _beginListenNew(server);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server accept failed, error: {ex}");
            }
        }
    }

    class ClientInfo
    {
        private byte[] m_buffer;
        public string nickName;
        public EndPoint id;
        public IntPtr handle;
        public string Name
        {
            get {
                if (!string.IsNullOrEmpty(nickName))
                {
                    return nickName;
                }
                else
                {
                    return string.Format("{0}#{1}", id, handle);
                }
            }
        }
    }

    class SocketMessage
    {
        public bool isLogin;
        public ClientInfo clientInfo;
        public string message;
        public DateTime time;
    }
}
