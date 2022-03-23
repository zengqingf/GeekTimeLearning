package com.tm.sdk.bridge.app;

import com.tm.sdk.bridge.call.ICallback;

public interface IBridgeApp {
    /*
    上层调用
    * */
    String OnCall(String param);

    void SetCallback(ICallback callback);
}
