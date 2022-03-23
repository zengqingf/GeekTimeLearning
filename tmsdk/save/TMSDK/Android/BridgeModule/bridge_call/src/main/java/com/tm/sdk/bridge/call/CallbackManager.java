package com.tm.sdk.bridge.call;

import com.tm.sdk.open.src.inter.ICallbackHandle;
import java.util.concurrent.ConcurrentHashMap;

public class CallbackManager {

    private ConcurrentHashMap<String, ICallbackHandle> callbackHandleMap;

    private CallbackManager()
    {
        callbackHandleMap = new ConcurrentHashMap<>();
    }

    public void AddCallbackHandle(String id, ICallbackHandle handle) {
        if(callbackHandleMap == null) {
            return;
        }
        callbackHandleMap.putIfAbsent(id, handle);
    }

    public boolean invokeCallback(String id, ConcurrentHashMap<String, Object> args) {
        if(callbackHandleMap == null) {
            return false;
        }
        if(!callbackHandleMap.containsKey(id)) {
            return false;
        }
        ICallbackHandle handle = callbackHandleMap.get(id);
        if(handle != null) {
            handle.onCallback(args);
            callbackHandleMap.remove(id);
            return true;
        }
        return false;
    }

    private static class SingletonHolder {
        private static final CallbackManager instance = new CallbackManager();
    }

    public static CallbackManager getInstance() {
        return CallbackManager.SingletonHolder.instance;
    }
}
