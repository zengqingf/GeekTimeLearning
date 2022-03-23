package com.tm.sdk.bridge.app;

import com.alibaba.fastjson.JSONObject;
import com.tm.sdk.bridge.call.CallManager;
import com.tm.sdk.bridge.call.CallResult;
import com.tm.sdk.bridge.call.CallResultUtils;
import com.tm.sdk.bridge.call.ICallback;
import com.tm.sdk.commonlib.output.LoggerWrapper;

public class BridgeApp implements IBridgeApp {
    protected ICallback callback;

    @Override
    public String OnCall(String param) {
        CallResult result;
        try {
            //LoggerWrapper.getInstance().logDebug("### engine call, param : %s", param);

            Object obj = CallManager.getInstance().invokeCall(param);
            //LoggerWrapper.getInstance().logDebug("### do call");

            result = CallResultUtils.getInstance().getSuccessResult(obj);
        }catch (Exception ex) {
            result = CallResultUtils.getInstance().getErrorResult(ex);
        }
        return JSONObject.toJSONString(result);
    }

    @Override
    public void SetCallback(ICallback callback) {
        this.callback = callback;
    }
}
