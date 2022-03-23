using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMSDKClient;

public class PluginManager : Singleton<PluginManager>
{
    public enum CallType
    {
        None,
        Init,
        Login,
        Pay,

        TestCallResult,

        InitBugly,
        SetBuglyUserId,
    }

    public static readonly string ON_LOGIN = "OnLogout";
    public static readonly string ON_TEST_GAMECALLBACK = "TestGameCallback";

    public override void Init()
    {
        SDKCallbackManager.GetInstance().AddCmd(ON_LOGIN, _OnLogout);
        SDKCallbackManager.GetInstance().AddCmd(ON_TEST_GAMECALLBACK, _OnTestGameCallback);
    }

    public override void UnInit()
    {
    }

    public void Call(CallType type)
    {
        switch (type)
        {
            case CallType.Init:
                _InitPlatformSDK();
                break;
            case CallType.Login:
                _LoginPlatformSDK();
                break;
            case CallType.Pay:
                _PayPlatformSDK();
                break;
            case CallType.TestCallResult:
                _TestCallResult();
                break;
            case CallType.InitBugly:
                _InitBugly();
                break;
            case CallType.SetBuglyUserId:
                _SetBuglyUserId();
                break;
        }
    }

    private void _InitPlatformSDK()
    {
        SDKCallInfo callInfo = new SDKCallInfo(1);
        callInfo.SetName("init");
        callInfo.SetIsCallback(false);
        callInfo.AddArg("isDebug", true);

        SDKUtility.SDKDebugFormat(DebugType.NormalNoMask, callInfo.ToString());

        SDKCallManager.GetInstance().Call<object>(callInfo);
    }

    private void _LoginPlatformSDK()
    {
        SDKCallInfo callInfo = new SDKCallInfo(0, true);
        callInfo.SetName("login");
        callInfo.SetIsCallback(false);
        SDKCallbackManager.GetInstance().AddCmd(callInfo.callbackId, _OnLogin);

        SDKUtility.SDKDebugFormat(DebugType.NormalNoMask, callInfo.ToString());

        SDKCallManager.GetInstance().Call<object>(callInfo);
    }

    private void _PayPlatformSDK()
    {
        //TODO
    }

    private void _TestCallResult()
    {
        SDKCallInfo callInfo = new SDKCallInfo(0, true);
        callInfo.SetName("testCallResult");
        callInfo.SetIsCallback(false);

        SDKUtility.SDKDebugFormat(DebugType.NormalNoMask, callInfo.ToString());

        var callRes = SDKCallManager.GetInstance().Call<bool>(callInfo);
        if (callRes != null) {
            SDKUtility.SDKDebugFormat(DebugType.NormalNoMask, "Test Call Result : code: {0}, msg: {1}, obj: {2}", callRes.code, callRes.message, callRes.obj);
        }
    }

    private void _InitBugly()
    {
        SDKCallInfo callInfo = new SDKCallInfo(2);
        callInfo.SetName("initCrashReport");
        callInfo.AddArg<string>("appId", "8888dda3f9");
        callInfo.AddArg<bool>("isDebug", true);

        SDKUtility.SDKDebugFormat(DebugType.NormalNoMask, callInfo.ToString());
        SDKCallManager.GetInstance().Call<object>(callInfo);
    }

    private void _SetBuglyUserId()
    {
        SDKCallInfo callInfo = new SDKCallInfo(1);
        callInfo.SetName("setUserId");
        callInfo.AddArg<string>("userId", "5211314a");

        SDKUtility.SDKDebugFormat(DebugType.NormalNoMask, callInfo.ToString());
        SDKCallManager.GetInstance().Call<object>(callInfo);
    }





    private void _OnLogin(object[] args, string callbackId)
    {
        if (args == null || args.Length <= 0)
        {
            SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "Onlogin, args is null or length is zero");
            return;
        }

        string openuid = SDKUtility.GetSDKCallArgValue<string>("openuid", args);
        string token = SDKUtility.GetSDKCallArgValue<string>("token", args);

        SDKUtility.SDKDebugFormat(DebugType.NormalNoMask, "Onlogin, openuid : {0}, token : {1}", openuid, token);
    }

    private void _OnPay(object[] args, string callbackId)
    {
        //TODO
    }

    private void _OnLogout(object[] args, string callbackId)
    {
        SDKUtility.SDKDebugFormat(DebugType.NormalNoMask, "Onlogout !");
    }

    private void _OnTestGameCallback(object[] args, string callbackId)
    {
        SDKUtility.SDKDebugFormat(DebugType.NormalNoMask, "_OnTestGameCallback !");

        //callback to android or ios
        SDKCallInfo callInfo = new SDKCallInfo(0);
        callInfo.SetName(callbackId);
        callInfo.SetIsCallback(true);
        SDKCallManager.GetInstance().Call<bool>(callInfo);
    }
}
