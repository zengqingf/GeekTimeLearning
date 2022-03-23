package com.tm.sdk.unitybridge;

/*
* Unity 调用 Android 接口 监听
* */
public interface IUnityCallListener {

    Object onUnitycall(String command);
}
