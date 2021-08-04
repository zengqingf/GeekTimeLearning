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

  

### 

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

  