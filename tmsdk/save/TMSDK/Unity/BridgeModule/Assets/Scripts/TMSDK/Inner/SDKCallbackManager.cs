using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMSDKClient
{
    public delegate void SDKCallbackHandler(object[] args, string callbackId);

    public class SDKCallbackManager : Singleton<SDKCallbackManager>
    {
        private Dictionary<string, SDKCallbackHandler> commands;

        public override void Init()
        {
            commands = new Dictionary<string, SDKCallbackHandler>();
        }

        public override void UnInit()
        {
            commands.Clear();
        }

        public void AddCmd(string key, SDKCallbackHandler handler)
        {
            commands[key] = handler;
        }

        public void ExecCmd(string json)
        {
            SDKCallInfo callInfo = _Deserialize(json);
            if (callInfo == null) {
                return;
            }
            if (commands.ContainsKey(callInfo.name))
            {
                SDKCallbackHandler handler = commands[callInfo.name];
                if (handler != null)
                {
                    handler(callInfo.args, callInfo.callbackId);
                    if (callInfo.callback) {
                        commands.Remove(callInfo.name);
                    }
                }
            }
        }

        private SDKCallInfo _Deserialize(string json)
        {
            return JsonLibUtil.ToObject<SDKCallInfo>(json);
        }
    }
}