package com.tm.apitest.core;

import java.nio.charset.StandardCharsets;
import java.util.Arrays;

public class TestString {

    //Java字符串的一个重要特点就是字符串不可变。这种不可变性是通过内部的private final char[]字段，以及没有任何修改char[]的方法实现的。
    String s1 = new String(new char[] {'h', 'e', 'l', 'l', 'o'} );
    String s2 = "hello";
    String s3 = "hello";
    String s4 = "HELLO".toLowerCase();
    String s5 = " \thello\r\n";
    String s6 = " \t\u3000hello\u3000";
    String s7 = " \n";
    String s8 = "";

    public void test1() {

        //比较字符串内容是否相同，必须使用equals 而不能用 ==
        //两个字符串比较，必须总是使用equals()方法
        //要忽略大小写比较，使用equalsIgnoreCase()方法
        //
        System.out.println("s1 == s2 ? " + (s1 == s2 ? "true" : "false"));
        System.out.println("s1 equals s2 ? " + (s1.equals(s2) ? "true" : "false"));
        System.out.println("s2 == s3 ? " + (s2 == s3 ? "true" : "false"));
        System.out.println("s2 equals s3 ? " + (s2.equals(s3) ? "true" : "false"));

        System.out.println("s3 == s4 ? " + (s3 == s4 ? "true" : "false"));
        System.out.println("s3 equals s4 ? " + (s3.equals(s4) ? "true" : "false"));

        //字符串不能改变，但是 变量指向 哪个字符串 可以改变
        System.out.println("s2 = " + s2);
        s2 = s2.toUpperCase();
        System.out.println("s2 = " + s2);

        System.out.println("contains : " + s2.contains("ll"));
        System.out.println("trim : " + s5.trim());
        //System.out.println("strip : " + s6.strip());  //java 11 可以移除 中文空格字符
        System.out.println("isEmpty : " + s8.isEmpty());
        //System.out.println("isBlank : " + s7.isBlank()); //java 11  空白字符

        //字符串拼接
        String[] arr = {"A", "B", "C"};
        String s = String.join("***", arr);
        System.out.println("String join : " + s);


        //format
        //%s    字符串
        //%d    整数
        //%x    十六进制整数
        //%f    显示浮点数
        //%.2f  显示两位小数
        System.out.println(String.format("Hi %s, your score is %.2f!", "Bob", 59.5));

        //String 类型转换
        System.out.println(String.valueOf(123));
        int n1 = Integer.parseInt("123");
        boolean b1 = Boolean.parseBoolean("true");

        //注意  Integer有个getInteger(String)方法，它不是将字符串转换为int，而是把该字符串对应的系统变量转换为Integer
        //System.getProperty
        System.out.println("java.version ==> Integer.getInteger: " + Integer.getInteger("java.version"));


        //String ==> char[]
        char[] cs = "Hello".toCharArray();
        //char[] ==> String
        String s8 = new String(cs);
        //不改变String的内容
        //new String(char[])创建新的String实例时，它并不会直接引用传入的char[]数组，而是会复制一份，
        // 所以，修改外部的char[]数组不会影响String实例内部的char[]数组，因为这是两个不同的数组。
        cs[0] = 'X';


        //如果传入的对象有可能改变，我们需要复制而不是直接引用
        int[] scores = new int[] { 88, 77, 51, 66 };
        Score s10 = new Score(scores);
        s10.printScores();
        scores[2] = 99;
        s10.printScores();


        /*
        * 字符编码
        *
        * Java中的String 和 Char  在内存中总是以 Unicode编码 表示的
        *
        * 转换编码就是将String和byte[]转换，需要指定编码；
        * 转换为byte[]时，始终优先考虑UTF-8编码。
        */
        //byte[] bt1 = "Hello".getBytes(); // 按系统默认编码转换，不推荐
        //byte[] bt2 = "Hello".getBytes("UTF-8"); // 按UTF-8编码转换
        //byte[] bt3 = "Hello".getBytes("GBK"); // 按GBK编码转换
        //byte[] bt4 = "Hello".getBytes(StandardCharsets.UTF_8); // 按UTF-8编码转换
        //String str1 = new String(bt2, "GBK"); // 按GBK转换
        //String str2 = new String(bt3, StandardCharsets.UTF_8); // 按UTF-8转换


        /*
        * 不同版本Java 对String在内存中的优化
        * 早期JDK版本的String总是以char[]存储
        * 较新的JDK版本的String则以byte[]存储
        *
        * 如果String仅包含ASCII字符，则每个byte存储一个字符，否则，每两个byte存储一个字符，
        * 这样做的目的是为了节省内存，因为大量的长度较短的String通常仅包含ASCII字符：
        * */
    }


    class Score {
        private int[] scores;
        public Score(int[] scores) {
            //如果传入的对象有可能改变，我们需要复制而不是直接引用
            this.scores = scores.clone();
        }

        public void printScores() {
            System.out.println(Arrays.toString(scores));
        }
    }
}