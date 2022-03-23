using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Global : ScriptableObject
{
    //public SDKChannel sdkChannel = SDKChannel.None;
    public string sdkChannelType = "NONE";
}


public class GlobalSetting
{
    public const string GLOBAL_PATH = "Resources/Global";
    private static Global global = null;
    public static Global mGlobal
    {
        get {
            if (null == global)
            {
                global = Resources.Load<Global>(PathUtil.EraseExtension(GLOBAL_PATH.Substring(10)));
            }
            return global;
        }
        set {

            global = value;
        }
    }
}