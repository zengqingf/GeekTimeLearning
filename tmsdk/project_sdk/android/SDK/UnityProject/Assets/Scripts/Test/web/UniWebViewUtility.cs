using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

namespace SDKClient
{
    #region 版本检查
    //public class VersionCheak
    //{
    //    public static bool IsLowVertion_Android(string minVertion)
    //    {
    //        string s = SystemInfo.operatingSystem;
    //        int index = s.IndexOf("API-") + 4;
    //        StringBuilder sb = new StringBuilder();
    //        while (char.IsNumber(s[index]))
    //        {
    //            sb.Append(s[index]);

    //        }
    //        string ver = sb.ToString();
    //        if (Convert.ToInt32(ver) >= Convert.ToInt32(minVertion))
    //        {
    //            return false;
    //        }
    //        return true;
    //    }
    //    public static bool IsLowVertion_IOS(string minVertion)
    //    {
    //        string s = SystemInfo.operatingSystem;
    //        int index = 0;
    //        while (!char.IsNumber(s[index]))
    //        {
    //            index++;
    //        }

    //        StringBuilder sb = new StringBuilder();
    //        while (s[index] != ' ')
    //        {
    //            sb.Append(s[index]);

    //        }
    //        string ver = sb.ToString();
    //        if (Convert.ToDecimal(ver) >= Convert.ToDecimal(minVertion))
    //        {
    //            return false;
    //        }
    //        return true;
    //    }

    //}
    #endregion

    [RequireComponent(typeof(UniWebView))]
    public class UniWebViewUtility : MonoBehaviour
    {
        public string LoadingNote = "正在努力加载中...请稍候...";
        public Image webViewNormal;
        [Header("已弃用")]
        [Obsolete]
        public Image webViewChanged;
        [Header("已弃用")]
        [Obsolete]
        public GameObject loadingTextGo;

        private UniWebView mUniWebView;
        //private Rect contentRect;
        //private Rect contentRect2;

        private Vector3 keyboardShowOffset;
        private Vector2 baseSize;
        private Vector2 deltaSize;
        private bool init_KeyboardEvent = false;

        private bool needClearCache = false;
        private string currCacheUrl = "";
        private string errorCacheUrl = "";
        private bool beloadErrorUrl = false;

        private List<string> urlSchemeList = new List<string>();

        bool showLog = false;
        bool updateFlag = false;

        void Start()
        {
            //正式  关闭日志
            UniWebViewLogger.Instance.LogLevel = UniWebViewLogger.Level.Debug;
            showLog = true;
        }

