package com.tm.sdk.bridge.service;

import com.tm.sdk.open.src.inter.ICallbackHandle;
import com.tm.sdk.open.src.inter.ICallbackHandler;

import java.util.concurrent.ConcurrentHashMap;

public class CallbackHandler implements ICallbackHandler {
    private String callbackId;
    private ConcurrentHashMap<String, Object> params;

    //set this for game callback android
    private ICallbackHandle callbackHandle = null;

    private boolean isCallback = false;

    /*
    * bCallback
    * true : game call android interface, and android interface contains CallbackHandler
    * false : android call game
    * */
    public CallbackHandler(String callbackId, boolean bCallback) {
        this.callbackId = callbackId;
        params = new ConcurrentHashMap<>();
        isCallback = bCallback;
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
    public void setCallback(ICallbackHandle callback) {
        this.callbackHandle = callback;
    }

    public void call() {
        ServiceManager.getInstance().doCall(callbackId, params, callbackHandle, isCallback);
    }
}
