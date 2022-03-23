package com.tm.sdk.open.src;

import android.content.Context;
import android.util.Log;

import com.alibaba.android.arouter.launcher.ARouter;

import com.tm.sdk.commonlib.output.LoggerWrapper;
import com.tm.sdk.open.src.inter.IService;
import com.tm.sdk.open.src.inter.ICallbackHandler;
import com.tm.sdk.open.src.router.RouterTable;
import com.tm.sdk.open.src.router.IContextControl;
import com.tm.sdk.open.src.router.IServiceControl;

public class ControlUtil {

    private static IServiceControl getServiceControl() {
        Log.d("SDK Open Src", " IServiceControl ready to create");
        IServiceControl isc =  (IServiceControl) ARouter.getInstance().build(RouterTable.PATH_SERVICE_CONTROL).navigation();
        Log.d("SDK Open Src", " IServiceControl is " + isc != null ? "not null" : "null");
        return isc;
    }

    public static void create(Class<? extends IService> serviceClass) {
        Log.d("SDK Open Src", String.format("create IService is %s", serviceClass != null ? "not null" : "null"));
        getServiceControl().create(serviceClass);
    }

    public static void doCall(ICallbackHandler callbackHandler) {
        getServiceControl().doCall(callbackHandler);
    }

    public static ICallbackHandler createCallHandler(String methodName) {
        return getServiceControl().createHandler(methodName);
    }

    private static IContextControl getContextControl() {
        return (IContextControl)ARouter.getInstance().build(RouterTable.PATH_CONTEXT_CONTROL).navigation();
    }

    public static void setCurrentContext(Context context) {
        getContextControl().setCurrentContext(context);
    }

    public static Context getCurrentContext() {
        return getContextControl().getCurrentContext();
    }

    public static void exit() {
        getContextControl().exit();
    }
}
