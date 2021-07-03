using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socket_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();

            SocketClient client = new SocketClient("127.0.0.1", 2345);
            client.ConnetServer(name);
        }
    }
}
