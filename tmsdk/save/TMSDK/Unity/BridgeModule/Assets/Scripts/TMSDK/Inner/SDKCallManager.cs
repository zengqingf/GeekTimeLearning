using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMSDKClient
{
    public class SDKCallManager : Singleton<SDKCallManager>
    {
        SDKBaseCaller sdkCaller = null;

        public override void Init()
        {
#if UNITY_EDITOR
            sdkCaller = new SDKDefaultCaller();
#elif UNITY_ANDROID
            sdkCaller = new SDKAndroidCaller();
#elif UNITY_IOS || UNITY_IPHONE
            sdkCaller = new SDKIOSCaller();
#else
            sdkCaller = new SDKDefaultCaller();
#endif
        }

        public override void UnInit()
        {
            sdkCaller = null;
        }

        public SDKCallResult<T> Call<T>(SDKCallInfo callInfo)
        {
            if (sdkCaller != null) {
                return sdkCaller.Call<T>(callInfo);
            }
            return null;
        }
    }
}
