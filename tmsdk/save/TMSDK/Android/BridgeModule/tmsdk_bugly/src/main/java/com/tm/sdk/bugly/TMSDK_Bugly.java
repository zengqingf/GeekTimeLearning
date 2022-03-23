package com.tm.sdk.bugly;

import android.content.Context;

import com.tencent.bugly.crashreport.CrashReport;

//just test
public class TMSDK_Bugly {

    public static void TestJavaCrash() {
        CrashReport.testJavaCrash();
    }

    public static void TestANRCrash() {
        CrashReport.testANRCrash();
    }

    public static void TestNativeCrash() {
        CrashReport.testNativeCrash();
    }

    public static void InitCrashReport(Context appContext, String appId, boolean bDebug) {
        CrashReport.initCrashReport(appContext, appId, bDebug);
    }

    public static void SetUserId(String userId) {
        CrashReport.setUserId(userId);
    }
}
