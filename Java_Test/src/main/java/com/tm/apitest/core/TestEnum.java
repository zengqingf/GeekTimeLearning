package com.tm.apitest.core;

public enum TestEnum {

    /*
    * 使用enum定义的枚举类是一种引用类型。前面我们讲到，引用类型比较，要使用equals()方法，
    * 如果使用==比较，它比较的是两个引用类型的变量是否是同一个对象。
    * 因此，引用类型比较，要始终使用equals()方法，但enum类型可以例外。
    *
    * 这是因为enum类型的每个常量在JVM中只有一个唯一实例，所以可以直接用==比较
    *
    * */


    /*
    特点：

    *   定义的enum类型总是继承自java.lang.Enum，且无法被继承；
        只能定义出enum的实例，而无法通过new操作符创建enum的实例；
        定义的每个实例都是引用类型的唯一实例；
        可以将enum类型用于switch语句。

        *
        *
        * 编译后的enum 和 普通类没有什么区别
    *

    因为enum是一个class，每个枚举的值都是class实例，因此，这些实例有一些方法：
    name() 和 默认toString() 返回值一致
    ordinal()   //改变枚举常量定义的顺序就会导致ordinal()返回值发生变化

    * */

    Test1(0, "测试一"), Test2(1, "测试二");

    //private 构造函数  给每个枚举常量添加字段(必须)
    public final int testValue;
    private TestEnum(int value, String chinese) {
        this.testValue = value;
        this.chinese = chinese;
    }

    private final String chinese;
    @Override
    public String toString() {
        return this.chinese;
    }
}


