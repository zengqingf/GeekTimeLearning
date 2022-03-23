package com.tm.sdk.platform;

public enum CallbackMethodType {
    OnLogout("OnLogout"),
    TestGameCallback("TestGameCallback");

    public String value;
    CallbackMethodType(String value) {
        this.value = value;
    }
}
