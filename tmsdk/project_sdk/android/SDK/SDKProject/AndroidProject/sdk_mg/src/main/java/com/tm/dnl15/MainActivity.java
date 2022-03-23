package com.tm.dnl15;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;

import com.example.administrator.myapplication.BaseActivity;
import com.tm.dnl.util.AndroidBug5497Workaround;
import com.mgbase.utils.BaseAnchorImpl;
import com.mgbase.utils.PreferenceUtils;
import com.mgbase.utils.StringUtils;
import com.mgpay.utils.MGPayAnchor;
import com.tm.dnl.util.GlobalParams;
import com.tm.dnl.util.IAndroidChannel;
import com.tm.dnl.util.Logger;
import com.tm.dnl.util.LoginFailCode;
import com.tm.dnl.util.ReportSceneType;
import com.tm.dnl.util.UnitySendMsg;
import com.xy.xypay.bean.BindPhoneResult;
import com.xy.xypay.bean.LoginResult;
import com.xy.xypay.bean.LogoutResult;
import com.xy.xypay.bean.PayArgsCheckResult;
import com.xy.xypay.bean.XPayArg;
import com.xy.xypay.inters.XYBindPhoneCallback;
import com.xy.xypay.inters.XYLoginCallback;
import com.xy.xypay.inters.XYLogoutCallback;
import com.xy.xypay.inters.XYPayCallback;
import com.xy.xypay.utils.XYPaySDK;
import com.tm.dnl.util.JsonParser;

import org.json.JSONException;
import org.json.JSONObject;

/**
 * Android Plugin for Unity
 * Created by Administrator on 2017/7/17.
 */
public class MainActivity extends BaseActivity implements IAndroidChannel
{

    private static final int CALL_QUIT = 1;//退出游戏
    private static final int CALL_RESUME = 2;//返回按键处理

    boolean isBoundPhone = false;
    String bindPhoneNum = "";
    Handler mgHandler;

