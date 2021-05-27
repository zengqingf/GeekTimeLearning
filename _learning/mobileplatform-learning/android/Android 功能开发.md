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
  ```

  ![](api/android_screen_coord_1.jpg)

  ![android_screen_coord_2](api/android_screen_coord_2.jpg)

  ![android_screen_coord_3](api/android_screen_coord_3.jpg)





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

  