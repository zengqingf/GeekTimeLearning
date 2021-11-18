# Android 功能开发



### 基础

* V1 V2 V3签名

  [android应用签名](https://jakkypan.gitbooks.io/android-develop-art-discovery/content/an-quan-xing/ying-yong-qian-ming.html)

  [android-安全性-应用签名](https://source.android.google.cn/security/apksigning?hl=zh-cn)

  ``` tex
  V1方案，基于JAR签名的方案
  V2方案，Android 7.0引入的方案
  
  v1 签名不保护 APK 的某些部分，例如 ZIP 元数据。APK 验证程序需要处理大量不可信（尚未经过验证）的数据结构，然后会舍弃不受签名保护的数据。这会导致相当大的受攻击面。此外，APK 验证程序必须解压所有已压缩的条目，而这需要花费更多时间和内存。
  
  在验证期间，v2+ 方案会将 APK 文件视为 blob，并对整个文件进行签名检查。对 APK 进行的任何修改（包括对 ZIP 元数据进行的修改）都会使 APK 签名作废。这种形式的 APK 验证不仅速度要快得多，而且能够发现更多种未经授权的修改。
  
  新的签名格式向后兼容，因此，使用这种新格式签名的 APK 可在更低版本的 Android 设备上进行安装（会直接忽略添加到 APK 的额外数据），但前提是这些 APK 还带有 v1 签名。
  ```



* MultiDex

  [为方法数超过 64K 的应用启用 MultiDex](https://developer.android.com/studio/build/multidex)

  ``` tex
  示例：UE4 Android Build Gradle构建方法数超标
  
  UE4 Plugin APL.xml中加入
  <buildGradleAdditions>
      <insert>
      <![CDATA[
          android {
              defaultConfig {
                  multiDexEnabled true
              }
          }
      ]]>
      </insert>
  </buildGradleAdditions>
  
  
  使用MultiDex分包后 ClassNotFoundException
  问题原因：为 Dalvik 可执行文件分包构建每个 DEX 文件时，构建工具会执行复杂的决策制定来确定 MainDEX 文件中需要的类，以便应用能够成功启动。如果启动期间需要的任何类未在MainDEX 文件中提供，那么您的应用将崩溃并出现错误 java.lang.NoClassDefFoundError。
  
  build.gradle
  android {
      defaultConfig {
          multiDexEnabled true
          multiDexKeepProguard file('gcloudsdk-gcloudcore/multidex-config.pro')
      }
  }
  ```

  





* 混淆

  [android打包混淆及语法规则详解](https://blog.csdn.net/P876643136/article/details/90668769)

  ``` tex
  android {
      …………
      buildTypes {
          release {
              // 是否进行混淆
              minifyEnabled true
              // 混淆文件的位置
              proguardFiles getDefaultProguardFile('proguard-android.txt'), 'proguard-rules.pro'
          }
          debug {
              ···
          }
      }
  }
  
  遇到过将minifyEnabled false后可以解决 multiDexEnabled true导致的class not found问题 
  java.lang.RuntimeException: Unable to create application com.epicgames.ue4.GameApplication: java.lang.NullPointerException: Attempt to invoke interface method 'void b.g.b.d.a.c.a.b(android.content.Context)' on a null object reference
  ```

  



* 静默安装

  [Android应用静默安装实现方案](https://taolin.me/2020/06/05/android-install-app-silently/)

  * shell

  ``` shell
  # android 7.0以下   通过adb或者代码中调用，可以正常运行，apk路径必须是绝对路径
  pm install -r -d /sdcard/Download/demo.apk
  
  # android 7.0到android 9.0以下
  # shell
  pm install -r -d /sdcard/Download/demo.apk
  # android api （需要用-i参数指定一个系统进程来安装，如下com.android.settings是一个系统应用）
  pm install -r -d -i com.android.settings /sdcard/Download/demo.apk
  
  
  #uninstall
  pm uninstall packagename
  
  # android 9.0及以上
  # 源码：android9.0在应用安装流程中，首先进行了进程的识别，如果当前是shell进程，直接安装失败。
  ```

  * android api

  ``` java
  // android 9.0以下
  public static boolean silentInstall(PackageManager packageManager, String apkPath) {
      Class<?> pmClz = packageManager.getClass();
      try {
          if (Build.VERSION.SDK_INT >= 21) {
              Class<?> aClass = Class.forName("android.app.PackageInstallObserver");
              Constructor<?> constructor = aClass.getDeclaredConstructor();
              constructor.setAccessible(true);
              Object installObserver = constructor.newInstance();
              Method method = pmClz.getDeclaredMethod("installPackage", Uri.class, aClass, int.class, String.class);
              method.setAccessible(true);
              method.invoke(packageManager, Uri.fromFile(new File(apkPath)), installObserver, 2, null);
          } else {
              Method method = pmClz.getDeclaredMethod("installPackage", Uri.class, Class.forName("android.content.pm.IPackageInstallObserver"), int.class, String.class);
              method.setAccessible(true);
              method.invoke(packageManager, Uri.fromFile(new File(apkPath)), null, 2, null);
          }
          return true;
      } catch (Exception e) {
          e.printStackTrace();
      }
      return false;
  }
  
  // android 9.0以上
  public static boolean install(Context context, String apkPath) {
      PackageInstaller packageInstaller = context.getPackageManager().getPackageInstaller();
      SessionParams params = new SessionParams(SessionParams.MODE_FULL_INSTALL);
      String pkgName = getApkPackageName(context, apkPath);
      if (pkgName == null) {
          return false;
      }
      params.setAppPackageName(pkgName);
      try {
          Method allowDowngrade = SessionParams.class.getMethod("setAllowDowngrade", boolean.class);
          allowDowngrade.setAccessible(true);
          allowDowngrade.invoke(params, true);
      } catch (Exception e) {
          e.printStackTrace();
      }
      OutputStream os = null;
      InputStream is = null;
      try {
          int sessionId = packageInstaller.createSession(params);
          PackageInstaller.Session session = packageInstaller.openSession(sessionId);
          os = session.openWrite(pkgName, 0, -1);
          is = new FileInputStream(apkPath);
          byte[] buffer = new byte[1024];
          int len;
          while ((len = is.read(buffer)) != -1) {
              os.write(buffer, 0, len);
          }
          session.fsync(os);
          os.close();
          os = null;
          is.close();
          is = null;
          session.commit(PendingIntent.getBroadcast(context, sessionId,
                  new Intent(Intent.ACTION_MAIN), 0).getIntentSender());
      } catch (Exception e) {
          Logger.e("" + e.getMessage());
          return false;
      } finally {
          if (os != null) {
              try {
                  os.close();
              } catch (IOException e) {
                  e.printStackTrace();
              }
          }
          if (is != null) {
              try {
                  is.close();
              } catch (IOException e) {
                  e.printStackTrace();
              }
          }
      }
      return true;
  }
  /**
   * 获取apk的包名
   */
  public static String getApkPackageName(Context context, String apkPath) {
      PackageManager pm = context.getPackageManager();
      PackageInfo info = pm.getPackageArchiveInfo(apkPath, 0);
      if (info != null) {
          return info.packageName;
      } else {
          return null;
      }
  }
  ```

  

* 分享功能

  [github - Share2](https://github.com/baishixian/Share2)

  **以下代码在android 新版本上可能不支持  因为file provider**

  ``` tex
  Android安装7.0兼容说明
  Android7.0强制启用了被称作 StrictMode的策略，带来的影响就是你的App对外无法暴露file://类型的URI了。 如果你使用Intent携带这样的URI去打开外部App(比如：通过url安装apk)，那么会抛出FileUriExposedException异常。
  
  摘自腾讯GCloud Dolphin
  manifest修改 --- AndroidManifest.xml添加FileProvider
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
  
  path文件添加 --- res/xml目录下添加apollo_file_paths.xml
  <?xml version="1.0" encoding="utf-8"?>  
  <paths>  
      <external-path path="." name="external_storage_root" />
      <files-path path="." name="file_patch_root" />
      <cache-path path="." name="cache_patch_root" />
  </paths>  
  ```

  ``` c#
  using UnityEngine;
  using System.Collections;
  using System.IO;
  
  public class NativeShareScript : MonoBehaviour {
      public GameObject CanvasShareObj;
  
      private bool isProcessing = false;
      private bool isFocus = false;
  
      public void ShareBtnPress()
      {
          if (!isProcessing)
          {
              CanvasShareObj.SetActive(true);
              StartCoroutine(ShareScreenshot());
          }
      }
  
      IEnumerator ShareScreenshot()
      {
          isProcessing = true;
  
          yield return new WaitForEndOfFrame();
  
          Application.CaptureScreenshot("screenshot.png", 2);
          string destination = Path.Combine(Application.persistentDataPath, "screenshot.png");
  
          yield return new WaitForSecondsRealtime(0.3f);
  
          if (!Application.isEditor)
          {
              AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
              AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
              intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
              AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
              AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
              intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"),
                  uriObject);
              intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"),
                  "Can you beat my score?");
              intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
              AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
              AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
              AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser",
                  intentObject, "Share your new score");
              currentActivity.Call("startActivity", chooser);
  
              yield return new WaitForSecondsRealtime(1);
          }
  
          yield return new WaitUntil(() => isFocus);
          CanvasShareObj.SetActive(false);
          isProcessing = false;
      }
  
      private void OnApplicationFocus(bool focus)
      {
          isFocus = focus;
      }
  }
  ```

  

* 本地通知

  [Android Notification 详解](https://www.cnblogs.com/travellife/p/Android-Notification-xiang-jie.html)

  [Android Notification 详解（一）——基本操作](https://www.cnblogs.com/travellife/p/Android-Notification-xiang-jie-yiji-ben-cao-zuo.html)



* 设备保持常亮（唤醒）

  [使设备保持唤醒状态 ](https://developer.android.com/training/scheduling/wakelock.html)



---



### 插件

* Fmod库

  [github - QQ变声效果](https://github.com/onestravel/QQVoiceChange)

* FFmpeg库

  [github - 视频播放](https://github.com/onestravel/FFmpegDemo)

* Volley库

  [Android Volley框架（三）：解决Volley请求服务器返回中文乱码问题](https://blog.csdn.net/lvyoujt/article/details/50667638)

  ``` text
  解决Volley请求服务器返回中文乱码问题
  
  新建继承自 Request<String / JSONObject>
      @Override
      protected Response<String> parseNetworkResponse(NetworkResponse response) {
          String parsed;
          try {
              Log.e("###", response.headers.toString());    //如果是乱码 这里可能无法找到 Content-type key值
              parsed = new String(response.data, "UTF-8");  //根据服务器端文本编码格式来处理
          } catch (UnsupportedEncodingException e) {
              parsed = new String(response.data);
          }
          return Response.success(parsed, HttpHeaderParser.parseCacheHeaders(response));
      }
  ```

  



---



### API

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
  
  [Android系统目录结构](https://www.cnblogs.com/pixy/p/4744501.html)
  
  [【Android】解析Android的路径](https://www.cnblogs.com/HDK2016/p/8707866.html)
  
  ``` text
  区分内部存储和外部存储
  
  内部存储：
  	路径：/data/data/应用包名/files
  	方法：
  		读：
  		//判断文件是否存在
  		File file = context.getFileStreamPath(fileName);
  		file == null || !file.exists() //判断文件是否存在
  		
  		写：
  		读写：
  		openFileOutput()    其中context不需要MainActivity（针对Unity主Activity）
  		例：FileOutputStream fos = /*some context if outside activity.*/openFileOutput("hello.txt", Context.MODE_PRIVATE);
  
  外部存储：SDCard
  	路径：/storage/
  		公有目录：/storage/emulated/0/.
  		私有目录：/storage/emulated/0/Android/data/应用包名/
  	方法：
  		读：
  		Environmant.getExternalStorageState().equals(Environment.MEDIA_MOUNTED)  //判断是否有SDKCard并且是否可读写
  		Environment.getExernalStorageDirectory()
  		
  		//判断文件是否存在
  		Environment.getExernalStorageDirectory().getPath() + filePath
  		File f = new File(file);
  		f.exists()
  		
  		写：
  		读写：
  		
  	权限：
  		<!– 在SDCard中创建与删除文件权限 –>
  		<uses-permission android:name=”android.permission.MOUNT_UNMOUNT_FILESYSTEMS”/>
  		<!– 往SDCard写入数据权限 –>
  		<uses-permission android:name=”android.permission.WRITE_EXTERNAL_STORAGE”/>
  ```
  
  
  
* Logger

  [github - orhanobut / logger](https://github.com/orhanobut/logger)

  [github - michelzanini / android-logger](https://github.com/michelzanini/android-logger)

  [github - noveogroup / android-logger](https://github.com/noveogroup/android-logger)

  [github - klinker41 / android-logger](https://github.com/klinker41/android-logger)

  [github - elvishew / xLog](https://github.com/elvishew/xLog)

  [github - ech0s7r / androidlog](https://github.com/ech0s7r/androidlog)

  [github - savio-zc / Android-Logger](https://github.com/savio-zc/Android-Logger)

  [github - danylovolokh / AndroidLogger](https://github.com/danylovolokh/AndroidLogger)



* 获取屏幕点击坐标

  [Android 中的窗口坐标体系和屏幕的触控事件](https://henleylee.github.io/posts/2018/74e7e0b3.html)

  [android onTouch()与onTouchEvent()的区别](https://blog.csdn.net/guyuealian/article/details/51637033)

  ``` java
  //触控事件  MotionEvent
  // onTouch() vs onTouchEvent()
  
  //Activity 重载
        @Override
        public boolean onTouchEvent(MotionEvent event)
        {
            //@注意：这个判断也不能省  直接获取getRawX() 和 getRawY() 会重复几次
  			switch(event.getAction()) {
  				case MotionEvent.ACTION_DOWN:
  					nativeTouchScreenPos(event.getRawX(), event.getRawY());
                      //@注意：使用break 无法中断 会重复进入几次
  					return true;
  			}
  			return false;
        }
  /*
  event：参数event为手机屏幕触摸事件封装类的对象，其中封装了该事件的所有信息，例如触摸的位置、触摸的类型以及触摸的时间等。该对象会在用户触摸手机屏幕时被创建。
  
  返回值：该方法的返回值机理与键盘响应事件的相同，同样是当已经完整地处理了该事件且不希望其他回调方法再次处理时返回true，否则返回false。
  */
  
  
  //onTouch()是OnTouchListener接口的方法，它是获取某一个控件的触摸事件，因此使用时，必须使用setOnTouchListener绑定到控件，然后才能鉴定该控件的触摸事件。当一个View绑定了OnTouchLister后，当有touch事件触发时，就会调用onTouch方法。通过getAction()方法可以获取当前触摸事件的状态
  myImage.setOnTouchListener(new OnTouchListener() {
  	public boolean onTouch(View v, MotionEvent event) {
  				switch (event.getAction()) {//当前状态
  				case MotionEvent.ACTION_DOWN:
  					break;
  				case MotionEvent.ACTION_MOVE:
  					break;
  				case MotionEvent.ACTION_UP:
  					break;
  				default:
  					break;
  				}
  				return true;//还回为true,说明事件已经完成了，不会再被其他事件监听器调用
  			}
  	});
  
  /*
  1、如果setOnTouchListener中的onTouch方法返回值是true（事件被消费）时，则onTouchEvent方法将不会被执行；
  2、只有当setOnTouchListener中的onTouch方法返回值是false（事件未被消费，向下传递）时，onTouchEvent方法才被执行。
  3、以上说的情况适用于View对象（事件会最先被最内层的View对象先响应）而不是ViewGroup对象（事件会最先被最外层的View对象先响应）。
  综合来讲：
  onTouchListener的onTouch方法优先级比onTouchEvent高，会先触发。
  假如onTouch方法返回false，会接着触发onTouchEvent，反之onTouchEvent方法不会被调用。
  内置诸如click事件的实现等等都基于onTouchEvent，假如onTouch返回true，这些事件将不会被触发。
  */
  
  
  //return value
        @Override
        public boolean onTouch(View view, MotionEvent motionEvent) {
        switch(motionEvent.getAction()) {
        case  MotionEvent.ACTION_DOWN:
        nativeTouchScreenPos(motionEvent.getRawX(), motionEvent.getRawY());
        Log.debug("on Screen Touch");
        break;
        }
        //注意返回值
        //true：view继续响应Touch操作；
        //false：view不再响应Touch操作，故此处若为false，只能显示起始位置，不能显示实时位置和结束位置
        return true;
        }
  ```
  
  ![](api/android_screen_coord_1.jpg)
  
  ![android_screen_coord_2](api/android_screen_coord_2.jpg)
  
  ![android_screen_coord_3](api/android_screen_coord_3.jpg)



* 屏幕点击

  ``` java
  //获取四指点击（多指）
  	@Override
        public boolean onTouchEvent(MotionEvent event)
        {
        switch(event.getAction()) {
        //case MotionEvent.ACTION_DOWN:
        //nativeTouchScreenPos(event.getRawX(), event.getRawY());
  		  case MotionEvent.ACTION_DOWN:
  		  case MotionEvent.ACTION_POINTER_DOWN:
  			  isTouchBegin = true;
  			  bTriggerMultiTouchEvent = false;
  			  break;
  		  case MotionEvent.ACTION_UP:
  		  case MotionEvent.ACTION_POINTER_UP:
  		  case MotionEvent.ACTION_CANCEL:
  		  case MotionEvent.ACTION_OUTSIDE:
  		  	  isTouchBegin = false;
  		  	  break;
  	  }
  	  int pointerCount = event.getPointerCount();
  	  if(pointerCount >= multiPointerCount) {
          if(isTouchBegin) {
              if(!bTriggerMultiTouchEvent) {
                  nativeMultiTouchEvent();
                  bTriggerMultiTouchEvent = true;
              }
          }
  	  }
        return true;
        }
        public native void nativeMultiTouchEvent();
  	  private boolean isTouchBegin = false;
        private int multiPointerCount = 4;
        private boolean bTriggerMultiTouchEvent = false;
        public void setMultiPointerCount(int count){		//设置手指数目
  	  multiPointerCount = count;
  	  }
  ```

  ``` tex
  ref: https://developer.android.com/reference/android/view/MotionEvent.html
  ref: [Android触摸事件--MotionEvent](https://www.jianshu.com/p/7c40dece7b22)
  
  ACTION_DOWN vs. ACTION_POINTER_DOWN
  ACTION_UP vs. ACTION_POINTER_UP
  
  ACTION_DOWN is for the first finger that touches the screen. This starts the gesture. The pointer data for this finger is always at index 0 in the MotionEvent.
  ACTION_POINTER_DOWN is for extra fingers that enter the screen beyond the first. The pointer data for this finger is at the index returned by getActionIndex().
  ACTION_POINTER_UP is sent when a finger leaves the screen but at least one finger is still touching it. The last data sample about the finger that went up is at the index returned by getActionIndex().
  ACTION_UP is sent when the last finger leaves the screen. The last data sample about the finger that went up is at index 0. This ends the gesture.
  ACTION_CANCEL means the entire gesture was aborted for some reason. This ends the gesture.
  
  ACTION_DOWN: 第一个手指按下时
  ACTION_MOVE:按住一点在屏幕上移动
  ACTION_UP：最后一个手指抬起时
  ACTION_CANCEL:当前的手势被取消了，并且再也不会接收到后续的触摸事件，这时我们就像ACTION_UP一样对待他以结束该手势操作，但是却不执行我们在ACTION_UP时需要执行的动作。
  要理解这个类型，就必须要了解ViewGroup分发事件的机制。一般来说，如果一个子视图接收了父视图分发给它的ACTION_DOWN事件，那么与ACTION_DOWN事件相关的事件流就都要分发给这个子视图，但是如果父视图希望拦截其中的一些事件，不再继续转发事件给这个子视图的话，那么就需要给子视图一个ACTION_CANCEL事件。这在后续文章中源码分析部分也有体现。
  ACTION_OUTSIDE: 表示用户触碰超出了正常的UI边界.
  ACTION_POINTER_DOWN:代表用户又使用一个手指触摸到屏幕上，也就是说，在已经有一个触摸点的情况下，又新出现了一个触摸点。
  ACTION_POINTER_UP::代表用户的一个手指离开了触摸屏，但是还有其他手指还在触摸屏上。也就是说，在多个触摸点存在的情况下，其中一个触摸点消失了。它与ACTION_UP的区别就是，它是在多个触摸点中的一个触摸点消失时（此时，还有触摸点存在，也就是说用户还有手指触摸屏幕）产生，而ACTION_UP可以说是最后一个触摸点消失时产生。会在多指触摸和Pointers章节详解。
  
  多点触碰事件流：
  先产生一个ACTION_DOWN事件，代表用户的第一个手指接触到了屏幕。
  再产生一个 ACTION_POINTER_DOWN 事件，代表用户的第二个手指接触到了屏幕。
  很多的 ACTION_MOVE 事件，但是在这些MotionEvent对象中，都保存着两个触摸点滑动的信息，相关的代码我们会在文章的最后进行演示。
  一个 ACTION_POINTER_UP 事件，代表用户的一个手指离开了屏幕。
  如果用户剩下的手指还在滑动时，就会产生很多ACTION_MOVE事件。
  一个 ACTION_UP 事件，代表用户的最后一个手指离开了屏幕
  ```

  



* 动态权限

  [聊一聊Android 6.0的运行时权限](https://droidyue.com/blog/2016/01/17/understanding-marshmallow-runtime-permission/index.html)





---



### 组件化



* link

  [Android组件化开发实践](https://www.jianshu.com/p/d0f5cf304fa4)

  [Android组件化实践项目分享](https://juejin.im/post/5c7f85b3e51d45721073f966)

  [github - 组件化学习](https://github.com/hufeiyang/ComponentLearning)

* 库

  * ARouter 路由

    [github - ARouter](https://github.com/alibaba/ARouter)

    [Android之注解、APT、android-apt 和 annotationProcessor 的区别](https://blog.csdn.net/LVXIANGAN/article/details/88350717)

    ``` text
    ```

    





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

  



---



### View

* 全面屏和刘海屏

  [android 兼容所有刘海屏的方案大全](https://blog.csdn.net/DJY1992/article/details/80689632)

  ``` xml
  AndroidManifest.xml
  
  <!--放在Application和展示Activity（包括MainActivity，更新界面等）下-->
  <meta-data android:name="android.min_aspect" android:value="1.0"/>
  <meta-data android:name="android.max_aspect" android:value="2.4"/>		 <!--全面屏分辨率支持比例-->
  
  <meta-data android:name="notch.config" android:value="portrait|landscape"/>
  <meta-data android:name="android.notch_support" android:value="true"/>   <!--默认支持，即设置中把当前应用的全面屏支持开关打开-->
  ```

  

* scrollview

  * 自定义宽高

    [自定义ScrollView最大内容显示高度](https://blog.csdn.net/my_rabbit/article/details/80845660)

    ``` java
    //需要自定义一个继承自原生 ScrollView的类
    
    public class CustomScrollView extends ScrollView {
    
        private Context mContext;
    
        public CustomScrollView(Context context) {
            this(context, null);
        }
    
        public CustomScrollView(Context context, AttributeSet attrs) {
            this(context, attrs, 0);
        }
    
        public CustomScrollView(Context context, AttributeSet attrs, int defStyleAttr) {
            super(context, attrs, defStyleAttr);
            this.mContext = context;
        }
    
        @Override
        protected void onMeasure(int widthMeasureSpec, int heightMeasureSpec) {
            try {
                Display display = ((Activity) mContext).getWindowManager().getDefaultDisplay();
                DisplayMetrics d = new DisplayMetrics();
                display.getMetrics(d);
                // 设置控件最大高度不能超过屏幕高度的一半
                heightMeasureSpec = MeasureSpec.makeMeasureSpec(d.heightPixels / 2, MeasureSpec.AT_MOST);
            } catch (Exception e) {
                e.printStackTrace();
            }
            // 重新计算控件的宽高
            super.onMeasure(widthMeasureSpec, heightMeasureSpec);
        }
    }
    ```

    ``` xml
    <com.wiggins.widget.MyScrollView
     android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:fadingEdge="none"
        android:fillViewport="true"
        android:overScrollMode="never">
    <LinearLayout
        android:orientation="vertical"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:paddingLeft="10dp"
        android:paddingRight="12dp">
        <TextView
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:padding="10dip"
            android:textSize="12sp" />
    </LinearLayout>
    </com.wiggins.widget.MyScrollView>
    
    <!--
    1、去除ScrollView边界阴影
    1.1 在xml中添加：android:fadingEdge=”none”
    1.2 代码中添加：scrollView.setHorizontalFadingEdgeEnabled(false);
    
    2、去除ScrollView拉到顶部或底部时继续拉动后出现的阴影效果，适用于2.3及以上
    2.1 在xml中添加：android:overScrollMode=”never”
    
    3、当ScrollView子布局不足以铺满全屏的时候其高度就是子布局高度之和，此时如果想让ScrollView铺满全屏时只需要设置以下属性即可
    3.1 在xml中添加：android:fillViewport=”true”
    -->
    ```





* 软键盘

  * 软键盘挡住输入框问题

    [Android 软键盘盖住输入框的问题](https://www.cnblogs.com/androidez/archive/2013/04/09/3011399.html)

    ``` tex
    方法一：在你的activity中的oncreate中setContentView之前写上这个代码getWindow().setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
    
    方法二：在项目的AndroidManifest.xml文件中界面对应的<activity>里加入android:windowSoftInputMode="stateVisible|adjustResize"，这样会让屏幕整体上移。如果加上的是android:windowSoftInputMode="adjustPan"这样键盘就会覆盖屏幕。
    
    方法三：把顶级的layout替换成ScrollView，或者说在顶级的Layout上面再加一层ScrollView的封装。这样就会把软键盘和输入框一起滚动了，软键盘会一直处于底部。
    ```

    

* 硬件加速

  ``` tex
  android manifest中配置
  <applicationandroid:hardwareAccelerated="true" ...>				整个应用程序都启用
  或者
  <application android:hardwareAccelerated="true">   
   <activity .../>   <activity android:hardwareAccelerated="false" />
  </application>
  
  windows级别
  getWindow().setFlags(   
  WindowManager.LayoutParams.FLAG_HARDWARE_ACCELERATED,   WindowManager.LayoutParams.FLAG_HARDWARE_ACCELERATED);
  
  view级别
  myView.setLayerType(View.LAYER_TYPE_SOFTWARE, null);		禁用硬件加速，但是不能在view中开启硬件加速
  
  
  判断是否开启硬件加速
  View.isHardwareAccelerated() 
  Canvas.isHardwareAccelerated()
  ```

  