using UnityEngine;
using System;

namespace TMSDKClient
{
    public class SDKAndroidCaller : SDKBaseCaller
    {
        private AndroidJavaObject androidCallObject;

        public SDKAndroidCaller()
        {
            // AndroidJavaClass gameBaseActivity = new AndroidJavaClass("com.tm.sdk.unitybridge.activity.BaseActivity");
            // AndroidJavaObject currentActivity = gameBaseActivity.CallStatic<AndroidJavaObject>("getCurrentActivity");
            // AndroidJavaObject unityPlayer = gameBaseActivity.CallStatic<AndroidJavaObject>("getUnityPlayer");

            //测试测试！！！
            // AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            // AndroidJavaObject currentActivity_2 = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            // AndroidJavaObject unityPlayer_Hack = currentActivity_2.Get<AndroidJavaObject>("mUnityPlayer");

            SDKAndroidCallback callback = new SDKAndroidCallback();
            //androidCallObject = new AndroidJavaObject("com.tm.sdk.unitybridge.AndroidCaller", callback, "SDK");
            androidCallObject = new AndroidJavaObject("com.tm.sdk.bridge.app.BridgeUtils");

            SDKUtility.SDKDebugFormat(DebugType.NormalNoMask, "Call Android... {0}", androidCallObject == null ? "call object is null" : androidCallObject.ToString());
            androidCallObject.CallStatic("Init", callback);
            SDKUtility.SDKDebugFormat(DebugType.NormalNoMask, "Call Android... Init Method");
        }

        private void _AssertAndroidCallObjectNotNull()
        {
            if (null == androidCallObject)
            {
                throw new NullReferenceException("the object of 'com.tm.sdk.unitybridge.AndroidCaller' is null");
            }
        }

        public override SDKCallResult<T> Call<T>(SDKCallInfo callInfo)
        {
            _AssertAndroidCallObjectNotNull();
            string resJson = androidCallObject.CallStatic<string>("Call", callInfo.ToString());
            SDKUtility.SDKDebugFormat(DebugType.NormalNoMask, "Call Android... {0}", callInfo.ToString());
            var res = JsonLibUtil.ToObject<SDKCallResult<T>>(resJson);
            SDKUtility.SDKDebugFormat(DebugType.NormalNoMask, "CallResult Android... {0}", string.IsNullOrEmpty(resJson) ? "null" : resJson);
            SDKUtility.SDKDebugFormat(DebugType.NormalNoMask, "CallResult Android to json... {0}", res == null ? "null" : res.obj.ToString());
            return res;
        }
    }
}
