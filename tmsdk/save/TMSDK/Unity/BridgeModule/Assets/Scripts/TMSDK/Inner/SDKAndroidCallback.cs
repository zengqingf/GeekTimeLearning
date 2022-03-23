using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMSDKClient
{
    class SDKAndroidCallback : AndroidJavaProxy, ISDKCallback
    {
        private static string ANDROID_UNITY_PROXY_CALLBACK_INTERFACE = "com.tm.sdk.bridge.call.ICallback"; 
        public SDKAndroidCallback() : base(ANDROID_UNITY_PROXY_CALLBACK_INTERFACE) {}

        public void Callback(string json)
        {
            SDKUtility.SDKDebugFormat(DebugType.NormalNoMask, "Callback json: {0}", json);
            SDKCallbackManager.GetInstance().ExecCmd(json);
        }
    }
}
