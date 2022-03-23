package com.tm.dnl.util;

/**
 * Created by tengmu on 2019/8/6.
 */
public interface IAndroidChannel {
    void Init(boolean isDebug, String apkInfo);
    void Login();
    void Logout();
    void Pay(String payInfo, String userInfo);
    void ReportRoleInfo(String roleInfo, int sceneType);
    void OpenBindPhone();
}
