package com.tm.sdk.bridge.app;

public class BridgeUnrealApp implements IBridgeApp {

    public native void testCall(String function, String params);

    @Override
    public void Request(String funcName, String params) {
        testCall(funcName, params);
    }
}
