using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Thrift.Protocol;
using Thrift.Transport;
using Common;

namespace Client
{
    class ClientTask
    {
        public static string GetServerData(string words, ChatService.Client client)
        {
            string allbooks = client.Say(words);
            return allbooks;
        }
    }
}
