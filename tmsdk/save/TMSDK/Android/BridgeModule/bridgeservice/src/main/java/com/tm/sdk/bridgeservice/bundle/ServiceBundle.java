package com.tm.sdk.bridgeservice.bundle;

import com.tm.sdk.open.src.inter.IService;

public class ServiceBundle {
    public Class<? extends IService> serviceClazz;
    public IService serviceObj;

    public ServiceBundle(Class<? extends IService> clazz, IService obj) {
        this.serviceClazz = clazz;
        this.serviceObj = obj;
    }
}
