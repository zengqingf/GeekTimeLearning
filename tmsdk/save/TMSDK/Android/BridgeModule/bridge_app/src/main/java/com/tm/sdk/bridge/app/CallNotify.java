package com.tm.sdk.bridge.app;

import com.tm.sdk.bridge.call.ICallInfo;
import com.tm.sdk.bridge.call.ICallNotify;
import com.tm.sdk.bridge.call.ICallback;
import com.tm.sdk.commonlib.output.LoggerWrapper;

public class CallNotify implements ICallNotify {

    ICallback callback = null;

    public CallNotify(ICallback callback) {
        this.callback = callback;
    }

    /*
     * 主动调用上层
     * */
    @Override
    public void doCall(ICallInfo callInfo) {
        LoggerWrapper.getInstance().logInfo("do call, try callback");
        _callback(callInfo.toString());
    }

    private void _callback(String param) {
        if(callback != null) {
            callback.Callback(param);
            LoggerWrapper.getInstance().logInfo("do call, callback params is %s", param);
        }
    }
}
