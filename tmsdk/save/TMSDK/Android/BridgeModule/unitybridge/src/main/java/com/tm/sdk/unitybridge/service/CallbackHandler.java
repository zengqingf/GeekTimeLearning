package com.tm.sdk.unitybridge.service;

import com.tm.sdk.open.src.inter.ICallbackHandler;

import java.util.concurrent.ConcurrentHashMap;

public class CallbackHandler implements ICallbackHandler {

    private ConcurrentHashMap<String, Object> params;
    private String callbackId;
    public CallbackHandler(String callbackId) {
        params = new ConcurrentHashMap<>();
        this.callbackId = callbackId;
    }

    @Override
    public void clearParams() {
        params.clear();
    }

    @Override
    public void addParam(String name, Object value) {
        params.put(name, value);
    }

    @Override
    public void call() {
        UnityServiceManager.getInstance().sendCallbackCommand(callbackId, params);
    }
}
