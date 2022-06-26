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
  
  ``` c#
  1. static readonly MyClass myins = new MyClass();
  2. static readonly MyClass myins = null;
  3. static readonly A = B * 20;
     static readonly B = 10;
  4. static readonly int [] constIntArray = new int[] {1, 2, 3};
  5. void SomeFunction()
     {
        const int a = 10;
        ...
     }
  
  /*
  1：不可以换成const。new操作符是需要执行构造函数的，所以无法在编译期间确定
  2：可以换成const。我们也看到，Reference类型的常量（除了String）只能是Null。
  3：可以换成const。我们可以在编译期间很明确的说，A等于200。
  4：不可以换成const。道理和1是一样的，虽然看起来1,2,3的数组的确就是一个常量。
  5：不可以换成readonly，readonly只能用来修饰类的field，不能修饰局部变量，也不能修饰property等其他类成员。
  */
  ```
  
  ``` c#
  //属于常量，但是不能使用const来声明
  public class Color
  {
      public static readonly Color Black = new Color(0, 0, 0);
      public static readonly Color White = new Color(255, 255, 255);
      public static readonly Color Red = new Color(255, 0, 0);
      public static readonly Color Green = new Color(0, 255, 0);
      public static readonly Color Blue = new Color(0, 0, 255);
  
      private byte red, green, blue;
  
      public Color(byte r, byte g, byte b)
      {
          red = r;
          green = g;
          blue = b;
      }
  }
  ```
  
  ``` c#
  /*
  static readonly需要注意的一个问题是，对于一个static readonly的Reference类型，只是被限定不能进行赋值（写）操作而已。而对其成员的读写仍然是不受限制的。
  */
  public static readonly MyClass myins = new MyClass();
  …
  myins.SomeProperty = 10;  //正常
  myins = new MyClass();    //出错，该对象是只读的
  
  /*
  如果MyClass是struct，则上述两句都出错
  */
  ```
  
  





