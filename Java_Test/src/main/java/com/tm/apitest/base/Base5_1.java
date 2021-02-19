package com.tm.apitest.base;

public class Base5_1 {

    //包作用域是指一个类允许访问同一个package的没有public修饰的class，以及没有private修饰的字段和方法。
    // （即 包的默认作用域， 范围小于public）
    public static void TestField() {
        Base5 base5 = new Base5();  //可以访问 没有public 修饰的 class
        base5.testField();          //！！！ 在同一个包下的类， 可以访问 不用private修饰的方法

        //base5.testField2();       //！！！ 无法访问private 修饰的方法
        base5.testField3();
        base5.testField4();
    }
}
