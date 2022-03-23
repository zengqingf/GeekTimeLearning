package com.tm.sdk.unitybridge.service;

import com.tm.sdk.open.src.inter.IService;

import java.lang.reflect.Method;

public interface IBridgeService {
    String getServiceName();
    Method[] getMethods();
    String[] getMethodNames(Method[] methods);
    String[] getMethodParamNames(Method method);
    Class<?>[] getMethodParamTypes(Method method);
    IService getService();
}
