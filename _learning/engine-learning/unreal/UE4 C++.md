# UE4 C++

* sprintf_s vs snprintf

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

  

  