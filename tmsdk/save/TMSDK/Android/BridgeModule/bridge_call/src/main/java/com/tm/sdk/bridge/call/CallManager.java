package com.tm.sdk.bridge.call;

import com.alibaba.fastjson.JSONObject;
import com.tm.sdk.bridge.exception.BridgeRuntimeException;
import com.tm.sdk.commonlib.output.LoggerWrapper;

import java.util.concurrent.ConcurrentHashMap;

public class CallManager {

    //on game call
    public Object invokeCall(String command) {
        //test
        /*
        try {
            CallInfo ci = JSONObject.parseObject(
                    "{\"name\":\"login\",\"args\":[{\"name\" : \"isDebug\", \"value\": true}],\"isCallback\":false,\"callbackId\":\"4571bde2ef67461586887abacdd8fec5\"}",
                    CallInfo.class);
            LoggerWrapper.getInstance().logDebug("#################  %s, %s, %s", ci.getName(), ci.getCallbackId(), ci.isCallback() + "");
        }catch (Exception e)
        {
            throw new BridgeRuntimeException(String.format("##################################. %s", e.toString()));
        }
         */

        Object callRes = null;
        ICallInfo callInfo = null;
        try {
            callInfo = CallInfo.Builder.create().build(command);
        } catch (Exception e) {
            throw new BridgeRuntimeException(String.format("Serialize json error, please check unity call json correctly or not. %s", e.toString()));
        }
        boolean commandAvailable = checkCallInfoAvailable(callInfo);
        if(!commandAvailable) {
            throw new BridgeRuntimeException("Call json format incorrect, please check unity call json correctly or not.");
        }
        if(CallUtils.isCallbackEnabled(callInfo)) {
            LoggerWrapper.getInstance().logInfo("call is callback type");
            callRes = CallUtils.invokeCallback(callInfo);
        }else {
            LoggerWrapper.getInstance().logInfo("call is call type");
            callRes = invokeCall(callInfo);
        }
        return callRes;
    }

    private Object invokeCall(ICallInfo callInfo) {
        if(callInvoke == null) {
            return null;
        }
        return callInvoke.invokeCall(callInfo);
    }

    //call game
    public void doCall(ICallInfo callInfo) {
        LoggerWrapper.getInstance().logInfo("do call , call notify is %s", callNotify == null ? "null" : "not null");
        if(callNotify == null) {
            return;
        }
        LoggerWrapper.getInstance().logInfo("do call , callinfo name is %s, callbackId is %s, isCallback is %s, arg size is %s",
                callInfo.getName(), callInfo.getCallbackId(), callInfo.isCallback(), callInfo.getArgs().size());
        callNotify.doCall(callInfo);
    }

    private boolean checkCallInfoAvailable(ICallInfo callInfo) {
        if(null == callInfo) {
            return false;
        }
        //注意判断顺序 先判断 == null
        if(callInfo.getName() == null || callInfo.getName().length() == 0) {
            return false;
        }
        return true;
    }

    private ICallNotify callNotify;
    public void setCallNotify(ICallNotify callNotify) {
        this.callNotify = callNotify;
    }

    private ICallInvoke callInvoke;
    public void setCallInvoke(ICallInvoke callInvoke) {
        this.callInvoke = callInvoke;
    }

    private static class SingletonHolder {
        private static final CallManager instance = new CallManager();
    }

    public static CallManager getInstance() {
        return SingletonHolder.instance;
    }
}
