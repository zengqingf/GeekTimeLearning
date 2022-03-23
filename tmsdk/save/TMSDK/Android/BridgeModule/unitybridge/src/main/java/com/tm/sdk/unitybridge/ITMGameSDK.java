package com.tm.sdk.unitybridge;

import org.json.JSONObject;


/*
* todo delete
*
* */
public interface ITMGameSDK {

    /*生命周期*/
    void mainActivityLifeCycle(int status);


    /*************      平台sdk开始    **************/
    void init(JSONObject json);
    void login(JSONObject json);
    //退出账号
    void logout();
    //退出游戏
    void exit();

    /*上报用户信息*/
    void reportUserInfo(JSONObject json);

    /*手机绑定*/
    void bindPhone(JSONObject json);
    boolean checkBindPhone();

    //打开应用商店
    void openAppstore();
    boolean launchFromAppstore();

    /*************      平台sdk结束  **************/
}

