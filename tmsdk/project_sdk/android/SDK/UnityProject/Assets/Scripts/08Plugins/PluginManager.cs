using System;
using System.Collections;
using System.Collections.Generic;
using SDKClient;
using UnityEngine;

public class PluginManager : Singleton<PluginManager>
{
    private const string BUGLY_ANDROID_APPID = "fac8cca1e6";
    private const string BUGLY_IOS_APPID = "ffa5a11f5e";

    bool isSDKInited = false;
    bool isSDKDebug = false;

    private void InitBugly(bool isSDKDebug)
    {
        //BuglyAgent.ConfigDebugMode(isSDKDebug);

#if UNITY_IPHONE || UNITY_IOS
		//BuglyAgent.InitWithAppId (BUGLY_IOS_APPID);
#elif UNITY_ANDROID
        //BuglyAgent.InitWithAppId (BUGLY_ANDROID_APPID);
#endif
        //BuglyAgent.EnableExceptionHandler();
    }

    public override void Init()
    {
        InitializeSDK();

        //InitBugly();
#if USE_LEBIAN
        LebianRequestUpdate();
#endif
    }

    public void InitializeSDK()
    {
        if (!isSDKInited)
        {
            _InitChannelSDK();
            isSDKInited = true;
        }
    }

    private void _InitChannelSDK()
    {
#if DEBUG_SETTING
		isSDKDebug = Global.Settings.isDebug;
#else
        isSDKDebug = false;

        byte[] data = null;
        string SPEICY_FILE_NAME = "DGameSDK.xml";
        if (FileArchiveAccessor.LoadFileInPersistentFileArchive(SPEICY_FILE_NAME, out data))
        {
            isSDKDebug = true;
        }

#endif

        //Test
        isSDKDebug = true;
        SDKInterface.Instance.Init(isSDKDebug);

        SDKInterface.Instance.loginCallbackGame = (SDKUserInfo userInfo) =>
        {
            ChannelSDKTest.sdkUserInfo = userInfo;
            Debug.LogErrorFormat("[loginCallbackGame] {0} {1} {2}", userInfo.token, userInfo.openUid, userInfo.ext);
        };

        SDKInterface.Instance.logoutCallbackGame = () =>
        {
            //注意 ： 需要判断当前游戏的状态 是否可以退出 ！！！
            Debug.LogFormat("### 通知客户端退出");
        };

        SDKInterface.Instance.payResultCallbackGame = (string res) =>
        {
            Debug.LogFormat("### 客户端支付结果（不作为真正支付结果）: {0}", res);
        };

        SDKInterface.Instance.bindPhoneCallbackGame = (string bindPhoneNum) =>
        {
            Debug.LogFormat("### 手机绑定成功 手机号: {0}", bindPhoneNum);
        };
    }

    #region Channel SDK

    #region SDK Channel Info

    public SDKChannelInfo GetSDKChannelInfo()
    {
        return SDKInterface.SDKChannelInfo;
    }

    /// <summary>
    /// 获取SDK渠道参数 小写格式
    /// </summary>
    /// <returns></returns>
    public string GetSDKChannelParam()
    {
        if (null != GetSDKChannelInfo())
        {
            return GetSDKChannelInfo().channelParam;
        }
        return "none";
    }

    /// <summary>
    /// 获取SDK渠道类型 大写格式
    /// </summary>
    /// <returns></returns>
    public string GetSDKChannelType()
    {
        if (null != GetSDKChannelInfo())
        {
            return GetSDKChannelInfo().channelType;
        }
        return "NONE";
    }

    public string GetSDKPayCallbackUrlFormat()
    {
        if (null != GetSDKChannelInfo())
        {
            return GetSDKChannelInfo().payCallbackUrlFormat;
        }
        return "http://{0}/mg_charge?";
    }

    public bool NeedPayToken()
    {
        if (null != GetSDKChannelInfo())
        {
            return GetSDKChannelInfo().needPayToken;
        }
        return false;
    }

    /// <summary>
    /// 内购参数
    /// </summary>
    /// <param name="s"></param>
    public void SetIosIap(string s)
    {
        if (string.IsNullOrEmpty(s)) return;
        SDKInterface.Instance.SetIosIap(s);
    }

    #endregion

