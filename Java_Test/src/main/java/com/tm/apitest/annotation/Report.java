package com.tm.apitest.annotation;

import java.lang.annotation.*;


/*
* 元注解  @Inherited  定义 子类是否可以继承父类定义的  Annotation
*
* 仅针对@Target(ElementType.TYPE)类型的annotation有效，并且仅针对class的继承，对interface的继承无效
*
*   使用场景
*   @Report(type=1)
    public class Person {
    }

    public class Student extends Person {
    }
*
* */

@Inherited
@Target(ElementType.TYPE)
@Retention(RetentionPolicy.RUNTIME)
public @interface Report {
    int type() default 0;
    String level() default "info";
    String value() default "";
}
