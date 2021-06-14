using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace ThriftServer
{
    public class TestServer : ChatService.Iface
    {
        public List<string> GetList(string function2_arg1, int function2_arg2, List<string> function2_arg3)
        {
            List<string> list = new List<string>();
            Parallel.ForEach(
                function2_arg3, m => {
                    list.Add($"{function2_arg1}, 年龄 {function2_arg2}, 正在：{m}");
                });
            return list;
        }

        public string Say(string thing)
        {
            return thing + "测试数据";
        }
    }
}
