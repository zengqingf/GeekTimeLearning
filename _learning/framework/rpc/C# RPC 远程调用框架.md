# RPC 远程调用框架

## Apache Thrift

* csharp

  [C# RPC远程方法调用框架thrift](https://blog.csdn.net/u011511086/article/details/79978292?utm_medium=distribute.pc_aggpage_search_result.none-task-blog-2~aggregatepage~first_rank_v2~rank_v28-2-79978292.nonecase&utm_term=c#%20rpc&spm=1000.2123.3001.4430)

  测试环境 windows 10 & vs 2017

  1. vs 2017 中菜单栏 工具-NuGet包管理器-管理解决方案NuGet程序包

     搜索 thrift-csharp 安装到Client 和 Server 和 Common 项目中

  2. 创建Server 和 Client代码

  3. [下载thrift-0.11.0.exe](https://downloads.apache.org/thrift/0.11.0/)

  4. 新建模板  IDL文件 （接口描述语言）
  
     ``` text
     namespace * Common
     
     service ChatService
     {
       string Say(1: string thing),
     
       list<string> GetList(
         1: string function2_arg1,
         2: i32 function2_arg2,
         3: list<string> function2_arg3
       ),
   }
     ```

  5. 执行命令
  
     ``` powershell
   thrift-0.11.0.exe -gen csharp demo-interface.thrift
     ```

  6. 上述4 、5两步后，会在命令执行的当前目录生成一个文件夹 gen-csharp 里面包含生成类 ChatService.cs，将Common引用添加到Client和Server中
  
  7. 执行Server.exe 执行Client.exe
  
  8. 额外参考
  
     [RPC学习--C#使用Thrift简介，C#客户端和Java服务端相互交互](https://www.cnblogs.com/amosli/p/3948342.html)
  
     [Thrift初探：简单实现C#通讯服务程序](https://www.cnblogs.com/hanmos/archive/2011/09/15/2177891.html)
     
     [Thrift 开源多语言服务开发框架](https://lucky521.github.io/blog/framework/2015/12/01/thirft.html)
     
     [Thrift 双向通信实现（C#版）](https://blog.csdn.net/lwwl12/article/details/77330968)
     
     [C#实现Thrift服务端与客户端](https://blog.csdn.net/lwwl12/article/details/77116253)
     
     [blog - thrift c# rpc 框架 入门](https://blog.iyangkai.cn/categories/Thrift/)
     
  9. 扩展
  
     [HslCommunication](http://www.hslcommunication.cn/)
  
     [C# RPC 远程RPC调用 C#服务器设计 CS架构设计 远程API接口 RPC可视化 MqttRpc实现 HslCommunication远程调用，同步网络访问，进度报告](https://www.cnblogs.com/dathlin/p/13864866.html)
  
  10. 
  
     
  
* java & android

  [Apache thrift 安装及使用](https://www.cnblogs.com/sumingk/articles/6073105.html)

  [Apache thrift RPC 双向通信](https://www.cnblogs.com/sumingk/p/6073824.html)

  [Apache Thrift – 可伸缩的跨语言服务开发框架](https://developer.ibm.com/zh/tutorials/j-lo-apachethrift/)

* ref

  [Apache Thrift Tutorial](https://thrift.apache.org/tutorial/)



## other

[github - hprose-dotnet](https://github.com/hprose/hprose-dotnet)

[RabbitMQ教程C#版 - 远程过程调用(RPC)](https://www.cnblogs.com/esofar/p/rabbitmq-rpc.html)

[C#远程调用技术WebService修炼手册](https://www.jianshu.com/p/fea3764af2c3)