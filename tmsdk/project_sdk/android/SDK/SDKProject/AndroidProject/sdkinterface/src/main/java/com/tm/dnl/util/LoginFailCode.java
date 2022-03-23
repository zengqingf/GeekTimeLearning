package com.tm.dnl.util;

/**
 * Created by tengmu on 2019/8/6.
 */
public enum LoginFailCode {
    UNKONW(0),
    LOGINFAIL(1001),
    LOGINCANCEL(1002),
    APPIDNOTFOUND(1003),
    NOTINIT(2001);

    private int mCode;
    private LoginFailCode(int _code)
    {
        this.mCode = _code;
    }

    @Override
    public String toString()
    {
        return String.valueOf(this.mCode);
    }

    public int toInt()
    {
        return this.mCode;
    }
}
