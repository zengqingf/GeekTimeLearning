using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMSDKClient
{
    public class SDKDefaultCaller : SDKBaseCaller
    {
        public override SDKCallResult<T> Call<T>(SDKCallInfo callInfo)
        {
            return null;
        }
    }
}