package com.tm.sdk.bridge.service;

import android.util.Log;

import com.tm.sdk.open.src.annotation.TMethod;
import com.tm.sdk.open.src.annotation.TParam;
import com.tm.sdk.open.src.annotation.TService;
import com.tm.sdk.open.src.inter.ICallbackHandle;
import com.tm.sdk.open.src.inter.IService;

import java.lang.annotation.Annotation;
import java.lang.reflect.Method;
import java.util.concurrent.ConcurrentHashMap;

public class ServiceUtils {
    public static void registerBridgeService(IBridgeService service){
        ServiceManager.getInstance().register(service);
    }

    /*
    public static void register(Class<? extends IService> serviceClass) {
        UnityServiceManager.getInstance().addInterface(serviceClass);
    }*/

    private static Class<?> _getServiceInterface(Class<? extends IService> serviceClass) {
        if(null == serviceClass) {
            return null;
        }
        Log.d("ServiceUtils", String.format("get IService start, service is %s", serviceClass != null ? serviceClass.toString() : "null"));
        Class<?> clazz = serviceClass.getSuperclass();
        if(null == clazz || clazz.isInstance(Object.class)) {
            clazz = serviceClass;
            Log.d("ServiceUtils", String.format("get IService no super class, service is %s", clazz != null ? clazz.toString() : "null"));
        }
        if(null == clazz) {
            return null;
        }
        Log.d("ServiceUtils", String.format("get IService end, service is %s", clazz != null ? clazz.toString() : "null"));
        Class<?>[] inters = clazz.getInterfaces();
        Log.d("ServiceUtils", String.format("get IService interface, inters size is %s", inters != null ? inters.length : "null"));
        if(null == inters || inters.length <= 0 ) {
            return null;
        }
        for(int i = 0; i < inters.length; i++) {
            TService ts = inters[i].getAnnotation(TService.class);
            if (ts != null) {
                Log.d("ServiceUtils", String.format("get IService interface, inter has TService is %s", inters[i].toString()));
                return inters[i];
            }
        }
        Log.d("ServiceUtils", String.format("get IService interface, inter has TService is null"));
        return null;
    }

    public static Method[] getServiceMethods(Class<? extends IService> serviceClass) {
        Class<?> inter = _getServiceInterface(serviceClass);
        if(null == inter) {
            return null;
        }
        Method[] methods = inter.getMethods();
        return methods;
    }

    public static String getServiceName(Class<? extends IService> serviceClass) {
        Class<?> inter = _getServiceInterface(serviceClass);
        if(null == inter) {
            return "";
        }
        TService ts = inter.getAnnotation(TService.class);
        if (ts != null) {
            return ts.value();
        }
        return "";
    }

    public static String[] getServiceMethodNames(Class<? extends IService> serviceClass) {
        Method[] methods =  getServiceMethods(serviceClass);
        return getServiceMethodNames(methods);
    }

    public static String[] getServiceMethodNames(Method[] methods) {
        if(null == methods || methods.length <= 0) {
            return null;
        }
        String[] methodNames = new String[methods.length];
        for (int i = 0; i < methods.length; i++) {
            TMethod tMethod = methods[i].getAnnotation(TMethod.class);
            if(null != tMethod) {
                methodNames[i] = tMethod.value();
            }else {
                methodNames[i] = "";
            }
        }
        return methodNames;
    }

    public static String[] getMethodParamNames(Method method) {
        if(null == method) {
            return null;
        }
        Annotation[][] annos = method.getParameterAnnotations();
        //二维数组判空 null / {} // {{}}
        if(null == annos || annos.length <= 0 || (annos.length == 1 && annos[0].length <= 0)) {
            return new String[0];
        }
        String[] paramNames = new String[annos.length];  //行的长度（每行 对应 一个参数）
        for (int rowIndex = 0; rowIndex < annos.length; rowIndex++) {
            Annotation[] annosOfName = annos[rowIndex];
            for (Annotation anno : annosOfName) {
                if (anno instanceof TParam) {
                    TParam param = (TParam) anno;
                    paramNames[rowIndex] = param.value();
                    break;
                }
            }
        }
        return paramNames;
    }

    public static Class<?>[] getMethodParamTypes(Method method) {
        if(null == method) {
            return null;
        }
        return method.getParameterTypes();
    }
}
