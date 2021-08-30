package com.tm.apitest.annotation;

import java.lang.annotation.*;


/*
* 元注解  @Target   定义 Annotation 能够被应用于 源码的哪些位置
*
*   类或接口：ElementType.TYPE；
    字段：ElementType.FIELD；
    方法：ElementType.METHOD；
    构造方法：ElementType.CONSTRUCTOR；
    方法参数：ElementType.PARAMETER

*   @Target({
    ElementType.METHOD,
    ElementType.FIELD
    })   添加{} 形成注解数组  即原理是：@Target定义的value是ElementType[]数组，只有一个元素时，可以省略数组的写法
*
*
* 元注解   @Retention  定义 Annotation 的生命周期
*
*   仅编译期：RetentionPolicy.SOURCE；
    仅class文件：RetentionPolicy.CLASS；
    运行期：RetentionPolicy.RUNTIME

*
*
* 元注解  @Repeatable  定义 Annotation 是否可重复
*
* 注意：复数 Annotation 的 元注解要和单数 Anonation 类型一样  当然 @Repeatable 这个不用加到复数那里
*
* */
@Target(ElementType.FIELD)
@Retention(RetentionPolicy.RUNTIME)
@Repeatable(Checks.class)
public @interface Check {
    int min() default 0;
    int max() default  100;
    int value() default 0;
}

