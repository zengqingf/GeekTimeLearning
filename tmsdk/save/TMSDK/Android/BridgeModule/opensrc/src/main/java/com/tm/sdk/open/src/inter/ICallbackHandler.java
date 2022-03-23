package com.tm.sdk.open.src.inter;

public interface ICallbackHandler {
    void clearParams();
    void addParam(String name, Object value);
    void setCallback(ICallbackHandle handle);
}
