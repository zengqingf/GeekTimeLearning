# Unreal Engine 4 编码

### Windows API

* 操作注册表

  [UE4 操作windows注册表](https://zhuanlan.zhihu.com/p/139364795)

* 剪切板

  [UE4 C++ windows操作](https://blog.csdn.net/u014532636/article/details/99726677)





---



### 源码剖析&工具

* UBT (UnrealBuildTool) 编译工具

  [UE4 源码剖析 - 1.1.1 类型系统构建 - 编译系统(UBT之Generate)](https://zhuanlan.zhihu.com/p/157965866)

  [从源码剖析虚幻引擎编译原理](https://yinpengd.github.io/2019/08/13/%E4%BB%8E%E6%BA%90%E7%A0%81%E5%89%96%E6%9E%90%E8%99%9A%E5%B9%BB%E5%BC%95%E6%93%8E%E7%BC%96%E8%AF%91%E5%8E%9F%E7%90%86/)

  [理解UnrealBuildTool](https://zhuanlan.zhihu.com/p/57186557)

  [Build flow of the Unreal Engine4 project](https://imzlp.me/posts/6362/)

* UHT (UnrealHeaderTool)



* BuildGraph

  [UE4基础：一键出包脚本](https://wangjie.rocks/2019/01/22/ue4-basic-package/)

  [Ue4 编译系统BuildGraph](https://zhuanlan.zhihu.com/p/36075110)

  [UE4 安装式编译](https://wangjie.rocks/2017/04/13/ue4-installed-build/)



---



### 工具链

[UE4 工具链配置与开发技巧](https://imzlp.me/posts/12143/)



---



### 工程目录

[UE4 工具链配置与开发技巧](https://zhuanlan.zhihu.com/p/130452475)

``` text
防止 EngineAssociation 字段改动污染整个项目组

将同一个引擎的相关游戏工程都组织到一起，可以共享一个解决方案，方便做测试
```

[工程目录结构](https://zhuanlan.zhihu.com/p/160917246)

[UE4目录结构](https://muyunsoft.com/blog/Unreal4/Basic/DirectoryStructure.html#%E9%80%9A%E7%94%A8%E7%9B%AE%E5%BD%95)



---



### 版本管理

* git

  [Unreal4 Git版本管理](https://zhuanlan.zhihu.com/p/104197715)

  [虚幻4版本管理教程](https://zhuanlan.zhihu.com/p/103376639)

  [UE4工程规范](https://github.com/ericzhou9/ue4-style-guide#toc)

  [项目与资产文件管理](https://zhuanlan.zhihu.com/p/84217372)

  [UnrealEngine.gitignore](https://github.com/github/gitignore/blob/master/UnrealEngine.gitignore)

  [用Git版本管理Unreal4工程](https://zhuowl.github.io/2018/07/12/Unreal4-and-Git/)



---



### C++编码

* 教程

  [UE4 C++基础教程](https://www.zhihu.com/column/ue4cpp)



---



### Compile

* 加快编译

  1. Precompile header
  2. 多线程编译
  3. 分布式编译
  4. 减少依赖性
  5. 增量编译(Incremantal)

* 工具

  * incredibuild

  * fastbuild

    [github - note_fastbuild](https://github.com/sbfhy/note_fastbuild)

    [保姆式教你使用FASTBuild对UE4进行联机编译](https://zhuanlan.zhihu.com/p/158400394)

  * distcc

    [搭建Linux下的分布式编译系统](http://blog.xeonxu.info/blog/2012/08/30/da-jian-linuxxia-de-fen-bu-shi-bian-yi-xi-tong/)

    [使用distcc分布式编译加速Android NDK原生项目编译生成](https://blog.k-res.net/archives/1298.html)
  
    [github-distcc](https://github.com/distcc/distcc)
  
    [Android系统分布式编译(distcc)](https://blog.csdn.net/hui5110/article/details/107046883)
  
  * ccache
  
  * clcache
  
  * stashed
  
    for cache
  
  * 补充
  
    * UE4 开关 IncredBuild  /  Fastbuild 加速编译插件
  
      ``` text
      如果没有incredibuild服务器，这个功能开启的话会造成cpu编译的时候不用全力（离线），即使是本地一个小的修改也会编译几十秒，甚至上百秒，可以通过ue的配置文件强制关闭
      
      目录：
    \UnrealEngine\Engine\Programs\UnrealBuildTool\BuildConfiguration.xml
       C:\Users\<user>\Documents\Unreal Engine\UnrealBuildTool\BuildConfiguration.xml
    
      <bAllowXGE>false</bAllowXGE>
      <bAllowFASTBuild>false</bAllowFASTBuild>
      ```
  
    * 使用超线程
  
      ``` text
    目录：
      \UnrealEngine\Engine\Programs\UnrealBuildTool\BuildConfiguration.xml
     C:\Users\<user>\Documents\Unreal Engine\UnrealBuildTool\BuildConfiguration.xml
      
      添加<ProcessorCountMultiplier>2</ProcessorCountMultiplier>
      ```
  
    * 使用SSD链接
  
      ``` text
      正常编译的时候I/O也会成为你的瓶颈，如果没有足够的ssd空间存放引擎和工程可以通过一些骚操作达到类似效果，你只需要把生成的中间文件和源文件联接到SSD上即可
      
      cd UnrealEngine\Engine mklink /J Intermediate C:\UE4\Test\Intermediate mklink /J Source C:\UE4\Test\Source
      ```
  
* 预编译宏

  ``` text
  #pragma clang diagnostic ignored "-W<warning>"            ->  android ndk / ios xcode
  #pragma GCC diagnostic ignored "-W<warning>"			  ->  
  ```

* 编译器

  * clang

    [Clang 12 documentation](https://clang.llvm.org/docs/UsersManual.html)

  * gcc
  
  * llvm

* link

  [C++服务编译耗时优化原理及实践](https://tech.meituan.com/2020/12/10/apache-kylin-practice-in-meituan.html)



---



### ThirdParty

* 引擎内置

  [简单介绍 - Unity与UE4引擎源码内使用到的第三方库的比较](https://blog.csdn.net/u010019717/article/details/108113589)

* HotPatcher

  [UE4 资源热更打包工具 HotPatcher](https://imzlp.me/posts/17590/)