* static class 静态类

  ``` tex
  类可以声明为 static 的，以指示它仅包含静态成员。
  静态类在加载包含该类的程序或命名空间时，由 .NET Framework 公共语言运行库 (CLR;特指：C#语言) 自动加载。
  使用静态类来包含不与特定对象关联的方法，具有通用性
  ```

  ``` tex
  注意事项：
  (1) 不能使用 new 关键字创建静态类的实例；
  (2) 仅包含静态成员；
  (3) 不能被实例化；
  (4) 密封的，不能被继承；
  (5) 不能包含实例构造函数，但可以包含静态构造函数；
  ```

  ``` tex
  关于静态构造函数的补充：
  (1) 静态构造函数不可继承；
  (2) 静态构造函数可以用于静态类，也可用于非静态类；
  (3) 静态构造函数无访问修饰符、无参数，只有一个 static 标志；
  (4) 静态构造函数不可被直接调用，当创建类实例或引用任何静态成员之前，静态构造函数被自动执行，并且只执行一次。
  ```

  ``` c#
      //ref: https://blog.csdn.net/xiaobai1593/article/details/7278014
  		public class ClassA
      {
          public static string AppName = "hello, this is a static class test";
          public static int num = 5;
   
          public ClassA()
          {
              num = 15;
          }
   
          public static int getNum()
          {
              return num;
          }
      }
  
          static void Main(string[] args)
          {
              int num=ClassA.getNum();
              Console.WriteLine(num);
              Console.ReadLine();
          }
  
  				//需要实例化
          static void Main(string[] args)
          {
              ClassA a = new ClassA();
              int num=ClassA.getNum();
              Console.WriteLine(num);
              Console.ReadLine();
          }
  ```

  ``` c#
     
  		//使用静态类
  		public static class ClassA
      {
          public static string AppName = "hello, this is a static class test";
          public static int num = 5;
   
          static ClassA()
          {
              num = 15;
          }
   
          public static int getNum()
          {
              return num;
          }
      }
  
  				//静态构造函数会在调用静态类的方法时自动调用
          static void Main(string[] args)
          {
              int num=ClassA.getNum();
              Console.WriteLine(num);
              Console.ReadLine();
          }
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

  

* this 用法

  ``` c#
  /*
  1. 限定被相似的名称隐藏的成员  this.name = name;
  2. 将对象作为参数传递到其他方法  
  3. 声明索引器 
  4. 扩展对象方法
  */
  
  //声明索引器
  public NameValueCollection Attr = new NameValueCollection(); 
  public string this[string key]
  {
    set { Attr[key] = value;}
    get { return Attr[key];}
  }
  
  //扩展对象方法
  public class Person
  {
    public string Sex{get;set;}
  }
  
  {
    public static string GetSex(this Person p)
    {
      return p.Sex;
    }
  }
  ```





* 获取时间戳

  ``` c#
  new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
  ```

  

* Disposable应用

  ref: https://blog.csdn.net/zhifeiya/article/details/8923280

  ``` c#
  [ComVisible(true)]
  public interface IDisposable
  {    // Methods
      void Dispose();
  }
  
  1.[ComVisible(true)]：
      指示该托管类型对 COM 是可见的.
  2.此接口的主要用途是释放非托管资源。
      当不再使用托管对象时，垃圾回收器会自动释放分配给该对象的内存。但无法预测进行垃圾回收的时间。另外，垃圾回收器对窗口句柄或打开的文件和流等非托管资源一无所知。将此接口的Dispose方法与垃圾回收器一起使用来显式释放非托管资源。当不再需要对象时，对象的使用者可以调用此方法。
  ```

  **Dispose()方法必须需要实现！**

  * 应用1

    ``` c#
    public class CaryClass :IDisposable
    {
        public void DoSomething()
        {
            Console.WriteLine("Do some thing....");
        }
        public void Dispose()
        {
             Console.WriteLine("及时释放资源");
        }
    }
    
    //调用方式1：使用Using语句会自动调用Dispose方法
      using (CaryClass caryClass = new CaryClass())
      {
           caryClass.DoSomething();
      }
    
    //调用方式2: 先实调用该接口的Dispose方法
     CaryClass caryClass = new CaryClass();
        try{
               caryClass.DoSomething();
         }
         finally
         {
           IDisposable disposable = caryClass as IDisposable;
           if (disposable != null)
                disposable.Dispose();
         }
    
    //两种方式的执行结果是一样的。
    /*
    使用try/finally 块比使用 using 块的好处是即使using中的代码引发异常，CaryClass的Dispose方法仍有机会清理该对象。所以从这里看还是使用try/catch好一些。
    */
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



* Directory & File

  ``` c#
  				static public void CopyFolder( string sourceFolder, string destFolder )
          {
              if (!Directory.Exists( destFolder ))
                  Directory.CreateDirectory( destFolder );
              string[] files = Directory.GetFiles( sourceFolder );
              foreach (string file in files)
              {
                  string name = Path.GetFileName( file );
                  string dest = Path.Combine( destFolder, name );
                  File.Copy( file, dest );
              }
              string[] folders = Directory.GetDirectories( sourceFolder );
              foreach (string folder in folders)
              {
                  string name = Path.GetFileName( folder );
                  string dest = Path.Combine( destFolder, name );
                  CopyFolder( folder, dest );
              }
          }
  ```






