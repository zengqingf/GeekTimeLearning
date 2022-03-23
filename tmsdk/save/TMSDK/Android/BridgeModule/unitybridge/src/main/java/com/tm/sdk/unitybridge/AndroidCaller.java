package com.tm.sdk.unitybridge;

import android.util.Log;

import androidx.annotation.NonNull;

import com.alibaba.fastjson.JSONObject;
import com.tm.sdk.commonlib.output.LoggerWrapper;
import com.tm.sdk.unitybridge.util.Globals;

public class AndroidCaller {
    private final String LOG_TAG = getClass().getName();

    private static IUnityCallback unityCallback;

    public AndroidCaller(IUnityCallback unityCallback, String unityGameObjectName) {
        this.unityCallback = unityCallback;
        UnityCaller.setUnityGameObjectName(unityGameObjectName);
        LoggerWrapper.getInstance().create(Globals.LOG_TAG, BuildConfig.IS_DEBUG);
    }

    public String unityCall(@NonNull String param) {
        CallResult result;
        try {
            //Log.d(Globals.LOG_TAG, String.format("unity void call, param 1 : %s", param));
            LoggerWrapper.getInstance().logDebug("unity void call, param : %s", param);
            //Log.d(Globals.LOG_TAG, String.format("unity void call, param 2: %s", param));
            Object obj = CallManager.getInstance().onUnitycall(param);
            result = CallResultUtils.getInstance().getSuccessResult(obj);
        }catch (Exception ex) {
            result = CallResultUtils.getInstance().getErrorResult(ex);
        }
        return JSONObject.toJSONString(result);
    }

    public static void callbackUnity(CallInfo callInfo) {
        if(unityCallback != null && callInfo != null) {
            unityCallback.Callback(callInfo.toString());
        }
    }

    /*
    *     //反射获取UnityPlayer
    *     Activity getActivity(){
             if(null == _unityActivity) {
                 try {
                     Class<?> classtype = Class.forName("com.unity3d.player.UnityPlayer");
                     Activity activity = (Activity) classtype.getDeclaredField("currentActivity").get(classtype);
                     _unityActivity = activity;
                 } catch (ClassNotFoundException e) {

                 } catch (IllegalAccessException e) {

                 } catch (NoSuchFieldException e) {

                 }
             }
             return _unityActivity;
         }
    *
    * */
}
