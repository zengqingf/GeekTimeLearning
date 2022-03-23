package com.tm.sdk.unitybridge;

/*
 * todo delete
 *
 * */
public interface ITMSDKCallback {
    void onSDKInited(String msg);
    void onLogined(String msg);
    void onLogout();

    void onPay();
}
