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

  ref:

  ​	[Unity3D - 技术人生 (luzexi.com)](http://luzexi.com/tag/Unity3D/)

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

  14. Unity UGUI源码分析 

      [UNITY EDITOR V4.3.1F1 源码编译笔记](https://leafnsand.com/post/build_unity_from_source_code)

  15. Unity 游戏案例架构分析
  
      [Unity3D手游开发实践《腾讯桌球》客户端开发经验总结](https://www.gameres.com/654759.html)
  
      控制反转：[strangeioc](http://strangeioc.github.io/strangeioc/TheBigStrangeHowTo.html)







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

  * 安卓组件化开发

    ARouter原理分析：https://www.jianshu.com/p/bc4c34c6a06c

    



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
     
     [ 如何系统学习C++](https://mp.weixin.qq.com/s/WW_X12bTm94iaCgWBgYtJw)
     
     [C++那些事](https://light-city.club/sc/)
     
     [learncpp](https://www.learncpp.com/)




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

  |      大纲      | 内容细分                                                     |
  | :------------: | :----------------------------------------------------------- |
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



### Unity TODO

- [ ] FixedUpdate和Update的执行顺序和区别

  ``` tex
  Update: 每帧调用一次（非物理对象的移动，简单计时器，输入检测等）
  	​不是按固定时间调用的，如果某一帧和下一帧的处理时长不同，则Update的调用时间间隔不同
  
  FixedUpdate: 按固定时间调用，调用时间间隔相同（物理对象，如Rigidbody刚体应该在FixedUpdate中执行，最好使用力来定义对象移动）
  	​受Edit - Time - Fixed Timestep的值影响
  ```
  
  
  
- [ ] 项目开发经验

  1. 聊天，协议优化（channeltype聊天频道, targetid目标id, msg（消息内容，统一到msg中，处理为超链接））



**Unity优化-官方**：

[Fixing Performance Problems](https://learn.unity.com/tutorial/fixing-performance-problems?uv=5.x)

[Understanding optimization in Unity](https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity.html)



内存管理：

- [ ] GC的应用场景和触发大量GC的情况

  [Unity游戏的GC(garbage collection)优化](https://blog.csdn.net/znybn1/article/details/76464896)

  ``` tex
  GC概念：
  
   垃圾（Garbage ）是存储无用数据的内存的术语，
   GC（Garbage Collection）使这些内存可以再次使用的过程   （主要是对堆中的内存进行操作）
   
   GC引起的性能问题 一般表现为 帧率过低、帧率波动大、间歇性卡顿；（其他原因也可以引起这些问题）
   定位上述问题的方法：Unity Profiler 确认是否是GC引起的
   
   Unity对自己引擎代码 使用手动内存管理；对开发人员的脚本代码 使用自动内存管理
   
   Unity可访问两个内存池，栈和堆（也称为托管堆），栈：短期存储小块数据（栈对象）；堆（堆栈）：长期存储较大数据段（堆对象）
   栈内存在变量超出作用域时被编译器释放，堆内存需要主动释放（不释放时仍保持被分配状态）
   
   垃圾收集器（garbage collector）识别并释放未使用的堆内存，定期运行清理堆
   
   
  栈：工作方式类似于 数据结构栈， 栈内存块以严格的顺序添加和删除元素，创建变量申请内存时从栈顶分配内存空间，当变量超出作用域时，立即回收该变量占用的内存
  堆：分配和释放并不总是按可预测的顺序，并且需要的内存块大小也不同
  	检查堆上是否有足够的空间内存
  	如果空闲内存不足，触发GC尝试释放未使用的堆内存（GC耗时较高）
  	如果GC后空闲内存仍不足，将向操作系统申请更多的内存以扩大堆大小（耗时较高）
  	
  GC过程：
  （当堆变量超出作用域后，存储该变量的内存并没有立即释放
  无用的堆内存只在执行GC时被释放）
  （GC是耗时操作，当堆上的对象越多，代码中引用数越多，GC就越费时）
  垃圾收集器检索堆上的每个对象
  垃圾收集器搜索所有当前对象引用，以确定堆上对象是否仍在作用域内
  不在作用域内的对象被标记为待删除
  删除被标记的对象，并将其占用内存返回给堆
  
  
  GC触发：
  堆分配时堆上可用内存不足  （频繁地堆分配和释放会导致GC频繁）
  GC会不时地自动执行（平台而异）
  手动强制GC
  
  
  GC问题：
  GC耗时太长（堆上有大量对象和对象引用要检查，检查过程慢）导致游戏卡顿和运行缓慢
  GC触发时机不合适，当CPU在游戏性能关键部分已经满负荷，此时触发GC（即使少量）也会导致帧率下降和其他性能问题
  堆碎片化严重，当从不同大小的内存块回收资源时，堆内存中会出现大量分隔的小空闲块，即使此时内存可用总量大，但是由于不连续的内存块太多，导致GC触发或不得不扩大堆内存（导致的后果：游戏内存大小会远高于实际所需要的大小；GC会被更频繁地触发）
  
  
  
  堆分配查看：Unity Profiler
  CPU Usage -> GC alloc （表示当前帧分配信息）
  
  
  减少GC影响:
  减少GC执行时间（减少堆分配、对象引用数量）
  减少GC执行频率（减少堆分配、重新分配频率）
  主动触发GC（加载场景时，以避开游戏运行的性能关键点）
  
  
  
  策略：
  使用内存池，缓存池 ---> 减少堆分配和释放的频率，特别在一些关键的性能点上 （更少地触发GC，降低了堆碎片的问题）
  减少在频繁调用的函数内的创建临时对象；减少类似在Update这种频繁调用方法中分配堆内存
  模块化，主要的性能点代码需要保证独立性（如战斗城镇代码不要耦合太大）；
  缓存容器引用并在重复使用的地方使用Clear()方式
  ---> 减少堆分配（尽量分配在栈上，简单数据结构尽量用struct）和减少对象的引用 （当触发GC时，运行时间更少）
  主动触发GC（场景加载时）以及 扩展堆大小，使GC在可控并合适的时候触发
  
  减少不必要的堆分配：
  C#的字符串不可变，使用+运算符进行拼接时，会创建一个包含更新值的新字符串，丢弃老的，将产生垃圾
  	减少不必要的字符串创建，多次使用相同的字符串值，应该创建一次并缓存
  	减少不必要的字符串操作，如一个经常更新的Text组件，包含一个连接的字符串，可以考虑将他们拆分成两个
  	运行时构建字符串，应该使用StringBuilder类，可以创建没有堆分配的字符串，并且在连接复杂字符串时减少生成的垃圾量
  	不需要调试时，立即删除对Debug.Log()的调用，即使没有输出任何内容，对Debug.Log()的调用依然会被执行，调用Debug.Log()创建和处理至少一个字符串
  	
  Unity函数调用产生不可预知的垃圾
  	缓存函数的返回
  	降低调用函数的频率
  	重构代码以使用不同的函数
  	
      //Mesh.normals 内置函数  
      void ExampleFunction()
      {
          //每次循环，都会生成一个新的数组
          for (int i = 0; i < myMesh.normals.Length; i++)
          {
              Vector3 normal = myMesh.normals[i];
          }
  
          //修改
          Vector3[] meshNormals = myMesh.normals;
          for (int i = 0; i < meshNormals.Length; i++)
          {
              Vector3 normal = meshNormals[i];
          }
      }
      GameObject.CompareTag 替换 GameObject.tag
      Input.GetTouch()和Input.touchCount 替换 Input.touches
      Physics.ShpereCastNonAlloc()替换Physics.SphereCastAll()
      
  装箱
  	值类型被用作引用类型所执行的操作
  	Object.Equals(int) ---> 当值类型用了
      String.Format("{0}", int)
      装箱时，Unity在堆上创建了一个临时System.Object来包装值类型变量，当值类型变量释放时，Object会GC产生垃圾碎片
      
  协程
  	调用StartCoroutine会产生少量垃圾，因为Unity必须创建一些管理协程实例的类
  	在游戏交互或者性能热点时，限制对StartCoroutine()的调用，如果必须在性能热点中运行的协程，应该提前启动
  	注意  使用可能包含对StartCoroutine()延迟调用的嵌套协程，需要特别注意
  	yield本身不会产生堆分配，但是传递给yield的语句可能会产生不必要的堆分配
  	如果只需等待一帧而不产生堆分配，用yield return null 替换 yield return 0    int会被装箱
  	yield 中多次使用相同值时 使用了 new ， 应该缓存new的对象
          WaitForSeconds delay = new WaitForSeconds(1f);
          while (!isComplete)
          {
              yield return delay;
          }
      要灵活在协程和其他功能性实现上做切换（代码中可以有多种方法来实现需求和解决问题）
      主要使用协程来管理时间，可以在一些情况下使用Update()来实现
      主要用协程来控制游戏中发生的事件顺序，可以创建一种消息系统来允许对象通信
      
  5.5之前的Unity foreach会在循环开始并终止时，会分配一个Object在堆上
  	推荐都使用for 或者 while
  	
  函数引用
  	引用匿名函数还是命名函数，都属于引用类型变量，会产生堆分配
  	如果将匿名函数转换成闭包（匿名函数可以在其创建时访问范围中的变量）显著增加了内存使用量和堆分配数量
  	减少使用函数引用和闭包
  	
  LINQ和正则
  	会装箱产生垃圾
  	
  代码构建最小化GC的影响
  	Struct是值类型变量，如果其中包含了引用类型，GC Collector必须检查整个结构体，大量的类似结构体将增加大量的检查工作
  	public struct ItemData
  	{
      	public string name;  //引用类型！
      	public int cost;
      	public Vector3 position;
  	}
  	//修改，存成单独数组，GC Collector只需要检查字符串数组就行
  	private string[] itemNames;
  	private int[] itemCosts;
  	private Vector3[] itemPositions;
  	
  	不必要的对象引用
  	当垃圾收集器搜索对堆上对象的引用时，它必须检查代码中的每个当前对象引用。 更少的对象引用意味着更少的工作量，即使我们不减少堆上的对象总数。
  	public class DialogData
      {
          private DialogData nextDialog; //指向下一个对象
          public DialogData GetNextDialog()
          {
              return nextDialog;
          }
      }
      //修改
      public class DialogData
  	{
          private int nextDialogID; //使用id 来指代下一个对象
          public int GetNextDialogID()
          {
              return nextDialogID;
          }
  	}
  	
  强制GC
  	System.GC.Collect() --->  C#
  	已知堆内存已被分配但不再使用（例如，如果我们的代码在加载资源时生成垃圾），并且我们知道垃圾收集冻结不会影响播放器（例如，当加载界面还显示时）
  ```

  ``` c#
  /*
  禁用垃圾回收器 通过分配 GarbageCollector.Mode.Disabled 可以完全禁用垃圾回收器。这意味着垃圾回收器线程将永远不会停止您的应用程序来执行收集。此外，调用 System.GC.Collect() 将无效并且不会启动收集。禁用垃圾回收器必须非常小心，因为禁用垃圾回收器后的持续分配将导致内存使用量的持续增加。
  
  建议仅为长期的分配禁用垃圾回收器。例如，在游戏中，应该为一个关卡分配所有必需的内存，然后禁用垃圾回收器以避免关卡期间的开销。在关卡结束并释放所有内存之后，可以再次启用垃圾回收器，并可在加载下一关卡之前调用 System.GC.Collect() 来回收内存。
  */
  using System;
  using UnityEngine;
  using UnityEngine.Scripting;
  public class GarbageCollectorExample
  {
      static void ListenForGCModeChange()
      {
          // Listen on garbage collector mode changes.
          GarbageCollector.GCModeChanged += (GarbageCollector.Mode mode) =>
          {
              Debug.Log("GCModeChanged: " + mode);
          };
      }
      static void LogMode()
      {
          Debug.Log("GCMode: " + GarbageCollector.GCMode);
      }
      static void EnableGC()
      {
          GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
          // Trigger a collection to free memory.
          GC.Collect();
      }
      static void DisableGC()
      {
          GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
      }
  }
  ```

  

- [ ] StringBuilder 原理

  [StringBuilder GC 分析](https://zhuanlan.zhihu.com/p/337219219)

- [ ] 内存泄露  &  内存碎片

  [C++内存泄漏和内存碎片的产生及避免策略](https://blog.csdn.net/zzucsliang/article/details/43876173)

  ``` text
  常指堆内存泄露
  堆内存：程序从堆中分配，大小任意的（内存块的大小可以在程序运行期决定），使用完后必须显式释放的内存
  c++/c层内存泄露的描述：一般用malloc, realloc, new等函数从堆中分配到一块内存，使用完后，程序必须负责相应的调用free或delete释放改内存块，否则这块内存就不能再次使用
  
  
  内存泄露的后果：
  对游戏来说，发生内存泄露时，随着游戏的运行，内存消耗越来越高，当新内存频繁被分配时，发生内存不足，触发频繁GC，导致系统卡顿
  
  
  内存泄露的避免：
  保证在每个调用malloc/new等内存申请的操作位置，都有相应的free操作
  
  
  内存碎片：
  描述系统中所有不可用的空闲内存，不可用的是因为负责分配内存的分配器使这些内存无法使用，空闲内存以小而不连续的方式出现在不同位置
  
  如何避免内存碎片的产生：
  1. 少用动态内存分配的函数（尽量用栈空间）
  2. 分配内存和释放内存尽量在同一函数中
  3. 尽量一次性申请较大的内存，2的指数次幂的内存空间，不要反复申请小内存
  4. 使用内存池来减少使用堆内存引起的内存碎片，一次性申请一块足够大的空间
  5. 尽可能少地申请空间，尽量少使用堆上的内存空间
  ```

  

- [ ] 值类型和引用类型

- [ ] 装箱和拆箱

- [ ] 匿名函数（闭包）

- [ ] 如何避免加载大量资源时引起卡顿

- [ ] 多线程（Unity网络编程）

  ``` tex
  在不使用UnityEngine API的情况下（Debug.Log()也不行），可以使用多线程，提高多核CPU的使用率
  ```

- [ ] 编辑器扩展  案例

  教程：https://catlikecoding.com/unity/tutorials/editor/

  

  现有工程优化：

  活动配表 --> 扩展编辑器辅助 --> 降低出错

- [ ] lua结合

- [ ] UI开发上的经验

  ComUIList 的 原理

  复用代码和预制体、统一的组件、

- [ ] UGUI优化

  结合分享的文档

  结合具体开发案例，BossHP条 背包界面 

- [ ] Unity新内容 ECS等

- [ ] 内存优化的实例

  GC优化：见上

  资源优化：

  资源加载：

  

- [ ] Shader了解

  渲染管线：

  后处理：

- [ ] 寻路：

  Unity自带的NavMesh



C#:

- [ ] C#泛型

- [ ] C#值类型和引用类型的概念和场景

- [ ] 协程的概念和应用

  [C#协程](https://phepe.github.io/2019/04/03/Unity3d/Coroutine/)

  ``` text
  关键字 yield   IEnumerator   
  
  概念：
  主程序运行过程中，开启另一段逻辑处理，协同主程序执行
  只能单核按帧顺序轮转，不同于多线程的多核并行，不存在线程间的同步和互斥问题
  
  
  基本原理：
  协程返回值迭代器 为IEnumerator，可以执行一个序列的某个节点位置的指针
  IEnumerator提供了两个接口 Current返回当前指向元素，MoveNext将指针后移一个单位，成功移动返回true
  yield声明序列中的下一个值或者一个无意义的值
  yield return x（x可以是一个具体对象或者数值） MoveNext返回true并且Current被赋值为当前x，能够从当前位置继续往下执行
  如果 yield break MoveNext会返回false  中断迭代
  
  
  操作方法：
  开启
  StartCoroutine(string methodName)。参数是方法名(字符串类型)；此方法可以包含一个参数，形参方法可以有返回值。  不推荐
  Coroutine = StartCoroutine（IEnumerator method)。参数是方法名(TestMethod()),方法中可以包含多个参数；
  
  IEnumrator类型的方法不能含有ref或者out类型的参数，但可以含有被传递的引用；
  
  必须有有返回值，且返回值类型为IEnumrator, 
  返回值使用（yield retuen +表达式或者值，或者 yield break）语句。
  
  
  停止
  StopCoroutine (string methodName)，只能终止指定的协程,在程序中调用StopCoroutine() 方法只能终止以字符串形式启动的协程。
  StopCoroutine(Coroutine)，终止StartCoroutine返回的Coroutine
  StopAllCoroutine()，终止所有协程
  
  
  挂起
  yield：挂起，程序遇到yield关键字时会被挂起，暂停执行，等待条件满足时从当前位置继续执行
  yield return 0 or yield return null:程序在下一帧中从当前位置继续执行
  yield return 1,2,3,......: 程序等待1，2，3…帧之后从当前位置继续执行
  yield return new WaitForSeconds(n):程序等待n秒后从当前位置继续执行
  yield new WaitForEndOfFrame():在所有的渲染以及GUI程序执行完成后从当前位置继续执行
  yield new WaitForFixedUpdate():所有脚本中的FixedUpdate()函数都被执行后从当前位置继续执行
  yield return WWW:等待一个网络请求完成后从当前位置继续执行
  yield return StartCoroutine():等待一个协程执行完成后从当前位置继续执行
  yield break:将会导致协程的执行条件不被满足，不会从当前的位置继续执行程序，而是直接从当前位置跳出函数体，回到函数的根部
  ```

- [ ] 扩展： 异步编程   async / await    (阻塞和不阻塞)



- [ ] Mono虚拟机

  ``` text
  .Net Framework 微软跨平台基础框架，C#是运行在该框架下的编程语言
  基于通用语言基础架构 (Common Language Infrastructure， CLI)实现的
  
  CLI旨在统一 不同高级开发语言在不同平台直接执行方式的差异，描述可执行代码的是一种叫做CIL(Common Intermediate Language)介于高级语言和机器语言间的中间语言
  
  不同的高级开发语言统一成CIL通过不同平台CLR(Common Language Runtime)翻译成机器指令，实现统一
  
  
  .Net Framework包含：
  高级编程语言,比如c#,jscript,j#,VB
  公共语言规范（CLS）和公共类型系统（CTS）
  提供高级编程语言使用的各种类库(Framework Class Library)FCL和(Base Class Library)BCL
  Compiler高级编程语言编译器（高级编程语言->CIL）
  CLR（Common Language Runtime） IL语言的运行时
  
  
  
  Mono是基于.Net Framework框架下的跨平台实现
  主要包括：
  Runtime interpreter解释器(CLR)
  Class Libraries(.net framework各种类库)
  C# Compiler
  
  
  .net c# compiler
  作用把C#翻译成CIL中间语言
  早期编译器是csc.exe (c++实现)
  后期编译器是roslyn  (自举：c#编译器由c#实现)
  
  mono c# compiler
  按照CLI技术规范，能生成满足CIL中间语言，即可以各自实现c#编译器
  早期 gmcs smcs dmcs
  后期 mcs
  
  
  CLR 公共语言运行时
  mono: mono.exe
  .net framework  是以dll形式存在
  可以通过Clrver.exe查看当前程序对应的CLR版本
  
  
  unity中的mono
  Unity中存在两个Mono文件夹
  /Mono
  /MonoBleedingEdge 
  
  gmcs.exe --> Unity\Editor\Data\Mono\lib\mono\2.0 
  csc.exe  --> Unity\Editor\Data\Tools/Roslyn
  
  ```

  

- [ ] il2cpp的原理

- [ ] IL原理

- [ ] 特性的作用

  Unity编辑器自定义扩展编辑器  结合 ScriptObject 存取编辑器持久化的数据

- [ ] 反射 

  Singleton == > 创建单例类  

  结合特性 做 编辑器枚举类的中文展示等 

  

- [ ] 网络开发

  TCP / UDP

  聊天房间 demo （**继续Socket_2 Demo学习**）
  
  https://www.cnblogs.com/dolphinX/p/3462496.html
  
  断线重连



---



### 小游戏的开发 和 类型游戏的基本架构







---



### Java TODO

- [ ] 32bit和64bit的区别？   

- [ ] 如8位系统，两个整数相加会溢出，如何解决？   （7bit）

- [ ] 内存中的栈和堆

  

- [ ] 进程的五个状态（创建、就绪、阻塞、结束、等待）

  ``` tex
  阻塞：
  进程执行中，需要获取资源，但是此时该资源无法获取，则进程挂起直到满足可操作的条件再进行操作
  ```

  

- [ ] 进程和线程的区别（线程是CPU最小的调度单元，进程是最小资源单元）

  ``` tex
  线程是进程的实际运作单元，进程的基本单元，一个进程可以包含若干个线程，某个程序开始时执行时，进程的第一个线程被默认为该进程的主线程
  
  一个进程中可以创建多个线程，多线程之间是并发执行的，一个线程可以终止其他线程或者开启其他线程，每个线程可以共享数据，但都拥有自己独立的堆栈空间和执行顺序
  
  进程即正在执行的程序，（把编译好的指令放在特定的一块内存里顺序执行）；
  单线程程序：整个进程只有一个主线程，所有代码在这个线程内顺序执行
  多线程程序：一个进程有多个线程同时执行
  ```
  
  ``` tex
  Unity中的多线程应用
  
  
  ```
  
  



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

- [ ] GC （Java对象回收和创建）

  [Java对象创建与回收](https://www.pocketdigi.com/2020-12/java_object_create.html#more)

  ``` tex
  一、对象创建：
  
  Java类的加载过程：加载->验证->准备->解析->初始化
  new创建对象过程：1.虚拟机判断对应类是否加载完成 2.加载类或者创建对象
  (当类加载完成时，待创建对象所需的内存大小已经确定了)
  (为对象分配空间，即把一块确定大小的内存从堆中划分出来)
  
  
  内存划分：
  	指针碰撞(Bump the Pointer)
  		前提：垃圾收集器采用标记整理算法，Java堆中的内存是规整的，空闲和使用过的内存分在两边，用一个指针作为分界点，分配内存时把指针向空闲空间移动一段与对象大小相等的距离。
  	空闲列表(Free List)
  		垃圾收集器采用标记清除算法，Java堆中的内存不规整，使用过和空闲内存相互交错，虚拟机维护一个记录可用内存块的列表，分配内存时从表中找到一块足够代销的空间给对象，并更新列表
  		
  
  内存分配存在问题：
  	并发：多线程同时创建对象；同时使用相同的内存地址
  	虚拟机处理方式：
  		CAS(compare and swap)
  			虚拟机采用CAS + 失败重试 保证更新操作的原子性 来对分配内存空间的动作同步处理
  		本地线程分配缓冲(Thread Local Allocation Buffer, TLAB)
  			把内存分配的动作按照线程划分在不同的空间之中进行，即每个线程在Java堆中预先分配一小块内存。
  			XX:+/-UseTLAB 设置虚拟机是否启用TLAB （JVM默认会开启）
  			XX:TLABSize 设置TLAB大小
              
              
  初始化：（保证了对象的实例字段在Java代码中可以不赋初值就可以直接使用，程序访问的是这些字段数据类型对应的零值）
  	内存分配完成后，虚拟机需要将分配到内存空间都初始化为零值（不包括对象头）
  	使用TLAB，上述操作可以提前到TLAB分配时进行
  	
  	
  设置对象头（Object Header）：
  	初始化之后，虚拟机对对象进行必要设置，即设置对象头中的信息：(对象是哪个类的实例，如何才能找到类的元数据信息，对象Hash码，对象GC的分代年龄等)
  	HotSpot虚拟机，对象在内存中的存储布局为：对象头（Header）、实例数据（Instance Data）、对齐填充（Padding，提高内存管理效率）
  		其中对象头分为：
  		    1.存储对象自身的运行时数据（哈希码、GC分代年龄(只占4bit，年龄最大为15 2^4-1)、锁状态标志、线程持有锁、偏向线程ID、偏向时间戳）
  		    2.类型指针（Klass Pointer），对象指向它的类元数据的指针，虚拟机通过指针确定这个对象是哪个类的实例
  		    
  
  对象在栈上分配：
  	对象逃逸分析：分析对象动态作用域，当一个对象在方法中被定义后，它可能被外部方法所引用，例如作为调用参 数传递到其他地方中：
  	标量替换：前提通过逃逸分析确定对象不会被外部访问，然后将对象进一步分解，JVM不会创建该对象，将对象成员变量分解若干个被（标量替换）的方法所使用的成员变量（在栈帧和寄存器上分配空间）所替换
  			标量与聚合量：标量不能进一步分解，聚合量可被进一步分解
  			
  对象在Eden区分配
  大对象（需要大量连续内存空间的对象）直接进入老年代：为了避免为大对象分配内存时复制操作而降低效率
  长期存活的对象将进入老年代
  对象动态年龄判断
  老年代空间分配担保机制
  
  
  二、对象内存回收
  	判断对象是否需要回收
  	1. 引用计数
  		给对象添加一个引用计数器，引用计数为0的对象就不能再使用，但因为对象间循环引用的问题，这个方法一般不被采用
  	2. 可达性分析
  		将GC Roots对象（线程栈的本地变量、静态变量、本地方法栈的变量）作为起点，从这些节点向下搜索引用对象，找到对象标记为非垃圾对象，其余未标记对象为垃圾对象
  ```

  

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

  [大白话说Java反射：入门、使用、原理](https://www.cnblogs.com/chanshuyi/p/head_first_of_reflection.html)

  反射：运行时加载类，获取类的完整构造，并调用相应的方法

  可以使用java反射

  反射创建对象过程：

  ``` tex
  先查找类资源  再使用类加载器创建  
  ```

  反射机制：

  ``` tex
  反射机制是在运行时，对于任意一个类，都能够知道这个类的所有属性和方法；对于任意个对象，都能够调用它的任意一个方法。在java中，只要给定类的名字，就可以通过反射机制来获得类的所有信息。 这种动态获取的信息以及动态调用对象的方法的功能称为Java语言的反射机制。
  
  举例：jdbc   Class.forName("com.mysql.jdbc.Driver.class")  //加载MySQL驱动类
  ```

  实现方式：

  ``` tex
  1. Class.forName("类的路径")
  2. 类名.class
  3. 对象名.getClass()
  4. 基本类型的包装类，可以调用包装类的Type属性来获取该包装类的Class对象
  ```

  反射类：

  	1. Class 表示正在运行的Java应用程序中的类和接口   所有获取对象的信息都需要Class类来实现
  	2. Field: 提供有关类和接口的属性信息，以及对它的动态访问权限
  	3. Constructor: 提供关于类的单个构造方法的信息以及它的访问权限
  	4. Method：提供类或接口中某个方法的信息
  优点：

  	1. 能够运行时动态获取类的实例
  	2. 与动态编译结合
  缺点：

  ``` tex
  性能较低，需要解析字节码 将内存中的对象进行解析
  ```

  优化：

  ``` tex
  1. 通过setAccessible(true)关闭JDK的安全检查来提升反射速度；
  2. 多次创建一个类的实例时，有缓存会快很多
  3. ReflflectASM工具类，通过字节码生成的方式加快反射速度
  4. 相对不安全，破坏了封装性（因为通过反射可以获得私有方法和属性）
  ```

  反射API

  ``` tex
  Class 类：反射的核心类，可以获取类的属性，方法等信息。
  Field 类：Java.lang.reflect 包中的类，表示类的成员变量，可以用来获取和设置类之中的属性值。
  Method 类： Java.lang.reflect 包中的类，表示类的方法，它可以用来获取类中的方法信息或者执行方法。
  Constructor 类： Java.lang.reflect 包中的类，表示类的构造方法。
  ```

  反射使用步骤（获取Class对象，调用对象方法）

  ``` tex
  获取想要操作的类的 Class 对象，他是反射的核心，通过 Class 对象我们可以任意调用类的方法。
  调用 Class 类中的方法，即反射的使用阶段。
  使用反射 API 来操作这些信息。
  ```

  反射动态创建

  ``` java
  //使用 Class 对象的 newInstance()方法来创建该 Class 对象对应类的实例，但是这种方法要求该 Class 对象对应的类有默认的空构造器。 
  //调用 Constructor 对象的 newInstance()
  Class clazz = Class.forName("reflection.Person");
  Person p = (Person)clazz.newInstance();
  //先使用 Class 对象获取指定的 Constructor 对象，再调用 Constructor 对象的 newInstance()方法来创建 Class 对象对应类的实例,
  //通过这种方法可以选定构造方法创建实例。
  Constructor c = clazz.getDeclaredConstructor(String.class, String.class, int.class);
  Person p1 = (Person) c.newInstance("张三", "male", 20);
  ```

  

- [ ] 

