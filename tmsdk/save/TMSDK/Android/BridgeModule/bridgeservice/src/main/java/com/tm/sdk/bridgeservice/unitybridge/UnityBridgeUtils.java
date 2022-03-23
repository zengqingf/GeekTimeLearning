package com.tm.sdk.bridgeservice.unitybridge;

import com.tm.sdk.open.src.annotation.TMethod;
import com.tm.sdk.open.src.annotation.TParam;
import com.tm.sdk.open.src.annotation.TService;
import com.tm.sdk.open.src.inter.IService;
import com.tm.sdk.unitybridge.service.IBridgeService;
import com.tm.sdk.unitybridge.service.UnityServiceManager;

import java.lang.annotation.Annotation;
import java.lang.reflect.Method;
import java.util.concurrent.ConcurrentHashMap;

public class UnityBridgeUtils {

    public static void registerBridgeService(IBridgeService service){
        UnityServiceManager.getInstance().register(service);
    }

    /*
    public static void register(Class<? extends IService> serviceClass) {
        UnityServiceManager.getInstance().addInterface(serviceClass);
    }*/

    public static void callUnity(String methodName, ConcurrentHashMap<String, Object> params) {
        UnityServiceManager.getInstance().callUnity(methodName, params);
    }

    /*
    public static void sendCallbackCommand(String callbackId, ConcurrentHashMap<String, Object> params) {
        UnityServiceManager.getInstance().sendCallbackCommand(callbackId, params);
    }*/

    private static Class<?> _getServiceInterface(Class<? extends IService> serviceClass) {
        if(null == serviceClass) {
            return null;
        }
        Class<?> clazz = serviceClass.getSuperclass();
        if(null == clazz) {
            clazz = serviceClass;
        }
        if(null == clazz) {
            return null;
        }
        Class<?>[] inters = clazz.getInterfaces();
        if(null == inters || inters.length <= 0 ) {
            return null;
        }
        return inters[0];
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
            return null;
        }
        TService ts = inter.getAnnotation(TService.class);
        if(null == ts) {
            return "";
        }
        return ts.value();
    }

    public static String[] getServiceMethodNames(Class<? extends IService> serviceClass) {
        Method[] methods =  getServiceMethods(serviceClass);
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
