using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMSDKClient
{
    public interface ISDKEventHander
    {
        event EventHandler<SDKEventArgs> EventHandler;
        void Register(ISDKEventHandle handle);
        void Detach(ISDKEventHandle handle);
        void DetachAll();
        void Invoke(SDKEventArgs e);
    }
}