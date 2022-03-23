package com.tm.sdk.ue4bridge;

import com.tm.sdk.bridge.call.ICallInfo;
import com.tm.sdk.bridge.call.ICallNotify;
import com.tm.sdk.bridge.call.ICallback;

public class CallNotify implements ICallNotify {
    /*
    * 主动调用上层
    * */
    @Override
    public void doCall(ICallInfo callInfo) {
        UE4Caller.getInstance().doCall(callInfo);
    }

    /*
    * 回调上层
    * */
    @Override
    public void doCallback(ICallInfo callInfo) {
        ICallback iCallback =  AndroidCaller.getInstance();
        if(iCallback != null){
            iCallback.Callback(callInfo.toString());
        }
    }
}
