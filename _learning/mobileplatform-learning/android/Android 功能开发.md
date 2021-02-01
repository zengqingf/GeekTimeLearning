# Android 功能开发

* Fmod库

  [github - QQ变声效果](https://github.com/onestravel/QQVoiceChange)

* FFmpeg库

  [github - 视频播放](https://github.com/onestravel/FFmpegDemo)

* 获取apk签名信息

  [Android 获取 apk 签名信息](https://juejin.cn/post/6844903950605352967)
  
  [Android 使用代码获取签名信息](https://blog.csdn.net/hcwfc/article/details/41560591)
  
  [Android在代码中获取应用签名](https://www.cnblogs.com/shenchanghui/p/7910695.html)
  
  ``` java
      public static Signature[] getSignatures(Context context, String packageName){
          try {
              PackageManager manager = context.getPackageManager();
              if(manager != null) {
                  PackageInfo packageInfo = manager.getPackageInfo(packageName, PackageManager.GET_SIGNATURES);
                  return packageInfo.signatures;
              }
          }catch (PackageManager.NameNotFoundException e){
              e.printStackTrace();
          }
          return null;
      }
  ```
  
* 路径

  [google - 访问应用专属文件](https://developer.android.com/training/data-storage/app-specific?hl=zh-cn#java)

  [Adnroid文件存储路径getFilesDir()与getExternalFilesDir的区别](https://blog.csdn.net/losefrank/article/details/53464646)

  [github - All Android Directory Path](https://gist.github.com/lopspower/76421751b21594c69eb2)
  
* Logger

  [github - orhanobut / logger](https://github.com/orhanobut/logger)

  [github - michelzanini / android-logger](https://github.com/michelzanini/android-logger)

  [github - noveogroup / android-logger](https://github.com/noveogroup/android-logger)

  [github - klinker41 / android-logger](https://github.com/klinker41/android-logger)

  [github - elvishew / xLog](https://github.com/elvishew/xLog)

  [github - ech0s7r / androidlog](https://github.com/ech0s7r/androidlog)

  [github - savio-zc / Android-Logger](https://github.com/savio-zc/Android-Logger)

  [github - danylovolokh / AndroidLogger](https://github.com/danylovolokh/AndroidLogger)



---



### 库

* 支持库

  [概念 - 官方 - 支持库软件包](https://developer.android.com/topic/libraries/support-library/packages)

  [引用 - maven仓库](https://mvnrepository.com/artifact/com.android.support/support-v4)

  [下载 - Android Support Library Direct Link for Downloading [closed]](https://stackoverflow.com/questions/12518002/android-support-library-direct-link-for-downloading)

  ``` text
  In General this 概念 - 官方 - 支持库软件包 list all Android Support Library Just Choose your Library version and Use Google repository Url:
  
  https://dl-ssl.google.com/android/repository/
  
  e.g.
  for Android Support Library revision 19 :
  https://dl-ssl.google.com/android/repository/support_r19.zip
  
  for Android Support Library revision 19.0.1 :
  https://dl-ssl.google.com/android/repository/support_r19.0.1.zip
  
  for Android Support Library revision 19.1.0 :
  https://dl-ssl.google.com/android/repository/support_r19.1.zip
  
  The Oldest Android Support Library
  https://dl-ssl.google.com/android/repository/support_r04.zip
  ```

  