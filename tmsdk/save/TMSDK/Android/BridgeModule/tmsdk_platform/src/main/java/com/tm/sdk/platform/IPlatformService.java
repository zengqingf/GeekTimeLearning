package com.tm.sdk.platform;

import android.annotation.SuppressLint;

import com.tm.sdk.open.src.annotation.TMethod;
import com.tm.sdk.open.src.annotation.TParam;
import com.tm.sdk.open.src.annotation.TService;
import com.tm.sdk.open.src.inter.ICallbackHandler;
import com.tm.sdk.open.src.inter.IService;

@TService("PlatformService")
public interface IPlatformService extends IService {
    @TMethod(value = "init", hasParams = true)
    void init(@TParam("isDebug") Boolean isDebug);

    @TMethod(value = "login")
    void login(@SuppressLint("only use one") ICallbackHandler callbackHandler);

    @TMethod("logout")
    void logout();

    @TMethod("exit")
    void exit();

    @TMethod(value = "pay", hasParams = true)
    void pay(
            @TParam("requestId")           String requestId,
            @TParam("price")               String price,
            @TParam("mallType")            String mallType,
            @TParam("productId")           String productId,
            @TParam("productName")         String productName,
            @TParam("productShortName")    String productShortName,
            @TParam("productDesc")         String productDesc,
            @TParam("productChId")         String productChId,
            @TParam("appName")             String appName,
            @TParam("serverId")            String serverId,
            @TParam("serverName")          String serverName,
            @TParam("notifyUrl")           String notifyUrl,
            @TParam("accountId")           String accountId,
            @TParam("accountName")         String accountName,
            @TParam("roleId")              String roleId,
            @TParam("roleName")            String roleName,
            @TParam("openuid")             String openuid,
            @TParam("extra")               String extra,
            @SuppressLint("only use one") ICallbackHandler callbackHandler
    );

    @TMethod(value = "testCallResult")
    boolean testCallResult();
}
