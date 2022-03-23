package com.tm.sdk.bridge.call;

public class CallResultUtils {
    private static class SingletonHolder {
        private static final CallResultUtils instance = new CallResultUtils();
    }

    public static CallResultUtils getInstance() {
        return CallResultUtils.SingletonHolder.instance;
    }

    private CallResult callResult;

    public CallResultUtils() {
        callResult = new CallResult();
    }

    public CallResult getSuccessResult(Object res) {
        callResult.code = CallResult.SUCCESS;
        callResult.message = "";
        callResult.obj = res == null ? new Object() : res;
        return callResult;
    }

    public CallResult getErrorResult(Object res) {
        callResult.code = CallResult.EXCEPTION;
        callResult.message = res.toString();
        callResult.obj = new Object();
        return callResult;
    }
}
