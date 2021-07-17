# GCloud Dolphin腾讯游戏更新接入

[官方文档地址](https://sdk.gcloud.tencent.com/documents/details/%E6%B8%B8%E6%88%8F%E6%9B%B4%E6%96%B0%20Dolphin)



## 一、基本功能

1. 整包（程序）更新

   形式：启动时检查并下载最新版本程序包后进行覆盖安装

   * 差异更新

     实现APP差异更新要求手机安装包的MD5和差异更新apk的MD5一致

   * 子渠道程序差异更新

     子渠道Wechat 程序版本1.0.0.0 可以获取到主渠道Main 程序版本2.0.0.0的差异包并更新

     

2. 资源更新

   形式：整包更新后下载当前整包版本对应的最新资源版本

   ​	**游戏资源版本不支持差异更新**

   * 全量更新

     上传资源版本时需要指定对应的程序版本，可以关联多个程序版本（但是资源版本无法进行）

     上传资源要使用原始资源打包.zip格式，使用提供的打包工具

   * 增量更新（非diff）

     GCloud游戏资源更新机制是**只更新新增和变更的资源文件**

     上传时需要指定基线版本

     ``` tex
     文件格式：仅支持 zip 文件，最大不超过 4 GB
     
     每次打包的资源必须是游戏对应的全量资源，在更新的过程中，dolphin的sdk会自动和玩家本地的资源对比，下载差异资源
     以打包到zip文件中的单个文件为单位做差异，所以单个资源文件不应过大，避免大文件少量数据区修改导致大文件下载
     资源更新只做文件替换和增加，不做文件删除，游戏应避免和删除相关的游戏逻辑
     ```

   * 资源秀姑

     进行资源修复时，需要把本地的资源版本号设置为老版本号（版本号不能为空），比资源版本线上需要修复到的版本号低。



3. 首包解压

   ``` tex
   首包解压说明
   首包解压是将打包在应用安装包中的资源解压到一个应用的磁盘目录，这样才能对这个目录中的资源进行资源更新，本更新方案的资源更新也是针对这个磁盘目录的，应用都从这个磁盘目录读取资源。
   
   首包解压的操作在用户初始化更新模块的时候指定，如果用户指定，会先解压资源之后在连接版本服务器获取更新
   
   打包资源成ifs，命名为x.png，打包到apk或ipa中
   初始化首包解压路径如何传：
   
   Android：apk://apkpath?ifsinapkpath
   apkpath：当前运行apk在手机上的绝对路径
   ifsinapkpath：x.png在apk中的路径
   示例：x.png在apk的assets目录下
   传入路径：apk:///data/app/x.apk?assets/x.png
   其他平台：游戏安装之后，在x.png在系统磁盘的绝对路径
   ```

   ``` tex
   首包解压分为几种类型
   
   全解压
   	UpdateInitType_FirstExtract_All 全解压的功能就是只提供首包解压IFS到相应的目录，不会有联网过程，解压完毕即回调成功结束流程。
   
   部分解压
   	UpdateInitType_FirstExtract_Part 部分解压的功能就是只解压指定的文件夹路径到相应的目录，用这个功能的时候，初始化必须设置需要解压的目录
   	needExtractList (多个文件夹之间可以用 “ ,|#”隔开，建议用‘，’就可以)
   	目前不支持指定文件名解压
   	
   解压修复
   	UpdateInitType_FirstExtract_Fix 解压修复功能应用场景比较特殊，主要应用在Android机型的这种情况
   	应用举例：
   		app v1.0 大包（包含完整资源，大小1.6G），不进行首包解压或只解压部分文件到sdcard（读取资源可以通过app内部和sdcard）
   		app v1.0 经过一系列资源更新，外部sdcard累计了400M最新版本资源
   		app v2.0 小包（200M）, 大包app v1.0需要更新到app v2.0
   		默认情况下，更新后app v1.0内部的一些资源就会丢失，需要重新下载1.2G资源
   		使用解压修复的话，先下载小包，再进行解压修复（把大包内部的ifs解压补齐外部资源，不会覆盖外部资源），修复完成后，拉起小包安装
   		
   	首包全解压-资源修复
   		UpdateInitType_SourceCheckAndSync
   	首包部分解压-资源修复
   		UpdateInitType_SourceCheckAndSync_Optimize_Part
   	首包不解压-资源修复
   		UpdateInitType_SourceCheckAndSync_Optimize_Full
   	散资源修复
   ```

   



* 升级概念

  * 基础

    * 基于上传程序包或者资源时所设置的版本号

    * 设置版本号格式为四位点分十进制，形式x.x.x.x，x对应一个数字（short长度），属于更新服务控制台上//创建的程序版本号集合，参照下面的版本号说明，**@注意：不用和市场以及包内版本号一致，自行管理**

      **游戏资源版本号不能与应用程序版本号重复**

      ``` tex
      版本号说明：
      更新时，需要游戏当前的版本号（资源或程序），初始化时传入。
      这里的版本号与更新控制台创建的版本号对应。比如游戏第一个程序版本，在控制台上创建第一个版本，版本号为0.0.0.1，0.0.0.1程序版本运行时初始化更新，这里就要传入0.0.0.1。当在控制台上创建0.0.0.2版本后，0.0.0.1版本就可以获取到0.0.0.2的更新，更新之后就变成了0.0.0.2版本，这时再运行时初始化更新，这里就需要传入0.0.0.2
      ```

    * 一般会存在一个程序包对应多个资源包的情况，如1.0.1.2000的程序版本可以有1.0.2001、1.0.2002的资源版本

      

  * 更新可选

    不存在指定版本设置强制更新，由源版本（请求版本）和线上所有版本的属性决定

    * 正式版本

      包含两个用户属性，相互独立

      * 针对普通用户

        当前版本对普通用户开放，设置为普通用户不可用时，需要强制更新才能进入游戏

      * 针对灰度用户

        当前版本对灰度用户开放，设置为灰度用户不可用时，需要强制更新才能进入游戏

        ``` tex
        灰度使用场景
        灰度版本即在版本发布时，先选取小规模用户测试新版本，验证后再全量发布。
        
        灰度策略
        百分比灰度：即选取一定比例的用户作为灰度用户。具体为：将 OpenID Hash 成整型（使用 Bernstein's hash, 即 djb2/times 33），再对 100 取模，若指定 10% 的用户为灰度用户，则余数为 0~9 的用户为灰度用户（10% 也可以定制为非 0~9 的用户）。
        地区灰度：选取某些地区的用户作为灰度用户，其中地区由用户 IP 决定。此外，地区也可以指定灰度比例。
        白名单策略：使用白名单方式提供灰度用户ID，在白名单中的即为灰度用户。
        区服策略：用区服id来判断用户是否是灰度用户，客户端上报用户选择的区服id，配置的区服id就是灰度条件，在这个集合内就是灰度用户。
        ```

        

    * 审核版本

      ``` tex
      审核版本用于新版本提交渠道商（如 Apple App Store）审核时使用，其意义有：
      
      审核版本对外网用户不可见，即外网低版本不会升级到高版本的审核版本
      渠道商体验时不会被回退到外网最高版本
      如果版本号在 gcloud 上不存在或不可用，则检查更新时会返回最高可用版本的强制更新，所以提交审核的版本在 gcloud 上一定要存在，但是可用版本对外网用户是可见的，因此，不能是普通可用或灰度可用版本，所以，才有了“审核”属性。
      
      可以将商店地址配置在控制台 上传界面的自定义字段中（userData）
      ```

      ``` tex
      程序审核版本的第一个资源版本一定要设置成“普通可用”
      
      如果1.0.0.0的程序版本包里不包含资源，则资源更新是会传入0.0.0.0的资源版本号，由于资源版本线上没有普通可用的资源版本，则会返回“找不到版本”的错误。
      ```

      

* 升级限制

  * 版本线

    1. 程序更新只能走程序版本线

    2. 资源更新只能走所在程序版本下的资源线

       如存在程序版本2.x.x.x的一系列资源版本和3.x.x.x的一些列资源版本，如（本地程序包2.0.0.0+2.1.0.0的资源包）进行资源版本检查时，只会获取到2.3.0.0这个资源包的更新

       

  * 版本可用到强制更新

    * 将源版本（请求版本）设置为当前用户不可用或者不存在，且所在版本线上存在可用的更高版本，则返回强制更新到最高版本

    * 若源版本（请求版本）存在且设置为当前用户可用，同时所在版本线上存在可用的更高版本，则启动更新时返回可选更新到最高版本

      **（@注意：如上配置，如果版本是灰度更新则返回不更新）**

    * 若源版本就是最高可用版本，则返回不更新

  

  * 程序更新

    * 差异更新

      不支持上传V2签名的android包

      不支持Diff版本的签名方式不同的差异生成，即Diff版本的签名也应该时V1签名，才能解包对比二进制

    * 首次上传

      **@注意：在创建 安卓程序 版本后（iOS不需要），需要创建一个全量的资源版本，否则客户端更新失败，可以上传一个空的zip文件，空zip文件请按empty.zip/a.txt 组织。其中，a.txt为空文件。**

    * 支持平台

      只支持Android，不支持iOS

      iOS特殊处理

      ``` tex
      void OnDolphinVersionInfo(GCloud::dolphinVersionInfo& newVersinInfo)
      
      //程序更新不针对ios的程序下载更新，可以通过我们的更新模块检测新版本，
      //通过页面创建版本时，将appstore对应的下载地址填写在版本创建的用户自定义字符串部分；
      //发布新版本后，会回调OnDolphinVersionInfo，调用CancelUpdate退出更新，再自行跳转到appstore下载更新。
      ```

      

    





##  二、游戏客户端更新流程

* android 7 程序更新覆盖安装 （旧版本GCloud SDK）

  ```
  Android安装7.0兼容说明
  Android7.0强制启用了被称作 StrictMode的策略，带来的影响就是你的App对外无法暴露file://类型的URI了。 如果你使用Intent携带这样的URI去打开外部App(比如：通过url安装apk)，那么会抛出FileUriExposedException异常。
  
  修复：
  manifest修改 --- AndroidManifest.xml添加FileProvider
  path文件添加 --- res/xml目录下添加apollo_file_paths.xml
  
  AndroidManifest.xml添加FileProvider：
  <!-- 7.0 fileShare for targeSdkVersion>=24 注意:
      1. authorities这里格式为应用包名packageName+".ApolloFileprovider" 
      2. resource属性：这里需要定义apollo_file_paths.xml文件放到工程res/xml下面-->
  <provider
      android:name="android.support.v4.content.FileProvider"
      android:authorities="包名.ApolloFileprovider"
      android:exported="false"
      android:grantUriPermissions="true" >
      <meta-data
          android:name="android.support.FILE_PROVIDER_PATHS"
          android:resource="@xml/apollo_file_paths" />
  </provider>
  
  
  添加apollo_file_paths.xml文件到res/xml/目录下
  <?xml version="1.0" encoding="utf-8"?>  
  <paths>  
      <external-path path="." name="external_storage_root" />
      <files-path path="." name="file_patch_root" />
      <cache-path path="." name="cache_patch_root" />
  </paths>  

