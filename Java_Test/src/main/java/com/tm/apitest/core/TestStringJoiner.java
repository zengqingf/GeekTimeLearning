package com.tm.apitest.core;

import java.util.StringJoiner;

public class TestStringJoiner {

    public void test1() {
        String[] names = {"Bob", "Alice", "Grace"};
        StringJoiner sj = new StringJoiner(", ", "Hello ", "!");  //可以添加开头结尾字符串
        for (String name : names) {
            sj.add(name);
        }
        System.out.println(sj.toString());

        //如果不需要添加开头结尾，直接使用join
        String s = String.join(", ", names);
    }
}