    public SDKDeviceInfo GetSDKDeviceInfo()
    {
        return SDKInterface.Instance.SDKDeviceInfo;
    }

    public void OpenSDKLogin()
    {
        SDKInterface.Instance.Login();
    }

    public void LogoutSDK()
    {
        SDKInterface.Instance.Logout();
    }

    public void PaySDK(SDKPayInfo payInfo)
    {
        if (payInfo == null)
        {
            Debug.LogErrorFormat("### PluginManager PaySDK param is null !");
            return;
        }
        SDKInterface.Instance.Pay(payInfo);
    }

    public void SetSpecialPayUserInfo(string openUid, string token)
    {
        SDKInterface.Instance.SetSpecialPayUserInfo(openUid, token);
    }

    public void OpenSDKMobileBind()
    {
        SDKInterface.Instance.OpenBindPhone();
    }

    public void CheckSDKMobileBind()
    {
        SDKInterface.Instance.CheckIsPhoneBind();
    }

    public void ReportSDKRoleInfo(SDKClient.SDKReportRoleInfo roleInfo)
    {
        if (roleInfo == null)
        {
            Debug.LogErrorFormat("### PluginManager ReportRoleInfo param is null !");
            return;
        }
        SDKInterface.Instance.ReportRoleInfo(roleInfo);
    }

    public string LoginVerifyUrl(SDKClient.SDKLoginVerifyUrl verifyUrl)
    {
        if (verifyUrl == null)
        {
            Debug.LogErrorFormat("### PluginManager LoginVerifyUrl param is null !");
            return "";
        }
        return SDKInterface.Instance.LoginVerifyUrl(verifyUrl);
    }

    #endregion
    #region UniWebView
    public void AddKeyboardShowListener(SDKInterface.KeyboardShowOut OnKeyboardres)
    {
        if (OnKeyboardres != null)
        {
            SDKInterface.Instance.keyboardShowCallbackGame = OnKeyboardres;
        }
    }

    public void RemoveKeyboardShowListener()
    {
        SDKInterface.Instance.keyboardShowCallbackGame = null;
    }
    #endregion
    #region LB
    public void LebianRequestUpdate()
    {
        SDKInterface.Instance.LBRequestUpdate();
    }

    public void LebianDownloadFullRes()
    {
        SDKInterface.Instance.LBDownloadFullRes();
    }

    public bool LebianIsAfterUpdate()
    {
        return SDKInterface.Instance.LBIsAfterUpdate();
    }

    public string LebianGetFullResPath()
    {
        return SDKInterface.Instance.LBGetFullResPath();
    }
    #endregion

    #region Common lib

    /// <summary>
    /// 初始化 SDK通用接口
    /// </summary>
    public void InitCommonLib()
    {
        SDKInterface.Instance.InitCommonLib();
    }

    /// <summary>
    /// 获取设备电量 0-1f
    /// </summary>
    /// <returns></returns>
    public float GetBatteryLevel()
    {
        return SDKInterface.Instance.GetBatteryLevel();
    }

    /// <summary>
    /// 系统时间 （时分） HH:mm
    /// </summary>
    /// <returns></returns>
    public string GetSystemTime_HHMM()
    {
        return SDKInterface.Instance.GetSystemTimeHHMM();
    }

    /// <summary>
    /// 获取系统剪切板
    /// </summary>
    /// <returns></returns>
    public string GetClipboardText()
    {
        return SDKInterface.Instance.GetClipboardText();
    }

    /// <summary>
    /// 触发设备震动
    /// </summary>
    public void TriggerMobileVibrate()
    {
        SDKInterface.Instance.MobileVibrate();
    }
    /// <summary>
    /// 设备是否存在刘海
    /// </summary>
    /// <param name="debug">在编辑器模拟刘海</param>
    /// <returns></returns>
    public bool HasNotch(bool debug = false)
    {
        return SDKInterface.Instance.HasNotch(debug);
    }


