package com.tm.apitest.base;

import java.util.Arrays;

public class Base4 {

    /*
    * 二维数组的每个数组元素的长度并不要求相同
    *
    *
    * */
    public static void two_dimentional_array () {
        int[][] ns = {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 }
        };
        System.out.println(ns.length); // 3


        //打印二维数组
        //1. 使用内置方法
        //2. 双重for
        System.out.println(Arrays.deepToString(ns));
        for (int[] arr : ns) {
            for (int n : arr) {
                System.out.print(n);
                System.out.print(", ");
            }
            System.out.println();
        }
    }


    /*
    * 画图方式
    *
    * 更直观
    *
    * ref : https://www.liaoxuefeng.com/wiki/1252599548343744/1259544232593792
    *
    * */
    public static void three_dimentional_array() {
        int[][][] ns = {
                {
                        {1, 2, 3},
                        {4, 5, 6},
                        {7, 8, 9}
                },
                {
                        {10, 11},
                        {12, 13}
                },
                {
                        {14, 15, 16},
                        {17, 18}
                }
        };
    }


    //二维数组就是数组的数组，三维数组就是二维数组的数组
    //多维数组的每个数组元素长度都不要求相同
    //最常见的多维数组是二维数组，访问二维数组的一个元素使用array[row][col]
}
