package com.tm.sdk.bridge.app;

import androidx.annotation.NonNull;

import com.tm.sdk.bridge.call.CallManager;
import com.tm.sdk.bridge.call.ICallback;
import com.tm.sdk.commonlib.output.LoggerWrapper;

public class BridgeUnrealApp extends BridgeApp implements ICallback {

    @Override
    public String OnCall(@NonNull String param) {
        CallManager.getInstance().setCallNotify( new CallNotify(this));
        //LoggerWrapper.getInstance().logDebug("### set call notify");
        return super.OnCall(param);
    }

    public native void Callback(String param);
}
