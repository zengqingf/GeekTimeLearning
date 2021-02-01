# Java API

### ref

[The Java™ Tutorials](https://docs.oracle.com/javase/tutorial/java/nutsandbolts/arrays.html)

[廖雪峰 - Java教程](https://www.liaoxuefeng.com/wiki/1252599548343744)

[菜鸟教程 - Java 教程](https://www.runoob.com/java/java-tutorial.html)



### 反射

* 反射优化方案 

  [如何利用缓存机制实现JAVA类反射性能提升30倍](https://blog.csdn.net/gao2175/article/details/103045600)

* 类型判断

  [Java中类型判断的几种方式](https://juejin.cn/post/6882161443819487246)

  * isAssignableFrom() vs instanceof

    [Java中isAssignableFrom()方法与instanceof()方法用法](https://www.cnblogs.com/bethunebtj/p/4681438.html)

* newInstance() 

  [newInstance() 的参数版本与无参数版本](https://www.cnblogs.com/alsf/p/8727660.html)

  



### Override

* equals

  [Overriding equals method in Java](https://www.geeksforgeeks.org/overriding-equals-method-in-java/)



### Genericity

* 限制类型参数类型

  [如何在 Java 泛型中限制类型参数的类型](https://www.91tech.org/archives/950)

  ``` text
  多重限制
  <T extends B1 & B2 & B3>
  
  
  类型参数 T 是列表中所有类型的子类。如果限制类型是类，那么必须放在第一个，例如：
  Class A { /* ... */ }
  interface B { /* ... */ }
  interface C { /* ... */ }
  //正确
  class D <T extends A & B & C> { /* ... */ }
  
  如果限制类型 A 没有放在第一位，那么会发生编译错误：
  //报错
  class D <T extends B & A & C> { /* ... */ }  // compile-time error
  ```




### Delegation

* ref

  [Java中的委托和继承(Delegation and Inheritance)](https://blog.csdn.net/Seriousplus/article/details/80462722)

  ``` java
  //委派：一个对象请求另一个对象的功能，捕获一个操作并将其发送到另一个对象。
  
  //1.
  public class A {
      void foo() {
          this.bar(); 
      }
      void bar() { 
          System.out.println("a.bar");
      }
  }
  
  //在B类中，不使用继承，而是利用委托结合A，达到复用A类中代码的效果：
  public class B {
      private A a;
      public B(A a) {
          this.a = a; 
      }
      void foo() {
          a.foo(); // call foo() on the a-instance }
      }
      void bar() {
          System.out.println("b.bar");
      }
  }
  
  
  //2.  通过实现接口
  public interface List<E> { 
      public boolean add(E e); 
      public E remove(int index);
      public void clear();
      …
  }
  
  public class LoggingList<E> implements List<E> {
      private final List<E> list;
      public LoggingList<E>(List<E> list) { 
          this.list = list; 
      } 
      public boolean add(E e) { 
          System.out.println("Adding " + e); 
          return list.add(e); 
      } 
      public E remove(int index) { 
          System.out.println("Removing at " + index); 
          return list.remove(index); 
      }
      …
  }
  
  
  /*
  委派的几种类型归纳
  1. Use (A use B)
  2. Composition/aggregation (A owns B)
  3. Association (A has B)
  
  */
  
  /*
  组合和聚合 区别
  */
  
  //聚合： 独立存在
  public class WebServer { 
      private HttpListener listener; 
      private RequestProcessor processor; 
      public WebServer(HttpListener listener, RequestProcessor processor) {
          this.listener = listener;
          this.processor = processor;
      } 
  }
  
  //组合：一起创建和销毁
  public class WebServer { 
      private HttpListener listener; 
      private RequestProcessor processor; 
      public WebServer() { 
          this.listener = new HttpListener(80); 
          this.processor = new RequestProcessor(“/www/root”);
      } 
  } 
  ```

  

