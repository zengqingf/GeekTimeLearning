package com.tm.sdk.ue4bridge;


import androidx.annotation.NonNull;

import com.alibaba.fastjson.JSONObject;
import com.tm.sdk.bridge.call.CallManager;
import com.tm.sdk.bridge.call.CallResult;
import com.tm.sdk.bridge.call.CallResultUtils;
import com.tm.sdk.bridge.call.ICallback;
import com.tm.sdk.commonlib.output.LoggerWrapper;

public class AndroidCaller implements ICallback {

    private static class SingletonHolder {
        private static final AndroidCaller instance = new AndroidCaller();
    }

    public static AndroidCaller getInstance() {return SingletonHolder.instance;}

    public String call(@NonNull String param) {
        CallResult result;
        try {
            LoggerWrapper.getInstance().logDebug("### engine call, param : %s", param);

            CallManager.getInstance().setCallNotify(new CallNotify());
            LoggerWrapper.getInstance().logDebug("### set call notify");

            Object obj = CallManager.getInstance().invokeCall(param);
            LoggerWrapper.getInstance().logDebug("### do call");

            result = CallResultUtils.getInstance().getSuccessResult(obj);
        }catch (Exception ex) {
            result = CallResultUtils.getInstance().getErrorResult(ex);
        }
        return JSONObject.toJSONString(result);
    }

    public native void Callback(String param);


    //Test
    AndroidCaller() {
        SetCallback(this);
    }
    public native void SetCallback(AndroidCaller caller);
    public String GameCallback(String param) {
        //TODO callback from c++
        return "";
    }
    public native void CallUE4(String param, ICallback callback) ;
}
