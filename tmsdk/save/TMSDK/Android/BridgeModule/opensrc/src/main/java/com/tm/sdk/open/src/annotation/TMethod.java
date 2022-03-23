package com.tm.sdk.open.src.annotation;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

@Target(ElementType.METHOD)
@Retention(RetentionPolicy.RUNTIME)
public @interface TMethod {
    boolean hasParams() default false;
    boolean needCallback() default false;
    String value() default "defalut";
}