        UniWebView CreateWebView(bool showLoading)
        {
            var webView = this.GetComponent<UniWebView>();
            //if (webViewNormal)
            //{
            //    contentRect = webViewNormal.rectTransform.rect;
            //}
            //if (webViewChanged)
            //{
            //    contentRect2 = webViewChanged.rectTransform.rect;
            //}
            if (webView == null)
            {
                webView = webViewNormal.gameObject.AddComponent<UniWebView>();
            }
            if (webView != null)
            {
                //webView.Frame = contentRect;
                webView.ReferenceRectTransform = webViewNormal.rectTransform;

                webView.SetShowSpinnerWhileLoading(showLoading);
                webView.SetSpinnerText(LoadingNote);

                webView.SetVerticalScrollBarEnabled(true);
                webView.SetHorizontalScrollBarEnabled(false);

                webView.BackgroundColor = new Color(1, 1, 1, 0);
            }
            return webView;
        }
        /// <summary>
        /// 判断当前版本是否低于该SDK需求的最低版本
        /// </summary>
        /// <returns></returns>
        public bool IsLowVersion()
        {

#if UNITY_ANDROID && !UNITY_EDITOR
        return SDKInterface.Instance.TryGetCurrVersionAPI() < 21;
#endif
#if (UNITY_IOS || UNITY_PHONE) && !UNITY_EDITOR
        return SDKInterface.Instance.TryGetCurrVersionAPI() < 9;
#endif
            return true;
        }
        public void InitWebView(bool needClearCache = false, bool showLoading = true)
        {
            this.needClearCache = needClearCache;
            if (mUniWebView == null)
            {
                mUniWebView = CreateWebView(showLoading);
                AddWebViewEventHandler();
            }
        }
        public void InitWebView(string url, bool showLoading = true, bool autoShow = true, bool needClearCache = false)
        {
            InitWebView(needClearCache, showLoading);
            LoadUrl(url);
            if (autoShow)
                ShowWebView();

        }
        public void UnInitWebView()
        {
            if (mUniWebView != null)
            {
                RemoveWebViewEventHandler();
                //RemoveAllUrlSchemes();
                HideWebView();
                TryClearWebCache();

                PageStarted = null;
                PageFinished = null;
                PageErrorReceived = null;

                mUniWebView = null;
            }
        }
        /// <summary>
        /// **仅限IOS**允许调用文件url，默认false
        /// </summary>
        /// <param name="flag"></param>
        public void SetAllowFileAccessFromFileURLs(bool flag)
        {
            if (mUniWebView != null)
            {
                mUniWebView.SetAllowFileAccessFromFileURLs(flag);
            }
        }
        /// <summary>
        /// 读取url
        /// </summary>
        /// <param name="url">若是本地url,请把与网页相关的文件放入StreamingAsset文件夹下的同一个目录，然后调用StreamingAssetURLForPath生成url</param>
        /// <param name="skipEncoding">是否忽略url传值</param>
        /// <param name="readAccessURL">**仅限IOS9，调用前需要SetAllowFileAccessFromFileURLs(true)**仅在第一个参数url为文件url时使用，通过IOS的webkit中的loadFileURL调用，其值为文件url所在文件夹，使其允许读取</param>
        public void LoadUrl(string url, bool skipEncoding = false, string readAccessURL = null)
        {
            if (mUniWebView != null)
            {
                mUniWebView.Load(url);
            }
        }
        /// <summary>
        /// 读取html
        /// </summary>
        /// <param name="htmlString"></param>
        /// <param name="baseUrl">用于解析html的url</param>
        /// <param name="skipEncoding">是否忽略url传值</param>
        public void LoadHTMLString(string htmlString, string baseUrl, bool skipEncoding = false)
        {
            if (mUniWebView != null)
            {
                mUniWebView.LoadHTMLString(htmlString, baseUrl, skipEncoding);

            }
        }
        public void ReLoadUrl()
        {
            if (mUniWebView != null)
            {
                mUniWebView.Reload();
            }
        }

        public void ShowWebView()
        {
            if (mUniWebView != null)
            {
                mUniWebView.Show();
            }
        }

        public void ShowWebViewFade()
        {
            if (mUniWebView != null)
            {
                mUniWebView.Show(true, UniWebViewTransitionEdge.None);
            }

        }

        public void HideWebView()
        {
            if (mUniWebView != null)
            {
                mUniWebView.Stop();
                mUniWebView.Hide();
            }
        }

        public void HideWebViewFade()
        {
            if (mUniWebView != null)
            {
                mUniWebView.Stop();
                mUniWebView.Hide(true, UniWebViewTransitionEdge.None);
            }
        }
        public void GoBackWebView()
        {
            if (mUniWebView != null)
            {
                if (mUniWebView.CanGoBack)
                {
                    mUniWebView.GoBack();
                }
            }
        }

        public bool CanWebViewGoBack()
        {
            if (mUniWebView != null)
            {
                return mUniWebView.CanGoBack;
            }
            return false;
        }
        public void GoForwardWebView()
        {
            if (mUniWebView != null)
            {
                if (mUniWebView.CanGoForward)
                {
                    mUniWebView.GoForward();
                }
            }
        }
        public bool CanWebViewGoForward()
        {
            if (mUniWebView != null)
            {
                return mUniWebView.CanGoForward;
            }
            return false;
        }
        [Obsolete]
        public void TweenMoveTo(float durationTime)
        {
            if (mUniWebView != null)
            {
                mUniWebView.AnimateTo(mUniWebView.Frame, durationTime);
            }
        }

        private T TryGetComponentInParents<T>(Transform t) where T : Component
        {
            var parent = t.parent;
            if (parent == null)
            {
                return null;
            }
            var c = parent.GetComponent<T>();
            if (c == null)
            {
                return TryGetComponentInParents<T>(parent);
            }
            return c;
        }

