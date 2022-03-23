package com.tm.sdk.unitybridge;

import androidx.annotation.NonNull;

import com.tm.sdk.commonlib.output.LoggerWrapper;
import com.tm.sdk.unitybridge.util.Globals;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

public class UnityCaller {

    private static String unityGameObjectName = "";
    public static void doUnity3dCall(@NonNull ICallInfo callInfo){
        if(callInfo != null) {
            callUnity(unityGameObjectName, callInfo.getName(), callInfo.toString());
        }else {
            LoggerWrapper.getInstance().logError("Send Message to Unity Failed, call info is null");
        }
    }

    public static void setUnityGameObjectName(String unityGoName) {
        unityGameObjectName = unityGoName;
    }

    static Class<?> unityplayerClass = null;
    static Method unitySendMsgMethod = null;

    static boolean callUnity(String gameObjName, String funcName, String args) {
        try {
            if(null == unityplayerClass) {
                unityplayerClass = Class.forName("com.unity3d.player.UnityPlayer");
            }
            if(null == unitySendMsgMethod) {
                unitySendMsgMethod = unityplayerClass.getMethod("UnitySendMessage", String.class, String.class, String.class);
            }
            unitySendMsgMethod.invoke(unityplayerClass, gameObjName, funcName, args);
            return true;
        } catch (ClassNotFoundException e) {
            e.printStackTrace();
        } catch (NoSuchMethodException e) {
            e.printStackTrace();
        } catch (IllegalAccessException e) {
            e.printStackTrace();
        } catch (InvocationTargetException e) {
            e.printStackTrace();
        }
        return false;
    }
}
