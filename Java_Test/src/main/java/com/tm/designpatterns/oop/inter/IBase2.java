package com.tm.designpatterns.oop.inter;

import java.util.ArrayList;
import java.util.List;

public interface IBase2 {

    /*
    * 接口
    * vs. 抽象类
    * 1. 抽象类可以包含字段
    * 2. 具体类继承（extends）抽象类， 具体类实现(implements)接口
    * 3. 抽象类中能被单个继承，接口可以被多个实现
    * 4. 抽象类可以定义非抽象方法，接口可以定义default方法
    * 5. 抽象类和接口都可以定义抽象方法
    *
    *
    *
    * */

    /*
    * 合理设计interface和abstract class的继承关系，可以充分复用代码。
    * 一般来说，公共逻辑适合放在abstract class中，具体逻辑放到各个子类，
    * 而接口层次代表抽象程度。可以参考Java的集合类定义的一组接口、抽象类以及具体子类的继承关系：
    *
    * 在使用的时候，实例化的对象永远只能是某个具体的子类，但总是通过接口去引用它，因为接口比抽象类更抽象：
    * List list = new ArrayList();
    * Collection coll = list;
    * Iterable it = coll;
    *
    * Iterable
    *    ↑                 Object
    * Collection              ↑
    *    ↑     ↑    AbstractCollection
    *   List                  ↑
    *          ↑    AbstractList
    *                         ↑
    *               ArrayList   LinkedList
    * */


    //default方法
    //实现类不需要override

    // 作用
    //  当我们需要给接口新增一个方法时，会涉及到修改全部子类。
    //  如果新增的是default方法，那么子类就不必全部修改，只需要在需要覆写的地方去覆写新增方法。

    //  default方法和抽象类的普通方法是有所不同的。
    //  因为interface没有字段，default方法无法访问字段，而抽象类的普通方法可以访问实例字段。
    default  void test() {
        System.out.println("测试接口 default 方法");
    }

    //接口可以定义 静态字段  必须为final类型
    public static final int Test1 = 1;
    //可以简写为
    int Test2 = 1;


}
