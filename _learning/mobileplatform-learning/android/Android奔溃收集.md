# Android奔溃收集

* 工具和资源

  [Android 平台 Native 代码的崩溃捕获机制及实现](https://mp.weixin.qq.com/s/g-WzYF3wWAljok1XjPoo7w?)

  [google-breakpad](https://chromium.googlesource.com/breakpad/breakpad/+/master)
  
  [linux - Google Breakpad](https://docs.sentry.io/platforms/native/guides/breakpad/)
  
  [linux-Google Breakpad：脱离符号的调试工具](https://jackwish.net/2015/introduction-of-google-breakpad.html)



---



### 扩展iOS奔溃收集

[Google Breakpad IOS](https://blog.csdn.net/losemymind/article/details/39157715)

[Google Breakpad Android](https://blog.csdn.net/losemymind/article/details/39179201)

* breakpad

  ``` objective-c
  //测试：
  
  //打开google-breakpad-read-only/src/tools/mac/dump_syms/dump_syms.xcodeproj编译dump_syms工具，编译成功后记录下可执行文件的地址
  //打开已有的ios工程，把google-breakpad-read-only/src/client/ios/Breakpad.xcodeproj拖进工程，前提是该工程是workspace
  //在didFinishLaunchingWithOptions方法里加入
  [[BreakpadController sharedInstance]start: YES];
  //在applicationWillTerminate方法时加入
  [[BreakpadController sharedInstance]stop];
  //Info.plist
  /*BreakpadProduct
  BreakpadProductDisplay
  BreakpadURL*/
  
  //异常日志文件
  //当前应用的Library/Caches/Breakpad生成xxxxx.dmp文件
  
  //日志文件查看
  /*
  使用dump_syms生成sym文件
  使用head -n1 xxx.sym查看
  创建对应的目录
  使用minidump_stackwalk查看错误信息
  */
  ```

  
