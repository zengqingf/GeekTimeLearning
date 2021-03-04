package com.tm.apitest.core;

import java.math.BigDecimal;

public class TestBigNumber {

    /* BigInteger
    *
    *BigInteger用于表示任意大小的整数；
BigInteger是不变类，并且继承自Number；
将BigInteger转换成基本类型时可使用longValueExact()等方法保证结果准确。
*
* BigInteger的值范围超过float    表示为 Infinity
    *
    * */



    /* BigDecimal
    *
    *  总是使用compareTo()比较两个BigDecimal的值，不要使用equals()
    *  因为：使用equals()方法不但要求两个BigDecimal的值相等，还要求它们的scale()相等
    *
    * BigDecimal ==>  BigInteger + scale
     *
    * */
}
