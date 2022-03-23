package com.tm.sdk.bridge.call;

public class CallArg<T> {
    public String name;
    public T value;

    public CallArg() {
    }

    public CallArg(String name, T value) {
        this.name = name;
        this.value = value;
    }
}
