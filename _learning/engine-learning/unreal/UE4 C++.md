# UE4 C++



* sprintf_s vs snprintf

  sprintf 用于将输出存到字符缓冲中

  ``` c++
  //函数原型：sprintf(char *buffer, const char *format, [argument]);
  
  int a=1,b=2;
  char s[10];
  sprintf(s,"a=%d,b=%d",1,2);
  puts(s);
  ```

  [sprintf_s的使用](https://www.cnblogs.com/dirt2/p/6104198.html)

  ``` text
  windows平台下线程安全的格式化字符串函数sprint_s并非标准C函数，因此linux下无法使用，但可以使用snprintf函数代替。
  
  *函数原型：*/
  int snprintf(char *dest, size_t n, const char *fmt, ...);
  
  /*函数说明: 最多从源串中拷贝n－1个字符到目标串中，然后再在后面加一个0。所以如果目标串的大小为n的话，将不会溢出。
  函数返回值: 若成功则返回存入数组的字符数，若编码出错则返回负值。
  推荐的用法：*/
  void f(const char *p)
  {
      char buf[11]={0};
      snprintf(buf, sizeof(buf), "%10s", p); // 注意：这里第2个参数应当用sizeof(str)，而不要使用硬编码11，也不应当使用sizeof(str)-1或10
      printf("%sn",buf);
  }
  ```

* vsprintf vs sprintf

  [C语言printf()、sprintf()、vsprintf() 的区别与联系](https://blog.csdn.net/Raito__/article/details/48860119)

  ``` c++
  //需要引入相关头文件 #include <stdarg.h>
  //函数原型： vsprintf(char *buffer, char *format, va_list param);
  
  //可以用 vsprintf() 来实现 sprintf()
  
  void Myprintf(const char* fmt,...);
   
  int a=1,b=2;
  char s[10];
  Myprintf("a=%d,b=%d",a,b);
   
  void Myprintf(const char* fmt,...)
  {
    char s[10];
    va_start(ap, fmt);	
    vsprintf(s,fmt,ap);
    va_end(ap);	
    puts(s);
  }
  
  
  //新需求：功能是将格式化字符串输出两遍
  void Myprintf(const char* fmt,...)
  {
    char s[10];
    sprintf(s,fmt);
    puts(s);
    puts(s);
  }
  //传入的其实是 sprintf(s,"a=%d,b=%d") 而不是 sprintf(s,"a=%d,b=%d",a,b)
  //类似这种封装用 sprintf() 是无法实现的，使用 sprintf() 只能原始的为它输入所有的参数而不能以传参的方式给它
  
  //修改为vsprintf
  void Myprintf(const char* fmt,...)
  {
    char s[10];
    va_list ap;
    va_start(ap,fmt);
    vsprintf(s,fmt,ap);
    va_end(ap);
    puts(s);
    puts(s);
  }
  //调用：Myprintf("a=%d,b=%d",a,b);
  //输出: 
  	//a=1,b=2
  	//a=1,b=2
  
  
  //vsprintf原理：
  /*
  执行函数时，函数参数是倒序压入栈中（栈，先进后出）
  
  vsprintf() 为了能够解析你传给它的多个参数，你必须告诉它参数从哪里开始。
  vadefs.h 头文件中这么定义 ：typedef char * va_list，于是我们定义了一个 va_list ap 来保存参数起始地址。
  va_start(ap,fmt) 就找出这个函数在栈中排列的一堆参数的起始地址，然后直接浏览栈中参数，并用 vsprintf() 实现格式化字符串的读取，最后 vs_end(ap) 释放ap，就像释放指针一样。通俗地说就是因为 vsprintf() 比 sprintf() 更加接近底层(栈)，因此能实现这个目的，也是因此能用 vsprintf() 来实现 sprintf()。
  */
  ```

  

* UE4 设置宏

  ``` c#
  //错误做法：UE4的VS工程，Games/项目 - 属性 - 预处理器定义
  //正确做法：
  
  //1. 项目模块编译文件 xxx.build.cs
  public class MyTest : ModuleRules
  {
      public MyTest(TargetInfo Target)
      {
          PublicDependencyModuleNames.AddRange(new string[] { "Core", "CoreUObject", "Engine", "InputCore" });
  
          Definitions.Add("HELLO_WORLD"); //添加 自定义的宏 或者 引擎的宏
      }
  }
  //2. 重新生成vs工程
  //3. 观察宏里面的代码是否高亮
  //4. build vs or compile ue4
  ```

  

* UE4 #if vs #ifdef

  ``` text
  #if 	判断这个宏 是否是true
  #ifdef  判断这个宏 是否定义（不一定为true）
  ```

  

* UE4 WITH_EDITOR vs WITH_EDITORONLY_DATA

  ``` text
  WITH_EDITOR used for methods(except cases when that methods needs some strictly related fields)
  WITH_EDITORONLY_DATA used for fields(except cases when that fields needs some strictly related methods, for example Getter/Setter)
  
  Whether to compile the editor or not. Only desktop platforms (Windows or Mac) will use this, other platforms force this to false.
    Whether to compile WITH_EDITORONLY_DATA disabled. Only Windows will use this, other platforms force this to false.
  
    WITH_EDITORONLY_DATA in headers for wrapping reflected members.
    WITH_EDITOR in CPP files for code.. Has nothing to do with reflection.
  
    # UPROPERTY和UFUNCTION包裹的成员变量，在头文件中应该使用WITH_EDITORONLY_DATA
    # WITH_EDITOR一般只是在CPP中使用的
  ```

