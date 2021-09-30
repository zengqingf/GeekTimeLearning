# C# API



## 关键字



* static readonly vs const

  [C# const和static readonly区别](https://www.cnblogs.com/wangshenhe/archive/2012/05/16/2503831.html)

  ``` text
  1. const 字段只能在该字段的声明中初始化。
     readonly 字段可以在声明或构造函数中初始化。因此，根据所使用的构造函数，readonly 字段可能具有不同的值。
  2. const 字段是编译时常数，而 readonly 字段可用于运行时常数。
  3. const 默认就是静态的，而 readonly 如果设置成静态的就必须显示声明。
  4．const 对于引用类型的常数，可能的值只能是 string 和 null。
     readonly可以是任何类型
     
  注意：  对于一个 readonly 的 Reference 类型，只是被限定不能进行赋值（写）操作而已。而对其成员的读写仍然是不受限制的。
  
  const的值是在编译期间确定的，因此只能在声明时通过常量表达式指定其值。而static readonly是在运行时计算出其值的，所以还可以通过静态构造函数来赋值
  ```

  ``` c#
  using System;
  class P
  {
      static readonly int A=B*10;
      static readonly int B=10;
      public static void Main(string[] args)
      {
          Console.WriteLine("A is {0},B is {1} ",A,B);
      }
  }
  //output: A is 0,B is 10
  
  class P
  {
      const int A=B*10;
      const int B=10;
      public static void Main(string[] args)
      {
          Console.WriteLine("A is {0},B is {1} ",A,B);
      }
  }
  //output: A is 100,B is 10;
  ```
  
  



* StringBuilderCache

  ``` c#
  StringBuilder sb = StringBuilderCache.Acquire();
  sb.Append("xxx");
  StringBuilder.GetStringAndRelease(sb);					//性能更优
  
  vs.
      
  StringBuilder sb = new StringBuilder();
  sb.Append("xxx");
  sb.ToString();
  ```

  



---



## System.IO



* EXE执行目录

  [C#获取当前程序运行路径的方法集合](https://www.cnblogs.com/cocoulong/archive/2010/01/30/1660119.html)

  ``` c#
  unityExePath = System.Environment.CurrentDirectory; //E:\ws\svn\ald_sdk\SDK_Projects\Many_Functions_SDK_2018
  unityExePath = this.GetType().Assembly.Location; //E:\ws\svn\ald_sdk\SDK_Projects\Many_Functions_SDK_2018\Library\ScriptAssemblies\Assembly-CSharp-Editor.dll
  unityExePath = System.AppDomain.CurrentDomain.BaseDirectory; //D:\programs\Unity\2018.4.23f1\Unity\Editor
  unityExePath = System.IO.Directory.GetCurrentDirectory();//E:\ws\svn\ald_sdk\SDK_Projects\Many_Functions_SDK_2018
  unityExePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase; //D:\programs\Unity\2018.4.23f1\Unity\Editor
  unityExePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName; 
  // D:\programs\Unity\2018.4.23f1\Unity\Editor\Unity.exe
  
  //获取当前进程的完整路径，包含文件名(进程名)。
  string str = this.GetType().Assembly.Location;
  result: X:\xxx\xxx\xxx.exe (.exe文件所在的目录+.exe文件名)
  
  //获取新的 Process 组件并将其与当前活动的进程关联的主模块的完整路径，包含文件名(进程名)。
  string str = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
  result: X:\xxx\xxx\xxx.exe (.exe文件所在的目录+.exe文件名)
  
  //获取和设置当前目录（即该进程从中启动的目录）的完全限定路径。
  string str = System.Environment.CurrentDirectory;
  result: X:\xxx\xxx (.exe文件所在的目录)
  
  //获取当前 Thread 的当前应用程序域的基目录，它由程序集冲突解决程序用来探测程序集。
  string str = System.AppDomain.CurrentDomain.BaseDirectory;
  result: X:\xxx\xxx\ (.exe文件所在的目录+"\")
  
  //获取和设置包含该应用程序的目录的名称。(推荐)
  string str = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
  result: X:\xxx\xxx\ (.exe文件所在的目录+"\")
  
  //获取启动了应用程序的可执行文件的路径，不包括可执行文件的名称。
  string str = System.Windows.Forms.Application.StartupPath;
  result: X:\xxx\xxx (.exe文件所在的目录)
  
  //获取启动了应用程序的可执行文件的路径，包括可执行文件的名称。
  string str = System.Windows.Forms.Application.ExecutablePath;
  result: X:\xxx\xxx\xxx.exe (.exe文件所在的目录+.exe文件名)
  
  //获取应用程序的当前工作目录(不可靠)。
  string str = System.IO.Directory.GetCurrentDirectory();
  result: X:\xxx\xxx (.exe文件所在的目录)
  ```






---



### C#示例

* md5

  ``` c#
  private string MD5Create(string STR) //STR为待加密的string
  {
  string pwd = "";
  //pwd为加密结果
  MD5 md5 = MD5.Create();
  byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(STR));
  //这里的UTF8是编码方式，你可以采用你喜欢的方式进行，比如UNcode等等
  for (int i = 0; i < s.Length; i++)
  {
  pwd = pwd + s[i].ToString();
  }
  return pwd;
  }
  ```

  





---





## C#运行原理

* C#简介

  ```
  C# 是微软推出的一种基于 .NET 框架的、面向对象的高级编程语言

* IL简介

  ``` tex
  中间语言 （CIL，Common Intermediate Language，也叫 MSIL）
  CIL 也是一个高级语言
  而运行 CIL 的虚拟机叫 CLR（Common Language Runtime）
  
  .Net framework (C#、CIL、CLR + 微软提供的一套基础类库)
  ```

* 虚拟机区分

  ![](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/20210717105506.jpg)

* .Net Framework vs. Mono

  ``` tex
  Mono 是跨平台的 .Net Framework 的实现
  将 CLR 在所有支持的平台上重新实现了一遍，将 .Net Framework 提供的基础类库也重新实现了一遍
  (Compile Time 的工作实际上可以直接用微软已有的成果，只要将 Runtime 的 CLR 在其他平台实现，这个工作量不仅大，而且需要保证兼容)
  ```

  ![](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/20210717105812.png)

* mono应用

  ``` tex
  Unity3D 内嵌了一个 Mono 虚拟机
  Unity3D 是通过 Mono 虚拟机，运行 C# 通过编译器编译后生成的 IL 代码
  也是通过mono实现游戏跨平台
  ```






---



### C#反射(Reflection)

* ref

  [C#反射(Reflection)详解](https://www.cnblogs.com/wangshenhe/p/3256657.html)

  [[C#反射]C#中的反射解析及使用.](https://www.cnblogs.com/wang-meng/p/5440515.html)

* 概念

  ``` tex
  反射：
  .Net获取运行时（CLR）类型信息的方式，.Net应用程序由“程序集(Assembly)” "模块(Module)" "类型(Class)"组成
  反射作为一种编程方式，可以在运行期间获取上述程序组成部分，提供了更灵活的代码设计方向（可以在程序预留接口中新增插件，可以不事先指定处理格式（通过读配置动态调整）等等）
  
  如：Assembly类可以获得正在运行的装配件信息，也可以动态的加载装配件，以及在装配件中查找类型信息，并创建该类型的实例。
  Type类可以获得对象的类型信息，此信息包含对象的所有要素：方法、构造器、属性等等，通过Type类可以得到这些要素的信息，并且调用之。
  MethodInfo包含方法的信息，通过这个类可以得到方法的名称、参数、返回值等，并且可以调用之。
  诸如此类，还有FieldInfo、EventInfo等等，这些类都包含在System.Reflection命名空间下。
  
  
  反射作用：
  1、可以使用反射动态地创建类型的实例，将类型绑定到现有对象，或从现有对象中获取类型
  2、应用程序需要在运行时从某个特定的程序集中载入一个特定的类型，以便实现某个任务时可以用到反射。
  3、反射主要应用与类库，这些类库需要知道一个类型的定义，以便提供更多的功能。
  
  
  装配件：
  	命名空间类似于Java的包，但是Java包需要按照目录结构来放置，命名空间没有这个限制
  装配件是.Net应用程序执行的最小单位，（.dll、.ext都是装配件）
  命名空间可以存在多个装配件中，装配件中可以由多个命名空间
  
  
  类型               作用 
  Assembly        通过此类可以加载操纵一个程序集，并获取程序集内部信息 
  EventInfo        该类保存给定的事件信息 
  FieldInfo         该类保存给定的字段信息 
  MethodInfo      该类保存给定的方法信息 
  MemberInfo     该类是一个基类，它定义了EventInfo、FieldInfo、MethodInfo、PropertyInfo的多个公用行为 
  Module            该类可以使你能访问多个程序集中的给定模块 
  ParameterInfo 该类保存给定的参数信息　　　　　　 
  PropertyInfo    该类保存给定的属性信息
  ```

* API

  ``` c#
  //获取反射类型
  Type t = Type.GetType("System.String");
  Assembly.GetType(...);
  System.Object.GetType(...);
  
  //根据类型创建对象
  Type t = ...
  SrcType st = (SrcType)Activator.CreateInstance(t);
  
  //获取类型中的方法
      //获取类型信息
      Type  t  =  Type.GetType("TestSpace.TestClass");
      //构造器的参数
      object[]  constuctParms  =  new  object[]{"timmy"};
      //根据类型创建对象
      object  dObj  =  Activator.CreateInstance(t,constuctParms);
      //获取方法的信息
      MethodInfo  method  =  t.GetMethod("GetValue");
      //调用方法的一些标志位，这里的含义是Public并且是实例方法，这也是默认的值
      BindingFlags  flag  =  BindingFlags.Public  |  BindingFlags.Instance;
      //GetValue方法的参数
      object[]  parameters  =  new  object[]{"Hello"};
      //调用方法，用一个object接收返回值
      object  returnValue  =  method.Invoke(dObj,flag,Type.DefaultBinder,parameters,null);
  
  
  //动态创建委托
      TestClass  obj  =  new  TestClass();
      //获取类型，实际上这里也可以直接用typeof来获取类型
      Type  t  =  Type.GetType(“TestSpace.TestClass”);
      //创建代理，传入类型、创建代理的对象以及方法名称
      TestDelegate  method  =  (TestDelegate)Delegate.CreateDelegate(t,obj,”GetValue”);
      String  returnValue  =  method(“hello”);
  ```

  

* 示例

  ``` c#
  //初版本Unity集成多渠道SDK
  private static SDKInterface _createInstanceByClassName(string name)
  {
     Assembly ass = Assembly.GetAssembly(typeof(SDKInterface));
     return ass.CreateInstance(name) as SDKInterface;
  }
  
  private static SDKInterface _getSDKInstance()
      {
  #if UNITY_EDITOR
  		return new SDKInterfaceDefault();
  #endif
  
  #if UNITY_ANDROID
          switch (Global.Settings.sdkChannel)
          {
              case SDKChannel.XY:
                  return _createInstanceByClassName("SDKInterfaceAndroid_XY");
              case SDKChannel.MG:
                  return _createInstanceByClassName("SDKInterfaceAndroid_MG");
              case SDKChannel.NONE:
              case SDKChannel.COUNT:
  				return _createInstanceByClassName("SDKInterfaceAndroid");
          }
  #elif UNITY_IOS || UNITY_IPHONE
          switch (Global.Settings.sdkChannel)
          {
              case SDKChannel.XY:
                  return _createInstanceByClassName("SDKInterfaceIOS_XY");
              case SDKChannel.AISI:
  				return _createInstanceByClassName("SDKInterfaceIOS_As");
  			case SDKChannel.NONE:
  				return _createInstanceByClassName("SDKInterfaceIOS");
          }
  #endif
          return new SDKInterfaceDefault();
  }
  
  public static SDKInterface instance {
  	get {
  		if (_instance == null) {
              _instance = _getSDKInstance();
  		}
  		return _instance;
  	}
  }
  ```

  





---





### C#闭包

