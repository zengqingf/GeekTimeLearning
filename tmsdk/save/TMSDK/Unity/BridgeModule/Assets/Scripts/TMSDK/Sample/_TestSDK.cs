using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMSDKClient;

public partial class _TestSDK : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /*
        SDKCallInfo callInfo = new SDKCallInfo(4);
        callInfo.SetName("testSDK");
        callInfo.SetIsCallback(true);
        callInfo.SetCallbackId(GUIDGenerator.GetUUID());
        callInfo.Add("test1", 1);
        callInfo.Add("test2", "2");
        callInfo.Add("test3", true);
        callInfo.Add("test4", 0.01);
        
        Debug.Log(callInfo.ToString());

        SDKAndroidCaller caller = new SDKAndroidCaller();
        caller.Call<object>(callInfo);
        */

        PluginManager.GetInstance().Call(PluginManager.CallType.Init);
        PluginManager.GetInstance().Call(PluginManager.CallType.Login);
        PluginManager.GetInstance().Call(PluginManager.CallType.TestCallResult);
        PluginManager.GetInstance().Call(PluginManager.CallType.InitBugly);
        PluginManager.GetInstance().Call(PluginManager.CallType.SetBuglyUserId);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


public partial class _TestSDK
{
    //TODO
    //分类中调用 Bridge 相关接口
}