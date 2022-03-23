using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDKClient;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.IO;
using System.Linq;

public class ChannelSDKTest : MonoBehaviour
{
    /// <summary>
    /// 相当于正式工程的PlayInfo中的openid, token, 和 sdkExt
    /// </summary>
    public static SDKUserInfo sdkUserInfo;
    public static string logText;

    [Header("测试渠道APPID")]
    public string AppId = "316";
    [Header("测试服务器信息")]
    public string ServerId = "100";
    public string ServerName = "测试服";
    [Header("测试登录验证服务器地址")]
    public string LoginVerifyAddress = "192.168.2.237:50000";
    [Header("测试角色信息")]
    public string RoleId = "111222333";
    public string RoleName = "测试角色";
    [Header("测试支付回调地址")]
    public string PayCallbackAddress = "101.37.173.236:58016";
    [Header("测试支付商品信息")]
    public string TestPrice = "0.01";
    public string MallType = "0";
    public string ProductId = "1";
    public string ProductName = "测试点券";
    public string ProductShortName = "test_point";
    public string ProductDesc = "测试点券描述信息";
    string testiap = ".6,.30,.60,.198,.328,.648,.M30,.98,.488,.LB1,.LB3,.LB6";

    float timer = 0f;

    void Start()
    {
#if UNITY_IPHONE || UNITY_IOS

        SDKInterface.Instance.IOSInfoExtra.checkUpdate = true;
        SDKInterface.Instance.IOSInfoExtra.isLoadU3dSmallGame = false;
        SDKInterface.Instance.SetIosIap("6");
        SDKInterface.Instance.SetIosIap("30");
        SDKInterface.Instance.SetIosIap("60");
#endif
        PluginManager.Instance.Init(); //有问题：这里初始化了两次

    }
    
    void OnGUI()
    {
        GUIStyle bb = new GUIStyle();
        bb.fontSize = 80;
        bb.richText = true;
        // bb.stretchWidth = true;
        bb.stretchHeight = true;
        bb.wordWrap = true;
        GUI.Label(new Rect(0, 0, 100, 200), "输出:", bb);
        GUI.Label(new Rect(100, 0, 1000, 200), logText, bb);

        if (GUI.Button(new Rect(0, 400, 200, 200), "登陆"))
        {
            PluginManager.Instance.OpenSDKLogin();
        }
        else if (GUI.Button(new Rect(250, 400, 200, 200), "验证"))
        {
            _VerifyLogin();
        }

        else if (GUI.Button(new Rect(500, 400, 200, 200), "充值"))
        {
            _Pay();
        }

        else if (GUI.Button(new Rect(0, 650, 100, 100), "打开手机绑定"))
        {
            PluginManager.Instance.OpenSDKMobileBind();
        }
        else if (GUI.Button(new Rect(200, 650, 100, 100), "检查是否绑定成功"))
        {
            PluginManager.Instance.CheckSDKMobileBind();
        }
    }

    void _Pay()
    {
        SDKPayInfo payInfo = new SDKPayInfo();
        payInfo.requestId = _GenerateRequestPayID(RoleId, ServerId);
        payInfo.price = TestPrice;
        payInfo.mallType = MallType;
        payInfo.productId = ProductId;
        payInfo.productName = ProductName;
        payInfo.productShortName = ProductShortName;
        payInfo.productDesc = ProductDesc;

        payInfo.serverId = ServerId;
        payInfo.serverName = ServerName;
        payInfo.roleId = RoleId;
        payInfo.roleName = RoleName;

        string payCbUrlFormat = PluginManager.Instance.GetSDKPayCallbackUrlFormat();
        payInfo.payCallbackUrl = string.Format(payCbUrlFormat, PayCallbackAddress);

        string cookie = "";
        if (sdkUserInfo != null)
        {
            cookie = string.Format("{0},{1},{2},{3},{4}", payInfo.mallType, payInfo.productId, RoleId, sdkUserInfo.openUid, ServerId);
        }
        else
        {
            Debug.LogError("### Test Pay() cookie is null");
        }
        //payInfo.extra = string.Format("{0},{1},{2}", payInfo.mallType, payInfo.productId, RoleId);
        payInfo.extra = cookie;

        PluginManager.Instance.PaySDK(payInfo);
    }

