# Android & Java API



* ref

  [GitHub- android 教程](https://github.com/kesenhoo/android-training-course-in-chinese)



---



### Java API

* @NotNull NonNull Nonnull

  [Java 里 NonNull 和 NotNull 区别](http://yansu.org/15775214814688.html)

  [Which @NotNull Java annotation should I use?](https://stackoverflow.com/questions/4963300/which-notnull-java-annotation-should-i-use)

* SoftReference

  [深入探讨 java.lang.ref 包](https://developer.ibm.com/zh/articles/j-lo-langref/)

  [Android性能优化之使用SoftReference缓存图片](https://blog.csdn.net/nugongahou110/article/details/47280461)

  ``` java
  /*   Copyright 2004 The Apache Software Foundation
   *
   *   Licensed under the Apache License, Version 2.0 (the "License");
   *   you may not use this file except in compliance with the License.
   *   You may obtain a copy of the License at
   *
   *       http://www.apache.org/licenses/LICENSE-2.0
   *
   *   Unless required by applicable law or agreed to in writing, software
   *   distributed under the License is distributed on an "AS IS" BASIS,
   *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   *   See the License for the specific language governing permissions and
   *  limitations under the License.
   */
  // Revised from xmlbeans
  
  import java.util.HashMap;
  import java.lang.ref.SoftReference;
  
  /**
   * @author Cezar Andrei (cezar.andrei at bea.com)
   *         Date: Apr 26, 2005
   */
  public class SoftCache
  {
      private HashMap map = new HashMap();
  
      public Object get(Object key)
      {
          SoftReference softRef = (SoftReference)map.get(key);
  
          if (softRef==null)
              return null;
  
          return softRef.get();
      }
  
      public Object put(Object key, Object value)
      {
          SoftReference softRef = (SoftReference)map.put(key, new SoftReference(value));
  
          if (softRef==null)
              return null;
  
          Object oldValue = softRef.get();
          softRef.clear();
  
          return oldValue;
      }
  
      public Object remove(Object key)
      {
          SoftReference softRef = (SoftReference)map.remove(key);
  
          if (softRef==null)
              return null;
  
          Object oldValue = softRef.get();
          softRef.clear();
  
          return oldValue;
      }
  }
  
  ```

* Java反射

  [Java 反射详解](https://www.cnblogs.com/cangqinglang/p/10077484.html)

  [java反射使用getDeclaredMethods会获取到父类方法的解决办法](https://monkeywie.cn/2019/07/03/java-reflect-getdeclaredmethods-issue/)

  [Java反射：通过父类对象调用子类方法](https://blog.csdn.net/u010429286/article/details/78541509)
  
* Java & Android 序列化

  [Android高级架构进阶之数据传输与序列化](https://zhuanlan.zhihu.com/p/90036011)

  [How can I make my custom objects Parcelable?](https://stackoverflow.com/questions/7181526/how-can-i-make-my-custom-objects-parcelable)

  [Difference between DTO, VO, POJO, JavaBeans?](https://stackoverflow.com/questions/1612334/difference-between-dto-vo-pojo-javabeans)

* Java 编译

  [空行会影响 Java 编译？](https://www.jianshu.com/p/3c2c7a3fd81b)

  ``` text
  报错时 Java 会抛出具体的报错行数信息，其实 Java 会去记录行数，以便 debug 调试。
  ```

  

* Handler

  **主要用于线程切换**

  还可以用于延时执行



* GetUrl

  解决报错：exposed beyond app through ClipData.Item.getUri() 

  ``` java
  //for sdk 24 and up, need use FileProvider.getUriForFile()
  Uri imageUri = FileProvider.getUriForFile(
              MainActivity.this,
              "com.example.homefolder.example.provider", //(use your app signature + ".provider" )
              imageFile);
  
  //androidManifest.xml
  //(In "authorities" use the same value than the second argument of the getUriForFile() method (app signature + ".provider"))
  <application>
    ...
       <provider
          android:name="android.support.v4.content.FileProvider"
          android:authorities="com.example.homefolder.example.provider"
          android:exported="false"
          android:grantUriPermissions="true">
          <!-- ressource file to create -->
          <meta-data
              android:name="android.support.FILE_PROVIDER_PATHS"
              android:resource="@xml/file_paths">  
          </meta-data>
      </provider>
  </application>
                  
  //file_paths, res/xml/file_paths.xml
  <?xml version="1.0" encoding="utf-8"?>
  <paths xmlns:android="http://schemas.android.com/apk/res/android">
      <external-path name="external_files" path="." />
  </paths>
  ```

  

  





---



### Android API

* ActivityLifecycleCallbacks

  [优雅的使用ActivityLifecycleCallbacks管理Activity和区分App前后台](https://blog.csdn.net/u010072711/article/details/77090313)
  
  [了解 Activity 生命周期](https://developer.android.com/guide/components/activities/activity-lifecycle)



* Permissions

  [github-permissions-dispatcher](https://github.com/permissions-dispatcher)



* Context

  [Context认识](https://www.jianshu.com/p/94e0f9ab3f1d)

  ![](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/202109230935933.jpg)

  ``` tex
  Context的两个子类分工明确，其中ContextImpl是Context的具体实现类，ContextWrapper是Context的包装类。Activity，Application，Service虽都继承自ContextWrapper（Activity继承自ContextWrapper的子类ContextThemeWrapper），但它们初始化的过程中都会创建ContextImpl对象，由ContextImpl实现Context中的方法。
  
  一个app中的Context数量=Activity数量+Service数量+1（Application）
  	Broadcast Receiver，Content Provider并不是Context的子类，他们所持有的Context都是其他地方传过去的，所以并不计入Context总数。
  	
  	
  	
  
  Application和Service两种不适用情况：
  
  1：如果我们用ApplicationContext去启动一个LaunchMode为standard的Activity的时候会报错android.util.AndroidRuntimeException: Calling startActivity from outside of an Activity context requires the FLAG_ACTIVITY_NEW_TASK flag. Is this really what you want?这是因为非Activity类型的Context并没有所谓的任务栈，所以待启动的Activity就找不到栈了。解决这个问题的方法就是为待启动的Activity指定FLAG_ACTIVITY_NEW_TASK标记位，这样启动的时候就为它创建一个新的任务栈，而此时Activity是以singleTask模式启动的。所有这种用Application启动Activity的方式不推荐使用，Service同Application。
  2：在Application和Service中去layout inflate也是合法的，但是会使用系统默认的主题样式，如果你自定义了某些样式可能不会被使用。所以这种方式也不推荐使用。
  
  凡是跟UI相关的，都应该使用Activity做为Context来处理；其他的一些操作，Service,Activity,Application等实例都可以
  
  
  getApplication() == getApplicationContext()，仅作用域不同，getApplication()仅在Activity和Service中能调用
  
  
  注意Context引用的持有，防止内存泄漏
  1.单例模式中持有Context，如果传入Activity，由于一般单例为静态对象，生命周期长于Activity,当Activity被销毁时，由于还被单例持有，不会GC
  2.View持有Activity引用，如果当一个静态的Drawable对象设置Drawable时，需要传入this(即当前Activity)，Drawable常驻内存，当前Activity被销毁时，也不能被GC掉
  
  1：当Application的Context能搞定的情况下，并且生命周期长的对象，优先使用Application的Context。
  2：不要让生命周期长于Activity的对象持有到Activity的引用。
  3：尽量不要在Activity中使用非静态内部类，因为非静态内部类会隐式持有外部类实例的引用，如果使用静态内部类，将外部实例引用作为弱引用持有。
  	（在Java中，非静态的内部类和匿名类会隐式地持有一个他们外部类的引用。静态内部类则不会。）
  	（In Android, Handler classes should be static or leaks might occur.）
  ```

  ```java
  //会产生内存泄露
  public class SampleActivity extends Activity {
      private finial Handler mLeakyHandler = new Handler() {
          @Override
          public void handleMessage(Message msg) {}
      }
      
      @Override
      protected void OnCreate(Bundle savedInstanceState) {
          super.onCreate(savedInstanceState);
          mLeakyHandler.postDelayed(new Runnable(){
              @Override
              public void run() {}
          }, 60 * 10 * 1000 );
          
          finish();
      }
  }
  /*
  当这个Activity被finished后，延时发送的消息会继续在主线程的消息队列中存活10分钟，直到他们被处理。这个消息持有这个Activity的Handler引用，这个Handler有隐式地持有他的外部类（在这个例子中是SampleActivity）。直到消息被处理前，这个引用都不会被释放。因此Activity不会被垃圾回收机制回收，泄露他所持有的应用程序资源
  
  匿名Runnable类也一样。匿名类的非静态实例持有一个隐式的外部类引用,因此context将被泄露
  
  Handler的子类应该定义在一个新文件中或使用静态内部类。静态内部类不会隐式持有外部类的引用。所以不会导致它的Activity泄露。如果你需要在Handle内部调用外部Activity的方法，那么让Handler持有一个Activity的弱引用（WeakReference）以便你不会意外导致context泄露。为了解决我们实例化匿名Runnable类可能导致的内存泄露，我们将用一个静态变量来引用他（因为匿名类的静态实例不会隐式持有他们外部类的引用）。
  */
  
  public class SampleActivity extends Activity {
      //匿名类的静态实例不会隐式持有他们外部类的引用
      private static final Runnable sRunnable = new Runnable() {
          @Override
          public void run() {}
      }
      private final MyHandler mHandler = new MyHandler(this);
      @Override
      protected void OnCreate(Bundle savedInstanceState) {
          super.onCreate(savedInstanceState);
          mHandler.postDelayed(sRunnable, 60 * 10 * 1000 );
          finish();
      }
      //静态内部类的实例不会隐式持有他们外部类的引用。
      private static class MyHandler extends Handler {
          private final WeakReference<SampleActivity> mActivity;
          
          public MyHandler(SampleActivity activity) {
              mActivity = new WeakReference<SampleActivity>(activity);
          }
          @Override
          public void handleMessage(Message msg) {
              SampleActivity activity = mActivity.get();
              if(activity != null) {}
          }
      }
  }
  ```

  

  * Context作用

    ``` tex
    弹出Toast、启动Activity、启动Service、发送广播、操作数据库等等
    ```

    * 作用域

      ![](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/202109230938103.jpg)





* 四大组件

  ``` tex
  activity:  一个activity通常是一个单独的屏幕；通过Intent通信；必须在AndroidManifest中声明；activity由task任务栈管理
  service:  用于在后台完成用户指定操作：started(启动，需要stop才能停止)、bound(绑定，绑定调用者，生命周期随调用者)
  content provider: 应用程序指定数据集提供给其他应用程序；实现数据共享；使用URL唯一标识数据集(content:// 作为前缀)
  broadcast receiver: 广播接收器对外部事件进行过滤，只对感兴趣的事件做出响应；没有用户界面，可以启动activity或service来响应收到的消息；
  					可以通过manifest静态注册或者程序动态注册（静态注册不受注册的Activity是否被销毁影响，广播接收器开启时即能被触发）
  ```

  



* 启动模式





---



### Android Design Patterns

* 组件化



* MVVM

  [如何构建Android MVVM 应用框架](https://tech.meituan.com/2016/11/11/android-mvvm.html)






---



### AndroidManifest

* ref

  [AndroidManifest理解](https://www.jianshu.com/p/6ed30112d4a4)



* android:configChanges

  ``` tex
  用于捕获手机状态改变
  Activity中添加android:configChanges，当指定属性状态改变时，通知程序调用onConfigurationChanged()
  
  设置方法：locale|navigation|orientation | ...
  
  “mcc“ 移动国家号码，由三位数字组成，每个国家都有自己独立的MCC，可以识别手机用户所属国家。
  “mnc“ 移动网号，在一个国家或者地区中，用于区分手机用户的服务商。
  “locale“ 所在地区发生变化。
  “touchscreen“ 触摸屏已经改变。（这不应该常发生。）
  “keyboard“ 键盘模式发生变化，例如：用户接入外部键盘输入。
  “keyboardHidden“ 用户打开手机硬件键盘
  “navigation“ 导航型发生了变化。（这不应该常发生。）
  “orientation“ 设备旋转，横向显示和竖向显示模式切换。
  “fontScale“ 全局字体大小缩放发生改变
  "screenSize" API 13（即Android 3.2） 
  
  不设置时：
  1.不设置Activity的android:configChanges时，切屏会重新调用各个生命周期，切横屏时会执行一次，切竖屏时会执行两次
  2.设置Activity的android:configChanges="orientation"时，切屏还是会重新调用各个生命周期，切横、竖屏时只会执行一次
  3.设置Activity的android:configChanges="orientation|keyboardHidden"时，切屏不会重新调用各个生命周期，只会执行onConfigurationChanged方法
  4.API 13后（设置minisdkversion 和 targetsdkversion >= 13），设置Activity的android:configChanges="orientation|keyboardHidden"，会重新调用各个生命周期的，因为screen size也跟着设备横竖屏切换而改变
  	解决：android:configChanges="orientation|screenSize“
  ```

  ``` tex
  2020年使用示例：
  
  android:configChanges="fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen"
  ```



* 合并manifest.xml

  [合并多个清单文件](https://developer.android.com/studio/build/manifest-merge?hl=zh-cn)

  





---



### Android Debug

* adb 

  * 环境搭建

    ``` tex
    android系统环境变量：
    android=xxx/platform-tools;
    
    windows:
    Path中添加%android%
    
    macos:
    ~/.bash_profile
    export ANDROID_HOME=/xxx/sdk
    export PATH=${PATH}:${ANDROID_HOME}/tools
    export PATH=${PATH}:${ANDROID_HOME}/platform-tools
    source ~/.bash_profile
    ```

    