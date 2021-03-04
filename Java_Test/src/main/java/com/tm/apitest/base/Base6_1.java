package com.tm.apitest.base;

public class Base6_1 {

    /*
    * Inner Class和Anonymous Class本质上是相同的，都必须依附于Outer Class的实例，
    * 即隐含地持有Outer.this实例，并拥有Outer Class的private访问权限
    *
    *
    * Static Nested Class是独立类，但拥有Outer Class的private访问权限
    * */
    public static void TestInnerClass() {
        Base6 base6 = new Base6();
        Base6.Inner inner = base6.new Inner();
        inner.TestInner();


        Base6.StaticInner staticInner = new Base6.StaticInner();
        staticInner.Test();
    }
}
