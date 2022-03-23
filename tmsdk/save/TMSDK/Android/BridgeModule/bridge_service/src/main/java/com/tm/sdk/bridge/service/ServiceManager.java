package com.tm.sdk.bridge.service;

import android.util.Log;

import com.tm.sdk.bridge.call.CallArg;
import com.tm.sdk.bridge.call.CallInfo;
import com.tm.sdk.bridge.call.CallManager;
import com.tm.sdk.bridge.call.CallUtils;
import com.tm.sdk.bridge.call.CallbackManager;
import com.tm.sdk.bridge.call.ICallInfo;
import com.tm.sdk.bridge.call.ICallInvoke;
import com.tm.sdk.bridge.exception.BridgeRuntimeException;

import com.tm.sdk.commonlib.output.LoggerWrapper;
import com.tm.sdk.open.src.ControlUtil;
import com.tm.sdk.open.src.inter.ICallbackHandle;
import com.tm.sdk.open.src.inter.ICallbackHandler;
import com.tm.sdk.open.src.inter.IService;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

public class ServiceManager implements ICallInvoke {
    private static class SingletonHolder {
        private static final ServiceManager instance = new ServiceManager();
    }

    public static ServiceManager getInstance() {
        return SingletonHolder.instance;
    }

    /*
    缓存IUnityService实现对象
    key : 实现对象的类 映射名称
    * */
    private ConcurrentHashMap<String, IService> serviceMap;

    private ConcurrentHashMap<String, String[]> serviceMethodNamesMap;

    private ConcurrentHashMap<String, Method[]> serviceMethodsMap;

    private ConcurrentHashMap<String, Method> methodsMap;

    private ConcurrentHashMap<String, String[]> methodParamNamesMap;

    private ConcurrentHashMap<String, Class<?>[]> methodParamTypesMap;

    private ServiceManager() {
        serviceMap = new ConcurrentHashMap<>();
        serviceMethodNamesMap = new ConcurrentHashMap<>();
        serviceMethodsMap = new ConcurrentHashMap<>();
        methodsMap = new ConcurrentHashMap<>();
        methodParamNamesMap = new ConcurrentHashMap<>();
        methodParamTypesMap = new ConcurrentHashMap<>();

        //attach to delegate for
        CallManager.getInstance().setCallInvoke(this);
    }

    public void doCall(String methodName, ConcurrentHashMap<String, Object> params, ICallbackHandle callbackHandle, boolean isCallback) {

        LoggerWrapper.getInstance().logDebug("do call : method is %s, params size is %d, is Callback is %s",
                methodName, params.size(), isCallback+ "");

        LoggerWrapper.getInstance().logDebug("do call : %s", callbackHandle == null ? "no callback handle" : "has callback handle");

        ICallInfo callInfo = null;
        //deal call game and wait game callback
        if(callbackHandle != null) {
            String callbackId = CallUtils.getUUID();
            CallbackManager.getInstance().AddCallbackHandle(callbackId, callbackHandle);
            callInfo = _createCallInfo(methodName, params, isCallback, callbackId);
        }
        else {
            //deal callback game
            // or call game waithout waiting game callback
            callInfo = _createCallInfo(methodName, params, isCallback, "");
        }
        CallManager.getInstance().doCall(callInfo);
    }

    private ICallInfo _createCallInfo(String methodName, ConcurrentHashMap<String, Object> params, boolean isCallback, String callbackId) {
        List<CallArg> callArgs = new ArrayList<>();
        if(params != null && !params.isEmpty()) {
            Iterator<Map.Entry<String, Object>> entries = params.entrySet().iterator();
            while (entries.hasNext()) {
                Map.Entry<String, Object> entry = entries.next();
                String pName = entry.getKey();
                Object pValue = entry.getValue();
                callArgs.add(new CallArg(pName, pValue));
            }
        }
        ICallInfo callInfo = CallInfo.Builder.create()
                .setName(methodName)
                .setArgs(callArgs)
                .setIsCallback(isCallback)
                .setCallbackId(callbackId)
                .build();
        return callInfo;
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
        Log.d("Service Manager",String.format("register service... %s", service != null ? service.getServiceName() : "service is null"));
        if(null == service) {
            return;
        }
        String serviceName = service.getServiceName();
        if(null == serviceName || serviceName.length() <= 0) {
            return;
        }
        serviceMap.put(serviceName, service.getService());

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
        serviceClassList.add(serviceClass);
    }*/

