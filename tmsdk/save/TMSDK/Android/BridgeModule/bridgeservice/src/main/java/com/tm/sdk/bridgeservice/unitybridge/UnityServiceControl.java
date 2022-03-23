package com.tm.sdk.bridgeservice.unitybridge;

import android.app.Activity;
import android.content.Context;
import android.os.Handler;
import android.os.Message;

import androidx.annotation.NonNull;

import com.alibaba.android.arouter.facade.annotation.Route;
import com.tm.sdk.bridgeservice.bundle.ServiceBundle;
import com.tm.sdk.open.src.inter.ICallbackHandle;
import com.tm.sdk.open.src.inter.ICallbackHandler;
import com.tm.sdk.open.src.inter.IService;
import com.tm.sdk.open.src.router.RouterTable;
import com.tm.sdk.open.src.router.IServiceControl;

import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;
import java.util.concurrent.ConcurrentHashMap;


@Route(path = RouterTable.PATH_SERVICE_CONTROL)
public class UnityServiceControl implements IServiceControl {

    private static Handler handler;

    @Override
    public void create(Class<? extends IService> serviceClass) {
        IService service = null;
        UnityBridgeService bridgeService = null;
        ServiceBundle bundle = null;
        try {
            //Log.d("UnityService", "create start");
            Constructor ct = serviceClass.getConstructor();
            service = (IService) ct.newInstance();

            bundle = new ServiceBundle(serviceClass, service);
            bridgeService = new UnityBridgeService(bundle);
            bridgeService.register();
            //Log.d("UnityService", "create end");
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
    public void doCall(Context context, String methodName, ConcurrentHashMap<String, Object> params, ICallbackHandle callbackHandle) {
        Activity activity = (Activity)context;
        if(activity != null) {
            activity.runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    UnityBridgeUtils.callUnity(methodName, params);
                }
            });
        }
    }

    @Override
    public void handleCallback(ICallbackHandler callbackHandler) {
        if(null == handler)
            return;
        Message msg = Message.obtain();
        msg.obj = callbackHandler;
        handler.sendMessage(msg);
    }

    @Override
    public void init(Context context) {
        //初始化工作  服务注入时会调用
        if(handler == null) {
            handler = new Handler() {
                @Override
                public void handleMessage(@NonNull Message msg) {
                    Activity activity = (Activity)context;
                    if(activity != null) {
                        activity.runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                ICallbackHandler cbHandler = (ICallbackHandler) msg.obj;
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