        public void ResetWebViewShow(bool toOriginal, int keyboardSize)
        {
            if (webViewNormal == null)
                return;

            if (init_KeyboardEvent == false)
            {
                var canvas = TryGetComponentInParents<Canvas>(transform);
                var rect = webViewNormal.rectTransform.rect;

                int keyboardSizeY = keyboardSize;
                baseSize = webViewNormal.rectTransform.sizeDelta;
                deltaSize = baseSize + new Vector2(0, (Screen.height - keyboardSizeY) - rect.height);
                webViewNormal.rectTransform.sizeDelta = deltaSize;
                float bottomOffsetY = canvas.transform.InverseTransformPoint(webViewNormal.transform.parent.TransformPoint(webViewNormal.transform.localPosition)).y + Screen.height * 0.5f - webViewNormal.rectTransform.rect.height * webViewNormal.rectTransform.pivot.y;
                keyboardShowOffset = new Vector3(0, keyboardSizeY - bottomOffsetY);
                init_KeyboardEvent = true;
            }

            if (mUniWebView != null)
            {
                if (toOriginal)
                {
                    webViewNormal.rectTransform.sizeDelta = baseSize;
                    webViewNormal.rectTransform.localPosition -= keyboardShowOffset;
                }
                else
                {
                    webViewNormal.rectTransform.sizeDelta = deltaSize;
                    webViewNormal.rectTransform.localPosition += keyboardShowOffset;
                }

                //Update and set current frame of web view to match the setting.
                //This is useful if the referenceRectTransform is changed and you need to sync the frame change to the web view.
                //This method follows the frame determining rules.
                mUniWebView.UpdateFrame();
            }
        }

        public void AddJS(string js, System.Action<UniWebViewNativeResultPayload> cb = null)
        {
            if (mUniWebView != null)
            {
                mUniWebView.AddJavaScript(js, cb);
            }
        }

        public void ExcuteJS(string js, System.Action<UniWebViewNativeResultPayload> cb = null)
        {
            if (mUniWebView != null)
            {
                mUniWebView.EvaluateJavaScript(js, cb);
            }
        }
        /// <summary>
        /// 当webview 位置、缩放、旋转改变时必须调用
        /// </summary>
        public void UpdateFrame()
        {
            if (mUniWebView != null)
            {
                mUniWebView.UpdateFrame();
            }
        }
        public void OnUpdate(float deltaTime)
        {
            if (PageViewUpdate != null)
            {
                if (updateFlag)
                {
                    PageViewUpdate();
                    updateFlag = false;
                }
            }
        }
        /// <summary>
        /// 获取StreamingAsset目录下的路径
        /// </summary>
        /// <param name="localPath"></param>
        /// <returns></returns>
        public string StreamingAssetURLForPath(string localPath)
        {
            return UniWebViewHelper.StreamingAssetURLForPath(localPath);

        }


        void TryClearWebCache()
        {
            if (needClearCache && mUniWebView != null)
            {
                mUniWebView.CleanCache();
            }
        }

        void AddUrlSchemes(string scheme)
        {
            if (mUniWebView != null && !string.IsNullOrEmpty(scheme))
            {
                mUniWebView.AddUrlScheme(scheme);

                if (urlSchemeList == null)
                    return;
                if (!urlSchemeList.Contains(scheme))
                {
                    urlSchemeList.Add(scheme);
                }
            }
        }

        void RemoveAllUrlSchemes()
        {
            if (urlSchemeList == null)
                return;
            if (mUniWebView != null)
            {
                for (int i = 0; i < urlSchemeList.Count; i++)
                {
                    mUniWebView.RemoveUrlScheme(urlSchemeList[i]);
                }
            }
            urlSchemeList = null;
        }

        bool HasUrlScheme(string urlScheme)
        {
            if (urlSchemeList == null || urlSchemeList.Count == 0)
                return false;
            if (urlSchemeList.Contains(urlScheme))
            {
                return true;
            }
            return false;
        }

        void OnKeyboardShowOut(string res)
        {
#if (UNITY_IOS || UNITY_PHONE) && !UNITY_EDITOR
        return;
#endif
            bool isKeyBoardShow = false;
            if (res.Equals("0"))
            {
                isKeyBoardShow = false;
            }
            else
            {
                isKeyBoardShow = true;
            }
            if (isKeyBoardShow)
            {
                ResetWebViewShow(false, Convert.ToInt32(res));
            }
            else
            {
                ResetWebViewShow(true, 0);
            }
        }

        //请在这里处理游戏UI刷新
        public delegate void OnPageViewUpdate();
        [HideInInspector]
        public OnPageViewUpdate PageViewUpdate;

