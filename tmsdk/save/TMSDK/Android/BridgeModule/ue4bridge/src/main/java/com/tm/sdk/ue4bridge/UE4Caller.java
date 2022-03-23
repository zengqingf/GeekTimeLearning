package com.tm.sdk.ue4bridge;

import androidx.annotation.NonNull;

import com.tm.sdk.bridge.call.ICallInfo;

public class UE4Caller {

    private static class SingletonHolder {
        private static final UE4Caller instance = new UE4Caller();
    }

    public static UE4Caller getInstance() {return UE4Caller.SingletonHolder.instance;}

    public void doCall(@NonNull ICallInfo callInfo){
        if(callInfo == null) {
            return;
        }
        AndroidCaller.getInstance().Callback(callInfo.toString());
    }
}
