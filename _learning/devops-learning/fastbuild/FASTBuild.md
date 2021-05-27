# FASTBuild

link:

[FASTBuild官网](https://www.fastbuild.org/docs/home.html)

[github - note_fastbuild](https://github.com/sbfhy/note_fastbuild)

[初识FASTBuild 一个大幅提升C/C++项目编译速度的分布式编译工具](https://www.cnblogs.com/tangxin-blog/p/8635438.html)

[保姆式教你使用FASTBuild对UE4进行联机编译](https://zhuanlan.zhihu.com/p/158400394)

[fastbuild进行ue4 shader连编](https://www.cnblogs.com/Shaojunping/p/11933626.html)

[fastbuild联编ue4 shader的使用](https://www.cnblogs.com/Shaojunping/p/11941712.html)

[UE4 用 fastbuild 实现并行编译 Shader](https://blog.kangkang.org/index.php/archives/409)

[Unreal Shader distributed build](https://github.com/fastbuild/fastbuild/issues/539)



---



* 测试环境

  ``` tex
  官网下载
  FASTBuild-Src-v1.04.zip
  FASTBuild-Windows-x64-v1.04.zip
  FASTBuild-Dashboard-master.zip
  
  Windows 10
  VS 2019
  
  unzip FASTBuild-Windows-x64-v1.04.zip
  添加解压后的目录到环境变量  
  运行cmd
  测试执行FBuild
  ```

  

* FastBuild源码编译步骤

  ``` tex
  unzip FASTBuild-Src-v1.04.zip
  cd FASTBuild-Src-v1.04/dist_v1.04/Code
  
  @注意： dist_v1.04/Code/fbuild.bff 这里的命名配置  用于后面生成可执行程序和解决方案（工程）
  
  FBuild All-x64-Release
  
  一般会遇到以下两个错误：
  解决1：修改dist_v1.04/SDK/VisualStudio/VS2019.bff
  			//
              // Failed
              //
              Print( '-----------------------------------------------------------------------' )
              Print( '- Unable to auto-detect VS2019 - please specify installation manually -' )
              Print( '-----------------------------------------------------------------------' )
              .VS2019_BasePath        = 'F:/_Dev/programs/Microsoft Visual Studio/2019/Community/VC' //.Set_Path_Here    // <-- Set path here
              .VS2019_Version         = '14.28.29910' //.Set_Version_Here // <-- Set version here
              
  解决2：修改dist_v1.04/External/SDK/Windows/Windows10SDK.bff
  // Root Paths
  //------------------------------------------------------------------------------
  .Windows10_SDKBasePath          = 'D:\Windows Kits/10'  //'$_CURRENT_BFF_DIR_$/10'
  .Windows10_SDKVersion           = '10.0.17763.0'
  #if CI_BUILD
      .Windows10_SDKBasePath      = 'D:\Windows Kits/10'  //'C:\Program Files (x86)\Windows Kits/10'
      .Windows10_SDKVersion       = '10.0.17763.0' 		//'10.0.17134.0'
  #endif
  ```

  ![](https://i.loli.net/2021/05/13/RayYLFHIhts9vEe.jpg)

  ![](https://i.loli.net/2021/05/13/lkJ3gA1UNe4nXaz.jpg)



* 修改FBuild源码

  ``` tex
  cd FASTBuild-Src-v1.04/dist_v1.04/Code
  生成sln
  FBuild solution
  ```

  