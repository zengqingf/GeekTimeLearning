# Android Studio 使用



[Android学习博客1](https://www.flysnow.org/categories/Android/)

[官网-探索 Android Studio](https://developer.android.com/studio/intro?hl=zh-CN)



## Gradle

* aar 和 jar 导出和使用

  [aar的使用（module或者library）](https://www.cnblogs.com/aimqqroad-13/p/8514274.html)
  
  [android studio 创建aar公共库笔记整理](https://www.jianshu.com/p/4f207868c986)
  
  [创建 Android 库](https://developer.android.com/studio/projects/android-library?hl=zh-cn)
  
  [android Studio如何打包和引用AAR](https://zhuanlan.zhihu.com/p/22242264)
  
  



* aar解包和组包

  [Android修改第三方.aar后重新打包](https://www.jianshu.com/p/f0a267551493)

  [二次打包（封装）AAR实用指南](https://zhuanlan.zhihu.com/p/31042358)

  [替换aar里面的资源文件](https://dltech21.github.io/2018/05/03/%E6%9B%BF%E6%8D%A2aar%E9%87%8C%E9%9D%A2%E7%9A%84%E8%B5%84%E6%BA%90%E6%96%87%E4%BB%B6/)



* 语法

  [Gradle User Manual](https://docs.gradle.org/current/userguide/userguide.html)

  [Gradle语法基础解析](https://www.cnblogs.com/sddai/p/10303978.html)

  [Gradle开发快速入门——DSL语法原理与常用API介绍](https://www.paincker.com/gradle-develop-basics#Groovy%E8%AF%AD%E8%A8%80%E7%AE%80%E4%BB%8B)

  [Android Gradle 看这一篇就够了](https://juejin.cn/post/6844903446814916621)

  [Gradle之build.gradle 语法案例](https://blog.csdn.net/shuizhihun07/article/details/80164460)

  [Gradle 入门--只此一篇](https://www.jianshu.com/p/001abe1d8e95)

  [Gradle 提示与诀窍](https://developer.android.com/studio/build/gradle-tips?hl=zh-cn)

  

* 应用

  [android gradle使用详解](https://www.jianshu.com/p/9467e1879093)
  
  [深入理解Android之Gradle](https://blog.csdn.net/innost/article/details/48228651)
  
  [写给 Android 开发者的 Gradle 系列（一）基本姿势](https://juejin.cn/post/6844903604642562061)
  
  [使用Gradle管理你的Android Studio工程](https://www.flysnow.org/2015/03/30/manage-your-android-project-with-gradle.html#%E5%89%8D%E8%A8%80)



* gradle简介

  ``` tex
  基于Groovy的动态DSL（领域特定语言（domain-specific language））
  
  
  Gradle从mavenCenter和JCenter中获取构件
  maven中央仓库（http://repo1.maven.org/maven2/）是由Sonatype公司提供的服务，它是Apache Maven、SBT和其他构建系统的默认仓库，并能很容易被Apache Ant/Ivy、Gradle和其他工具所使用。开源组织例如Apache软件基金会、Eclipse基金会、JBoss和很多个人开源项目都将构件发布到中央仓库。 maven中央仓库已经将内容浏览功能禁掉了，可在http://search.maven.org/查询构件。
  
  JCenter（https://jcenter.bintray.com ）是由JFrog公司提供的Bintray中的Java仓库。它是当前世界上最大的Java和Android开源软件构件仓库。 所有内容都通过内容分发网络（CDN）使用加密https连接获取。JCenter是Goovy Grape内的默认仓库，Gradle内建支持（jcenter()仓库），非常易于在（可能除了Maven之外的）其他构建工具内进行配置。
  
  JCenter相比mavenCenter构件更多，性能也更好。但还是有些构件仅存在mavenCenter中。
  ```

  * android gradle

    [Gradle for Android](http://wuxiaolong.me/2016/03/30/gradle4android1/)

    [Gradle Android Plugin中文手册](https://chaosleong.gitbooks.io/gradle-for-android/content/)

    [Gradle For Android](https://blog.csdn.net/xude1985/article/details/50833246)

    ``` tex
    apply plugin: ‘com.android.application’，表示该module是一个app module，应用了com.android.application插件，如果是一个android library，那么这里写apply plugin: ‘com.android.library’
    compileSdkVersion：基于哪个SDK编译，这里是API LEVEL
    buildToolsVersion：基于哪个构建工具版本进行构建的。
    defaultConfig：默认配置，如果没有其他的配置覆盖，就会使用这里的。
    applicationId：配置包名的
    versionCode：版本号
    versionName：版本名称
    buildTypes是构建类型，常用的有release和debug两种，可以在这里面启用混淆，启用zipAlign以及配置签名信息等。
    dependencies：不属于Android专有的配置了，它定义了该module需要依赖的jar，aar，jcenter库信息。
    ```

    ``` tex
    aar文件引入
    
    新建目录：app/aars
    修改文件：app/build.gradle
            android {
                //...
            }
            repositories {
                flatDir {
                    dirs 'aars' 
                }
            }
            dependencies {
                   compile(name:'libraryname', ext:'aar')
            }
    ```

    ``` tex
    自定义BuildConfig
    
    defaultConfig {
           buildConfigField 'Boolean','IsDebug','true'              
       }
    buildConfigField 一共有3个参数，第一个是数据类型，和Java的类型是对等的
    							 第二个参数是常量名
    							 第三个参数就是你要配置的值  
    							 三个参数都不能为空
    ```

    ``` tex
    启用混淆
    
    minifyEnabled为true表示启用混淆，proguardFile是混淆使用的配置文件，这里是module根目录下的proguard-rules.pro文件
    
    android {
        buildTypes {
            release {
                minifyEnabled true//是否启动混淆
    			shrinkResources true //是否移除无用资源文件，shrinkResources依赖于minifyEnabled，必须和minifyEnabled一起用
                proguardFiles getDefaultProguardFile('proguard-android.txt'), 'proguard-rules.pro'
            }
       }
    }
    ```

    ``` groovy
    //多渠道打包
    
    //flavors基础 ref: https://github.com/RyfThink/Android-Gradle-Multy-Flavor
    //示例：
    //基础配置，build后会出现两个apk
    productFlavors {
        googlePlay {
        }
        huaweiPlay {
        }
    }
    //多个包名
    productFlavors {
        googlePlay {
            applicationId 'com.mjx.mulityflavors_googlePlay'
        }
        huaweiPlay {
            applicationId 'com.mjx.mulityflavors_huaweiPlay'
        }
    }
    //多个BuildConfig
    productFlavors {
        googlePlay {
            buildConfigField "String","storeName","\"Google 应用商店\""
        }
        huaweiPlay {
            buildConfigField "String","storeName","\"HuaWei 应用商店\""
        }
    }
    //同时修改，循环
    productFlavors.all { flavor ->
        // replace all buildConfigField -> SotreName
        flavor.buildConfigField 'String', 'storeName', '"默认商店名"'
    }
    //多资源
    //Android Studio 的工程目录默认地将源代码和资源都放在了 src/main 文件夹下，在 main 的同级目录，也就是 src 文件夹下创建全路径文件
    /*
    src/googlePlay/res/values/strings.xml   googlePlay和上文保持一致
    <resources>
        <string name="app_name">Flavor: Google Play</string>
    </resources>
    
    src/huaweiPlay/res/values/strings.xml
    <resources>
        <string name="app_name">Flavor: HuaWei Store</string>
    </resources>
    使用
    getResources().getString(R.string.hello_flavor)
    不同的 flavor 构建成的 APK 运行起来打印出的结果便不同了
    */
    
    //占位符
     defaultConfig {
         // Placeholder
         manifestPlaceholders = [CHANNEL_VALUE: 'channel_testing']
     }
     
     productFlavors.all { flavor ->
         // replace all placeholders
         flavor.manifestPlaceholders.put("CHANNEL_VALUE", name)
     }
     
     productFlavors {
         googlePaly {
             ...
         }
         huaweiPlay {
             ...
             manifestPlaceholders.put("CHANNEL_VALUE", 'channel_huawei')
         }
     }
    
    //指定apk名输出， android同级
    def buildTime() {
         def date = new Date()
         def formattedDate = date.format('yyyy-MM-dd')
         return formattedDate
     }
    
    //可以全局
    applicationVariants.all { variant ->
         variant.outputs.each { output ->
             def parent = output.outputFile.parent;
             def apkName = "${variant.flavorName}_${variant.versionName}_${buildTime()}.apk"
             output.outputFile = new File(parent, apkName);
         }
     }
    //针对某个buildType配置
    buildTypes {
         release {
             ...
         }
         debug {
             applicationVariants.all { variant ->
                 variant.outputs.each { output ->
                     if (output.outputFile != null 
                     && output.outputFile.name.endsWith('.apk') 
                     &&'debug'.equals(variant.buildType.name){
                         def parent = output.outputFile.getParent();
                         def apkName = "${variant.flavorName}_${variant.versionName}_${buildTime()}.apk"
                         def apkFile = new File(parent,apkName)
                         output.outputFile = apkFile
                     }
                 }
             }
         }
     }
    
    //Mulity Source Codes
    //创建googlePaly/java/com/mjx/mulityflavors/Flavor.java
    package com.mjx.mulityflavors;
    public class Flavor {
     	public String getFlavorName () {
     		return "Hello Code : GooglePlay";
     	}
    }
    //创建huaweiPlay/java/com/mjx/mulityflavors/Flavor.java
    package com.mjx.mulityflavors;
    public class Flavor {
     	public String getFlavorName () {
     		return "Hello Code : HuaWeiPlay";
     	}
    }
    //MainActivity.java
    new Flavor().getFlavorName()
    
    -----------------------------------------------------------------------------------------------------------
    
    android {
        buildTypes{
            release{
                    //定制生成的apk文件名：gradle4android_v1.0_2021-09-23_xiaomi.apk
                    applicationVariants.all { variant ->
                     if (variant.buildType.name.equals('release')) {
                         variant.outputs.each { output ->
                             def outputFile = output.outputFile
                             if (outputFile != null && outputFile.name.endsWith('.apk')) {
                                 def fileName = 			"gradle4android_v${defaultConfig.versionName}_${releaseTime()}_${variant.flavorName}.apk"
                                 output.outputFile = new File(outputFile.parent, fileName)
                             }
                         }
                     }
                 }
                
                    productFlavors.all { flavor ->
                       manifestPlaceholders.put("UMENG_CHANNEL_VALUE",name)		//循环替换占位符
                   }
            }
        }
        
        productFlavors {//多渠道打包
            xiaomi {
                applicationId 'com.mjx.gradle4android1'
            }
            googlepaly {
                applicationId 'com.mjx.gradle4android2'
            }
    	}
    }
             
    def releaseTime() {
        return new Date().format("yyyy-MM-dd", TimeZone.getTimeZone("UTC"))
    }
    
    //androidmanifest.xml中的占位符替换
    <meta-data
               android:name="UMENG_CHANNEL"
               android:value="${UMENG_CHANNEL_VALUE}" />
    <meta-data
               android:name="is_debug"
               android:value="${is_debug}" />
    
    /*
    如果在productFlavors和buildTypes里面都进行了替换，那么是以productFlavors里面的为准。
    如果不区分productFlavors和buildTypes的话，也可以在defaultConfig里进行替换：
    */
    defaultConfig {
           manifestPlaceholders = [UMENG_CHANNEL_VALUE: 'dev']  //占位符替换为dev
           manifestPlaceholders = [is_debug: true]
       }
    //占位符使用
    boolean value = context.getPackageManager().
        getApplicationInfo(context.getPackageName(), PackageManager.Get_META_DATA).
        metaData.getBoolean(metadataName, defaultValue);
    ```

    ``` groovy
    //配置默认签名信息
    
    //方法1
    android {  
        signingConfigs {  
            debug {  
                storeFile file("mjx.keystore")
                storePassword 'android'
                keyAlias 'android'
                keyPassword '123456'
            }          
        }  	
    }
    
    //方法2
    android {  
        signingConfigs {  
            release {  
                storeFile file("mjx.keystore")
                storePassword '123456'
                keyAlias 'android'
                keyPassword '123456'
            }          
        }  
    	buildTypes {
            debug {
                signingConfig signingConfigs.release
            }
            release {
                //minifyEnabled false
                //proguardFiles getDefaultProguardFile('proguard-android.txt'), 'proguard-rules.pro'
                signingConfig signingConfigs.release
            }
        }
    }
    
    //命令行签名
    gradlew assembleRelease
    //output:
    xxx/app/build/outputs/apk/app-release.apk
    ```

    





* 修改gradle版本

  * ref

    [Android Studio 修改gradle版本](https://www.jianshu.com/p/23c4663ee326)

  基于Android Studio 3.5.3 & Windows 10

  1. 本地gradle路径位：

     ``` text
     C:\Users\用户名\.gradle\wrapper\dists
     ```

  2. gradle官网下载地址：[下载地址](https://services.gradle.org/distributions/)

  3. 本地手动添加gradle方式：

     ``` text
     官网下载 xxx-bin.zip 或者 xxx-all.zip 
     存放到 “C:\Users\用户名\.gradle\wrapper\dists”
     解压
     ```

     ![](E:\ws\mjx\base_learning\GeekTimeLearning\_learning\mobileplatform-learning\android\gradle-windows-本地目录新增版本.jpg)

     ![](E:\ws\mjx\base_learning\GeekTimeLearning\_learning\mobileplatform-learning\android\androidstduio配置本地gradle.png)

     使用gradle wrapper 在线下载

     使用local gradle distribution 使用本地下载 同时可以全局设置 Offline work 离线模式

  4. gradle wrapper 修改 distributionUrl

     ![](./androidstduio-gradle配置url.jpg)

  5. 配置gradle对应的插件，查询插件版本号

     https://jcenter.bintray.com/com/android/tools/build/gradle/

     如果最新的版本号是2.3.3 可以使用2.3.3+ 后续不需要再修改

     ![](./androidstudio-配置gradle插件版本.jpg)





* 修改module的packagename 

  ``` text
  每个Module都有一个AndroidManifest.xml 
  修改根节点的packagename 即可
  
  修改目录名
  Project面板选择右上角齿轮（设置），选择“Compact Empty Middle Packages”（紧凑），再右键对应文件夹，Refactor/Rename/...
  ```

  

* 把so加入到build后的aar中

  [How to Build *.so Library Files into AAR Bundle in Android Studio](https://www.dynamsoft.com/codepool/build-so-aar-android-studio.html)

  ``` text
  在对应module的src/main/下新建文件夹 jniLibs ，将so按cpu架构目录分别放入（armeabi-v7a / x86 ...）
  ```



* 导出jar并混淆

  [Android Studio导出Jar包并混淆](http://notes.stay4it.com/2016/02/26/export-jar-and-proguard/)

  ``` groovy
  task clearJar(type: Delete) {
      delete 'build/outputs/classes.jar'
  }
  
  task makeJar(type: proguard.gradle.ProGuardTask, dependsOn: "build") {
      // 未混淆的jar
      injars 'build/intermediates/bundles/release/    classes.jar'
      // 混淆后的jar路径
      outjars 'build/http.jar'
      // 具体需要keep住的类
      configuration 'proguard-rules.pro'
  }
  
  makeJar.dependsOn(clearJar, build)
  
  //在termial中输入 ./gradlew makeJar就能将classes.jar复制倒http.jar
  
  //如果module中同时也依赖其它libs，那需要在proguard中声明那些libs。比如：#-libraryjars libs\gson-2.2.2.jar，大部分都跟apk混淆类似的，只不过是局限于某个module而已。
  ```
  
  ``` xml
  proguard-rules.pro
  
  -libraryjars 'D:\softwave\android\sdk\platforms\android-26\android.jar'
  
  -optimizations !code/simplification/arithmetic
  -allowaccessmodification
  -repackageclasses ''
  -keepattributes *Annotation*
  -dontpreverify
  -dontwarn android.support.**
  
  -keep public class * extends android.app.Activity
  -keep public class * extends android.app.Application
  -keep public class * extends android.app.Service
  -keep public class * extends android.content.BroadcastReceiver
  -keep public class * extends android.content.ContentProvider
  -keep public class * extends android.view.View {
      public <init>(android.content.Context);
      public <init>(android.content.Context,android.util.AttributeSet);
      public <init>(android.content.Context,android.util.AttributeSet,int);
      public void set*(...);
  }
  
  -keepclasseswithmembers class * {
      public <init>(android.content.Context,android.util.AttributeSet);
  }
  -keepclasseswithmembers class * {
      public <init>(android.content.Context,android.util.AttributeSet,int);
  }
  -keepclassmembers class * extends android.content.Context {
      public void *(android.view.View);
      public void *(android.view.MenuItem);
  }
  -keepclassmembers class * extends android.os.Parcelable {
      static ** CREATOR;
  }
  -keepclassmembers class **.R$* {
      public static <fields>;
  }
  -keepclassmembers class * {
      @android.webkit.JavascriptInterface
      <methods>;
  }
  ```
  
  



* gradle删除没有用到的资源和代码

  ``` groovy
  //有效果，但是lib下的类还是会打进包里，同时，去除无效代码要依赖混淆，混淆是一个耗时操作
  android {
          buildTypes {
              release {					//debug也可以配置
                  minifyEnabled true
                  shrinkResources true   //是否去除无效的资源文件 依赖于minifyEnabled为true
                  
                  proguardFiles getDefaultProguardFile('proguard-android.txt'), 'proguard-rules.pro'
              }        
          }
      }
  ```

  

* 基础导入导出jar、aar、so

  ``` groovy
  //task to delete the old jar
  task deleteOldJar(type: Delete) {
      delete 'release/AndroidPlugin.jar'
  }
   
  //task to export contents as jar
  task exportJar(type: Copy) {
      from('build/intermediates/bundles/release/')
      into('release/')
      include('classes.jar')
      ///Rename the jar
      rename('classes.jar', 'AndroidPlugin.jar')
  }
   
  exportJar.dependsOn(deleteOldJar, build)
  
  //添加jar: 
  compile files('libs/mytool_1.0.jar')
  
  
  
  //生成aar
  //build/assembleRelease
  //build/outputs/aar/xxx.aar
  
  
  //添加aar:
  repositories {
      flatDir {
          dirs 'libs'			//找到aar的目录，相对于app
      }
  }
  compile(name: 'toollibrary-1.0', ext: 'aar')
  
  
  
  //添加so
  //新建src/main/jniLibs/armeabi/xxx.so
  ```

  



* 启用 MultiDex 

  [为方法数超过 64K 的应用启用 MultiDex ](https://developer.android.com/studio/build/multidex.html?hl=zh-CN)

  ``` groovy
  android {
      defaultConfig {
          ...
          minSdkVersion 15
          targetSdkVersion 28
          multiDexEnabled true    //基本步骤，还有其他额外操作
      }
      ...
  }
  
  dependencies {
      implementation "androidx.multidex:multidex:2.0.1"
  }
  ```




* 解决androidx 和 support库共存导致编译出错的问题

  ``` groovy
  /*
  ref: https://marspring.xyz/2021/03/31/UE-4-18%E5%8D%87%E7%BA%A7Androidx/
  
  背景：UE4.27打Android包，编译报错：
  		由于4.27引入google插件，同时引入了<import androidx.core.content.FileProvider;import androidx.core.app.NotificationManagerCompat;> 
  		导致下面报错：
          import android.support.v4.app.ActivityCompat;   //error:找不到ActivityCompat
          import android.support.v4.content.ContextCompat;//error:找不到content
  
  同一个项目中不能同时有androidx、support库，否则编译出错，有两种方案，要么升级，要么降级，综合评估了下，为了快速出demo，先将游戏support库升级到androidx，降级需要插件测改动的代码比较多。
  
  如果游戏所依赖的插件都是gradle 或者aar那好办，只需要打开android.useAndroidX=true 和android.enableJettifier=true即可，
  但由于Unreal 引擎的特殊性，有部分代码是在编译的时候通过编译工具动态生成的，这部分代码模板引入的库是support库。
  */
  
  
  			allprojects {
              def mappings = [
              'android.support.annotation': 'androidx.annotation',
              'android.arch.lifecycle': 'androidx.lifecycle',
              'android.support.v4.content.FileProvider':'androidx.core.content.FileProvider',
              'android.support.v4.app.NotificationManagerCompat':'androidx.core.app.NotificationManagerCompat',
              'android.support.v4.app.NotificationCompat': 'androidx.core.app.NotificationCompat',
              'android.support.v4.app.ActivityCompat': 'androidx.core.app.ActivityCompat',
              'android.support.v4.content.ContextCompat': 'androidx.core.content.ContextCompat',
              'android.support.v13.app.FragmentCompat': 'androidx.legacy.app.FragmentCompat',
              'android.arch.lifecycle.Lifecycle': 'androidx.lifecycle.Lifecycle',
              'android.arch.lifecycle.LifecycleObserver': 'androidx.lifecycle.LifecycleObserver',
              'android.arch.lifecycle.OnLifecycleEvent': 'androidx.lifecycle.OnLifecycleEvent',
              'android.arch.lifecycle.ProcessLifecycleOwner': 'androidx.lifecycle.ProcessLifecycleOwner',
              ]
  
              beforeEvaluate { project ->
              project.rootProject.projectDir.traverse(type: groovy.io.FileType.FILES, nameFilter: ~/.*\.java$/) { f ->
              mappings.each { entry ->
              if (f.getText('UTF-8').contains(entry.key)) {
              println "Updating ${entry.key} to ${entry.value} in file ${f}"
              ant.replace(file: f, token: entry.key, value: entry.value)
              }
              }
              }
              }
              }
  ```

  



---



### Ant

[Apache ANT](https://ant.apache.org/)





---



### Groovy

[Apache Groovy](https://groovy-lang.org/)