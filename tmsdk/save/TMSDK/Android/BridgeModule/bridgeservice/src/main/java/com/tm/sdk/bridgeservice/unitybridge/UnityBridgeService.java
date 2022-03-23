package com.tm.sdk.bridgeservice.unitybridge;

import com.tm.sdk.bridgeservice.bundle.ServiceBundle;
import com.tm.sdk.open.src.inter.IService;
import com.tm.sdk.unitybridge.service.IBridgeService;

import java.lang.reflect.Method;

public class UnityBridgeService implements IBridgeService {

    Class<? extends IService> serviceClass;
    IService serviceObj;

    public UnityBridgeService(ServiceBundle serviceBundle) {
        if(null == serviceBundle)
            return;
        serviceClass = serviceBundle.serviceClazz;
        serviceObj = serviceBundle.serviceObj;
    }

    public void register() {
        UnityBridgeUtils.registerBridgeService(this);
    }

    @Override
    public String getServiceName() {
        if(null == serviceClass) {
            return null;
        }
        return UnityBridgeUtils.getServiceName(serviceClass);
    }

    @Override
    public Method[] getMethods() {
        if(null == serviceClass) {
            return null;
        }
        return UnityBridgeUtils.getServiceMethods(serviceClass);
    }

    @Override
    public String[] getMethodNames(Method[] methods) {
        return UnityBridgeUtils.getServiceMethodNames(methods);
    }

    @Override
    public String[] getMethodParamNames(Method method) {
        return UnityBridgeUtils.getMethodParamNames(method);
    }

    @Override
    public Class<?>[] getMethodParamTypes(Method method) {
        return UnityBridgeUtils.getMethodParamTypes(method);
    }

    @Override
    public IService getService() {
        return serviceObj;
    }
}
