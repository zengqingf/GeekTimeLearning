using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMSDKClient
{
    public interface ISDKEventHandle
    {
        void OnEvent(object sendse, SDKEventArgs e);
    }
}