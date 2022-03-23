using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMSDKClient
{
    public enum JsonLibType
    {
        Default,
        LitJson
    }

    public static class JsonLibUtil
    {
//Notice: need add Marco "LitJson" to "Unity Scripting Define Symbols"
#if LitJson
        public static readonly JsonLibType LibType = JsonLibType.LitJson;
#else
        public static readonly JsonLibType LibType = JsonLibType.Default;
#endif

        public static string ToJson<T>(T obj) where T : class
        {
            if (obj == default(T))
            {
                return "";
            }
            switch(LibType)
            {               
                case JsonLibType.LitJson:
                    return LitJson.JsonMapper.ToJson(obj);
                default:
                #if UNITY_2018
                    return JsonUtility.ToJson(obj);           
                #endif 
            }
        }

        public static T ToObject<T>(string json) where T : class
        {
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }
            switch (LibType)
            {
                case JsonLibType.LitJson:
                    return LitJson.JsonMapper.ToObject<T>(json);
                default:
                #if UNITY_2018
                    return JsonUtility.FromJson<T>(json);
                #endif
            }
        }

    }
}
