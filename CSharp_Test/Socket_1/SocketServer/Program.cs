using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    /*
     Socket不是协议，是为了方便使用TCP和UDP抽象出来的一层，位于应用层和传输层之间的一组接口，实现进程在网络中的通信
      
     当两台主机通信时 必须通过Socket连接 Socket利用TCP/IP协议建立TCP连接 TCP连接更依赖于底层IP协议

        传输层协议：
        TCP: 面向连接 （首先会建立连接，如三次握手，
        建立连接是为了在客户端和服务器维护连接，
        从而建立一定的数据结构来维护双方交互的状态，
        用这样的数据结构来保证所谓面向连接的特性） 

        提供可靠服务

        面向字节流
        (发送的是流，不同于IP包发的是IP包)

        有拥塞控制
        (当发现丢包或者网络环境差，会根据情况调整自身行为，看看是不是需要减慢发送频次)

        有状态服务
        （精确记录发送和接收的是否到达，当前发送和接收是否一致）


        UDP: 无连接  
        
        不可靠（继承了IP包的特性，不保证不丢失，不保证按顺序到达）  
        
        传输速度快

        基于数据报，按个发，按个收

        没有拥塞控制
        （应用让发就发）

        无状态服务
        （发出后不维护）

         */

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("服务器：{0}", Dns.GetHostEntry(Dns.GetHostName()).AddressList[3]);

            SocketServer_TCP ss = new SocketServer_TCP(2345);
            Console.ReadKey();
        }
    }
}
