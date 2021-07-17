# Maven

### Nexus搭建Maven私服

* link

  [使用Nexus搭建自己的Maven私服及上传下载aar](https://blog.csdn.net/u011424451/article/details/87378439)

  [Android-Nexus 搭建自己的 Maven 仓库 & Gradle 上传依赖包](https://blog.csdn.net/qq_32452623/article/details/79385595)

  [Android中公共代码仓库与私服的使用](https://youngerdev.com/Android%E4%B8%AD%E5%85%AC%E5%85%B1%E4%BB%A3%E7%A0%81%E4%BB%93%E5%BA%93%E4%B8%8E%E7%A7%81%E6%9C%8D%E7%9A%84%E4%BD%BF%E7%94%A8.html)

  



* nexus下载

  [下载地址](https://help.sonatype.com/repomanager3/download)
  
  [官方文档](https://help.sonatype.com/repomanager3)

* 需要整理 搭建 - 手动上传 - 脚本上传 - 下载使用  （注明 软件版本和搭建环境）

  ``` tex
  linux: 见docker搭建
  windows: 管理目录和启动批处理脚本
  ```

* 创建仓库

  ``` tex
  1、proxy
  代理类型，比如我们建立一个仓库来代理JCenter的仓库。
  2、hosted
  自己的主机仓库，一般放自己上传的lib。
  3、group
  仓库组，可以包含多个仓库，例如默认就有的仓库组maven-public。
  ```

  ``` tex
  如果需要上传本地自建jar、aar等，新建maven2(hosted)
  如果需要代理外网仓库，如JCenter，新建maven2(proxy)
  如果在Gradle中配置过多仓库地址，新建maven2(group)，可以指明过滤（Filter）和已包含（Members）的仓库
  ```

  ``` tex
  需要建立两个仓库
  release 和 snapshot 
  可以使用默认的 maven-releases 和 maven-snapshots (hosted类型)，也可以新建（注意命名和使用注意事项）
  
  
  @注意：一般针对游戏项目，搭建一套通用的使用默认release和snapshots仓库即可
  	同时为了引用方便，搭建一个maven2（group）供多个项目使用
  
  
  release库是存放稳定版本包的仓库，线上发布的程序都应从release库中引用正确版本进行使用。
  	release库仓库名中带有“releases”标识
  	release 库不允许删除版本
  	release 库不允许同版本覆盖
  	release库上传的jar包版本号（version）不能以“-SNAPSHOT”结束（版本号中的SNAPSHOT是release版和snapshot版区别的唯一标识）
  	第三方包（非公司内部开发）仅可引用 release 版
  	最好提供源码包 sources.jar 和方法文档 javadoc.jar，方便引用方使用
  	
  snapshot库是存放中间版本包的仓库，代表该库中依赖包的程序处于不稳定状态。当代码在开发过程中有其他程序需要引用时，可以提供snapshot版用于调试和测试。由于snapshot库的包依然处于测试状态，所以随时可以上传同版本最新包来替换旧包，基于这种不稳定状态，maven允许snapshot库中的包被编译时随时更新最新版，这就可能会导致每次打包编译时同一个版本jar会包含不同的内容，所以snapshot库中的包是不能用来发布的；
  	snapshot 库仓库名中带有“snapshots”标识
  	snapshot 库可以删除版本
  	snapshot 库可以实现版本覆盖
  	第三方包（非公司内部开发）不允许引用 snapshot 版
  	快照库上传的版本号（version）必须以“-SNAPSHOT”结束，并上传至私服后系统将自动将“-SNAPSHOT”替换为时间戳串
  	（本地代码引用时依然用“-SNAPSHOT”结束的版本号，无需替换时间戳），一个快照包线上将存在至少两个版本。
  ```

  

* 上传库

  ``` tex
  上传文件添加扩展名 如aar
  
  标识：GroupId : ArtifactId : Version
  对应：例如implemention com.android.support:support-v4:26.0.0
  
  @注意：release库不能同版本覆盖
  ```

  ``` tex
  pom.project {
                  version maven_pom_version
                  artifactId maven_pom_artifactId
                  groupId maven_pom_groupid
                  packaging maven_pom_packaging
                  description maven_pom_description
              }
  
  maven_local_url maven仓库中相应repository的地址
  maven_local_username 上传类库到仓库的用户名
  maven_local_password 上传类库到仓库的密码
  maven_pom_version 要上传的类库的版本号
  maven_pom_groupid 类库的分组标记，一般使用公司名或包名即可
  maven_pom_artifactId 类库的名称
  maven_pom_packaging 类库的格式可以支持 jar ，aar , so 等
  maven_pom_description 类库描述
  maven_pom_archives_file 类库文件在项目中的位置（相对于 build.gradle）
  #如果只需要上传项目编译时产生的 aar，artifacts 可以省略，因为 artifacts 默认就包含了编译产生的 aar 或 apk
  ```

  ``` tex
  // 进行数字签名
  signing {
      // 当 发布版本 & 存在"uploadArchives"任务时，才执行
      required { isReleaseBuild() && gradle.taskGraph.hasTask("uploadArchives") }
      sign configurations.archives
  }
  
  snapshot版是不需要进行数字签名的，但release必须数字签名。
  ```

  ``` tex
  问题解决：
  Q: Return code is: 405, ReasonPhrase: Method Not Allowed.
  A: @注意 maven2(group)类型仓库不能作为上传地址！！！
  ```

  

* 更新库

  ``` tex
  按需求增加maven_pom_version 
  执行gradle uploadArchives
  ```

  ``` tex
  获取最新版本
  
  dependencies {
      compile 'com.dboy:reader:1.+'
  }
  dependencies {
      compile 'com.dboy:reader:+'
  }
  ```

  

* 使用库

  ``` groovy
  //根build.gradle
  allprojects {
      repositories {
          ...
          maven { url 'http://xxx/repository/maven-public/' }
      }
  }
  
  //单独子库 build.gradle
  dependencies {
  	//...
  	implementation 'com.android.test:testaar:1.0.0@aar'
  }
  
  
  //本地依赖aar
  android {
      repositories {
          flatDir {
              dirs 'libs'
          }
      }
  }
  depenfencies {
      //...
      compile fileTree(include: ['*.jar', '*.aar'], dir: 'libs')
      compile (name:'xxx-release', ext:'aar')
  }
  ```

  ``` tex
  使用gradle引用snapshot库，若遇到已经上传最新版本，但是无法更新
  可尝试
  //Windows
  gradlew build --refresh-dependencies
  （手动删除后重新下载：C:\Users\(用户名)\.gradle\caches\modules-2\files-2.1）
  
  //Mac
  ./gradlew build --refresh-dependencies  
  （手动删除后重新下载：/Users/(用户名)/.gradle/caches/modules-2/files-2.1）
  ```

  



* 使用脚本上传

  maven_push.gradle

  ``` groovy
  //添加maven插件 以支持将项目发布到maven仓库
  apply plugin: 'maven'
  
  apply plugin: 'signing'
  
  // 判断 库版本  release or snapshot
  // VERSION_NAME后面加上 -SNAPSHOT
  def isReleaseBuild() {
      return !VERSION_NAME.toUpperCase().contains("SNAPSHOT")
  }
  
  //获取 仓库 账户名
  def getRepositoryAccount() {
      return hasProperty('NEXUS_ACC') ? NEXUS_ACC : ""
  }
  
  //获取 仓库 账号密码
  def getRepositoryPassword() {
      return hasProperty('NEXUS_PWD') ? NEXUS_PWD : ""
  }
  
  //获取 仓库 url
  def getRepositoryUrl() {
      return isReleaseBuild() ? RELEASE_URL : SNAPSHOT_URL
  }
  
  //配置阶段 评估完成后进入
  /*
  添加在评估此项目后立即调用的闭包。项目作为参数传递给闭包。
  当属于此项目的构建文件被执行时，这样的侦听器会得到通知。
  例如，父项目可以向其子项目添加这样的侦听器。
  这样的侦听器可以根据子项目的构建文件运行后的状态进一步配置这些子项目。
  */
  afterEvaluate { project ->
      uploadArchives {
          //configurations = configurations.archives
          repositories {
              mavenDeployer {
  
                  /* 弃用了
                  beforeDeplpoyment {
                      MavenDeployment deployment -> signing.signPom(deployment)
                  }*/
  
                  //配置在同级目录 gradle.properties中
                  //pom.groupId = GROUP_ID                                  //唯一标识（通常为模块包名）
                  //pom.artifactId = ARTIFACT_ID                            //项目名称（通常位类库模块名称）
                  //pom.version = VERSION_NAME                              //版本号
  
                  pom.project {
                      version VERSION_NAME
                      artifactId ARTIFACT_ID
                      groupId GROUP_ID
                      packaging  POM_PACKING_TYPE                              //打包下载上传格式
  
                      description POM_DESCRIPTION
                      name POM_NAME                                           //库名称
  
                      /*
                      licenses {
                          license {
                              name 'The Apache Software License, Version 2.0'
                              url 'http://www.apache.org/licenses/LICENSE-2.0.txt'
                          }
                      }*/
                  }
  
                  //指定正式版本 maven仓库url和账号密码
                  repository(url: getRepositoryUrl()) {
                      authentication(userName: getRepositoryAccount(), password: getRepositoryPassword())
                  }
              }
          }
      }
  
      /*  更多任务 对工程生成javadoc.jar、上传source.jar
      task androidJavadocs(type: Javadoc) {
          source = android.sourceSets.main.java.srcDirs
          classPath += project.files(android.getBootClasspath().join(File.pathSeparator))
      }
      task androidJavadocsJar(type: Jar, dependsOn: androidJavadocs) {
          classifier = 'javadoc'
          from androidJavadocs.destinationDir
      }
      task androidSourcesJar(type: Jar) {
          classifier = 'sources'
          from android.sourceSets.main.java.srcDirs
      }
      //解决 JavaDoc 中文注释生成失败的问题
      tasks.withType(Javadoc) {
          options.addStringOption('Xdoclint:none', '-quiet')
          options.addStringOption('encoding', 'UTF-8')
          options.addStringOption('charSet', 'UTF-8')
      }
      artifacts {
          archives androidSourcesJar
          archives androidJavadocsJar
      }*/
  }
  
  //运行这个会报错！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
  //配置需要上传到maven仓库的文件
  /*
  artifacts {
      archives file("release/TMUnityBridge.aar")
  }*/
  
  //snapshot版是不需要进行数字签名的，但release必须数字签名。
  //找不到
  signing {
      // 当  是发布版本 & 存在 uploadArchives task 时 才执行
      required { isReleaseBuild() && gradle.taskGraph.hasTask("uploadArchives") }
      sign configurations.archives
  }
  ```

  全局（根）gradle.properties  +  单独库的 gradle.properties

  ``` tex
  # For nexus maven sync #
  RELEASE_URL=http://192.168.2.200:8081/repository/maven-release-central/
  SNAPSHOT_URL=http://192.168.2.200:8081/repository/maven-snapshots/
  NEXUS_ACC=admin
  NEXUS_PWD=XXXXXXXXXXX
  POM_PACKING_TYPE=aar
  POM_DESCRIPTION = common-upload()
  POM_NAME=YYY
  
  
  
  VERSION_NAME=0.0.1
  GROUP_ID=com.zzz
  ARTIFACT_ID=ZZZ
  ```

  

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
  
  # 查看admin密码
  docker ps
  docker exec -it <<sonatype/nexus3对应的ContainerID>> /bin/sh
  sh-XXX$ cat nexus-data/admin.password
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

