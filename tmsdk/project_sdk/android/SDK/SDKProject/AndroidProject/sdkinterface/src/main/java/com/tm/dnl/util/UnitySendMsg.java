package com.tm.dnl.util;

import com.unity3d.player.UnityPlayer;

/**
 * Created by tengmu on 2019/7/29.
 */
public class UnitySendMsg {

    public final static  String SDKCALLBACK_GAMEOBJECT_NAME = "Singleton of SDKClient.SDKCallback";
    public final static  String SDKCALLBACK_LOGIN = "OnLogin";
    public final static  String SDKCALLBACK_LOGOUT = "OnLogout";
    public final static  String SDKCALLBACK_LOGINFAIL = "OnLoginFail";

    public final static  String SDKCALLBACK_ONPAY = "OnPayResult";              //0-成功 1-失败 2-取消
    public final static  String SDKCALLBACK_ISBINDPHONESUCC = "OnBindPhoneSucc";
    public final static  String SDKCALLBACK_KEYBOARD = "OnKeyBoardShowOn";

    public static void SendMsgToUnity(String methodName,String methodParam) {
        try {
            UnityPlayer.UnitySendMessage(SDKCALLBACK_GAMEOBJECT_NAME, methodName, methodParam);
        }
        catch (Exception e)
        {
            Logger.LogError("SendMsgToUnity method name : %s , method params : %s",methodName, methodParam);
        }
    }
}
