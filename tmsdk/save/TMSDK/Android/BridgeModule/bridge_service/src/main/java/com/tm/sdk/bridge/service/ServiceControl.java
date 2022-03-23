package com.tm.sdk.bridge.service;

import android.app.Activity;
import android.content.Context;
import android.os.Handler;
import android.os.Message;
import android.util.Log;

import androidx.annotation.NonNull;

import com.alibaba.android.arouter.facade.annotation.Route;
import com.tm.sdk.commonlib.output.LoggerWrapper;
import com.tm.sdk.open.src.ControlUtil;
import com.tm.sdk.open.src.inter.ICallbackHandler;
import com.tm.sdk.open.src.inter.IService;
import com.tm.sdk.open.src.router.IServiceControl;
import com.tm.sdk.open.src.router.RouterTable;

import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;

@Route(path = RouterTable.PATH_SERVICE_CONTROL)
public class ServiceControl implements IServiceControl {
    private static Handler handler;

    @Override
    public void create(Class<? extends IService> serviceClass) {
        IService service = null;
        BridgeService bridgeService = null;
        ServiceBundle bundle = null;
        try {
            Log.d("ServiceControl", "create start");
            Log.d("ServiceControl", String.format("create IService start, serviceClass is %s", serviceClass != null ? serviceClass.toString() : "null"));
            Constructor ct = serviceClass.getConstructor();
            service = (IService) ct.newInstance();

            bundle = new ServiceBundle(serviceClass, service);
            bridgeService = new BridgeService(bundle);
            bridgeService.register();
            Log.d("ServiceControl", String.format("create IService end, service is %s", service != null ? service.toString() : "null"));
        } catch (NoSuchMethodException e) {
            e.printStackTrace();
        } catch (IllegalAccessException e) {
            e.printStackTrace();
        } catch (InstantiationException e) {
            e.printStackTrace();
        } catch (InvocationTargetException e) {
            e.printStackTrace();
        }
    }

    @Override
    public void doCall(ICallbackHandler callbackHandler) {
        if(null == handler)
            return;
        Message msg = Message.obtain();
        msg.obj = callbackHandler;
        handler.sendMessage(msg);
    }

    @Override
    public ICallbackHandler createHandler(String methodName) {
        return new CallbackHandler(methodName, false);
    }

    @Override
    public void init(Context context) {
        //初始化工作  服务注入时会调用
        if(handler == null) {
            handler = new Handler() {
                @Override
                public void handleMessage(@NonNull Message msg) {
                    //notice : can use init(Context context)
                    // it was just initilized once , so context is Application not Activity
                    Activity activity = (Activity) ControlUtil.getCurrentContext();
                    if(activity != null) {
                        activity.runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                CallbackHandler cbHandler = (CallbackHandler) msg.obj;
                                if(cbHandler != null) {
                                    cbHandler.call();
                                }
                            }
                        });
                    }
                }
            };
        }
    }
}
