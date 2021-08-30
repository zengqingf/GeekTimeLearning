package com.tm.apitest.base;

public class Base2 {
    /*
    * 输入 输出
    *
    * System.out.println() / print() / printf()
    * printf 格式化
    *   %d	格式化输出整数
        %x	格式化输出十六进制整数
        %f	格式化输出浮点数
        %e	格式化输出科学计数法表示的浮点数
        %s	格式化字符串

    System.out.printf("n=%d, hex=%08x", n, n); // 注意，两个%占位符必须传入两个数
    *
    *   import java.util.Scanner;
    *   Scanner scanner = new Scanner(System.in); // 创建Scanner对象
        System.out.print("Input your name: "); // 打印提示
        String name = scanner.nextLine(); // 读取一行输入并获取字符串
        System.out.print("Input your age: "); // 打印提示
        int age = scanner.nextInt(); // 读取一行输入并获取整数
        System.out.printf("Hi, %s, you are %d\n", name, age); // 格式化输出
    * */

    /*
    *  流程控制
    *
    *  多个if串联 要特别注意判断顺序
    *  注意边界条件
    *  注意 浮点数用==判断不靠谱  用差值的绝对值判断
    *
    *
    * */

    //判断是否相等：
/*
* 在Java中，判断值类型的变量是否相等，可以使用==运算符。但是，判断引用类型的变量是否相等，==表示“引用是否相等”，
* 或者说，是否指向同一个对象。
* 例如，下面的两个String类型，它们的内容是相同的，但是，分别指向不同的对象，用==判断，结果为false：
*
*  String s1 = "hello";
   String s2 = "HELLO".toLowerCase();
   *
  要判断引用类型的变量内容是否相等，必须使用equals()方法：
  注意 s1.equals(s2)  如果s1 为空时 会判空引用
  * 建议方法 如果用equals比较是 有必定不为空的值 可以
  * “hello”.equals(s1)
  *
  *

  *
  switch case
  * 1.case 具有穿透性 不写break会出现异常结果
  * 2.case 叠加 多个case执行的代码块一样，可以叠加写
  * 3.case 可以比较字符串，比较的是内容相等
  *
  *
  * Java14 新语法 不支持Java8
  *     int opt = switch (fruit) {
            case "apple" -> 1;
            case "pear", "mango" -> 2;
            default -> 0;
        }; // 注意赋值语句要以;结束

    yield新语法
        int opt = switch (fruit) {
            case "apple" -> 1;
            case "pear", "mango" -> 2;
            default -> {
                int code = fruit.hashCode();
                yield code; // switch语句返回值
            }
        };
 * */
}
