package com.tm.sdk.ss79.platform;

import android.app.Activity;

import com.sswl.template.SSWLSdkApi;
import com.sswl.template.bean.LoginResult;
import com.sswl.template.bean.PayParam;
import com.sswl.template.callback.ISSWLCallback;
import com.tm.sdk.commonlib.output.LoggerWrapper;
import com.tm.sdk.open.src.ControlUtil;
import com.tm.sdk.open.src.inter.ICallbackHandle;
import com.tm.sdk.open.src.inter.ICallbackHandler;
import com.tm.sdk.platform.CallbackMethodType;
import com.tm.sdk.platform.CallbackParamType;
import com.tm.sdk.platform.IPlatformService;
import com.tm.sdk.platform.PlatformService;

import java.util.concurrent.ConcurrentHashMap;

public class ServiceImpl extends PlatformService implements ICallbackHandle {

    private ICallbackHandler loginCallback;
    private ICallbackHandler payCallback;

    private int age = -1;

    @Override
    public void init(Boolean isDebug) {
        super.init(isDebug);
        Global.IS_DEBUG = isDebug.booleanValue();

        //Test
        testGameCallback();

        SSWLSdkApi.getInstance().init((Activity) ControlUtil.getCurrentContext(), new ISSWLCallback() {
            @Override
            public void onInitSuccess() {
            }

            @Override
            public void onInitFail(String s) {
            }

            @Override
            public void onLoginSuccess(LoginResult loginResult) {
                age = loginResult.getAge();
                if(loginCallback != null) {
                    loginCallback.clearParams();
                    loginCallback.addParam(CallbackParamType.openuid.value, loginResult.getUserId());
                    loginCallback.addParam(CallbackParamType.token.value, loginResult.getCode());
                    ControlUtil.doCall(loginCallback);
                }
                //loginResult.getAge();
                //loginResult.getUnderage();
                //"code = "+loginResult.getCode() + " , token= "+loginResult.getToken()
                //"userId = "+loginResult.getUserId() +",userName = "+loginResult.getUserName());
            }

            @Override
            public void onLoginFail(String s) {
            }
            /***
             * 登出回调
             * @param autoShowLogin 是否自动弹出渠道的登录界面,即true即为切换账号，false为简单登出而已，不会自动弹出登录界面
             */
            @Override
            public void onLogout(boolean autoShowLogin) {
                age= -1;
                ICallbackHandler handler = ControlUtil.createCallHandler(CallbackMethodType.OnLogout.value);
                ControlUtil.doCall(handler);
            }

            @Override
            public void onChannelExit() {
            }

            @Override
            public void onGameExit() {
            }

            @Override
            public void onPaySuccess() {
                _OnPay(CallbackParamType.paysuccess);
            }

            @Override
            public void onPayFail(String s) {
                _OnPay(CallbackParamType.payfailed);
            }
        });

    }

    @Override
    public void login(ICallbackHandler callbackHandler) {
        super.login(callbackHandler);

        this.loginCallback = callbackHandler;

        SSWLSdkApi.getInstance().login((Activity)ControlUtil.getCurrentContext());
    }

    @Override
    public void logout() {
        super.logout();

        SSWLSdkApi.getInstance().logout((Activity)ControlUtil.getCurrentContext());
    }

    @Override
    public void exit() {
        super.exit();
    }

    @Override
    public void pay(String requestId,
                    String price,
                    String mallType,
                    String productId,
                    String productName,
                    String productShortName,
                    String productDesc,
                    String productChId,
                    String appName,
                    String serverId,
                    String serverName,
                    String notifyUrl,
                    String accountId,
                    String accountName,
                    String roleId,
                    String roleName,
                    String openuid,
                    String extra,
                    ICallbackHandler callbackHandler) {

        super.pay(requestId, price, mallType, productId, productName, productShortName, productDesc, productChId,
                appName, serverId, serverName,  notifyUrl, accountId, accountName, roleId, roleName, openuid, extra, callbackHandler);

        this.payCallback = callbackHandler;

        PayParam p = new PayParam()
                .setProductId(productId)
                .setProductName(productName)
                .setProductDesc(productDesc)
                .setPrice(Integer.valueOf(price) * 100)
                .setCount(1)
                .setServerName(serverId) //TODO  need server name
                .setServerID(serverId)
                .setRoleID(roleId)
                .setRoleName(roleName)
                .setCpOrderId(requestId)
                .setExtend(extra);

        SSWLSdkApi.getInstance().pay((Activity)ControlUtil.getCurrentContext(), p);
    }

    private void _OnPay(CallbackParamType type) {
        if(payCallback != null) {
            payCallback.addParam(type.value, type.valueInt);
            ControlUtil.doCall(payCallback);
        }
    }

    //***************************************************just Test !***************************************************
    @Override
    public boolean testCallResult() {
        LoggerWrapper.getInstance().logInfo("test call result !!!");
        return true;
    }

    public void testGameCallback() {
        LoggerWrapper.getInstance().logInfo("test call game, wait game callback !!!");
        ICallbackHandler handler = ControlUtil.createCallHandler(CallbackMethodType.TestGameCallback.value);
        handler.setCallback(this);
        ControlUtil.doCall(handler);
    }

    @Override
    public void onCallback(ConcurrentHashMap<String, Object> args) {
        LoggerWrapper.getInstance().logInfo("test, game callback !!!");
    }
    //***************************************************just Test !***************************************************
}
