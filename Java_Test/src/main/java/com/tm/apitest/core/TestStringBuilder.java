package com.tm.apitest.core;

public class TestStringBuilder {
    public void test1() {

        //链式调用
        /*
        链式调用的类的构建方法
        * 调用方法返回this
        * */
        StringBuilder sb = new StringBuilder(1024);
        sb.append("Mr ")
                .append("Bob")
                .append("!")
                .insert(0, "Hello, ");
        System.out.println(sb.toString());


        /*
        * 对于普通的字符串+操作，并不需要我们将其改写为StringBuilder，
        * 因为Java编译器在编译时就自动把多个连续的+操作编码为StringConcatFactory的操作。
        * 在运行期，StringConcatFactory会自动把字符串连接操作优化为数组复制或者StringBuilder操作
        *
        * */
        /*
        * StringBuffer 线程安全版本 通过同步来保证多个线程操作StringBuffer是安全，但是同步也降低了执行速度
        * StringBuilder和StringBuffer接口完全相同，现在完全没有必要使用StringBuffer。
         * */
    }
}
