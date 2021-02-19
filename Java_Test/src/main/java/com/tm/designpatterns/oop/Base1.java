package com.tm.designpatterns.oop;


public class Base1 {


    //继承
    /*
    * 子类不会继承任何父类的构造方法。子类默认的构造方法是编译器自动生成的，不是继承的
    *
    * 子类构造函数中  使用super() 或者传入指定参数 调用 构造函数
    *
    * 阻止继承
    * 正常情况下，只要某个class没有final修饰符，那么任何类都可以从该class继承。
    *
    * 从Java 15开始，允许使用sealed修饰class，并通过permits明确写出能够从该class继承的子类名称。
    * 例：
    * public sealed class Shape permits Rect, Circle, Triangle {
      }

      //编译成功
      public final class Rect extends Shape {...}

      //编译失败
      public final class Ellipse extends Shape {...}
      *
      * 2021 应该不是预览状态了
      //sealed类在Java 15中目前是预览状态，要启用它，必须使用参数--enable-preview和--source 15。
      *
      *
      *
    *
    * 向上转型（upcasting）
    * 把一个子类类型安全地变为父类类型的赋值
    *
    * 向下转型 (downcasting)
    *   Person p1 = new Student(); // upcasting, ok
        Person p2 = new Person();
        Student s1 = (Student) p1; // ok
        Student s2 = (Student) p2; // runtime error! ClassCastException!
        *
        *
        *
      向下转型 尽量先判断类型
      instanceof 判断实例是不是某种类型
      从Java 14开始，判断instanceof后，可以直接转型为指定变量，避免再次强制转型。例如，对于以下代码：
      Object obj = "hello";
        if (obj instanceof String s) {
            // 可以直接使用变量s:
            System.out.println(s.toUpperCase());
        }
    * */


    //多态 Polymorphic
    /*
    * Java的实例方法调用是基于运行时的实际类型的动态调用，而非变量的声明类型。
    * 多态的特性就是，运行期才能动态决定调用的子类方法。对某个类型调用某个方法，执行的实际方法可能是某个子类的覆写方法。
    *
    *
    *
    *
    *  Override  Object 基类 方法
    *
       toString()：把instance输出为String；
       equals()：判断两个instance是否逻辑相等；
       hashCode()：计算一个instance的哈希值。
       *
       *

       *
       Super  子类调用父类方法
       *
       *

       *
       *

*  final 关键字
        final修饰符不是访问权限，它可以修饰class、field和method；

        用final修饰class可以阻止被继承：
        用final修饰method可以阻止被子类覆写：
        用final修饰field可以阻止被重新赋值：
        用final修饰局部变量可以阻止被重新赋值：

        final  父类方法添加final 不能被子类Override
        final  父类添加final  不能被继承
        final  字段添加final  不能被修改  但是可以在构造函数中初始化
       *
       *

    * */

    //final  字段添加final  不能被修改  但是可以在构造函数中初始化
    class Person {
        public final String name = "Unamed";
    }
    Person p = new Person();
    //p.name = "New Name"; // compile error!

    class Person1 {
        public final String name;
        public Person1(String name) {
            this.name = name;
        }
    }

    //用final修饰局部变量可以阻止被重新赋值：
    protected void Test(final int i) {
        //i = 1; // compile error !
    }
}
