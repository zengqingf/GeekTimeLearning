# TodoList

* **链接没用，要把里面内容读懂**
  
  <details>
      <summary>折叠</summary>
      <p>测试</p>
      <pre><code>示例</code></pre>
      ``` shell
      	echo "hello world"
      ```
  </details>
  
* books

  [github - xjq7 - books](https://github.com/xjq7/books)

  [github - itdevbooks - pdf](https://github.com/itdevbooks/pdf)

  [github - awesome-programming-books](https://github.com/jobbole/awesome-programming-books)

  [github - GitHub-Chinese-Top-Charts](https://github.com/kon9chunkit/GitHub-Chinese-Top-Charts)

  

* Devops

  link: [awesome-game-tester](https://github.com/jianbing/awesome-game-tester)

  1. Ansible 配置版本机环境 如 SVN需要使用自己配置的执行库 不用系统自带的 避免升级系统后的问题

  2. 考虑指定版本合入打包 如有需求是在已有包基础上 合入一些单独的改动 保持已有包大致内容一直 出包

  3. ftp上的html php脚本抽出

  4. 打安装包考虑是否要重打资源 或者 添加资源缓存

  5. Mac版本机问题

     [如果 Mac 在启动时出现问号](https://support.apple.com/zh-cn/HT204323)

     [How to use safe mode on your Mac](https://support.apple.com/lv-lv/HT201262)

  6. jenkins打包流程 整理

  7. 线上版本管理

     ``` text
     整包上线时需要存到线上版本号配置文件
     ```

     ![](.\0006.png)

  8. - [x] stf 扩展 (**搭建在虚拟机上**)  

     - [ ] atx server2

     ``` text
     链接其他设备上的android设备等
     ```

     [github - ShareDevice](https://github.com/sunshine4me/ShareDevice)

     [github - atx-server](https://github.com/openatx-archive/atx-server)

     [github - awesome-game-tester](https://github.com/jianbing/awesome-game-tester)

     

  9. spug 自动化运维平台搭建 （主机管理）

  10. visual svn server 新增用户 修改密码

  11. maven / cocoapods

      [github - unity-jar-resolver](https://github.com/googlesamples/unity-jar-resolver)

  12. 钉钉主动触发事件

      [github - DingtalkChatbot](https://github.com/zhuifengshen/DingtalkChatbot)

  13. fastbuild 

      [github - fastbuild](https://github.com/fastbuild/fastbuild)

      - [ ] fastbuild源码分析，原理

  14.  

      - [x] apple 远程添加设备被下载到本地

  15. 

      1.0 jenkins工程整理

      jenkins pipeline 自己搭建一次  （union build ==> unity 分布打包）

      

* Unity
  
  1. Unity接口源码 如AndroidJavaClass AndroidJavaObject 内部实现
  
  2. 类似dll的插件导入
  
  3. Android runOnUiThread实现
  
  4. SDK插件化 
  
     ``` text
     SDK功能整理出来  插件化  快速集成 
     - 一些自己的插件功能，做成方便集成到项目里的模块
     ```
  
  5. 优化
  
     [UWA 优化规则](https://mp.weixin.qq.com/mp/appmsgalbum?__biz=MzI3MzA2MzE5Nw==&action=getalbum&album_id=1521119040851771393&、scene=173&from_msgid=2668921564&from_itemidx=1&count=3#wechat_redirect)
  
     ![](.\0007.jpg)
  
     ``` text
     杜哥文档，动静分离、合批
     Editor 检查文本 图片 射线等的工具
     
     mask vs rect mask 2D  是否影响合批  多级mask 嵌套的合批表现  使用rect mask可能出现负优化
     
     预制体合图 图集引用 检查工具  优化
     
     界面打开drawcall 检查
     
     上述editor工具是否支持 运行时放入场景中的预制体  更准备 （因为有些图集都是动态加载的）
     
     1.5 优化后的预制体
     
     comuilist  statecontroller扩展 看下
     
     spine动画 播放的 状态机代码编写
     
     
     ```
  
  6. lua使用
  
     ``` text
     ilruntime   适合小项目
     xlua  比较完善
     ```
  
  7. 小地图和大地图  功能
  
     ``` 
     位置映射
     深度搜索 DFS  广度优先  最短路径  a星
     
     分享方向
     1. 代码优化
     2. 小功能扩展到整个功能
     3. 核心算法
     4. 分析现有功能的优化点
     5. 重做功能时 顺便优化现有代码结构
     ```
  
  7. 通用组件 
  
  8. UI辅助Editor
  
  9. 做功能时，一些代码注意点
  
     ``` text
     if流程走不到 需要有反馈 客户端信息
     如 按钮点了没反应
     交互的地方要有提示
     链式引用 尽量避免 要判空
     
     有些需求 需要自己过一遍 不能加一些判断 如运营活动 领取时加客户端判断等级 这种就没必要了 很不好维护啊
     ```
  
  10. H5 战斗逻辑  状态机
  
  11. 帧同步
  
      ``` text
      github 状态同步 帧同步 有个小游戏分享  版署包  chijidemo 
      ```
  
  13. Unity 4.3源码 查看JNI实现
  
      
  
* CSharp
  
  1. dll生成
  
  2. C# 4.x 新特性
  
     ``` text
     於乐凯: 因为现在公司有些项目unity升到2018（2019）后，支持了c#4.x运行环境，已经有人开始用4.x的一些特性了(?,??,?=等）。
     这里大家需要看下这个文章https://www.xuanyusong.com/archives/4713，里面有些坑。
     我这边是不推荐使用，除非你清楚不会出问题的情况下。
     ```
  
     
  
* Android & Java
  1. 安卓四大组件
  
  2. 安卓性能优化
  
     [Android性能优化盘点 - 内存优化](https://www.jianshu.com/p/fba7b43bdc9c)
  
     [Android内存优化大盘点](https://mp.weixin.qq.com/s/ghupyR4z0yYD-Fsd14rgEg)
  
  3. runOnUiThread
  
  4. Java 和 Cpp
  
  5. Gradle
  
  6. 混淆
  
  7. SDK中间件（混淆、消息协议、现有多平台接口在新框架中重构）
  
  8. Logger 组件重构  目前用的是github上的
  
     ``` text
     orhanobut/logger   
     
      java.util.logging.Logger  vs. log4j
     ```
  
  9. breakpad   极客时间
  
  10. SDK源码开发
  
      ![](.\0005.png)
  
* 奔溃闪退收集

  * breakpad

    第三方SDK : bugly / test+
    
  * link

    [如何高效定位Unity安卓开发中的闪退问题](https://blog.uwa4d.com/archives/USparkle_Crash.html)
  
    [Unity il2cpp打包 拷贝符号表](https://support.unity3d.com/hc/en-us/articles/115000177543-Where-I-can-get-the-symbols-file-for-the-libil2cpp-so-library-in-an-Android-IL2CPP-build-to-symbolicate-call-stacks-from-crashes-on-my-production-builds-)
  
  ``` text
  ./arm-linux-androideabi-addr2line -f -C -e XXX/armeabi-v7a/libil2cpp.so 0x2eabfd8 0x2...
  ```
  
  ![](.\0004.png)
  
  
  
  Logcat.bat  从 Android SDK目录下拷贝依赖库 “adb.exe AdbWinApi.dll AdbWinUsbApi.dll fastboot.exe ” 
  
  ``` shell
  @echo off  
   
  set "year=%date:~0,4%"
  set "month=%date:~5,2%"
  set "day=%date:~8,2%"
  set "hour_ten=%time:~0,1%"
  set "hour_one=%time:~1,1%"
  set "minute=%time:~3,2%"
  set "second=%time:~6,2%"
   
  set adb="%~dp0\adb.exe"
  echo %adb%
   
  %adb% logcat -v time -d >  %year%%month%%day%%hour_ten%%hour_one%%minute%%second%.log &
  ```
  
  
  
  



* UE4
  
  1. Android JNI 原理
  
  2. ulua UI框架 
  
  3. ue4 delegate
  
     > 参考教程
     >
     > [知乎1](https://www.zhihu.com/column/unrealengine4)
     >
     > [详解UE4静态库与动态库的导入与使用](https://blog.csdn.net/u012999985/article/details/71554628)
     >
     > [从源码剖析虚幻引擎编译原理](https://yinpengd.github.io/2019/08/13/%E4%BB%8E%E6%BA%90%E7%A0%81%E5%89%96%E6%9E%90%E8%99%9A%E5%B9%BB%E5%BC%95%E6%93%8E%E7%BC%96%E8%AF%91%E5%8E%9F%E7%90%86/)
     >
     > [UE4 Gameplay之GameMode流程分析(一)](https://zhuanlan.zhihu.com/p/70045930)
     >
     > [InsideUE4](https://zhuanlan.zhihu.com/insideue4)
     >
     > [UnityToUE4](https://muyunsoft.com/blog/Unreal4/Basic/UnityToUE4.html)
     >
     > [查LP - 循迹研究室 - UnrealEngine](https://imzlp.com/categories/Unreal-Engine/)
     >
     > [UE4 - blog1](https://al3ix.com/)
     >
     > [UE4 - blog2](https://blog.csdn.net/u010019717)
     >
     > [UE4 - blog3](http://www.leoychen.com/categories/UE4/)
     >
     > [UE4 - blog4](https://blog.ch-wind.com/)
     >
     > [UE4 - blog5](https://exkulo.github.io/categories/UE4/)
     >
     > [Gamedev Guide](https://bebylon.dev/ue4guide/)
     >
     > [UE4 - blog6](https://blog.csdn.net/qq_36409711/category_6964420.html)
     >
     > [UE4 - blog7](https://dawnarc.com/tags/ue4/)
     
  4. ue4 packing & 热更新资源
  
     > [浅析创建自定义资源](https://zhuanlan.zhihu.com/p/77784922)
     >
     > [UE4补丁与DLC](https://blog.ch-wind.com/ue4-patch-release-dlc/)
     >
     > [github - Michael Delva - UE4Packager](https://github.com/TheEmidee/UE4Packager)
     >
     > [github - jashking - UnrealPakViewer](https://github.com/jashking/UnrealPakViewer)
     >
     > [Aery的UE4 C++游戏开发之旅（4）加载资源&创建对象](https://www.cnblogs.com/KillerAery/p/12031057.html)
     >
     > 
     >
     > [UE4对象系统_UObject&UClass](https://www.jianshu.com/p/1f2de6ea383c)
     >
     > [《InsideUE4》UObject（一）开篇](https://zhuanlan.zhihu.com/p/24319968)
     >
     > 
     >
     > [UE4 热更新：基于 UnLua 的 Lua 编程指南](https://imzlp.com/posts/36659/)
     >
     > [Lua热更新框架差异](https://chenanbao.github.io/2018/07/30/Lua%E7%83%AD%E6%9B%B4%E6%96%B0%E6%A1%86%E6%9E%B6%E5%B7%AE%E5%BC%82/)
     >
     > [github - tencent - sluaunreal](https://github.com/Tencent/sluaunreal)
     >
     > 
     >
     > [【UE4】GamePlay框架初步研究(附项目)](https://zhuanlan.zhihu.com/p/139286878)
     >
     > 
     >
     > [FLYING TREE - UE4 blog](https://wangjie.rocks/categories/UnrealEngine4/)
  
  5. Plugins
  
     [github - UE4Bugly](https://github.com/jashking/UE4Bugly)
  
     [github - unlua](https://github.com/Tencent/UnLua)
  
  6. MacOS 环境
  
     [github - ue4-xcode-vscode-mac](https://github.com/botman99/ue4-xcode-vscode-mac)
  
  7. 自学步骤
  
     ``` text
     https://www.bilibili.com/video/BV1et411b73Z?p=1
     【零基础】黑马C++教程，1到83课 （推荐学习时长：7天）
     
     https://github.com/19PDP/Bilibili-plus
     侯捷C++面向对象上、C++面向对象下、和C++标准库（推荐学习时长：7天）
     
     黑马C++教程的机房管理系统和演讲比赛系统选一个做（推荐学习时长：1天）
     
     
     
     https://www.bilibili.com/video/BV164411Y732?from=search&seid=1363697695672523050
     谌嘉诚的UE4初学者系列教程合集，P1到P9（推荐学习时长：0.5天）
     
     
     https://www.bilibili.com/video/BV1kt411k7mF?p=1
     http://www.sikiedu.com/course/514/tasks
     SiKi学院的飞机大作战（推荐学习时长：1.5天）
     
     https://docs.unrealengine.com/zh-CN/index.html
     先玩法 、 UI  （推荐学习时长：1天）
     再渲染
     
     https://www.youtube.com/watch?v=r4tltrLLVuQ&list=PLZlv_N0_O1gZalvQWYs8sc7RP_-8eSr3i&index=2
     UE4 UMG (虚幻示意图形界面设计器（Unreal Motion Graphics UI Designer）)  （推荐学习时长：1天）
     
     
     https://www.youtube.com/watch?v=DywBqQtTHMo&list=PLL0cLF8gjBprG6487lxqSq-aEo6ZXLDLg&index=2
     FPS蓝图项目 Creating A First Person Shooter Game 。（推荐学习时长：5天）
     改造成C++项目
     
     http://www.sikiedu.com/course/294/tasks
     Unreal中级案例-RPG游戏开发（推荐学习时长：10天）
     
     https://www.youtube.com/watch?v=DywBqQtTHMo&list=PLL0cLF8gjBprG6487lxqSq-aEo6ZXLDLg&index=2
     FPS蓝图项目 Creating A First Person Shooter Game    改造成C++项目
     https://www.youtube.com/watch?v=zEcNn4gWas0&list=PL3gCaTLUSAUsHG2BzsAs-HIeP08DyWtHh
     【免费】Unreal Engine C++ Tutorial加上FPS蓝图项目改成C++实现。（推荐学习时长：10天）
     
     扩展：
     材质系统、过场动画、行为树
     
     
     
     https://www.bilibili.com/video/av86357013/
     https://www.bilibili.com/video/BV1CV411U7jy
     https://www.bilibili.com/video/BV14K411J7v2
     https://www.bilibili.com/video/BV18V411Y7KR/
     https://www.bilibili.com/video/BV125411h7c4
     https://www.bilibili.com/video/BV1Ly4y1q7UG/?spm_id_from=333.788.b_636f6d6d656e74.6
     
     https://space.bilibili.com/138827797?from=search&seid=3007263283936567819
     ```
  
     



* Lua
  
  1. Unity和Lua 如龙之谷
  
* C / C++

  1. 侯捷教程

     [github - Bilibili-plus](https://github.com/19PDP/Bilibili-plus)

  2. C++    学习的项目demo https://github.com/miloyip/json-tutorial

     > [C++ 语言构造参考手册](https://www.bookstack.cn/read/cppreference-language/01b6abd9b6a13eeb.md)
     >
     > [C++ Tutorial](https://www.cprogramming.com/tutorial/c++-tutorial.html?inl=nv)
     >
     > [C++模板的偏特化与全特化](https://harttle.land/2015/10/03/cpp-template.html)
     >
     > [Google C++ Style Guide](https://google.github.io/styleguide/cppguide.html)
     >
     > [Google 开源项目风格指南](https://zh-google-styleguide.readthedocs.io/en/latest/google-cpp-styleguide/)
     >
     > 
     >
     > [C++博客园](http://www.cppblog.com/)
     >
     > 
     >
     > [【UE4源代码观察】可视化所有模块的依赖情况](https://blog.csdn.net/u013412391/article/details/104419789)
     >
     > [UE4 项目的设计规范和代码标准](https://imzlp.com/posts/25915/)
     >
     > [cplusplus 教程](https://www.cplusplus.com/)
     >
     > [News, Status & Discussion about Standard C++](https://isocpp.org/)
     >
     > [C++ reference](https://en.cppreference.com/w/cpp)
     >
     > [Microsoft C++ language documentation](https://docs.microsoft.com/en-us/cpp/cpp/?view=msvc-160)

  3. clang

     [LLVM & Clang 入门](https://github.com/CYBoys/Blogs/blob/master/LLVM_Clang/LLVM%20%26%20Clang%20%E5%85%A5%E9%97%A8.md)

  4. blog

     [c++知识点](https://www.cnblogs.com/eilearn/category/1210518.html)






* Project学习

  吃鸡demo  关卡编辑器 

  事件系统 （网络 (EventRouter) / UI(UIEvent (UIEventSystem)  vs.  UIEventNew (UIEventManager)) / 战斗(BeEvent vs. BeEventHandleNew) ...）
  
  ``` c#
  //UIEventHandleNew 中会添加以下代码 
  if(m_stackLevel > Global.TriggerSingleEventStackLevelLimit)
              {
                  if (m_stackLevel <= Global.MaxStackLevelLogLimit)
                  {
                      Logger.LogErrorFormat("UIEventHandleNew SendUIEvent id {0} DoFunc out of Recurse stack Level {1}", eventType, m_stackLevel);
                  }
              }
  ```
  
  
  
  吃鸡demo 查传送带 是 根据场景格子 添加到机制格子里
  
  
  
  战斗文档整理导出
  
  
  
* 设计模式

* 数据结构

  - [ ] c++ : 写一个泛型的数据结构：例如，线性表，数组，链表，二叉树  （写一个可以在不同数据结构、不同的元素类型上工作的泛型函数，例如求和）

    

* 算法

* Confluence笔记整理 （SDK组、战斗、系统、引擎、工具）

* 之前工作笔记整理

* 收藏资源整理（钉钉、微信）

* github上本地和远端的仓库整理，star整理

* 程序基础技术

  ``` text
  程序语言： 原理、编程范式、设计模式、代码设计、类库
  系统：计算机原理、操作系统、网络协议、数据库
  中间件：消息队列、缓存、网关、代理
  理论知识：算法、数据结构、系统架构、分布式
  ```



---



### 基础

* 进制转换

  ``` text
  二进制（B） - 十进制（D）  (101011)B => 1 * 2^0 + 1 * 2^1 + ...
  八进制（O） - 十进制      (53)O => 3 * 8^0 + 5 * 8^1
  十六进制（H） - 十进制    (2B)H => B * 16 + 2 * 16^1
  
  
  
  十进制 - 二进制（除2取余）
  十进制 - 八进制（除8取余）、（先十 - 二，再二 - 八）
  十进制 - 十六进制 （除16取余）
  
  
  
  二进制 - 八进制(取3舍1)  (11010111.0100111)B => {0}11 010 111 . 010 011 1{00} => (327.234)O
  					 (1100011100)B => {00}1 100 011 100 => (1434)O
  二进制 - 十六进制
  
  八进制 - 十六进制  (327)O => 011 010 111 => {0} 1101 0111 => (D7)H
  十六进制 - 八进制
  
  十六进制 - 十进制 
  
  包含小数的进制转换  （ABC.8C）H => 10 * 16^2 + 11 * 16^1 + 12 * 16^0 _ 8 * 16^-1 + 12 * 16^-2
  
  byte a = 0x65 => 0110 0101 => 5 * 16^0 + 6 * 16 ^1 = 101
  ```

  



---



### blog读后感



[[万字干货]Unity游戏工程师求职面试指南](https://mp.weixin.qq.com/s/ovBvVEhB3IFX5NzrK8Tlgw)

* 笔记结构

  |      大纲      |                           内容细分                           |
  | :------------: | :----------------------------------------------------------: |
  |     Unity      | API使用、资源管理、Editor（编辑器扩展）、UI（UGUI研究）、Shader、Plugins、功能实现、玩法实现、算法实现、性能优化 |
  |    安卓开发    |                                                              |
  |    编程语言    |                                                              |
  |      工作      |                                                              |
  |   计算机基础   |                                                              |
  |    软件使用    |                                                              |
  | 算法和数据结构 |                                                              |

  ![](.\0001.jpg)

* 工作中的注意点

  不能只专注自己的工作范围，需要看同事代码，学他们的技术，也可以知道优点和缺点，取长补短，也为之后改Bug更得心应手

  看同事代码会持续较长时间，目的是对整个项目的架构清楚，对项目核心技术点完全掌握

  掌握分三阶段：（也是面试时考察项目经验的主要思路）

  知道怎么做？

  知道为什么这么做？

  知道为什么不那么做？

  如：带预测回滚的ECS p2p帧同步方案

  Q：

  ``` text
  如何实现帧同步？
  为什么使用帧同步？优点？缺点？难点？
  为什么使用p2p不用cs架构？
  预测回滚是怎么做的？为什么要用预测回滚？除了预测回滚外还有什么方式可以实现不卡顿的同步？
  为什么用ECS？
  网络波动时怎么处理？掉线又怎么处理？
  ```

  ![](.\0002.jpg)

  Q：手游热门用lua写

  ``` text
  为什么用lua？lua跟其他热更方案比，有什么优缺？（比如比较lua和ILRuntime）
  用的是xlua还是tolua还是其他方案或者自己写的？理由是？-
  lua和c#之间是怎么交互的？
  使用lua需要注意什么问题？
  ```

  

* 扩展学习

  如果你想往上走，想升职加薪，你就要从比你目前的职位更高的角度去看待问题。

  一定要利用下班时间自己找学习资料学习，图形学、引擎底层、性能优化等都是不错的方向。

* 基础巩固

  一般问完项目经验，会考察基础知识的掌握情况

  ``` text
  语言基础
  计算机组成原理
  算法数据结构
  图形学基础（这个是因为我简历上有写，如果没写可能不会问）
  网络协议
  ```

  基础知识常固定形式

  ``` text
  语言基础：引用类型和值类型，GC算法，字典的实现原理，Lua和C#交互原理（《Lua程序设计》第24章到第28章），为什么要拆箱装箱，堆和栈。
  计算机组成原理：虚拟内存和物理内存，动态链接库和静态链接库，内存对齐，浮点数表示，多线程。（这些基础的东西大厂比较爱考）
  算法数据结构：topk问题，排序，红黑树和B树（这个很偏了，不会也没有关系，但是二叉搜索树还是得会的），数组和链表复杂度分析，用栈实现队列
  图形学：渲染管线，空间变换矩阵，shader优化
  网络协议：比较TCP和UDP，为什么TCP要三次握手及为什么四次挥手，如何实现可靠有序UDP（TCP和UDP几乎是必考的）。
  ```

* 学习资料

  ``` text
  书籍：-
  《CLR via C#》
  《Lua程序设计》
  《计算机组成原理》
  《剑指offer》
  《DirectX 9.0 3D游戏开发编程基础》
  《Unity Shader入门精要》
  
  视频&博客
  极客学院·算法面试通关40讲（付费，总结了大量经典算法数据结构题，建议全部看完并练习）
  极客学院·趣谈网络协议 （可以免费白嫖4篇文章，建议看TCP和UDP的就够了）
  极客学院·计算机组成原理（付费，没有时间看《计算机组成原理》的可以看这个，应付面试足以）
  哔哩哔哩·侯捷C++入门
  哔哩哔哩·侯捷C++ STL分析
  哔哩哔哩·侯捷C++内存管理
  JAVA并发和多线程·英文版（这个和C#都是想通的）
  JAVA并发和多编程·中文版
  拜托，面试别再问我TopK了
  超详细十大经典排序算法总结（堆排、快排比较重要）
  垃圾回收算法详解
  ```

* 简历编写

  ``` text
  简历内容控制在1到2页，使用pdf格式，注意排版美观。要有条理分12345，不要挤在一块很难看
  有博客或者开源项目的话要放上去
  没用的东西不要写，没人关心你的兴趣爱好和自我评价
  对于你的工作内容，需要言简意赅。完全由你负责的写“负责”，只参与一部分工作写“参与”（这样就算问得特别细你也可以说这部分不是你做的）
  简历上最好要有你的技术亮点
  比如使用了xxx算法、xxx数据结构、xxx设计模式、xxx架构
  实现了xxx，带来了xxx好处或者解决了xxx问题
  ```

  ![](.\0003.jpg)





---



### Java TODO

- [ ] 32bit和64bit的区别？   

- [ ] 如8位系统，两个整数相加会溢出，如何解决？   （7bit）

- [ ] 进程的五个状态（创建、就绪、阻塞、结束、等待）

- [ ] 进程和线程的区别（线程是最小的进度单元）

- [ ] 进程调度算法？

- [ ] 进程间的通信？（共享内存、管道、消息、信号量、套接字socket）

- [ ] 程序奔溃的解释？程序申请的1G内存，还能用吗？

- [ ] 死循环的程序，为什么操作系统还没有卡死？（时间片轮转等）

- [ ] 内存屏障？

- [ ] 锁、乐观锁（CAS，效率高）、悲观锁

- [ ] Java class文件从加载到结束的过程 （加载、验证、准备、解析、初始化）

- [ ] Java 双亲委托

- [ ] Java JVM，JVM引用（强弱软虚）

- [ ] JVM调优 （GC）

- [ ] GC

- [ ] 内存泄漏，如何检测？

- [ ] Java JIT (just in time)  java是解释型语言和编译型语言 (强类型语言)

- [ ] 设计模式（发布订阅）（如何做到 随机取消订阅，插入订阅，但需要保证订阅者有先后顺序）（发布者维护一个列表，存储订阅者，并给每个订阅者添加先后顺序编号）

- [ ] 数据结构（栈如何扩容）

- [ ] 栈溢出的原因（方法递归过深，并且栈容量有限，65535  固定大小，）

- [ ] hashmap hash的实现 原理

- [ ] hash取模运算 （桶）（对谁取模、如何确定放到哪个桶） （取模即取余）

- [ ] 取模运算的应用（循环队列、hash如何计算）

- [ ] websocket  tcp   （计算机网络 自顶向下的 方法）

- [ ] 多线程（并发和并行的区别）

- [ ] 线程池

- [ ] 算法- 排序 ：拓扑排序（图）

- [ ] 除new创建对象外，还有什么方法可以创建对象

  可以使用java反射

  反射创建对象过程：

  先查找类资源  再使用类加载器创建  

  反射机制：

  反射机制是在运行时，对于任意一个类，都能够知道这个类的所有属性和方法；对于任意个对象，都能够调用它的任意一个方法。在java中，只要给定类的名字，就可以通过反射机制来获得类的所有信息。 这种动态获取的信息以及动态调用对象的方法的功能称为Java语言的反射机制。

  举例：jdbc   Class.forName("com.mysql.jdbc.Driver.class")  //加载MySQL驱动类

  实现方式：

  1. Class.forName("类的路径")

  2. 类名.class

  3. 对象名.getClass()

  4. 基本类型的包装类，可以调用包装类的Type属性来获取该包装类的Class对象

  反射类：

  	1. Class 表示正在运行的Java应用程序中的类和接口   所有获取对象的信息都需要Class类来实现
   	2. Field: 提供有关类和接口的属性信息，以及对它的动态访问权限
   	3. Constructor: 提供关于类的单个构造方法的信息以及它的访问权限
   	4. Method：提供类或接口中某个方法的信息

  优点：

  	1. 能够运行时动态获取类的实例
   	2. 与动态编译结合

  缺点：

   	1. 性能较低，需要解析字节码 将内存中的对象进行解析

  优化：

  ​	1. 通过setAccessible(true)关闭JDK的安全检查来提升反射速度；

  ​	2. 多次创建一个类的实例时，有缓存会快很多

  ​	3. ReflflectASM工具类，通过字节码生成的方式加快反射速度

  ​	4. 相对不安全，破坏了封装性（因为通过反射可以获得私有方法和属性）

  反射API

  ​	Class 类：反射的核心类，可以获取类的属性，方法等信息。

  ​	Field 类：Java.lang.reflect 包中的类，表示类的成员变量，可以用来获取和设置类之中的属性值。

  ​	Method 类： Java.lang.reflect 包中的类，表示类的方法，它可以用来获取类中的方法信息或者执行方法。

  ​	Constructor 类： Java.lang.reflect 包中的类，表示类的构造方法。

  反射使用步骤（获取Class对象，调用对象方法）

  ​	获取想要操作的类的 Class 对象，他是反射的核心，通过 Class 对象我们可以任意调用类的方法。

  ​	调用 Class 类中的方法，即反射的使用阶段。

  ​	使用反射 API 来操作这些信息。

  反射动态创建

  ​	使用 Class 对象的 newInstance()方法来创建该 Class 对象对应类的实例，但是这种方法要求该 Class 对象对应的类有默认的空构造器。 

  ​	调用 Constructor 对象的 newInstance()

  ​			Class clazz = Class.forName("reflection.Person");

  ​			Person p = (Person)clazz.newInstance();

  ​	先使用 Class 对象获取指定的 Constructor 对象，再调用 Constructor 对象的 newInstance()方法来创建 Class 对象对应类的实例,

  ​	通过这种方法可以选定构造方法创建实例。

  ​			Constructor c = clazz.getDeclaredConstructor(String.class, String.class, int.class);

  ​			Person p1 = (Person) c.newInstance("张三", "male", 20);

- [ ] 

