package com.tm.sdk.unitybridge.service;

import com.tm.sdk.open.src.inter.IService;
import com.tm.sdk.unitybridge.CallInfo;
import com.tm.sdk.unitybridge.UnityCaller;
import com.tm.sdk.unitybridge.exception.UnityProxyRuntimeException;
import com.tm.sdk.unitybridge.AndroidCaller;
import com.tm.sdk.unitybridge.CallArg;
import com.tm.sdk.unitybridge.ICallInfo;

import com.tm.sdk.open.src.inter.ICallbackHandler;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

public class UnityServiceManager {

    private static class SingletonHolder {
        private static final UnityServiceManager instance = new UnityServiceManager();
    }

    public static UnityServiceManager getInstance() {
        return SingletonHolder.instance;
    }

    public UnityServiceManager() {
        unityServiceMap = new ConcurrentHashMap<>();
        serviceMethodNamesMap = new ConcurrentHashMap<>();
        serviceMethodsMap = new ConcurrentHashMap<>();
        methodsMap = new ConcurrentHashMap<>();
        methodParamNamesMap = new ConcurrentHashMap<>();
        methodParamTypesMap = new ConcurrentHashMap<>();

        //unityServiceClassList = new ArrayList<>();
    }

    /*
    缓存IUnityService实现对象
    key : 实现对象的类 映射名称
    * */
    private ConcurrentHashMap<String, IService> unityServiceMap;

    private ConcurrentHashMap<String, String[]> serviceMethodNamesMap;

    private ConcurrentHashMap<String, Method[]> serviceMethodsMap;

    private ConcurrentHashMap<String, Method> methodsMap;

    private ConcurrentHashMap<String, String[]> methodParamNamesMap;

    private ConcurrentHashMap<String, Class<?>[]> methodParamTypesMap;

    /*
    缓存IUnityService实现类
    key : 实现类 映射名称
    * */
    //private List<Class<? extends IService>> unityServiceClassList;

    public void callUnity(String methodName, ConcurrentHashMap<String, Object> params) {
        CallInfo callInfo = CallInfo.Builder.create().build();
        callInfo.setName(methodName);
        if(params != null && !params.isEmpty()) {
            Iterator<Map.Entry<String, Object>> entries = params.entrySet().iterator();
            while (entries.hasNext()) {
                Map.Entry<String, Object> entry = entries.next();
                String pName = entry.getKey();
                Object pValue = entry.getValue();
                callInfo.addArgs(pName, pValue);
            }
        }
        UnityCaller.doUnity3dCall(callInfo);
    }

    public void sendCallbackCommand(String callbackId, ConcurrentHashMap<String, Object> params) {
        CallInfo callInfo = CallInfo.Builder.create().build();
        callInfo.setName(callbackId);
        callInfo.setIsCallback(true);
        if(params != null && !params.isEmpty()) {
            Iterator<Map.Entry<String, Object>> entries = params.entrySet().iterator();
            while (entries.hasNext()) {
                Map.Entry<String, Object> entry = entries.next();
                String pName = entry.getKey();
                Object pValue = entry.getValue();
                callInfo.addArgs(pName, pValue);
            }
        }
        AndroidCaller.callbackUnity(callInfo);
    }

    /*
    * 注册提供Unity方法的Service实例
    *
    * 通过service提供的接口获取反射对象
    *
    * 注意：不要在这个方法中再使用反射接口获取反射对象了
    *
    * 注册服务接口
    * */
    public void register(IBridgeService service) {
        if(null == service) {
            return;
        }
        String serviceName = service.getServiceName();
        if(null == serviceName || serviceName.length() <= 0) {
            return;
        }
        unityServiceMap.put(serviceName, service.getService());

        Method[] methods = service.getMethods();
        serviceMethodsMap.put(serviceName, methods);
        String[] methodNames = service.getMethodNames(methods);
        serviceMethodNamesMap.put(serviceName, methodNames);

        if(null != methods && methods.length > 0 && methods.length == methodNames.length) {
            for(int i = 0; i < methods.length; i++) {
                String mdName = methodNames[i];
                Method md = methods[i];
                if(null == mdName || mdName.length() <= 0) {
                    continue;
                }
                methodsMap.put(mdName, md);
                String[] methodParamNames = service.getMethodParamNames(md);
                methodParamNamesMap.put(mdName, methodParamNames);
                Class<?>[] methodParamTypes = service.getMethodParamTypes(md);
                methodParamTypesMap.put(mdName, methodParamTypes);
            }
        }
    }

    /*
    * 注册服务接口
    * */
    /*
    public void addInterface(Class<? extends IService> serviceClass) {
        unityServiceClassList.add(serviceClass);
    }*/