* String Format() / ToString()

  ``` c#
  String.Format (String, Object) 将指定的 String 中的格式项替换为指定的 Object 实例的值的文本等效项。
  String.Format (String, Object[]) 将指定 String 中的格式项替换为指定数组中相应 Object 实例的值的文本等效项。
  String.Format (IFormatProvider, String, Object[]) 将指定 String 中的格式项替换为指定数组中相应 Object 实例的值的文本等效项。指定的参数提供区域性特定的格式设置信息。
  String.Format (String, Object, Object) 将指定的 String 中的格式项替换为两个指定的 Object 实例的值的文本等效项。
  String.Format (String, Object, Object, Object) 将指定的 String 中的格式项替换为三个指定的 Object 实例的值的文本等效项。
  ```

  | **字符** | **说明**         | **示例**                               | **输出**   |
  | -------- | ---------------- | -------------------------------------- | ---------- |
  | C        | 货币             | **string.Format**("{0:C3}", 2)         | ＄2.000    |
  | D        | 十进制           | **string.Format**("{0:D3}", 2)         | 002        |
  | E        | 科学计数法       | 1.20E+001                              | 1.20E+001  |
  | G        | 常规             | **string.Format**("{0:G}", 2)          | 2          |
  | N        | 用分号隔开的数字 | **string.Format**("{0:N}", 250000)     | 250,000.00 |
  | X        | 十六进制         | **string.Format**("{0:X000}", 12)      | C          |
  |          |                  | **string.Format**("{0:000.000}", 12.2) | 012.200    |

  * 数字格式

    ``` c#
     string str1 =string.Format("{0:N1}",);               //result: 56,789.0
     string str2 =string.Format("{0:N2}",);               //result: 56,789.00
     string str3 =string.Format("{0:N3}",);               //result: 56,789.000
     string str8 =string.Format("{0:F1}",);               //result: 56789.0
     string str9 =string.Format("{0:F2}",);               //result: 56789.00
     string str11 =( / 100.0).ToString("#.##");           //result: 567.89
     string str12 =( / ).ToString("#.##");             //result: 567
    ```

  * 格式化货币

    ``` c#
    //跟随系统语言，中文为人民币 英文为美元，默认保留两位小数
    string.Format("{0:C}",0.2)   ¥0.20  ｜ $0.20
    
    //默认格式化小数点后面保留两位小数，如果需要保留一位或者更多，可以指定位数，（会四舍五入）
    string.Format("{0:C1}",23.15)  ¥23.2.  
    ```

  * 十进制数字（**格式化成固定的位数，位数不能少于未格式化前，只支持整形**）

    ``` c#
    string.Format("{0:D3}",) //结果为：023
    string.Format("{0:D2}",) //结果为：1223，（精度说明符指示结果字符串中所需的最少数字个数。）
    ```

  * **用分号隔开的数字，并指定小数点后的位数**

    ``` c#
    string.Format("{0:N}", ) //结果为：14,200.00 （默认为小数点后面两位）
    string.Format("{0:N3}", 14200.2458) //结果为：14,200.246 （自动四舍五入）
    ```

  * 格式化百分比

    ``` c#
    string.Format("{0:P}", 0.24583) //结果为：24.58% （默认保留百分的两位小数）
    string.Format("{0:P1}", 0.24583) //结果为：24.6% （自动四舍五入）
    ```

  * 零占位符和数字占位符

    ``` c#
    string.Format("{0:0000.00}", 12394.039) //结果为：12394.04
    string.Format("{0:0000.00}", 194.039) //结果为：0194.04
    string.Format("{0:###.##}", 12394.039) //结果为：12394.04
    string.Format("{0:####.#}", 194.039) //结果为：194
      
    /*
    零占位符： 如果格式化的值在格式字符串中出现“0”的位置有一个数字，则此数字被复制到结果字符串中。小数点前最左边的“0”的位置和小数点后最右边的“0”的位置确定总在结果字符串中出现的数字范围。 “00”说明符使得值被舍入到小数点前最近的数字，其中零位总被舍去。
    数字占位符： 如果格式化的值在格式字符串中出现“#”的位置有一个数字，则此数字被复制到结果字符串中。否则，结果字符串中的此位置不存储任何值。
    
    如果“0”不是有效数字，此说明符永不显示“0”字符，即使“0”是字符串中唯一的数字。如果“0”是所显示的数字中的有效数字，则显示“0”字符。 “##”格式字符串使得值被舍入到小数点前最近的数字，其中零总被舍去。
    */
    ```

  * 日期格式化

    ``` c#
    string.Format("{0:d}",System.DateTime.Now) //结果为：2009-3-20 （月份位置不是03）
    string.Format("{0:D}",System.DateTime.Now) //结果为：2009年3月20日
    string.Format("{0:f}",System.DateTime.Now) //结果为：2009年3月20日 15:37
    string.Format("{0:F}",System.DateTime.Now) //结果为：2009年3月20日 15:37:52
    string.Format("{0:g}",System.DateTime.Now) //结果为：2009-3-20 15:38
    string.Format("{0:G}",System.DateTime.Now) //结果为：2009-3-20 15:39:27
    string.Format("{0:m}",System.DateTime.Now) //结果为：3月20日
    string.Format("{0:t}",System.DateTime.Now) //结果为：15:41
    string.Format("{0:T}",System.DateTime.Now) //结果为：15:41:50
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

