package com.tm.sdk.bridge.method;

public class BridgeMethod
{
    public int Key() {
        return this.getClass().getName().hashCode();
    }

    public void Callback(String value) {}

    public void CallApp() {}
}
