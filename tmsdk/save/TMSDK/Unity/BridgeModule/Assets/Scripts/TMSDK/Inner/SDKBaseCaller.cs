using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMSDKClient
{
    public abstract class SDKBaseCaller : ISDKCall
    {
        public abstract SDKCallResult<T> Call<T>(SDKCallInfo callInfo);
    }
}
