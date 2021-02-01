# Android & Java API



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







---



### Android API

* ActivityLifecycleCallbacks

  [优雅的使用ActivityLifecycleCallbacks管理Activity和区分App前后台](https://blog.csdn.net/u010072711/article/details/77090313)





---



### Android Design Patterns

* 组件化

  