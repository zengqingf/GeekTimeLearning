package com.tm.sdk.mg.platform;

import com.mgpay.utils.PreferenceUtils;
import com.mgpay.utils.StringUtils;
import com.tm.sdk.commonlib.output.LoggerWrapper;
import com.tm.sdk.open.src.ControlUtil;
import com.tm.sdk.open.src.inter.ICallbackHandle;
import com.tm.sdk.open.src.inter.ICallbackHandler;
import com.tm.sdk.platform.CallbackMethodType;
import com.tm.sdk.platform.CallbackParamType;
import com.tm.sdk.platform.PlatformService;
import com.xy.xypay.bean.LoginResult;
import com.xy.xypay.bean.LogoutResult;
import com.xy.xypay.bean.PayArgsCheckResult;
import com.xy.xypay.bean.XPayArg;
import com.xy.xypay.inters.XYLoginCallback;
import com.xy.xypay.inters.XYLogoutCallback;
import com.xy.xypay.inters.XYPayCallback;
import com.xy.xypay.utils.XYPaySDK;

import java.util.concurrent.ConcurrentHashMap;

public class ServiceImpl extends PlatformService implements ICallbackHandle {
    boolean isBoundPhone = false;
    String bindPhoneNum = "";

    //TODO  注意删除不需要的引用， 注意调用SDK接口时 是否需要添加try catch  有些第三方SDK可能存在设备不兼容或者没判空的问题 导致闪退 卡死
    //TODO  可以考虑当 isDebug = true时 开启防御Crash框架 ？？？
    @Override
    public void init(Boolean isDebug) {
        super.init(isDebug);
        Global.IS_DEBUG = isDebug.booleanValue();

        //test!
        testGameCallback();
    }

    @Override
    public void login(ICallbackHandler callbackHandler) {
        super.login(callbackHandler);

        //TODO  账号转移功能
        XYPaySDK.login(ControlUtil.getCurrentContext(), new XYLoginCallback() {
            @Override
            public void onLogin(LoginResult loginResult) {
                switch (loginResult.getCode()) {
                    case LoginResult.LOGIN_CODE_SUCCESS:
                        _Onlogin(callbackHandler, loginResult);
                        break;
                    default:
                        //TODO 输出异常日志
                }
            }
        });
    }

    private void _Onlogin(ICallbackHandler callbackHandler, LoginResult loginResult) {
        if(callbackHandler != null && loginResult != null) {
            callbackHandler.addParam(CallbackParamType.openuid.value,
                    loginResult.getOpenUid());
            callbackHandler.addParam(CallbackParamType.token.value,
                    PreferenceUtils.getToken(ControlUtil.getCurrentContext()));
            ControlUtil.doCall(callbackHandler);
        }
    }

    @Override
    public void logout() {
        super.logout();

        XYPaySDK.logout(ControlUtil.getCurrentContext(), new XYLogoutCallback() {
            @Override
            public void onLogout(LogoutResult logoutResult) {
                switch (logoutResult.getCode()) {
                    case LogoutResult.LOGOUT_CODE_OUT:
                        exit();
                        break;
                    default:
                        //TODO 输出异常日志
                }
            }
        });
    }

    @Override
    public void exit() {
        super.exit();
        ControlUtil.exit();
    }

    //TODO 一定要做混淆
    //TODO logger通用
    //TODO isDebug 放到全局里
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

        XPayArg payArg = new XPayArg();
        payArg.APP_NAME = appName;
        payArg.APP_EXT1 = extra;
        payArg.APP_EXT2 = extra;
        if(Global.IS_DEBUG || StringUtils.isEmpty(price)) {
            payArg.AMOUNT = "0.01";
        }else {
            payArg.AMOUNT = price;
        }
        payArg.SID = serverId;
        payArg.APP_ORDER_ID = extra;
        payArg.PRODUCT_ID = productId;
        payArg.PRODUCT_NAME = productName;
        payArg.NOTIFY_URI = notifyUrl;
        payArg.APP_USER_ID = accountId;
        payArg.APP_USER_NAME = accountName;
        payArg.OPEN_UID = openuid;
        payArg.APP_KEY = Global.APP_KEY;

        XYPaySDK.pay(ControlUtil.getCurrentContext(), payArg, new XYPayCallback() {
            @Override
            public void onPayFinished(int status) {
                switch(status){
                    case XYPaySDK.XYPay_RESULT_CODE_SUCCESS:
                        _OnPay(callbackHandler, CallbackParamType.paysuccess);
                        break;
                    case XYPaySDK.XYPay_RESULT_CODE_FAILURE:
                        _OnPay(callbackHandler, CallbackParamType.payfailed);
                        break;
                    case XYPaySDK.XYPay_RESULT_CODE_CANCEL:
                    default:
                        _OnPay(callbackHandler, CallbackParamType.paycancal);
                        break;
                }
            }

            @Override
            public void onStart(PayArgsCheckResult payArgsCheckResult) {
                String msg = payArgsCheckResult.getMessage();
                /*switch (payArgsCheckResult.getCode()){
                    case PayArgsCheckResult.CHECK_RESULT_OK:
                        msg = "参数正常";
                        break;
                    case PayArgsCheckResult.NOT_INIT:
                        msg = "未调用XYPaySDK.init()方法.";
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_NULL:
                        msg = "支付参数未配置";
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_CALLBACK_NULL:
                        msg = "支付回调函数未配置";
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_AMOUNT:
                        msg = "金额无效";
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_APP_ID:
                        msg = "app_id无效,请检查AndroidManifest.xml文件";
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_APP_NAME:
                        msg = "应用名称无效";
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_APP_USER_ID:
                        msg = "用户编号(角色)为空";
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_APP_USER_NAME:
                        msg = "角色名为空";
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_NOTIFY_URI:
                        msg = "通知服务器地址为空";
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_OPEN_UID:
                        msg = "用户open_uid为空";
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_ORDER_ID:
                        msg = "订单号无效";
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_PRODUCT_ID:
                        msg = "产品号为空";
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_PRODUCT_NAME:
                        msg = "产品名称为空";
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_RESOURCE_ID:
                        msg = "未登录或APP_KEY无效";
                        break;
                    default:
                        msg = "未知错误";
                }*/
                LoggerWrapper.getInstance().logInfo(msg);
            }
        });
    }

    private void _OnPay(ICallbackHandler callbackHandler, CallbackParamType type) {
        if(callbackHandler != null) {
            callbackHandler.addParam(type.value, type.valueInt);
            ControlUtil.doCall(callbackHandler);
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
