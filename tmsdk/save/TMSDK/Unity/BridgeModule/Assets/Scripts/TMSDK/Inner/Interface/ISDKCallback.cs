using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMSDKClient
{
    public interface ISDKCallback
    {
        void Callback(string json);
    }
}
