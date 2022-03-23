package com.tm.sdk.bugly;

import com.tencent.bugly.crashreport.CrashReport;
import com.tm.sdk.open.src.ControlUtil;

public class BuglyService implements IBuglyService {
    @Override
    public void initCrashReport(String appId, Boolean isDebug) {
        CrashReport.initCrashReport(ControlUtil.getCurrentContext(), appId, isDebug.booleanValue());
    }

    @Override
    public void setUserId(String userId) {
        CrashReport.setUserId(userId);
    }
}
