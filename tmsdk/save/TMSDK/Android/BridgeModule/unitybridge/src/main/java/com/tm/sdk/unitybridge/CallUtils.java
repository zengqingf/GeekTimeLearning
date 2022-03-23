package com.tm.sdk.unitybridge;

import com.tm.sdk.unitybridge.service.UnityServiceManager;

import java.util.UUID;

public class CallUtils {

    public static boolean callIsCallback(ICallInfo callInfo) {
        if(null == callInfo) {
            return false;
        }
        return callInfo.isCallback();
    }

    public static boolean callHasCallback(ICallInfo callInfo) {
        if(null == callInfo) {
            return false;
        }
        if(callInfo.getCallbackId() == null || callInfo.getCallbackId().length() <= 0) {
            return false;
        }
        return true;
    }

    public static Object invokeCall(ICallInfo callInfo) {
        return UnityServiceManager.getInstance().invokeCall(callInfo);
    }

    public static String getUUID() {
        return UUID.randomUUID().toString().replace("-", "");
    }
}
