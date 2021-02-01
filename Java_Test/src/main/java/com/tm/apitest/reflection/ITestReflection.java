package com.tm.apitest.reflection;

@ReflectClass("Test")
public interface ITestReflection {

    @ReflectMethod("method2")
    void method2();
}
