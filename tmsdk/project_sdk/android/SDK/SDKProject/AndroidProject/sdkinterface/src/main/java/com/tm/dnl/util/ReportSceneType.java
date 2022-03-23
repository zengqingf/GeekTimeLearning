package com.tm.dnl.util;

/**
 * Created by tengmu on 2019/8/6.
 */
public enum ReportSceneType {
    None(0),
    Login(1),
    CreateRole(2),
    LevelUp(3),
    Logout(4);

    private int mCode;
    private ReportSceneType(int _code)
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
