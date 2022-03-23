package com.tm.sdk.platform;

public enum CallbackParamType {

    openuid("openuid"),
    token("token"),
    bindphone("bindphone"),
    timestamp("timestamp"),
    level("level"),

    paysuccess("paysuccess", 0),
    payfailed("payfailed", 1),
    paycancal("paycancal", 2),

    none("none", -1);


    public String value;
    public int valueInt;
    CallbackParamType(String value, int valueInt) {
        this.value = value;
        this.valueInt = valueInt;
    }
    CallbackParamType(String value) {
        this.value = value;
    }
}
