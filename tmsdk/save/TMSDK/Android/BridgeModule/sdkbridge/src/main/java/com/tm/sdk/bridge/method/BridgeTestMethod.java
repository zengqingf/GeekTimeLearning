package com.tm.sdk.bridge.method;

import com.tm.sdk.bridge.BridgeUtils;

public class BridgeTestMethod extends BridgeMethod {
    private final static String FuncName = "Test";

    @Override
    public void Callback(String value) {

    }

    @Override
    public void CallApp() {
        BridgeUtils.request(FuncName, "", this);
    }
}
