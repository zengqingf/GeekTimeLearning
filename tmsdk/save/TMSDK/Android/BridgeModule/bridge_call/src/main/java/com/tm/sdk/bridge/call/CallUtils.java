package com.tm.sdk.bridge.call;

import android.text.TextUtils;

import java.util.List;
import java.util.UUID;
import java.util.concurrent.ConcurrentHashMap;

public class CallUtils {
    public static boolean isCallbackEnabled(ICallInfo callInfo) {
        if(null == callInfo) {
            return false;
        }
        if(!callInfo.isCallback()) {
            return false;
        }
        //notice: not check callbackId, callbackId just for call not callback, callback command's name will be callbackId
        if(TextUtils.isEmpty(callInfo.getName())) {
            return false;
        }
        return true;
    }

    public static String getUUID() {
        return UUID.randomUUID().toString().replace("-", "");
    }

    public static ConcurrentHashMap<String, Object> getCallArgsMap(List<CallArg> args) {
        if(args == null || args.size() <= 0) {
            return null;
        }
        ConcurrentHashMap<String, Object> argsMap = new ConcurrentHashMap<>();
        for (CallArg arg : args) {
            if (arg == null) {
                continue;
            }
            argsMap.putIfAbsent(arg.name, arg.value);
        }
        return argsMap;
    }

    public static boolean invokeCallback(ICallInfo callInfo) {
        if(callInfo == null) {
            return false;
        }
        ConcurrentHashMap<String, Object> argsMap = getCallArgsMap(callInfo.getArgs());
        boolean isSucc = CallbackManager.getInstance().invokeCallback(callInfo.getName(), argsMap);
        return isSucc;
    }
}
