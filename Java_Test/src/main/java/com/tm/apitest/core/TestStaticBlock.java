package com.tm.apitest.core;

/*
* 测试 static块和构造函数的调用先后顺序
*
* 输出：
*
*   [TestStaticBlock] - static block
    [TestStaticBlock] - ctor block
* */
public class TestStaticBlock {
    public TestStaticBlock() {
        System.out.println("[TestStaticBlock] - ctor block");
    }

    static
    {
        System.out.println("[TestStaticBlock] - static block");
    }
}
