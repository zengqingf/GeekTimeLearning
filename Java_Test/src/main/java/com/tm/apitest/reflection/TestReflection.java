package com.tm.apitest.reflection;

import java.lang.reflect.AnnotatedType;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

@ReflectClass("test")
public class TestReflection extends BaseReflection  {

    public void method1(String p1, int p2, String p3) {
        System.out.println(p1 + p2 + p3);
    }

    @Override
    public void method2() {

        System.out.println(this.getClass());
        System.out.println(TestReflection.class);

        ITestReflection itr = new TestReflection();
        System.out.println(itr.getClass());

        System.out.println("impl method2");
    }

    public static void test() {

        Class<?> inter = ITestReflection.class;
        System.out.println(String.format("getInterface.Method - %s", inter.getMethods().length));

        Class<?> clazz = TestReflection.class;
        System.out.println(String.format("getClass.Name - %s", clazz.getName()));
        try {
            System.out.println(String.format("getMethod.ParamTypes - %s", clazz.getMethod("method1", String.class, int.class, String.class).getParameterTypes().length));
        } catch (NoSuchMethodException e) {
            e.printStackTrace();
        }

        Class<?>[] an = clazz.getSuperclass().getInterfaces();
        System.out.println(String.format("getAnnotations interface - %s", an[0].getAnnotations()[0].annotationType()));
        System.out.println(String.format("getAnnotations interface - %s", an[0].getAnnotation(ReflectClass.class)));
        System.out.println(String.format("getAnnotations interface - %s", an[0].getMethods()[0].getAnnotation(ReflectMethod.class)));
        /*try {
            an[0].getMethods()[0].invoke(new TestReflection(), null);
        } catch (IllegalAccessException e) {
            e.printStackTrace();
        } catch (InvocationTargetException e) {
            e.printStackTrace();
        }*/

        AnnotatedType[] an2 = clazz.getAnnotatedInterfaces();
        System.out.println(String.format("getAnnotations interface - %s", an2[0].getAnnotations().length));

        Class<?> an1 = clazz.getSuperclass();
        System.out.println(String.format("getAnnotations super class - %s", an1.isAnnotationPresent(ReflectClass.class)));

        /*
        ServiceLoader<ITestReflection> loader = ServiceLoader.load(ITestReflection.class);
        for(ITestReflection itf : loader) {
           itf.method2();
        }*/

        try {
            Method m1 = clazz.getMethod("method1", String.class, int.class, String.class);
            System.out.println(String.format("getMethod.Name - %s", m1.getName()));
            Method[] ms = clazz.getMethods();
            System.out.println(String.format("getMethods.Name 1 - %s", ms[0].getName()));
        } catch (NoSuchMethodException e) {
            e.printStackTrace();
        }
    }
}

