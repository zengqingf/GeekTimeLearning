package com.tm.sdk.bridge.call;

import java.util.List;

/*
* call protocol between game and android
* */
public interface ICallInfo {
    String getName();

    List<CallArg> getArgs();

    boolean isCallback();

    String getCallbackId();   //32位随机字符串
}
