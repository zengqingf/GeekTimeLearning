# SDK中间件开发

## Unity环境

### Android交互

* ref

  [Unity与Android通信的中间件](https://ykbjson.github.io/2019/01/30/Unity%E4%B8%8EAndroid%E9%80%9A%E4%BF%A1%E7%9A%84%E4%B8%AD%E9%97%B4%E4%BB%B6(%E4%B8%80)/)

  [Unity与Android通信的中间件(二)](https://ykbjson.github.io/2019/05/21/Unity%E4%B8%8EAndroid%E9%80%9A%E4%BF%A1%E7%9A%84%E4%B8%AD%E9%97%B4%E4%BB%B6(%E4%BA%8C)/)

  [Unity3d Android SDK接入解析（四）通用的Android SDK接入中间件](https://blog.csdn.net/yang8456211/article/details/52231305)

  [浅谈Unity与Android原生的桥接](https://juejin.cn/post/6844904165760565261)

  [Unity 与 Android 互调用](https://www.cnblogs.com/alps/p/11206465.html)

  [Unity Android Callback AndroidJavaProxy](https://www.cnblogs.com/herenzhiming/articles/8334117.html)

  [unity写android插件时的回调接口处理AndroidJavaProxy](http://www.unitycn.cn/?p=237)

  [Unity中的回调监听器 - 如何从Android中的UnityPlayerActivity调用脚本文件方法?](https://codefanyi.com/post/53748615829)

  [Unity 调用java代码 以及java回调](https://blog.csdn.net/BDDNH/article/details/100583822)

  [Unity 与 Android 互调用](https://www.jianshu.com/p/b5e3cfcdf081)

  [unity写android插件时的回调接口处理AndroidJavaProxy](https://blog.csdn.net/generallizhong/article/details/105951805)

  [Unity与Android交互之AndroidJavaProxy的使用](https://www.jianshu.com/p/ceaac83808f2)

  [Unity的AndroidJavaProxy的使用](https://blog.csdn.net/qq_33337811/article/details/87815573)

  [Unity Android 之 AndroidJavaProxy 交互，实现 Unity 委托事件到 Android 端（aar包形式）](https://blog.csdn.net/u014361280/article/details/105866782)

  [Unity通过NDK、JNI实现C#、java、C/C++之间的相互调用](https://www.xiaoheidiannao.com/219697.html)

  

  

  [Unity3d Android SDK接入解析（一）Unity3d 与 Android之间的互相调用](https://blog.csdn.net/yang8456211/article/details/51331358)

  ``` text
  所以，我们使用的currentActivity 就是当前activity的上下文。写到这里，我还有一个疑问，就是currentActivity是拿到的UnityPlayerNativeActivity的上下文，但是是怎么调用到的Myactivity里面的方法的呢？希望知道的人告诉我。
  ```

  

  [Unity与Android交互](https://blog.csdn.net/ykmzy/article/details/82704534)

  [在unity中如何高效的使用内置android方法](https://blog.csdn.net/osuckseed/article/details/84940618)

  
  
  
  
  [unity开发之android与unity跨平台开发](https://www.jianshu.com/p/74722e8b29ec)
  
  ![](./Unity-Android_UML.png)







---



* Unity 插件导入方式

  [Unity中 .asmdef文件的作用](https://zhuanlan.zhihu.com/p/139090680)





---



* Unity Android 交互

  * 注意点
  
    ``` c#
    //测试环境：Unity 2018.4.23f1
    // 下文中的参数方法 是反编译 classes.jar获取到的混淆后的值  ”a“ "c"
    //可行1：
    using (AndroidJavaClass testjo1 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    {
        AndroidJavaObject testjo2 = testjo1.GetStatic<AndroidJavaObject>("currentActivity");
        var value1 = testjo1.CallStatic<string>("a", testjo2);   //out : Game view
    }
    //可行2：
    using (AndroidJavaObject testjo1 = new AndroidJavaObject("com.unity3d.player.UnityPlayer"))
    {
        AndroidJavaObject testjo2 = testjo1.GetStatic<AndroidJavaObject>("currentActivity");
        var value1 = testjo1.CallStatic<string>("a", testjo2);
    }
    //可行3：
    using (AndroidJavaClass testjo1 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    {
        var value1 = testjo1.Get<int>("c");  //out : 0
    }
    //可行4：
    using (AndroidJavaObject testjo1 = new AndroidJavaObject("com.unity3d.player.UnityPlayer"))
    {
        var value1 = testjo1.Get<int>("c");
    }
    //可行5 6：
    //5: using (AndroidJavaClass testjo1 = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) 
    // class com.unity3d.player.UnityPlayer
    //6: using (AndroidJavaObject testjo1 = new AndroidJavaObject("com.unity3d.player.UnityPlayer"))
    {
        AndroidJavaObject activity = testjo1.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject unityPlayer = activity.Get<AndroidJavaObject>("mUnityPlayer");
        AndroidJavaObject view = unityPlayer.Call<AndroidJavaObject>("getView");
    }
    //可行7:
    using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
    {
         view.Call("getWindowVisibleDisplayFrame", Rct);
    }
    //不可行8:
    using (AndroidJavaClass Rct = new AndroidJavaClass("android.graphics.Rect"))  //final class android.graphics.Rect
    //using (AndroidJavaObject Rct = new AndroidJavaClass("android.graphics.Rect"))
    {
         view.Call("getWindowVisibleDisplayFrame", Rct);
    }
    /*
    2020-12-28 16:11:12.315 4701-4729/? E/Unity: Platform Android GetKeyboardSize failed :UnityEngine.AndroidJavaException: java.lang.NoSuchMethodError: no non-static method with name='getWindowVisibleDisplayFrame' signature='(Ljava/lang/Class;)V' in class Ljava.lang.Object;
          at UnityEngine._AndroidJNIHelper.GetMethodID (System.IntPtr jclass, System.String methodName, System.String signature, System.Boolean isStatic) [0x00041] in <d71499eab75645c19df09ba17af9a506>:0 
          at UnityEngine.AndroidJNIHelper.GetMethodID (System.IntPtr javaClass, System.String methodName, System.String signature, System.Boolean isStatic) [0x00001] in <d71499eab75645c19df09ba17af9a506>:0 
          at UnityEngine._AndroidJNIHelper.GetMethodID (System.IntPtr jclass, System.String methodName, System.Object[] args, System.Boolean isStatic) [0x00009] in <d71499eab75645c19df09ba17af9a506>:0 
          at UnityEngine.AndroidJNIHelper.GetMethodID (System.IntPtr jclass, System.String methodName, System.Object[] args, System.Boolean isStatic) [0x00001] in <d71499eab75645c19df09ba17af9a506>:0 
          at UnityEng
    */
    
  //可行9 10：
    //9:  using (AndroidJavaClass wifiManagerClass = new AndroidJavaClass("android.net.wifi.WifiManager"))
    //	  using (AndroidJavaObject wifiManagerClass = new AndroidJavaObject("android.net.wifi.WifiManager"))
    //10: using (AndroidJavaClass contextClass = new AndroidJavaClass("android.content.Context"))
    //    using (AndroidJavaObject contextClass = new AndroidJavaObject("android.content.Context"))
    ```
    
    ``` c#
    //用例
     protected int GetKeyboardSize()
        {
            //using (AndroidJavaClass testjo1 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            //using (AndroidJavaObject testjo1 = new AndroidJavaObject("com.unity3d.player.UnityPlayer")) 
            //{            
            //    int value1 = Int32.MaxValue;
            //    value1 = testjo1.Get<int>("c");                                                                                         
            //    androidKeyboardSize = "测试1 ： " + value1;
    
            //    AndroidJavaObject testjo2 = testjo1.GetStatic<AndroidJavaObject>("currentActivity");
            //    string value2 = "Null";
            //    value2 = testjo1.CallStatic<string>("a", testjo2);      
            //    androidKeyboardSize += "\n测试2 ： " + value2;
            //    return 0;
            //}
    
            
            using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            // using (AndroidJavaObject UnityClass = new AndroidJavaObject("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject View = null;
    
                try
                {
                    if (UnityClass == null)
                        return 0;
                    AndroidJavaObject activity = UnityClass.GetStatic<AndroidJavaObject>("currentActivity");
                    if (activity == null)
                        return 0;
                    AndroidJavaObject unityPlayer = activity.Get<AndroidJavaObject>("mUnityPlayer");
                    if (unityPlayer == null)
                    {
                        androidKeyboardSize = string.Format("AndroidJavaObject Get mUnityPlayer is null");
                    }
                    else
                    {
                        androidKeyboardSize = string.Format("AndroidJavaObject Get mUnityPlayer type : {0}, \n str : {1} \n raw class intptr : {2} \n raw object intptr : {3}",
                         unityPlayer.GetType(), unityPlayer.ToString(), unityPlayer.GetRawClass().ToString(), unityPlayer.GetRawObject().ToString());
                    }
                    View = unityPlayer.Call<AndroidJavaObject>("getView");
                }
                catch (Exception e)
                {
                    Logger.LogError("try GetUnityPlayerView in Android failed : "+e.ToString());
                    return 0;
                }
    
                if (View == null)
                    return 0;
    
                using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
                //using (AndroidJavaObject Rct = new AndroidJavaClass("android.graphics.Rect"))  //不可行
                //using (AndroidJavaClass Rct = new AndroidJavaClass("android.graphics.Rect"))  //不可行
                {
                    if (Rct == null)
                    {
                        androidKeyboardSize = string.Format("android.graphics.Rect is null");
                        return 0;
                    }else
                    {
                        androidKeyboardSize = string.Format("android.graphics.Rect ： {0}", Rct.ToString());
                    }
                    try
                    {
                        View.Call("getWindowVisibleDisplayFrame", Rct);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("Platform Android GetKeyboardSize failed :"+e.ToString());
                        return 0;
                    }
                    int rect = Screen.height - Rct.Call<int>("height");
                  androidKeyboardSize = string.Format("result : {0}", rect);
                    return rect;
                }       
            }
        }
    ```
    





---



* Unity Android JNI

  [Unity通过NDK、JNI实现C#、java、C/C++之间的相互调用](https://linxinfa.blog.csdn.net/article/details/108642977)

  [使用AndroidJNI优化AndroidJavaClass](https://blog.csdn.net/noetic_wxb/article/details/53102671)

  ``` c#
  1. AndroidJavaClass 
  AndroidJavaClass SystemClock = new AndroidJavaClass("android.os.SystemClock");// 获取java类
  System.Int64 t = SystemClock.CallStatic<System.Int64>("uptimeMillis");// 调用无参函数
  
  2. AndroidJNI
  System.IntPtr class_SystemClock =  AndroidJNI.FindClass("android.os.SystemClock"); // 获取jiava类
  System.IntPtr method_uptimeMillis = AndroidJNIHelper.GetMethodID(class_SystemClock, "uptimeMillis","",true); // 查找方法
  System.Int64 t = AndroidJNI.CallStaticLongMethod(class_SystemClock, method_uptimeMillis, new jvalue[0]); // 调用方法
  
  /*
  直接使用AndroidJNI有一些要注意的地方：
  1、对System.IntPtr要使用System.IntPtr.Zero判空，而不能使用null，否则出错。比如找不到某个方法时，AndroidJNI/AndroidJNIHelper返回的Intptr.Zero，但!=null
  
  2、调用无参数的函数时，要使用new jvalue[0]作为参数，否则会异常退出
  
  尽管AndroidJNI的使用要复杂一些，但是在性能敏感的地方，最好使用AnroidJNI方式。从调用方式可以看到，AndroidJavaClass要把方法的名字作为参数传入，那内部肯定要根据函数名字符串查找函数指针，如果Unity3D没有cache的话，那甚至需要每次使用JNI从JVM中查找。从实际测试的情况看，AndroidJNI比AndroidJavaClass快了10倍以上。
  */
  ```

  [Unity 通过JNI传递数组到Android](https://blog.csdn.net/qjh5606/article/details/85298981)

  [Passing arrays through the JNI](https://forum.unity.com/threads/passing-arrays-through-the-jni.91757/)

  ``` c#
  public class TestArrayThroughJNI
  {
      private static AndroidJavaObject _plugin;
  
      static GPlayIABPlugin()
      {
          if( Application.platform != RuntimePlatform.Android )
              return;
  
          //This is apparently good practice, but seems to have no effect anyways!
          AndroidJNI.AttachCurrentThread();
  
          // find the plugin instance
          using (var pluginClass = new AndroidJavaClass("com.company.myplugin"))
              _plugin = pluginClass.CallStatic<AndroidJavaObject>( "instance" );
      }
      public static void PassArrayToJava()
      {
          float[] testFltArray = {1.5f, 2.5f, 3.5f, 3.6f};
  
          IntPtr jAryPtr = AndroidJNIHelper.ConvertToJNIArray(testFltArray);
          jvalue[] blah = new jvalue[1];
          blah[0].l = jAryPtr;
  
          IntPtr methodId = AndroidJNIHelper.GetMethodID(_plugin.GetRawClass(), "testArrayFunction");
          AndroidJNI.CallVoidMethod(_plugin.GetRawObject(), methodId, blah);
      }
  }
  ```

  ``` java
  public class JavaTestArray
  {
      //Used by Unity to grab the instance of this Java class.
      private static IABUnityPlugin _instance;
  
      //Magic happens here.
      public void testArrayFunction(float[] test)
      {
          Log.i("Unity", "Float[]: " + test + " and length: " + test.length);
  
          for(int i = 0; i < test.length; i++)
          {
              Log.i("Unity", "[" + i + "] " + test[i]);
          }
      }
  
      public static JavaTestArray instance()
      {
          if(_instance == null)
              _instance = new JavaTestArray();
  
          return _instance;
      }
  }
  /*
  Output:
  Float[]: [F@405608d0 and length: 4
  [0] 1.5
  [1] 2.5
  [2] 3.5
  [3] 3.6
  */
  ```

  



---



* Unity  C++ / NDK / IOS

  [Unity3D研究院之Android NDK编译C/C++结合Unity实现本地数据共享（二十八）](https://www.xuanyusong.com/archives/1129)





---



* 中间件思考分析

  [Unity SDK 正确的集成方法](https://networm.me/2020/01/05/unity-sdk-integration/)

  





---





* 扩展

  * 反射

    [Unity3D Android动态反射加载程序集](https://www.cnblogs.com/mrblue/p/7323896.html)

    [Unity通过反射给gameObject添加组件](https://blog.csdn.net/linxinfa/article/details/86580046)

    [unity3D数据库反射操作 ](https://www.jianshu.com/p/dca2a25c1c29)

    [Unity C#基础之 反射反射，程序员的快乐](https://www.jianshu.com/p/2f0cfdf116c8)

    [【JAVA与C#比较】反射](https://blog.csdn.net/u014650759/article/details/79547495)

  * Unity跨平台

    [结合代码来看 Unity 3D 的跨平台特性实现](https://juejin.cn/post/6844904077894090766)

    [github - QinGeneral - CppCSInvoke](https://github.com/QinGeneral/CppCSInvoke)

    [github - UnityTech - uaal-example](https://github.com/Unity-Technologies/uaal-example)

    [Unity中的 原生插件/平台交互 原理](https://blog.csdn.net/u010019717/article/details/78451660)

    [腾讯桌球客户端开发实战总结](https://gameinstitute.qq.com/community/detail/107639)

    [Unity与iOS平台交互和原生插件开发](https://aabao.github.io/Unity_iOS_NativPlugin/)

  * Unity协程

    [【Unity 3D引擎源码分析】全面解析Coroutine技术](https://gameinstitute.qq.com/community/detail/100275)

  * Unity热更新

    [Unity3D引擎跨平台底层原理及为何无法在iOS平台上热更新](https://blog.csdn.net/Wei_Yuan_2012/article/details/86560822)

  * Unity 配置文件 json库
  
    [客户端配置文件优化策略](https://blog.uwa4d.com/archives/2045.html)

