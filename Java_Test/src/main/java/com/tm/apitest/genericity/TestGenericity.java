package com.tm.apitest.genericity;

import java.util.ArrayList;
import java.util.List;

public class TestGenericity<T> {
    List<Number> list = new ArrayList<Number>();
    List<Number> list2 = new ArrayList<>();
    List<Number> list3 = new ArrayList();


    private T first, last;

    public TestGenericity(T first, T last){
        this.first = first;
        this.last = last;
    }

    // 静态泛型方法  应该使用其他类型名 （k） 作为区分
    // static 后的 <K> 和 TestGenericity类后的 <T> 没有关系
    public static <K> TestGenericity<K> create(K first, K last) {
        return new TestGenericity<>(first, last);
    }
    // static 后的 <T> 和 TestGenericity类后的 <T> 没有关系
    public static <T> TestGenericity<T> create2(T first, T last) {
        return new TestGenericity<>(first, last);
    }
}
