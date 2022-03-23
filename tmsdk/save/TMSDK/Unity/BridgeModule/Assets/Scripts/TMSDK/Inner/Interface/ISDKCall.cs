using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TMSDKClient
{
    public interface ISDKCall
    {
        SDKCallResult<T> Call<T>(SDKCallInfo callInfo);
    }
}
