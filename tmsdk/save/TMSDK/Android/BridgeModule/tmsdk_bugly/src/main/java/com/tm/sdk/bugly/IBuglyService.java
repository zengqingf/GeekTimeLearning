package com.tm.sdk.bugly;

import com.tm.sdk.open.src.annotation.TMethod;
import com.tm.sdk.open.src.annotation.TParam;
import com.tm.sdk.open.src.annotation.TService;
import com.tm.sdk.open.src.inter.IService;

@TService("BuglyService")
public interface IBuglyService extends IService {

    @TMethod(value = "initCrashReport", hasParams = true)
    void initCrashReport(@TParam("appId") String appId,
                         @TParam("isDebug") Boolean isDebug);

    @TMethod(value = "setUserId", hasParams = true)
    void setUserId(@TParam("userId") String userId);
}
