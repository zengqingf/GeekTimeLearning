# Unity Crash

* link

  [Symbolicate Android crash](https://support.unity.com/hc/en-us/articles/115000292166-Symbolicate-Android-crash)

  ``` text
  android symbols
  
  MacOs:
  /Applications/Unity/PlaybackEngines/AndroidPlayer/Variations/mono/Release/Symbols
  
  Windows:
  Unity\Editor\Data\PlaybackEngines\AndroidPlayer\Variations\mono\Release\Symbols\armeabi-v7a\libunity.sym.so
  
  
  There are two possible library files to use here.
  
  Il2cpp.so: this file is generated when you build your project and is located on the symbols.zip file that is created next to the APK, abb or Gradle project.
  libunity.so: this file is located in the Unity installation folder: 
  <installation Folder>/PlaybackEngines/AndroidPlayer/Variations/il2cpp/Development/Symbols
  
  You have to use the symbols for the same architecture as the device where the call stack is taken: ARM64-v8a, ARMEABI-v7a or x86 (which is already deprecated).
  
  When you build using IL2CPP, if Strip Engine Code option is enabled, Unity creates a new libunity.so that it is different from the installation folder, so you need to use this for the symbolization.
  （使用il2cpp编译时，libunity.so会重新生成，使用时需要用这份）
  ```

  [Unity3D Android使用Bugly定位崩溃问题总结](https://blog.csdn.net/Sparrowfc/article/details/78384248)

  ``` text
  游戏的Crash引起原因分为这么几类：
  1.内存不足
  2.逻辑代码导致 
  3.Unity引擎自身问题 
  4.第三方库的自身问题
  
  一般来说Unity的crash基本上有这么几类:
  1.shader编译的平台适配问题
  2.跟资源加载相关的问题
  3.跟动画、粒子这种多线程处理模块相关的问题
  
  常用解决方法：
  1.shader问题的话基本上调整调整shader就能搞定
  2.剩下那些问题直接在google上搜索一下相关的堆栈信息或者日志提示信息基本在stackoverflow或者unity官方论坛上有相关的主题讨论，一般会有人提出一些workaround。
  3.另外就是浏览一下后面版本unity patch release发布说明里面的fixed项，看看有没有相关crash的修复。
  
  麻烦点：
  第三方库奔溃，由于没有符号表
  
  
  看Log定位崩溃原因时不要仅仅被红字所吸引，bugly应该是会把Error标成红色，也就是第四项为E的log，但是Error不等同于崩溃，还要具体看Error后面的信息，如果信息里包含什么SIG啊Abort啊crash这类的的才能说这一条Error是crash的点。但是如果你看到的Log类型是F，也就是Fatal，那肯定就属于系统崩溃日志了，当然也没准是无用信息
  
  一般来说光看崩溃信息是没有什么用的，标准的系统奔溃说明，只能用于定位系统奔溃点
  接下来需要 找相同 点
  运行时间（刚启动还是长时间运行）
  设备机型（是否是某一特定机型的兼容性问题）
  系统版本（是否是特定系统的兼容性问题）
  最重要的是，奔溃点之间的日志
  
  结合bugly 
  例如
  #unknown(1523)
  SIGABRT
  
  #00 pc 001b6a68 /data/app-lib/XXX/libmono.so (armeabi-v7a)
  
  由于#unknown 排除不是 UnityMain线程 即不是因为逻辑代码【直接】导致奔溃
  再看设备信息
  如果发现设备型号一致，还得看 CPU架构
  如果发现CPU架构是 x86  (可能包括了模拟器了)
  
  android studio日志格式
  时间 - 进程ID - 线程ID - 错误类型 - 具体信息
  
  
  
  为bugly提取unity 符号表  “libunity.sym.so”
  每升级一次unity版本，都要使用bugly提供的buglySymbolAndroid.jar工具来提取libunity.sym.so的符号表（包括debug/release、armeabi-v7a/x86）。只有development打包方式才可以看到报错的行号
  
  sym.so文件打成zip包传到bugly上就行，每个版本好像要重新传一次
  
  结合bugly提供的android 和 ios端的符号表提取并上传工具 自动化部署
  ```

  



---



* 闪退信息

  * VM aborting 关键字

    [关于AttachCurrentThread和DetachCurrentThread的故事](https://blog.csdn.net/wangchenggggdn/article/details/7819708)

    ``` c++
    //当在一个线程里面调用AttachCurrentThread后，如果不需要用的时候一定要DetachCurrentThread，否则线程无法正常退出。
    
    
    static JNIEnv *Adapter_GetEnv()
    {
    	int status;
    	JNIEnv *envnow = NULL;
    	status = (*g_JavaVM)->GetEnv(g_JavaVM,(void **) &envnow, JNI_VERSION_1_4);
    	if(status < 0)
    	{
    		status = (*g_JavaVM)->AttachCurrentThread(g_JavaVM,&envnow, NULL);
    		if(status < 0)
    		{
    			return NULL;
    		}
    		g_bAttatedT = TRUE;
    	}
    	return envnow;
    }
     
    static void DetachCurrent()
    {
    	if(g_bAttatedT)
    	{
    		(*g_JavaVM)->DetachCurrentThread(g_JavaVM);
    	}
    }
    
    /*
    07-24 15:02:23.874: DEBUG/dalvikvm(4932): threadid=9: thread exiting, not yet detached (count=0)
    07-24 15:02:23.874: DEBUG/dalvikvm(4932): threadid=9: thread exiting, not yet detached (count=1)
    07-24 15:02:23.874: ERROR/dalvikvm(4932): threadid=9: native thread exited without detaching
    07-24 15:02:23.874: ERROR/dalvikvm(4932): VM aborting
    */
    ```

    