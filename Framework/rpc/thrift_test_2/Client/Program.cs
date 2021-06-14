using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Thrift.Transport;
using Thrift.Protocol;
using Common;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var transport = new TSocket("localhost", 15789);
            var protocol = new TBinaryProtocol(transport);

            var client = new ChatService.Client(protocol);
            transport.Open();


            //test 1
            Task.Run(() =>
            {
                while (true)
                {
                    Random random = new Random();
                    int ronum = random.Next(10000);
                    string words = ronum + "说" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string backMsg = ClientTask.GetServerData(words, client);

                    Console.WriteLine(backMsg);
                    Task.Delay(1000).Wait();
                }
            });

            
            //test 2
            //List<string> data = new List<string>();
            //data.Add("吃饭");
            //data.Add("看书");
            //data.Add("跑路");

            //List<string> list = client.GetList("杨过", 40, data);
            //Parallel.ForEach(list, m=> {
            //    Console.WriteLine(m);
            //});
            //Console.WriteLine(client.Say("过过"));

            Console.ReadKey();
        }
    }
}