    public Object invokeCall(ICallInfo callInfo) {
        if(null == callInfo) {
            return null;
        }
        String methodName = callInfo.getName();
        Method md = _getInvokeMethod(methodName);
        IService service = _getUnityService(methodName);
        //Log.d(Globals.LOG_TAG, String.format("method name : %s, Method : %s, IBridge : %s", methodName,
        //        md != null ? md.getName() : "null",
        //        service != null ? "not null" : "null"));
        //Log.d(Globals.LOG_TAG, callInfo.toString());
        return _invokeMethod(callInfo, methodName, md, service);
    }

    private Object _invokeMethod(ICallInfo callInfo,
                                 String methodName,
                                 Method method,
                                 IService service) {
        if(null == callInfo) {
            return null;
        }
        Object[] methodParams = null;

        Class<?>[] paramTypes = null;
        if(methodParamTypesMap.containsKey(methodName)) {
           paramTypes = methodParamTypesMap.get(methodName);
        }
        if(paramTypes != null && paramTypes.length > 0) {
            methodParams = new Object[paramTypes.length];
            for (int i = paramTypes.length - 1; i >= 0; i++) {
                Class<?> pType = paramTypes[i];
                Object callbackHandler = _getCallbackHandlerParamType(pType, callInfo.getCallbackId());
                if(callbackHandler != null) {
                    methodParams[i] = callbackHandler;
                    break;
                }
            }
        }

        String[] paramNames = null;
        if(methodParamNamesMap.containsKey(methodName)) {
            paramNames = methodParamNamesMap.get(methodName);
        }
        if(paramNames.length > paramTypes.length) {
            return null;
        }
        if( paramNames != null && paramNames.length > 0) {
            for (int i = 0; i < paramNames.length; i++) {
                String pName = paramNames[i];
                Object param = _getCallParam(callInfo.getArgs(), pName, paramTypes[i]);
                if(param != null) {
                    methodParams[i] = param;
                }
            }
        }

        try {
            return method.invoke(service, methodParams);
        } catch (IllegalAccessException e) {
            e.printStackTrace();
        } catch (InvocationTargetException e) {
            e.printStackTrace();
        }
        return null;
    }

    private Object _getCallbackHandlerParamType(Class<?> paramType, String callbackId) {
        if(paramType.isAssignableFrom(ICallbackHandler.class)) {
            CallbackHandler callbackHandler = new CallbackHandler(callbackId);
            return callbackHandler;
            /*
            try {
                Constructor ct = paramType.getConstructor(String.class);
                return ct.newInstance(callbackId);
            } catch (NoSuchMethodException e) {
                e.printStackTrace();
            } catch (IllegalAccessException e) {
                e.printStackTrace();
            } catch (InstantiationException e) {
                e.printStackTrace();
            } catch (InvocationTargetException e) {
                e.printStackTrace();
            }*/
        }
        return null;
    }

    private Object _getCallParam(List<CallArg> args, String paramName, Class<?> paramType) {
        if(null == args || args.isEmpty()) {
            throw new UnityProxyRuntimeException("Invoke method error, call args is empty");
        }
        for (CallArg arg : args) {
            if (arg != null && arg.name.equals(paramName)) {
                if(paramType.isInstance(arg.value)) {
                    return arg.value;
                }
            }
        }
        return null;
    }

    private Method _getInvokeMethod(String methodName) {
        if(null == methodsMap || methodsMap.isEmpty()) {
            return null;
        }
        if(!methodsMap.containsKey(methodName)) {
            return null;
        }
        return methodsMap.get(methodName);
    }

    private IService _getUnityService(String methodName) {
        if(null == serviceMethodNamesMap || serviceMethodNamesMap.isEmpty()) {
            return null;
        }
        String serviceName = null;
        Iterator<Map.Entry<String, String[]>> entries = serviceMethodNamesMap.entrySet().iterator();
        while(entries.hasNext()) {
            Map.Entry<String, String[]> entry = entries.next();
            String[] mdNames = entry.getValue();
            if(null == mdNames || mdNames.length <= 0) {
                continue;
            }
            for(String mdName : mdNames) {
                if(mdName.equals(methodName)) {
                    serviceName = entry.getKey();
                    break;
                }
            }
            if(serviceName != null && serviceName.length() > 0) {
                break;
            }
        }
        if(null == unityServiceMap || unityServiceMap.isEmpty()) {
            return null;
        }
        if(unityServiceMap.containsKey(serviceName)) {
            return unityServiceMap.get(serviceName);
        }
        return null;
    }
}
