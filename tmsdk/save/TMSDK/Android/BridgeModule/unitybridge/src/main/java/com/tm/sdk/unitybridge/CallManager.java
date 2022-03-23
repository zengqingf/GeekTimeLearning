package com.tm.sdk.unitybridge;

import com.tm.sdk.unitybridge.exception.UnityProxyRuntimeException;

/*
* Unity Call Android Result Code:
*
* 0 : Success
*
* 1001 : Serialize json error
*
* 1002 : Call json format incorrect
*
*
* */

public class CallManager implements IUnityCallListener {

    @Override
    public Object onUnitycall(String command) {
        Object callRes = null;
        CallInfo callInfo;
        try {
            callInfo = CallInfo.Builder.create().build(command);
        } catch (Exception e) {
            throw new UnityProxyRuntimeException(String.format("Serialize json error, please check unity call json correctly or not. %s", e.toString()));
        }
        boolean commandAvailable = checkCallInfoAvailable(callInfo);
        if(!commandAvailable) {
            throw new UnityProxyRuntimeException("Call json format incorrect, please check unity call json correctly or not.");
        }
        callRes = CallUtils.invokeCall(callInfo);
        /*
        if(CallUtils.callIsCallback(callInfo)) {
            callRes = CallbackUtils.invokeCallback(callInfo);
        }else {
            callRes = CallUtils.invokeCall(callInfo);
        }*/
        return callRes;
    }

    private boolean checkCallInfoAvailable(CallInfo callInfo) {
        if(null == callInfo) {
            return false;
        }
        //注意判断顺序 先判断 == null
        if(callInfo.getName() == null || callInfo.getName().length() == 0) {
            return false;
        }
        return true;
    }

    private static class SingletonHolder {
        private static final CallManager instance = new CallManager();
    }

    public static CallManager getInstance() {
        return SingletonHolder.instance;
    }
}
