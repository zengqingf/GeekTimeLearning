package com.tm.apitest.annotation;

public class TestAnnotation {
    @Check(min=0, max=100, value=55)
    public int n;

    @Check
    public int p;

    @Check(value = 99)
    public int t;

    @Check(99)
    public int s;
}