    void _VerifyLogin()
    {
        SDKLoginVerifyUrl lvUrl = new SDKLoginVerifyUrl();
        lvUrl.serverUrl = LoginVerifyAddress;
        lvUrl.serverId = ServerId;
        lvUrl.channelParam = PluginManager.Instance.GetSDKChannelParam();

        //需要额外签名的时候        
        string timeStamp = GetSystemTimeStamp();

        string sign = null;
        var sdkDeviceInfo = PluginManager.Instance.GetSDKDeviceInfo();
        if (sdkDeviceInfo != null && sdkUserInfo != null)
        {
            sign = GetSignStr(sdkDeviceInfo.deviceId, lvUrl.serverId, sdkUserInfo.openUid, lvUrl.channelParam, timeStamp);
        }
        string param = "";
        int isAccountValidateRet = -1;
        byte[] hashval = new byte[20];

        string url = PluginManager.Instance.LoginVerifyUrl(lvUrl);

        logText = "开始登录验证,url = " + url;

        UnityEngine.Debug.LogErrorFormat("[开始点击] 验证 {0}", url);

        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

        UnityEngine.Debug.LogErrorFormat("[开始点击] 创建WebReq {0}", url);

        req.BeginGetResponse((IAsyncResult ar) => {
            UnityEngine.Debug.LogErrorFormat("[开始点击] WebReq收到x消息 {0}", url);
            logText = "收到消息";
            string resText = null;
            try
            {
                HttpWebResponse res = (HttpWebResponse)req.EndGetResponse(ar);
                Stream stream = res.GetResponseStream();

                byte[] resBytes = new byte[1024];
                stream.Read(resBytes, 0, resBytes.Length);
                resText = StringHelper.BytesToString(resBytes);
            }
            catch
            {
                string str = string.Format("Get Server List From {0} failed", url);
                logText = str;
                Logger.LogErrorFormat(str);
                isAccountValidateRet = 1;
                return;
            }


            if (resText == null)
            {
                isAccountValidateRet = 1;
                return;
            }

            try
            {
                Hashtable ret = (Hashtable)XUPorterJSON.MiniJSON.jsonDecode(resText);

                isAccountValidateRet = Int32.Parse(ret["result"].ToString());
                Debug.LogErrorFormat("isAccountValidateRet: {0}", isAccountValidateRet);

                param = ret["params"].ToString();
                Debug.LogErrorFormat("Recv account validate return: {0}", param);

                string hashHex = ret["hashval"].ToString();
                for (int i = 0; i < hashHex.Count() / 2; i++)
                {
                    hashval[i] = Convert.ToByte(hashHex.Substring(i * 2, 2), 16);
                }

                //TODO SDK手机绑定 返回Json中取到当前角色绑定的手机号

#if UNITY_ANDROID
                sdkUserInfo.openUid = ret["openid"].ToString();
                if (PluginManager.GetInstance().NeedPayToken())
                {                    
                    sdkUserInfo.token = ret["token"].ToString();
                    PluginManager.GetInstance().SetSpecialPayUserInfo(sdkUserInfo.openUid, sdkUserInfo.token);
                    Debug.LogErrorFormat("设置特殊的支付时 帐号信息！！！");
                }                
#endif

                if (isAccountValidateRet == 0)
                {
                    logText = "验证成功";
                    Debug.LogErrorFormat("验证成功");
                }
                else
                {
                    logText = "验证失败，错误码：" + isAccountValidateRet;
                    Debug.LogErrorFormat("验证失败，错误码：{0}", isAccountValidateRet);
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Decode Http response failed, {0}", e.ToString());
            }

        }, null);
    }


    /// <summary>
    /// 订单号生成 (字母数字)
    /// </summary>
    /// <param name="roleId">角色id</param>
    /// <param name="serverId">服务器id</param>    
    /// <param name="timeStamp">时间戳</param>
    /// <param name="ext">其他</param>
    /// <param name="limitLenght">限制字符数</param>
    /// <returns></returns>
    private string _GenerateRequestPayID(string roleId, string serverId, string timeStamp = "", string ext = "", int limitLenght = -1)
    {
        string reqId = "";
        if (timeStamp == "")
        {
            //timeStamp = Tenmove.Runtime.Client.TimeManager.Instance.GetServerTime().ToString();
            timeStamp = TransLocalNowDateToStamp().ToString();
        }
        reqId = string.Format("{0}{1}{2}{3}", roleId, serverId, timeStamp, ext);
        if (limitLenght > 0 && string.IsNullOrEmpty(reqId))
        {
            reqId = reqId.Substring(0, limitLenght);
        }
        return reqId;
    }

    private uint TransLocalNowDateToStamp()
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        uint timeStamp = (uint)(DateTime.Now - startTime).TotalSeconds;
        return timeStamp;
    }

    private string GetSystemTimeStamp()
    {
        return TransNowDateToStamp().ToString();
    }

    private string GetSignStr(string deviceId, string id, string openid, string platform, string timestamp)
    {
        string sign = string.Format("{0}did={1}&id={2}&openid={3}&platform={4}&timestamp={5}",
                                    AppId, deviceId, id, openid, platform, timestamp);
        sign = MD5CreateNormal(sign);
        return sign;
    }

    public DateTime TransTimeStampToDate(UInt32 timeStamp)
    {
        System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
        return dt;
    }

    public string TransTimeStampToStr(UInt32 timeStamp)
    {
        System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
        // return string.Format("{0}年{1}月{2}日 {3:HH:mm}", dt.Year, dt.Month, dt.Day, dt);
        return string.Format("{0}月{1}日{2:HH:mm}", dt.Month, dt.Day, dt);
    }

    public uint TransNowDateToStamp()
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        uint timeStamp = (uint)(DateTime.Now - startTime).TotalSeconds;
        return timeStamp;
    }

    public uint TransTodayZeroDateToStamp()
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        System.DateTime todayZeroTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
        uint timeStamp = (uint)(todayZeroTime - startTime).TotalSeconds;
        return timeStamp;
    }

    private string MD5Create(string STR)
    {
        string res = "";
        if (string.IsNullOrEmpty(STR))
            return res;
        MD5 md5 = MD5.Create();
        byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(STR));
        for (int i = 0; i < s.Length; i++)
        {
            res = res + s[i].ToString();
        }
        return res;
    }

    private string MD5CreateNormal(string STR)
    {
        string res = "";
        if (string.IsNullOrEmpty(STR))
            return res;
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        res = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(STR)));
        res = res.Replace("-", "");
        return res.ToLowerInvariant();
    }
}
