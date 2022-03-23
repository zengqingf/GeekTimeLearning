package com.tm.sdk.open.src.inter;

import java.util.concurrent.ConcurrentHashMap;

public interface ICallbackHandle {
    void onCallback(ConcurrentHashMap<String, Object> args);
}
