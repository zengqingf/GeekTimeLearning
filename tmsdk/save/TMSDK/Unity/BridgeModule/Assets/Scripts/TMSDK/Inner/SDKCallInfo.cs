using System;
using System.Collections;
using System.Collections.Generic;

namespace TMSDKClient 
{
    [Serializable]
    public class SDKCallInfo
    {
        public string name;
        public object[] args;               //针对 object[] 目前暂时只支持 LitJson 解析
        public bool callback;
        public string callbackId;

        //TODO 检查key唯一
        //private string[] argKeys;
        private int argIndex;
        private Dictionary<string, int> argIndexMap;


        //注意：json转换 都需要默认构造函数
        public SDKCallInfo() { }

        public SDKCallInfo(int argCount = 0, bool needCallback = false)
        {
            name = "";
            args = new object[argCount];            
            callback = false;
            callbackId = "";

            argIndex = 0;
            argIndexMap = new Dictionary<string, int>(argCount);

            if (needCallback) {
                SetCallbackId();
            }
        }

        public override string ToString()
        {           
            return JsonLibUtil.ToJson(this);
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void AddArg<T>(string key, T value)
        {
            SDKCallArg<T> arg = new SDKCallArg<T>(key, value);
            argIndexMap.Add(key, argIndex);
            args[argIndex++] = arg;
        }

        public T GetArg<T>(string key)
        {
            if (argIndexMap == null || argIndexMap.Count <= 0)
            {
                return default(T);
            }
            if (argIndexMap.ContainsKey(key))
            {
                int index = argIndexMap[key];
                SDKCallArg<T> arg = (SDKCallArg<T>)args[index];
                if (arg == null) {
                    SDKUtility.SDKDebugFormat(DebugType.ErrorNoMask, "SDKCallInfo args, key {0}，convert to SDKCallArg failed!", key);
                    return default(T);
                }
                return arg.value;
            }
            return default(T);
        }

        public void SetIsCallback(bool isCallback)
        {
            this.callback = isCallback;
        }

        public void SetCallbackId(string callbackId)
        {
            this.callbackId = callbackId;
        }

        public void SetCallbackId()
        {
            string callbackId = GUIDGenerator.GetUUID();
            SetCallbackId(callbackId);
        }

        public void Clear()
        {
            //notice : for json , can not set to null !!!
            name = string.Empty;
            args = new object[0];
            argIndex = 0;
            callback = false;
            callbackId = string.Empty;
            argIndexMap.Clear();
        }
    }
}
