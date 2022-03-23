package com.tm.sdk.unitybridge;

import java.io.Serializable;
import java.util.List;

public interface ICallInfo extends Serializable {

    String getName();

    List<CallArg> getArgs();

    boolean isCallback();

    String getCallbackId();   //32位随机字符串
}
