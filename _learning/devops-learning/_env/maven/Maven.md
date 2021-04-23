# Maven

### Nexus搭建Maven私服

* nexus下载

  [下载地址](https://help.sonatype.com/repomanager3/download)



* 需要整理 搭建 - 手动上传 - 脚本上传 - 下载使用  （注明 软件版本和搭建环境）



[【Maven学习】如何上传jar或aar包至Maven私服，让Android项目可以支持Maven库在线依赖的方式来使用jar包](https://blog.csdn.net/ouyang_peng/article/details/90482592)

[使用Nexus搭建自己的Maven私服及上传下载aar](https://blog.csdn.net/u011424451/article/details/87378439)

[Android-Nexus 搭建自己的 Maven 仓库 & Gradle 上传依赖包](https://blog.csdn.net/qq_32452623/article/details/79385595)

[使用AS上传lib(module)包到Nexus私服（3.x）  iOS环境 ](https://www.jianshu.com/p/84e3da6b9cfd)

[Android中公共代码仓库与私服的使用](https://youngerdev.com/Android%E4%B8%AD%E5%85%AC%E5%85%B1%E4%BB%A3%E7%A0%81%E4%BB%93%E5%BA%93%E4%B8%8E%E7%A7%81%E6%9C%8D%E7%9A%84%E4%BD%BF%E7%94%A8.html)

[STUDIO(INTELLIJ)+GRADLE(1.0+)+JENKINS 打包&上传私服](https://www.cnblogs.com/lake19901126/p/4551380.html)



[Android-Nexus 搭建自己的 Maven 仓库 & Gradle 上传依赖包](https://blog.csdn.net/qq_32452623/article/details/79385595)

[Android搭建本地Maven私服 - 上传aar/jar - 并依赖使用](https://www.jianshu.com/p/5923528f9504)

[【Maven学习】如何上传jar或aar包至Maven私服，让Android项目可以支持Maven库在线依赖的方式来使用jar包](https://blog.csdn.net/ouyang_peng/article/details/90482592)

[Android 中打包成aar并上传到Nexus搭建的maven仓库](https://blog.csdn.net/jun5753/article/details/83864023)

[如何将Android studio中Library发布到私服Nexus仓库](https://programtip.com/zh/art-40240)

[使用Maven私服管理aar包](https://magicken.com/2019/11/12/put-my-aar-to-private-maven/)

[Android依赖管理与私服搭建](https://www.cnblogs.com/zyw-205520/p/6502183.html)

[使用Nexus Repository搭建属于自己公司的私有maven服务器](http://zmywly8866.github.io/2016/01/05/android-private-maven-repository.html)

[Anroid 搭建Maven私服(Android Studio)](https://my.oschina.net/u/4415286/blog/4004269)

[android 搭建maven私服管理类库](https://www.jianshu.com/p/1b48489eb23a)



[Maven私有仓库搭建和使用](https://www.cnblogs.com/zhangxh20/p/5671834.html)

[使用Nexus搭建maven私有仓库](https://www.jianshu.com/p/9740778b154f)

[官方- Repository Manager 3](https://help.sonatype.com/repomanager3)





[Android Studio上传项目到Maven仓库](https://www.jianshu.com/p/57f8af15ef9c)

[Android studio 配置使用maven](https://www.cnblogs.com/Jieth/p/8566012.html)





[Nexus在Windows与Linux上的安装步骤及注意事项和服务配置](https://zhuanlan.zhihu.com/p/30162290)





* 权限管理

  [第八节：Maven搭建Nexus私服与权限管理](https://blog.csdn.net/hellow__world/article/details/71855883)

  * bug

    ``` text
    Q: Received status code 401 from server: Unauthorized
    
    A: 进入设置界面，Administration -> Security -> Anonymous Access ， 选中Allow Anonymous Access，然后点击存。
    ```

    

* docker 搭建 nexus

  [Docker中Maven私服的搭建](https://www.cnblogs.com/niceyoo/p/11204143.html)

  [docker - sonatype/nexus3](https://hub.docker.com/r/sonatype/nexus3)

  ``` shell
  # 下载
  docker pull sonatype/nexus3
  # 挂载目录
  docker run -d -p 8081:8081 --name nexus -v /root/nexus-data:/var/nexus-data --restart=always sonatype/nexus3
  
  
  ```

  



---

* Gradle

  [在内网使用Gradle构建Android Studio项目](https://www.cnblogs.com/rainboy2010/p/7076509.html)



---

* jcenter

  [android 代码上传到jcenter](https://www.cnblogs.com/lixiangyang521/p/7760754.html)

  [上传 aar 到 jcenter 步骤详解](https://blog.csdn.net/a260724032/article/details/103700932)

  

  [Android Studio之maven Central，JCenter](https://yuanfentiank789.github.io/2017/08/30/Android-Studio%E4%B9%8Bmaven-Central-JCenter/)

  

---



* maven仓库

  > Maven中央仓库地址：https://repo1.maven.org/maven2/
  >
  > 阿里代理仓库地址：https://maven.aliyun.com/repository/public



---



* 教程

  [Maven高手系列](https://mp.weixin.qq.com/mp/appmsgalbum?__biz=MzA5MTkxMDQ4MQ==&action=getalbum&album_id=1318992648564424704&scene=173&from_msgid=2648933483&from_itemidx=1&count=3#wechat_redirect)