    /// <summary>
    /// 设备的刘海左下点(x y)右上点(z w)，注意会因设备旋转处于左右状态，返回的数值为屏幕分辨率缩放比为1的情况下，若要适配当前分辨率，需要乘以手机原始分辨率与当前分辨率的比值，具体数值在GraphicSettings.OnResolutionChanged
    /// </summary>
    /// <param name="debug">(使用前设置显示分辨率为1920*1080)在编辑器模拟刘海</param>
    /// <param name="screenOrientation">(使用前设置显示分辨率为1920*1080)在编辑器模拟刘海所处位置</param>
    /// <returns>左下点(x y)右上点(z w)</returns>
    public Vector4 GetNotchSize(SDKInterface.NotchDebugType debug = SDKInterface.NotchDebugType.None, ScreenOrientation screenOrientation = ScreenOrientation.LandscapeLeft)
    {
        var n = SDKInterface.Instance.GetNotchSize(debug, screenOrientation);
        if (debug != SDKInterface.NotchDebugType.None)
        {
            n = OpenGL2DirectX(n);
            n = DirectX2UGUI(n);
            n = Resort(n);

            Debug.DrawLine(new Vector3(n[0], n[1]), new Vector3(n[2], n[3]), Color.green);
            Debug.DrawLine(new Vector3(n[0], n[3]), new Vector3(n[2], n[1]), Color.blue);
            return new Vector4(n[0], n[1], n[2], n[3]);
        }
        if (n.Length == 2)//SDK24-28厂商提供的接口只返回刘海长宽，无位置信息
        {
            if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.Landscape)
            {
                //转换成左刘海矩形
                n = new int[4] { 0, (Screen.height + n[1]) / 2, n[0], (Screen.height - n[1]) / 2 };
                //转换坐标系到UGUI坐标系
                n = DirectX2UGUI(OpenGL2DirectX(n));
                //重新排序，使x y 为左下点坐标，z w 为右上点坐标
                n = Resort(n);
            }
            else if (Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                //转换成右刘海矩形
                n = new int[4] { Screen.width - n[0], (Screen.height + n[1]) / 2, Screen.width, (Screen.height - n[1]) / 2 };
                //转换坐标系到UGUI坐标系
                n = DirectX2UGUI(OpenGL2DirectX(n));
                //重新排序，使x y 为左下点坐标，z w 为右上点坐标
                n = Resort(n);
            }
        }
        else if (n.Length == 4)//SDK28及以上会返回刘海所在矩形
        {
            n = DirectX2UGUI(OpenGL2DirectX(n));
            n = Resort(n);
        }
        else
        {
            return new Vector4(0, 0, 0, 0);
        }

        return new Vector4(n[0], n[1], n[2], n[3]);
    }
    public int GetSystemVersion()
    {
        return SDKInterface.Instance.GetSystemVersion();
    }
    public bool IsLowVersion(int android, int ios)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return GetSystemVersion() < android;
#endif
#if (UNITY_IOS || UNITY_PHONE) && !UNITY_EDITOR
        return GetSystemVersion() < ios;
#endif
        return true;
    }
    private int[] OpenGL2DirectX(int[] n)
    {
        return new int[4] { n[0], Screen.height - n[1], n[2], Screen.height - n[3] };
    }
    private int[] DirectX2UGUI(int[] n)
    {
        return new int[4] { n[0] - Screen.width / 2, n[1] - Screen.height / 2, n[2] - Screen.width / 2, n[3] - Screen.height / 2 };

    }
    private int[] Resort(int[] n)
    {
        int tmp = 0;
        if (n[0] > n[2])
        {
            tmp = n[0];
            n[0] = n[2];
            n[2] = tmp;
        }

        if (n[1] > n[3])
        {
            tmp = n[1];
            n[1] = n[3];
            n[3] = tmp;
        }
        return n;
    }


    #region 截图残留
    //public void SaveImage2Album(Camera renderCamera)
    //{

    //}
    //private IEnumerator SaveImageCort(Camera camera)
    //{
    //    yield return new WaitForEndOfFrame();

    //    RenderTexture tmp = RenderTexture.GetTemporary(Screen.width, Screen.height);

    //    var a = camera.targetTexture;
    //    RenderTexture.active = camera.targetTexture;
    //    camera.Render();

    //    Texture2D image = new Texture2D(Screen.width, Screen.height);
    //    image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
    //    image.Apply();

    //    RenderTexture.active = tmp;
    //    RenderTexture.ReleaseTemporary(tmp);

    //    byte[] bytes = image.EncodeToJPG();

    //    SDKInterface.Instance.SaveImage2Album(bytes);
    //}
    #endregion
    #endregion
}