    @Override
    public Object invokeCall(ICallInfo callInfo) {
        if(null == callInfo) {
            return null;
        }
        String methodName = callInfo.getName();
        Method md = _getInvokeMethod(methodName);
        IService service = _getService(methodName);
        LoggerWrapper.getInstance().logDebug(
                String.format("method name : %s, Method : %s, IBridge : %s", methodName,
                md != null ? md.getName() : "null",
                service != null ? "not null" : "null"));
        LoggerWrapper.getInstance().logDebug(callInfo.toString());
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
        String[] paramNames = null;

        if(methodParamTypesMap.containsKey(methodName)) {
            paramTypes = methodParamTypesMap.get(methodName);
        }
        if(methodParamNamesMap.containsKey(methodName)) {
            paramNames = methodParamNamesMap.get(methodName);
        }

        LoggerWrapper.getInstance().logInfo("method param names size is %d, param types size is %d", paramNames.length, paramTypes.length);

        //param type may be ICallbackHandler type, but param name has no ICallbackHandler name
        if(paramNames.length > paramTypes.length) {
            return null;
        }

        if(paramTypes != null && paramTypes.length > 0) {
            methodParams = new Object[paramTypes.length];
            for (int i = paramTypes.length - 1; i >= 0; i--) {
                Class<?> pType = paramTypes[i];
                ICallbackHandler callbackHandler = _createCallbackHandler(pType, callInfo.getCallbackId());
                if(callbackHandler != null) {
                    methodParams[i] = callbackHandler;
                    break;
                }
            }
        }

        if(paramNames != null && paramNames.length > 0) {
            for (int i = 0; i < paramNames.length; i++) {
                LoggerWrapper.getInstance().logInfo("method convert param name");
                String pName = paramNames[i];
                Object param = _getCallParam(callInfo.getArgs(), pName, paramTypes[i]);
                LoggerWrapper.getInstance().logInfo("method param name is %s, value is %s", pName, param != null ? param.toString(): "null");
                if(param != null) {
                    methodParams[i] = param;
                    LoggerWrapper.getInstance().logInfo("method param ...... is %s", methodParams[i].toString());
                }
            }
        }

        try {
            LoggerWrapper.getInstance().logInfo("call invoke method");
            return method.invoke(service, methodParams);
        } catch (IllegalAccessException e) {
            e.printStackTrace();
        } catch (InvocationTargetException e) {
            e.printStackTrace();
        }
        return null;
    }

    private ICallbackHandler _createCallbackHandler(Class<?> paramType, String callbackId) {
        if(paramType.isAssignableFrom(ICallbackHandler.class)) {
            ICallbackHandler callbackHandler = new CallbackHandler(callbackId, true);
            callbackHandler.clearParams();
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
            throw new BridgeRuntimeException("Invoke method error, call args is empty");
        }
        LoggerWrapper.getInstance().logDebug("_getCallParam, paramName is %s, paramType is %s", paramName, paramType.toString());
        for (CallArg arg : args) {
            LoggerWrapper.getInstance().logDebug("CallArg name is %s, value is %s, value type is %s",
                                                    arg.name, arg.value.toString(), arg.value.getClass().toString());
            //notice : boolean is not Boolean
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
            LoggerWrapper.getInstance().logDebug("methods map is empty");
            return null;
        }
        LoggerWrapper.getInstance().logDebug("methods map length is %s", methodsMap.size());
        if(!methodsMap.containsKey(methodName)) {
            LoggerWrapper.getInstance().logDebug("has no key %s", methodName);
            return null;
        }
        LoggerWrapper.getInstance().logDebug("try get key %s", methodName);
        return methodsMap.get(methodName);
    }

    private IService _getService(String methodName) {
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
        if(null == serviceMap || serviceMap.isEmpty()) {
            return null;
        }
        if(serviceMap.containsKey(serviceName)) {
            return serviceMap.get(serviceName);
        }
        return null;
    }
}
