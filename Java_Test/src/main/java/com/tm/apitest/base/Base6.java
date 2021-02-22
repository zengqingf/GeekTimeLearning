package com.tm.apitest.base;

/* 作用域

 访问权限
 Java内建的访问权限包括public、protected、private和package权限；

 局部变量
 使用局部变量时，应该尽可能把局部变量的作用域缩小，尽可能延后声明局部变量。
 Java在方法内部定义的变量是局部变量，局部变量的作用域从变量声明开始，到一个块结束；




如果不确定是否需要public，就不声明为public，即尽可能少地暴露对外的字段和方法。
把方法定义为package权限有助于测试，因为测试类和被测试类只要位于同一个package，测试代码就可以访问被测试类的package权限方法。
一个.java文件只能包含一个public类，但可以包含多个非public类。如果有public类，文件名必须和public类的名字相同。




 * 嵌套类  (内部类)
 * 由于Java支持嵌套类，如果一个类内部还定义了嵌套类，那么，嵌套类拥有访问private的权限：
 *
 *
   定义在一个class内部的class称为嵌套类（nested class），Java支持好几种嵌套类。


   非静态的 Inner class 不能单独存在，需要依附于一个 Outer class


   要实例化一个Inner，我们必须首先创建一个Outer的实例，然后，调用Outer实例的new来创建Inner实例：
   Base6 base6 = new Base6();
   Base6.Inner inner = base6.new Inner();
   这是因为Inner Class除了有一个this指向它自己，
   还隐含地持有一个Outer Class实例，可以用Outer.this访问这个实例。
   ！！！所以，实例化一个Inner Class不能脱离Outer实例。

查看编译后的class
   Outer类被编译为Outer.class，而Inner类被编译为Outer$Inner.class


   匿名内部类 Anonymous class

 *
 * */

import java.util.HashMap;

public class Base6 {

    private String name;
    private static String alias;

    private static void Test() {
        System.out.println("私有静态方法");
    }

    static class StaticInner {
        public void Test() {
            Base6.Test();
        }
    }

    class Inner {

    }

    //匿名内部类
    void annoymousInner() {

        /*
        * Runnable 是 interface  不能被实例化
        *
        * 以下 实现了Runnable 接口的匿名类  并实例化了 然后转型为 Runnable
        *
        * 定义匿名类时，必须就要实例化了
        *
        * 和Inner class一样，可以访问Outer class的private字段和方法
        *
        * 匿名类会编译成  Outer$1.class
        * 有多个匿名类 会编译成  Outer$1.class  Outer$2.class .。。
        *
        * 可以访问 Outer.this 和 Outer private
        * */

        //可以替换成lambda
        // Runnable r = () -> System.out.println("annoymous inner ...");
        Runnable r = new Runnable() {
            @Override
            public void run() {
                System.out.println("annoymous inner ..." + Base6.this.name); //可以省略  Base6.this
            }
        };
        new Thread(r).start();

        /*
        * 除接口外 匿名类 也能完全继承自 普通类
        *
        *
        *
        * */

        HashMap<String, String> map1 = new HashMap<>();
        HashMap<String, String> map2 = new HashMap<String, String>() {}; //匿名类
        //匿名类不能省略 HashMap<String, String> map2 = new HashMap<>() {};
        HashMap<String, String> map3 = new HashMap<String, String>() {
            {
                put("A", "1");
                put("B", "2");
            }
        };
    }

    //static nested class
    // 不同于普通内部类， 不在依附于 Outer实例， 而是一个完全独立的类， 无法引用 Outer.this,
    // 可以访问 Outer private static 字段和方法
    static class StaticNested {
        void TestStaticNested() {
            System.out.println("static nested ..." + Base6.alias);  // 可以省略 Base6.
        }
    }
}
