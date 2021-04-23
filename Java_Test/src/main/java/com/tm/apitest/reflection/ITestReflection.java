package com.tm.apitest.reflection;

@ReflectClass("Test")
public interface ITestReflection extends IReflection {

    @ReflectMethod("method2")
    void method2();
}
