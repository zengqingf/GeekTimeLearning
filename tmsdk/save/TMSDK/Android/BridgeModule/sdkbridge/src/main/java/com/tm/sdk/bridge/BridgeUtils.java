package com.tm.sdk.bridge;

import com.tm.sdk.bridge.app.BridgeUnityApp;
import com.tm.sdk.bridge.app.BridgeUnrealApp;
import com.tm.sdk.bridge.app.IBridgeApp;
import com.tm.sdk.bridge.method.BridgeMethod;

import java.util.concurrent.ConcurrentHashMap;

public class BridgeUtils {

    private static ConcurrentHashMap<Integer, BridgeMethod> mCallbacks = new ConcurrentHashMap<>();

    private static IBridgeApp mBridgeApp = null;

    //改为工厂模式
    static
    {
        try {
            Class<?> clazz = Class.forName("com.unity3d.player.UnityPlayer");
            if(clazz != null){
                mBridgeApp = new BridgeUnityApp(clazz.getMethod("UnitySendMessage", String.class, String.class, String.class));
            }
        }catch (Exception e) {
            mBridgeApp = new BridgeUnrealApp();
        }
    }

    public static void request(String funcName, String params, BridgeMethod handler)
    {
        try {
            String temp = params;
            if(handler != null)
            {
                int key = handler.Key();
                if(!mCallbacks.containsKey(key)) {
                    mCallbacks.put(key, handler);
                }
                temp = String.format("%s%s%d", temp, BridgeConstant.SplitFlag, key);
            }
            mBridgeApp.Request(funcName, temp);
        }catch (Exception e)
        {
            e.printStackTrace();
        }
    }

    public static void respone(int key, String result) {
        if(mCallbacks.containsKey(key)) {
            mCallbacks.get(key).Callback(result);
        }
    }

    public static void remove(BridgeMethod handler) {
        if(handler != null) {
            int key = handler.Key();
            if(!mCallbacks.containsKey(key)) {
                mCallbacks.remove(key);
            }
        }
    }
}