        //***下面的回调只能在内部使用 不要在这些回调里处理游戏UI***
        public delegate void OnPageStarted();
        private OnPageStarted PageStarted;
        public delegate void OnPageFinished();
        private OnPageFinished PageFinished;
        public delegate void OnPageErrorReceived();
        private OnPageErrorReceived PageErrorReceived;
        public delegate void OnPageLoadReceiveMsg(UniWebViewMessage msg);
        [HideInInspector]
        public OnPageLoadReceiveMsg PageLoadReveiveMsg;

        void AddWebViewEventHandler()
        {
            if (mUniWebView != null)
            {
                //当前版本 UniWebView 回调中不处理具体逻辑
                mUniWebView.OnPageStarted += OnPageStart;

                mUniWebView.OnPageFinished += OnPageFinish;

                mUniWebView.OnPageErrorReceived += OnPageError;

                mUniWebView.OnMessageReceived += OnPageMsgReceived;

                //返回键不会关闭网页
                mUniWebView.OnShouldClose += (view) =>
                {
                    return false;
                };
                //关闭安卓返回键
#if UNITY_ANDROID && !UNITY_EDITOR
                SetBackButtonEnabled(false);
#endif
#if (UNITY_IOS || UNITY_PHONE) && !UNITY_EDITOR
            mUniWebView.OnWebContentProcessTerminated += (view) =>
            {
                //清理可释放的内存，重新加载
                mUniWebView.CleanCache();
                ReLoad();
            };
#endif

                PluginManager.GetInstance().AddKeyboardShowListener(OnKeyboardShowOut);

            }
        }
        public void ReLoad()
        {
            if (mUniWebView != null)
            {
                mUniWebView.Reload();
            }
        }
        void RemoveWebViewEventHandler()
        {
            if (mUniWebView != null)
            {
                //当前版本 UniWebView 回调中不处理具体逻辑
                mUniWebView.OnPageStarted -= OnPageStart;

                mUniWebView.OnPageFinished -= OnPageFinish;

                mUniWebView.OnPageErrorReceived -= OnPageError;

                mUniWebView.OnMessageReceived -= OnPageMsgReceived;

                PluginManager.GetInstance().RemoveKeyboardShowListener();


            }
        }


        //void OnLoadPageStarted(GameClient.UIEvent uiEvent)
        //{
        //    if (PageStarted != null)
        //    {
        //        PageStarted();


        //    }
        //}

        //void OnLoadPageFinished(GameClient.UIEvent uiEvent)
        //{
        //    if (PageFinished != null)
        //    {
        //        PageFinished();
        //    }
        //}

        //void OnLoadPageError(GameClient.UIEvent uiEvent)
        //{
        //    if (PageErrorReceived != null)
        //    {
        //        PageErrorReceived();
        //    }
        //}


        void OnPageMsgReceived(UniWebView view, UniWebViewMessage message)
        {
            WebViewLog("OnPageMsgReceived : " + message.RawMessage);
            if (message.Equals(null))
                return;

            WebViewLog("OnPageMsgReceived rec scheme = " + message.Scheme);
            WebViewLog("OnPageMsgReceived rec path = " + message.Path);
            WebViewLog("OnPageMsgReceived rec rawMessage = " + message.RawMessage);

            if (PageLoadReveiveMsg != null)
            {
                PageLoadReveiveMsg(message);
            }
        }

        void OnPageStart(UniWebView view, string loadingUrl)
        {
            //不要尝试在sdk回调里处理主线程UI逻辑 ！！！

            WebViewLog(" OnPageStart Loading start url = " + loadingUrl);
        }
        void OnPageFinish(UniWebView view, int statusCode, string url)
        {
            //不要尝试在sdk回调里处理主线程UI逻辑 ！！！

            if (statusCode == 200)
            {
                updateFlag = true;
            }

            WebViewLog("OnPageFinished error url : " + errorCacheUrl);

            WebViewLog("OnPageFinished success = " + statusCode + " url = " + url);
        }
        void OnPageError(UniWebView view, int errorCode, string errorMsg)
        {

            WebViewLog("OnPageErrorReceived error url : " + errorCacheUrl);
            WebViewLog("OnPageErrorReceived errorCode = " + errorCode + " errorMsg = " + errorMsg);
        }

