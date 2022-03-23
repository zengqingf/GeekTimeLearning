using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;

namespace SDKClient
{
    //渠道SDK信息
    public class SDKChannelInfo
    {
        public SDKChannelInfo()
        {
            channelName                 = "内网测试";
            channelParam                = "none";
            channelType                 = "NONE";
            platformType                = SDKInterface.OnlineServicePlatformType.Android.ToString();

            payCallbackUrlFormat        = "http://{0}/charge";      

            needPayToken                = false;
            needPayResultNotify         = false;
            serverListName              = "serverList.xml";
            needUriEncodeOpenUid        = false;
            needShowChannelRankBtn      = false;
            needLocalNotification       = false;
            needBindMobilePhone         = false;
        }

        public void LogContents(SDKInterface.DebugType debugType)
        {
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:channelName:{0}", channelName);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:channelParam:{0}", channelParam);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:channelType:{0}", channelType);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:platformType:{0}", platformType);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:payCallbackUrlFormat:{0}", payCallbackUrlFormat);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:needPayToken:{0}", needPayToken);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:needPayResultNotify:{0}", needPayResultNotify);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:serverListName:{0}", serverListName);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:needUriEncodeOpenUid:{0}", needUriEncodeOpenUid);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:needShowChannelRankBtn:{0}", needShowChannelRankBtn);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:needLocalNotification:{0}", needLocalNotification);
            SDKUtility.SDKDebugFormat(debugType, "sdkchannel:needBindMobilePhone:{0}", needBindMobilePhone);
        }

        public string channelName;                          //渠道名
        public string channelParam;                         //渠道参数
        public string channelType;                          //渠道类型
        public string platformType;                         //平台类型

        public string payCallbackUrlFormat;                 //支付回调地址格式

        public bool         needPayToken;                        //支付时需要提前传入服务器返回的登陆信息
        public bool         needPayResultNotify;                 //是否需要把支付结果通知给客户端
        public string       serverListName;                      //服务器列表名称
        public bool         needUriEncodeOpenUid;                //是否需要EncodeUri Uid
        public bool         needShowChannelRankBtn;              //是否需要显示渠道排行榜按钮
        public bool         needLocalNotification;               //是否需要本地推送
        public bool         needBindMobilePhone;                 //是否需要手机绑定
    }
    /// <summary>
    /// sdk 支付接口信息
    /// </summary>
    public class SDKPayInfo
    {

        public string requestId;             //本地构建的订单号
        public string price;

        public string mallType;              //商城类型
        public string productId;             //商品ID

        public string productName;           //商品名称
        public string productShortName;      //商品其他名称
        public string productDesc;           //商品描述

        public string productSDKId;          //商品在SDK设置的Id，苹果是内购档位最后一位字段（前面是bundleid），像酷派联想就是后台配置的id

        public string extra;                 //支付透传参数

        public string serverId;

        public string serverName;
        public string roleId;
        public string roleName;

        public string payCallbackUrl;        //支付回调地址
    }

    public class SDKReportRoleInfo
    {
        public SDKInterface.ReportSceneType sceneType;
        protected uint serverId;
        protected string serverName;
        protected string roleId;
        protected string roleName;
        protected int roleLevel;
        protected int jobId;
        protected string jobName;
        protected int vipLevel;
        protected int couponNum;
    }

    public class SDKLoginVerifyUrl
    {
        public string serverUrl;
        public string serverId;
        public string channelParam;
        public string ext;
    }

    /// <summary>
    /// Apk 信息
    /// </summary>
    public class ApkInfo
    {
        public string 		appName;
        public string 		companyName;
        public string 		appBundleId;
    }

    /// <summary>
    /// iOS初始化额外信息
    /// </summary>
    public class IOSInfoExtra
    {
        public ArrayList iap = new ArrayList();
        public bool checkUpdate;
        public bool isLoadU3dSmallGame;
    }
    
    /// <summary>
    /// sdk 获取的用户信息
    /// </summary>
    public class SDKUserInfo
    {
        public string        openUid;
        public string        token;
        public string        ext;
    }

    /// <summary>
    /// 设备 信息
    /// </summary>
    public class SDKDeviceInfo
    {
        public string       deviceId;
        public string       modeltype;
        public string       osInfo;
    }

    public class SDKInterface
    {
        public enum OnlineServicePlatformType
        {
            [Description("安卓")]
            Android = 0,
            [Description("ios")]
            IOS = 1,
            [Description("ios越狱")]
            IOSOther = 2,
            [Description("其他")]
            Other = 3,
        }


        public enum VipAuthPlatformType
        {
            Other           = 0,
            Android         = 1,
            IOS             = 2,
        }

        public enum ReportSceneType
        {
			None		   	= 0,
            Login          	= 1,
            CreateRole      = 2,
            LevelUp        	= 3,
            Logout         	= 4,
        }

        public enum DebugType
        {
            NormalMask,
            WarningMask,
            ErrorMask,
            NormalNoMask,
            WardingNoMask,
            ErrorNoMask,
        }

        public delegate void LoginCallback(SDKUserInfo userInfo);
        public delegate void LogoutCallback();
        public delegate void PayResultCallback(string result);
        public delegate void BindPhoneCallBack(string bindedPhoneNum);
        public delegate void KeyboardShowOut(string result);
        public delegate void SmallGameLoad(string startSceneName);                      //初始加载小游戏

        public LoginCallback loginCallbackGame;
        public LogoutCallback logoutCallbackGame;
        public PayResultCallback payResultCallbackGame;
        public BindPhoneCallBack bindPhoneCallbackGame;
        public KeyboardShowOut keyboardShowCallbackGame;
        public SmallGameLoad smallGameLoadCallbackGame;

        public const string SDK_CHANNEL_CONFIG_FILENAME = "sdkchannel.conf";
        public const string SDK_NO_CHANNEL_TYPE_NAME = "NONE";//沒有渠道默认值

        private static SDKInterface _instance;

        private static SDKInterface _GetSDKInstance(string currChannelType)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (currChannelType != SDK_NO_CHANNEL_TYPE_NAME)
            {
                return new SDKInterfaceAndroidChannel();
            } 
            else
            {
                return new SDKInterfaceAndroid();
            }
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            if (currChannelType != SDK_NO_CHANNEL_TYPE_NAME)
            {
                return new SDKInterfaceIOSChannel();
            }
            else
            {
                return new SDKInterfaceIOS();
            }
#endif
            return new SDKInterfaceDefault();
        }

        public static SDKInterface Instance
        {
            get
            {
                if (_instance == null)
                {
                    string currChannelType = _LoadSDKChannelConfig();
                    _instance = _GetSDKInstance(currChannelType);
                }
                return _instance;
            }
        }

        protected static SDKChannelInfo m_SDKChannelInfo = new SDKChannelInfo();
        public static SDKChannelInfo SDKChannelInfo
        {
            get {
                if (m_SDKChannelInfo == null)
                {
                    m_SDKChannelInfo = new SDKChannelInfo();
                }
                return m_SDKChannelInfo;
            }
        }

        protected SDKUserInfo m_SDKUserInfo = new SDKUserInfo();
        public SDKUserInfo SDKUserInfo
        {
            get
            {
                if (m_SDKUserInfo == null)
                {
                    m_SDKUserInfo = new SDKUserInfo();
                }
                return m_SDKUserInfo;
            }
            set
            {
                m_SDKUserInfo = value;
            }
        }

        protected SDKDeviceInfo m_SDKDeviceInfo = new SDKDeviceInfo();
        public SDKDeviceInfo SDKDeviceInfo
        {
            get
            {
                if (m_SDKDeviceInfo == null)
                {
                    m_SDKDeviceInfo = new SDKDeviceInfo();
                }
                return m_SDKDeviceInfo;
            }
        }

        protected ApkInfo m_ApkInfo = new ApkInfo();
        public ApkInfo GetApkInfo
        {
            get
            {
                if (m_ApkInfo == null)
                {
                    m_ApkInfo = new ApkInfo();
                }
                return m_ApkInfo;
            }
        }

        protected IOSInfoExtra m_IOSInfoExtra = new IOSInfoExtra();
        public IOSInfoExtra IOSInfoExtra
        {
            get
            {
                if (m_IOSInfoExtra == null)
                {
                    m_IOSInfoExtra = new IOSInfoExtra();
                }
                return m_IOSInfoExtra;
            }
            set
            {
                m_IOSInfoExtra = value;
            }
        }

        /// <summary>
        /// 内购参数
        /// </summary>
        /// <param name="s"></param>
        public void SetIosIap(string s)
        {

            string iap = string.Format("{0}.{1}", Application.identifier, s);
            IOSInfoExtra.iap.Add(iap);
            foreach (var ss in m_IOSInfoExtra.iap)
            {
                Debug.Log("=====iap===" + ss);
            }
        }



        private static byte[] _LoadConfigData()
        {
            byte[] data = null;
            try
            {
                //测试用
                //if (!FileArchiveAccessor.LoadFileInPersistentFileArchive(SDK_CHANNEL_CONFIG_FILENAME, out data))
                //{
                //    SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "can not load file in persistentDataPath : {0}", SDK_CHANNEL_CONFIG_FILENAME);
                //}

                if (!FileArchiveAccessor.LoadFileInLocalFileArchive(SDK_CHANNEL_CONFIG_FILENAME, out data))
                {
                    SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "can not load file in streamingPath: {0}", SDK_CHANNEL_CONFIG_FILENAME);
                }
            }
            catch (Exception e)
            {
                SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "error : {0}", e.ToString());
            }

            return data;
        }

        private static string _LoadSDKChannelConfig()
        {
            string sdkChannelType = SDK_NO_CHANNEL_TYPE_NAME;
            try
            {
                byte[] data = _LoadConfigData();
                if (null != data)
                {
                    string content = System.Text.ASCIIEncoding.Default.GetString(data);
                    if (string.IsNullOrEmpty(content))
                    {
                        SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "load config content is null");
                    }
                    else
                    {
                        m_SDKChannelInfo = LitJson.JsonMapper.ToObject<SDKChannelInfo>(content);
                        if (m_SDKChannelInfo == null)
                        {
                            m_SDKChannelInfo = new SDKChannelInfo();
                        }
                        else
                        {
                            sdkChannelType = m_SDKChannelInfo.channelType;
                        }
                    }
                }
                return sdkChannelType;
            }
            catch (Exception e)
            {
                SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "error : {0}", e.ToString());
                return sdkChannelType;
            }
        }

        #region Channel SDK

        public virtual void Init(bool debug)
        {
            SDKUtility.OPEN_SDK_LOG_WHOLE = debug;

            var sdkCallback = SDKCallback.instance;

            if (debug && null != m_SDKChannelInfo)
            {
                m_SDKChannelInfo.LogContents(DebugType.NormalNoMask);
            }

            if (null != m_SDKDeviceInfo)
            {
                m_SDKDeviceInfo.deviceId = UnityEngine.SystemInfo.deviceUniqueIdentifier;
                m_SDKDeviceInfo.modeltype = GetDeviceModelType();
                m_SDKDeviceInfo.osInfo = GetOperationSystemInfo();
            }
            if (null != m_ApkInfo)
            {
                m_ApkInfo.appBundleId = UnityEngine.Application.identifier;
                m_ApkInfo.appName = UnityEngine.Application.productName;
                m_ApkInfo.companyName = UnityEngine.Application.companyName;
            }            
        }

        public virtual void Login() { }
        public virtual void Logout() { }
        public virtual void Pay(SDKPayInfo payInfo) { }
        public virtual void ReportRoleInfo(SDKReportRoleInfo roleInfo) { }

        //服务器登录验证URL
        public virtual string LoginVerifyUrl(SDKLoginVerifyUrl verifyUrl)
        {
            if (verifyUrl == null || m_SDKChannelInfo == null || m_SDKUserInfo == null)
            {
                return "";
            }
            return string.Format("http://{0}/login?id={1}&openid={2}&token={3}&did={4}&platform={5}&model={6}&device_version={7}",
                verifyUrl.serverUrl, verifyUrl.serverId, m_SDKUserInfo.openUid, m_SDKUserInfo.token,
                m_SDKDeviceInfo.deviceId, verifyUrl.channelParam, m_SDKDeviceInfo.modeltype, m_SDKDeviceInfo.osInfo);
        }

        /// <summary>
        /// 设置特殊的支付时角色信息
        /// 服务器返回帐号信息 传回给SDK
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="accesstoken"></param>
        public virtual void SetSpecialPayUserInfo(string openUid, string token)
        {
            if (m_SDKUserInfo != null)
            {
                m_SDKUserInfo.openUid = openUid;
                m_SDKUserInfo.token = token;
            }
        }

        public virtual bool NeedSDKBindPhoneOpen()
        {
            if (m_SDKChannelInfo != null)
            {
                return m_SDKChannelInfo.needBindMobilePhone;
            }
            return false;
        }
        public virtual void OpenBindPhone() { }
        public virtual void CheckIsPhoneBind() { }

        /// <summary>
        /// 游戏包从小游戏加载的启动场景名
        /// </summary>
        /// <returns></returns>
        public virtual string GetSmallGameLoadFirstSceneName() { return ""; }

        /// <summary>
        /// 游戏是否从小游戏加载
        /// </summary>
        /// <returns></returns>
        public virtual bool IsLoadFromSmallGame() { return false; }

        /// <summary>
        /// 是否从游戏中心启动
        /// </summary>
        /// <returns></returns>
        public virtual bool IsLaunchFromGameCenter() { return false; }

        /// <summary>
        /// 启动游戏中心
        /// </summary>
        public virtual void OpenGameCenter() { }

        public virtual bool IsOppoChannel()
        {
            if (m_SDKChannelInfo == null)
            {
                return false;
            }
            if (m_SDKChannelInfo.channelType.Equals("OPPO"))
            {
                return true;
            }
            return false;
        }

        public virtual bool IsVivoChannel()
        {
            if (m_SDKChannelInfo == null)
            {
                return false;
            }
            if (m_SDKChannelInfo.channelType.Equals("VIVO"))
            {
                return true;
            }
            return false;
        }

        public virtual bool IsOtherChannel()
        {
            if (m_SDKChannelInfo == null)
            {
                return false;
            }
            if (m_SDKChannelInfo.channelType.Equals("Other"))
            {
                return true;
            }
            return false;
        }

        public virtual bool IsAndroidPlatform()
        {
            if (m_SDKChannelInfo == null)
            {
                return false;
            }
            if (m_SDKChannelInfo.platformType.Equals(OnlineServicePlatformType.Android.ToString()))
            {
                return true;
            }
            return false;
        }

        public virtual bool IsIOSAppstorePlatform()
        {
            if (m_SDKChannelInfo == null)
            {
                return false;
            }
            if (m_SDKChannelInfo.platformType.Equals(OnlineServicePlatformType.IOS.ToString()))
            {
                return true;
            }
            return false;
        }

        public virtual bool IsIOSOtherPlatform()
        {
            if (m_SDKChannelInfo == null)
            {
                return false;
            }
            if (m_SDKChannelInfo.platformType.Equals(OnlineServicePlatformType.IOSOther.ToString()))
            {
                return true;
            }
            return false;
        }

        public virtual string GetCurrentChannelParam()
        {
            if (m_SDKChannelInfo == null)
            {
                return "none";
            }
            return m_SDKChannelInfo.channelParam;
        }

        public virtual string GetCurrentChannelType()
        {
            if (m_SDKChannelInfo == null)
            {
                return "NONE";
            }
            return m_SDKChannelInfo.channelType;
        }

        public virtual string NeedUriEncodeOpenid(string openUid)
        {
            if (string.IsNullOrEmpty(openUid))
            {
                return "";
            }
            if (m_SDKChannelInfo == null)
            {
                return "";
            }
            if (m_SDKChannelInfo.needUriEncodeOpenUid)
            {
                openUid = Uri.EscapeDataString(openUid);
            }
            return openUid;
        }

        public virtual bool NeedShowChannelRankBtn()
        {
            if (m_SDKChannelInfo == null)
            {
                return false;
            }
            return m_SDKChannelInfo.needShowChannelRankBtn;
        }

        #endregion

        #region UniWebView
        public virtual int GetKeyboardSize() { return 0; }
        public virtual int TryGetCurrVersionAPI() { return 0; }
        #endregion
        #region Lebian Interface

        public virtual void LBRequestUpdate() { }

		public virtual void LBDownloadFullRes() { }

        public virtual bool LBIsAfterUpdate() { return false; }

        public virtual string LBGetFullResPath() { return string.Empty; }

        #endregion

        //TODO SVN Merge from 1.0
        #region Common lib
        
        /// <summary>
        /// 初始化SDK通用接口
        /// </summary>
        public virtual void InitCommonLib() { }

        /// <summary>
        /// 保持屏幕常亮
        /// </summary>
        /// <param name="isOn">开启常亮</param>
        public virtual void KeepScreenOn(bool isOn) { }

        /// <summary>
        /// 设置屏幕亮度 (0 - 1f)
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetScreenBrightness(float value) { }
        public virtual float GetScreenBrightness() { return 0.5f; }

        /// <summary>
        /// 获取当前电量 <= 1.00f
        /// </summary>
        /// <returns></returns>
        public virtual float GetBatteryLevel() { return 1.00f; }
        public virtual bool IsBatteryCharging() { return true; }

        /// <summary>
        /// 获取系统时间(时分) HH:mm
        /// </summary>
        /// <returns></returns>
        public virtual string GetSystemTimeHHMM()
        {
            DateTime nowTime = DateTime.Now;
            return string.Format("{0:HH:mm}", nowTime);
        }

        /// <summary>
        /// 获取系统剪切板
        /// </summary>
        /// <returns></returns>
        public virtual string GetClipboardText() { return ""; }

        /// <summary>
        /// 获取系统版本信息
        /// </summary>
        /// <returns></returns>
        public string GetOperationSystemInfo()
        {
            return UrlFormatFilter(SystemInfo.operatingSystem);
        }
        /// <summary>
        /// 获取设备型号
        /// </summary>
        /// <returns></returns>
        public string GetDeviceModelType()
        {
            return UrlFormatFilter(SystemInfo.deviceModel);
        }

        private string UrlFormatFilter(string originStr)
        {
            if (string.IsNullOrEmpty(originStr))
            {
                return "";
            }
            return originStr.Replace(" ", "_").Replace("&", "_").Replace("|", "_").Replace(";", "_");
        }

        /// <summary>
        /// 设备震动
        /// </summary>
        public virtual void MobileVibrate()
        {
#if UNITY_IOS || UNITY_ANDROID
            Handheld.Vibrate();
#endif
        }
        public enum NotchDebugType
        {
            None,
            Android,
            IOS
        }
        public virtual bool HasNotch(bool debug = false)
        {
            if (debug)
                return true;
            return false;
        }
        /// <summary>
        /// opengl数据,left bottom right top
        /// </summary>
        /// <param name="debug"></param>
        /// <returns></returns>
        public virtual int[] GetNotchSize(NotchDebugType debug = NotchDebugType.None, ScreenOrientation screenOrientation = ScreenOrientation.LandscapeLeft)
        {
            int[] n;
            switch (debug)
            {
                case NotchDebugType.None:
                    n = new int[4] { 0, 0, 0, 0 };
                    break;
                case NotchDebugType.Android:
                    if (screenOrientation == ScreenOrientation.LandscapeLeft || screenOrientation == ScreenOrientation.Landscape)
                        n = new int[4] { 0, Screen.height / 2 + 150, 80, Screen.height / 2 - 150 };
                    else
                        n = new int[4] { Screen.width - 80, Screen.height / 2 + 150, Screen.width, Screen.height / 2 - 150 };
                    break;
                case NotchDebugType.IOS:
                    if (screenOrientation == ScreenOrientation.LandscapeLeft || screenOrientation == ScreenOrientation.Landscape)
                        n = new int[4] { 0, Screen.height, 132, 0 };
                    else
                        n = new int[4] { Screen.width - 132, Screen.height, Screen.width, 0 };
                    break;
                default:
                    n = new int[4] { 0, 0, 0, 0 };
                    break;
            }
            return n;
        }
        public virtual int GetSystemVersion()
        {
            return 0;
        }
        //public virtual void SaveImage2Album(byte[] bytes, string suc= "截图成功", string fail="截图失败")
        //{

        //}

        #region 通知相关
        public virtual void ResetBadge() { }
        public virtual void SetNotification(int nid, string content, string title, int hour) { }
        public virtual void SetNotificationWeekly(int nid, string content, string title, int weekday, int hour, int minute) { }
        public virtual void RemoveNotification(int nid) { }
        public virtual void RemoveAllNotification() { }
        #endregion

        #endregion

        #region iOSAppstore

        public virtual void GetNewVersionInAppstore() { }

        public virtual string GetIOSAppstoreSmallGameLoadSceneName() { return "select"; }

        public virtual bool IsIOSAppstoreLoadSmallGame() { return false; }

        #endregion
    }
}