    @Override
    protected void onCreate(Bundle bundle) {
        super.onCreate(bundle);
        AndroidBug5497Workaround.assistActivity(this);

        mgHandler = new Handler(new Handler.Callback() {
            @Override
            public boolean handleMessage(Message message) {
                switch(message.what)
                {
                    case CALL_QUIT:
                        try {
                            //新加： 2018-07-21
                            //sdk v 3.0.10
                            PreferenceUtils.setUid(MainActivity.this, "");
                            HideFloatWindow();
                            //悬浮球里切换帐号时退出游戏这里处理
                            //String uid = message.obj.toString();//MG平台的用户id
                            UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_LOGOUT,"");
                            //ShowToast("MG - 悬浮球里切换帐号时退出游戏这里处理？ send msg to unity!!! " + uid);
                        }catch(Exception e)
                        {
                            Logger.LogError("### MG 切换帐号错误 ：%s", e.toString());
                        }
                        break;
                    case CALL_RESUME:
                        Logger.LogDebug("MG - 切换帐号 取消操作");
                        //从全屏切换回游戏，游戏黑屏的问题，可以在CALL_RESUME中处理，可以调用onWindowFocusChanged(true)或者调用onResume()
                        onResume();
                        onWindowFocusChanged(true);
                        break;
                    default:
                        break;
                }
                return false;
            }
        });
    }

    @Override
    public void onWindowFocusChanged(boolean hasFocus) {
        super.onWindowFocusChanged(hasFocus);
    }

    @Override
    public void Init(boolean isDebug, String apkInfo)
    {
        GlobalParams.IsDebug = isDebug;
        Logger.LOG_TAG = "MG_DEBUG";
        GlobalParams.APP_ID = "316";

        //Logger.LogDebug("apkInfo is %s : ",apkInfo);

        JSONObject apkInfoJson = null;
        try {
            apkInfoJson = new JSONObject(apkInfo);
            GlobalParams.APP_NAME = JsonParser.getString(apkInfoJson,"appName");
            GlobalParams.APP_COMPANY_NAME = JsonParser.getString(apkInfoJson,"companyName");
            GlobalParams.APP_BUNDLE_ID = JsonParser.getString(apkInfoJson,"appBundleId");

            Logger.LogDebug("apkInfo is %s : ",GlobalParams.APP_COMPANY_NAME);
        }catch (Exception e)
        {
            e.printStackTrace();
            Logger.LogError("init decode json failed: %s", e.getMessage());
        }

        XYPaySDK.init(getApplication(),new BaseAnchorImpl(),new MGPayAnchor());
        XYPaySDK.setHandler(mgHandler);
        Logger.LogDebug("初始化成功，是否调试？ "+isDebug);
    }

    @Override
    public void Login()
    {
        try {
            if (!XYPaySDK.isInit()) {
                XYPaySDK.init(getApplication(), new BaseAnchorImpl(), new MGPayAnchor());
            }
            XYPaySDK.login(MainActivity.this, new XYLoginCallback() {
                @Override
                public void onLogin(LoginResult loginResult) {
                    if (loginResult.getCode() == LoginResult.LOGIN_CODE_SUCCESS) {
                        String token = PreferenceUtils.getToken(getApplicationContext());
                        String openUid = loginResult.getOpenUid();
                        Logger.LogDebug("登录成功 1 ，openuid : %s， token : %s" , loginResult.getOpenUid() , token);

                        String sendMsg = String.format("%s,%s",openUid, token);
                        Logger.LogDebug("登录成功 2 , send unity msg : %s", sendMsg);
                        UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_LOGIN, sendMsg);

                        String uid = PreferenceUtils.getUid(MainActivity.this);
                        Logger.LogDebug("登录成功 3 , uid : %s", uid);

                        isBoundPhone = loginResult.isBindPhone();
                        Logger.LogDebug("登录成功 4 , isBoundPhone : %s", uid);

                    } else if (loginResult.getCode() == LoginResult.LOGIN_CODE_APPID_NOT_FOUND) {
                        Logger.LogErrorDebug("登录失败，appid没找到");
                        UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_LOGINFAIL,
                                String.valueOf(LoginFailCode.APPIDNOTFOUND.toInt()));
                    } else if (loginResult.getCode() == LoginResult.LOGIN_CODE_FAILED) {
                        Logger.LogErrorDebug("登录失败");
                        UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_LOGINFAIL,
                                String.valueOf(LoginFailCode.LOGINFAIL.toInt()));

                    } else if (loginResult.getCode() == LoginResult.LOGIN_CODE_CANCEL) {
                        Logger.LogErrorDebug("登录取消");
                        UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_LOGINFAIL,
                                String.valueOf(LoginFailCode.LOGINCANCEL.toInt()));

                    } else if(loginResult.getCode() == LoginResult.NOT_INIT) {
                        Logger.LogErrorDebug("SDK未初始化");
                        UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_LOGINFAIL,
                                String.valueOf(LoginFailCode.NOTINIT.toInt()));

                    }else{
                        UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_LOGINFAIL,
                                String.valueOf(LoginFailCode.UNKONW.toInt()));
                    }
                }
            });
        }catch(Exception e)
        {
           Logger.LogError("login: %s", e.getMessage());
            UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_LOGINFAIL,
                    String.valueOf(LoginFailCode.UNKONW.toInt()));

        }
    }

    /*
    游戏内流程是  登录 - 登录验证 - 点击切换账号按钮 - 登录 （切换账号功能 在SDK内实现）
    public void ChangeAccount()
    {
        XYPaySDK.changAccount(this, new XYLoginCallback() {
            @Override
            public void onLogin(LoginResult loginResult) {
                //登录成功
                if (loginResult.getCode() == LoginResult.LOGIN_CODE_SUCCESS) {
                    ShowToast("登录成功:"+loginResult.isBindPhone());
                    String token = PreferenceUtils.getToken(getApplicationContext());
                    UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_LOGIN, loginResult.getOpenUid()+","+token);
                } else if (loginResult.getCode() == LoginResult.LOGIN_CODE_APPID_NOT_FOUND) {
                    ShowToast("APP_ID未在AndroidManifest.xml中配置");
                } else if (loginResult.getCode() == LoginResult.LOGIN_CODE_FAILED) {
                    ShowToast("登录失败");
                } else if (loginResult.getCode() == LoginResult.LOGIN_CODE_CANCEL) {
                    ShowToast("登录取消");
                }
            }
        });
    }*/

    @Override
    public void Logout()
    {
        try {
            if (!XYPaySDK.isInit()) {
                return;
            }
            XYPaySDK.logout(MainActivity.this, new XYLogoutCallback() {
                @Override
                public void onLogout(LogoutResult logoutResult) {
                    switch (logoutResult.getCode()) {
                        case LogoutResult.LOGOUT_CODE_OUT:
                            //finish();
                            //System.exit(0);
                            Logger.LogDebug("登出SDK成功");
                            break;
                        case LogoutResult.LOGOUT_CANCEL:
                            Logger.LogDebug("取消登出SDK");
                            break;
                        case LogoutResult.LOGOUT_CODE_BBS:
                            Logger.LogDebug("登出SDK到论坛");
                            break;
                    }
                }
            });
        }catch(Exception e)
        {
            Logger.LogError("logout: %s", e.getMessage());
        }
    }

    @Override
    public void Pay(String payInfo, String userInfo)
    {
        XPayArg payArg = new XPayArg();
        JSONObject payInfoJson = null;
        JSONObject userInfoJson = null;
        try
        {
            if(payInfo == "" || userInfo == "")
            {
                Logger.LogError("### Pay out param");
                return;
            }
            Logger.LogDebug("payInfo is  : %s ", payInfo);
            Logger.LogDebug("userInfo is  : %s ", userInfo);
            payInfoJson = new JSONObject(payInfo);
            userInfoJson = new JSONObject(userInfo);
        }catch(Exception e)
        {
            Logger.LogError("### Pay json 解析失败");
        }

        try
        {
            if(payInfoJson == null || userInfoJson == null)
            {
                return;
            }
            String payPrice = JsonParser.getString(payInfoJson,"price");
            if(StringUtils.isEmpty(payPrice) || GlobalParams.IsDebug)
                payArg.AMOUNT = "0.01";                                                 //测试数值 isDebug
            else
                payArg.AMOUNT = payPrice;
            payArg.APP_EXT1 = "ext1";
            payArg.APP_EXT2 = "ext2";
            payArg.APP_KEY = GlobalParams.APP_ID;
            payArg.APP_NAME = GlobalParams.APP_NAME;
            String extra = JsonParser.getString(payInfoJson,"extra");
            if(!StringUtils.isEmpty(extra))
            {
                //特殊渠道MG 订单号为透传参数
                //因为这个渠道的两个ext1 ext2不传给游戏服。。。
                //透传带有英文逗号 这个MG渠道发送给游戏服是支持的
				String[] strs =  extra.split(",");
                if(strs != null && strs.length >= 3) {
                    //和1.0的订单号构成一致，这样服务器就不改了
                    payArg.APP_ORDER_ID = String.format("%s,%s,%s", strs[0], strs[1], strs[2]);
                }else
				{
                	payArg.APP_ORDER_ID = extra;
				}
            }else
            {
                Logger.LogError("### Call Pay() extra is null !");
            }
            payArg.APP_USER_ID = JsonParser.getString(userInfoJson,"openUid");  //注意，和1.0保持一致，也传openuid !!!
            payArg.APP_USER_NAME = JsonParser.getString(payInfoJson,"roleName");
            payArg.NOTIFY_URI = JsonParser.getString(payInfoJson,"payCallbackUrl");
            payArg.OPEN_UID = JsonParser.getString(userInfoJson,"openUid");
            payArg.PRODUCT_ID = JsonParser.getString(payInfoJson,"productId");
            payArg.PRODUCT_NAME = JsonParser.getString(payInfoJson,"productName");
            payArg.SID = JsonParser.getString(payInfoJson,"serverId");
        }
        catch (Exception e)
        {
            e.printStackTrace();
            Logger.LogError("### Call Pay() failed");
        }

        Logger.LogDebug("payArg.NOTIFY_URI:%s, payArg.SID:%s, payArg.APP_KEY: %s,payArg.APP_NAME: %s",payArg.NOTIFY_URI,payArg.SID, payArg.APP_KEY,payArg.APP_NAME);
        Logger.LogDebug("payArg.APP_ORDER_ID:%s, payArg.APP_USER_ID:%s, payArg.APP_USER_NAME: %s,payArg.OPEN_UID: %s",payArg.APP_ORDER_ID,payArg.APP_USER_ID, payArg.APP_USER_NAME,payArg.OPEN_UID);
        Logger.LogDebug("payArg.AMOUNT:%s, payArg.PRODUCT_NAME:%s,payArg.PRODUCT_ID: %s",payArg.AMOUNT,payArg.PRODUCT_NAME,payArg.PRODUCT_ID);

        XYPaySDK.pay(this, payArg, new XYPayCallback() {
            @Override
            public void onPayFinished(int status) {
                switch(status){
                    case XYPaySDK.XYPay_RESULT_CODE_SUCCESS:
                        UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_ONPAY,"0");
                        Logger.LogDebug("支付成功");
                        break;
                    case XYPaySDK.XYPay_RESULT_CODE_FAILURE:
                        UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_ONPAY,"1");
                        Logger.LogDebug("支付失败");
                        break;
                    case XYPaySDK.XYPay_RESULT_CODE_CANCEL:
                    default:
                        UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_ONPAY,"2");
                        Logger.LogDebug("支付取消");
                        break;
                }
            }

            @Override
            public void onStart(PayArgsCheckResult payArgsCheckResult) {
                switch (payArgsCheckResult.getCode()){
                    case PayArgsCheckResult.CHECK_RESULT_OK:
                        Logger.LogDebug("参数正常");
                        break;
                    case PayArgsCheckResult.NOT_INIT:
                        Logger.LogDebug("未调用XYPaySDK.init()方法.");
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_NULL:
                        Logger.LogDebug("支付参数未配置");
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_CALLBACK_NULL:
                        Logger.LogDebug("支付回调函数未配置");
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_AMOUNT:
                        Logger.LogDebug("金额无效");
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_APP_ID:
                        Logger.LogDebug("app_id无效,请检查AndroidManifest.xml文件");
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_APP_NAME:
                        Logger.LogDebug("应用名称无效");
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_APP_USER_ID:
                        Logger.LogDebug("用户编号(角色)为空");
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_APP_USER_NAME:
                        Logger.LogDebug("角色名为空");
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_NOTIFY_URI:
                        Logger.LogDebug("通知服务器地址为空");
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_OPEN_UID:
                        Logger.LogDebug("用户open_uid为空");
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_ORDER_ID:
                        Logger.LogDebug("订单号无效");
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_PRODUCT_ID:
                        Logger.LogDebug("产品号为空");
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_PRODUCT_NAME:
                        Logger.LogDebug("产品名称为空");
                        break;
                    case PayArgsCheckResult.CHECK_RESULT_PAY_INVALID_RESOURCE_ID:
                        Logger.LogDebug("未登录或APP_KEY无效");
                        break;
                }
            }
        });
    }

    /*
      手机号绑定 - 打开绑定页面
  * */
    public void OpenBindPhone()
    {
        try {
            XYPaySDK.showBindPhone(this, new XYBindPhoneCallback() {
                @Override
                public void onBindPhone(BindPhoneResult bindPhoneResult) {
                    if (bindPhoneResult.isBindSuccess()) {
                        isBoundPhone = true;
                        bindPhoneNum = bindPhoneResult.getPhone();
                        UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_ISBINDPHONESUCC, bindPhoneResult.getPhone());
                    }
                }
            });
        }catch(Exception e)
        {
            Log.e("MG_DEBUG","打开手机绑定页面失败 : "+e.getMessage());
            e.printStackTrace();
        }
    }

    /*
           手机号绑定 - 监听绑定成功回调
    * */
    public void CheckBindPhoneSucc()
    {
        try
        {
            XYPaySDK.getBindPhone(MainActivity.this, new XYBindPhoneCallback() {
                @Override
                public void onBindPhone(BindPhoneResult bindPhoneResult) {
                    if(bindPhoneResult.isBindSuccess())
                    {
                        isBoundPhone = true;
                        bindPhoneNum = bindPhoneResult.getPhone();
                        UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_ISBINDPHONESUCC, bindPhoneResult.getPhone());
                    }
                }
            });
        }catch(Exception e)
        {
            Log.e("MG_DEBUG","get bind phone num failed : "+e.getMessage());
            e.printStackTrace();
        }
    }

    public String GetBindPhoneNum()
    {
        return bindPhoneNum;
    }

    public boolean IsBindPhoneSucc()
    {
        return isBoundPhone;
    }

    /*
        上报角色信息
    * */
    public void ReportRoleInfo(String roleInfo, int sceneType)
    {
        JSONObject roleJsonObject = null;
        try {
            roleJsonObject = new JSONObject(roleInfo);
        } catch (JSONException e) {
            e.printStackTrace();
            Log.e("MG_DEBUG","ROLEJSON解析失败 : "+e.getMessage());
        }
        try {
            String roleId = JsonParser.getString(roleJsonObject, "roleId");
            String roleName = JsonParser.getString(roleJsonObject, "roleName");
            String roleLevel = JsonParser.getString(roleJsonObject, "roleLevel");
            String serverName = JsonParser.getString(roleJsonObject, "serverName");
            String serverId = JsonParser.getString(roleJsonObject, "serverId");
            String jobName = JsonParser.getString(roleJsonObject, "jobName");

            if( sceneType == ReportSceneType.CreateRole.toInt())
            {
                XYPaySDK.createRole(
                        roleId,
                        serverName,
                        roleName);
                Logger.LogDebug("createRole to sdk, roleId: %s, serverName : %s, roleName : %s",roleId, serverName, roleName);
            }
            else if(sceneType == ReportSceneType.Login.toInt())
            {
                XYPaySDK.roleLogin(roleId, serverId, roleName, roleLevel, jobName);
                Logger.LogDebug("role login to sdk, roleId: %s, serverName : %s, roleName : %s, serverId : %s, roleLevel : %s, jobName : %s",
                        roleId, serverName, roleName, serverId, roleLevel, jobName);
            }
        }catch(Exception e)
        {
            e.printStackTrace();
            Logger.LogError("report create role info failed !");
        }
    }

    @Override
    protected void onPause() {
        super.onPause();
        HideFloatWindow();
    }

    @Override
    protected void onResume() {
        super.onResume();
        ShowFloatWindow();
    }

    @Override
    protected void onDestroy() {
        HideFloatWindow();
        super.onDestroy();
    }

    public void ShowFloatWindow()
    {
        try {
            XYPaySDK.showFloatWindow(this);
        }
        catch(Exception e)
        {
            e.printStackTrace();
            Logger.LogError("showFloatWindow failed !");
        }
    }

    public void HideFloatWindow()
    {
        try {
            XYPaySDK.hideFloatWindow(this);
        }catch(Exception e)
        {
            e.printStackTrace();
            Logger.LogError("hideFloatWindow failed !");
        }
    }

    public void OpenUserCenter()
    {
        XYPaySDK.userInfo(this);
    }

    /**
     * 热更开始接口
     */
    public void OnHotStart(){

        try {
            String type="hotStart";//热更开始
            XYPaySDK.hotActivation(MainActivity.this,type);
        }catch (Exception e)
        {
            Logger.LogError("report start hotfix failed !");
            e.printStackTrace();
        }
    }
    /**
     * 热更结束接口
     */
    public void OnHotEnd(){

        try {
            String type="hotEnd";//热更结束
            XYPaySDK.hotActivation(MainActivity.this,type);
        }catch (Exception e)
        {
            Logger.LogError("report end hotfix failed !");
            e.printStackTrace();
        }
    }
}