        void SetLoadingTextActive(bool active)
        {

        }

        void WebViewLog(string msg)
        {
            if (showLog)
            {
                Debug.Log(msg);
            }
        }
        #region webview状态设置
        /// <summary>
        /// 网页中的链接是否由外部浏览器打开，默认false
        /// </summary>
        /// <param name="flag"></param>
        public void OpenLinksInExternalBrowser(bool flag)
        {

            if (mUniWebView == null)
            {
                return;
            }
            mUniWebView.SetOpenLinksInExternalBrowser(flag);
        }
        /// <summary>
        /// 网页视频是否自动播放，默认false
        /// </summary>
        /// <param name="flag"></param>
        public void SetAllowAutoPlay(bool flag)
        {

            UniWebView.SetAllowAutoPlay(flag);
        }
        /// <summary>
        /// **仅限IOS**是否在当前页面播放视频而不是打开一个新的全屏页面，默认false
        /// </summary>
        /// <param name="flag"></param>
        public void SetAllowInlinePlay(bool flag)
        {

            UniWebView.SetAllowInlinePlay(flag);
        }
        /// <summary>
        /// 长按是否弹出长按菜单，默认true
        /// </summary>
        /// <param name="enabled"></param>
        public void SetCalloutEnabled(bool enabled)
        {
            if (mUniWebView == null)
            {
                return;
            }
            mUniWebView.SetCalloutEnabled(enabled);

        }
        /// <summary>
        /// **仅限IOS11**是否允许拖拽交互,默认true
        /// </summary>
        /// <param name="enabled"></param>
        public void SetDragInteractionEnabled(bool enabled)
        {
            if (mUniWebView == null)
            {
                return;
            }
            mUniWebView.SetDragInteractionEnabled(enabled);
        }
        /// <summary>
        /// 是否显示水平滚动条，webview默认为true，接口初始化为false
        /// </summary>
        /// <param name="enabled"></param>
        public void SetHorizontalScrollBarEnabled(bool enabled)
        {
            if (mUniWebView == null)
            {
                return;
            }
            mUniWebView.SetHorizontalScrollBarEnabled(enabled);

        }
        /// <summary>
        /// 是否显示垂直滚动条，默认为true
        /// </summary>
        /// <param name="enabled"></param>
        public void SetVerticalScrollBarEnabled(bool enabled)
        {
            if (mUniWebView == null)
            {
                return;
            }
            mUniWebView.SetVerticalScrollBarEnabled(enabled);

        }
        /// <summary>
        /// 是否允许双指缩放页面，默认为false
        /// </summary>
        /// <param name="enabled"></param>
        public void SetZoomEnabled(bool enabled)
        {
            if (mUniWebView == null)
            {
                return;
            }
            mUniWebView.SetZoomEnabled(enabled);

        }
        /// <summary>
        /// **仅限Android**是否允许安卓自带虚拟按键中的返回键的调用，webview默认为true，接口初始化为false
        /// </summary>
        /// <param name="enabled"></param>
        public void SetBackButtonEnabled(bool enabled)
        {
            if (mUniWebView == null)
            {
                return;
            }
            mUniWebView.SetBackButtonEnabled(enabled);

        }
        /// <summary>
        /// 设置能否通过unity调用js，默认为true，关闭可提高性能
        /// </summary>
        /// <param name="enabled"></param>
        public void SetJavaScriptEnabled(bool enabled)
        {
            UniWebView.SetJavaScriptEnabled(enabled);
        }
        /// <summary>
        /// **仅限Android和Mac**是否允许网页的日志输出，默认false
        /// </summary>
        /// <param name="enabled"></param>
        public void SetWebContentsDebuggingEnabled(bool enabled)
        {
            UniWebView.SetWebContentsDebuggingEnabled(enabled);
        }
        #endregion
        /// <summary>
        /// 锁定当前页面
        /// </summary>
        public void LockPage()
        {
            if (mUniWebView == null)
            {
                return;
            }
            mUniWebView.AddUrlScheme("http");
            mUniWebView.AddUrlScheme("https");
        }
        /// <summary>
        /// 解锁当前页面
        /// </summary>
        public void UnLockPage()
        {
            if (mUniWebView == null)
            {
                return;
            }
            mUniWebView.RemoveUrlScheme("http");
            mUniWebView.RemoveUrlScheme("https");

        }
    }

}
