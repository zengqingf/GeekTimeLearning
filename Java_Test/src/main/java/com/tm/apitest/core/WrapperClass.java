package com.tm.apitest.core;

/*
* 包装类型
* Java核心库提供的包装类型可以把基本类型包装为class
*
* 基本类型	对应的引用类型
boolean	    java.lang.Boolean
byte	    java.lang.Byte
short	    java.lang.Short
int	        java.lang.Integer
long	    java.lang.Long
float	    java.lang.Float
double	    java.lang.Double
char	    java.lang.Character
*
*
* 自动装箱和自动拆箱都是在编译期完成的（JDK>=1.5）
*
* 装箱和拆箱会影响执行效率，且拆箱时可能发生NullPointerException；
*
* 包装类型的比较必须使用equals()
*
* 整数和浮点数的包装类型都继承自Number
*
* */
public class WrapperClass {

    public void test1() {
        int i = 100;
        // 通过new操作符创建Integer实例(不推荐使用,会有编译警告):
        Integer n1 = new Integer(i);
        // 通过静态方法valueOf(int)创建Integer实例:
        Integer n2 = Integer.valueOf(i);
        // 通过静态方法valueOf(String)创建Integer实例:
        Integer n3 = Integer.valueOf("100");
        System.out.println(n3.intValue());

        //Auto Boxing 自动装箱
        //Auto Unboxing 自动拆箱
        //自动装箱和自动拆箱都是在编译期完成的（JDK>=1.5）
        Integer ni = Integer.valueOf(i);
        int xi = ni.intValue();

        //比较
        /*
        ==比较，较小的两个相同的Integer返回true，较大的两个相同的Integer返回false，
        因为Integer是不变类，编译器把Integer x = 127;自动变为Integer x = Integer.valueOf(127);
        为了节省内存，Integer.valueOf()对于较小的数，始终返回相同的实例，因此，==比较“恰好”为true，
        不能因为Java标准库的Integer内部有缓存优化就用==比较，
        必须用equals()方法比较两个Integer
        * */
        Integer x = 127;
        Integer y = 127;
        Integer m = 99999;
        Integer n = 99999;
        System.out.println("x == y: " + (x==y)); // true
        System.out.println("m == n: " + (m==n)); // false
        System.out.println("x.equals(y): " + x.equals(y)); // true
        System.out.println("m.equals(n): " + m.equals(n)); // true


        //静态工厂方法
        /*
        * 把能创建“新”对象的静态方法称为静态工厂方法。
        *
        * Integer.valueOf()就是静态工厂方法，它尽可能地返回缓存的实例以节省内存。
         * */

        //返回的Byte实例都是缓存实例
        //Byte.valueOf();

        /*
        * 程序设计的原则：
        * 数据的存储和显示要分离
        *
        * */
        System.out.println(Integer.toString(100)); // "100",表示为10进制
        System.out.println(Integer.toString(100, 36)); // "2s",表示为36进制
        System.out.println(Integer.toHexString(100)); // "64",表示为16进制
        System.out.println(Integer.toOctalString(100)); // "144",表示为8进制
        System.out.println(Integer.toBinaryString(100)); // "1100100",表示为2进制


        // boolean只有两个值true/false，其包装类型只需要引用Boolean提供的静态字段:
        Boolean t = Boolean.TRUE;
        Boolean f = Boolean.FALSE;
        // int可表示的最大/最小值:
        int max = Integer.MAX_VALUE; // 2147483647
        int min = Integer.MIN_VALUE; // -2147483648
        // long类型占用的bit和byte数量:
        int sizeOfLong = Long.SIZE; // 64 (bits)
        int bytesOfLong = Long.BYTES; // 8 (bytes)

        // 向上转型为Number:
        Number num = new Integer(999);
        // 获取byte, int, long, float, double:
        byte b = num.byteValue();
        int nint = num.intValue();
        long ln = num.longValue();
        float fl = num.floatValue();
        double d = num.doubleValue();

        // 无符号整型
        byte b1 = -1;   //有符号整型的范围 -128 ~  127   无符号整型范围 0 ~ 255
        byte b2 = 127;
        System.out.println(Byte.toUnsignedInt(b1)); // 255
        System.out.println(Byte.toUnsignedInt(b2)); // 127

        /*
        *
        * 因为byte的-1的二进制表示是11111111  (补码形式 表示)，以无符号整型转换后的int就是255。
          类似的，可以把一个short按unsigned转换为int，把一个int按unsigned转换为long
        * */
    }
}
