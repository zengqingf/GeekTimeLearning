package com.tm.apitest.base;

public class Base3 {
    /*
    * while 死循环
    * */
    //表面上看，上面的while循环是一个死循环，但是，Java的int类型有最大值，达到最大值后，再加1会变成负数，结果，意外退出了while循环。
    public static void test_while() {
        int sum = 0;
        int n = 1;
        while (n > 0) {
            sum = sum + n;
            n ++;
        }
        System.out.println(n); // -2147483648
        System.out.println(sum);


        while (n <= 100) { // 循环条件是n <= 100
            sum = sum + n; // 把n累加到sum中
            n ++; // n自身加1
        }
        System.out.println(sum); // 5050
    }

    //在Java中，while循环是先判断循环条件，再执行循环。而另一种do while循环则是先执行循环，再判断条件，条件满足时继续循环，条件不满足时退出。
    //do while循环会至少循环一次
    public static void test_dowhile() {
        int sum = 0;
        int n = 1;
        do {
            sum = sum + n;
            n ++;
        } while (n <= 100);
        System.out.println(sum);
    }

    //使用for循环时，千万不要在循环体内修改计数器！在循环体中修改计数器常常导致莫名其妙的逻辑错误。
    //如果希望只访问索引为奇数的数组元素， i = i + 2  避免了在循环体内去修改变量i
    public static void test_for_cycle() {
        int[] ns = { 1, 4, 9, 16, 25 };
        for (int i=0; i<ns.length; i=i+2) {
            System.out.println(ns[i]);
        }


        //使用for循环时，计数器变量i要尽量定义在for循环中：
        int ii;
        for (ii=0; ii<ns.length; ii++) {
            System.out.println(ns[ii]);
        }
        // 仍然可以使用i
        // 破坏了变量应该把访问范围缩到最小的原则 !
        int n = ii;


        //可以省略写法 特殊情况使用
        /*
        for (;;) {
        }
        for (int i = 0; ;) {
        }
        for (int i = 0; ; i++) {
        }
         */


        //for each 写法
        //for each循环能够遍历所有“可迭代”的数据类型
        for (int ni : ns) {
            System.out.println(ni);
        }
    }


    /*
    * 圆周率π可以使用公式计算
    * pi / 4 = 1 - 1/3 + 1/5 - 1/7 + 1/9 - ...
    * */
    public static void cal_pi_value() {
        double pi = 0;
        //解法 1
        /*
        for (double i = 1, j = 1; i < 10000; i += 2) {
            pi += (j % 2 != 0 ? 1 : -1) * ( 1 / i);
            j++;
        }*/

        //解法 2
        for(double i = 1; i < 99999; i += 4) {
            pi += 4 / i;
            pi -= 4 / (i + 2);
        }
        System.out.println(pi);
    }

    //break语句总是跳出自己所在的那一层循环

    //continue则是提前结束本次循环，直接继续执行下次循环
    //在多层嵌套的循环中，continue语句同样是结束本次自己所在的循环

}
