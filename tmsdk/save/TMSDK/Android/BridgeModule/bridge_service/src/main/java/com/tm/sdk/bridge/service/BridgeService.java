package com.tm.sdk.bridge.service;

import com.tm.sdk.open.src.inter.IService;

import java.lang.reflect.Method;

public class BridgeService implements IBridgeService{
    Class<? extends IService> serviceClass;
    IService serviceObj;

    public BridgeService(ServiceBundle serviceBundle) {
        if(null == serviceBundle)
            return;
        serviceClass = serviceBundle.serviceClazz;
        serviceObj = serviceBundle.serviceObj;
    }

    public void register() {
        ServiceUtils.registerBridgeService(this);
    }

    @Override
    public String getServiceName() {
        if(null == serviceClass) {
            return null;
        }
        return ServiceUtils.getServiceName(serviceClass);
    }

    @Override
    public Method[] getMethods() {
        if(null == serviceClass) {
            return null;
        }
        return ServiceUtils.getServiceMethods(serviceClass);
    }

    @Override
    public String[] getMethodNames(Method[] methods) {
        return ServiceUtils.getServiceMethodNames(methods);
    }

    @Override
    public String[] getMethodParamNames(Method method) {
        return ServiceUtils.getMethodParamNames(method);
    }

    @Override
    public Class<?>[] getMethodParamTypes(Method method) {
        return ServiceUtils.getMethodParamTypes(method);
    }

    @Override
    public IService getService() {
        return serviceObj;
    }
}
