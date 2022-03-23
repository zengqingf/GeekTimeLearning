package com.tm.sdk.bridge.app;

import com.tm.sdk.bridge.call.CallManager;
import com.tm.sdk.bridge.call.ICallback;
import com.tm.sdk.commonlib.output.LoggerWrapper;

//import java.lang.reflect.InvocationTargetException;
//import java.lang.reflect.Method;

public class BridgeUnityApp extends BridgeApp {

    //private Method mUnitySendMsg = null;
    //private String mUnityGameObj = "TMSDK";

    @Override
    public String OnCall(String param) {
        CallManager.getInstance().setCallNotify(new CallNotify(this.callback));
        //LoggerWrapper.getInstance().logDebug("### set call notify");
        return super.OnCall(param);
    }

    /*
    public void DoCall(String methodName, String param) {
        try {
            mUnitySendMsg.invoke(null, mUnityGameObj, methodName, param);
        } catch (IllegalAccessException e) {
            e.printStackTrace();
        } catch (InvocationTargetException e) {
            e.printStackTrace();
        }
    }*/
}
