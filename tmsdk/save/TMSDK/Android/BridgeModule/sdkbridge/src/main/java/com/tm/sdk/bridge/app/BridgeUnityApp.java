package com.tm.sdk.bridge.app;

import java.lang.reflect.Method;

public class BridgeUnityApp implements IBridgeApp {

    private final static String UnityGameObjectName = "TMSDK";
    private Method mUnitySendMessage = null;

    public BridgeUnityApp(Method method) {
        mUnitySendMessage = method;
    }

    @Override
    public void Request(String funcName, String params) {
        try {
            mUnitySendMessage.invoke(null, UnityGameObjectName, funcName, params);
        }catch (Exception e) {
            e.printStackTrace();
        }
    }
}
