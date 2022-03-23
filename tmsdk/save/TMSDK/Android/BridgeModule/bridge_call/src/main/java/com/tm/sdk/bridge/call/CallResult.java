package com.tm.sdk.bridge.call;

public class CallResult {
    public static final int SUCCESS = 0;
    public static final int EXCEPTION = -1;

    /*
     * 返回码
     * */
    public int code = 0;
    /*
     * 返回信息
     * */
    public String message;

    /*
     * 返回对象
     * */
    public Object obj;
}
