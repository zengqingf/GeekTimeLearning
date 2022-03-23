package com.tm.sdk.open.src.router;

import android.content.Context;

import com.alibaba.android.arouter.facade.template.IProvider;
import com.tm.sdk.open.src.inter.ICallbackHandle;
import com.tm.sdk.open.src.inter.IService;
import com.tm.sdk.open.src.inter.ICallbackHandler;

import java.util.concurrent.ConcurrentHashMap;

public interface IServiceControl extends IProvider {
    void create(Class<? extends IService> service);
    void doCall(ICallbackHandler callbackHandler);
    ICallbackHandler createHandler(String methodName);
}
