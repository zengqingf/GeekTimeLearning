package com.tm.sdk.open.src.type;

public enum ServiceType {
    INIT(0),
    LOGIN(1),
    LOGOUT(2),
    EXIT(3),
    PAY(4);

    public final int value;
    ServiceType(int value) {
        this.value = value;
    }
}
